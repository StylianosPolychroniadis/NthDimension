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

//#define HEAD

using NthDimension.Rendering.GameViews;

namespace NthDimension.Rendering.Drawables.Models
{
    using System;
    using System.Text;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using NthDimension.Algebra;
    using Rendering.Animation;
    using Rendering.Geometry;
    using Rendering.Serialization;
    using Rendering.Utilities;

    using System.Xml;
    using NthDimension.Rendering.Loaders;
    using NthDimension.Rendering.Scenegraph;

    public partial class AnimatedModel : PhysModel, IClickable
    {
        new public static string                            nodename                        = "animodel";

        private ListAnimationRuntime                        m_animationsRuntime             = new ListAnimationRuntime();

        #region Properties
        /// <summary>
        /// The total animation frame count 
        /// </summary>
        public float                                        AnimationTotalFrames            = 0f;
        /// <summary>
        /// The current animation data set
        /// </summary>
        public AnimationRuntime                             AnimationFrame
        {
            get;
            private set;
        }
        /// <summary>
        /// Information to override the animation data loaded from Cache
        /// </summary>
        public List<AnimationSettings>                      AnimationOverrideSettings       = new List<AnimationSettings>();
        /// <summary>
        /// The current animation index
        /// </summary>
        public int                                          CurrentAnimation                = 0;
        public string                                       CurrentAnimationName
        {
            get { return this.meshes[0].AnimationData[CurrentAnimation].name; }
        }

        /// <summary>
        /// A dictionary of known vertices to be used as attachment points
        /// The Keys to the collection are the string names of the attachment points (ie 'head')
        /// </summary>
        public ConcurrentDictionary<string, Vector3>        KnownVertices                   = new ConcurrentDictionary<string, Vector3>();

        #endregion

        #region Ctor
        public AnimatedModel(ApplicationObject parent)
            : base(parent)
        {
            this.meshes = new MeshVbo[] {};
        }
        #endregion

        private void                buildAnimationData()
        {
            if (null == this.m_animationsRuntime)
                this.m_animationsRuntime = new ListAnimationRuntime();


            try
            {
                // Default Animations
                for (int d = 0; d < this.meshes[0].AnimationData.Count; d++)
                {
                    if (this.meshes[0].AnimationData[d].Matrices == null) continue; // Weird bug? with incosistencies in AnimationData??? (Oct-05-18)
                    // This is referenced data from AnimationLoader. Should contain animation data all across the models
                    this.m_animationsRuntime.Add(new AnimationRuntime(this.meshes[0].AnimationData[d].identifier,
                        this.meshes[0].AnimationData[d].name,
                        this.meshes[0].AnimationData[d].pointer,
                        this.meshes[0].AnimationData[d].stepSize,
                        this.meshes[0].AnimationData[d].lastFrame,
                        this.meshes[0].AnimationData[d].animationPos,
                        this.meshes[0].AnimationData[d].AnimationSpeed,
                        this.meshes[0].AnimationData[d].activeMatrices,
                        this.meshes[0].AnimationData[d].Matrices));
                }

                // Extended Animations
                for (int i = 0; i < AnimationOverrideSettings.Count; i++)
                {
                    AnimationSettings extended = AnimationOverrideSettings[i];

                    if (null != extended)
                    {

                        AnimationData d = AnimationLoader.Animations.First(xx => xx.name == extended.Name);

                        if (null != d)
                        {
                            AnimationRuntime runTime = new AnimationRuntime(d);
                            if (!this.m_animationsRuntime.Contains(runTime))
                                this.m_animationsRuntime.Add(runTime);

                            if (extended.Default)
                                this.CurrentAnimation = runTime.Identifier;
                        }

                    }
                }
                    

            }
            catch(Exception aE)
            {
                ConsoleUtil.errorlog("Failed to build animation data ", aE != null ? aE.Message : "No amimation frames");
            }
            finally
            {
                this.AnimationFrame = this.m_animationsRuntime[CurrentAnimation];
            }
        }

