using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApi.Model;

namespace WeatherApi.Service
{
    public interface IWeatherDataService
    {
        Task SaveWeatherData(IEnumerable<WeatherDatum> weatherDatum);
        Task<IEnumerable<WeatherDatum>> GetAllWeatherData();
    }

    public class WeatherDataService : IWeatherDataService
    {
        public Task<IEnumerable<WeatherDatum>> GetAllWeatherData()
        {
            throw new NotImplementedException();
        }

        public Task SaveWeatherData(IEnumerable<WeatherDatum> weatherDatum)
        {
            throw new NotImplementedException();
        }
    }

}
