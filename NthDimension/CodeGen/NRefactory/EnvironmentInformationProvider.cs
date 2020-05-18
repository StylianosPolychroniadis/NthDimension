// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace NRefactory
{
	public interface IEnvironmentInformationProvider
	{
		bool HasField(string fullTypeName, string fieldName);
	}
	
	class DummyEnvironmentInformationProvider : IEnvironmentInformationProvider
	{
		public bool HasField(string fullTypeName, string fieldName)
		{
			return false;
		}
	}
}
