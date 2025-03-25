namespace WeatherApi.Exceptions
{
    public class BlobNotSavedException : Exception
    {
        public BlobNotSavedException()
        {
        }

        public BlobNotSavedException(string? message) : base(message)
        {
        }
    }
}
