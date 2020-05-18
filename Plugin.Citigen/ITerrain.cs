using NthDimension.Algebra;

namespace RoadGen
{
    public interface ITerrain : IHeightmap
    {
        int GetHeightmapDownscale();
        TerrainData GetData();

    }

    public class TerrainData
    {
        // MISSING
    }
}