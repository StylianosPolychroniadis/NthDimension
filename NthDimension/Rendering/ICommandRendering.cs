using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering
{
    public interface ICommandRendering : ICommand
    {
        void Execute(Object commandData, ApplicationCommandRendering command);
    }
}
