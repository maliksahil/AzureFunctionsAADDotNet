using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Microsoft.Azure.Services.AppAuthentication;
using System.Net.Http;
using System.Net;

using System.Data;
using System.Data.SqlClient;

namespace MyFunctionProj
{
    public static class MyHttpTrigger
    {
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("MyHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                var serviceResourceIDURI = "https://sahilcalledfunction.sahilmalikgmail.onmicrosoft.com";
                var secureFunctionAPIURL = "https://sahilcalledfunction.azurewebsites.net/authenticated";
                var managedIdentityId = "08e84df5-23e7-4c83-8e88-e2b670ec24b8";
                var connectionString = "RunAs=App;AppId=" + managedIdentityId + ";TenantId=dd1790d8-0aaa-403b-8a1c-43e7cca9b589";

                var azureServiceTokenProvider = new AzureServiceTokenProvider(connectionString);
                string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(serviceResourceIDURI);

                // make a post request to the secure service with the access token.
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(secureFunctionAPIURL),
                    Headers = { { HttpRequestHeader.Authorization.ToString(), "Bearer " + accessToken } }
                };
                log.LogInformation(accessToken); // bad bad bad .. but this is demo code, ensure you don't do this in prod.
                var response = await httpClient.SendAsync(httpRequestMessage);
                var result = await response.Content.ReadAsStringAsync();
                log.LogInformation(result);
                return (ActionResult)new OkObjectResult(result);
            }
            catch (Exception e)
            {
                return (ActionResult)
                new OkObjectResult(e.ToString());
            }
        }
    }
}