using System.Collections.Generic;
using System.Threading.Tasks;

namespace samples.microservice.core
{
    public interface IRepository
    {
        /// <summary>
        /// Upsert to create or update an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="modifiedby"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        Task<bool> SaveAsync<TEntity>(TEntity entity, string modifiedby = null) where TEntity : Entity;

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        Task DeleteAsync<TEntity>(object id) where TEntity : Entity;

        /// <summary>
        /// Read all or specific records from the data store
        /// </summary>
        /// <param name="id">i </param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        Task<TEntity> ReadSingularAsync<TEntity>(object id) where TEntity : Entity;

        Task<List<TEntity>> ReadAsync<TEntity>(string partitionKey = null, int maxItemCount = 10) where TEntity : Entity;

    }
}