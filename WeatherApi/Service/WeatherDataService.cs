using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WeatherApi.Client;
using WeatherApi.Model;
using WeatherApi.Repository;

namespace WeatherApi.Service
{
    public interface IWeatherDataService
    {
        Task<Guid> SaveWeatherReportAsync(WeatherPayload payload);
        Task<IEnumerable<WeatherPayload>> LoadAllWeatherDataAsync();
        Task<WeatherPayload> GetWeatherReportAsync();
        Task<WeatherPayload> LoadWeatherReportAsync(Guid id);
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


        public async Task<IEnumerable<WeatherPayload>> LoadAllWeatherDataAsync()
        {
            var data = _weatherDataRepository.GetWeatherDataAsync();
            var result = new List<WeatherPayload>();
            await foreach (var item in data)
            {
                result.Add(item.WeatherPayload);
            }
            return result;
        }


        public async Task<WeatherPayload> LoadWeatherReportAsync(Guid id)
        {
            var data = await _weatherDataRepository.GetWeatherReportBlobAsync(id);
            return data.WeatherPayload;
        }

        public async Task<Guid> SaveWeatherReportAsync(WeatherPayload payload)
        {
            var newId = Guid.NewGuid();
            var blob = new WeatherBlob(
                Id: newId,
                FetchedTimeStamp: _dateTimeProvider.UtcNow,
                WeatherPayload: payload
                );
            await _weatherDataRepository.SaveWeatherPayloadAsync(blob);
            return newId;
        }

        public async Task<WeatherPayload> GetWeatherReportAsync()
        {
            var report = await _weatherClient.GetWeatherReportAsync();
            return report;
        }
    }

}
