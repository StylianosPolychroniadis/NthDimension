// This is a DRAWABLE CLASS - In the original creates the VBO IBO and performs the Render call

namespace NthDimension.Procedural.City
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Security.Policy;
    using NthDimension.Algebra;
    using NthDimension.Procedural.MockTBD;            // WARNING Ref

    public class BuildingSet
    {
        private const int NB_PATCHES_BUILDINGSET    = 4;
        private const int GRID_PRECISION            = 7;
        
        public struct Vertex
        {
            public Vector3 Position;
            public Vector3 Normal;
        }

        public class Patch                         // WARNING Ref // Warning converted from struct to class for ctor requirement
        {
            public VertexBuffer<Vertex>     vb;
            public IndexBuffer<ushort>      ib;

            public List<Building>           buildings;
            public List<Building>           newBuildings;

            public ushort                   nbVertices;
            public ushort                   nbIndices;

            public Patch()                  // See BuildingSet.h
            {

            }
        }

        public class Shader { }                     // WARNING Ref

        private float                       m_height;
        private Shader                      m_shader;
        private List<Building>              m_buildings     = new List<Building>();
        private Patch[]                     m_patches       = new Patch[(int)ZoneType.NB_ZONE_TYPE * NB_PATCHES_BUILDINGSET * NB_PATCHES_BUILDINGSET];

        private Vector3[]                   zoneColor       = new Vector3[(int)ZoneType.NB_ZONE_TYPE]
        {
            new Vector3(0.5f, 0.3f, 0.8f),
            new Vector3(1.0f, 0.4f, 0.0f),
            new Vector3(0.3f, 0.5f, 0.8f),
            new Vector3(0.0f, 0.5f, 0.0f),
            new Vector3(0.5f, 0.5f, 0.5f)
        };
            


        public void addBuilding(Building b, ZoneType type)
        {
            Vector3[] bVerts = b.getVertices().ToArray();
            Vector3 p = bVerts[0];


            int i = (int)p.X / ((1 << GRID_PRECISION) * NB_PATCHES_BUILDINGSET);
            int j = (int)p.Z / ((1 << GRID_PRECISION) * NB_PATCHES_BUILDINGSET);

            this.m_patches[i + j * NB_PATCHES_BUILDINGSET + (int)type * NB_PATCHES_BUILDINGSET * NB_PATCHES_BUILDINGSET].newBuildings.Add(b);
            this.m_buildings.Add(b);
        }

        public void commitChange()
        {
            this.recomputeBuffer();
        }

		public void recomputeBuffer()
		{
			//for (int i = 0; i < (int)ZoneType.NB_ZONE_TYPE * NB_PATCHES_BUILDINGSET * NB_PATCHES_BUILDINGSET; ++i)
			//{
			//	List<Building> newBuildings = m_patches[i].newBuildings;
			//	List<Building> buildings = m_patches[i].buildings;

			//	if (buildings.Count == 0 && newBuildings.Count == 0)
			//	{
			//		m_patches[i].vb = null;
			//		m_patches[i].ib = null;
			//	}
			//	else if (newBuildings.Count > 0)
			//	{
			//		ushort nbVertices = 0;
			//		ushort nbIndices = 0;
			//		foreach (Building it in newBuildings)
			//		{
			//			nbVertices += (ushort)(it.getVertices().Count);
			//			nbIndices += (ushort)(it.getIndices().Count);
			//		}

			//		if (m_patches[i].vb == null || m_patches[i].vb.getSize() < (uint)(m_patches[i].nbVertices + nbVertices))
			//		{
			//			uint size = ((uint)(m_patches[i].nbVertices + nbVertices) * 3) / 2;

			//			//Assert(size < 65536);

			//			m_patches[i].vb = null;

			//			//m_patches[i].vb = m_media.getRenderer().createVertexBuffer<Vertex>(size);

			//			Vertex vertices = m_patches[i].vb.@lock(0, m_patches[i].nbVertices);

			//			foreach (Building it in buildings)
			//			{
			//				for (uint j = 0; j < it.getVertices().Count; ++j)
			//				{
			//					vertices.Position = it.getVertices()[j];
			//					vertices.Normal = it.getNormals()[j];
			//					++vertices;
			//				}
			//			}

			//			m_patches[i].vb.unlock();
			//		}

			//		if (m_patches[i].ib == null || m_patches[i].ib.getSize() < (uint)(m_patches[i].nbIndices + nbIndices))
			//		{
			//			int size = ((m_patches[i].nbIndices + nbIndices) * 3) / 2;

			//			//Assert(size < 65536);

			//			m_patches[i].ib = null;

			//			m_patches[i].ib = m_media.getRenderer().createIndexBuffer<ushort>(size);

			//			int indices = m_patches[i].ib.@lock(0, m_patches[i].nbIndices);

			//			int startIndices = 0;
			//			foreach (Building it in buildings)
			//			{
			//				foreach (int it2 in it.getIndices())
			//				{
			//					indices = startIndices + it2;
			//					++indices;
			//				}

			//				startIndices += (ushort)(it.getVertices().size());
			//			}

			//			m_patches[i].ib.unlock();
			//		}

			//		Vertex vertices = m_patches[i].vb.@lock(m_patches[i].nbVertices, nbVertices);
			//		int indices = m_patches[i].ib.@lock(m_patches[i].nbIndices, nbIndices);

			//		int startIndices = m_patches[i].nbVertices;
			//		foreach (Building it in newBuildings)
			//		{
			//			for (int j = 0; j < it.getVertices().Count; ++j)
			//			{
			//				vertices.Position = it.getVertices()[j];
			//				vertices.Normal = it.getNormals()[j];
			//				++vertices;
			//			}

			//			foreach (int it2 in it.getIndices())
			//			{
			//				indices = startIndices + it2;
			//				++indices;
			//			}

			//			startIndices += (it.getVertices().Count);

			//			buildings.Add(it);
			//		}

			//		m_patches[i].vb.unlock();
			//		m_patches[i].ib.unlock();

			//		m_patches[i].nbVertices += nbVertices;
			//		m_patches[i].nbIndices += nbIndices;
			//		newBuildings.Clear();
			//	}
			//}

		}

        public void reset()
        {
            for(int i = 0; i < (int)ZoneType.NB_ZONE_TYPE * NB_PATCHES_BUILDINGSET*NB_PATCHES_BUILDINGSET; ++i)
            {
                this.m_patches[i].buildings.Clear();
                this.m_patches[i].newBuildings.Clear();
                this.m_patches[i].nbIndices = 0;
                this.m_patches[i].nbVertices = 0;
            }
            this.m_buildings.Clear();
            this.recomputeBuffer();
        }

        public void setHeight(float height)
        {
            this.m_height = height;
        }


    }
}
