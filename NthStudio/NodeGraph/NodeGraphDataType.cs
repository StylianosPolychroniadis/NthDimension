using System.Drawing;


namespace NthStudio.NodeGraph
{
    // Todo : Switch to Nano Equivalents
   
    public abstract class NodeGraphDataType
    {
        protected Pen m_LinkPen;
        protected SolidBrush m_LinkArrowBrush;

        protected Pen m_ConnectorOutlinePen;
        protected SolidBrush m_ConnectorFillBrush;

        protected string m_TypeName;
        protected bool m_hasLink = true;
        public Pen LinkPen
        {
            get
            {
                return this.m_LinkPen;
            }
        }
        public SolidBrush LinkArrowBrush
        {
            get
            {
                return this.m_LinkArrowBrush;
            }
        }
        public Pen ConnectorOutlinePen
        {
            get
            {
                return this.m_ConnectorOutlinePen;
            }
        }
        public SolidBrush ConnectorFillBrush
        {
            get
            {
                return this.m_ConnectorFillBrush;
            }
        }
        public string TypeName
        {
            get { return m_TypeName; }
        }
    }

    
}
