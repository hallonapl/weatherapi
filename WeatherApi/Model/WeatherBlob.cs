using System;

namespace WeatherApi.Model
{
    public record WeatherBlob(
        Guid Id,
        DateTime FetchedTimeStamp,
        WeatherPayload WeatherPayload
        );
}

