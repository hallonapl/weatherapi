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
        Task SaveWeatherPayload(WeatherPayload payload);
        Task<IEnumerable<WeatherPayload>> GetAllWeatherData();
    }

    public class WeatherDataService : IWeatherDataService
    {
        private readonly ILogger<WeatherDataService> _logger;
        private readonly IWeatherDataRepository _weatherDataRepository;

        public WeatherDataService(ILogger<WeatherDataService> logger, IWeatherDataRepository weatherDataRepository)
        {
            _logger = logger;
            _weatherDataRepository = weatherDataRepository;
        }

        public async Task<IEnumerable<WeatherPayload>> GetAllWeatherData()
        {
            var data = _weatherDataRepository.GetAllWeatherData();
            var result = new List<WeatherPayload>();
            await foreach (var item in data)
            {
                if (item == null)
                {
                    throw new Exception("Null value is not valid.");
                }
                result.Add(item.WeatherPayload);
            }
            return result;

            //var result = new List<WeatherResponse>();
            //await foreach (var item in data)
            //{
            //    if (item == null)
            //    {
            //        throw new Exception("Null value is not valid.");
            //    }
            //    result.Add(new WeatherResponse(
            //        City: item.WeatherDescription.Name,
            //        Country: item.WeatherDescription.Sys.Country,
            //        Temperature:item.WeatherDescription.Main.Temp,
            //        FeelsLike:item.WeatherDescription.Main.FeelsLike,
            //        MinTemperature: item.WeatherDescription.Main.TempMin,
            //        MaxTemperature: item.WeatherDescription.Main.TempMax,
            //        Pressure: item.WeatherDescription.Main.Pressure,
            //        Humidity: item.WeatherDescription.Main.Humidity,
            //        WindSpeed: item.WeatherDescription.Wind.Speed,
            //        WindDegree: item.WeatherDescription.Wind.Deg,
            //        Cloudiness: item.WeatherDescription.Clouds.All,
            //        Visibility: item.WeatherDescription.Visibility,
            //        TimeStamp: item.TimeStamp
            //    ));
            //}
            //return result;
        }

        public Task SaveWeatherPayload(WeatherPayload payload)
        {
            var blob = new WeatherBlob(
                Id: Guid.NewGuid(),
                FetchedTimeStamp: DateTime.UtcNow,
                WeatherPayload: payload
                );
            return _weatherDataRepository.SaveWeatherPayload(blob);
        }
    }

}
