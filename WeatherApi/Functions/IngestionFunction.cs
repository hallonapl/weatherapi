using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherApi.Exceptions;
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
        [FixedDelayRetry(2, "00:00:10")]
        public async Task Run([TimerTrigger("0 * * * * *", RunOnStartup = true)] TimerInfo timerInfo, FunctionContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timer trigger run at {timeStamp}", _dateTimeProvider.UtcNow);
            try
            {
                var data = await _weatherDataService.GetWeatherReportAsync(cancellationToken);
                var id = await _weatherDataService.SaveWeatherReportAsync(data, cancellationToken);
                await _weatherLogService.LogWeatherRequestAsync(id, "some description", cancellationToken);
            }
            catch (WeatherClientFetchException ex)
            {
                _logger.LogInformation("Failed to fetch weather data: {exception}", ex.Message);
                await _weatherLogService.LogWeatherRequestFailureAsync("Failed to get weather data.", cancellationToken);
            }
            catch (BlobNotSavedException ex)
            {
                _logger.LogInformation("Failed to fetch weather data: {exception}", ex.Message);
                await _weatherLogService.LogWeatherRequestFailureAsync("Failed to save weather data.", cancellationToken);
            }
        }
    }
}
