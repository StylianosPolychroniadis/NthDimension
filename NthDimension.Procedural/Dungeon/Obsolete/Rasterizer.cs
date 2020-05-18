using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon
{
    public enum RasterizationStep
    {
        Initialize = 5,

        Background = 6,
        Corridor = 7,
        Room = 8,
        Overlay = 9,

        Finish = 10
    }

    public class Rasterizer
    {
        readonly Random rand;
        readonly LevelGraph graph;
        readonly BitmapRasterizer<LevelTile> rasterizer;

        static readonly LevelTileType Space = new LevelTileType(0x00fe, "Space");

        public RasterizationStep Step { get; set; }

        public Rasterizer(int seed, LevelGraph graph)
        {
            rand = new Random(seed);
            this.graph = graph;
            rasterizer = new BitmapRasterizer<LevelTile>(graph.Width, graph.Height);
            Step = RasterizationStep.Initialize;
        }

        public void Rasterize(RasterizationStep? targetStep = null)
        {
            while (Step != targetStep && Step != RasterizationStep.Finish)
            {
                RunStep();
            }
        }

        void RunStep()
        {
            switch (Step)
            {
                case RasterizationStep.Initialize:
                    rasterizer.Clear(new LevelTile
                    {
                        TileType = Space
                    });
                    graph.Template.InitializeRasterization(graph);
                    break;

                case RasterizationStep.Background:
                    var bg = graph.Template.CreateBackground();
                    bg.Init(rasterizer, graph, rand);
                    bg.Rasterize();
                    break;

                case RasterizationStep.Corridor:
                    RasterizeCorridors();
                    break;

                case RasterizationStep.Room:
                    RasterizeRooms();
                    break;

                case RasterizationStep.Overlay:
                    var overlay = graph.Template.CreateOverlay();
                    overlay.Init(rasterizer, graph, rand);
                    overlay.Rasterize();
                    break;
            }
            Step++;
        }

        void RasterizeCorridors()
        {
            var corridor = graph.Template.CreateCorridor();
            corridor.Init(rasterizer, graph, rand);

            foreach (var room in graph.Rooms)
                foreach (var edge in room.Edges)
                {
                    if (edge.RoomA != room)
                        continue;
                    RasterizeCorridor(corridor, edge);
                }
        }

        void CreateCorridor(LevelRoom src, LevelRoom dst, out Point srcPos, out Point dstPos)
        {
            var edge = src.Edges.Single(ed => ed.RoomB == dst);
            var link = edge.Linkage;

            if (link.Direction == Direction.South || link.Direction == Direction.North)
            {
                srcPos = new Point(link.Offset, src.Pos.Y + src.Length / 2);
                dstPos = new Point(link.Offset, dst.Pos.Y + dst.Length / 2);
            }
            else if (link.Direction == Direction.East || link.Direction == Direction.West)
            {
                srcPos = new Point(src.Pos.X + src.Width / 2, link.Offset);
                dstPos = new Point(dst.Pos.X + dst.Width / 2, link.Offset);
            }
            else
                throw new ArgumentException();
        }

        void RasterizeCorridor(LevelMapCorridor corridor, Edge edge)
        {
            Point srcPos, dstPos;
            CreateCorridor(edge.RoomA, edge.RoomB, out srcPos, out dstPos);
            corridor.Rasterize(edge.RoomA, edge.RoomB, srcPos, dstPos);
        }

        void RasterizeRooms()
        {
            foreach (var room in graph.Rooms)
                room.Rasterize(rasterizer, rand);
        }

        public LevelTile[,] ExportMap()
        {
            return rasterizer.Bitmap;
        }
    }
}
