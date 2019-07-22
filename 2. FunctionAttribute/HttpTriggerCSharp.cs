using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotNetAuthorizeFunction
{
    public class HttpTriggerCSharp
    {
        [Authorize]
        [FunctionName(nameof(Hello))]
        public async Task<IActionResult> Hello(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {

            log.LogInformation("C# HTTP TRIGGER HANDLER INVOKED");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}. You're authenticated!")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
