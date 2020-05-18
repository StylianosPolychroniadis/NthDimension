using NthDimension.Rendering;

namespace RoadGen
{
    public interface IAllotmentBuilder
    {
        ApplicationObject Build(Allotment allotment, IHeightmap heightmap);

    }

}