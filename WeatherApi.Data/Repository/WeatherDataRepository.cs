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
        Task SaveWeatherData(IEnumerable<WeatherDatum> weatherDatum);
        Task<IEnumerable<WeatherDatum>> GetAllWeatherData();
    }

    public class WeatherDataRepository : IWeatherDataRepository
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