        #region specialLoad/save
        protected override void specialLoad(ref System.Xml.XmlTextReader reader, string type)
        {
            //base.specialLoad(ref reader, type);       // THIS IS THE CORRECT WAY TO CALL IT

            #region from PhysModel
            if (reader.Name == "materials" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Materials = GenericMethods.StringListFromString(reader.Value);
            }

            if (reader.Name == "meshes" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Meshes = GenericMethods.StringListFromString(reader.Value);
            }

            if (reader.Name == "pboxes" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                PhysBoxes = GenericMethods.StringListFromString(reader.Value);
            }

            if (reader.Name == "isstatic" || reader.Name == "static")
                IsStatic = true;            

            if (reader.Name.ToLower() == "noshadow")
                CastShadows = false;
            #endregion

            #region animations overrides
            if (reader.Name == "animation" && reader.HasAttributes && reader.NodeType != XmlNodeType.EndElement)
            {
                AnimationSettings setting = new AnimationSettings();

                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == "name")
                        setting.Name = reader.Value;
                    if (reader.Name == "playback")
                        setting.Playback = reader.Value;
                    if (reader.Name == "speed")
                        setting.Speed = GenericMethods.FloatFromString(reader.Value);
                    if (reader.Name == "transition")
                        setting.Transition = reader.Value;
                    if (reader.Name == "default")
                        bool.TryParse(reader.Value.ToString(), out setting.Default);
                }

                AnimationOverrideSettings.Add(setting);

            }
            #endregion

            #region Known Vertices
            if (reader.Name == "vertex" && reader.HasAttributes && reader.NodeType != XmlNodeType.EndElement)
            {
                string key = string.Empty;
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == "name")
                        key = reader.Value;
                    if (reader.Name == "vertex")
                        if (key != string.Empty)
                            this.KnownVertices.GetOrAdd(key, GenericMethods.Vector3FromString(reader.Value));
                }
            }
            #endregion

            //                  isclickable
            if (reader.Name == "isclickable" || reader.Name == "clickable")
                this.ClickEnabled = true;
        }

        public override void save(ref StringBuilder sb, int level)
        {
            // reading Object Atrributes and Converting them to Strings
            string position = GenericMethods.StringFromVector3(this.Position);
            string rotation = GenericMethods.StringFromJMatrix(RigidBody.Orientation);
            string stringMaterial = GenericMethods.StringFromStringList(Materials);
            string meshes = GenericMethods.StringFromStringList(Meshes);
            string pboxes = GenericMethods.StringFromStringList(PhysBoxes);

            string tab = GenericMethods.tabify(level - 1);
            string tab2 = GenericMethods.tabify(level);

            sb.AppendLine(tab + "<" + nodename + " name='" + Name + "'>");
            sb.AppendLine(tab2 + "<position>" + position + "</position>");
            sb.AppendLine(tab2 + "<rotation>" + rotation + "</rotation>");
            sb.AppendLine(tab2 + "<materials>" + stringMaterial + "</materials>");
            sb.AppendLine(tab2 + "<meshes>" + meshes + "</meshes>");
            sb.AppendLine(tab2 + "<pboxes>" + pboxes + "</pboxes>");

            // TODO:: Animation Overrides

            if (IsStatic)
                sb.AppendLine(tab2 + "<isstatic/>");



            /*
            // Creating Sql Command
            sb.Append("INSERT INTO WorldObjects (id, name, position, rotation , material, meshes, pboxes, static )" +
                " VALUES(NULL, '" + name + "', '" + position + "', '" + rotation + "' , '" + stringMaterial + "' , '"
                + meshes + "' , '" + pboxes + "' , " + isstatic + ");");

             */

            Utilities.ConsoleUtil.log(string.Format("@ Saving Model: '{0}'", Name));

            saveChilds(ref sb, level);

            sb.AppendLine(tab + "</" + nodename + ">");
        }
        #endregion specialLoad

        #region SetupMatrices
        protected override void SetupMatrices(ref ViewInfo curView, ref Shaders.Shader curShader, ref MeshVbo curMesh)
        {
            base.SetupMatrices(ref curView, ref curShader, ref curMesh);

            if(null == this.AnimationFrame)
                this.buildAnimationData();

            Matrix4[] matrices = this.AnimationFrame.ActiveMatrices;

            if (null == matrices)
                return;

            int bonecount = matrices.Length;
            for (int i = 0; i < bonecount; i++)
            {
                try
                {
                    ApplicationBase.Instance.Renderer.UniformMatrix4(curShader.BoneMatrixLocations[i], false, ref matrices[i]);
                }
                catch (Exception e)
                {
                    ConsoleUtil.errorlog("AnimatedModel.SetupMatrices", e.Message);
                }

                //ConsoleUtil.log(curframe);
            }
        }
        #endregion

        public AnimationRuntime     GetAnimationRuntime(AnimationData data)
        {
            for (int i = 0; i < m_animationsRuntime.Count; i++)
                if (m_animationsRuntime[i].Identifier       == data.identifier &&
                    m_animationsRuntime[i].Name             == data.name &&
                    m_animationsRuntime[i].Pointer          == data.pointer)
                        return m_animationsRuntime[i];

            throw new Exception("Animation data not found");
        }
        public virtual void SetAnimation(int animationIndex, bool updateServer = true)
        {

            if (m_animationsRuntime.Count == 0)
                this.buildAnimationData();

            if (null == this.m_animationsRuntime)
                this.m_animationsRuntime = new ListAnimationRuntime();

            if (this.meshes[0].AnimationData[animationIndex] != null)
            {
                this.AnimationTotalFrames = this.m_animationsRuntime[animationIndex].LastFrame;
                this.AnimationFrame = this.m_animationsRuntime[animationIndex];
                this.CurrentAnimation = animationIndex;
            }

            //System.Diagnostics.Debug.Print("SetAnimation: " + CurrentAnimationName);
#if DEBUG
            ConsoleUtil.log("SetAnimation: " + CurrentAnimationName);
#endif

        }


        public virtual void         SetAnimationByName(string animationName, bool updateServer = true)
        {
            for (int a = 0; a < this.meshes[0].AnimationData.Count; a++)
            {
                if (this.meshes[0].AnimationData[a].name == animationName)
                {
                    this.SetAnimation(this.meshes[0].AnimationData[a].identifier, updateServer);
                    break;
                }
            }
        }

