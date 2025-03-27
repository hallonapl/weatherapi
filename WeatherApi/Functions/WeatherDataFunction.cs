using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherApi.Service;

namespace WeatherApi.Functions
{
    public class WeatherDataFunction
    {
        private readonly ILogger<WeatherDataFunction> _logger;
        private readonly IWeatherDataService _weatherDataService;

        public WeatherDataFunction(ILogger<WeatherDataFunction> logger, IWeatherDataService weatherDataService)
        {
            _logger = logger;
            _weatherDataService = weatherDataService;
        }

        [Function("WeatherDataFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            try
            {

            }
            catch (Exception ex)
            {
                return new OkObjectResult("Welcome to Azure Functions!");
                _logger.LogError("Unhandled exception in WeatherDataFunction {exception}", ex.Message);
                throw;
            }
            var data = _weatherDataService.GetAnyWeatherDatum();
        }
    }
}
