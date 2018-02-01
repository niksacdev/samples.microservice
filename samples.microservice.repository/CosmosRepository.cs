using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using samples.microservice.core;
using samples.microservice.entities;
using Microsoft.Extensions.Logging;

namespace samples.microservice.repository
{
    public class CosmosRepository: BaseRepository
    {
        private DocumentClient _client;
        private readonly string _databaseName = "niksac-docdb";
        private readonly string _databaseCollectionName ="niksac-docdb-col1";
        private string _cosmosEndPoint;
        private SecureString _cosmosKey;

        private ILogger logger;

        /// <summary>
        /// Constructor for CosmosRepository
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public CosmosRepository(IConfiguration configuration, ILogger logger) : base(configuration, logger)
        {
            try
            {
                // TODO: Move this away from constructor into OWIN middleware
                // get the cosmos values from configuration
                _cosmosEndPoint = configuration["cosmos-uri"];
                _cosmosKey = new SecureString();
                foreach (var keyChar in configuration["cosmos-key"].ToCharArray())
                {
                    _cosmosKey.AppendChar(keyChar);
                }

                _databaseName = configuration["cosmos-dbName"];
                _databaseCollectionName = configuration["cosmos-db-CollectionName"];
            }
            catch (Exception e)
            {
                logger.LogError($"Error occurred: {e.Message}", e);
                throw;
            }
        }

        /// <summary>
        /// Initialized the database if not already
        /// </summary>
        /// <returns></returns>
        private async Task<bool> InitializeDatabaseAsync()
        {
            try
            {
                // connecting to the Cosmos Document db store
                _client = new DocumentClient(new Uri(_cosmosEndPoint), _cosmosKey)
                {
                    ConnectionPolicy =
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol = Protocol.Tcp,
                    }
                };

                // Create a database if not exists
                await this._client.CreateDatabaseIfNotExistsAsync(new Database {Id = _databaseName});

                // create a collection if not exists
                var response = await this._client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_databaseName),
                    new DocumentCollection {Id = _databaseCollectionName});

                return true;
            }
            catch (Exception e)
            {
                logger.LogError($"Error occurred: {e.Message}", e);
                throw;
            }



        }

        /// <summary>
        /// saves (Upsert) into the database
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="modifiedby"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<bool> SaveAsync<TEntity>(TEntity entity, string modifiedby = null)
        {
            await InitializeDatabaseAsync();
            throw new NotImplementedException();
        }

        /// <summary>
        /// delete an entity from the database
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task DeleteAsync<TEntity>(object id)
        {
            await InitializeDatabaseAsync();
            throw new NotImplementedException();
        }

        /// <summary>
        /// reads the entity based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<TEntity> ReadAsync<TEntity>(object id)
        {
            await InitializeDatabaseAsync();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the entity from the database and returns the top 10 documents
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public override async Task<List<TEntity>> ReadAsync<TEntity>()
        {
            await InitializeDatabaseAsync();
            var entities = new List<TEntity>();
            var queryOptions = new FeedOptions { MaxItemCount = -1 };
            var documentQuery = this._client.CreateDocumentQuery<MyDocument>(UriFactory
                    .CreateDocumentCollectionUri(_databaseName, _databaseCollectionName), queryOptions)
                .Where(f => f.Id == "1");

            foreach (var document in documentQuery)
            {
                entities.Add(document as TEntity);
            }

            return entities;
        }
    }
}