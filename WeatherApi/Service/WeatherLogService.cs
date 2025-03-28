using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApi.Model;
using WeatherApi.Model.Persistence;
using WeatherApi.Repository;

namespace WeatherApi.Service
{
    public interface IWeatherLogService
    {
        Task LogWeatherRequestAsync(Guid id, string description, CancellationToken cancellationToken);
        Task LogWeatherRequestFailureAsync(string description, CancellationToken cancellationToken);
        Task<IEnumerable<WeatherRequestLog>> GetWeatherRequestLogsAsync(CancellationToken cancellationToken);

    }
    public class WeatherLogService : IWeatherLogService
    {
        private readonly ILogger<WeatherLogService> _logger;
        private readonly IWeatherLogRepository _logRepository;

        public WeatherLogService(ILogger<WeatherLogService> logger, IWeatherLogRepository logRepository)
        {
            _logger = logger;
            _logRepository = logRepository;
        }

        public async Task<IEnumerable<WeatherRequestLog>> GetWeatherRequestLogsAsync(CancellationToken cancellationToken)
        {
            var logs = _logRepository.GetLogEntriesAsync(cancellationToken);
            var result = new List<WeatherRequestLog>();
            await foreach (var log in logs)
            {
                result.Add(new WeatherRequestLog(log.Id, log.Success, log.Description));

            }
            return result;
        }

        public async Task LogWeatherRequestAsync(Guid id, string description, CancellationToken cancellationToken)
        {
            await _logRepository.AddLogEntryAsync(
                new WeatherLogEntry(id, success: true, description),
                cancellationToken);
        }

        public async Task LogWeatherRequestFailureAsync(string description, CancellationToken cancellationToken)
        {
            await _logRepository.AddLogEntryAsync(
                new WeatherLogEntry(null, success: false, description),
                cancellationToken);
        }
    }
}
