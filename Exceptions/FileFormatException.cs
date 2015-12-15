using System;
using System.Runtime.Serialization;

namespace VPKAccess.Exceptions
{
    public class FileFormatException : FormatException
    {
        public FileFormatException()
        {
        }

        public FileFormatException(string message) : base(message)
        {
        }

        public FileFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FileFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}