using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Algebra;
using static NthDimension.Procedural.City.Road;

namespace NthDimension.Procedural.City
{
    public class Crossing 
    {
        private Vector3                 m_position;
        private List<Road>              m_roads;
        private List<Vector3>           m_junctionPositions;


        public Crossing(Vector3 position)
        {
            this.m_position             = position;
            this.m_roads                = new List<Road>();
            this.m_junctionPositions    = new List<Vector3>();
        }

        #region TODO:: Convert to Properties

        public void setPosition(Vector3 position)
        {
            this.m_position = position;
        }
        public Vector3 getPosition()
        {
            return this.m_position;
        }
        public Vector2 get2DPosition()
        {
            return new Vector2(this.m_position.X, this.m_position.Z);
        }
        public Road[] getRoads()
        {
            return this.m_roads.ToArray();
        }
        #endregion

        public void setJunctionPositions(List<Vector3> positions)
        {
            
            //this.m_junctionPositions.Swap ???
        }
        public Vector3 getJunctionPosition(uint index)
        {
            throw new NotImplementedException();
        }
        public void addRoad(Road road)
        {
            this.m_roads.Add(road);
        }
        public void removeRoad(Road road)
        {
            this.m_roads.Remove(road);
        }
        public bool isNeighbor(Crossing crossing)
        {
            foreach (Road it in m_roads)
                if (it.getEndCrossing(this) == crossing)                
                    return true;                
         
            return false;
        }
        public bool isNeighbor(Road road)
        {
            foreach (Road it in m_roads)
                if (it == road)
                    return true;

            return false;
        }
        public bool hasColinearRoad(Road road, float cosTreshold)
        {
            Vector2 dir = road.get2DVector();
            dir.NormalizeFast();

            if(cosTreshold < .0f)
            {
                dir = -dir;
                cosTreshold = -cosTreshold;
            }

            foreach(Road it in m_roads)
            {
                if (it == road) continue;

                Vector2 dir2 = it.get2DVector();
                dir2.NormalizeFast();

                if (this != it.getStartCrossing())
                    dir2 = -dir2;

                if (Vector2.Dot(dir, dir2) > cosTreshold)
                    return true;
            }
            return false;
        }
        public int getNbStreet()
        {
            int nb = 0;
            for (int i = 0; i < m_roads.Count; ++i)
            {
                if (m_roads[i].getType() == ERoadType.RT_STREET)
                {
                    ++nb;
                }
            }
            return nb;
        }
        public void sortRoad()
        {
            throw new NotImplementedException();
        }
        public int getRoadIndex(Road road)
        {
            for (int i = 0; i < m_roads.Count; ++i)
            {
                if (m_roads[i] == road)
                {
                    return (int)i;
                }
            }
            return -1;
        }


    }
}
