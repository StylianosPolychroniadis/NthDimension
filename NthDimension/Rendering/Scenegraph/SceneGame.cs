/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

// Features
#define VOXELS
// Debug
//#define DEBUG
//#define FBOSHADOW_DEBUG
#define FBOSHADOW_DEBUG1
#define MEASUREPERFORMANCE

using NthDimension.Rendering.GameViews;

namespace NthDimension.Rendering
{
    #region enum RenderPass
    public enum enuGameRenderPass
    {
        albedo,
        diffuse,
        defInfo,
        transparent,
        shadow,
        water,
        selection
    }
    #endregion
}

namespace NthDimension.Rendering.Scenegraph
{
    #region using
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Text;
    using System.Xml;

    
    using NthDimension.Algebra;
    using Rendering.Drawables;
    using Rendering.Drawables.Framebuffers;
    using Rendering.Drawables.Gui;
    using Rendering.Drawables.Lights;
    using Rendering.Drawables.Models;
    using Rendering.Drawables.Particles;
    using Rendering.Drawables.Voxel;
    using Rendering.Geometry;
    using Rendering.Partition;
    using Rendering.Shaders;
    using Rendering.Utilities;
    using NthDimension.Rendering.Culling;

    using NthDimension.Rasterizer;
    using System.Runtime.InteropServices;
    using NthDimension.Network;
    using NthDimension.Rendering.Serialization;
    #endregion using

   

#if _WINDOWS_

#endif
#pragma warning disable CS0414
    public partial class SceneGame : Drawable
    {
        #region Properties
        public const string                         nodeName                = "scene";
        public string                               SceneXsnFile            = "scene.xsn";
        public bool                                 BloomEnabled            { get { return b_bloomEnabled; } }
        public Vector2                              BloomSize               { get { return b_bloomSize; } }
        public readonly Vector2                     BloomSizeDefault        = new Vector2(1.2f, 1.2f);
        public readonly float                       BloomExposureDefault    = 1.2f;                     // Hard-coded in bloom_curve.fs [Uniform is NOT configured to pass]
        public readonly float                       BloomStrengthDefault    = 1.1f;                     // Hard-coded in bloom_curve.fs [Uniform is NOT configured to pass]

        public Vector2                              Gamma                   { get { return compositeMod; } }
        public bool                                 Initialized             = false;                    // Added Apr-11-18 after multithread assets loading

        public Quad2d                               Filter2d_RenderSurface;
        // 
        public List<Drawable>                       Drawables               = new List<Drawable> { };
        public List<LightSpot>                      Spotlights              = new List<LightSpot> { };
        public List<LightDirectional>               DirectionalLights       = new List<LightDirectional>() { };
        // Todo
        public List<LightPoint>                     PointLights             = new List<LightPoint>();
        // Culling
        public ConcurrentDictionary<Drawable,
                                    List<MeshVbo>>  VisibleDrawables        = new ConcurrentDictionary<Drawable, List<MeshVbo>>();     // Conversion to Thread-Based Engine
        // Gui
        public List<Gui>                            Guis                    = new List<Gui> { };
       // Particles
        public List<ParticleGenerator>              ParticleGenerators      = new List<ParticleGenerator>();
        //public ParticleSystem                       ParticleSystem;
        //public List<ParticleAffector>               ParticleControllers         = new List<ParticleAffector> { };


        public Framebuffer                          SunFrameBufferFar3;
        public Framebuffer                          SunFrameBufferFar2;
        public Framebuffer                          SunFrameBufferFar1;
        public Framebuffer                          SunFrameBufferNear;

#if FBOSHADOW_DEBUG
        public Framebuffer[]                        SunFrameBufferPssm;
        public Framebuffer[]                        SunInnerFrameBufferPssm;
#endif

       
        
#if VOXELS
        public VoxelManager                         VoxelManager;
#endif


        public Vector3                              PlayerSpawnAt           = new Vector3(); 
        public Vector3                              EyePos;
        public Matrix4                              WaterMatrix;
        //public Matrix4 ProjectionMatrix;
        //public Matrix4 ModelviewMatrix;

        public int[]                                EnvTextures             = new int[6];
        public int[]                                BackdropTextures;
        public int                                  CurrentLight;
        public int                                  LightCount;
        public static float                         WaterLevel              = 0.3f;

        public int                                  DrawCallTotal           = 0;
        public float                                AvatarYOffset           = 1f;

        public BufferRemotePlayers                  RemotePlayers           = new BufferRemotePlayers();
        public BufferPendingAvatars                 RemotePlayersPending    = new BufferPendingAvatars();
        public BufferRemotePlayers                  RemotePlayersDestroy    = new BufferRemotePlayers();
        public BufferPendingNonPlayers              NonPlayers              = new BufferPendingNonPlayers();
        public BufferPendingNonPlayers              NonPlayersPending       = new BufferPendingNonPlayers();
        public BufferPendingNonPlayers              NonPlayersDestroy       = new BufferPendingNonPlayers();

        //public bool                                 DoF_Motion = false;

        public int                                  ShadowResolution
        {
            get { return Settings.Instance.video.shadowResolution; }
        }

        public float                                GroundSize
        {
            get { return groundSize; }
        }
        private SceneEditor m_sceneEditor;
        public SceneEditor Editor
        {
            get
            {
                if (null == m_sceneEditor)
                    m_sceneEditor = new SceneEditor(this);
                return m_sceneEditor;
            }
        }

#endregion

#region OctreeWorld Properties
        //public 
#endregion

#region Fields
        protected WeakReference             _renderer;


        //private Skybox                      mSkyModel;
        private Vector2                     compositeMod                = Vector2.Zero;                        // Note Gamma is compositeMod.X value
        private Texture[]                   worldTextures;
        private Shaders.Shader[]            shaders;

#region private #Bloom
        private bool                        b_bloomEnabledDefault       = false;
        private bool                        b_bloomEnabled;
        private /*readonly*/ Vector2        b_bloomSize                 = new Vector2(1.2f, 1.2f);
        private float                       b_bloomExposure             = 1.2f;                     // Hard-coded in bloom_curve.fs [Uniform is NOT configured to pass]
        private float                       b_bloomStrength             = 1.1f;                     // Hard-coded in bloom_curve.fs [Uniform is NOT configured to pass]
#endregion

        public const float                  groundSize                  = 6000f;                     // 6000f;

        private DateTime                    framebufferResizeLast       = DateTime.Now;
        private long                        framebufferResizeInterval   = 2;
        private bool                        framebufferResize           = false;
        
        // Used only for debug
        //private float                       __debug_sun_theta         = 0f;
        //private float                       __debug_sun_radius        = 8000f;
        #endregion

