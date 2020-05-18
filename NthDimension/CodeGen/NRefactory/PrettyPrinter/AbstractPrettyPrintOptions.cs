// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1966 $</version>
// </file>

namespace NRefactory.PrettyPrinter
{
	/// <summary>
	/// Description of PrettyPrintOptions.	
	/// </summary>
	public class AbstractPrettyPrintOptions
	{
		char indentationChar = '\t';
		int  tabSize         = 4;
		int  indentSize      = 4;
		
		public char IndentationChar {
			get {
				return indentationChar;
			}
			set {
				indentationChar = value;
			}
		}
		
		public int TabSize {
			get {
				return tabSize;
			}
			set {
				tabSize = value;
			}
		}
		
		public int IndentSize {
			get {
				return indentSize;
			}
			set {
				indentSize = value;
			}
		}
	}
}
