using System;

namespace NthDimension.Procedural.City
{
    using NthDimension.Procedural.MockTBD;                // WARNING Ref

    public class RoadNetwork
    {
        public class Renderer { }                   // WARNING Ref


        public RoadNetwork(MediaManager media)
        {
            
        }

        public void addRoad(Road road)
        {
            throw new NotImplementedException();
        }
        public void addCrossing(Crossing crossing)
        {
            throw new NotImplementedException();
        }
        public Road[] divideRoad(Road road, Crossing crossing)  // unsure about Road[] original was Road * divideRoad(Road &road, Crossing &c);
        {
            throw new NotImplementedException();
        }
        public void sortRoadsOnCrossing()
        {
            
        }
        public void removeOneway()
        {
            
        }
        public void commitChange()
        { }
        public void render(Renderer renderer)
        {
            
        }
        public void reset()
        {
            
        }

        protected void recomputeBuffer() { throw new NotImplementedException(); }

        #region TODO:: Convert to Properties
        public Road[] getRoads()
        {
            throw new NotImplementedException();
        }

        public Crossing[] getCrossings()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
