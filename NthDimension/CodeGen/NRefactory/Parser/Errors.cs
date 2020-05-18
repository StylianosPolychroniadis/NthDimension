// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1609 $</version>
// </file>

using System;
using System.Text;

namespace NRefactory.Parser
{
	public delegate void ErrorCodeProc(int line, int col, int n);
	public delegate void ErrorMsgProc(int line, int col, string msg);
	
	public class Errors
	{
		int count = 0;  // number of errors detected
		public ErrorCodeProc SynErr;
		public ErrorCodeProc SemErr;
		public ErrorMsgProc  Error;
        public int LineNo = 0;
        public int ColumnNo = 0;
		StringBuilder errorText = new StringBuilder();
		
		public string ErrorOutput {
			get {
				return errorText.ToString();
			}
		}
		
		public Errors()
		{
			SynErr = new ErrorCodeProc(DefaultCodeError);  // syntactic errors
			SemErr = new ErrorCodeProc(DefaultCodeError);  // semantic errors
			Error  = new ErrorMsgProc(DefaultMsgError);    // user defined string based errors
		}
		
		public int Count {
			get {
				return count;
			}
		}
		
	//	public void Exception (string s)
	//	{
	//		Console.WriteLine(s); 
	//		System.Environment.Exit(0);
	//	}
	
		void DefaultCodeError(int line, int col, int n)
		{
            LineNo = line;
            ColumnNo = col;
			//errorText.AppendLine(String.Format("-- line {0} col {1}: error {2}", line, col, n));
            errorText.AppendLine(String.Format("{0}", n));
			count++;
		}
	
		void DefaultMsgError(int line, int col, string s) {
            LineNo = line;
            ColumnNo = col;
			//errorText.AppendLine(String.Format("-- line {0} col {1}: {2}", line, col, s));
            errorText.AppendLine(String.Format("{0}", s));
			count++;
		}
	} // Errors
}
