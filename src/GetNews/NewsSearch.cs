using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.CognitiveServices.Search.WebSearch;

namespace GetNews
{
    public static class NewsSearch
    {
        private static WebSearchClient client;

        [FunctionName("GetNews")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] string req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            client = new WebSearchClient(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("CognitiveApiKey")))
            {
                Endpoint = Environment.GetEnvironmentVariable("CognitiveEndpoint")
            };

            var request = JsonConvert.DeserializeObject<request>(req);

            if (request.countryCode == null)
            {
                request.countryCode = "de-de";
            }

            if (request.language == null)
            {
                request.language = "de";
            }

            var webData = await client.Web.SearchAsync(request.query, acceptLanguage: request.language, countryCode: request.countryCode, safeSearch: "Strict", setLang: request.language, market: request.countryCode);

            return new OkObjectResult(webData);
        }
    }
}
