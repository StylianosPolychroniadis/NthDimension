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

/*  Design note: 
                 This ApplicationObject class facilitates as a Graph Node object
                 I need to revise and extract the Scene-specifics to place them in an (interface?), etc
                 Also this class was originally designed to be the OctreeObject
                 This class should therefore participate in multiple graph systems/graph handlers

*/


using NthDimension.Rendering.Drawables.Lights;
using NthDimension.Rendering.Drawables.Models;

namespace NthDimension.Rendering
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;


    using NthDimension.Algebra;
    using NthDimension.Rendering.Scenegraph;
    using NthDimension.Rendering.Utilities;
    using NthDimension.Physics.Dynamics;
    using NthDimension.Rendering.Drawables.Particles;

    public abstract partial class ApplicationObject // : OctreeObject // This is a GraphNode object 
    {

        #region Members
        // Graph properties
        private ApplicationObject               parent;
        private string                          name                            = "";
        protected int                           childId;
        protected List<ApplicationObject>       childs                          = new List<ApplicationObject> { };

        // Visual Scene Properties
        private SceneGame                           scene;
        protected Vector3                       position;
        protected Vector3                       pointingDirection;
        protected bool                          forceupdate                     = false; //selected = false;
        protected Vector3                       size                            = new Vector3(1, 1, 1);
        protected Vector4                       color                           = new Vector4(1, 1, 1, 1);
        protected Vector3                       colorRgb                        = new Vector3(1, 1, 1);
        public bool                             wasUpdated                      = true;

        // Visual Scene serialization properties
        public static string                    nodename                        = "";
        private const string                    _nodeBloom                      = "bloom";
        private const string                    _nodeAttributeBloomSize         = "curve";          // TODO:: Pass Uniform to bloom curve shader
        private const string                    _nodeAttributeBloomExposure     = "exposure";       // TODO:: Pass Uniform to bloom curve shader
        private const string                    _nodeAttributeBloomStrength     = "strength";       // TODO:: Pass Uniform to bloom curve shader
        private const string                    _nodeskybox                     = "skybox";
        private const string                    _nodesunlight                   = "sunlight";
        private const string                    _nodeMetamodel                  = "metamodel";
        private const string                    _nodetmodel                     = "tmodel"; // terrain
        private const string                    _nodesmodel                     = "smodel";
        private const string                    _nodepmodel                     = "pmodel";
        private const string                    _nodeanimodel                   = "animodel";
        private const string                    _nodeplayermodel                = "playermodel";
        private const string                    _nodenpcmodel                   = "npcmodel";
        private const string                    _nodelamp                       = "lamp";   // todo refactor to spot instead of lamp
        private const string                    _nodeparticles                  = "particles";

        private const string                    _nodeAttributename              = "name";
        private const string                    _nodeAttributeenabled           = "enabled";
        private const string                    _nodeAttributePosition          = "position";
        private const string                    _nodeAttributeRotation          = "rotation";
        private const string                    _nodeAttributePointing          = "pointing";
        private const string                    _nodeAttributeDirection         = "direction";
        private const string                    _nodeAttributeSize              = "size";        
        private const string                    _nodeAttributeColor             = "color";
        #endregion

        #region Properties
        public bool                             Forceupdate { get { return forceupdate; } set { forceupdate = value; } }
        public bool                             MarkForDelete = false;

        public virtual string                   Name
        {
            get { return name; }
            set
            {
                if (scene != null)
                    if (scene.getChild(value) == null)
                        name = value;
                    else
                        name = scene.GetUniqueName();
            }
        }
        public virtual Vector3                  PointingDirection { get { return pointingDirection; } set { pointingDirection = value; } }
        public virtual Vector3                  Position { get { return position; } set { position = value; } }
        public virtual Matrix4                  Orientation { get; set; }
        public virtual Vector3                  Size { get { return size; } set { size = value; } }

        public virtual Vector4                  Color { get { return color; } set { color = value; colorRgb = value.Xyz; } }
        public virtual RigidBody                AvatarBody { get; set; }

        public List<Model>                      Models
        {
            get { return childs.OfType<Model>().ToList(); }
        }

        public virtual SceneGame                    Scene
        {
            get { return scene = ApplicationBase.Instance.Scene; }
            set
            {
                ApplicationBase.Instance.Scene = scene = value;
                if (ApplicationBase.Instance.Scene.getChild(Name) != null && name != "")
                    Name = ApplicationBase.Instance.Scene.GetUniqueName();
            }
        }
        #endregion Properties

        #region constructor
        protected ApplicationObject()
        {

        }
        protected ApplicationObject(ApplicationObject parent)
        {
            Parent = parent;
        }

        #endregion constructor


        #region save
        public virtual void save(ref StringBuilder sb, int level)
        {
            saveChilds(ref sb, level);
        }

        public void saveChilds(ref StringBuilder sb, int level)
        {
            level++;
            foreach (var child in childs)
            {
                try
                {
                    child.save(ref sb, level);
                }
                catch (Exception sE)
                {
                    ConsoleUtil.errorlog(string.Format("Error Saving {0}", child.Name), sE.Message);
                }

            }
        }
        #endregion

        #region load
        protected virtual void load(ref XmlTextReader reader, string type)
        {
            while (reader.Read() && reader.Name != type)
            {
                genericLoad(ref reader, type);

                specialLoad(ref reader, type);
            }
        }

        protected virtual void specialLoad(ref XmlTextReader reader, string type)
        {
        }

        protected void genericLoad(ref XmlTextReader reader, string type)
        {
            #region generic
            if (reader.Name == _nodeAttributePosition && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Position = GenericMethods.Vector3FromString(reader.Value);
            }

            if (reader.Name == _nodeAttributeRotation && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Orientation = GenericMethods.ToOpenTKMatrix(GenericMethods.JMatrixFromString(reader.Value));
            }

            if (reader.Name == _nodeAttributeSize && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Size = GenericMethods.Vector3FromString(reader.Value);
            }

            if (reader.Name == _nodeAttributeDirection && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                PointingDirection = GenericMethods.Vector3FromString(reader.Value);
            }

            if (reader.Name == _nodeAttributeColor && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Color = new Vector4(GenericMethods.Vector3FromString(reader.Value), 1);
            }

            if (reader.Name == _nodeBloom && reader.NodeType != XmlNodeType.EndElement)
            {
                bool bloomEnabled = false;
                if (reader.HasAttributes)
                {
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == _nodeAttributeenabled)
                            bloomEnabled = bool.Parse(reader.Value);
                    }
                }

                Vector2 bloomSize = Scene.BloomSizeDefault;
                float bloomExposure = Scene.BloomExposureDefault;
                float bloomStrength = Scene.BloomStrengthDefault;

                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == _nodeAttributeBloomSize)
                        bloomSize = GenericMethods.Vector2FromString(reader.Value);
                    if (reader.Name == _nodeAttributeBloomExposure)
                        bloomExposure = GenericMethods.FloatFromString(reader.Value);
                    if (reader.Name == _nodeAttributeBloomStrength)
                        bloomStrength = GenericMethods.FloatFromString(reader.Value);
                }

                if (bloomEnabled)
                    Scene.SetBloomEnabled(bloomSize, bloomExposure, bloomStrength);
                else
                    Scene.SetBloomDisabled();
            }
            #endregion

            // TODO:: Repository Pattern

            if (reader.Name == Skybox.nodename)
            {
                string childname = scene.GetUniqueName(); // Common API function call, todo extract functionality

                while (reader.MoveToNextAttribute()) // Common API function call, todo extract functionality
                {
                    if (reader.Name == _nodeAttributename)
                        childname = reader.Value;
                }

                ApplicationObject child = new Skybox(this);
                child.name = childname;
                child.load(ref reader, _nodeskybox);
            }

            if (reader.Name == LightDirectional.nodename)
            {
                string childname = scene.GetUniqueName(); // Common API function call, todo extract functionality
                bool lightEnabled = true;


                while (reader.MoveToNextAttribute()) // Common API function call, todo extract functionality
                {
                    if (reader.Name == _nodeAttributename)
                        childname = reader.Value;
                    if (reader.Name == _nodeAttributeenabled)
                        bool.TryParse(reader.Value, out lightEnabled);
                }

                ApplicationObject child = new LightDirectional(this);
                child.name = childname;
                ((LightDirectional)child).Enabled = lightEnabled;
                child.load(ref reader, _nodesunlight);
            }

            if (reader.Name == LightSpot.nodename)
            {
                string childname = scene.GetUniqueName(); // Common API function call, todo extract functionality

                while (reader.MoveToNextAttribute()) // Common API function call, todo extract functionality
                {
                    if (reader.Name == _nodeAttributename)
                        childname = reader.Value;
                }

                ApplicationObject child = new LightSpot(this);
                child.Name = childname;
                child.load(ref reader, _nodelamp);
                child.wasUpdated = true;
            }

            if (reader.Name == StaticModel.nodename)
            {
                string childname = scene.GetUniqueName(); // Common API function call, todo extract functionality

                while (reader.MoveToNextAttribute()) // Common API function call, todo extract functionality
                {
                    if (reader.Name == _nodeAttributename)
                        childname = reader.Value;
                }

                ApplicationObject child = new StaticModel(this);
                child.Name = childname;
                child.load(ref reader, _nodesmodel);
            }

            if (reader.Name == PhysModel.nodename)
            {
                string childname = scene.GetUniqueName(); // Common API function call, todo extract functionality

                while (reader.MoveToNextAttribute()) // Common API function call, todo extract functionality
                {
                    if (reader.Name == _nodeAttributename)
                        childname = reader.Value;
                }

                ApplicationObject child = new PhysModel(this);
                child.Name = childname;
                child.load(ref reader, _nodepmodel);
            }

            if (reader.Name == AnimatedModel.nodename)
            {
                string childname = scene.GetUniqueName(); // Common API function call, todo extract functionality

                while (reader.MoveToNextAttribute()) // Common API function call, todo extract functionality
                {
                    if (reader.Name == _nodeAttributename)
                        childname = reader.Value;
                }

                ApplicationObject child = new AnimatedModel(this);
                child.Name = childname;
                child.load(ref reader, _nodeanimodel);
            }

            if (reader.Name == PlayerModel.nodename)
            {
                string childname = scene.GetUniqueName(); // Common API function call, todo extract functionality

                while (reader.MoveToNextAttribute()) // Common API function call, todo extract functionality
                {
                    if (reader.Name == _nodeAttributename)
                        childname = reader.Value;
                }

                ApplicationObject child = new AnimatedModel(this);
                child.Name = childname;
                child.load(ref reader, _nodeplayermodel);
            }

            if (reader.Name == NonPlayerModel.nodename)
            {
                string childname = scene.GetUniqueName(); // Common API function call, todo extract functionality

                while (reader.MoveToNextAttribute()) // Common API function call, todo extract functionality
                {
                    if (reader.Name == _nodeAttributename)
                        childname = reader.Value;
                }

                ApplicationObject child = new NonPlayerModel(this);
                child.Name = childname;
                child.load(ref reader, _nodenpcmodel);
            }

