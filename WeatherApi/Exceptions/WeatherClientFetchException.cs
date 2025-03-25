namespace WeatherApi.Exceptions
{
    public class WeatherClientFetchException : Exception
    {
        public WeatherClientFetchException()
        {
        }

        public WeatherClientFetchException(string? message) : base(message)
        {
        }
    }

    }
