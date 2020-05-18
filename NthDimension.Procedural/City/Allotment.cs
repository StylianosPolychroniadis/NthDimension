using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Algebra;

namespace NthDimension.Procedural.City
{
    public class Allotment
    {
        public struct Edge
        {
            public int          index;
            public Vector3      n;
            public Vector3      v1;
            public Vector3      v2;
            public Vector3      vector;
            public float        length;
            public bool         marker;
        }

        private ZoneType        m_type;
        private List<Vector3>   m_vertices  = new List<Vector3>();
        private List<bool>      m_markers   = new List<bool>();
        private List<Edge>      m_edges     = new List<Edge>();

        private Vector3 m_gravityCenter;
        private Vector3 m_position;


        public Allotment(ZoneType type, Vector3[] vertices)
        {
            this.m_type             = type;
            this.m_vertices.AddRange(vertices);

            int size                = m_vertices.Count();


            for (int i = 0; i < m_vertices.Count; i++)
            {
                Edge nEdge = new Edge();

                nEdge.index = i;
                nEdge.marker = false;
                nEdge.v1 = vertices[i % size];
                nEdge.v2 = vertices[(i + 1) % size];
                nEdge.vector = nEdge.v2 - nEdge.v1;
                nEdge.length = nEdge.vector.Length;

                Vector3 pV = new Vector3(nEdge.vector.Z, 0.0f, -nEdge.vector.X);

                nEdge.n = pV.Normalized(); //D3DXVec3Normalize(out &m_edges[i].n, in &pV);  // TBD DX
                
            }

            m_position = gravityCenter();

        }
        public Allotment(ZoneType type, Vector3[] vertices, bool[] markers) 
            : this(type, vertices)
        {
            this.m_markers = new List<bool>();
            this.m_markers.AddRange(markers);

            int size = m_vertices.Count;
          
            for (int i = 0; i < size; i++)
            {
                Edge nEdge = new Edge();

                nEdge.index = i;
                nEdge.marker = markers[i];
                nEdge.v1 = vertices[i % size];
                nEdge.v2 = vertices[(i + 1) % size];
                nEdge.vector = nEdge.v2 - nEdge.v1;
                nEdge.length = nEdge.vector.Length;     // D3DXVec3Length(&nEdge.vector);

                Vector3 pV = new Vector3(nEdge.vector.Z, 0.0f, -nEdge.vector.X);
                
                nEdge.n = pV.Normalized(); //D3DXVec3Normalize(&nEdge.n, &pV);
            }
            
            m_position = gravityCenter();
        }

        Vector3 gravityCenter()
        {
            Vector3 sumA1Ai = new Vector3(0f, 0f, 0f);
            for(int i = 0; i < m_vertices.Count; i++)
            {
                sumA1Ai += new Vector3(m_vertices[i].X, 0f, m_vertices[i].Z);
            }

            return sumA1Ai / m_vertices.Count;
        }

        //#region Convert to Properties
        public List<Edge> getEdges()
        {
            return this.m_edges;
        }
        public Vector3[] getVertices()
        {
            return this.m_vertices.ToArray();
        }
        public Vector3 getPosition()
        {
            return this.m_position;
        }
        public ZoneType getZoneType()
        {
            return this.m_type;
        }
        public void setZoneType(ZoneType ztype)
        {
            this.m_type = ztype;
        }
        //#endregion

        public void addVertex(Vector3 vertex) { }
        public void addEdge(Edge edge) { }
        public bool isIn()
        {
            bool res = false;
            for(int i =0; i < m_edges.Count; i++)            
                if (!m_edges[i].marker) return false;
            
            return true;          
        }

        

    }
}
