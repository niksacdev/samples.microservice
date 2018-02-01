using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace samples.microservice.core
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepository<T>(this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger) where T : BaseRepository
        {
            return services.AddSingleton<IRepository, T>(_ =>
                Activator.CreateInstance(typeof(T), configuration, logger) as T);
        }
    }
}