        #region Ctor
        public SceneGame(/*Shape groundshape*/)
        {
            base.Scene = this;

            //prepare list of world textures
            int texCount = Enum.GetValues(typeof(Rendering.Material.WorldTexture)).Length;
            if (worldTextures == null)
                worldTextures = new Texture[texCount];

            #region Physics Collision System
            this.CreatePhysics();
#endregion

                     
        }
        #endregion

        #region Render Target (Quad2d)
        internal void CreateRenderSurface()
        {
            if (Filter2d_RenderSurface != null)
            {
                this.RemoveDrawable(Filter2d_RenderSurface); // ??? why? not added at the end
                this.removeChild(Filter2d_RenderSurface); // ??? why? not added at the end
            }

            //if(null == SceneRenderSurface)
            Filter2d_RenderSurface = new Quad2d(this);  // Does this cause access violation changing scenes?
            compositeMod.X = Settings.Instance.video.gamma;            
        }
        public void Create()
        {
            this.CreateRenderSurface();
        }
        #endregion Render Target

        #region database

        /// <summary>
        /// USE THIS FUNCTION ASAP!!! First find where you print the scene file and diff with this :-)
        /// </summary>
        public override void save(ref StringBuilder sb, int level)
        {

#region Sql Call
            /*
            SQLiteConnection connection = new SQLiteConnection();

            connection.ConnectionString = "Data Source=" + dataSource;
            connection.Open();

            SQLiteCommand command = new SQLiteCommand(connection);

            //preparing tables for insertion

            // delete table
            command.CommandText = "DROP TABLE IF EXISTS WorldObjects;";
            command.ExecuteNonQuery();

            // recreate table
            command.CommandText = "CREATE TABLE IF NOT EXISTS WorldObjects ( id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "name VARCHAR(100), position TEXT, rotation TEXT, material TEXT, meshes TEXT, pboxes TEXT, static INTEGER);";
            command.ExecuteNonQuery();

            // delete table
            command.CommandText = "DROP TABLE IF EXISTS WorldLights;";
            command.ExecuteNonQuery();

            // recreate table
            command.CommandText = "CREATE TABLE IF NOT EXISTS WorldLights ( id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "name VARCHAR(100), position TEXT, direction TEXT, color TEXT, parent TEXT);";
            command.ExecuteNonQuery();



            command.CommandText = sw.ToString();
            command.ExecuteNonQuery();

            // Freigabe der Ressourcen.
            command.Dispose();

            connection.Close();
            connection.Dispose();
             */
#endregion

            sb.AppendLine("<scene>");
            
            #region Bloom
            sb.AppendLine(string.Format("<bloom enabled=\"{0}\">", BloomEnabled ? "true" : "false"));
            sb.AppendLine(string.Format(" <curve>{0}|{1}</curve>", BloomSize.X, BloomSize.Y));
            sb.AppendLine(string.Format(" <exposure>{0}</exposure>", BloomExposureDefault));
            sb.AppendLine(string.Format(" <strength>{0}</strength>", BloomStrengthDefault));
            sb.AppendLine("</bloom>");
            #endregion
            
            //call tree to create string for saving
            saveChilds(ref sb, 1);

            sb.AppendLine("</scene>");

            using (StreamWriter outfile = new StreamWriter(SceneXsnFile))
            {
                outfile.Write(sb.ToString());
            }
        }

        /// <summary>
        /// Operates on Current-Directory file system access
        /// </summary>
        /// <param name="xsnFile"></param>
        public void loadObjects(string xsnFile)
        {
            if (string.IsNullOrEmpty(xsnFile)) // Sanity
                xsnFile = SceneXsnFile;

            XmlTextReader reader    = new XmlTextReader(xsnFile);
            //XmlReader reader0 = new XmlTextReader(xsnFile, XmlNodeType.Document, new XmlParserContext());// (xsnFile);

            try
            {
                reader.Read();

                // Attributes within the <Scene ...> tag
                if (reader.HasAttributes)
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name.ToLower() == "cache")
                        {

                        }
                    }

                load(ref reader, "scene");
            }
            catch (Exception lxe)
            {
                ConsoleUtil.errorlog("ERROR Loading Scene ", string.Format("Error establishing scene '{0} Line Number:{1} Line Position:{2}'. Please check definition file and try again.", SceneXsnFile, reader.LineNumber, reader.LinePosition));
                ConsoleUtil.errorlog(string.Format("{0}Reason: {1}", Environment.NewLine, lxe.Message), string.Format("{0}Stack Trace: {1}  ", Environment.NewLine, lxe.StackTrace));
                Console.WriteLine(string.Format("{0} {1}", Environment.NewLine, "Press Enter to continue..."));
                Console.ReadLine();
                
                // TODO:: YesNo Dialog to ask whether to continue loading or terminating the app
                
                //ApplicationBase.Instance.DisconnectClient();
                //ApplicationBase.Instance.Exit();                
            }
            finally
            {
                reader.Close();
            }
        }

        

#endregion database

