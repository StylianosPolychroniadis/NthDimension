using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Drawables.Models
{
    public class GridPlane : Model
    {
        public enum enuGridPlane
        {
            XZPlane = 0,
            XYPlane = 1,
            ZYPlane = 2,
            AllPlanes = -1
        }

        private List<Vector3>   m_vertices;
        private List<Vector3>   m_normals;      // todo: interesting research grid w lighting
        private List<int>       m_indices;
        private List<Vector2>   m_uv;
        private float           m_gridSize;
        private float           m_gridStep;
        private enuGridPlane    m_gridPlane;
        private float           m_originX;      // todo: make grid placement (world position) user-defined
        private float           m_originY;
        private float           m_originZ;


        public GridPlane(ApplicationObject parent, 
                         float size = 100.0F, 
                         float step = 10.0F, 
                         enuGridPlane plane = enuGridPlane.XYPlane,
                         float x = 0, 
                         float y = 0,
                         float z = 0)
            :base(parent)
        {
            // TODO:: 

            this.PrimitiveType  = Rasterizer.PrimitiveType.Lines;
            this.m_vertices     = new List<Vector3>();
            this.m_normals      = new List<Vector3>();
            this.m_indices      = new List<int>();
            this.m_uv           = new List<Vector2>();

            this.m_gridSize     = size;
            this.m_gridStep     = step;
            this.m_gridPlane    = plane;

            this.createGeometry();

            List<Geometry.Face> gridLines = new List<Geometry.Face>();

            Geometry.MeshVbo mGrid = ApplicationBase.Instance.MeshLoader.FromMesh(m_vertices.ToArray(),
                                                                                  m_normals.ToArray(),
                                                                                  m_uv.ToArray(),
                                                                                  gridLines.ToArray());
        }

        private void createGeometry()
        {
            m_vertices.Clear();
            m_indices.Clear();

            float step  = m_gridSize / m_gridStep;
            float hsize = m_gridSize - m_gridStep;

            float Xa    = 0f;
            float Ya    = 0f;
            float Za    = 0f;
            float Xb    = 0f;
            float Yb    = 0f;
            float Zb    = 0f;
            float Xc    = 0f;
            float Yc    = 0f;
            float Zc    = 0f;
            float Xd    = 0f;
            float Yd    = 0f;
            float Zd    = 0f;

            for (float i = -hsize; i < hsize; i++)
            {              
                #region Define Plane
                switch (m_gridPlane)
                {
                    case enuGridPlane.XZPlane:
                        Xa = (float)i;
                        Ya = 0f;
                        Za = (float)-hsize;

                        Xb = (float)i; // Xa
                        Yb = 0f;
                        Zb = (float)hsize;

                        Xc = (float)-hsize;
                        Yc = 0f;
                        Zc = (float)i;

                        Xd = (float)hsize;
                        Yd = 0f;
                        Zd = (float)i;
                        break;
                    case enuGridPlane.XYPlane:
                        Xa = (float)i;
                        Ya = (float)-hsize;
                        Za = 0f;

                        Xb = (float)i;
                        Yb = (float)hsize;
                        Zb = 0f;

                        Xc = (float)-hsize;
                        Yc = (float)i;
                        Zc = 0f;

                        Xd = (float)hsize;
                        Yd = (float)i;
                        Zd = 0f;
                        break;
                    case enuGridPlane.ZYPlane:
                        Xa = 0f;
                        Ya = (float)-hsize;
                        Za = (float)i;

                        Xb = 0f;
                        Yb = (float)hsize;
                        Zb = (float)i;

                        Xc = 0f;
                        Yc = (float)i;
                        Zc = (float)-hsize;

                        Xd = 0f;
                        Yd = (float)i;
                        Zd = (float)hsize;
                        break;
                }
                #endregion

                #region Enabled Main Grid
                if ((float)i != 0)
                {
                    m_vertices.Add(new Vector3(Xa, Ya, Za));
                    m_vertices.Add(new Vector3(Xb, Yb, Zb));
                    m_vertices.Add(new Vector3(Xc, Yc, Zc));
                    m_vertices.Add(new Vector3(Xd, Yd, Zd));
                }
                #endregion
            }

            #region Origin Axes
            float originY = 0f;
         
            //_renderer.Color4(GridColorOrigin);
            // TODO:: Another Mesh?

            switch (m_gridPlane)
            {
                case enuGridPlane.XZPlane:
                    m_vertices.Add(new Vector3(0, originY, -hsize));
                    m_vertices.Add(new Vector3(0, originY, 0));
                    m_vertices.Add(new Vector3(-hsize, originY, 0));
                    m_vertices.Add(new Vector3(0, originY, 0));
                    break;
                case enuGridPlane.XYPlane:
                    m_vertices.Add(new Vector3(originY, -hsize, 0));
                    m_vertices.Add(new Vector3(originY, hsize, 0));
                    m_vertices.Add(new Vector3(-hsize, originY, 0));
                    m_vertices.Add(new Vector3(hsize, originY, 0));
                    break;
                case enuGridPlane.ZYPlane:
                    m_vertices.Add(new Vector3(0, -hsize, originY));
                    m_vertices.Add(new Vector3(0, hsize, originY));
                    m_vertices.Add(new Vector3(0, originY, -hsize));
                    m_vertices.Add(new Vector3(0, originY, hsize));
                    break;
            }
            #endregion
        }

    }
}
