using System;

namespace WeatherApi.Model.Persistence
{
    public record WeatherBlob(
        Guid Id,
        DateTime FetchedTimeStamp,
        WeatherPayload WeatherPayload
        );
}

