using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Drawables.Models
{
    public class Cube : PhysModel
    {

        public Cube(ApplicationObject parent) : base(parent)
        {

            PrimitiveType = Rasterizer.PrimitiveType.Quads;

            var
                mesh = ApplicationBase.Instance.MeshLoader.FromMesh(
                new Serialization.ListVector3 {
                new Algebra.Vector3(-1, -1, -1),
                new Algebra.Vector3( 1, -1, -1),
                new Algebra.Vector3( 1, -1,  1),
                new Algebra.Vector3(-1, -1,  1),
                new Algebra.Vector3(-1,  1, -1),
                new Algebra.Vector3( 1,  1, -1),
                new Algebra.Vector3( 1,  1,  1),
                new Algebra.Vector3(-1,  1,  1)
            },
                new Serialization.ListVector3
                {
                    new Algebra.Vector3( 0,  0, -1),
                    new Algebra.Vector3( 0,  0,  1),
                    new Algebra.Vector3( 0, -1,  1),
                    new Algebra.Vector3( 0,  1,  0),
                    new Algebra.Vector3( 1,  0,  0),
                    new Algebra.Vector3(-1,  0,  0)
                },
                new Serialization.ListVector2 {
                    new Algebra.Vector2(0, 0),
                    new Algebra.Vector2(0, 1),
                    new Algebra.Vector2(1, 1),
                    new Algebra.Vector2(1,0)
                },
                new Serialization.ListFace
                {
                    // TODO
                    new Geometry.Face(new VertexIndices(0, 0, 0),
                                      new VertexIndices(1, 1, 0),
                                      new VertexIndices(2, 2, 0),
                                      new VertexIndices(3, 3, 0)),

                    new Geometry.Face(new VertexIndices(5, 0, 1),
                                      new VertexIndices(4, 1, 1),
                                      new VertexIndices(7, 2, 1),
                                      new VertexIndices(6, 3, 1)),

                    new Geometry.Face(new VertexIndices(1, 0, 2),
                                      new VertexIndices(5, 1, 2),
                                      new VertexIndices(6, 2, 2),
                                      new VertexIndices(2, 3, 2)),

                    new Geometry.Face(new VertexIndices(7, 0, 3),
                                      new VertexIndices(4, 1, 3),
                                      new VertexIndices(0, 2, 3),
                                      new VertexIndices(3, 3, 3)),

                    new Geometry.Face(new VertexIndices(4, 0, 4),
                                      new VertexIndices(5, 1, 4),
                                      new VertexIndices(1, 2, 4),
                                      new VertexIndices(0, 3, 4)),

                    new Geometry.Face(new VertexIndices(6, 0, 5),
                                      new VertexIndices(7, 1, 5),
                                      new VertexIndices(3, 2, 5),
                                      new VertexIndices(2, 3, 5))
                });

            if (null == mesh)
                throw new Exception("Cube failed");

            meshes = new Geometry.MeshVbo[1];
            meshes[0] = mesh;            
        }
    }
}
