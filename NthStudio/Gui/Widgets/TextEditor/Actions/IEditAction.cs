using NthDimension.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor.Actions
{
    /// <summary>
    /// To define a new key for the textarea, you must write a class which
    /// implements this interface.
    /// </summary>
    public interface IEditAction
    {
        /// <value>
        /// An array of keys on which this edit action occurs.
        /// </value>
        Keys[] Keys
        {
            get;
            set;
        }

        /// <remarks>
        /// When the key which is defined per XML is pressed, this method will be launched.
        /// </remarks>
        void Execute(TextCanvas pTextCanvas);
    }

    /// <summary>
    /// To define a new key for the textarea, you must write a class which
    /// implements this interface.
    /// </summary>
    public abstract class AbstractEditAction : IEditAction
    {
        Keys[] keys = null;

        /// <value>
        /// An array of keys on which this edit action occurs.
        /// </value>
        public Keys[] Keys
        {
            get
            {
                return keys;
            }
            set
            {
                keys = value;
            }
        }

        /// <remarks>
        /// When the key which is defined per XML is pressed, this method will be launched.
        /// </remarks>
        public abstract void Execute(TextCanvas pTextCanvas);
    }
}
