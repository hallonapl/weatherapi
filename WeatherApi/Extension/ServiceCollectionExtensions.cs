using Microsoft.Extensions.DependencyInjection;
using WeatherApi.DataAccess;
using WeatherApi.Repository;
using WeatherApi.Service;

namespace WeatherApi.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, string blobStorageConnectionString)
        {
            services.AddSingleton<IWeatherDataService, WeatherDataService>();
            services.AddScoped<IWeatherDataRepository, WeatherDataRepository>();
            services.AddSingleton<BlobServiceClientFactory>(services =>
            {
                return new BlobServiceClientFactory(blobStorageConnectionString);
            });
        }
    }
}
