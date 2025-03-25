using Microsoft.Extensions.Logging;
using WeatherApi.Client;
using WeatherApi.Model;
using WeatherApi.Model.Persistence;
using WeatherApi.Repository;

namespace WeatherApi.Service
{
    public interface IWeatherDataService
    {
        Task<Guid> SaveWeatherReportAsync(WeatherPayload payload, CancellationToken cancellationToken);
        Task<WeatherPayload> GetWeatherReportAsync(CancellationToken cancellationToken);
        Task<WeatherPayload> LoadWeatherReportAsync(Guid id, CancellationToken cancellationToken);
    }

    public class WeatherDataService : IWeatherDataService
    {
        private readonly ILogger<WeatherDataService> _logger;
        private readonly IWeatherDataRepository _weatherDataRepository;
        private readonly IWeatherClient _weatherClient;
        private readonly IDateTimeProvider _dateTimeProvider;

        public WeatherDataService(ILogger<WeatherDataService> logger, IWeatherDataRepository weatherDataRepository, IWeatherClient weatherClient, IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _weatherDataRepository = weatherDataRepository;
            _weatherClient = weatherClient;
            _dateTimeProvider = dateTimeProvider;
        }


        public async Task<WeatherPayload> LoadWeatherReportAsync(Guid id, CancellationToken cancellationToken)
        {
            var data = await _weatherDataRepository.GetWeatherBlobByIdAsync(id, cancellationToken);
            return data.WeatherPayload;
        }

        public async Task<Guid> SaveWeatherReportAsync(WeatherPayload payload, CancellationToken cancellationToken)
        {
            var newId = Guid.NewGuid();
            var blob = new WeatherBlob(
                Id: newId,
                FetchedTimeStamp: _dateTimeProvider.UtcNow,
                WeatherPayload: payload
                );
            await _weatherDataRepository.SaveWeatherBlobAsync(blob, cancellationToken);
            return newId;
        }

        public async Task<WeatherPayload> GetWeatherReportAsync(CancellationToken cancellationToken)
        {
            var report = await _weatherClient.GetWeatherReportAsync(cancellationToken);
            return report;
        }
    }

}
