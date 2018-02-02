# The Zero Config Microservice: using Kubernetes and Azure KeyVault

A sample microservice project to demonstrate the following: 
- use of Azure keyvault and Kubernetes ConfigMaps for Configuration, it stresses on the importance of separating DevOps functions from Developer functions by have no appSettings, Secrets, blah blah .json files in code. All data is injected through Kubernetes or asked specifically from Azure Key Vault. 

Other features in the sample:
- use of Serilog for strucutered logging, 
- use of repository for cosmosdb, a generic repository that can be used and extended for CosmosDB Document operations.
- deployment of asp.net core Containers to Kubernetes

Project Structure
- samples.microservice.api - a ASP.NET Core 2.0 MicroService that allows for operations on Vehicle Records
- sample.microservice.core - core interfaces and patterns used in the microservice.
- samples.microservice.entities - a class library to hold custom entities that will be passed as messages to the microservice.
- samples.microservice.repository - the CosmosDB repository that can be used in a generic way to interact with CosmosDB documents. The repository is independent of the micro-service and can be used with minimal changes for any application.


