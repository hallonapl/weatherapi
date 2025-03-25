using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherApi.Configuration;
using WeatherApi.DataAccess;
using WeatherApi.Exceptions;
using WeatherApi.Model.Persistence;

namespace WeatherApi.Repository
{
    public interface IWeatherLogRepository
    {
        Task AddLogEntryAsync(WeatherLogEntry weatherLogEntry, CancellationToken cancellationToken);
        IAsyncEnumerable<WeatherLogEntry> GetLogEntriesAsync(CancellationToken cancellationToken);
    }

    public class WeatherLogRepository : IWeatherLogRepository
    {
        private readonly ILogger<WeatherLogRepository> _logger;
        private readonly TableServiceClientFactory _clientFactory;
        private readonly IOptions<StorageSettings> _options;

        public WeatherLogRepository(ILogger<WeatherLogRepository> logger, TableServiceClientFactory clientFactory, IOptions<StorageSettings> options)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _options = options;
        }
        public async Task AddLogEntryAsync(WeatherLogEntry weatherLogEntry, CancellationToken cancellationToken)
        {
            var client = _clientFactory.GetTableServiceClient();
            var table = client.GetTableClient(_options.Value.LogTableName ?? throw new Exception("Log table name configuration missing."));

            // Add entity to the table
            await table.AddEntityAsync(weatherLogEntry, cancellationToken);
        }

        public async IAsyncEnumerable<WeatherLogEntry> GetLogEntriesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var client = _clientFactory.GetTableServiceClient();
            var table = client.GetTableClient(_options.Value.LogTableName ?? throw new Exception("Log table name configuration missing."));

            var entityPages = table.QueryAsync<WeatherLogEntry>(cancellationToken: cancellationToken).AsPages();

            await foreach (var entityPage in entityPages)
            {
                foreach (var entity in entityPage.Values)
                {
                    yield return entity;
                }
            }
        }
    }
}
