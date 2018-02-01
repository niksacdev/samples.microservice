using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace samples.microservice.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // ensure settings are being read
                    var env = context.HostingEnvironment;
                    config.AddEnvironmentVariables();
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true,
                        reloadOnChange: true);

                    // adding logging configuration to enable serilog
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .WriteTo.ColoredConsole().CreateLogger();


                    // add keyvault configuration
                    config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddEnvironmentVariables()
                        .AddJsonFile($"secrets.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);

                    // add azure key vault configuration
                    var buildConfig = config.Build();

                    // get the key vault  uri
                    var vaultUri = buildConfig["kvuri"].Replace("{vault-name}", buildConfig["vault"]);

                    // setup KeyVault store for getting configuration values
                    config.AddAzureKeyVault(vaultUri, buildConfig["clientId"], buildConfig["clientSecret"]);

                })
                .ConfigureLogging((context, logging) =>
                {
                    // configure serilog as the middleware
                    logging.AddSerilog(dispose: true);
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseStartup<Startup>()
                .Build();
    }
}