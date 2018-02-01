using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using samples.microservice.core;
using samples.microservice.repository;
using Microsoft.Extensions.Logging;

namespace samples.microservice.api
{
    public class Startup
    {
        ///  <summary>
        ///  </summary>
        ///  <param name="configuration"></param>
        /// <param name="loggerFactory"></param>
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            Logger = loggerFactory.CreateLogger("samples.microservices");
        }

        public IConfiguration Configuration { get; }

        public ILogger Logger { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // add MVC as a service
            services.AddMvc();

            // add the cosmos repository to the configuration
            // services.AddSingleton<IRepository, CosmosRepository>();

            //add the repositories to the configuration
            services.AddRepository<CosmosRepository>(configuration: Configuration, logger: Logger);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory factory)
        {
            // show custom error pages
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                factory.AddConsole();
                factory.AddDebug();
            }
            else
            {
                app.UseExceptionHandler("/error/500");
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            // add support for custom exception handling middleware,
            // this will allow for all exception messages to be handled by a common middleware

            app.UseMiddleware<ErrorHandlingMiddleware>();
            // use MVC framework and map routes to controllers
            app.UseMvc(routes =>
            {
                routes.MapAreaRoute("api_route", "cosmosapi", "api/{Controller}/{Action}/{id?}");
            });
        }
    }
}