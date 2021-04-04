using NthDimension.Algebra;
using NthDimension.Rendering.Partition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.City
{
    public partial class RoadNetwork
    {
        //public Rect<Vector4> quadtreeParams = new Rect<Vector4>(-25000, -25000, 50000, 50000);
        public int quadtreeMaxObjects = 10;
        public int quadtreeMaxLevels = 10;
        public int segmentCountLimit = 400;
        public int derivationStepLimit = 400;
        public float streetSegmentLength = 50;
        public float highwaySegmentLength = 120;
        public float streetSegmentWidth = 30;
        public float highwaySegmentWidth = 30;
        public float streetBranchProbability = 0.1f;
        public float highwayBranchProbability = 0.025f;
        public float streetBranchPopulationThreshold = 0.1f;
        public float highwayBranchPopulationThreshold = 0.15f;
        public int streetBranchTimeDelayFromHighway = 5;
        public float minimumIntersectionDeviation = 30;
        public float snapDistance = 30;
        public float allotmentMinHalfDiagonal = 60;
        public float allotmentMaxHalfDiagonal = 80;
        public float allotmentMinAspect = 1;
        public float allotmentMaxAspect = 1.45f;
        public int allotmentPlacementLoopLimit = 3;
        public int settlementSpawnDelay = 10;
        public int settlementDensity = 40;
        public float settlementRadius = 400.0f;
        public float settlementInCrossingProbability = 0.9f;
        public float settlementInHighwayProbability = 0.1f;
        //// Road network
        public bool generateHighways = true;
        public bool generateStreets = true;
        private List<Segment> segments;        
        private QuadTree quadtree;  
        private int mask = 0;
        private bool finished = false;
        private Rect<Vector3> boundingBox;

        public List<Segment>        Segments
        {
            get
            {
                return segments;
            }
        }
        QuadTree                    QuadTree
        {
            get
            {
                return quadtree;
            }
        }
        public Rect<Vector3>        BoundingBox
        {
            get
            {
                return boundingBox;
            }
        }
        public int                  Mask
        {
            get
            {
                return mask;
            }
        }
        public bool                 Finished
        {
            get
            {
                return finished;
            }
        }

        void Start()
        {
            //SetupConfig();

            // ---

            //RoadNetworkGenerator.DebugData debugData;
            //RoadNetworkGenerator.Generate(out segments, out quadtree, out debugData);

            //Debug.Log(segments.Count + " segments");

            // ---

            mask = ((generateHighways) ? RoadNetworkTraversal.HIGHWAYS_MASK : 0) | ((generateStreets) ? RoadNetworkTraversal.STREETS_MASK : 0);

            float minX = float.MaxValue,
                maxX = -float.MaxValue,
                minY = float.MaxValue,
                maxY = -float.MaxValue;
            HashSet<Segment> visited = new HashSet<Segment>();
            foreach (var segment in segments)
            {
                RoadNetworkTraversal.PreOrder(segment, (a) =>
                {
                    minX = System.Math.Min(System.Math.Min(a.Start.X, a.End.X), minX);
                    minY = System.Math.Min(System.Math.Min(a.Start.Y, a.End.Y), minY);
                    maxX = System.Math.Max(System.Math.Max(a.Start.X, a.End.X), maxX);
                    maxY = System.Math.Max(System.Math.Max(a.Start.Y, a.End.Y), maxY);
                    return true;
                }, mask, ref visited);
            }
            Vector2 size = new Vector2(maxX - minX, maxY - minY);
            Vector2 center = new Vector2(minX, minY);
            throw new NotImplementedException("Rect(Vector3 center, Vector3 size)");
            //boundingBox = new Rect<Vector2>(center, size);

            finished = true;
        }

    }

    public class Segment //: ICollidable
    {
        private int index;
        private int roadRevision;                   // Note: incrementation of variable commented-out (see Vector2 Start, Vector2 End)
        private int directionRevision;
        private int lengthRevision;
        //private LineCollider collider;
        private float cachedDirection;
        private float cachedLength;
        private bool highway;
        private bool severed;
        private List<Segment> sources;
        private List<Segment> destinations;
        private List<Segment> branches;
        private List<Segment> forwards;
        private Action setupBranchLinksCallback;

        public Segment(Vector2 start, Vector2 end, bool highway = false, bool severed = false)
        {
            index = -1;
            roadRevision = 0;
            directionRevision = -1;
            lengthRevision = -1;
            cachedDirection = -1;
            cachedLength = -1;
            //float width = highway ? Config.highwaySegmentWidth : Config.streetSegmentWidth;
            //collider = new LineCollider(this, start, end, width);
            sources = new List<Segment>();
            destinations = new List<Segment>();
            branches = new List<Segment>();
            forwards = new List<Segment>();
            setupBranchLinksCallback = null;
            this.highway = highway;
            this.severed = severed;
        }

        private static Vector2 ComputeEnd(Vector2 start, float direction, float length)
        {            
            return new Vector2(
                start.X + length * (float)System.Math.Sin(MathFunc.DegreesToRadians(direction)),
                start.Y + length * (float)System.Math.Cos(MathFunc.DegreesToRadians(direction))
            );
        }

        public Segment(Segment other) : this(other.Start, other.End, other.highway, other.severed)
        {
        }

        public Segment(Vector2 start, float direction, float length, bool highway, bool severed) : this(start, ComputeEnd(start, direction, length), highway, severed)
        {
        }

        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        public Vector2 Start
        {
            get;
            set;
            //get
            //{
            //    return collider.Start;
            //}
            //set
            //{
            //    collider.Start = value;
            //    roadRevision++;
            //}
        }

        public Vector2 End
        {
            get; set;
            //get
            //{
            //    return collider.End;
            //}
            //set
            //{
            //    collider.End = value;
            //    roadRevision++;
            //}
        }

        public float Width
        {
            get; set;
            //get
            //{
            //    return collider.Width;
            //}
            //set
            //{
            //    collider.Width = value;
            //    roadRevision++;
            //}
        }

        public Action SetupBranchLinksCallback
        {
            set
            {
                setupBranchLinksCallback = value;
            }
        }

        public List<Segment> Sources
        {
            get
            {
                return sources;
            }
            set
            {
                sources = value;
            }
        }

        public List<Segment> Destinations
        {
            get
            {
                return destinations;
            }
            set
            {
                destinations = value;
            }
        }

        public List<Segment> Branches
        {
            get
            {
                return branches;
            }
            set
            {
                branches = value;
            }
        }

        public List<Segment> Forwards
        {
            get
            {
                return forwards;
            }
            set
            {
                forwards = value;
            }
        }

        public bool Highway
        {
            get
            {
                return highway;
            }
        }

        public bool Severed
        {
            get
            {
                return severed;
            }
            set
            {
                severed = value;
            }
        }

        public float Direction
        {
            get
            {
                Vector2 direction;
                if (directionRevision != roadRevision)
                {
                    directionRevision = roadRevision;
                    direction = (End - Start);
                    if (Length == 0)
                        cachedDirection = 0;
                    else
                        cachedDirection = (float)MathFunc.RadiansToDegrees(System.Math.Sign(direction.X) * System.Math.Acos(direction.Y / Length));
                }
                return cachedDirection;
            }
        }

        public float Length
        {
            get
            {
                if (lengthRevision != roadRevision)
                {
                    lengthRevision = roadRevision;
                    cachedLength = (End - Start).Length;
                }
                return cachedLength;
            }
        }

        public void SetupBranchLinks()
        {
            if (setupBranchLinksCallback != null)
                setupBranchLinksCallback();
        }

        public List<Segment> LinksForEndContaining(Segment segment)
        {
            if (branches.IndexOf(segment) != -1)
                return branches;
            else if (forwards.IndexOf(segment) != -1)
                return forwards;
            else
                return null;
        }

        public bool StartIsBackwards()
        {
            if (branches.Count > 0)
                return (branches[0].Start == Start) || (branches[0].Start == End);
            else if (forwards.Count > 0)
                return (forwards[0].Start == End) || (forwards[0].End == End);
            else
                return false;
        }

        //public bool Intersect(Segment other, out Vector2 intersection, out float t)
        //{
        //    return Collision.LineSegmentLineSegmentIntersection(Start, End, other.Start, other.End, out intersection, out t);
        //}

        //public Collider GetCollider()
        //{
        //    return collider;
        //}

        public override int GetHashCode()
        {
            return index.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Segment)
                return index == ((Segment)obj).index;
            return false;
        }

    }

    public class RoadNetworkTraversal
    {
        public const int HIGHWAYS_MASK = 1;
        public const int STREETS_MASK = 2;

        public delegate bool Visitor0(Segment s0);
        public delegate bool Visitor1(Segment s0, Segment s1);
        public delegate bool Visitor2<T>(Segment s0, ref T context);
        public delegate bool Visitor3<T, U>(Segment s0, ref T context, U inData, out U outData);
        public delegate bool Visitor4<T, U>(Segment s0, Segment s1, ref T context, U inData, out U outData);

        public static void PreOrder(Segment s0, Visitor0 visitor, int mask, ref HashSet<Segment> visited)
        {
            if (s0.Highway && (mask & HIGHWAYS_MASK) == 0 || !s0.Highway && (mask & STREETS_MASK) == 0)
                return;
            if (visited.Contains(s0))
                return;
            visited.Add(s0);
            if (!visitor(s0))
                return;
            foreach (var destination in s0.Destinations)
                PreOrder(destination, visitor, mask, ref visited);
        }

        public static void PreOrder(Segment s0, Visitor1 visitor, int mask, ref HashSet<Segment> visited)
        {
            PreOrder(null, s0, visitor, mask, ref visited);
        }

        public static void PreOrder(Segment s0, Segment s1, Visitor1 visitor, int mask, ref HashSet<Segment> visited)
        {
            if (s1.Highway && (mask & HIGHWAYS_MASK) == 0 || !s1.Highway && (mask & STREETS_MASK) == 0)
                return;
            if (visited.Contains(s1))
                return;
            visited.Add(s1);
            if (s1.Destinations.Count == 0)
            {
                if (!visitor(s1, null))
                    return;
            }
            else
            {
                if (!visitor(s0, s1))
                    return;
                foreach (var s2 in s1.Destinations)
                    PreOrder(s1, s2, visitor, mask, ref visited);
            }
        }

        public static void PreOrder<T>(Segment s0, ref T context, Visitor2<T> visitor, int mask, ref HashSet<Segment> visited)
        {
            if (s0.Highway && (mask & HIGHWAYS_MASK) == 0 || !s0.Highway && (mask & STREETS_MASK) == 0)
                return;
            if (visited.Contains(s0))
                return;
            visited.Add(s0);
            if (!visitor(s0, ref context))
                return;
            foreach (var destination in s0.Destinations)
                PreOrder(destination, ref context, visitor, mask, ref visited);
        }

        public static void PreOrder<T, U>(Segment s0, ref T context, U inData, Visitor3<T, U> visitor, int mask, ref HashSet<Segment> visited)
        {
            if (s0.Highway && (mask & HIGHWAYS_MASK) == 0 || !s0.Highway && (mask & STREETS_MASK) == 0)
                return;
            if (visited.Contains(s0))
                return;
            visited.Add(s0);
            U outData;
            if (!visitor(s0, ref context, inData, out outData))
                return;
            foreach (var destination in s0.Destinations)
                PreOrder(destination, ref context, outData, visitor, mask, ref visited);
        }

        public static void PreOrder<T, U>(Segment s0, ref T context, U inData, Visitor4<T, U> visitor, int mask, ref HashSet<Segment> visited)
        {
            PreOrder<T, U>(null, s0, ref context, inData, visitor, mask, ref visited);
        }

        public static void PreOrder<T, U>(Segment s0, Segment s1, ref T context, U inData, Visitor4<T, U> visitor, int mask, ref HashSet<Segment> visited)
        {
            if (s1.Highway && (mask & HIGHWAYS_MASK) == 0 || !s1.Highway && (mask & STREETS_MASK) == 0)
                return;
            if (visited.Contains(s1))
                return;
            visited.Add(s1);
            U outData;
            if (s1.Destinations.Count == 0)
            {
                if (!visitor(s1, null, ref context, inData, out outData))
                    return;
            }
            else
            {
                if (!visitor(s0, s1, ref context, inData, out outData))
                    return;
                foreach (var s2 in s1.Destinations)
                    PreOrder(s1, s2, ref context, outData, visitor, mask, ref visited);
            }
        }

    }
}
