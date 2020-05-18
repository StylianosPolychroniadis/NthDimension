﻿
using System;

using NthStudio.Gui.Widgets.TextEditor.Document;

namespace NthStudio.Gui.Widgets.TextEditor
{
    /// <summary>
    /// This class contains more information on a document event
    /// </summary>
    public class DocumentEventArgs : EventArgs
    {
        IDocument document;
        int offset;
        int length;
        string text;

        /// <returns>
        /// always a valid Document which is related to the Event.
        /// </returns>
        public IDocument Document
        {
            get
            {
                return document;
            }
        }

        /// <returns>
        /// -1 if no offset was specified for this event
        /// </returns>
        public int Offset
        {
            get
            {
                return offset;
            }
        }

        /// <returns>
        /// null if no text was specified for this event
        /// </returns>
        public string Text
        {
            get
            {
                return text;
            }
        }

        /// <returns>
        /// -1 if no length was specified for this event
        /// </returns>
        public int Length
        {
            get
            {
                return length;
            }
        }

        /// <summary>
        /// Creates a new instance off <see cref="DocumentEventArgs"/>
        /// </summary>
        public DocumentEventArgs(IDocument document)
            : this(document, -1, -1, null)
        {
        }

        /// <summary>
        /// Creates a new instance off <see cref="DocumentEventArgs"/>
        /// </summary>
        public DocumentEventArgs(IDocument document, int offset)
            : this(document, offset, -1, null)
        {
        }

        /// <summary>
        /// Creates a new instance off <see cref="DocumentEventArgs"/>
        /// </summary>
        public DocumentEventArgs(IDocument document, int offset, int length)
            : this(document, offset, length, null)
        {
        }

        /// <summary>
        /// Creates a new instance off <see cref="DocumentEventArgs"/>
        /// </summary>
        public DocumentEventArgs(IDocument document, int offset, int length, string text)
        {
            this.document = document;
            this.offset = offset;
            this.length = length;
            this.text = text;
        }

        public override string ToString()
        {
            return String.Format("[DocumentEventArgs: Document = {0}, Offset = {1}, Text = {2}, Length = {3}]",
                                 Document,
                                 Offset,
                                 Text,
                                 Length);
        }
    }
}
