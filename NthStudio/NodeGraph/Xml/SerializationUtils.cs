using System;
namespace NthStudio.NodeGraph.Xml
{
	internal static class SerializationUtils
	{
		public static string GetFullTypeName(object o)
		{
			return o.GetType().FullName;
		}
	}
}
