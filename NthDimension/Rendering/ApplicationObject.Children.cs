using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering
{
    public partial class ApplicationObject
    {
        

        public virtual int addChild(ApplicationObject newChild)
        {
            childs.Add(newChild);
            return childs.Count - 1;
        }

        public virtual void removeChild(ApplicationObject reChild)
        {
            childs.Remove(reChild);
        }
        public int getChildId(string name)
        {
            int length = childs.Count;
            for (int i = 0; i < length; i++)
            {
                if (childs[i].Name == name)
                    return i;
            }
            return -1;
        }
        public ApplicationObject getChild(string name)
        {
            if (name != "")
            {
                int length = childs.Count;
                for (int i = 0; i < length; i++)
                {
                    ApplicationObject curChild = childs[i];
                    if (curChild.Name == name)
                        return curChild;
                    else
                    if ((curChild = curChild.getChild(name)) != null)
                        return curChild;
                }
            }
            return null;
        }

        public virtual ApplicationObject Parent
        {
            get { return parent; }
            set
            {

                if (parent != null)
                {
                    parent.removeChild(this);
                }

                if (value != null)
                {
                    //gameWindow = value.gameWindow;

                    if (value.Scene != null)
                    {
                        scene = value.Scene;
                    }

                    parent = value;

                    childId = parent.addChild(this);
                }
            }
        }
        
    }
}