#region init
        public void OnInitialize()
        {
            b_bloomEnabled    = b_bloomEnabledDefault;
            b_bloomSize       = BloomSizeDefault;
            b_bloomExposure   = BloomExposureDefault;
            b_bloomStrength   = BloomStrengthDefault;

            // TODO:: Reset All Items
            //this.CreateRenderSurface();
            this.DirectionalLights.Clear();
            this.createShadowMap();
            this.initSkyModel(); // Clears previous skybox instances if any
            this.initGoundPlane();
            this.initParticleSystem_Example();
            this.initVoxelSystem();
            this.Initialized = true;
        }
        public void OnResize()
        {
            // ISSUE___
            //this.createShadowMap(); // Removed Oct-06-19 Caused flicker in shadows 
            //int far1res = ShadowResolution;
            //ConsoleUtil.log(string.Format(@"-Creating Wide B/4 SunShadow  FBO Far 0 {0}x{1} px", far1res, far1res));
            //if (null != SunFrameBufferFar1)
            //    SunFrameBufferFar1.Delete();
            //SunFrameBufferFar1 = ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebuffer", far1res, far1res, PixelInternalFormat.Rgba8, false); // Warning increased by 2 for artifact removal

            //int nearres = ShadowResolution;
            //ConsoleUtil.log(string.Format(@"-Creating Short A/4 SunShadow FBO {0}x{1} px", nearres, nearres));
            //if (null != SunFrameBufferNear)
            //    SunFrameBufferNear.Delete();
            //SunFrameBufferNear = ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebufferInner", nearres, nearres, PixelInternalFormat.Rgba8, false);

            if (this.framebufferResize) return;
            this.framebufferResize = true;

        }

        private void createShadowMap()
        {

            #region Key Light
            //if (SunLight_Key != null)
            //{
            //    this.RemoveDrawable(SunLight_Key);
            //    this.removeChild(SunLight_Key);
            //}

            //Vector3 ambient = new Vector3(0.01875f, 0.01875f, 0.01875f) * 0.001f;

            //SunLight_Key = new LightSun(this);
            //SunLight_Key.lightAmbient = new Vector3(ambient);          //= new Vector3(1f, 1f, 1f) * 1000f;//  0.46f;
            //SunLight_Key.PointingDirection = SunLight_Key.ViewInfo.PointingDirection = new Vector3(0.4817817f, -0.4817817f, 0.731965f);//Vector3.Normalize(new Vector3(674, -674, 1024));
            //SunLight_Key.Enabled = true;
            #endregion

            #region Fill Light
            //if (SunLight_Fill != null)
            //{
            //    this.RemoveDrawable(SunLight_Fill);
            //    this.removeChild(SunLight_Fill);
            //}

            //SunLight_Fill = new LightSun(this);
            ////SunLight_Fill.lightAmbient = ambient / 10f;     // SunLight_Key.lightAmbient * 0.0601f; //  0.46f;
            //SunLight_Fill.lightAmbient = new Vector3(ambient) * 0.1f;     // SunLight_Key.lightAmbient * 0.0601f; //  0.46f;
            //SunLight_Fill.PointingDirection = SunLight_Fill.ViewInfo.PointingDirection = new Vector3(-0.4817817f, -0.4817817f, 0.731965f);
            #endregion

            #region Back Light
            //if (SunLight_Back != null)
            //{
            //    this.RemoveDrawable(SunLight_Back);
            //    this.removeChild(SunLight_Back);
            //}

            //SunLight_Back = new LightSun(this);
            //SunLight_Back.Enabled = false; // TODO: Need the LookAt.Inversed
            ////SunLight_Back.lightAmbient = new Vector3(0.6875f, 0, 0); // Debug -RED-
            //SunLight_Fill.lightAmbient = new Vector3(0.3635f, 0.3635f, 0.3435f); //SunLight_Key.lightAmbient; 
            //SunLight_Back.PointingDirection = SunLight_Back.ViewInfo.PointingDirection = new Vector3(0.4817817f, -0.4817817f, -0.731965f);
            #endregion




#if FBOSHADOW_DEBUG
            if (Settings.Instance.video.pssmShadows)
            {
                SunFrameBufferPssm = new Framebuffer[4]
                {
                    ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebufferPSSM0", ShadowResolution * 2, ShadowResolution * 2, PixelInternalFormat.Rgba8, false),
                    ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebufferPSSM1", ShadowResolution * 2, ShadowResolution * 2, PixelInternalFormat.Rgba8, false),
                    ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebufferPSSM2", ShadowResolution * 2, ShadowResolution * 2, PixelInternalFormat.Rgba8, false),
                    ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebufferPSSM3", ShadowResolution * 2, ShadowResolution * 2, PixelInternalFormat.Rgba8, false)
                };

                SunInnerFrameBufferPssm = new Framebuffer[4]
                {
                    ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebufferInner0", ShadowResolution * 2, ShadowResolution * 2, PixelInternalFormat.Rgba8, false),
                    ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebufferInner1", ShadowResolution * 2, ShadowResolution * 2, PixelInternalFormat.Rgba8, false),
                    ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebufferInner2", ShadowResolution * 2, ShadowResolution * 2, PixelInternalFormat.Rgba8, false),
                    ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebufferInner3", ShadowResolution * 2, ShadowResolution * 2, PixelInternalFormat.Rgba8, false)
                };
             } else {
#endif
            int far3res = ShadowResolution; // * 2;
            int far2res = ShadowResolution;
            int far1res = ShadowResolution;
            int nearres = ShadowResolution;

        
            ConsoleUtil.log(string.Format(@"-Creating Wide D/4 SunShadow  FBO {0}x{1} px", far3res, far3res));
            if (null != SunFrameBufferFar3)
                SunFrameBufferFar3.Delete();
            SunFrameBufferFar3 = ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebufferFarthest", far3res, far3res, PixelInternalFormat.Rgba8, false); // Warning increased by 2 for artifact removal

            ConsoleUtil.log(string.Format(@"-Creating Wide C/4 SunShadow  FBO {0}x{1} px", far2res, far2res));
            if (null != SunFrameBufferFar2)
                SunFrameBufferFar2.Delete();
            SunFrameBufferFar2 = ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebufferFarther", far2res, far2res, PixelInternalFormat.Rgba8, false); // Warning increased by 2 for artifact removal

            ConsoleUtil.log(string.Format(@"-Creating Wide B/4 SunShadow  FBO Far 0 {0}x{1} px", far1res, far1res));
            if (null != SunFrameBufferFar1)
                SunFrameBufferFar1.Delete();
            SunFrameBufferFar1          = ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebuffer", far1res, far1res, PixelInternalFormat.Rgba8, false); // Warning increased by 2 for artifact removal

            ConsoleUtil.log(string.Format(@"-Creating Short A/4 SunShadow FBO {0}x{1} px", nearres, nearres));
            if (null != SunFrameBufferNear)
                SunFrameBufferNear.Delete();
            SunFrameBufferNear     = ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("SunShadowFramebufferInner", nearres, nearres, PixelInternalFormat.Rgba8, false);

#if FBOSHADOW_DEBUG
        }
#endif


    }
              
        /// <summary>
        /// Clears previous skybox instances if any
        /// </summary>
        private void initSkyModel()
        {
            ApplicationObject[] lst = Scene.childs.Where(m => m is Skybox).ToArray();
            for (int x = 0; x < lst.Length; x++)
            {
                if (lst[x] is Skybox)
                {

                    this.RemoveDrawable(lst[x] as Skybox);
                    this.removeChild(lst[x] as Skybox);
                }
            }

            
        }
        private void initGoundPlane()
        {
            //mGroundPlane = new GroundPlane(this);
            //mGroundPlane.setMaterial("floor.xmf");
            //mGroundPlane.setMesh("base\\water_plane.obj");
            //mGroundPlane.Position = new Vector3(0f, 0f, 0);
            //mGroundPlane.Position = new Vector3(0, WaterLevel, 0);
        }
        private void initParticleSystem_Example()
        {
            if (Settings.Instance.video.Particles)
            {
                generateParticleSystem_Example();

                //ParticleControllers.Add(new ParticleAffectorWind(new Vector3(1, -0.5f, 0) * 0.01f));
                //ParticleControllers.Add(new ParticleAffectorFriction(0.1f));
                //ParticleControllers.Add(new ParticleAffectorFloorKiller(WaterLevel));
                //ParticleControllers.Add(new ParticleAffectorLifeTimeKiller(this));
                if (null != ParticleGenerators[0])
                {
                    ParticleGenerators[0].Affectors.Add(new ParticleAffectorWind(new Vector3(1, -0.5f, 0) * 0.01f));
                    ParticleGenerators[0].Affectors.Add(new ParticleAffectorFriction(0.1f));
                    ParticleGenerators[0].Affectors.Add(new ParticleAffectorFloorKiller(WaterLevel));
                    ParticleGenerators[0].Affectors.Add(new ParticleAffectorLifeTimeKiller(this));
                }
            }
        }
        private void initVoxelSystem()
        {
#if _DEVUI_
            return;
#endif

#if VOXELS

            if (null != VoxelManager)
            {
                this.removeChild(VoxelManager);
            }


            VoxelManager = new VoxelManager(this);
#endif
        }
