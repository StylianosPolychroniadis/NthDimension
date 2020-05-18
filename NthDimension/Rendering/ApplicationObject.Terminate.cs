using System;

namespace NthDimension.Rendering
{
    public partial class ApplicationObject
    {
        public virtual void kill()
        {
            Parent = null;

            killChilds();
        }

        protected void killChilds()
        {
            try
            {
                while (childs.Count > 0)
                {
                    childs[0].MarkForDelete = true;
                    childs[0].kill();
                }
            }
            catch (Exception k)
            {
                Utilities.ConsoleUtil.errorlog("ApplicationObject.killchilds() ", string.Format("{0}\n{1}", k.Message, k.StackTrace));
            }
        }
    }
}
