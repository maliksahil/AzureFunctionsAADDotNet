# This repo contains the various identity related scenarios with Azure functions in DotNet

## You can find 
1. DotNet and Azure Functions examples at https://github.com/maliksahil/AzureFunctionsAADDotNet
2. NodeJS and Azure Functions examples at https://github.com/maliksahil/AzureFunctionsAADNodeJS
3. Python and Azure Functions examples at https://github.com/maliksahil/AzureFunctionsAADPython

You will find two examples in this repo
1. Function - which shows how to do bearer token validation using MSAL
2. FunctionAttribute - same but gives you an AuthorizeAttribute, currently depends on some obsolete APIs.

## Prerequisites
1. You must have Visual Studio Code installed
2. You must have Azure Functions core tools installed `npm install -g azure-functions-core-tools`
3. Azure functions VSCode extension (https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)
4. Your local dev environment must be setup to use dotnetcore 2.2

## Register an AAD App

The Azure function acts as a WebAPI. 

Register a new app, with the following redirect URIs,
For local testing - 
```
  http://localhost:7071/callback
  http://localhost:7071
```

For testing in Azure (this must match your Azure functions URL which you will create later in this tutorial) - 
```
  https://<yournodejsfunction>.azurewebsites.net/callback
  https://<yournodejsfunction>.azurewebsites.net
```
Note down it's application id. I will refer to this application id as `client_id_api` for the purposes of this document.

Also choose to generate a client secret - save it somewhere, you'll need it soon.

Also save the app id URI of this app.

Update all these values in the Constants.cs class in your project.

 ## Test your function - locally

 1. With the project open in VSCode, just hit F5, or you can also run `func host start` from the CLI.
 2. You will need an access token to call this function. In order to get the access token, open browser in private mode and visit
 ```
 https://login.microsoftonline.com/<tenant_name>.onmicrosoft.com/oauth2/v2.0/authorize?response_type=code&client_id=<client_id_api>&redirect_uri=https://localhost:7071/callback&scope=openid
```

This will prompt you to perform authentication, and it will return a code. 
Use that code in the following request to get an access token, remember to put in the code and client secret.

```
curl -X POST \
  https://login.microsoftonline.com/<tenant_name>.onmicrosoft.com/oauth2/v2.0/token \
  -H 'Accept: */*' \
  -H 'Cache-Control: no-cache' \
  -H 'Connection: keep-alive' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -H 'Host: login.microsoftonline.com' \
  -H 'accept-encoding: gzip, deflate' \
  -H 'cache-control: no-cache' \
  -d 'redirect_uri=https%3A%2F%2Flocalhost:7071%2Fcallback&client_id=<client_id_api>&grant_type=authorization_code&code=<put code here>&client_secret=<put client secret here>&scope=https%3A%2F%2Fmytestapp.<tenant_name>.onmicrosoft.com%2F.default'
  ```
 
 3. Once you get the access token, make a GET request to `http://localhost:7071/Hello` for the FunctionAttribute project `http://localhost:7071/Authenticated` and  with the access token as a Authorization Bearer header. Verify that the output you receive includes the user's name. Modify the access token slightly to make it invalid, Verify that you get HTTP 401 in the Function project, and HTTP 500 in the FunctionAttribute project.

 ## Test your function - in Azure

1. Choose to deploy the function
2. You will need an access token to call this function. In order to get the access token, open browser in private mode and visit
 ```
 https://login.microsoftonline.com/<tenant_name>.onmicrosoft.com/oauth2/v2.0/authorize?response_type=code&client_id=<client_id_api>&redirect_uri=https://<yournodejsfunction>.azurewebsites.net/callback&scope=openid
```

This will prompt you to perform authentication, and it will return a code. 
Use that code in the following request to get an access token, remember to put in the code and client secret.

```
curl -X POST \
  https://login.microsoftonline.com/<tenant_name>.onmicrosoft.com/oauth2/v2.0/token \
  -H 'Accept: */*' \
  -H 'Cache-Control: no-cache' \
  -H 'Connection: keep-alive' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -H 'Host: login.microsoftonline.com' \
  -H 'accept-encoding: gzip, deflate' \
  -H 'cache-control: no-cache' \
  -d 'redirect_uri=https%3A%2F%2F<yournodejsfunction>.azurewebsites.net%2Fcallback&client_id=<client_id_api>&grant_type=authorization_code&code=<put code here>&client_secret=<put client secret here>&scope=https%3A%2F%2Fmytestapp.<tenant_name>.onmicrosoft.com%2F.default'
  ```
 
 3. Once you get the access token, make a GET request to `https://<yournodejsfunction>.azurewebsites.net/Hello` for the FunctionAttribute project and `https://<yournodejsfunction>.azurewebsites.net/Authenticated` for the function project with the access token as a Authorization Bearer header.Verify that the output you receive includes the user's name. Modify the access token slightly to make it invalid, Verify that you get HTTP 401 in the Function project, and HTTP 500 in the FunctionAttribute project.