#endregion



#region settings
        public void SetBloomEnabled(Vector2 size, float exposure, float strength)
        {
            if(!Settings.Instance.video.bloom)
            {
                b_bloomEnabled = false;
                ConsoleUtil.log(string.Format(" <!> Bloom is disabled in settings"));
                return;
            }

            b_bloomEnabled = true;

            this.b_bloomSize      = size;
            this.b_bloomExposure  = exposure;
            this.b_bloomStrength  = strength;

            ConsoleUtil.log(string.Format(" <!> Bloom Enabled: \n\t\tSize: {0}\n\t\tExposure: {1} (fixed in shader)\n\t\tStrength: {2} (fixed in shader)\n", b_bloomSize, b_bloomExposure, b_bloomStrength));

            // TODO:: Pass these variables to bloom curve uniforms
        }
        public void SetBloomDisabled()
        {
            b_bloomEnabled = false;           

            ConsoleUtil.log(string.Format(" <!> Bloom Disabled"));
        }
#endregion

#region Add - Remove Drawable

        private object _lockDa = new object();
        public void AddDrawable(Drawable drawable)
        {

            lock (_lockDa)
            {
                this.Drawables.Add(drawable);
            }

            //for (int m = 0; m < drawable.meshes.Length; m++)
            //{
            //    if (GenericMethods.nearlyEqual(Vector3.Subtract(drawable.meshes[m].OctreeBounds.Max, drawable.meshes[m].OctreeBounds.Min).Length, 0f))
            //        drawable.UpdateBounds();
            //    this.OctreeWorld.Add(drawable.meshes[m]);
            //}
        }
        public void RemoveDrawable(Drawable drawable)
        {
            lock (_lockDa)
            {
                this.Drawables.Remove(drawable);

                if (drawable is PlayerModel)
                {
                    foreach (Model m in ((PlayerModel)drawable).AttachmentModels)
                        this.Drawables.Remove(m);
                }
            }
            //this.Drawables.RemoveAll(x => x.MarkForDelete);

            //if (this.RemotePlayers.Contains(drawable) && drawable is PlayerRemoteModel)
            //    this.RemotePlayers.Remove(drawable as PlayerRemoteModel);
            ////for (int m = 0; m < drawable.meshes.Length; m++)
            ////    this.OctreeWorld.Remove(drawable.meshes[m]);
        }
#endregion

#region Generate Particle System
        public void generateParticleSystem_Example()
        {
            ParticleGenerators.Add(new ParticleGenerator(
                Scene,
                RenderLayer.Transparent,
                1000,
                new Vector4(0.8f, 0.3f, 0.8f, 1.0f),
                new string[2, 2]
                {
                    {"particles\\particle_a.xmf",  "base\\sprite_plane.obj"},
                    {"particles\\particle_b.xmf",  "base\\sprite_plane.obj"}
                },
                Scene.Position,
                Vector3.One * 1f,
                Matrix4.Identity
                ));

            //ParticleSystem = new ParticleSystem(Scene);
            //ParticleSystem.addMaterial("particles\\particle_a.xmf");
            //ParticleSystem.addMesh("base\\sprite_plane.obj");

            //ParticleSystem.addMaterial("particles\\particle_b.xmf");
            //ParticleSystem.addMesh("base\\sprite_plane.obj");

            //ParticleSystem.Color = new Vector4(0.8f, 0.3f, 0.8f, 1.0f);

            //ParticleSystem.Position = Scene.Position;
            //ParticleSystem.Parent = Scene;
            //ParticleSystem.Size = Vector3.One * 1f;

            //ParticleSystem.Orientation = Matrix4.Identity;

            //ParticleSystem.generateParticles(1000);

            //ParticleSystem.Renderlayer = RenderLayer.Transparent;
        }
#endregion

#region Setup Scene Filter Shaders
        private bool sceneFiltersLoaded = false;
        public void SetupSceneFilterShaders()
        {

            ///if (sceneFiltersLoaded) return;

            string[] names = Enum.GetNames(typeof(enuShaderFilters));
            int length = names.Length;
            shaders = new Shaders.Shader[names.Length];

            for (int i = 0; i < length; i++)
            {
                string tmpName = "SceneFilters\\" + names[i] + ".xsp";
                try
                {
                    shaders[i] = ApplicationBase.Instance.ShaderLoader.GetShaderByName(tmpName);
                }
                catch
                {
                    ConsoleUtil.log(string.Format("Warning Scene Filter {0} not found!", tmpName));
                }
            }
            sceneFiltersLoaded = true;
        }
#endregion

#region Get - Set Assets

        public Drawable GetDrawableByName(string name)
        {
            foreach (var d in Drawables)
            {
                if (d.Name == name)
                    return d;
            }
            return null;
        }
        public Model GetModelbyName(string name)
        {
            return (Model)getChild(name);
        }
        public Shader GetShader(enuShaderFilters shaderType)
        {
            return shaders[(int)shaderType];
        }
        public string GetUniqueName()
        {
            if (String.IsNullOrEmpty(Name))
                Name = "new";

            int i = 0;
            string tmpName = string.Format(@"scenes\{0}\GameObject", Name);
            while (getChild(tmpName) != null)
            {
                tmpName = string.Format(@"scenes\{0}\GameObject{1}", Name, i);
                i++;
            }
            return tmpName;
        }
        public int GetTextureId(Rendering.Material.WorldTexture type)
        {
            return worldTextures[(int)type].texture;
        }
        public void SetTextureId(Rendering.Material.WorldTexture type, int id)
        {
            worldTextures[(int)type].texture = id;
        }
#endregion

