// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2517 $</version>
// </file>

using System;
using System.Collections.Generic;
using NRefactory.Ast;

namespace NRefactory
{
	/// <summary>
	/// Parser interface.
	/// </summary>
	public interface IParser : IDisposable
	{
		Parser.Errors Errors {
			get;
		}
		
		Parser.ILexer Lexer {
			get;
		}
		
		CompilationUnit CompilationUnit {
			get;
		}
		
		bool ParseMethodBodies {
			get; set;
		}
		
		void Parse();
		
		Expression ParseExpression();
		BlockStatement ParseBlock();
		List<INode> ParseTypeMembers();
	}
}
