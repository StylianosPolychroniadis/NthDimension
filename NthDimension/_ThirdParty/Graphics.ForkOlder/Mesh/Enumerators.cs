using System;

namespace NthDimension.Graphics.Mesh
{
    public enum MeshResult
    {
        Ok                                  = 0,
        Failed_NotAVertex                   = 1,
        Failed_NotATriangle                 = 2,
        Failed_NotAnEdge                    = 3,

        Failed_BrokenTopology               = 10,
        Failed_HitValenceLimit              = 11,

        Failed_IsBoundaryEdge               = 20,
        Failed_FlippedEdgeExists            = 21,
        Failed_IsBowtieVertex               = 22,
        Failed_InvalidNeighbourhood         = 23,       // these are all failures for CollapseEdge
        Failed_FoundDuplicateTriangle       = 24,
        Failed_CollapseTetrahedron          = 25,
        Failed_CollapseTriangle             = 26,
        Failed_NotABoundaryEdge             = 27,
        Failed_SameOrientation              = 28,

        Failed_WouldCreateBowtie            = 30,
        Failed_VertexAlreadyExists          = 31,
        Failed_CannotAllocateVertex         = 32,

        Failed_WouldCreateNonmanifoldEdge   = 50,
        Failed_TriangleAlreadyExists        = 51,
        Failed_CannotAllocateTriangle       = 52

    };


    [Flags]
    public enum MeshComponents
    {
        None                                = 0,
        VertexNormals                       = 1,
        VertexColors                        = 2,
        VertexUVs                           = 4,
        FaceGroups                          = 8,
        All                                 = VertexNormals | VertexColors | VertexUVs | FaceGroups
    }

    [Flags]
    public enum MeshHints
    {
        None                                = 0,
        IsCompact                           = 1
    }
}
