
using NthStudio.NodeGraph;


namespace NthStudio.Gui.NodeGraph.Data
{
    public class Float : NodeGraphData
    {
        private float m_fValue;
        public float Value
        {
            get
            {
                return this.m_fValue;
            }
            set
            {
                this.m_fValue = value;
            }
        }
        public Float(float p_Value)
        {
            this.m_fValue = p_Value;
        }
    }
    



}
