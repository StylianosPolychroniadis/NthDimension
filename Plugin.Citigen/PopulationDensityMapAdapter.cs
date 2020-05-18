using NthDimension.Algebra;
using RoadGen;

public class PopulationDensityMapAdapter : /*MonoBehaviour*/ IMap
{
    public float GetWidth()
    {
        throw new System.NotImplementedException();
        {

            //return RoadGen.Config.QuadtreeParams.width;
        }
    }

    public float GetHeight()
    {
        throw new System.NotImplementedException();
        {

         //   return RoadGen.Config.QuadtreeParams.height;
        }
    }

    public float GetMinX()
    {
        throw new System.NotImplementedException();
        {

            //return RoadGen.Config.QuadtreeParams.xMin;
        }
    }

    public float GetMaxX()
    {
        throw new System.NotImplementedException();
        {

            //return RoadGen.Config.QuadtreeParams.xMax;
        }
    }

    public float GetMinY()
    {
        throw new System.NotImplementedException();
        {

            //return RoadGen.Config.QuadtreeParams.yMin;
        }
    }

    public float GetMaxY()
    {
        throw new System.NotImplementedException();
        {

         //   return RoadGen.Config.QuadtreeParams.yMax;
        }
    }

    public float GetNormalizedValue(float x, float y)
    {
        return RoadGen.PopulationDensityMap.DensityAt(x, y);
    }

    public bool Finished()
    {
        return true;
    }

}
