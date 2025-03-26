using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApi.Model.Api
{
    public record WeatherResponse(
        string City,
        string Country,
        double Temperature,
        double FeelsLike,
        double MinTemperature,
        double MaxTemperature,
        double Pressure,
        double Humidity,
        double WindSpeed,
        double WindDegree,
        double Cloudiness,
        double UvIndex,
        DateTime Date);
}
