using Azure.Storage.Blobs.Models;
using Google.Protobuf.WellKnownTypes;
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
using WeatherApi.Model.Persistence;

namespace WeatherApi.Repository
{
    public interface IWeatherDataRepository
    {
        Task SaveWeatherBlobAsync(WeatherBlob weatherBlob, CancellationToken cancellationToken);
        Task<WeatherBlob> GetWeatherBlobByIdAsync(Guid id, CancellationToken cancellationToken);
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

        public async Task<WeatherBlob> GetWeatherBlobByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var client = _clientFactory.GetBlobServiceClient();
            var container = client.GetBlobContainerClient(_settings.Value.WeatherDataContainerName ?? throw new Exception("Blob container name configuration missing."));
            var blob = container.FindBlobsByTags($"id='{id.ToString()}'", cancellationToken).FirstOrDefault();
            if (blob is null)
            {
                throw new BlobNotFoundException($"No blob with id {id} found in storage");
            }
            var blobClient = container.GetBlobClient(blob.BlobName);
            var weatherBlob = await blobClient.DownloadContentAsync(cancellationToken);
            var json = weatherBlob.Value.Content.ToString();
            var result = JsonSerializer.Deserialize<WeatherBlob>(json, SerializerOptions.PayloadSerializerOptions);
            if (result == null)
            {
                throw new BlobSerializationException($"Cannot deserialize blob with name {blob.BlobName}, id {id}");
            }
            return result!;
        }

        public async Task SaveWeatherBlobAsync(WeatherBlob blob, CancellationToken cancellationToken)
        {
            try
            {
                var client = _clientFactory.GetBlobServiceClient();
                var container = client.GetBlobContainerClient(_settings.Value.WeatherDataContainerName ?? throw new Exception("Blob container name configuration missing."));
                var blobClient = container.GetBlobClient(blob.Id.ToString());
                using var blobStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(blob, SerializerOptions.PayloadSerializerOptions)));
                await blobClient.UploadAsync(blobStream, new BlobUploadOptions() 
                {
                    Tags = new Dictionary<string, string> { { "id", blob.Id.ToString() } }
                },
                cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save weather payload: {exception}", ex.Message);
                throw new BlobNotSavedException("Failed to save weather blob");
            }
        }
    }
}
