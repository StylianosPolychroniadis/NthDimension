using NthDimension.Algebra;
using NthDimension.Procedural.Dungeon;
using NthDimension.Procedural.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Procedural
{
    public class DungeonFactory
    {
        //protected static DungeonFactory _instance;
        //public static DungeonFactory Instance
        //{
        //    get
        //    {
        //        if (null == _instance)
        //            _instance = new DungeonFactory();

        //        return _instance;
        //    }
        //}
        private Rasterizer          ras;

        static private NthDimension.Procedural.Dungeon.LevelMapTemplate     gDefaultTemplate        = new NthDimension.Procedural.Dungeon.Templates.Lab.LabTemplate();
        private NthDimension.Procedural.DungeonGenerator                    gLevel;
        private static readonly Random                                             rand             = new Random();
        ///<summary>Instance of furniture layout algorithm solver</summary>
        private NthDimension.Procedural.Room.Furnisher                      furnisher;          // TODO:: Implement
        ///<summary>Placement information for fittings</summary>
        private NthDimension.Procedural.Room.FittingPlacement[]             fittingPlacements;  // TODO:: Implement

        public List<LevelRoom>      Rooms
        {
            get { return this.gLevel.GetRooms().ToList(); }
        }
        public LevelGraph           Graph
        {
            get { return this.gLevel.ExportGraph(); }
        }

        public DungeonFactory()  
            : this(rand.Next(), gDefaultTemplate) { }
        public DungeonFactory(NthDimension.Procedural.Dungeon.LevelMapTemplate template)
            : this(rand.Next(), template) { }
        public DungeonFactory(int seed, NthDimension.Procedural.Dungeon.LevelMapTemplate template, string fittingDatabase = "data\\FittingDatabase.xml")
        {
            this.gLevel = new NthDimension.Procedural.DungeonGenerator(seed, template);
            this.gLevel.Generate(null);

            // Load fitting semantics from XML file
            this.furnisher = new Furnisher(fittingDatabase);
        }

        public void FurnishAllRooms()
        {
            var model = furnisher.GetFittingModels();

            foreach (var doom in Rooms)
            {
                NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("{0}Furnitures{0}----------{0}", Environment.NewLine));

                int furnCount = rand.Next(0, model.Count());
                // Rectangular room dimensions (width, depth, height, 
                // list of arrays for creating the doors (center x position, center y position, 
                // door breadth in meters, axis-aligned normal direction into room, optionally height), 
                // list of arrays for creating the windows (center x position, center y position, 
                // window breadth in meters, axis-aligned normal direction into room, optionally height, optionally elevation above floor))
                List<string> fittings = new List<string>();
                for (int i = 0; i < furnCount; i++)
                {
                    int idx = rand.Next(0, model.Count() - 1);
                    var roomFit = model.ToArray()[idx];

                    while (!fittings.Contains(roomFit.Id))
                    {
                        idx = rand.Next(0, model.Count() - 1);
                        roomFit = model.ToArray()[idx];

                        fittings.Add(roomFit.Id);
                    }

                }
                //foreach(var room in Rooms)
                
                #region calculate firring call
                this.calculateFittingPlacements(new Room(
                  5f,
                  4f,
                  2.6f,
                  //doom.Width,
                  //doom.Depth,
                  //doom.Length, // TODO:: Height
                  new List<float[]>()
                  {
                    new float[] {-0.5f, 2f, 0.9f, (float)Math.PI/2*3}
                  },
                  new List<float[]>()
                  {
                    new float[] {-1.65f, -2f, 0.9f, (float)Math.PI/2*1},
                    new float[] {-0.55f, -2f, 0.9f, (float)Math.PI/2*1},
                    new float[] {0.55f, -2f, 0.9f, (float)Math.PI/2*1},
                    new float[] {1.65f, -2f, 0.9f, (float)Math.PI/2*1},
                  }),
                    // new List<string>()
                    //{
                    //    "Frenhaus burlap sofa",
                    //    "KLEA floor lamp",
                    //    "Armaldi armchair",
                    //    "Armaldi wooden table",
                    //    "Armaldi wooden chair",
                    //    "Donnerstag coffee table",
                    //    "LuckyPanel flatscreen TV",
                    //    "KLEA white bookcase"
                    //}.ToArray()
                fittings.ToArray()                
                );
                #endregion

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testRoom"></param>
        /// <param name="fittingModels">
        // Rectangular room dimensions (width, depth, height, 
        // list of arrays for creating the doors (center x position, center y position, 
        // door breadth in meters, axis-aligned normal direction into room, optionally height), 
        // list of arrays for creating the windows (center x position, center y position, 
        // window breadth in meters, axis-aligned normal direction into room, optionally height, optionally elevation above floor))      
        /// </param>
        private void calculateFittingPlacements(Room testRoom, string[] fittingModels)
        {
            // Run the fitting placer algorithm to get fitting placements
            fittingPlacements = furnisher.GeneratePlacements(testRoom, new List<string>(fittingModels));

            // Output fitting placements
            foreach (FittingPlacement placement in fittingPlacements)
            {
                Console.WriteLine("{0}: ({1} , {2}) and {3} degrees turned. ", placement.RepresentationObject.FittingTypeId, placement.PositionX, placement.PositionY, (int)(placement.Orientation * 180 / Math.PI));
            }
        }


    }

    public static class Cube
    {
        public static Vector3[] Positions = new Vector3[]{
            new Vector3(-1.0f, -1.0f,  1.0f),
            new Vector3( 1.0f, -1.0f,  1.0f),
            new Vector3( 1.0f,  1.0f,  1.0f),
            new Vector3(-1.0f,  1.0f,  1.0f),
            new Vector3(-1.0f, -1.0f, -1.0f),
            new Vector3( 1.0f, -1.0f, -1.0f),
            new Vector3( 1.0f,  1.0f, -1.0f),
            new Vector3(-1.0f,  1.0f, -1.0f) };

        public static int[] Indices = new int[]{
             // front face
                0, 1, 2, 2, 3, 0,
                // top face
                3, 2, 6, 6, 7, 3,
                // back face
                7, 6, 5, 5, 4, 7,
                // left face
                4, 0, 3, 3, 7, 4,
                // bottom face
                0, 1, 5, 5, 4, 0,
                // right face
                1, 5, 6, 6, 2, 1, };

    }
}
