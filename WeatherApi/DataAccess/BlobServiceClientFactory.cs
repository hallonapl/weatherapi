using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApi.DataAccess
{
    public class BlobServiceClientFactory
    {
        private readonly string _connectionString;

        public BlobServiceClientFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BlobServiceClient GetBlobServiceClient()
        {
            return new BlobServiceClient(_connectionString);
        }
    }
}