#region Update
        public override void Update()
        {
            base.Update();

#region Update Animation Frame
            for (int i = 0; i < vaoHandle.Length; i++)
            {
                AnimationRuntime animationData = this.AnimationFrame;

                if (null == animationData.Matrices)
                    continue;

#region Override Animation Settings
                if (AnimationOverrideSettings.Count > 0)  
                {
                    int animOverride = AnimationOverrideSettings.FindIndex(a => a.Name == animationData.Name);

                    if (animOverride >= 0)
                    {
                        if (AnimationOverrideSettings[animOverride].Speed > 0f)
                            animationData.AnimationSpeed = AnimationOverrideSettings[animOverride].Speed;

                        if (AnimationOverrideSettings[animOverride].Playback != string.Empty)
                            animationData.Playback = AnimationOverrideSettings[animOverride].Playback;

                        if (AnimationOverrideSettings[animOverride].Transition != string.Empty)
                            animationData.Transition = AnimationOverrideSettings[animOverride].Transition;

                        AnimationOverrideSettings.RemoveAt(animOverride);
                    }
                    else // -1 means we are using an animation other than the default avatar animations (ie idle, walk, etc). These animations are set in Scene.xsn file
                    {
                        AnimationSettings external = AnimationOverrideSettings.FirstOrDefault(a => a.Default);

                        if (null != external)
                        {
                            
                            AnimationData d = AnimationLoader.Animations.First(xx => xx.name == external.Name);

                            if (null != d)
                            {
                                AnimationRuntime runTime = new AnimationRuntime(d);
                                if (!this.m_animationsRuntime.Contains(runTime))
                                    this.m_animationsRuntime.Add(runTime);

                                if (this.AnimationFrame != runTime)
                                    this.AnimationFrame = animationData = runTime;
                            }
                            // else -> let user know no animation loaded
                                

                        }

                    }
                    //else
                    //    ConsoleUtil.errorlog(" Animation Override Warning ", string.Format("{0} does not contain animation '{1}'", this.Name, animationData.Name));
                }
#endregion

                animationData.AnimationPos += ApplicationBase.Instance.VAR_FrameTime_Last * animationData.AnimationSpeed;

                if (animationData.AnimationPos > animationData.LastFrame)
                    animationData.AnimationPos -= animationData.LastFrame;

                int curframe = (int)(animationData.AnimationPos / animationData.StepSize);


                int framecount = animationData.Matrices.Length;           


                if (curframe > framecount - 1 && this.AnimationFrame.Playback == "loop")
                    curframe = framecount - 1;
                if (curframe > framecount - 1 && this.AnimationFrame.Playback == "once")
                    if (this.AnimationFrame.Transition != string.Empty)
                        this.SetAnimationByName(this.AnimationFrame.Transition);

                animationData.ActiveMatrices = animationData.Matrices[curframe];
            }
            
            
#endregion
            wasUpdated = true;

#region moved to PlayerModel
            // ---------------------- FINDING HEAD MATRICES -----------------------
            //#if HEAD

            //            int female_vertex_id_for_face_attachment    = 20841;
            //            int male_vertex_id_for_face_attachment      = 7206;

            //            if (!faceCreated)
            //            {
            //                this.createFaceMode();

            //                for (int m = 0; m < this.meshes[0].MeshData.Positions.Length; m++)
            //                    if (this.meshes[0].MeshData.Positions[m].Y > highest.Y)
            //                        highest = this.meshes[0].MeshData.Positions[m];

            //            }

            //            Vector3 restoreScale        = this.Size;
            //            Vector3 offset              = new Vector3(0f, -.18f, .11f);//new Vector3(0f, 1.25f, .20f);
            //            Matrix4 rotation            = new Matrix4();
            //            Matrix4 scale               = Matrix4.CreateScale(restoreScale);
            //            Quaternion qrot             = new Quaternion();
            //            Vector3 headPos             = new Vector3();

            //            this.Size                   = new Vector3(1f,1f,1f);

            //            Matrix4 boneMat               = new Matrix4(this.AnimationFrame.ActiveMatrices[5].ToArray());

            //            qrot                        = boneMat.ExtractRotation();
            //            rotation                    = Matrix4.CreateFromQuaternion(qrot);

            //            Vector3 lhighest = highest + offset;
            //            Vector3 v = lhighest;
            //            boneMat.TransformVector(ref v);
            //            scale.TransformVector(ref v);
            //            headPos = v;


            //            this.avatarFaceModel.Orientation    = rotation;//Matrix4.CreateFromQuaternion(fmat.ExtractRotation());
            //            this.avatarFaceModel.Position       = this.Position + headPos;


            //            this.avatarFaceModel.IgnoreCulling = true;
            //            if (this.avatarFaceModel.meshes[0].CurrentLod != Mesh.MeshLod.Level0)
            //                this.avatarFaceModel.meshes[0].CurrentLod = Mesh.MeshLod.Level0;
            //            this.avatarFaceModel.Size = new Vector3(1,1,1);
            //            this.Size = restoreScale;
            //#endif
#endregion
        }
