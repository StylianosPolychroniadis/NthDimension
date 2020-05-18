using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering
{
    public class ApplicationStack : ICommandRenderingStack
    {
        // Plugin
        internal readonly Dictionary<ushort, ApplicationCommandRenderingHandler> _renderingCommandHandlers;
        public void AddCommandHandler(ushort commandCode, ApplicationCommandRenderingHandler renderingCommand)
        {
            _renderingCommandHandlers.Add(commandCode, renderingCommand);
        }
        public void RegisterCommands()
        {
            throw new NotImplementedException();
        }

        // Undo Pattern
        public event EventHandler       EnableDisableUndoRedoFeature;

        private Stack<ICommand>         _undocommands = new Stack<ICommand>();
        private Stack<ICommand>         _redocommands = new Stack<ICommand>();

        public void Undo(int levels)
        {
            for (int i = 1; i <= levels; i++)
            {
                if (_undocommands.Count != 0)
                {
                    ICommand command = _undocommands.Pop();
                    command.UnExecute();
                    _redocommands.Push(command);
                }

            }
            if (EnableDisableUndoRedoFeature != null)
            {
                EnableDisableUndoRedoFeature(null, null);
            }
        }
        public void Redo(int levels)
        {
            for (int i = 1; i <= levels; i++)
            {
                if (_redocommands.Count != 0)
                {
                    ICommand command = _redocommands.Pop();
                    command.Execute();
                    _undocommands.Push(command);
                }

            }
            if (EnableDisableUndoRedoFeature != null)
            {
                EnableDisableUndoRedoFeature(null, null);
            }
        }
        public bool IsUndoPossible()
        {
            if (_undocommands.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsRedoPossible()
        {

            if (_redocommands.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
