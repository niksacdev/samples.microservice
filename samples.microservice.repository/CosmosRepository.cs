using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using samples.microservice.core;
using samples.microservice.entities;

namespace samples.microservice.repository
{
    public class CosmosRepository: IRepository
    {
        private readonly DocumentClient _client;
        private string _databaseName = "niksac-docdb";
        private string _databaseCollectionName ="niksac-docdb-col1";

        public CosmosRepository(IConfiguration configuration)
        {
            try
            {
                // TODO: Move this away from constructor into OWIN middleware
                // get the cosmos values from configuration
                var connectionString = configuration["cosmos-connstr"];
                var databaseName = configuration["cosmos-dbname"];
                var collectionName = configuration["cosmos-db-CollectionName"];
                var databaseEndPoint = configuration["cosmos-uri"];
                var databaseKey = configuration["cosmos-key"];


                // connecting to the Cosmos Document db store
                _client = new DocumentClient(new Uri(databaseEndPoint), databaseKey);

            }
            catch (Exception e)
            {
                //TODO: Add logging
                throw;
            }
        }

        private async Task<bool> InitializeDatabase()
        {

            // Create a database if not exists
            await this._client.CreateDatabaseIfNotExistsAsync(new Database {Id = "niksac-docdb"});

            // create a collection if not exists
            var response = await this._client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_databaseName),
                new DocumentCollection {Id = _databaseCollectionName});

            return true;

        }
        public async Task<bool> SaveAsync<TEntity>(TEntity entity, string modifiedby = null) where TEntity : Entity
        {
            return true;
        }

        public async Task DeleteAsync<TEntity>(object id) where TEntity : Entity
        {
            throw new System.NotImplementedException();
        }

        public async Task<TEntity> ReadAsync<TEntity>(object id) where TEntity : Entity
        {
            throw new System.NotImplementedException();

        }

        public async Task<List<TEntity>> ReadAsync<TEntity>() where TEntity : Entity
        {
            var entities = new List<TEntity>();
            var queryOptions = new FeedOptions { MaxItemCount = -1 };
            var documentQuery = this._client.CreateDocumentQuery<MyDocument>(UriFactory
                    .CreateDocumentCollectionUri(_databaseName, _databaseCollectionName), queryOptions)
                    .Where(f => f.Id == "1");

            if (documentQuery.Any())
            {
                foreach (var document in documentQuery)
                {
                    entities.Add(document as TEntity);
                }
            }

            return entities;
        }
    }
}