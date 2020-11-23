

namespace NthDimension.Procedural.City
{
    using System;
    using System.Collections.Generic;
    using NthDimension.Procedural.MockTBD;                // WARNING Ref
    using NthDimension.Rendering;
    using NthDimension.Rendering.Drawables.Models;
    using NthDimension.Rendering.Geometry;
    using NthDimension.Rendering.Serialization;
    using static NthDimension.Procedural.City.Road;

    public partial class RoadNetwork : Model, IDisposable
    {
        // 1. Creates Vbo
        // 2. Renders

        private List<Road> m_roads = new List<Road>();
        private List<Crossing> m_crossings = new List<Crossing>();

        public void Dispose()
        {
            m_roads.Clear(); m_roads = null;
            m_crossings.Clear(); m_crossings = null;

        }
        


        public RoadNetwork()
        {
            this.PrimitiveType = Rasterizer.PrimitiveType.LineStrip;
        }

        public void addRoad(Road road)
        {
            this.m_roads.Add(road);
        }
        public void addCrossing(Crossing crossing)
        {
            this.m_crossings.Add(crossing);
        }
        public Road[] divideRoad(ref Road road, ref Crossing crossing)  
        {
            Crossing end = road.getEndCrossing();
            road.setCrossing(end, crossing);
            end.removeRoad(road);
            crossing.addRoad(road);
            
            Road newRoad = /*new (__FILE__, __LINE__)*/ new Road(crossing, end, road.getType());
            crossing.addRoad(newRoad);
            end.addRoad(newRoad);

            //return newRoad; // Original?

            return new Road[2] {    // TBT
                road,
                newRoad
            };
        }
        public void sortRoadsOnCrossing()
        {
            for (int i = 0; i < m_crossings.Count; ++i)
            {
                m_crossings[i].sortRoad();
            }
        }
        public void removeOneway()
        {
            for (int i = 0; i < m_crossings.Count; ++i)
            {
                int index = i;
                while (m_crossings[index].getRoads().Length == 1)
                {
                    Road r = m_crossings[index].getRoads()[0];
                    Crossing other = r.getEndCrossing(m_crossings[index]);
                    r.detach();                    
                    m_roads.Remove(r);
                    //if (r != null)
                    //{
                    //    r.Dispose();
                    //}
                    //if (m_crossings[index] != null)
                    //{
                    //    m_crossings[index].Dispose();
                    //}
                    m_crossings.RemoveAt(index);
                    --i;

                    index = m_crossings.IndexOf(other);// std::find(m_crossings.GetEnumerator(), m_crossings.end(), other) - m_crossings.GetEnumerator();
                }
            }
        }
        public void commitChange()
        { 
            recomputeBuffer(); 
        }


        public void reset()
        {
            this.m_roads.Clear();
            this.m_crossings.Clear();
            this.recomputeBuffer();
        }

        protected void recomputeBuffer() 
        {
            //HashMap < Tuple<Crossing, ERoadType>, uint, 1024 > crossIndex = new HashMap<Tuple<Crossing, ERoadType>, uint, 1024>();
            Dictionary<Tuple<Crossing, ERoadType>, int> crossIndex = new Dictionary<Tuple<Crossing, ERoadType>, int>(1024);
            
            int nbCrossing = 0;

            //m_vb = null;
            //m_ib = null;
            MeshVbo m_vb = null;
            int[] m_ib = null;

            if (m_roads.Count == 0)
            {
                //m_vb = new MeshVbo();
                //m_ib = new int[] { };
            }
            else
            {
                foreach (Road it in m_roads)
                {
                    ERoadType type = it.getType();
                    Tuple<Crossing, ERoadType> pair = new Tuple<Crossing, ERoadType>(it.getStartCrossing(), type);
                    if (!crossIndex.ContainsKey(pair))
                    {
                        crossIndex[pair] = nbCrossing++;
                    }

                    throw new NotImplementedException("pair.Item1 = it.getEndCrossing()");
//pair.Item1 = it.getEndCrossing();
                    
                    if (!crossIndex.ContainsKey(pair))
                    {
                        crossIndex[pair] = nbCrossing++;
                    }
                }

                //m_vb = m_media.getRenderer().createVertexBuffer<Vertex>(nbCrossing);
                //m_ib = m_media.getRenderer().createIndexBuffer<ushort>((uint)(2 * m_roads.Count));
                m_vb = new MeshVbo();
                m_ib = new int[] { };

                ListVertex vertices = new ListVertex(); //Vertex[] vertices = m_vb.@lock();
                int[] indices = m_ib;

                


                for (int i = 0; i < m_roads.Count; ++i)
                {
                    ERoadType type = m_roads[i].getType();
                    Crossing c1 = m_roads[i].getStartCrossing();
                    Crossing c2 = m_roads[i].getEndCrossing();

                    throw new NotImplementedException("crossIndex");
//Tuple<Crossing, ERoadType> key1 = new Tuple<Crossing, ERoadType>(c1, type);
//Tuple<Crossing, ERoadType> key2 = new Tuple<Crossing, ERoadType>(c2, type);

//int i1 = -1;

//if (!crossIndex.ContainsKey(key1))
//{
//    crossIndex.Add(key1, crossIndex.Count);
//    i
//}


//int i1 = crossIndex.ContainsKey(key1) ?
//           crossIndex[key1] : crossIndex.Add(key1, crossIndex.Count);

//uint i1 = crossIndex[new Tuple<Crossing, ERoadType>(c1, type)];
//uint i2 = crossIndex[new Tuple<Crossing*, ERoadType>(c2, type)];

////C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
////ORIGINAL LINE: vertices[i1].position = c1->getPosition();
//vertices[i1].position.CopyFrom(c1.getPosition());
////C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
////ORIGINAL LINE: vertices[i2].position = c2->getPosition();
//vertices[i2].position.CopyFrom(c2.getPosition());

//if (type == ERoadType.RT_STREET)
//{
//    vertices[i1].color = new D3DXVECTOR3(0.5f, 0.5f, 0.5f);
//    vertices[i2].color = new D3DXVECTOR3(0.5f, 0.5f, 0.5f);
//}
//else
//{
//    vertices[i1].color = new D3DXVECTOR3(1.0f, 1.0f, 1.0f);
//    vertices[i2].color = new D3DXVECTOR3(1.0f, 1.0f, 1.0f);
//}

//indices[2 * i] = (ushort)i1;
//indices[2 * i + 1] = (ushort)i2;
                }

                //m_ib.unlock();
                //m_vb.unlock();
            }
        }

      
        public Road[] getRoads()
        {
            return m_roads.ToArray();
        }

        public Crossing[] getCrossings()
        {
            return this.m_crossings.ToArray();
        }

     
    }
}
