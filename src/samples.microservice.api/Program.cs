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

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    /*** CONVENTIONAL WAY: See below for conventional way of getting secrets from a config file, this requires a configuration file with
                     the following details
                        {
                          "vault": "<your keyvault name here>",
                          "clientId": "<your client Id here>",
                          "clientSecret": "<your client secret here>",
                          "kvuri":"https://{vault-name}.vault.azure.net/"
                        }

                        and then the below code to get the data.

                        // add the configuration file for getting keyvault configuration data
                        config.SetBasePath(Directory.GetCurrentDirectory())
                            .AddEnvironmentVariables()
                            .AddJsonFile($"secrets.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);

                        // add azure key vault configuration
                        var buildConfig = config.Build();

                        // get the key vault  uri
                        var vaultUri = buildConfig["kvuri"].Replace("{vault-name}", buildConfig["vault"]);

                        In this sample we have elimnated the need to get the above file from a secret file and rather use
                        Kubernetes ConfigMaps and Secrets, the main configuration is still kept within KeyVault.
                    ****/

                    // ***** CONTAINER WAY: Instead of getting KV configuration details from a settings file, Kubernetes will push those details as
                    // Environment variables for the container, use the Enviroment variables directly to access information

                    // add the environment variables to config
                    config.AddEnvironmentVariables();

                    // add azure key vault configuration using environment variables
                    var buildConfig = config.Build();

                    // get the key vault  uri
                    var vaultUri = buildConfig["kvuri"].Replace("{vault-name}", buildConfig["vault"]);

                    // setup KeyVault store for getting configuration values
                    config.AddAzureKeyVault(vaultUri, buildConfig["clientId"], buildConfig["clientSecret"]);
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}