#region Update
        //private VisibleFrustum frustum;
        //private VisibleFrustum frustum;

        private object _lockPa = new object();
        private bool prevAni = false;
       
        public override void Update()
        {
            if (framebufferResize)
            {
                framebufferResize = false;
                this.createShadowMap();
            }

            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Entering Scene Update {0}:{1}", this.GetType(),
                Name));

#region debug sun motion [Key F9]

            //bool cAni = Game.Instance.PlayerInput.F9;
            //if (cAni && !prevAni)
            //{
            //    //if (Game.Instance.PlayerInput.KeypadAdd)
            //    //    debug_sun_radius += 10f;

            //    //if (Game.Instance.PlayerInput.KeypadMinus)
            //    //    debug_sun_radius -= 10f;

            //    //Vector3 sunPos = this.SunLight0.ViewInfo.Position;

            //    //sunPos = new Vector3(sunPos.X + debug_sun_radius * (float)Math.Cos(debug_sun_theta), sunPos.Y,
            //    //    sunPos.Z + debug_sun_radius * (float)Math.Sin(debug_sun_theta));
            //    //sunPos.Y = 4600;

            //    ////this.sunLight.Position = this.sunLight.ViewInfo.Position = sunPos;
            //    //this.SunLight0.PointingDirection = sunPos;


            //    //debug_sun_theta += 0.01f;

            //    //if (debug_sun_theta > MathHelper.Pi * 4) debug_sun_theta = 0f;

            //    //ConsoleUtil.log(string.Format("Sun Distance {0} Position: {1}", debug_sun_radius, sunPos));



            //    foreach(Drawable d in Drawables)
            //        if(d is AnimatedModel)
            //            //for (int m = 0; m < d.meshes.Length; m++)
            //            {
            //            int sani = ((AnimatedModel)d).CurrentAnimation + 1;

            //            if ((sani) > d.meshes[0].AnimationCount - 1)
            //                sani = 0;

            //            ((AnimatedModel)d).SetAnimation(sani);
            //            //break;



            //        }
            //}
            //prevAni = cAni;

#endregion

#region Step Physics
            //call physicengine to step
            worldPhysics.Step((float)1.0f / 60.0f, true);
#endregion

#region Update Children
            //update scene tree
            updateChilds();
#endregion

#region Update Shadow map
            //update shadowmap
            //LightCount = Spotlights.Count;

            //int fbTargetRes = (int) Game.Instance.FBO_SHADOW[fs].Size.X/ShadowResolution;
            //int fbTargetRes = (int)Game.Instance.FBO_Shadow.Size.X / ShadowResolution;
            //int fbTargetRes = (int)ApplicationBase.Instance.FBO_Shadow.Size.X / ShadowResolution;


            //if (fbTargetRes != LightCount && LightCount > 0)

            if (LightCount < Spotlights.Count)
            {


#if FBOSHADOW_DEBUG
                if (Settings.Instance.video.pssmShadows)
                {
#pragma warning disable CS0162
                    for (int pss =0; pss < ApplicationBase.Instance.FBO_ShadowPssm.Length; pss++)
                    {
                        throw new NotImplementedException("Simple vs PSSM fbTargetRes");
                        //LightCount = Spotlights.Count;
                        //int fbTargetRes = (int)ApplicationBase.Instance.FBO_ShadowPssm[pss].Size.X / ShadowResolution;
                        ////Game.Instance.FBO_SHADOW[fs] = Game.Instance.FramebufferCreator.createFrameBuffer("shadowFramebuffer",
                        
                        

                        //ConsoleUtil.log(string.Format("{0} - Updating Shadow Framebuffer Target: {1}, No of Lights: {2}",
                        //                            DateTime.Now.ToString("HH:mm:ss.fff"), fbTargetRes, LightCount));

                        //ApplicationBase.Instance.FBO_ShadowPssm[pss] = ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("shadowFramebuffer",
                        //    ShadowResolution * LightCount, ShadowResolution, PixelInternalFormat.Rgba16f, false);


                        

                        //foreach (var light in Spotlights)
                        //    light.viewInfo.wasUpdated = true;
                    }
                            }
            else
                {

#endif
                LightCount = Spotlights.Count;
                    int fbTargetRes     = (int)ApplicationBase.Instance.FBO_Shadow.Size.X / ShadowResolution;

                    ConsoleUtil.log(string.Format("{0} - Updating spotlights. Re-Creating Shadow Framebuffer (Target: {1}), No of spotlights: {2}",
                        DateTime.Now.ToString("HH:mm:ss.fff"), fbTargetRes, LightCount));
                //Game.Instance.FBO_SHADOW[fs] = Game.Instance.FramebufferCreator.createFrameBuffer("shadowFramebuffer",

                //if (null != ApplicationBase.Instance.FBO_Shadow) ApplicationBase.Instance.FBO_Shadow.Delete();
                ApplicationBase.Instance.FBO_Shadow = 
                                ApplicationBase.Instance.FramebufferCreator.createFrameBuffer("shadowFramebuffer",
                                                                                              //ShadowResolution * LightCount,
                                                                                              ShadowResolution, // Original
                                                                                              ShadowResolution,
                                                                                              PixelInternalFormat.Rgba16f, 
                                                                                              false);

                    foreach (var light in Spotlights)
                        light.viewInfo.wasUpdated = true;
#if FBOSHADOW_DEBUG
            }
#endif
            }

            //foreach (LightSpot ls in Scene.Spotlights)    // Troubleshooting spotligth
            //    ls.Update();

#endregion

#region Sort Drawables by distance
            //sort drawables by distance        //foreach (var drawable in Drawables)
            Parallel.ForEach(this.Drawables, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (drawable) =>

            {
                Vector3 vectorTobObject = (drawable.Position) -
                                            (null == ApplicationBase.Instance.Player ?
                                                    Vector3.Zero :
                                                    ApplicationBase.Instance.Player.Position);        // NOTE-WARNING: This may affect shader operation
                drawable.DistToCamera = vectorTobObject.LengthFast;

                // TODO:: Define what drawable represents here -> what about drawable.meshes???

            }
            );
            Drawables.Sort(compareByDistance);
#endregion

#region Occlusion Culling Queries
            VisibleDrawables.Clear();

            if (null != ApplicationBase.Instance.Player)
                ApplicationBase.Instance.Player.ViewInfo.Update();     // Make sure ViewInfo has been updated

