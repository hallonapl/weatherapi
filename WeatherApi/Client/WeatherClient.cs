using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherApi.Configuration;
using WeatherApi.Exceptions;
using WeatherApi.Model;

namespace WeatherApi.Client
{
    public interface IWeatherClient
    {
        public Task<WeatherPayload> GetWeatherReportAsync(CancellationToken cancellationToken);
    }

    public class WeatherClient : IWeatherClient
    {
        private readonly ILogger<WeatherClient> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<WeatherClientSettings> _options;

        public WeatherClient(ILogger<WeatherClient> logger, IHttpClientFactory httpClientFactory, IOptions<WeatherClientSettings> options)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = options;
        }

        public async Task<WeatherPayload> GetWeatherReportAsync(CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient(nameof(WeatherClient));
            var response = await client.GetAsync($"?q=London&appid={_options.Value.ApiKey}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get weather report");
                throw new WeatherClientFetchException("Failed to get weather report");
            }
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<WeatherPayload>(content, SerializerOptions.PayloadSerializerOptions);
            if (result is null)
            {
                throw new WeatherClientFetchException("Failed to deserialize weather report");
            }
            return result;
        }
    }
}