using System;
using System.Runtime.Serialization;

namespace NthStudio.IoC.Context
{
    [Serializable]
    public class StopRequestedException : Exception
    {
        public StopRequestedException() { }

        public StopRequestedException(String message) : base(message) { }

        public StopRequestedException(string message, Exception innerException) : base(message, innerException) { }

        protected StopRequestedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
