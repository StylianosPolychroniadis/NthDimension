using System;
using System.Runtime.Serialization;

namespace NthDimension.Math.Geography.Exceptions
{
	[Serializable]
	public sealed class DecayException : PropagationException
	{
		public DateTime DecayTime
		{
			get;
			private set;
		}

		public string SatelliteName
		{
			get;
			private set;
		}

		public DecayException()
		{
		}

		public DecayException(string message)
			: base(message)
		{
		}

		public DecayException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public DecayException(DateTime decayTime, string satelliteName)
		{
			DecayTime = decayTime;
			SatelliteName = satelliteName;
		}

		private DecayException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
