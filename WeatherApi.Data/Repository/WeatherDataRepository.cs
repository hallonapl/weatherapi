using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherApi.Data.Configuration;
using WeatherApi.Data.DataAccess;
using WeatherApi.Data.Model;

namespace WeatherApi.Data.Repository
{
    public interface IWeatherDataRepository
    {
        Task SaveWeatherDescription(WeatherBlob weatherDatum);
        IAsyncEnumerable<WeatherBlob?> GetAllWeatherData();
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

        //private IEnumerable<WeatherBlob> DummyData => new List<WeatherBlob>
        //{
        //    new WeatherBlob(
        //        new Coord(-0.1257, 51.5085),
        //        new List<Weather> { new Weather(802, "Clouds", "scattered clouds", "03d") },
        //        "stations",
        //        new Main(287.7, 286.96, 287.01, 289.21, 1025, 67, 1025, 1021),
        //        10000,
        //        new Wind(1.54, 0),
        //        new Clouds(30),
        //        1742989819,
        //        new Sys(2, 2091269, "GB", 1742968155, 1743013372),
        //        0,
        //        2643743,
        //        "London",
        //        200
        //    ),
        //    new WeatherBlob(
        //        new Coord(-118.2437, 34.0522),
        //        new List<Weather> { new Weather(800, "Clear", "clear sky", "01d") },
        //        "stations",
        //        new Main(295.0, 294.0, 293.0, 297.0, 1010, 55, 1010, 1005),
        //        10000,
        //        new Wind(3.0, 200),
        //        new Clouds(0),
        //        1742989819,
        //        new Sys(2, 2091269, "US", 1742968155, 1743013372),
        //        -25200,
        //        5368361,
        //        "Los Angeles",
        //        200
        //    ),
        //    new WeatherBlob(
        //        new Coord(139.6917, 35.6895),
        //        new List<Weather> { new Weather(801, "Clouds", "few clouds", "02d") },
        //        "stations",
        //        new Main(303.0, 302.0, 301.0, 305.0, 1008, 65, 1008, 1003),
        //        10000,
        //        new Wind(6.0, 170),
        //        new Clouds(20),
        //        1742989819,
        //        new Sys(2, 2091269, "JP", 1742968155, 1743013372),
        //        32400,
        //        1850147,
        //        "Tokyo",
        //        200
        //    ),
        //    new WeatherBlob(
        //        new Coord(151.2093, -33.8688),
        //        new List<Weather> { new Weather(802, "Clouds", "scattered clouds", "03d") },
        //        "stations",
        //        new Main(293.0, 292.0, 291.0, 295.0, 1013, 50, 1013, 1008),
        //        10000,
        //        new Wind(7.0, 190),
        //        new Clouds(30),
        //        1742989819,
        //        new Sys(2, 2091269, "AU", 1742968155, 1743013372),
        //        36000,
        //        2147714,
        //        "Sydney",
        //        200
        //    ),
        //    new WeatherBlob(
        //        new Coord(-74.0060, 40.7128),
        //        new List<Weather> { new Weather(800, "Clear", "clear sky", "01d") },
        //        "stations",
        //        new Main(298.0, 297.0, 296.0, 300.0, 1012, 60, 1012, 1007),
        //        10000,
        //        new Wind(5.5, 180),
        //        new Clouds(0),
        //        1742989819,
        //        new Sys(2, 2091269, "US", 1742968155, 1743013372),
        //        -14400,
        //        5128581,
        //        "New York",
        //        200
        //    )
        //};

        public async IAsyncEnumerable<WeatherBlob?> GetAllWeatherData()
        {
            var client = _clientFactory.CreateBlobServiceClient();
            var container = client.GetBlobContainerClient(_settings.Value.WeatherDataContainerName);
            var blobPages = container.GetBlobsAsync().AsPages();
            await foreach (var blobPage in blobPages) {
                foreach (var blob in blobPage.Values)
                {
                    var blobClient = container.GetBlobClient(blob.Name);
                    var weatherBlob = await blobClient.DownloadContentAsync();
                    var json = weatherBlob.Value.Content.ToString();
                    var result = JsonSerializer.Deserialize<WeatherBlob>(json);
                    yield return result;
                }
            }
                
        }

        public Task SaveWeatherDescription(WeatherBlob weatherDatum)
        {
            throw new Exception("Fail!");
        }
    }
}
