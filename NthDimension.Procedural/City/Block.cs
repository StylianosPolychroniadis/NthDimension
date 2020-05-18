
using System;
using System.Collections.Generic;

namespace NthDimension.Procedural.City
{
    using NthDimension.Algebra;

    public class Block
    {
        private List<Vector3>       m_vertices = new List<Vector3>();
        private ZoneType            m_type;
#pragma warning disable CS0649
        private List<Building>      m_buildings = new List<Building>();

        public Block(Vector3[] vertices, ZoneType type)
        {
            this.m_type = type;
            this.m_vertices = new List<Vector3>();
            this.m_vertices.AddRange(vertices);

            Vector3 lastPoint = m_vertices[0];            
            m_vertices.Add(vertices[0]);

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 diff = new Vector3(vertices[i] - lastPoint);
                if(diff.LengthSquared > 0.000001f) // or float.Epsilon ?
                {
                    lastPoint = vertices[i];                    
                    m_vertices.Add(lastPoint);
                }
            }        
        }
        ~Block()
        {
            //for (std::vector<Building*>::iterator it = m_buildings.begin(); it != m_buildings.end(); ++it)
            //{
            //    delete* it;
            //}
            m_buildings.Clear();
        }

       // #region TODO:: Convert to Properties

        public Vector3[] getVertices()
        {
            return this.m_vertices.ToArray();
        }

        public void setType(ZoneType type)
        {
            this.m_type = type;
        }
        public ZoneType getZoneType()
        {
            return this.m_type;
        }

        public Building[] getBuildings()
        {
            return this.m_buildings.ToArray();
        }
        //#endregion

        public void addVertex(Vector3 vertex)
        {
            m_vertices.Add(vertex);
        }
        public void addBuilding(Building building)
        {
            m_buildings.Add(building);
        }
        public bool pointInside(Vector3 point)
        {
            float sum = 0.0f;
            for(int i = 0; i < m_vertices.Count; i++)
            {
                Vector2 vOP1 = new Vector2(m_vertices[i].X, m_vertices[i].Z) - new Vector2(point.X, point.Z);
                Vector2 vOP2 = new Vector2(m_vertices[(i + 1) % m_vertices.Count].X, m_vertices[(i + 1) % m_vertices.Count].Z) - new Vector2(point.X, point.Z);
                vOP1.Normalize();
                vOP2.Normalize();

                float angle = Vector2.Dot(vOP1, vOP2);
                angle = (angle > 1.0f) ? 1.0f : (angle < -1.0f) ? -1.0f : angle;
                angle = (float)Math.Acos(angle);

                float ccw = (vOP1.X * vOP2.Y - vOP1.Y * vOP2.X);
                if (!Vector2.IsCCW(vOP1, vOP2))
                    angle = -angle;
                sum += angle;
            }

            return Math.Abs(sum) >= 2 * MathHelper.Pi;

            #region C++
            //float sum = 0.0f;
            //for (unsigned i = 0; i < m_vertices.size(); i++)
            //{
            //    D3DXVECTOR2 vOP1 = D3DXVECTOR2(m_vertices[i].x, m_vertices[i].z) - point,
            //                vOP2 = D3DXVECTOR2(m_vertices[(i + 1) % m_vertices.size()].x, m_vertices[(i + 1) % m_vertices.size()].z) - point;
            //    D3DXVec2Normalize(&vOP1, &vOP1);
            //    D3DXVec2Normalize(&vOP2, &vOP2);

            //    float angle = D3DXVec2Dot(&vOP1, &vOP2);
            //    angle = (angle > 1.0f) ? 1.0f : (angle < -1.0f) ? -1.0f : angle;
            //    angle = std::acos(angle);
            //    if (D3DXVec2CCW(&vOP1, &vOP2) < 0.0f)
            //    {
            //        angle = -angle;
            //    }
            //    sum += angle;
            //}

            //return std::abs(sum) >= 2 * D3DX_PI;
            #endregion
        }
    }
}
