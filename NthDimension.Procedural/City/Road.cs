using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Algebra;

namespace NthDimension.Procedural.City
{
    public class Road
    {
        public enum ERoadType
        {
            RT_STREET,
            RT_HIGHWAY
        };

        public enum ERoadZoneType
        {
            RZT_UNDEFINED = 0x01,
            RZT_COMMERCIAL = 0x02,
            RZT_RESIDENTIAL = 0X04
        };

        private Crossing            m_crossing1;
        private Crossing            m_crossing2;
        private ERoadType           m_type;
        private uint                m_zoneType;
        private bool                m_isBridge;

        public Road(Crossing crossingA, Crossing crossingB, ERoadType type)
        {
            this.m_crossing1 = crossingA;
            this.m_crossing2 = crossingB;
            this.m_type = type;
        }

        #region TODO:: Convert to Properties?

        public Vector2 get2DVector()
        {
            throw new NotImplementedException();
        }

        public Vector3 get3DVector()
        {
            throw new NotImplementedException();
        }

        public ERoadType getType()
        {
            return this.m_type;
        }
        #endregion


        public void detach()
        {
            
        }

        public void setCrossing(Crossing oldCrossing, Crossing newCrossing)
        {
            throw new NotImplementedException();
        }
        public Crossing getEndCrossing(Crossing startCrossing)
        {
            throw new NotImplementedException();
        }
        public Crossing getStartCrossing()
        {
            throw new NotImplementedException();
        }
        public Crossing getEndCrossing()
        {
            throw new NotImplementedException();
        }

        public void addZoneType(ERoadZoneType type)
        {
            throw new NotImplementedException();
        }
        public uint getZoneType()
        {
            throw new NotImplementedException();
        }
        public void setBridge()
        {
            throw new NotImplementedException();
        }
        public bool isBridge()
        {
            throw new NotImplementedException();
        }

    }
}