#region Culling
            //Parallel.ForEach(this.Drawables, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (d, state) =>
            foreach (Drawable d in this.Drawables)
            {

#region UpdateBounds

                for (int m = 0; m < d.meshes.Length; m++)
                    if (null != d.meshes[m])
                        try
                        {
                            if (GenericMethods.nearlyEqual(Vector3.Subtract(d.meshes[m].OctreeBounds.Max, d.meshes[m].OctreeBounds.Min).LengthFast, 0f))
                                d.UpdateBounds();
                        }
                        catch { }

#endregion

#region Culling

#region Skip List
                if (d is Skybox ||
                    d.Parent is ApplicationUser ||
                    d is LightSpot ||
                    d is LightDirectional ||
                    d is AnimatedModel ||
                    d is ElectricArcModel || 
                    d.IgnoreCulling)
                {

                    for (int v = 0; v < d.meshes.Length; v++)
                    {
                        if(null == d.meshes[v])
                        {
                            ConsoleUtil.log(string.Format("Mesh is null {0} mesh id {1}", d.Name, v));
                            continue;
                        }
                        d.meshes[v].CurrentLod = MeshVbo.MeshLod.Level0;

                        if (!VisibleDrawables.ContainsKey(d))
                            VisibleDrawables.TryAdd(d, d.meshes.ToList());
                        else if (VisibleDrawables.ContainsKey(d) &&
                                 !VisibleDrawables[d].Contains(d.meshes[v]))
                            VisibleDrawables[d].Add(d.meshes[v]);

                        //return;
                    }
                }
#endregion

#region Fast Frustum Culling

                foreach (MeshVbo m in d.meshes)
                {
                    //if ((GameBase.Instance.Player.Position - m.Centroid).LengthFast < 30f)
                    ////if ((GameBase.Instance.Player.Position - (m.Centroid + d.Position)).LengthFast < 30f)                          // If distance to mesh is small [TODO:: Place in settings]
                    //{
                    //    m.CurrentLod = Mesh.MeshLod.Level0;

                    //    if (!VisibleDrawables.ContainsKey(d))
                    //        VisibleDrawables.TryAdd(d, new List<Mesh>() { m });
                    //    else if (VisibleDrawables.ContainsKey(d) &&
                    //                !VisibleDrawables[d].Contains(m))
                    //        VisibleDrawables[d].Add(m);
                    //}
                    //else 
                    if (!VisibleDrawables.ContainsKey(d) ||                                             // Does not contain the drawable   -OR-
                                (VisibleDrawables.ContainsKey(d) && !VisibleDrawables[d].Contains(m)))          // Contains the Drawable but NOT the mesh
                        if (frustrumLODCheck(ApplicationBase.Instance.Player.ViewInfo, m, d))
                        {
                            if (!VisibleDrawables.ContainsKey(d))
                                VisibleDrawables.TryAdd(d, new List<MeshVbo>() { m });
                            else if (VisibleDrawables.ContainsKey(d) &&
                                        !VisibleDrawables[d].Contains(m))
                                VisibleDrawables[d].Add(m);
                        }
                }
#endregion

#endregion

            }
            //);
#endregion

            //////Debug
            //foreach (Drawable d in VisibleDrawables.Keys)
            //    d.IsVisible = true;

            //ApplicationBase.Instance.Player.FirstPersonView.

            //////foreach(Drawable d in VisibleDrawables.Keys)
            //////{
            //////    if (d.Name.Contains("arc"))
            //////        ConsoleUtil.log("Drawing ARC!!!!");
            //////}

#endregion

#region Particle System Position
            if (Settings.Instance.video.Particles)
            {
                try
                {
                    if (null == ParticleGenerators[0].System)
                        initParticleSystem_Example();

                    ParticleGenerators[0].System.Position = EyePos;
                }
                catch (Exception e)
                {
                    ConsoleUtil.errorlog("Update particle system position ", e.Message);
                }
            }
#endregion

#region Add/Remove RemotePlayers and NPCs (Avoids Cross Thread operation)

