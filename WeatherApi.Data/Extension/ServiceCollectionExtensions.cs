using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApi.Data.DataAccess;
using WeatherApi.Data.Repository;

namespace WeatherApi.Data.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataServices(this IServiceCollection services, string blobStorageConnectionString)
        {
            services.AddScoped<IWeatherDataRepository, WeatherDataRepository>();
            services.AddSingleton<BlobServiceClientFactory>(services =>
                {
                    //return new BlobServiceClientFactory("UseDevelopmentStorage=true");
                    return new BlobServiceClientFactory(blobStorageConnectionString);
                });
        }
    }
}
