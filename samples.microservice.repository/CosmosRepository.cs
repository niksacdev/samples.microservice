using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using samples.microservice.core;

namespace samples.microservice.repository
{
    public class CosmosRepository : BaseRepository
    {
        private static DocumentClient _client;
        private readonly string _cosmosEndPoint;
        private readonly string _cosmosKey;
        private readonly string _databaseCollectionName;
        private readonly string _databaseName;

        /// <summary>
        ///     Constructor for CosmosRepository
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="loggerFactory"></param>
        public CosmosRepository(IConfiguration configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
            try
            {
                // get the cosmos values from configuration
                _cosmosEndPoint = configuration["cosmos-uri"];
                _cosmosKey = configuration["cosmos-key"];
                _databaseName = configuration["cosmos-dbName"];
                _databaseCollectionName = configuration["cosmos-db-CollectionName"];
            }
            catch (Exception e)
            {
                Logger.LogError($"Error occurred: {e.Message}", e);
                throw;
            }
        }

        /// <summary>
        ///     Initialized the database if not already
        /// </summary>
        /// <returns></returns>
        private async Task<bool> EnsureClientConnected(CancellationToken token)
        {
            try
            {
                // connecting to the Cosmos Document db store
                //TODO: make client singleton and lazy initialize
                if (_client != null) return true;
                _client = new DocumentClient(new Uri(_cosmosEndPoint), _cosmosKey)
                {
                    ConnectionPolicy =
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol =
                            Protocol.Https // TODO: move to TCP, firewall configuration blocking at the moment
                    }
                };

                // ensure client is loaded to optimize performance
                await _client.OpenAsync(token);
                return true;
            }
            catch (DocumentClientException de)
            {
                var baseException = de.GetBaseException();
                Logger.LogError($"Error occurred: {de.Message} Base exception was: {baseException.Message}", de);
                throw;
            }
            catch (Exception e)
            {
                Logger.LogError($"Error occurred: {e.Message}", e);
                throw;
            }
        }

        /// <summary>
        ///     saves (Upsert) into the database
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="modifiedby"></param>
        /// <param name="token"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public override async Task<bool> SaveAsync<TEntity>(TEntity entity, string modifiedby = null,
            CancellationToken token = default(CancellationToken))
        {
            //TODO: Add Polly retry logic to ensure to connect back
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var id = entity.Id.Trim();

            try
            {
                await EnsureClientConnected(token);

                // first check if the records already exists, if it does then update
                var options = new RequestOptions {PartitionKey = new PartitionKey(id)};
                var response = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseName,
                    _databaseCollectionName,
                    id), options);
                Logger.LogInformation("Found {0} during upsert", id);

                // Update the document instead of adding a new one
                await _client.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(_databaseName, _databaseCollectionName, entity.Id),
                    entity);
                Logger.LogInformation("Entity {0} Updated", id);
            }
            catch (DocumentClientException de)
            {
                // if the document does not exist then attempt to insert
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    var options = new RequestOptions {PartitionKey = new PartitionKey(entity.Id)};
                    await _client.CreateDocumentAsync(
                        UriFactory.CreateDocumentCollectionUri(_databaseName, _databaseCollectionName), entity,
                        options);
                    Logger.LogInformation("Created new entity {0}", id);
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        /// <summary>
        ///     delete an entity from the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<bool> DeleteAsync<TEntity>(string id, CancellationToken token)
        {
            try
            {
                await EnsureClientConnected(token);
                var options = new RequestOptions {PartitionKey = new PartitionKey(id.Trim())};
                await _client.DeleteDocumentAsync(
                    UriFactory.CreateDocumentUri(_databaseName, _databaseCollectionName, id.Trim()), options);
                Logger.LogInformation($"Deletion of {id} was successfull.");
                return true;
            }
            catch (Exception e)
            {
                Logger.LogInformation($"Error occurred {e.Message}  details: {e}");
                throw;
            }
        }

        /// <summary>
        ///     reads the entity based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public override async Task<TEntity> ReadSingularAsync<TEntity>(string id,
            CancellationToken token = default(CancellationToken))
        {
            try
            {
                await EnsureClientConnected(token);
                id = id.Trim();
                var queryOptions = new FeedOptions {MaxItemCount = -1, EnableCrossPartitionQuery = true, PartitionKey = new PartitionKey(id)};
                var query = _client
                    .CreateDocumentQuery<TEntity>(UriFactory
                        .CreateDocumentCollectionUri(_databaseName, _databaseCollectionName), queryOptions)
                    //.Where(c => c.Id == id) // filter only on partition key
                    .AsDocumentQuery();

                var response = await query.ExecuteNextAsync<TEntity>(token);
                Logger.LogInformation($"Records returned for singular query {response.Count}");
                return response.FirstOrDefault();
            }
            catch (Exception e)
            {
                Logger.LogInformation($"Error occurred {e.Message}  details: {e}");
                throw;
            }
        }

        /// <summary>
        ///     Reads the entity from the database and returns the top 10 documents
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public override async Task<List<TEntity>> ReadAsync<TEntity>(string partitionKey, int maxItemCount,
            CancellationToken token)
        {
            try
            {
                await EnsureClientConnected(token);
                var entities = new List<TEntity>();
                var queryOptions = new FeedOptions {MaxItemCount = maxItemCount, EnableCrossPartitionQuery = true};
                var documentQuery = _client.CreateDocumentQuery<TEntity>(UriFactory
                    .CreateDocumentCollectionUri(_databaseName, _databaseCollectionName), queryOptions);
                entities.AddRange(documentQuery);
                Logger.LogInformation($"Records returned for multiple query {entities.Count}");
                return entities;
            }
            catch (Exception e)
            {
                Logger.LogInformation($"Error occurred {e.Message}  details: {e}");
                throw;
            }
        }
    }
}