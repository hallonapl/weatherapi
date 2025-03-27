using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApi.Model;

namespace WeatherApi.Service
{
    public interface IWeatherLogService
    {
        Task LogWeatherRequestAsync(Guid id, string name);
        Task<IEnumerable<WeatherRequestLog>> GetWeatherRequestLogsAsync();

    }
    public class WeatherLogService : IWeatherLogService
    {
        public Task<IEnumerable<WeatherRequestLog>> GetWeatherRequestLogsAsync()
        {
            throw new NotImplementedException();
        }

        public Task LogWeatherRequestAsync(Guid id, string name)
        {
            throw new NotImplementedException();
        }
    }
}
