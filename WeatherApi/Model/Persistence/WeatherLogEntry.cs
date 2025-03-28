using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApi.Model.Persistence
{
    public class WeatherLogEntry : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public Guid? Id { get; set; }
        public bool Success { get; set; }
        public string Description { get; set; }

        public WeatherLogEntry()
        {
        }

        public WeatherLogEntry(Guid? id, bool success, string description)
        {
            PartitionKey = "WeatherLog";
            RowKey = Guid.NewGuid().ToString();
            Id = id;
            Success = success;
            Description = description;
        }
    }
}
