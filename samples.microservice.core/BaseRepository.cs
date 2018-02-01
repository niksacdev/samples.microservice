using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Logging;

namespace samples.microservice.core
{
    public abstract class BaseRepository: IRepository
    {
        protected IConfiguration Configuration;
        protected ILogger Logger;

        protected BaseRepository(IConfiguration configuration, ILogger logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public abstract Task<bool> SaveAsync<TEntity>(TEntity entity, string modifiedby = null) where TEntity : Entity;
        public abstract Task DeleteAsync<TEntity>(object id) where TEntity : Entity;
        public abstract Task<TEntity> ReadAsync<TEntity>(object id) where TEntity : Entity;
        public abstract Task<List<TEntity>> ReadAsync<TEntity>() where TEntity : Entity;
    }
}