using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApi.Model
{
    public record WeatherRequestLog(
        Guid? Id,
        bool Success,
        string Description);
}
