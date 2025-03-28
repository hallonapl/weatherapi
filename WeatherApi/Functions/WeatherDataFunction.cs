using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherApi.Exceptions;
using WeatherApi.Service;

namespace WeatherApi.Functions
{
    public class WeatherDataFunction
    {
        private readonly ILogger<WeatherDataFunction> _logger;
        private readonly IWeatherDataService _weatherDataService;

        public WeatherDataFunction(ILogger<WeatherDataFunction> logger, IWeatherDataService weatherDataService, IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _weatherDataService = weatherDataService;
        }

        [Function(nameof(WeatherDataFunction))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "weather/{id}")] HttpRequest req, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(req.RouteValues["id"] as string, out var id))
            {
                return new BadRequestResult();
            }

            try
            {
                var data = await _weatherDataService.LoadWeatherReportAsync(id, cancellationToken);
                return new OkObjectResult(data);
            }
            catch (BlobNotFoundException ex)
            {
                _logger.LogWarning("Blob not found: {exception}", ex.Message);
                return new NotFoundResult();
            }
            catch (BlobSerializationException ex)
            {
                _logger.LogError("Blob serialization error: {exception}", ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unhandled exception: {exception}", ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
