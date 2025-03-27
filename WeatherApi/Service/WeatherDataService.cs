using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WeatherApi.Model;
using WeatherApi.Repository;

namespace WeatherApi.Service
{
    public interface IWeatherDataService
    {
        Task<Guid> SaveWeatherReportAsync(WeatherPayload payload);
        Task<IEnumerable<WeatherPayload>> GetAllWeatherDataAsync();
        Task<WeatherPayload> GetWeatherReportAsync(Guid id);
    }

    public class WeatherDataService : IWeatherDataService
    {
        private readonly ILogger<WeatherDataService> _logger;
        private readonly IWeatherDataRepository _weatherDataRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public WeatherDataService(ILogger<WeatherDataService> logger, IWeatherDataRepository weatherDataRepository, IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _weatherDataRepository = weatherDataRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IEnumerable<WeatherPayload>> GetAllWeatherDataAsync()
        {
            var data = _weatherDataRepository.GetWeatherDataAsync();
            var result = new List<WeatherPayload>();
            await foreach (var item in data)
            {
                result.Add(item.WeatherPayload);
            }
            return result;
        }

        public async Task<WeatherPayload> GetWeatherReportAsync(Guid id)
        {
            var data = await _weatherDataRepository.GetWeatherReportBlobAsync(id);
            return data.WeatherPayload;
        }

        public async Task<Guid> SaveWeatherReportAsync(WeatherPayload payload)
        {
            //Create weather api client
            //Get data from weather api client
            //Log success or failure uisng weather log service
            //Save data to repository

            var newId = Guid.NewGuid();
            var blob = new WeatherBlob(
                Id: newId,
                FetchedTimeStamp: _dateTimeProvider.UtcNow,
                WeatherPayload: payload
                );
            await _weatherDataRepository.SaveWeatherPayloadAsync(blob);
            return newId;
        }
    }

}
