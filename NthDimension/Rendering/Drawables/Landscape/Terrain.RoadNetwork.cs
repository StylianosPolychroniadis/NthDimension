using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Drawables.Models
{
    public partial class Terrain
    {
		public enum ERoadType
		{
			RT_STREET,
			RT_HIGHWAY
		}

		public enum ERoadZoneType
		{
			RZT_UNDEFINED = 0x01,
			RZT_COMMERCIAL = 0x02,
			RZT_RESIDENTIAL = 0X04
		}

		private float					m_streetLength				= 5.0f;
		private float					m_streetRange				= 0.2f;
		private float					m_highwayLength				= 30.0f;
		private float					m_highwayRange				= 0.4f;
		private float					m_probHighwayChangeAngle	= 0.3f;
		private float					m_probStreetChangeLength	= 0.02f;
		private float					m_probStreetBranch			= 0.4f;
		private float					m_mergeDistance				= 0.7f;
		private float					m_densityTreshold			= 0.2f;
		private float					m_maxBridgeLength			= 20f;

		#region Road
		public class Road
		{
			private const float STREET_WIDTH = 0.4f;
			private const float HIGWAY_WIDTH = 0.9f;


			private Crossing m_crossing1;
			private Crossing m_crossing2;
			private ERoadType m_type;
			private ERoadZoneType m_zoneType;
			private bool m_isBridge;

			public Road(Crossing c1, Crossing c2, ERoadType type)
			{
				this.m_crossing1 = c1;
				this.m_crossing2 = c2;
				this.m_type = type;
				this.m_zoneType = ERoadZoneType.RZT_UNDEFINED;
				this.m_isBridge = false;
			}

			public float getWidth()
			{
				return m_type == ERoadType.RT_STREET ? STREET_WIDTH : HIGWAY_WIDTH;
			}
			public float get2dLength()
			{
				return get2dVector().Length;
			}
			public Vector2 get2dVector()
			{
				return m_crossing2.getPosition2d() - m_crossing1.getPosition2d();
			}
			public Vector3 get3dVector(Crossing start)
			{
				return getEndCrossing(start).getPosition() - start.getPosition();
			}
			public ERoadType getRoadType()
			{
				return m_type;
			}
			public Crossing getEndCrossing(Crossing startCrossing)
			{
				if (startCrossing == m_crossing1)
					return m_crossing2;
				if (startCrossing == m_crossing2)
					return m_crossing1;

				throw new Exception("Crossing not on road");
			}
			public Crossing getStartCrossing()
			{
				return m_crossing1;
			}
			public Crossing getEndCrossing()
			{
				return m_crossing2;
			}
			public void setCrossing(Crossing oldCrossing, Crossing newCrossing)
			{
				if (oldCrossing == m_crossing1)
					m_crossing1 = newCrossing;
				else if (oldCrossing == m_crossing2)
					m_crossing2 = newCrossing;
				else
					throw new Exception("Crossing not on road");
			}
			public void detach()
			{
				m_crossing1.removeRoad(this);
				m_crossing2.removeRoad(this);
			}
			public bool isBridge()
			{
				return m_isBridge;
			}
			public void setBridge()
			{
				m_isBridge = true;
			}
			public void addZoneType(ERoadZoneType type)
			{
				m_zoneType |= type;
			}
			public ERoadZoneType getZoneType()
			{
				return m_zoneType;
			}
		}
        #endregion Road

        #region RoadAttrib
		public class RoadAttrib
		{
			public int Delay;
			public float Data;

			public RoadAttrib(int delay, float data)
			{
				this.Delay = delay;
				this.Data = data;
			}
		}
        #endregion RoadAttrib

        #region RoadAngleComp
        public class RoadAngleComp
		{
			readonly Crossing m_start;
			public RoadAngleComp(Crossing start)
			{
				this.m_start = start;
			}
			public float getRoadAngle(Road r)
			{
				Vector2 vec = r.get2dVector().Normalized();
				if (r.getStartCrossing() != m_start)
					vec = -vec;
				MathFunc.Clamp(vec.X, -1.0f, 1.0f);
				float angle = (float)System.Math.Acos(vec.X);
				return vec.Y < 0.0f ? -angle : angle;
			}

			public  bool functorMethod(Road r1, Road r2)
			{
				return getRoadAngle(r1) < getRoadAngle(r2);
			}
		}
        #endregion RoadAngleComp

        #region Crossing
        public class Crossing
		{
			private Vector3 m_position = new Vector3();
			private readonly List<Road> m_roads = new List<Road>();
			private List<Vector3> m_junctionPositions = new List<Vector3>();

			public Crossing(Vector3 pos)
			{
				this.m_position = pos;
			}

			public void addRoad(Road r)
			{
				this.m_roads.Add(r);
			}
			public void removeRoad(Road r)
			{
				this.m_roads.Remove(r);
			}
			public Vector3 getPosition()
			{
				return m_position;
			}
			public Vector2 getPosition2d()
			{
				return new Vector2(m_position.X, m_position.Z);
			}
			public List<Road> getRoads()
			{
				return new List<Road>(m_roads);
			}
			public void setPosition(Vector3 newPos)
			{
				this.m_position = newPos;
			}
			public bool isNeighbor(Crossing c)
			{
				foreach (Road r in m_roads)
					if (r.getEndCrossing(this) == c)
						return true;

				return false;
			}
			public bool isNeighbor(Road c)
			{
				foreach (Road r in m_roads)
					if (r == c)
						return true;

				return false;
			}
			public bool hasColinearRoad(Road r, float cosThreshold)
			{
				Vector2 dir = r.get2dVector().Normalized();
				
				if(cosThreshold < 0.0f)
				{
					dir = -dir;
					cosThreshold = -cosThreshold;
				}

				foreach(Road road in this.m_roads)
				{
					if (road == r) continue;
					Vector2 dir2 = road.get2dVector().Normalized();

					if(this != road.getStartCrossing())
					{
						dir2 = -dir2;
						if (Vector2.Dot(dir, dir2) > cosThreshold)
							return true;
					}
				}

				return false;
			}
			public int getNbStreet()
			{
				int nb = 0;
				foreach (Road r in m_roads)
					if (r.getRoadType() == ERoadType.RT_STREET)
						++nb;
				return nb;
			}
			public void sortRoad()
			{
				throw new NotImplementedException();
				//std::sort<List<const Road>.Enumerator, const RoadAngleComp &>(m_roads.GetEnumerator(), m_roads.end(), new RoadAngleComp(this));
			}
			public int getRoadIndex(Road r)
			{
				return m_roads.IndexOf(r);
			}
			public void setJunctionPositions(Vector3[] positions)
			{
				this.m_junctionPositions.Clear();
				this.m_junctionPositions.AddRange(positions);
			}
			public Vector3 getJunctionPosition(int index)
			{
				return m_junctionPositions[index];
			}

		}
        #endregion Crossing

        #region RoadNetwork
        public class RoadNetwork
		{
			private List<Road>			m_roads						= new List<Road>();
			private List<Crossing>		m_crossings					= new List<Crossing>();

			public void addRoad(Road road)
			{
				this.m_roads.Add(road);
			}
			public void addCrossing(Crossing c)
			{
				this.m_crossings.Add(c);
			}
			public Road divideRoad(Road road, Crossing c)
			{
				Crossing end = road.getEndCrossing();
				road.setCrossing(end, c);
				end.removeRoad(road);
				c.addRoad(road);

				Road newRoad = new Road(c, end, road.getRoadType());
				c.addRoad(newRoad);
				end.addRoad(newRoad);

				return newRoad;
			}
			public List<Road> getRoads()
			{
				return new List<Road>(m_roads);
			}
			public List<Crossing> getCrossings()
			{
				return new List<Crossing>(m_crossings);
			}
			public void sortRoadsOnCrossing()
			{
				foreach (Crossing c in m_crossings)
					c.sortRoad();
			}
			public void removeOneWay()
			{
				for (int i = 0; i < m_crossings.Count; ++i)
				{
					int index = i;
					while (m_crossings[index].getRoads().Count == 1)
					{
						Road r = m_crossings[index].getRoads()[0];
						Crossing other = r.getEndCrossing(m_crossings[index]);
						r.detach();
						m_roads.Remove(r);//.erase(std::find(m_roads.GetEnumerator(), m_roads.end(), r));
						if (r != null)
						{
							//r.Dispose();
						}
						if (m_crossings[index] != null)
						{
							//m_crossings[index].Dispose();
						}
						m_crossings.RemoveAt(index);
						--i;

						index = m_crossings.IndexOf(other);//std::find(m_crossings.GetEnumerator(), m_crossings.end(), other) - m_crossings.GetEnumerator();
					}
				}
			}
		}
        #endregion RoadNetwork
    }
}
