using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace samples.microservice.core
{
    /// <summary>
    ///     Template pattern for repository building
    /// </summary>
    public abstract class BaseRepository : IRepository
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;

        protected BaseRepository(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            Logger = loggerFactory.CreateLogger<BaseRepository>();
        }

        public abstract Task<bool> SaveAsync<TEntity>(TEntity entity, string modifiedby = null,
            CancellationToken token = default(CancellationToken)) where TEntity : Entity;

        public abstract Task<bool> DeleteAsync<TEntity>(string id, CancellationToken token) where TEntity : Entity;

        public abstract Task<TEntity> ReadSingularAsync<TEntity>(string id,
            CancellationToken token = default(CancellationToken)) where TEntity : Entity;

        public abstract Task<List<TEntity>> ReadAsync<TEntity>(string partitionKey, int maxItemCount,
            CancellationToken token) where TEntity : Entity;
    }
}