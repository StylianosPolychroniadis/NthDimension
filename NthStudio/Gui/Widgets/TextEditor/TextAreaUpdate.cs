using System;
using NthStudio.Gui.Widgets.TextEditor.Document;

namespace NthStudio.Gui.Widgets.TextEditor
{
    /// <summary>
    /// This class is used to request an update of the textarea
    /// </summary>
    public class TextAreaUpdate
    {
        TextLocation position;
        TextAreaUpdateType type;

        public TextAreaUpdateType TextAreaUpdateType
        {
            get
            {
                return type;
            }
        }

        public TextLocation Position
        {
            get
            {
                return position;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="TextAreaUpdate"/>
        /// </summary>
        public TextAreaUpdate(TextAreaUpdateType type)
        {
            this.type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="TextAreaUpdate"/>
        /// </summary>
        public TextAreaUpdate(TextAreaUpdateType type, TextLocation position)
        {
            this.type = type;
            this.position = position;
        }

        /// <summary>
        /// Creates a new instance of <see cref="TextAreaUpdate"/>
        /// </summary>
        public TextAreaUpdate(TextAreaUpdateType type, int startLine, int endLine)
        {
            this.type = type;
            this.position = new TextLocation(startLine, endLine);
        }

        /// <summary>
        /// Creates a new instance of <see cref="TextAreaUpdate"/>
        /// </summary>
        public TextAreaUpdate(TextAreaUpdateType type, int singleLine)
        {
            this.type = type;
            this.position = new TextLocation(0, singleLine);
        }

        public override string ToString()
        {
            return String.Format("[TextAreaUpdate: Type={0}, Position={1}]", type, position);
        }
    }
}
