using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherApi.Exceptions;
using WeatherApi.Service;

namespace WeatherApi.Functions
{
    public class LogEntryFunction
    {
        private readonly ILogger<LogEntryFunction> _logger;
        private readonly IWeatherLogService _logService;

        public LogEntryFunction(ILogger<LogEntryFunction> logger, IWeatherLogService logService)
        {
            _logger = logger;
            _logService = logService;
        }

        [Function(nameof(LogEntryFunction))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "logs")] HttpRequest req, CancellationToken cancellationToken)
        {
            var logs = await _logService.GetWeatherRequestLogsAsync(cancellationToken);
            return new OkObjectResult(logs);
        }
    }
}
