
namespace NthDimension.Rendering.Drawables.Models
{
    using System;
    using System.Xml;
    using System.Drawing;



    using NthDimension.Algebra;
    using Rendering.Animation;
    using Rendering.Geometry;
    using Rendering.Shaders;
    using Rendering.Imaging;
    using Rendering.Loaders;
    using Rendering.Utilities;
    using NthDimension.Network;
    using NthDimension.Rendering.Culling;
    using NthDimension.Rendering.Scenegraph;

    public partial class NonPlayerModel : AnimatedModel, Scenegraph.IClickable
    {
        new public static string nodename = "npcmodel";

        // PlayerModel
        private Matrix4 m_bodyRotationOffset;
        private Vector3 m_bodyPosition
        {
            get { return this.Position; }
            set
            {
                Vector3 svec = value;
                this.Position = Position = svec;
            }
        }
        private Vector3 m_topVertex = new Vector3();
        private bool m_topVertexCalculated = false;

        public Vector3 BodyPosition
        {
            get { return m_bodyPosition; }
            set
            {
                m_bodyPosition = value; // TODO Apply to mesh
            }
        }
        public Matrix4 BodyRotationOffset
        {
            get { return m_bodyRotationOffset; }
            set { m_bodyRotationOffset = value; }
        }
        public bool Visible
        {
            get
            {
                return this.IsVisible;
            }
            set
            {
                this.IsVisible = value;
            }
        }
        public Model[] AttachmentModels
        {
            get
            {
                return new Model[]
                {
                    //avatarFaceModel,
                    //avatarHairModel,
                    //avatarShirtModel,
                    //avatarPantsModel,
                    //avatarShoesModel
                };
            }
        }

       

        public Color SkinColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
        // PlayerRemoteModel
        private object              _lock                           = new object();
        private object              _lock0                          = new object();
        private const float         _LerpFactor                     = 0.005f;

        private Vector3             m_goto;
        private Vector3             m_receivedPositionGoto          = new Vector3();
        private Vector3             m_receivedPositionCurrent       = new Vector3();
        private Matrix4             m_receivedOrientationCurrent    = Matrix4.Identity;

        private volatile bool       m_updateTransformation          = false;

       

        #region Ctor
        public NonPlayerModel(ApplicationObject parent)
            :base(parent)
        {
            //ClickEnabled = true;
            ApplicationBase.Instance.Scene.NonPlayersPending.Add(this);                    
        }
        #endregion Ctor

        #region Top Vertex - Get/Set
        public void SetTopVertex(Vector3 vertex)
        {
            this.m_topVertex = vertex;
            this.m_topVertexCalculated = true;

        }
        public Vector3 GetTopVertex()
        {
            if (this.m_topVertexCalculated)
                return this.m_topVertex;

            return Vector3.Zero;
        }
        #endregion Top Vertex - Get/Set

        #region Update
        public override void Update()
        {
            if (m_updateTransformation)
            {
                this.Position = this.m_receivedPositionCurrent;
                this.Orientation = this.m_receivedOrientationCurrent;
                this.m_goto = this.m_receivedPositionGoto;

                this.m_updateTransformation = false;
            }

            this.Position = Vector3.Lerp(this.Position, this.m_goto, _LerpFactor);
            this.updateBounds();
            base.Update();

        }
        public void UpdateTransformation(Vector3 currentPosition, Vector3 gotoPosition, Matrix4 currentOrientation)
        {
            lock (_lock)
            {
                Vector3 oc = this.m_receivedPositionCurrent;
                Vector3 og = this.m_receivedPositionGoto;
                Matrix4 oo = this.m_receivedOrientationCurrent;

                this.m_receivedPositionCurrent = currentPosition;
                this.m_receivedPositionGoto = gotoPosition;
                this.m_receivedOrientationCurrent = currentOrientation;

                //this.Position = this.m_receivedPositionCurrent;
                //this.Orientation = this.m_receivedOrientationCurrent;
                this.m_updateTransformation = true;

#if DEBUG
                if (oc == this.m_receivedPositionCurrent)
                    NthDimension.Rendering.Utilities.ConsoleUtil.errorlog("Received Pos ", "Failed to Assign");

                if (og == this.m_receivedPositionGoto)
                    NthDimension.Rendering.Utilities.ConsoleUtil.errorlog("Received Goto ", "Failed to Assign");

                if (oo == this.m_receivedOrientationCurrent)
                    NthDimension.Rendering.Utilities.ConsoleUtil.errorlog("Received Ori ", "Failed to Assign");
#endif
            }
        }
        public void SetGotoOnce(Vector3 go)
        {
            this.m_goto = this.m_receivedPositionGoto = go;
        }

        public void SetBodyAnimation(string name)
        {
            this.SetAnimationByName(name, false);
        }
        public void SetBodyMesh(string mesh, string material, string position, string orientation, string size)
        {

            this.setMaterial(material);
            this.setMesh(mesh);
            this.Position = GenericMethods.Vector3FromString(position);
            this.Orientation = m_bodyRotationOffset
                                                //= m_hairRotationOffset
                                                //= m_shirtRotationOffset
                                                //= m_pantsRotationOffset
                                                //= m_shoesRotationOffset
                                                = Matrix4.Identity;
            this.Size = GenericMethods.Vector3FromString(size);

            this.Scene = Scene;



        }
        #endregion

        #region Special Load
        protected override void specialLoad(ref XmlTextReader reader, string type)
        {
            #region from PhysModel
            if (reader.Name == "materials" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Materials = GenericMethods.StringListFromString(reader.Value);
                this.setMaterial(reader.Value);
            }

            if (reader.Name == "meshes" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Meshes = GenericMethods.StringListFromString(reader.Value);
                this.setMesh(reader.Value);
            }

            if (reader.Name == "pboxes" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                PhysBoxes = GenericMethods.StringListFromString(reader.Value);
            }

            if (reader.Name == "isstatic")
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

            if (reader.Name == "isclickable" || reader.Name == "clickable")
                this.ClickEnabled = true;
        }
        #endregion

        

        public void SetAnimationIdle()
        {
            if (AnimationOverrideSettings.Count <= 0) return;

            string avatar_base_animation = AnimationOverrideSettings[0].Name;

            this.SetAnimationByName(avatar_base_animation, false);

            //this.SetAnimation(49);
            //this.avatarShirtModel.SetAnimationByName(string.Format("{0}_tshirt_idle", avatar_base_animation), false);
            //this.avatarPantsModel.SetAnimationByName(string.Format("{0}_leggings_idle", avatar_base_animation), false);
            //this.avatarShoesModel.SetAnimationByName(string.Format("{0}_shoes_idle", avatar_base_animation), false);
        }
        //public void SetAnimationWalk()
        //{
        //    string avatar_base_animation = string.Format("{0}_{1}_{2}",
        //                                                this.AvatarSex == enuAvatarSex.Male ? "male" : "female",
        //                                                this.AvatarBodyType,
        //                                                this.AvatarFaceType);

        //    this.SetAnimationByName(string.Format("{0}_walk", avatar_base_animation), false);
        //    this.avatarShirtModel.SetAnimationByName(string.Format("{0}_tshirt_walk", avatar_base_animation), false);
        //    this.avatarPantsModel.SetAnimationByName(string.Format("{0}_leggings_walk", avatar_base_animation), false);
        //    this.avatarShoesModel.SetAnimationByName(string.Format("{0}_shoes_walk", avatar_base_animation), false);
        //}

    }
}
