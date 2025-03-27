using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherApi.Service;

namespace WeatherApi
{
    public class IngestionFunction
    {
        private readonly ILogger<IngestionFunction> _logger;
        private readonly IWeatherDataService _weatherDataService;
        private readonly IWeatherLogService _weatherLogService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public IngestionFunction(ILogger<IngestionFunction> logger, IWeatherDataService weatherDataService, IWeatherLogService weatherLogService, IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _weatherDataService = weatherDataService;
            _weatherLogService = weatherLogService;
            _dateTimeProvider = dateTimeProvider;
        }

        [Function(nameof(IngestionFunction))]
        [FixedDelayRetry(5, "00:00:10")]
        public void Run([TimerTrigger("0 * * * * *")] TimerInfo timerInfo, FunctionContext context)
        {
            _logger.LogInformation("Timer trigger run at {timeStamp}", _dateTimeProvider.UtcNow);
            //try

            //{
            //    var data = _weatherDataService.FetchWeatherAsync();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError("Unhandled exception in IngestionFunction: {exception}", ex.Message);
            //    _weatherLogService.LogWeatherDataErrorAsync(id, data);

            // call service method for fetching, saving and logging weather data
            // catch exceptions and log errors


        }
    }
}
