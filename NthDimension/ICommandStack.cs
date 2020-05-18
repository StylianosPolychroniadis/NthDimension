using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension
{
    /// <summary>
    /// Command Pattern Server
    /// </summary>
    public interface ICommandRenderingStack
    {
        void AddCommandHandler(ushort commandCode, Rendering.ApplicationCommandRenderingHandler renderingCommand);
        void RegisterCommands();
    }
}
