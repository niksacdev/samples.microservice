using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using samples.microservice.core;
using samples.microservice.repository;

namespace samples.microservice.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // add MVC as a service
            services.AddMvc();

            // add the repository to the configuration
            services.AddSingleton<IRepository, CosmosRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // show custom error pages
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/500");
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            // add support for custom exception handling middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();

            // use MVC framework and map routes to controllers
            app.UseMvc(routes =>
            {
                routes.MapAreaRoute("api_route", "cosmosapi", "api/{Controller}/{Action}/{id?}");
            });
        }
    }
}