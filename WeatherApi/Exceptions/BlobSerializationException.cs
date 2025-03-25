using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApi.Exceptions
{
    public class BlobSerializationException : Exception
    {
        public BlobSerializationException()
        {
        }

        public BlobSerializationException(string? message) : base(message)
        {
        }
    }
}
