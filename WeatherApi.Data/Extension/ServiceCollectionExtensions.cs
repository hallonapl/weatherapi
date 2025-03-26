using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApi.Data.Repository;

namespace WeatherApi.Data.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataServices(this IServiceCollection services)
        {
            services.AddSingleton<IWeatherDataRepository, WeatherDataRepository>();
        }
    }
}