#region Add
            lock (_lockPa)
            {
                {
                    if (RemotePlayersPending.Count > 0)
                        foreach (PendingAvatar pa in RemotePlayersPending)
                        {
                            try
                            {
                                PlayerRemoteModel remote = new PlayerRemoteModel(this, pa.Avatar.AvatarSex);

                                remote.Name = pa.Avatar.AvatarName;
                                remote.UserId = pa.Avatar.UserId;
                                remote.AvatarBodyType = pa.Avatar.BodyType;
                                remote.AvatarFaceType = pa.Avatar.FaceType;

#region Body
                                remote.SetBodyMesh(pa.Avatar.BodyMesh,
                                    pa.Avatar.BodyMaterial.ToLower(),
                                    pa.Avatar.SpawnAt,
                                    pa.Avatar.BodyRotation,
                                    pa.Avatar.BodySize);
                                remote.BodyRotationOffset = GenericMethods.Matrix4FromString(pa.Avatar.BodyRotation);
#endregion

#region Find Top Vertex
                                Vector3 m_topVertex = new Vector3();

                                for (int m = 0; m < remote.meshes[0].MeshData.Positions.Length; m++)
                                    if (remote.meshes[0].MeshData.Positions[m].Y > m_topVertex.Y)
                                        m_topVertex = remote.meshes[0].MeshData.Positions[m];

                                remote.SetTopVertex(m_topVertex);
#endregion

#region Face - Index 0
                                if (null != pa.Avatar.Attachments[0])
                                {
                                    remote.SetFaceMesh(pa.Avatar.Attachments[0].Mesh,
                                        pa.Avatar.Attachments[0].Material,
                                        pa.Avatar.Attachments[0].Matrix,
                                        pa.Avatar.Attachments[0].Vertex,
                                        pa.Avatar.Attachments[0].Offset,
                                        pa.Avatar.Attachments[0].Size,
                                        pa.Avatar.Attachments[0].Orientation);
                                    remote.FaceRotationOffset = GenericMethods.Matrix4FromString(pa.Avatar.Attachments[0].Orientation);
                                }
#endregion

#region Hair - Index 1
                                if (null != pa.Avatar.Attachments[1])
                                {
                                    remote.SetHairMesh(pa.Avatar.Attachments[1].Mesh,
                                        pa.Avatar.Attachments[1].Material.ToLower(),
                                        pa.Avatar.Attachments[1].Matrix,
                                        pa.Avatar.Attachments[1].Vertex,
                                        pa.Avatar.Attachments[1].Offset,
                                        pa.Avatar.Attachments[1].Size,
                                        pa.Avatar.Attachments[1].Orientation);
                                    remote.HairRotationOffset = GenericMethods.Matrix4FromString(pa.Avatar.Attachments[1].Orientation);
                                }
#endregion

#region Shirt - Index 2
                                if (null != pa.Avatar.Attachments[2])
                                {
                                    remote.SetShirtMesh(pa.Avatar.Attachments[2].Mesh,
                                        pa.Avatar.Attachments[2].Material.ToLower(),
                                        pa.Avatar.Attachments[2].Matrix,
                                        pa.Avatar.Attachments[2].Vertex,
                                        pa.Avatar.Attachments[2].Offset,
                                        pa.Avatar.Attachments[2].Size,
                                        pa.Avatar.Attachments[2].Orientation);
                                    remote.ShirtRotationOffset = GenericMethods.Matrix4FromString(pa.Avatar.Attachments[2].Orientation);

                                    remote.SetShirtAnimation(pa.Avatar.Attachments[2].Animation_Idle,
                                                             pa.Avatar.Attachments[2].Animation_Walk,
                                                             pa.Avatar.Attachments[2].Animation_Sit,
                                                             pa.Avatar.Attachments[2].Animation_SitIdle,
                                                             pa.Avatar.Attachments[2].Animation_Stand,
                                                             pa.Avatar.Attachments[2].Animation_Wave);
                                }
#endregion

#region Pants - Index 3
                                if (null != pa.Avatar.Attachments[3])
                                {
                                    remote.SetPantsMesh(pa.Avatar.Attachments[3].Mesh,
                                        pa.Avatar.Attachments[3].Material.ToLower(),
                                        pa.Avatar.Attachments[3].Matrix,
                                        pa.Avatar.Attachments[3].Vertex,
                                        pa.Avatar.Attachments[3].Offset,
                                        pa.Avatar.Attachments[3].Size,
                                        pa.Avatar.Attachments[3].Orientation);
                                    remote.PantsRotationOffset = GenericMethods.Matrix4FromString(pa.Avatar.Attachments[3].Orientation);

                                    remote.SetPantsAnimation(pa.Avatar.Attachments[3].Animation_Idle,
                                                             pa.Avatar.Attachments[3].Animation_Walk,
                                                             pa.Avatar.Attachments[3].Animation_Sit,
                                                             pa.Avatar.Attachments[3].Animation_SitIdle,
                                                             pa.Avatar.Attachments[3].Animation_Stand,
                                                             pa.Avatar.Attachments[3].Animation_Wave);
                                }
#endregion

#region Shoes - Index 4
                                if (null != pa.Avatar.Attachments[4])
                                {
                                    remote.SetShoesMesh(pa.Avatar.Attachments[4].Mesh,
                                        pa.Avatar.Attachments[4].Material.ToLower(),
                                        pa.Avatar.Attachments[4].Matrix,
                                        pa.Avatar.Attachments[4].Vertex,
                                        pa.Avatar.Attachments[4].Offset,
                                        pa.Avatar.Attachments[4].Size,
                                        pa.Avatar.Attachments[4].Orientation);
                                    remote.HairRotationOffset = GenericMethods.Matrix4FromString(pa.Avatar.Attachments[4].Orientation);

                                    remote.SetShoesAnimation(pa.Avatar.Attachments[4].Animation_Idle,
                                                             pa.Avatar.Attachments[4].Animation_Walk,
                                                             pa.Avatar.Attachments[4].Animation_Sit,
                                                             pa.Avatar.Attachments[4].Animation_SitIdle,
                                                             pa.Avatar.Attachments[4].Animation_Stand,
                                                             pa.Avatar.Attachments[4].Animation_Wave);
                                }
#endregion

                                this.RemotePlayers.Add(remote);

                                remote.Position = GenericMethods.Vector3FromString(pa.Avatar.SpawnAt);
                                remote.SetGotoOnce(remote.Position);
                                remote.SetAnimationIdle();


                                //this.RemotePlayers.Add(remote);

                                remote.OnSelected += delegate (ModelSelection e)
                                {
                                    ApplicationBase.Instance.OnSelectAvatar((AnimatedModel.AnimatedModelSelection)e);
                                };
                                remote.OnUnselected += delegate
                                {
                                    // TODO:: TBD
                                };
                            }
#pragma warning disable CS0168
                            catch (Exception e)
                            { }

                            //CreateRemoteAvatars.RemoveAt(0);
                        }

                    if (NonPlayersPending.Count > 0)
                        foreach(NonPlayerModel npc in NonPlayersPending)
                        {
                            try
                            {
                                npc.Scene = this;

                                this.NonPlayers.Add(npc);

                                npc.SetGotoOnce(npc.Position);
                                npc.SetAnimationIdle();

                                npc.OnSelected += delegate (ModelSelection e)
                                {
                                    ApplicationBase.Instance.OnSelectAvatar((AnimatedModel.AnimatedModelSelection)e);
                                };
                                npc.OnUnselected += delegate
                                {

                                };
                            }
#pragma warning disable CS0168
                            catch (Exception e)
                            { }
                        }
                }

                this.RemotePlayersPending.Clear();
                this.NonPlayersPending.Clear();
            }
#endregion

#region Remove 
            List<PlayerRemoteModel> dissolving = new List<PlayerRemoteModel>();
            lock (_lockPa)
            {
                foreach (PlayerRemoteModel pa in RemotePlayersDestroy)
                {
                    var rp = RemotePlayers.FirstOrDefault(x => x.UserId == pa.UserId);

                    if (null != rp)
                        dissolving.Add(rp);
                }

                RemotePlayersDestroy.Clear();
            }

            foreach (PlayerRemoteModel rp in dissolving)
            {
                rp.dissolve();

                RemotePlayers.Remove(rp);

                rp.MarkForDelete = true;

                for (int i = 0; i < rp.AttachmentModels.Length; i++)
                    rp.AttachmentModels[i].MarkForDelete = true;
            }

            List<NonPlayerModel> dissolvingNpc = new List<NonPlayerModel>();

            lock (_lockPa)
            {
                foreach (NonPlayerModel np in NonPlayersDestroy)
                {
                    if (null != np)
                        dissolvingNpc.Add(np);
                }

                NonPlayersDestroy.Clear();
            }

            foreach(NonPlayerModel rn in dissolvingNpc)
            {
                rn.dissolve();

                NonPlayers.Remove(rn);
                rn.MarkForDelete = true;

                for (int i = 0; i < rn.AttachmentModels.Length; i++)
                    rn.AttachmentModels[i].MarkForDelete = true;

            }

            this.Drawables.RemoveAll(m => m.MarkForDelete);
#endregion
#endregion

#region Add/Remove SpotLights

            lock (_lockPa)
            {
                //foreach(LightSpot spot in CreateSpotLights)

            }
