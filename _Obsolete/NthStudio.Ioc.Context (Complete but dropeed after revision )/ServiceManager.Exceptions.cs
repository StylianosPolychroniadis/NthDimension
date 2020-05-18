using System;
using System.Runtime.Serialization;

namespace NthStudio.Ioc.Context
{
    [Serializable]
    public class ServiceManagerBaseException : Exception
    {
        public ServiceManagerBaseException() { }

        public ServiceManagerBaseException(String message) : base(message) { }

        public ServiceManagerBaseException(string message, Exception innerException) : base(message, innerException) { }

        protected ServiceManagerBaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class StopRequestedException : Exception
    {
        public StopRequestedException() { }

        public StopRequestedException(String message) : base(message) { }

        public StopRequestedException(string message, Exception innerException) : base(message, innerException) { }

        protected StopRequestedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
