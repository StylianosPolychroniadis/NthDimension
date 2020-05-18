using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering
{
    public abstract class ApplicationCommandRendering : ICommandRendering
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void Execute(Object commandData, ApplicationCommandRendering command)
        {
            throw new NotImplementedException();
        }        

        public void UnExecute()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class ApplicationCommandRenderingHandler
    {
        protected ICommandRenderingStack m_stack;

        public ApplicationCommandRenderingHandler(ICommandRenderingStack stack)
        {
            m_stack = stack;
        }

        public abstract void Execute(Object commandData, ApplicationCommandRendering command);
        
    }
}
