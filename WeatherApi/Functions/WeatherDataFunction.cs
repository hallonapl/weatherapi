using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace WeatherApi.Functions
{
    public class WeatherDataFunction
    {
        private readonly ILogger<IngestionFunction> _logger;

        public WeatherDataFunction(ILogger<IngestionFunction> logger)
        {
            _logger = logger;
        }

        [Function("WeatherDataFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
