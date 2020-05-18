namespace NthStudio.Game.Procedural
{
    using System.Collections.Generic;
    using System.Xml;
    // Core Library
    using NthDimension.Algebra;
    using NthDimension.Rendering;
    using NthDimension.Rendering.Geometry;
    using NthDimension.Rendering.Drawables.Models;    
    // Plugins
    using NthDimension.Procedural.Traffic;
    

    public class RoadModel : Model
    {

        new public static string nodename = "road";

        //List<PhysModel> drivingLanes = new List<PhysModel>();
        //List<PhysModel> auxiliaryLanes = new List<PhysModel>();

        public float LaneWidth = 5;
        public float SidewalkWidth = 1.65f;
        public float LaneHeight = 0.02f;
        public float SidewalkHeight = 0.1f;
        public float SplitWidth = 1;
        public bool Traffic;

        private Vector2[] line;
        private int segments;
        private int roadType = -1;

        public RoadModel(/*ApplicationObject parent,*/ Vector2[] line, int segments, int roadType, string material, float minHeight = 0f)
            :base(/*parent*/)

        {
            this.PrimitiveType = NthDimension.Rasterizer.PrimitiveType.Triangles;

            this.line = line;
            this.segments = segments;
            this.roadType = roadType;

            LaneMesh lane = new LaneMesh(line, segments, 0, LaneWidth, LaneHeight, minHeight);


            NthDimension.Rendering.Geometry.MeshVbo vboRoad = StudioWindow.Instance.MeshLoader.FromMesh(
                                lane.Vertices,
                                lane.Normals,
                                lane.TextureCoordinates,
                                lane.Faces);

            this.meshes = new NthDimension.Rendering.Geometry.MeshVbo[1]
            {
                vboRoad
            };

            this.setMaterial(material);
            this.CreateVAO();

        }
        protected override void specialLoad(ref XmlTextReader reader, string type)
        {
            if (reader.Name.ToLower() == "lanewidth" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                float.TryParse(reader.Value, out this.LaneWidth);
            }
            if (reader.Name.ToLower() == "sidewalkwidth" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                float.TryParse(reader.Value, out this.SidewalkWidth);
            }
            if (reader.Name.ToLower() == "laneheight" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                float.TryParse(reader.Value, out this.LaneHeight);
            }
            if (reader.Name.ToLower() == "sidewalkheight" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                float.TryParse(reader.Value, out this.SidewalkHeight);
            }
        }



    }
}
