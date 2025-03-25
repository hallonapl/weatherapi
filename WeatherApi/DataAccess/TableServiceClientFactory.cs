using Azure.Data.Tables;
using Azure.Storage.Blobs;

namespace WeatherApi.DataAccess
{
    public class TableServiceClientFactory
    {
        private readonly string _connectionString;

        public TableServiceClientFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TableServiceClient GetTableServiceClient()
        {
            return new TableServiceClient(_connectionString);
        }
    }
}
