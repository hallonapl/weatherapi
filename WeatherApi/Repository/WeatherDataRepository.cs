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
using WeatherApi.Exceptions;
using WeatherApi.Model;

namespace WeatherApi.Repository
{
    public interface IWeatherDataRepository
    {
        Task SaveWeatherPayloadAsync(WeatherBlob weatherDatum);
        IAsyncEnumerable<WeatherBlob> GetWeatherDataAsync();
        Task<WeatherBlob> GetWeatherReportBlobAsync(Guid id);
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

        public async IAsyncEnumerable<WeatherBlob> GetWeatherDataAsync()
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
                        throw new BlobSerializationException($"Cannot deserialize blob with name {blob.Name}");
                    }
                    yield return result!;
                }
            }
        }

        public async Task<WeatherBlob> GetWeatherReportBlobAsync(Guid id)
        {
            var client = _clientFactory.CreateBlobServiceClient();
            var container = client.GetBlobContainerClient(_settings.Value.WeatherDataContainerName);
            var blob = container.FindBlobsByTags($"id={id}").FirstOrDefault();
            if (blob is null)
            {
                throw new BlobNotFoundException($"No blob with id {id} found in storage");
            }
            var blobClient = container.GetBlobClient(blob.BlobName);
            var weatherBlob = await blobClient.DownloadContentAsync();
            var json = weatherBlob.Value.Content.ToString();
            var result = JsonSerializer.Deserialize<WeatherBlob>(json, SerializerOptions.PayloadSerializerOptions);
            if (result == null)
            {
                throw new BlobSerializationException($"Cannot deserialize blob with name {blob.BlobName}, id {id}");
            }
            return result!;
        }

        public async Task SaveWeatherPayloadAsync(WeatherBlob blob)
        {
            try
            {
                var client = _clientFactory.CreateBlobServiceClient();
                var container = client.GetBlobContainerClient(_settings.Value.WeatherDataContainerName);
                using var blobStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(blob, SerializerOptions.PayloadSerializerOptions)));
                await container.UploadBlobAsync(blob.Id.ToString(), blobStream);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save weather payload: {exception}", ex.Message);
                throw new BlobNotSavedException("Failed to save weather blob");
            }
        }
    }
}
