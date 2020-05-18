using NthStudio.NodeGraph;

namespace NthStudio.Gui.NodeGraph.Data
{


    public class NewT<T> : NodeGraphData
    {
        private T m_tvalue;
        public T Value
        {
            get
            {
                return this.m_tvalue;
            }
            set
            {
                this.m_tvalue = value;
            }
        }

        public NewT(T tvalue)
        {
            this.m_tvalue = tvalue;
        }
            
            

    }
}
