using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor.Document.FormattingStrategy
{
    /// <summary>
    /// This interface handles the auto and smart indenting and formating
    /// in the document while  you type. Language bindings could overwrite this 
    /// interface and define their own indentation/formating.
    /// </summary>
    public interface IFormattingStrategy
    {
        /// <summary>
        /// This function formats a specific line after <code>ch</code> is pressed.
        /// </summary>
        void FormatLine(TextCanvas textCanvas, int line, int caretOffset, char charTyped);

        /// <summary>
        /// This function sets the indentation level in a specific line
        /// </summary>
        /// <returns>
        /// The target caret position (length of new indentation).
        /// </returns>
        int IndentLine(TextCanvas textCanvas, int line);

        /// <summary>
        /// This function sets the indentlevel in a range of lines.
        /// </summary>
        void IndentLines(TextCanvas textCanvas, int begin, int end);

        /// <summary>
        /// Finds the offset of the opening bracket in the block defined by offset skipping
        /// brackets in strings and comments.
        /// </summary>
        /// <param name="document">The document to search in.</param>
        /// <param name="offset">The offset of an position in the block or the offset of the closing bracket.</param>
        /// <param name="openBracket">The character for the opening bracket.</param>
        /// <param name="closingBracket">The character for the closing bracket.</param>
        /// <returns>Returns the offset of the opening bracket or -1 if no matching bracket was found.</returns>
        int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket);

        /// <summary>
        /// Finds the offset of the closing bracket in the block defined by offset skipping
        /// brackets in strings and comments.
        /// </summary>
        /// <param name="document">The document to search in.</param>
        /// <param name="offset">The offset of an position in the block or the offset of the opening bracket.</param>
        /// <param name="openBracket">The character for the opening bracket.</param>
        /// <param name="closingBracket">The character for the closing bracket.</param>
        /// <returns>Returns the offset of the closing bracket or -1 if no matching bracket was found.</returns>
        int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket);
    }
}
