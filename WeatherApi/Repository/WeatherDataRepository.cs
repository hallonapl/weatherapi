using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherApi.Configuration;
using WeatherApi.DataAccess;
using WeatherApi.Model;

namespace WeatherApi.Repository
{
    public interface IWeatherDataRepository
    {
        Task SaveWeatherPayload(WeatherBlob weatherDatum);
        IAsyncEnumerable<WeatherBlob> GetAllWeatherData();
        Task<WeatherBlob> GetAnyWeatherDatum();
    }

    public class WeatherDataRepository : IWeatherDataRepository
    {
        private readonly ILogger<WeatherDataRepository> _logger;
        private readonly IOptions<StorageSettings> _settings;
        private readonly BlobServiceClientFactory _clientFactory;

        public WeatherDataRepository(ILogger<WeatherDataRepository> logger, IOptions<StorageSettings> settings, BlobServiceClientFactory clientFactory)
        {
            _logger = logger;
            _settings = settings;
            _clientFactory = clientFactory;
        }

        public async IAsyncEnumerable<WeatherBlob> GetAllWeatherData()
        {
            var client = _clientFactory.CreateBlobServiceClient();
            var container = client.GetBlobContainerClient(_settings.Value.WeatherDataContainerName);
            var blobPages = container.GetBlobsAsync().AsPages();
            await foreach (var blobPage in blobPages)
            {
                foreach (var blob in blobPage.Values)
                {
                    var blobClient = container.GetBlobClient(blob.Name);
                    var weatherBlob = await blobClient.DownloadContentAsync();
                    var json = weatherBlob.Value.Content.ToString();
                    var result = JsonSerializer.Deserialize<WeatherBlob>(json);
                    if (result == null)
                    {
                        throw new Exception("Cannot deserialize blob");
                    }
                    yield return result!;
                }
            }
        }

        public async Task<WeatherBlob> GetAnyWeatherDatum()
        {
            var client = _clientFactory.CreateBlobServiceClient();
            var container = client.GetBlobContainerClient(_settings.Value.WeatherDataContainerName);
            var blob = container.GetBlobs().First();
            var blobClient = container.GetBlobClient(blob.Name);
            var weatherBlob = await blobClient.DownloadContentAsync();
            var json = weatherBlob.Value.Content.ToString();
            var result = JsonSerializer.Deserialize<WeatherBlob>(json);
            if (result == null)
            {
                throw new Exception("Cannot deserialize blob");
            }
            return result!;
        }

        public Task SaveWeatherPayload(WeatherBlob blob)
        {
            throw new Exception("Fail!");
        }
    }
}
