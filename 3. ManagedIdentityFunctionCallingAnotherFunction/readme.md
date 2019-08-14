# Register an AAD App

Register an app with the following characteristics,
1. Web App
2. Redirect URI of https://sahilcalledfunction.azurewebsites.net
3. Under expose an API add an AppID URI of https://sahilcalledfunction.sahilmalikgmail.onmicrosoft.com
4. Under Authentication enable id token and resource token under implicit grant

Copy the client id and update Constants.cs under the CalledFunction project
Also update the serviceResourceIDURI under MyHttpTrigger.cs in the CallingFunction Project

# Create a resource group
1. Create a resource group, example `az group create -n sahilfunctionapp --location eastus` .. note the name, you'll use this in the various commands below.

# Deploy the calling function
1. Create a user-assigned managed identity `az group deployment create --resource-group sahilfunctionapp --template-file armtemplates\provisionManagedIdentity.json  --parameters identityName=functionidentity`
2. Create a function app `az group deployment create --resource-group sahilfunctionapp --template-file armtemplates\callingfunction.json --parameters functionAppName=sahilcallingfunction identityName=functionidentity`, it will ask for the name of the function identity, give it the identity name.
3. Deploy the calling function, run this command from the calling function directory `func azure functionapp publish sahilcallingfunction`

# Deploy the called function
1. Create a function app `az group deployment create --resource-group sahilfunctionapp --template-file armtemplates\calledfunction.json --parameters functionAppName=sahilcalledfunction`, it will ask for the name of the function identity, give it the identity name.
2. Deploy the called function, run this command from the called function directory `func azure functionapp publish sahilcalledfunction`

# Overall scripts

```
az login
az group create -n sahilfunctionapp --location eastus
az group deployment create --resource-group sahilfunctionapp --template-file armtemplates/provisionManagedIdentity.json  --parameters identityName=functionidentity
az identity show --resource-group sahilfunctionapp --name functionidentity --query "clientId"
```

Here update the MyHttpTrigger.cs class with the clientId of the managed identity.

```
az group deployment create --resource-group sahilfunctionapp --template-file armtemplates/callingfunction.json --parameters functionAppName=sahilcallingfunction identityName=functionidentity
cd CallingFunction
func azure functionapp publish sahilcallingfunction
cd ..
az group deployment create --resource-group sahilfunctionapp --template-file armtemplates/calledfunction.json --parameters functionAppName=sahilcalledfunction
cd CalledFunction
func azure functionapp publish sahilcalledfunction
cd ..
```
When done `az group delete -n sahilfunctionapp`