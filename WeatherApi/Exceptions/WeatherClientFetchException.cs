namespace WeatherApi.Exceptions
{
    internal class WeatherClientFetchException : Exception
    {
        public WeatherClientFetchException()
        {
        }

        public WeatherClientFetchException(string? message) : base(message)
        {
        }
    }

    }
