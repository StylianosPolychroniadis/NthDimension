using System;
using System.Runtime.Serialization;

namespace NthStudio.IoC.Context
{
    [Serializable]
    public class ServiceManagerBaseException : Exception
    {
        public ServiceManagerBaseException() { }

        public ServiceManagerBaseException(String message) : base(message) { }

        public ServiceManagerBaseException(string message, Exception innerException) : base(message, innerException) { }

        protected ServiceManagerBaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
