using NthStudio.Gui.Widgets.TextEditor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor.Document
{
    public class Highlight
    {
        public TextLocation OpenBrace { get; set; }
        public TextLocation CloseBrace { get; set; }

        public Highlight(TextLocation openBrace, TextLocation closeBrace)
        {
            this.OpenBrace = openBrace;
            this.CloseBrace = closeBrace;
        }
    }

    public class BracketHighlightingSheme
    {
        char opentag;
        char closingtag;

        public char OpenTag
        {
            get
            {
                return opentag;
            }
            set
            {
                opentag = value;
            }
        }

        public char ClosingTag
        {
            get
            {
                return closingtag;
            }
            set
            {
                closingtag = value;
            }
        }

        public BracketHighlightingSheme(char opentag, char closingtag)
        {
            this.opentag = opentag;
            this.closingtag = closingtag;
        }

        public Highlight GetHighlight(IDocument document, int offset)
        {
            int searchOffset;
            if (document.TextEditorProperties.BracketMatchingStyle == BracketMatchingStyle.After)
            {
                searchOffset = offset;
            }
            else
            {
                searchOffset = offset + 1;
            }
            char word = document.GetCharAt(Math.Max(0, Math.Min(document.TextLength - 1, searchOffset)));

            TextLocation endP = document.OffsetToPosition(searchOffset);
            if (word == opentag)
            {
                if (searchOffset < document.TextLength)
                {
                    int bracketOffset = TextUtility.SearchBracketForward(document, searchOffset + 1, opentag, closingtag);
                    if (bracketOffset >= 0)
                    {
                        TextLocation p = document.OffsetToPosition(bracketOffset);
                        return new Highlight(p, endP);
                    }
                }
            }
            else if (word == closingtag)
            {
                if (searchOffset > 0)
                {
                    int bracketOffset = TextUtility.SearchBracketBackward(document, searchOffset - 1, opentag, closingtag);
                    if (bracketOffset >= 0)
                    {
                        TextLocation p = document.OffsetToPosition(bracketOffset);
                        return new Highlight(p, endP);
                    }
                }
            }
            return null;
        }
    }
}
