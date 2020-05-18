using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Graphics.Mesh
{
    public interface IMeshBuilder
    {
        // return ID of new mesh
        int AppendNewMesh(bool bHaveVtxNormals, bool bHaveVtxColors, bool bHaveVtxUVs, bool bHaveFaceGroups);
        int AppendNewMesh(DMesh3 existingMesh);

        void SetActiveMesh(int id);

        int AppendVertex(double x, double y, double z);
        int AppendVertex(NewVertexInfo info);

        int AppendTriangle(int i, int j, int k);
        int AppendTriangle(int i, int j, int k, int g);


        // material handling

        // return client-side unique ID of material
        int BuildMaterial(GenericMaterial m);

        // do material assignment to mesh, where meshID comes from IMeshBuilder
        void AssignMaterial(int materialID, int meshID);

        // optional
        bool SupportsMetaData { get; }
        void AppendMetaData(string identifier, object data);
    }
}
