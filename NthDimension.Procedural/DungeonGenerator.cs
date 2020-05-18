using NthDimension.Rendering.Utilities;
using NthDimension.Procedural.Dungeon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural
{
    

    public class DungeonGenerator
    {
        readonly Random                 rand;
        readonly LevelMapTemplate       template;

        RoomCollision                   collision;
        LevelRoom                       rootRoom;
        List<LevelRoom>                 rooms;
        int                             maxDepth;
        int                             minRoomNum;
        int                             maxRoomNum;

        public GenerationStep Step { get; set; }

        public Random Random {  get { return rand; } }

        //public List<LevelRoom> Rooms { get { return rooms; } private set { rooms = value; } }




        public DungeonGenerator(int seed, LevelMapTemplate template)
        {
            rand = new Random(seed);
            this.template = template;
            Step = GenerationStep.Initialize;
        }

        public void Generate(GenerationStep? targetStep = null)
        {
            while (Step != targetStep && Step != GenerationStep.Finish)
            {
                RunStep();
            }

            ConsoleUtil.log(string.Format("\nGenerated Level: {0}", template.ToString()));

            foreach (LevelRoom lvr in rooms)
            {
                ConsoleUtil.log(string.Format("{0}\tBranches {1}\t{2}x{3}m\tPosition:{4}", lvr.Type, lvr.NumBranches, lvr.Length, lvr.Width, lvr.Pos));

            }
        }

        public IEnumerable<LevelRoom> GetRooms()
        {
            return rooms;
        }

        

        void RunStep()
        {
            switch (Step)
            {
                case GenerationStep.Initialize:
                    template.SetRandom(rand);
                    template.Initialize();
                    collision = new RoomCollision();
                    rootRoom = null;
                    rooms = new List<LevelRoom>();
                    break;

                case GenerationStep.TargetGeneration:
                    if (!GenerateTarget())
                    {
                        Step = GenerationStep.Initialize;
                        return;
                    }
                    break;

                case GenerationStep.SpecialGeneration:
                    GenerateSpecials();
                    break;

                case GenerationStep.BranchGeneration:
                    GenerateBranches();
                    break;
            }
            Step++;
        }

        Link? PlaceRoom(LevelRoom src, LevelRoom target, int connPt)
        {
            var sep = template.RoomSeparation.Random(rand);
            if (src is LevelRootRoom && target is LevelRootRoom)
                return PlaceRoomFixed((LevelRootRoom)src, (LevelRootRoom)target, connPt, sep);
            if (src is LevelRootRoom)
                return PlaceRoomSourceFixed((LevelRootRoom)src, target, connPt, sep);
            if (target is LevelRootRoom)
                return PlaceRoomTargetFixed(src, (LevelRootRoom)target, connPt, sep);

            return PlaceRoomFree(src, target, (Direction)connPt, sep);
        }

        Link? PlaceRoomFree(LevelRoom src, LevelRoom target, Direction connPt, int sep)
        {
            int x, y;
            Link? link = null;

            switch (connPt)
            {
                case Direction.North:
                case Direction.South:
                    // North & South
                    int minX = src.Pos.X + template.CorridorWidth - target.Width;
                    int maxX = src.Pos.X + src.Width - template.CorridorWidth;
                    x = rand.Next(minX, maxX + 1);

                    if (connPt == Direction.South)
                        y = src.Pos.Y + src.Length + sep;
                    else
                        y = src.Pos.Y - sep - target.Length;

                    target.Pos = new Point(x, y);
                    if (collision.HitTest(target))
                        return null;

                    var linkX = new Range(src.Pos.X, src.Pos.X + src.Width).Intersection(
                        new Range(target.Pos.X, target.Pos.X + target.Width));
                    link = new Link(connPt, new Range(linkX.Begin, linkX.End - template.CorridorWidth).Random(rand));
                    break;

                case Direction.East:
                case Direction.West:
                    // East & West
                    int minY = src.Pos.Y + template.CorridorWidth - target.Length;
                    int maxY = src.Pos.Y + src.Length - template.CorridorWidth;
                    y = rand.Next(minY, maxY + 1);

                    if (connPt == Direction.East)
                        x = src.Pos.X + src.Width + sep;
                    else
                        x = src.Pos.X - sep - target.Width;

                    target.Pos = new Point(x, y);
                    if (collision.HitTest(target))
                        return null;

                    var linkY = new Range(src.Pos.Y, src.Pos.Y + src.Length).Intersection(
                        new Range(target.Pos.Y, target.Pos.Y + target.Length));
                    link = new Link(connPt, new Range(linkY.Begin, linkY.End - template.CorridorWidth).Random(rand));
                    break;
            }

            collision.Add(target);
            return link;
        }

        Link? PlaceRoomSourceFixed(LevelRootRoom src, LevelRoom target, int connPt, int sep)
        {
            var conn = src.ConnectionPoints[connPt];
            int x, y;
            Link? link = null;

            switch (conn.Item1)
            {
                case Direction.North:
                case Direction.South:
                    // North & South
                    int minX = src.Pos.X + conn.Item2 + template.CorridorWidth - target.Width;
                    int maxX = src.Pos.X + conn.Item2;
                    x = rand.Next(minX, maxX + 1);

                    if (conn.Item1 == Direction.South)
                        y = src.Pos.Y + src.Length + sep;
                    else
                        y = src.Pos.Y - sep - target.Length;

                    target.Pos = new Point(x, y);
                    if (collision.HitTest(target))
                        return null;

                    link = new Link(conn.Item1, src.Pos.X + conn.Item2);
                    break;

                case Direction.East:
                case Direction.West:
                    // East & West
                    int minY = src.Pos.Y + conn.Item2 + template.CorridorWidth - target.Length;
                    int maxY = src.Pos.Y + conn.Item2;
                    y = rand.Next(minY, maxY + 1);

                    if (conn.Item1 == Direction.East)
                        x = src.Pos.X + src.Width + sep;
                    else
                        x = src.Pos.X - sep - target.Width;

                    target.Pos = new Point(x, y);
                    if (collision.HitTest(target))
                        return null;

                    var linkY = new Range(src.Pos.Y, src.Pos.Y + src.Length).Intersection(
                        new Range(target.Pos.Y, target.Pos.Y + target.Length));
                    link = new Link(conn.Item1, src.Pos.Y + conn.Item2);
                    break;
            }

            collision.Add(target);
            return link;
        }

        Link? PlaceRoomTargetFixed(LevelRoom src, LevelRootRoom target, int connPt, int sep)
        {
            var targetDir = ((Direction)connPt).Reverse();

            var connPts = (Tuple<Direction, int>[])target.ConnectionPoints.Clone();
            rand.Shuffle(connPts);
            Tuple<Direction, int> conn = null;
            foreach (var pt in connPts)
            {
                if (pt.Item1 == targetDir)
                {
                    conn = pt;
                    break;
                }
            }

            if (conn == null)
                return null;

            int x, y;
            Link? link = null;
            switch (conn.Item1)
            {
                case Direction.North:
                case Direction.South:
                    // North & South
                    int minX = src.Pos.X - conn.Item2;
                    int maxX = src.Pos.X + src.Width - template.CorridorWidth - conn.Item2;
                    x = rand.Next(minX, maxX + 1);

                    if (conn.Item1 == Direction.North)
                        y = src.Pos.Y + src.Length + sep;
                    else
                        y = src.Pos.Y - sep - target.Length;

                    target.Pos = new Point(x, y);
                    if (collision.HitTest(target))
                        return null;

                    link = new Link((Direction)connPt, target.Pos.X + conn.Item2);
                    break;

                case Direction.East:
                case Direction.West:
                    // East & West
                    int minY = src.Pos.Y - conn.Item2;
                    int maxY = src.Pos.Y + src.Length - template.CorridorWidth - conn.Item2;
                    y = rand.Next(minY, maxY + 1);

                    if (conn.Item1 == Direction.West)
                        x = src.Pos.X + src.Width + sep;
                    else
                        x = src.Pos.X - sep - target.Width;

                    target.Pos = new Point(x, y);
                    if (collision.HitTest(target))
                        return null;

                    link = new Link((Direction)connPt, target.Pos.Y + conn.Item2);
                    break;
            }

            collision.Add(target);
            return link;
        }

        Link? PlaceRoomFixed(LevelRootRoom src, LevelRootRoom target, int connPt, int sep)
        {
            var conn = src.ConnectionPoints[connPt];

            var targetDirection = conn.Item1.Reverse();
            var targetConns = (Tuple<Direction, int>[])target.ConnectionPoints.Clone();
            rand.Shuffle(targetConns);
            Tuple<Direction, int> targetConnPt = null;
            foreach (var targetConn in targetConns)
                if (targetConn.Item1 == targetDirection)
                {
                    targetConnPt = targetConn;
                    break;
                }

            if (targetConnPt == null)
                return null;

            int x, y;
            Link? link = null;
            switch (conn.Item1)
            {
                case Direction.North:
                case Direction.South:
                    // North & South
                    x = src.Pos.X + conn.Item2 - targetConnPt.Item2;

                    if (conn.Item1 == Direction.South)
                        y = src.Pos.Y + src.Length + sep;
                    else
                        y = src.Pos.Y - sep - target.Length;

                    target.Pos = new Point(x, y);
                    if (collision.HitTest(target))
                        return null;

                    link = new Link(conn.Item1, src.Pos.X + conn.Item2);
                    break;

                case Direction.East:
                case Direction.West:
                    // East & West
                    y = src.Pos.Y + conn.Item2 - targetConnPt.Item2;

                    if (conn.Item1 == Direction.East)
                        x = src.Pos.X + src.Width + sep;
                    else
                        x = src.Pos.X - sep - target.Width;

                    target.Pos = new Point(x, y);
                    if (collision.HitTest(target))
                        return null;

                    link = new Link(conn.Item1, src.Pos.Y + conn.Item2);
                    break;
            }

            collision.Add(target);
            return link;
        }

        int GetMaxConnectionPoints(LevelRoom rm)
        {
            if (rm is LevelRootRoom)
                return ((LevelRootRoom)rm).ConnectionPoints.Length;
            return 4;
        }

        bool GenerateTarget()
        {
            var targetDepth = (int)template.TargetDepth.NextValue();

            rootRoom = template.CreateStart(0);
            rootRoom.Pos = new Point(0, 0);
            collision.Add(rootRoom);
            rooms.Add(rootRoom);

            if (GenerateTargetInternal(rootRoom, 1, targetDepth))
            {
                minRoomNum = targetDepth * template.NumRoomRate.Begin;
                maxRoomNum = targetDepth * template.NumRoomRate.End;
                maxDepth = rooms.Count;
                return true;
            }
            return false;
        }

        bool GenerateTargetInternal(LevelRoom prev, int depth, int targetDepth)
        {
            var connPtNum = GetMaxConnectionPoints(prev);
            var seq = Enumerable.Range(0, connPtNum).ToList();
            rand.Shuffle(seq);

            bool targetPlaced;
            do
            {
                LevelRoom rm;
                if (targetDepth == depth)
                    rm = template.CreateTarget(depth, prev);
                else
                    rm = template.CreateNormal(depth, prev);

                Link? link = null;
                foreach (var connPt in seq)
                    if ((link = PlaceRoom(prev, rm, connPt)) != null)
                    {
                        seq.Remove(connPt);
                        break;
                    }

                if (link == null)
                    return false;

                if (targetDepth == depth)
                    targetPlaced = true;
                else
                    targetPlaced = GenerateTargetInternal(rm, depth + 1, targetDepth);

                if (targetPlaced)
                {
                    rm.Depth = depth;
                    Edge.Link(prev, rm, link.Value);
                    rooms.Add(rm);
                }
                else
                    collision.Remove(rm);
            } while (!targetPlaced);
            return true;
        }

        void GenerateSpecials()
        {
            if (template.SpecialRmCount == null)
                return;

            int numRooms = (int)template.SpecialRmCount.NextValue();
            for (int i = 0; i < numRooms; i++)
            {
                int targetDepth;
                do
                {
                    targetDepth = (int)template.SpecialRmDepthDist.NextValue();
                } while (targetDepth > maxDepth * 3 / 2);

                bool generated = false;
                do
                {
                    var room = rooms[rand.Next(rooms.Count)];
                    if (room.Depth >= targetDepth)
                        continue;
                    generated = GenerateSpecialInternal(room, room.Depth + 1, targetDepth);
                } while (!generated);
            }
        }

        bool GenerateSpecialInternal(LevelRoom prev, int depth, int targetDepth)
        {
            var connPtNum = GetMaxConnectionPoints(prev);
            var seq = Enumerable.Range(0, connPtNum).ToList();
            rand.Shuffle(seq);

            bool specialPlaced;
            do
            {
                LevelRoom rm;
                if (targetDepth == depth)
                    rm = template.CreateSpecial(depth, prev);
                else
                    rm = template.CreateNormal(depth, prev);

                Link? link = null;
                foreach (var connPt in seq)
                    if ((link = PlaceRoom(prev, rm, connPt)) != null)
                    {
                        seq.Remove(connPt);
                        break;
                    }

                if (link == null)
                    return false;

                if (targetDepth == depth)
                    specialPlaced = true;
                else
                    specialPlaced = GenerateSpecialInternal(rm, depth + 1, targetDepth);

                if (specialPlaced)
                {
                    rm.Depth = depth;
                    Edge.Link(prev, rm, link.Value);
                    rooms.Add(rm);
                }
                else
                    collision.Remove(rm);
            } while (!specialPlaced);
            return true;
        }

        void GenerateBranches()
        {
            int numRooms = new Range(minRoomNum, maxRoomNum).Random(rand);

            List<LevelRoom> copy;
            while (rooms.Count < numRooms)
            {
                copy = new List<LevelRoom>(rooms);
                rand.Shuffle(copy);

                bool worked = false;
                foreach (var room in copy)
                {
                    if (rooms.Count > numRooms)
                        break;
                    if (rand.Next() % 2 == 0)
                        continue;
                    worked |= GenerateBranchInternal(room, room.Depth + 1, template.MaxDepth, true);
                }
                if (!worked)
                    break;
            }
        }

        bool GenerateBranchInternal(LevelRoom prev, int depth, int maxDepth, bool doBranch)
        {
            if (depth >= maxDepth)
                return false;

            var connPtNum = GetMaxConnectionPoints(prev);
            var seq = Enumerable.Range(0, connPtNum).ToList();
            rand.Shuffle(seq);

            if (doBranch)
            {
                var numBranch = prev.NumBranches.Random(rand);
                numBranch -= prev.Edges.Count;
                for (int i = 0; i < numBranch; i++)
                {
                    var rm = template.CreateNormal(depth, prev);

                    Link? link = null;
                    foreach (var connPt in seq)
                        if ((link = PlaceRoom(prev, rm, connPt)) != null)
                        {
                            seq.Remove(connPt);
                            break;
                        }

                    if (link == null)
                        return false;

                    Edge.Link(prev, rm, link.Value);
                    if (!GenerateBranchInternal(rm, depth + 1, maxDepth, false))
                    {
                        collision.Remove(rm);
                        Edge.UnLink(prev, rm);
                        return false;
                    }
                    rm.Depth = depth;
                    rooms.Add(rm);
                }
            }
            else
            {
                while (prev.Edges.Count < prev.NumBranches.Begin)
                {
                    var rm = template.CreateNormal(depth, prev);

                    Link? link = null;
                    foreach (var connPt in seq)
                        if ((link = PlaceRoom(prev, rm, connPt)) != null)
                        {
                            seq.Remove(connPt);
                            break;
                        }

                    if (link == null)
                        return false;

                    Edge.Link(prev, rm, link.Value);
                    if (!GenerateBranchInternal(rm, depth + 1, maxDepth, false))
                    {
                        collision.Remove(rm);
                        Edge.UnLink(prev, rm);
                        return false;
                    }
                    rm.Depth = depth;
                    rooms.Add(rm);
                }
            }
            return true;
        }

        public LevelGraph ExportGraph()
        {
            if (Step != GenerationStep.Finish)
                throw new InvalidOperationException();
            return new LevelGraph(template, rooms.ToArray());
        }
    }
}
