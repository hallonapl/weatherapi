using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApi.Configuration
{
    public class StorageSettings
    {
        public string? WeatherDataContainerName { get; set; }
        public string? LogTableName { get; set; }
    }
}
