using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NthDimension.Procedural.City.Allotment;

namespace NthDimension.Procedural.City
{
    public class Building
    {
        public struct Face
        {
            public List<Vector3> vertices;
            public List<Edge> edges;
        }

        private Allotment m_allotment;
        private float m_height;

        private List<Vector3> m_vertices;
        private List<Vector3> m_normals;
        private List<ushort> m_indices;


        public Building(Allotment allotment, float height)
        {
            this.m_allotment = allotment;
            this.m_height = height;
        }

        public Allotment getAllotment()
        {
            return this.m_allotment;
        }

        public List<Vector3> getVertices()
        {
            //throw new NotImplementedException();
            return this.m_vertices;
        }
        public List<Vector3> getNormals()
        {
            //throw new NotImplementedException();
            return this.m_normals;
        }
        public List<ushort> getIndices()
        {
            //throw new NotImplementedException();
            return this.m_indices;
        }

        void addIndex(ushort index)
        {
            m_indices.Add(index);
        }

        void addVertex(Vector3 vertex)
        {
            m_vertices.Add(vertex);
        }

        void computeNormals()
        {
            //m_normals.resize(m_vertices.size(), D3DXVECTOR3(0.0f, 0.0f, 0.0f));

            for (ushort i = 0; i < m_indices.Count; i += 3)
            {
                Vector3 u = m_vertices[m_indices[i + 1]] - m_vertices[m_indices[i]];
                Vector3 v = m_vertices[m_indices[i + 2]] - m_vertices[m_indices[i]];
                Vector3 w;


                Vector3.Cross(ref u, ref v, out w);             // D3DXVec3Cross(&w, &u, &v);

                if (w.LengthSquared > 0.00000001f)               //if (D3DXVec3LengthSq(&w) > 0.00000001f)
                {
                    Vector3.Normalize(ref w, out w);            // D3DXVec3Normalize(&w, &w);

                    for (ushort j = 0; j < 3; ++j)
                        m_normals[m_indices[i + j]] += w;

                }
            }

            for (ushort i = 0; i < m_normals.Count; ++i)
            {
                //D3DXVec3Normalize(&m_normals[i], &m_normals[i]);
                //Vector3.Normalize(ref m_normals[i], out m_normals[i]);
                m_normals[i].Normalize();
            }
        }
    }
}
