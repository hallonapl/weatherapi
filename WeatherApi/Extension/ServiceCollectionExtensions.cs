using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Client;
using WeatherApi.DataAccess;
using WeatherApi.Repository;
using WeatherApi.Service;

namespace WeatherApi.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, string storageConnectionString)
        {
            services.AddScoped<IWeatherDataService, WeatherDataService>();
            services.AddScoped<IWeatherDataRepository, WeatherDataRepository>();
            services.AddScoped<IWeatherClient, WeatherClient>();
            services.AddScoped<IWeatherLogService, WeatherLogService>();
            services.AddScoped<IWeatherLogRepository, WeatherLogRepository>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<BlobServiceClientFactory>(services =>
            {
                return new BlobServiceClientFactory(storageConnectionString);
            });
            services.AddSingleton<TableServiceClientFactory>(services =>
            {
                return new TableServiceClientFactory(storageConnectionString);
            });
        }
    }
}
