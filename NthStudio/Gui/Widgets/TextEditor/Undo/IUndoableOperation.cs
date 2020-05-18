using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor.Undo
{
    /// <summary>
    /// This Interface describes a the basic Undo/Redo operation
    /// all Undo Operations must implement this interface.
    /// </summary>
    public interface IUndoableOperation
    {
        /// <summary>
        /// Undo the last operation
        /// </summary>
        void Undo();

        /// <summary>
        /// Redo the last operation
        /// </summary>
        void Redo();
    }
}