#if VOXELS
            if (reader.Name == VoxelModel.nodename)
            {
                string childname = scene.GetUniqueName(); // Common API function call, todo extract functionality

                while (reader.MoveToNextAttribute()) // Common API function call, todo extract functionality
                {
                    if (reader.Name == _nodeAttributename)
                        childname = reader.Value;
                }

                ApplicationObject child = new VoxelModel(this);
                child.Name = childname;
                child.load(ref reader, _nodeMetamodel);
            }
#endif


            if (reader.Name == Terrain.nodename)
            {
                // todo
            }            

            if(reader.Name == ParticleSystem.nodename)
            {
                string childname = scene.GetUniqueName(); // Common API function call, todo extract functionality

                while (reader.MoveToNextAttribute()) // Common API function call, todo extract functionality
                {
                    if (reader.Name == _nodeAttributename)
                        childname = reader.Value;
                }

                ApplicationObject child = new ParticleSystem(this);
                child.Name = childname;
                child.load(ref reader, ParticleSystem.nodename);
                child.wasUpdated = true;

            }

            if (reader.Name == ApplicationScript.nodename)
            {
                string childname = scene.GetUniqueName(); // Common API function call, todo extract functionality

                while (reader.MoveToNextAttribute()) // Common API function call, todo extract functionality
                {
                    if (reader.Name == _nodeAttributename)
                        childname = reader.Value;
                }

                ApplicationObject child = new ApplicationScript();
                child.Name = childname;
                child.load(ref reader, ApplicationScript.nodename);
                child.wasUpdated = true;
            }
            
            
            // Only for debug
            //if(reader.Name == "cull" && reader.NodeType != XmlNodeType.EndElement)
            //{
            //    reader.Read();
            //    /*
            //    glEnable(GL_CULL_FACE); // enables face culling    
            //    glCullFace(GL_BACK); // tells OpenGL to cull back faces (the sane default setting)
            //    glFrontFace(GL_CW); // tells OpenGL which faces are considered 'front' (use GL_CW or GL_CCW)      

            //     */
            //}


        }


        #endregion
    }
}
