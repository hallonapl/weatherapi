using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApi.Data.Model;

namespace WeatherApi.Data.Repository
{
    public interface IWeatherDataRepository
    {
        Task SaveWeatherDatum(WeatherDatum weatherDatum);
        Task<IEnumerable<WeatherDatum>> GetAllWeatherData();
    }

    public class WeatherDataRepository : IWeatherDataRepository
    {
        private IEnumerable<WeatherDatum> DummyData => new List<WeatherDatum>
            {
                new WeatherDatum("New York", "USA", 22.5, 21.0, 20.0, 25.0, 1012, 60, 5.5, 180, 20, 5, DateTime.Now),
                new WeatherDatum("Los Angeles", "USA", 25.0, 24.0, 22.0, 28.0, 1010, 55, 3.0, 200, 10, 7, DateTime.Now),
                new WeatherDatum("London", "UK", 18.0, 17.0, 15.0, 20.0, 1015, 70, 4.0, 150, 80, 3, DateTime.Now),
                new WeatherDatum("Tokyo", "Japan", 30.0, 29.0, 28.0, 32.0, 1008, 65, 6.0, 170, 50, 9, DateTime.Now),
                new WeatherDatum("Sydney", "Australia", 20.0, 19.0, 18.0, 23.0, 1013, 50, 7.0, 190, 30, 6, DateTime.Now)
            };

        public Task<IEnumerable<WeatherDatum>> GetAllWeatherData()
        {
            return Task.FromResult(DummyData);
        }

        public Task SaveWeatherDatum(WeatherDatum weatherDatum)
        {
            throw new Exception("Fail!");
        }
    }
}
