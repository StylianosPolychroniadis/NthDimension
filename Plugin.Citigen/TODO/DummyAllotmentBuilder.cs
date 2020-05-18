using System;
using NthDimension.Algebra;
using NthDimension.Rendering;
using RoadGen;

public class DummyAllotmentBuilder : /*/*MonoBehaviour*/ IAllotmentBuilder
{
    public float height = 12;
    public Material material;

    public ApplicationObject Build(Allotment allotment, IHeightmap heightmap)
    {
        throw new System.NotImplementedException();
        {

            //GameObject allotmentGO = new GameObject("Allotment");
            //float z = heightmap.GetHeight(allotment.Center.X, allotment.Center.Y);
            //allotmentGO.AddComponent<MeshFilter>().mesh = StandardGeometry.CreateCubeMesh(allotment.Corners, height, z);
            //allotmentGO.AddComponent<MeshRenderer>().material = material;
            //return allotmentGO;
        }
    }

}