#endregion


        //#if HEAD
        //        private Vector3 highest = new Vector3();
        //        private bool                        faceCreated = false;
        //        protected Model                     avatarFaceModel;
        //        private Vector3                     m_avatarScaleOffsetVec = new Vector3(1f, 1f, 1f);
        //        private readonly string             avatarFaceMaterialFile = "test_avatarFace.xmf";            // From Avatar Model Configuration TODO 
        //        private readonly string             avatarFaceMeshFile = "characters\\test_face.obj";    // From Avatar Model Configuration TODO 
        //        private void createFaceMode()
        //        {
        //            avatarFaceModel = new Model(this);
        //            avatarFaceModel.setMaterial(avatarFaceMaterialFile);
        //            avatarFaceModel.setMesh(avatarFaceMeshFile);
        //            //avatarFacePosition = new Vector3(0f, m_avatarHeadToBodyOffset, 0f);
        //            ////avatarFaceModel.Orientation = m_avatarFaceRotationOffset = Matrix4.Identity * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90));
        //            //avatarFaceModel.Orientation = m_avatarFaceRotationOffset = Matrix4.Identity; // * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90));
        //            avatarFaceModel.Size = m_avatarScaleOffsetVec;       // TODO:: Switch to Matrix
        //            avatarFaceModel.IsVisible = true;
        //            avatarFaceModel.Scene = Scene;
        //            faceCreated = true;
        //        }
        //#endif
    }

    public class AnimationSettings
    {
        public string Name;
        public float  Speed;
        public string Playback;
        public string Transition;
        public bool Default;
    }
}

