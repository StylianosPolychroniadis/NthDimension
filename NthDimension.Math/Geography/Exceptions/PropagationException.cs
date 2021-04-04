using System;
using System.Runtime.Serialization;

namespace NthDimension.Math.Geography.Exceptions
{
	[Serializable]
	public class PropagationException : Exception
	{
		public PropagationException()
		{
		}

		public PropagationException(string message)
			: base(message)
		{
		}

		public PropagationException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected PropagationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
