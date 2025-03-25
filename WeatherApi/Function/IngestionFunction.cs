using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherApi.Service;

namespace WeatherApi.Function
{
    public class IngestionFunction
    {
        private readonly ILogger<IngestionFunction> _logger;
        private readonly IWeatherDataService _weatherDataService;

        public IngestionFunction(ILogger<IngestionFunction> logger, IWeatherDataService weatherDataService)
        {
            _logger = logger;
            _weatherDataService = weatherDataService;
        }

        [Function("IngestionFunction")]
        public void Run([TimerTrigger("0 */1 * * * *")] TimerInfo timerInfo, FunctionContext context)
        {
            _logger.LogInformation("C# timer trigger function started at {timeStamp}.", DateTime.UtcNow);
        }
    }
}
