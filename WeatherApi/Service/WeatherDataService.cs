using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WeatherApi.Data.Repository;
using WeatherApi.Model;

namespace WeatherApi.Service
{
    internal interface IWeatherDataService
    {
        Task SaveWeatherDatum(WeatherDatum weatherDatum);
        Task<IEnumerable<WeatherDatum>> GetAllWeatherData();
    }

    internal class WeatherDataService : IWeatherDataService
    {
        private readonly ILogger<WeatherDataService> _logger;
        private readonly IWeatherDataRepository _weatherDataRepository;

        public WeatherDataService(ILogger<WeatherDataService> logger, IWeatherDataRepository weatherDataRepository)
        {
            _logger = logger;
            _weatherDataRepository = weatherDataRepository;
        }

        public async Task<IEnumerable<WeatherDatum>> GetAllWeatherData()
        {
            var data = await _weatherDataRepository.GetAllWeatherData();
            return data.Select(x => new WeatherDatum(
                x.City,
                x.Country,
                x.Temperature,
                x.FeelsLike,
                x.MinTemperature,
                x.MaxTemperature,
                x.Pressure,
                x.Humidity,
                x.WindSpeed,
                x.WindDegree,
                x.Cloudiness,
                x.UvIndex,
                x.Date));
        }

        public Task SaveWeatherDatum(WeatherDatum weatherDatum)
        {
            var datum = new Data.Model.WeatherDatum(
                weatherDatum.City,
                weatherDatum.Country,
                weatherDatum.Temperature,
                weatherDatum.FeelsLike,
                weatherDatum.MinTemperature,
                weatherDatum.MaxTemperature,
                weatherDatum.Pressure,
                weatherDatum.Humidity,
                weatherDatum.WindSpeed,
                weatherDatum.WindDegree,
                weatherDatum.Cloudiness,
                weatherDatum.UvIndex,
                weatherDatum.Date);
            return _weatherDataRepository.SaveWeatherDatum(datum);
        }
    }

}