#endregion

            Drawables.Sort(compareByMaterial);

            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Leaving Scene Update {0}:{1}", this.GetType(), Name));
        }

        bool frustrumLODCheck(ViewInfo view, MeshVbo mesh, Drawable drawable) // Vector3 drawablePosition)
        {
            Vector3 drawablePosition = drawable.Position;
            //Vector4 drawablePosition = GenericMethods.Mult(new Vector4(drawable.Position, 1), info0.modelviewProjectionMatrix);
#if _DEVUI_
            return false;
#endif

            ViewInfo vi = view;
            float visibility = Settings.Instance.view.Visibility;



            float lod0 = visibility * (mesh.IsAnimated
                ? Settings.Instance.view.DistanceAnimated0
                : Settings.Instance.view.DistanceStatic0);  //0.55f;  // 
            float lod1 = visibility * (mesh.IsAnimated
                ? Settings.Instance.view.DistanceAnimated1
                : Settings.Instance.view.DistanceStatic1); //0.65f; // 152
            float lod2 = visibility * (mesh.IsAnimated
                ? Settings.Instance.view.DistanceAnimated2
                : Settings.Instance.view.DistanceStatic2); //0.70f; // 102
            float lod3 = visibility * (mesh.IsAnimated
                ? Settings.Instance.view.DistanceAnimated3
                : Settings.Instance.view.DistanceStatic3); //0.75f; // 22

            if (!mesh.HasCentroid)
                mesh.CalculateCentroid(drawablePosition);       // TODO:: HERE I MAY CONVERT MESH CENTROID FROM LOCAL TO WORLDSPACE CENTROID
                                                                //mesh.CalculateCentroid();
            //float distToDrawAble = (mesh.Centroid - info.Position).LengthFast;
            //mesh.DistanceToCamera = ((mesh.CentroidWorld) - info.Position).LengthFast;    // Question: Centroid is in Local or World space? If local add DrawablePosition to convert to World space
            mesh.DistanceToCamera = ((mesh.CentroidWorld) - vi.Position).LengthFast + drawablePosition.LengthFast;    // Question: Centroid is in Local or World space? If local add DrawablePosition to convert to World space

            if (!drawable.IgnoreLod)
            {
                // LOD Level Calculation
                if (mesh.DistanceToCamera /*distToDrawAble*/ < lod0)
                    mesh.CurrentLod = MeshVbo.MeshLod.Level0;

                if (mesh.DistanceToCamera /*distToDrawAble*/  > lod1 && mesh.DistanceToCamera /*distToDrawAble*/  <= lod2)
                    if (mesh.GetPositions(MeshVbo.MeshLod.Level1) != null)
                        mesh.CurrentLod = MeshVbo.MeshLod.Level1;

                if (mesh.DistanceToCamera /*distToDrawAble*/  > lod2 && mesh.DistanceToCamera /*distToDrawAble*/  <= lod3)
                    if (mesh.GetPositions(MeshVbo.MeshLod.Level2) != null)
                        mesh.CurrentLod = MeshVbo.MeshLod.Level2;

                if (mesh.DistanceToCamera /*distToDrawAble*/  > lod3)
                    if (mesh.GetPositions(MeshVbo.MeshLod.Level3) != null)
                        mesh.CurrentLod = MeshVbo.MeshLod.Level3;
            }
            else
                mesh.CurrentLod = MeshVbo.MeshLod.Level0;

            //Vector4     vSpacePos   = GenericMethods.Mult(new Vector4(mesh.Centroid, 1), info.modelviewProjectionMatrix);

            Vector3 mCenter = mesh.CentroidLocal; //mesh.CentroidLocal;
            
            float range = mesh.BoundingSphere;

            if (mesh.DistanceToCamera /*distToDrawAble*/  <= range * 1.1f)
                return true;

            if (mesh.DistanceToCamera /*distToDrawAble*/  - range >= vi.zFar)
            {
#if DEBUG
                ConsoleUtil.log(string.Format("Mesh {0} visible FALSE due to: distToDrawAble - range > info.zFar ", mesh.Name), false);
#endif
                return false;
            }

           
            Vector4 vSpacePos = GenericMethods.Mult(new Vector4(mCenter, 1),  vi.modelviewProjectionMatrix); // Question: Centroid is in Local or World space? If local add DrawablePosition to convert to World space

            //            if (vSpacePos.W <= 0)
            //            {
            //#if DEBUG
            //                //ConsoleUtil.log(string.Format("Mesh {0} visible FALSE due to: vSpacePos.W <= 0 ", mesh.Name));
            //#endif
            //                return false;
            //            }

            range /= vSpacePos.W * 0.6f; // 0.6f

            if (range <= 55 && range > -20) return true;

            if (float.IsNaN(range) || float.IsInfinity(range))
            {
#if DEBUG
                ConsoleUtil.log(string.Format("Mesh {0} visible FALSE due to: float.IsNaN(range) || float.IsInfinity(range) ", mesh.Name), false);
#endif
                return false;
            }

            vSpacePos /= vSpacePos.W;

            // TODO:: FIX
            //BoundingFrustum bF = new BoundingFrustum(info.projectionMatrix);
            //return (bF.Contains(mesh.BoundingBox) == enuContainmentType.Contains || bF.Contains(mesh.BoundingBox) == enuContainmentType.Intersects);

            bool ret = ( //5f causes early culling, use 1
                vSpacePos.X <= (1f + range) &&
                vSpacePos.X >= -(1f + range) &&
                vSpacePos.Y <= (1f + range * vi.aspect) &&
                vSpacePos.Y >= -(1f + range * vi.aspect)
                );

#if DEBUG
            if(!ret)
                ConsoleUtil.log(string.Format("Mesh {0} visible FALSE due to: FINAL CONDITION Range = {1} ", mesh.Name, range), false);
#endif

            //if(!ret && )

            return ret;
            
        }
        static int compareByDistance(Drawable drawableA, Drawable drawableB)
        {
            return drawableA.DistToCamera.CompareTo(drawableB.DistToCamera);
        }
        static int compareByMaterial(Drawable drawableA, Drawable drawableB)
        {
            if (null == drawableA || null == drawableB) return 0;
            if (drawableA.Materials.Count == 0 || drawableB.Materials.Count == 0) return 0;
            return drawableA.Materials[0].CompareTo(drawableB.Materials[0]);
        }

       
#endregion Update

#region draw
       
        // See Scene.Rendering.cs

#endregion draw

    }

    //public class PendingAvatar
    //{
    //    public int Index;
    //    public AvatarInfoDesc Avatar;
    //    public string AvatarLocation;

    //    public PendingAvatar(int index, string location,  AvatarInfoDesc avatar)
    //    {
    //        this.Index              = index;
    //        this.Avatar             = avatar;
    //        this.AvatarLocation     = location;
    //    }
    //}

   
}
