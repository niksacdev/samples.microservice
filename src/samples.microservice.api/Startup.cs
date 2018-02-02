using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using samples.microservice.core;
using samples.microservice.repository;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace samples.microservice.api
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="loggerFactory"></param>
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // add MVC as a service
            services.AddMvc();

            // Add SeriLog as provider for logging
            // Configure Serilog
            //TODO: configure logger per environment, this can be done in config as well
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.ColoredConsole().CreateLogger();
            _loggerFactory.AddSerilog(dispose: true);

            //add the repositories to the configuration
            services.AddRepository<CosmosRepository>(Configuration, _loggerFactory);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // show custom error pages
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                _loggerFactory.AddConsole();
                _loggerFactory.AddDebug();
            }
            else
            {
                app.UseExceptionHandler("/error/500");
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            // add support for custom exception handling middleware,
            // this will allow for all exception messages to be handled by a common middleware
            // app.UseMiddleware<ErrorHandlingMiddleware>();

            // use MVC framework and map routes to controllers
            app.UseMvc();
        }
    }
}