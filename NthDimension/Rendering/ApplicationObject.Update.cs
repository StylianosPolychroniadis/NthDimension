using System;

namespace NthDimension.Rendering
{
    public partial class ApplicationObject
    {
        public virtual void forceUpdate()
        {
            updateChilds();
        }

        public virtual void Update()
        {
            this.childs.RemoveAll(m => m.MarkForDelete == true);
            updateChilds();
        }

        protected void updateChilds()
        {
            try
            {
                this.childs.RemoveAll(m => m.MarkForDelete);

                //foreach (var c in this.childs)
                ////Parallel.ForEach(this.childs, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (c) =>
                //{
                //    if (null == c) continue;
                //    c.Update();
                //}
                ////);

                for (int c = 0; c < this.childs.Count; c++)
                {
                    if (null != this.childs[c])
                        this.childs[c].Update();
                }
            }
            catch (Exception c) { Utilities.ConsoleUtil.errorlog("GameObject.updateChilds() ", string.Format("{0}\n{1}", c.Message, c.StackTrace)); }
        }
    }
}
