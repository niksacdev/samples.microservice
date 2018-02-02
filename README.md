# The Zero Config Microservice: using Kubernetes and Azure KeyVault

### A sample microservice project to demonstrate the following: 
- use of Azure keyvault and Kubernetes ConfigMaps for Configuration, it stresses on the importance of separating DevOps functions from Developer functions by have no appSettings, Secrets, blah blah .json files in code. All data is injected through Kubernetes or asked specifically from Azure Key Vault. 

### Other features in the sample:
- use of Serilog for strucutered logging, 
- use of repository for cosmosdb, a generic repository that can be used and extended for CosmosDB Document operations.
- deployment of asp.net core Containers to Kubernetes

### Project Structure
- samples.microservice.api - a ASP.NET Core 2.0 MicroService that allows for operations on Vehicle Records
- sample.microservice.core - core interfaces and patterns used in the microservice.
- samples.microservice.entities - a class library to hold custom entities that will be passed as messages to the microservice.
- samples.microservice.repository - the CosmosDB repository that can be used in a generic way to interact with CosmosDB documents. The repository is independent of the micro-service and can be used with minimal changes for any application.

### What do you need to run the sample:
All the configuration files for ASP.NET Core have been moved out so you will not see any appSettings.json rather you need to configure Kubernetes and Azure KeyVault with the right configurations:
- Configure Azure KeyVault and add configuration values to it, here is a sample to do it: https://medium.com/@patoncrispy/securing-settings-in-dot-net-core-apps-using-azure-key-vault-d4ec82de00b8. DONOT put the configuration in the appSettings file as mentioned in the blog post, we will instead use Kubernetes config Maps for that. You need the following values in your KeyVault for the service to work, you can add additional values and pull them in the application as well:

````
Azure KeyVault Key-Pairs required for Cosmos DB repository:
1. cosmos-connstr - database connection string
2. cosmos-db-CollectionName - cosmos collection name
3. cosmos-dbname - name of the database for the Document API
4. cosmos-key - your primary key
5. cosmos-uri - e.g. https://<yourcosmos>.documents.azure.com:443/
````
If you dont have a CosmosDB database, you can create one using the steps here: https://docs.microsoft.com/en-us/azure/cosmos-db/. You dont need to create the API using this tutorial just the Azure portal database account steps.

- Configure Kubernetes with ConfigMaps and Secrets
Instead of putting your Azure Key Vault details in a appSettings or secrets.json file, we will use ConfigMaps and Secrets to automatically inject these values during pod deployment. This is where you get clear separation between DevOps teams who manage the config data and Development teams who are only interested in the key to fetch the value. 
You can use kubectl to create ConfigMaps and Secrets, you will need the below configurations:
````
kubectl create configmap vault --from-literal=vault=<<your keyvault name here>>   
kubectl create configmap kvuri --from-literal=kvuri=https://{vault-name}.vault.azure.net/ #DONT change this, the value gets injected automatically for this config.
kubectl create configmap clientid --from-literal=clientId=<<your client Id here>>  
kubectl create secret generic clientsecret --from-literal=clientSecret=<<your client secret here>>
````
- Configure your docker hub image name and registry settings:
Once you have the config settings in, you need to push the container in your docker hub or private registry. 

````
docker push <<your registry name>/<<your container name>>:<<your version tag>  
````

- Next, provide the docker container details to the vehicle-api-deployment.yaml  

````
containers:
      - ....
        image: <yourdockerhub>/<youdockerimage>:<youdockertagversion> #CHANGE THIS: to the tag for your container
````

- Finally, deploy the container in your kubernetes cluster and you have a ready to use Vehicle Microservice that can do all basic CRUD operations. Enjoy!!

````
kubectl create -f vehicle-api-deployment.yaml 
kubectl create -f vehicle-api-service.yaml  

````

*Note that in my Dockerfile I am using a private registry secret alias, if you want to deploy in a private registry you will have to provide a docker-secret to kubernetes. If you dont need and want to try a public docker registry instead, simply comment the following in the vehicle-api-deployment.yaml

````
imagePullSecrets: #secret to get details of private repo, disable this if using public docker repo
      - name: regsecret
````
