using NthStudio.Gui.Widgets.TextEditor.Document.LineManagement;
using System;
using System.Collections.Generic;

namespace NthStudio.Gui.Widgets.TextEditor.Document.FoldingStrategy
{
    /// <summary>
    /// The class to generate the foldings, it implements ICSharpCode.TextEditor.Document.IFoldingStrategy
    /// </summary>
    public class RegionFoldingStrategy : IFoldingStrategy
    {
        /// <summary>
        /// Generates the foldings for our document.
        /// </summary>
        /// <param name="document">The current document.</param>
        /// <param name="fileName">The filename of the document.</param>
        /// <param name="parseInformation">Extra parse information, not used in this sample.</param>
        /// <returns>A list of FoldMarkers.</returns>
        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            char[] ctrims = { ' ', '\t', '\n' };
            var list = new List<FoldMarker>();

            var startLines = new Stack<int>();

            // Create foldmarkers for the whole document, enumerate through every line.
            for (int i = 0; i < document.TotalNumberOfLines; i++)
            {
                LineSegment seg = document.GetLineSegment(i);
                int offs, end = document.TextLength;
                char c;
                for (offs = seg.Offset; offs < end && ((c = document.GetCharAt(offs)) == ' ' || c == '\t'); offs++)
                {
                }
                if (offs == end)
                    break;
                int spaceCount = offs - seg.Offset;

                // now offs points to the first non-whitespace char on the line
                if (document.GetCharAt(offs) == '#')
                {
                    string text = document.GetText(offs, seg.Length - spaceCount);
                    if (text.StartsWith("#region", StringComparison.InvariantCulture))
                        startLines.Push(i);
                    if (text.StartsWith("#endregion", StringComparison.InvariantCulture) && startLines.Count > 0)
                    {
                        // Add a new FoldMarker to the list.
                        int start = startLines.Pop();
                        LineSegment segStart = document.GetLineSegment(start);
                        text = document.GetText(segStart).TrimEnd(ctrims).TrimStart(ctrims);

                        string text2;

                        if (text.Length > 6)
                        {
                            if (text.Length >= 7)
                                text2 = text.Substring(7, text.Length - 7);
                            else
                                text2 = text.Substring(6, text.Length - 6);

                            text2 = text2.TrimStart(ctrims);
                        }
                        else
                            text2 = "...";

                        list.Add(new FoldMarker(document, start,
                                                spaceCount, /*document.GetLineSegment(start).Length*/
                                                i, segStart.Length, FoldType.Region, text2)); //spaceCount + "#endregion".Length + 10));
                    }
                }
            }

            return list;
        }
    }
}
