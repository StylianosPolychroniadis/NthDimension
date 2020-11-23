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

    public partial class PlayerModel : AnimatedModel
    {
        new public static string    nodename                                = "playermodel";

        internal AvatarFaceModel    avatarFaceModel;
        internal AvatarHairModel    avatarHairModel;

        internal AvatarShirtModel   avatarShirtModel;
        internal AvatarPantsModel   avatarPantsModel;
        internal AvatarShoesModel   avatarShoesModel;


        private Matrix4             m_bodyRotationOffset;         
        private Matrix4             m_faceRotationOffset;
        private Matrix4             m_hairRotationOffset;
        private Matrix4             m_shirtRotationOffset;
        private Matrix4             m_pantsRotationOffset;
        private Matrix4             m_shoesRotationOffset;
        private Vector3             m_bodyPosition
        {
            get { return this.Position; }
            set
            {
                Vector3 svec = value;
                svec.Y = ApplicationBase.Instance.Scene.AvatarYOffset;
                this.Position = Position = svec;
            }
        }
        private Vector3             m_facePosition
        {
            get { return avatarFaceModel.Position; }
            set
            {
                Vector3 svec = value;
                svec.Y = ApplicationBase.Instance.Scene.AvatarYOffset + m_avatarHeadToBodyOffset;
                avatarFaceModel.Position = svec;
            }
        }
        private Vector3             m_hairPosition
        {
            get { return avatarHairModel.Position; }
            set { avatarHairModel.Position = value; }
        }
        private Vector3             m_shirtPosition
        {
            get { return avatarShirtModel.Position; }
            set { avatarShirtModel.Position = value; }
        }
        private Vector3             m_pantsPosition
        {
            get { return avatarPantsModel.Position; }
            set { avatarPantsModel.Position = value; }
        }
        private Vector3             m_shoesPosition
        {
            get { return avatarShoesModel.Position; }
            set { avatarShoesModel.Position = value; }
        }

        private Vector3             m_faceVertexOffset;
        private Vector3             m_hairVertexOffset;
        private Vector3             m_shirtVertexOffset;
        private Vector3             m_pantsVertexOffset;
        private Vector3             m_shoesVertexOffset;
        private Vector3             m_topVertex                             = new Vector3();

        private bool                m_topVertexCalculated                   = false;

        private readonly float      m_avatarHeadToBodyOffset                = 0.0f;
        private readonly float      m_avatarTorsoToBodyOffset               = 0.0f;
        private readonly float      m_avatarLegsToBodyOffset                = 0.0f;
        private readonly float      m_avatarFeetToBodyOffset                = 0.0f;

        #region Properties
        //public List<Attachment>     Attachments                             = new List<Attachment>();

        public Vector3              BodyPosition
        {
            get { return m_bodyPosition; }
            set { m_bodyPosition = value; // TODO Apply to mesh
            }
        }
        public Vector3              FacePosition
        {
            get { return m_facePosition; }
            set
            {
                m_facePosition = value; // TODO Apply to mesh
            }
        }
        public Vector3              HairPosition
        {
            get {  return m_hairPosition; }
            set { m_hairPosition = value; }
        }
        public Vector3              ShirtPosition
        {
            get {  return m_shirtPosition; }
            set { m_shirtPosition = value; }
        }
        public Vector3              PantsPosition
        {
            get {  return m_pantsPosition; }
            set { m_pantsPosition = value; }
        }
        public Vector3              ShoesPosition
        {
            get {  return m_shoesPosition; }
            set { m_shoesPosition = value; }
        }
        
        public Matrix4              BodyRotationOffset
        {
            get {  return m_bodyRotationOffset; }
            set { m_bodyRotationOffset = value; }
        }
        public Matrix4              FaceRotationOffset
        {
            get {  return m_faceRotationOffset; }
            set { m_faceRotationOffset = value; }
        }
        public Matrix4              HairRotationOffset
        {
            get {  return m_hairRotationOffset; }
            set { m_hairRotationOffset = value; }
        }
        public Matrix4              ShirtRotationOffset
        {
            get {  return m_shirtRotationOffset; }
            set { m_shirtRotationOffset = value; }
        }
        public Matrix4              PantsRotationOffset
        {
            get {  return m_pantsRotationOffset; }
            set { m_pantsRotationOffset = value; }
        }
        public Matrix4              ShoesRotationOffset
        {
            get {  return m_shoesRotationOffset; }
            set { m_shoesRotationOffset = value; }
        }

        public bool                 Visible
        {
            get { return this.IsVisible && 
                         avatarFaceModel.IsVisible &&
                         avatarHairModel.IsVisible &&
                         avatarShirtModel.IsVisible &&
                         avatarPantsModel.IsVisible &&
                         avatarShoesModel.IsVisible; }
            set { this.IsVisible =
                  avatarFaceModel.IsVisible =
                  avatarHairModel.IsVisible =
                  avatarShirtModel.IsVisible =
                  avatarPantsModel.IsVisible =
                  avatarShoesModel.IsVisible = value; }
        }
        public string               UserId
                                    { get; set; }

        public enuAvatarSex         AvatarSex;
        public string               AvatarBodyType;
        public string               AvatarFaceType;

        public Model[]              AttachmentModels
        {
            get
            {
                return new Model[]
                {
                    avatarFaceModel,
                    avatarHairModel,
                    avatarShirtModel,
                    avatarPantsModel,
                    avatarShoesModel
                };
            }
        }

        public Color                SkinColor               = System.Drawing.Color.FromArgb(0,0,0,0);
        #endregion

        #region Ctor

        public PlayerModel(ApplicationObject parent)
            : base(parent)
        {
            
        }

        public PlayerModel(ApplicationObject parent, enuAvatarSex sex):this(parent)
        {
            this.AvatarSex = sex;
        }
        #endregion

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

        public void SetBodyMesh(string mesh, string material, string position, string orientation, string size)
        {
            
            this.setMaterial(material);
            this.setMesh(mesh);
            this.Position                       = GenericMethods.Vector3FromString(position);
            this.Orientation                    = m_bodyRotationOffset 
                                                = m_hairRotationOffset
                                                = m_shirtRotationOffset
                                                = m_pantsRotationOffset
                                                = m_shoesRotationOffset
                                                = Matrix4.Identity;    
            this.Size                           = GenericMethods.Vector3FromString(size);
            
            this.Scene                          = Scene;

            

        }
        public void SetFaceMesh(string mesh, string material, string matrixIndex, string vertex, string offset, string scale, string rotation)
        {
            //int faceMatrixIndex                     = Int32.Parse(matrixIndex);     // Not used?
            //string faceVertexPosition               = vertex;                       // not used?

            avatarFaceModel                         = new AvatarFaceModel(this);

            if (material.Contains("DynamicMaterial@") ||
                material.ToLower().Contains("dynamicmaterial@"))
            {
                string faceTexture = string.Empty;
                string dynamicMat = Material.CreateDynamic(material, mesh, out faceTexture);
                string finalMat = dynamicMat.Replace(Configuration.GameSettings.MaterialFolder, "");
                                  


                avatarFaceModel.setMaterial(finalMat);

                #region Face Texture Avg Color

                if (true) // Disabled
                {
                    if (!String.IsNullOrEmpty(faceTexture))
                    {
                        BaseDetector detector = new BaseDetector(new Bitmap(faceTexture), new SimpleSkinDetector4());
                        int avgR = 0;
                        int avgG = 0;
                        int avgB = 0;
                        int validPixelCount = 0;

                        using (Bitmap detected = detector.SkinDetectionImage)
                        {
                            for (int x = 0; x < detected.Width; x++)
                            for (int y = 0; y < detected.Height; y++)
                            {
                                Color c = detected.GetPixel(x, y);
                                if (c.A != 0)
                                {
                                    avgR += c.R;
                                    avgB += c.B;
                                    avgG += c.G;
                                    validPixelCount++;
                                }
                            }
                        }

                        // WARNING: If unnatural colors exist in the image we may get a division by zero
                        if (validPixelCount == 0)
                            this.SkinColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
                        else
                            this.SkinColor = System.Drawing.Color.FromArgb(255,
                            avgR / validPixelCount,
                            avgG / validPixelCount,
                            avgB / validPixelCount);



                        Utilities.ConsoleUtil.log(string.Format("Face AVG Color: {0}", this.SkinColor));
                    }
                    else
                        NthDimension.Rendering.Utilities.ConsoleUtil.errorlog("Calculate face color failed: ", faceTexture);
                }

                #endregion Face Texture Avg Color
            }
            else
            {
                avatarFaceModel.setMaterial(material);
            }


            avatarFaceModel.setMesh(mesh);
            m_facePosition                          = new Vector3(0f, m_avatarHeadToBodyOffset, 0f);
            avatarFaceModel.Orientation             = m_faceRotationOffset 
                                                    = GenericMethods.Matrix4FromString(rotation); 
            avatarFaceModel.Size                    = GenericMethods.Vector3FromString(scale);
            m_faceVertexOffset                      = GenericMethods.Vector3FromString(offset);
            avatarFaceModel.Scene                   = Scene;
        }
        public void SetHairMesh(string mesh, string material, string matrixIndex, string vertex, string offset, string scale, string rotation)
        {
            avatarHairModel                         = new AvatarHairModel(this);
            avatarHairModel.setMaterial(material);
            avatarHairModel.setMesh(mesh);
            m_hairPosition                          = new Vector3(0f, m_avatarHeadToBodyOffset, 0f);
            avatarHairModel.Orientation             = m_hairRotationOffset 
                                                    = GenericMethods.Matrix4FromString(rotation);
            ConsoleUtil.log(string.Format("Hair Orientation: {0}", avatarHairModel.Orientation));
            avatarHairModel.Size                    = GenericMethods.Vector3FromString(scale);
            m_hairVertexOffset                      = GenericMethods.Vector3FromString(offset);
            avatarHairModel.Scene                   = Scene;
        }
        public void SetShirtMesh(string mesh, string material, string matrixIndex, string vertex, string offset, string scale, string rotation)
        {
            avatarShirtModel                        = new AvatarShirtModel(this);
            avatarShirtModel.setMaterial(material);
            avatarShirtModel.setMesh(mesh);
            m_shirtPosition                         = new Vector3(0f, m_avatarTorsoToBodyOffset, 0f);
            avatarShirtModel.Orientation            = m_shirtRotationOffset
                                                    = GenericMethods.Matrix4FromString(rotation);
            avatarShirtModel.Size                   = GenericMethods.Vector3FromString(scale);
            m_shirtVertexOffset                     = GenericMethods.Vector3FromString(offset);
            avatarShirtModel.Scene                  = Scene;
        }
        public void SetPantsMesh(string mesh, string material, string matrixIndex, string vertex, string offset, string scale, string rotation)
        {
            avatarPantsModel                        = new AvatarPantsModel(this);
            avatarPantsModel.setMaterial(material);
            avatarPantsModel.setMesh(mesh);
            m_pantsPosition                         = new Vector3(0f, m_avatarLegsToBodyOffset, 0f);
            avatarPantsModel.Orientation            = m_pantsRotationOffset
                                                    = GenericMethods.Matrix4FromString(rotation);
            avatarPantsModel.Size                   = GenericMethods.Vector3FromString(scale);
            m_pantsVertexOffset                     = GenericMethods.Vector3FromString(offset);
            avatarPantsModel.Scene                  = Scene;
        }
        public void SetShoesMesh(string mesh, string material, string matrixIndex, string vertex, string offset, string scale, string rotation)
        {
            avatarShoesModel                        = new AvatarShoesModel(this);
            avatarShoesModel.setMaterial(material);
            avatarShoesModel.setMesh(mesh);
            m_pantsPosition                         = new Vector3(0f, m_avatarFeetToBodyOffset, 0f);
            avatarShoesModel.Orientation            = m_shoesRotationOffset
                                                    = GenericMethods.Matrix4FromString(rotation);
            avatarShoesModel.Size                   = GenericMethods.Vector3FromString(scale);
            m_shoesVertexOffset                     = GenericMethods.Vector3FromString(offset);
            avatarShoesModel.Scene                  = Scene;
        }

        public void AddAnimationOverride(string animationName, string animationSpeed, string animationPlayback = "once", string animationTransition = "")
        {
            AnimationSettings setting   = new AnimationSettings();
            setting.Name                = animationName;
            setting.Speed               = GenericMethods.FloatFromString(animationSpeed);
            setting.Playback            = animationPlayback;
            setting.Transition          = animationTransition;
            AnimationOverrideSettings.Add(setting);
        }

        // Animation Overrides
        public void SetShirtAnimation(string idle = "", string walk = "", string sit = "", string sitidle = "", string stand = "", string wave = "")
        {
            if (idle    != string.Empty) this.avatarShirtModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(idle));
            if (walk    != string.Empty) this.avatarShirtModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(walk));
            if (sit     != string.Empty) this.avatarShirtModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(sit));
            if (sitidle != string.Empty) this.avatarShirtModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(sitidle));
            if (stand   != string.Empty) this.avatarShirtModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(stand));
            if (wave    != string.Empty) this.avatarShirtModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(wave));
        }
        public void SetPantsAnimation(string idle = "", string walk = "", string sit = "", string sitidle = "", string stand = "", string wave = "")
        {
            if (idle    != string.Empty) this.avatarPantsModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(idle));
            if (walk    != string.Empty) this.avatarPantsModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(walk));
            if (sit     != string.Empty) this.avatarPantsModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(sit));
            if (sitidle != string.Empty) this.avatarPantsModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(sitidle));
            if (stand   != string.Empty) this.avatarPantsModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(stand));
            if (wave    != string.Empty) this.avatarPantsModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(wave));
        }
        public void SetShoesAnimation(string idle = "", string walk = "", string sit = "", string sitidle = "", string stand = "", string wave = "")
        {
            if (idle    != string.Empty) this.avatarShoesModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(idle));
            if (walk    != string.Empty) this.avatarShoesModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(walk));
            if (sit     != string.Empty) this.avatarShoesModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(sit));
            if (sitidle != string.Empty) this.avatarShoesModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(sitidle));
            if (stand   != string.Empty) this.avatarShoesModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(stand));
            if (wave    != string.Empty) this.avatarShoesModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(wave));
        }

        // Main SetAnimation Mechanism
        public override void SetAnimation(int animationIndex, bool updateServer = true)
        {
            base.SetAnimation(animationIndex, updateServer);

            string      curAniName  = this.CurrentAnimationName;
            string[]    curAniParts = this.CurrentAnimationName.Split('_');
            string      gender      = this.AvatarSex == enuAvatarSex.Male ? "male" : "female";


            // Version 1.2.1 - Each Mesh requires it's own animation data
            string shirtAnimation = string.Format("{0}_{1}_{2}_tshirt_{3}",
                                                    gender,
                                                    this.AvatarBodyType,
                                                    this.AvatarFaceType,
                                                    curAniParts[curAniParts.Length - 1]);
            this.avatarShirtModel.SetAnimationByName(shirtAnimation);


            string leggingsAnimation = string.Format("{0}_{1}_{2}_leggings_{3}",
                                                    gender,
                                                    this.AvatarBodyType,
                                                    this.AvatarFaceType,
                                                    curAniParts[curAniParts.Length - 1]);
            this.avatarPantsModel.SetAnimationByName(leggingsAnimation);


            string shoesAnimation = string.Format("{0}_{1}_{2}_shoes_{3}",
                                                    gender,
                                                    this.AvatarBodyType,
                                                    this.AvatarFaceType,
                                                    curAniParts[curAniParts.Length - 1]);
            this.avatarShoesModel.SetAnimationByName(shoesAnimation);


            if (updateServer)
            {
                int ani = Int32.Parse(animationIndex.ToString());
                ApplicationBase.Instance.BroadcastAnimationChange(curAniName, shirtAnimation, leggingsAnimation, shoesAnimation);
            }

        }
        // TODO:: Switch to those function for all control
        public void SetAnimationIdle()
        {
            string avatar_base_animation = string.Format("{0}_{1}_{2}",
                                                        this.AvatarSex == enuAvatarSex.Male ? "male" : "female",
                                                        this.AvatarBodyType,
                                                        this.AvatarFaceType);

            this.SetAnimationByName(string.Format("{0}_idle", avatar_base_animation), false);
            this.avatarShirtModel.SetAnimationByName(string.Format("{0}_tshirt_idle", avatar_base_animation), false);
            this.avatarPantsModel.SetAnimationByName(string.Format("{0}_leggings_idle", avatar_base_animation), false);
            this.avatarShoesModel.SetAnimationByName(string.Format("{0}_shoes_idle", avatar_base_animation), false);
        }
        public void SetAnimationWalk()
        {
            string avatar_base_animation = string.Format("{0}_{1}_{2}",
                                                        this.AvatarSex == enuAvatarSex.Male ? "male" : "female",
                                                        this.AvatarBodyType,
                                                        this.AvatarFaceType);

            this.SetAnimationByName(string.Format("{0}_walk", avatar_base_animation), false);
            this.avatarShirtModel.SetAnimationByName(string.Format("{0}_tshirt_walk", avatar_base_animation), false);
            this.avatarPantsModel.SetAnimationByName(string.Format("{0}_leggings_walk", avatar_base_animation), false);
            this.avatarShoesModel.SetAnimationByName(string.Format("{0}_shoes_walk", avatar_base_animation), false);
        }

        #region Update
        public override void Update()
        {
            base.Update();

            if (this.Meshes.Count == 0)
                return;

            this.updateBounds();

            Vector3 restoreScale    = this.Size;
            this.Size               = new Vector3(1f, 1f, 1f);

           
            if (!m_topVertexCalculated && this.Meshes.Count > 0)
            {
                for (int m = 0; m < this.meshes[0].MeshData.Positions.Length; m++)
                    if (this.meshes[0].MeshData.Positions[m].Y > m_topVertex.Y)
                        m_topVertex = this.meshes[0].MeshData.Positions[m];

                m_topVertexCalculated = true;
            }

            #region Face
            Matrix4         scaleMat            = Matrix4.CreateScale(restoreScale);
            Matrix4         boneMat             = new Matrix4(this.AnimationFrame.ActiveMatrices[5].ToArray());
            Quaternion      boneQtr             = boneMat.ExtractRotation();
            Matrix4         rotaMat             = Matrix4.CreateFromQuaternion(boneQtr);


            Vector3         facePoint       = m_topVertex + m_faceVertexOffset;

            boneMat.TransformVector(ref facePoint);
            scaleMat.TransformVector(ref facePoint);
            this.Orientation.TransformVector(ref facePoint);

            Vector3 facePos = facePoint;

            this.avatarFaceModel.Orientation    = rotaMat * this.Orientation;
            this.avatarFaceModel.Position       = this.Position + facePos;

            this.avatarFaceModel.IgnoreCulling  = true;

            if (this.avatarFaceModel.meshes[0].CurrentLod != MeshVbo.MeshLod.Level0)
                this.avatarFaceModel.meshes[0].CurrentLod = MeshVbo.MeshLod.Level0;

            #endregion

            #region Hair
        


            if (null != avatarHairModel)
            {
                Vector3     hairPoint           = m_topVertex + m_hairVertexOffset;

                boneMat.TransformVector(ref hairPoint);
                scaleMat.TransformVector(ref hairPoint);
                this.Orientation.TransformVector(ref hairPoint);

                Vector3 hairPos = hairPoint;

                avatarHairModel.Orientation     = rotaMat * this.Orientation ;
                avatarHairModel.Position        = this.Position + hairPos;

                this.avatarHairModel.IgnoreCulling = true;

                if (this.avatarHairModel.meshes[0].CurrentLod != MeshVbo.MeshLod.Level0)
                    this.avatarHairModel.meshes[0].CurrentLod = MeshVbo.MeshLod.Level0;
            }

            #endregion

            this.Size               = restoreScale;

            #region Shirt

            if (null != avatarShirtModel)
            {
                avatarShirtModel.Position       = this.Position;
                avatarShirtModel.Orientation    = this.Orientation;
                avatarShirtModel.Update();
            }
            #endregion

            #region Pants

            if (null != avatarPantsModel)
            {
                avatarPantsModel.Position       = this.Position;
                avatarPantsModel.Orientation    = this.Orientation;
                avatarPantsModel.Update();
            }
            #endregion

            #region Shoes

            if (null != avatarShoesModel)
            {
                avatarShoesModel.Position       = this.Position;
                avatarShoesModel.Orientation    = this.Orientation;
                avatarShoesModel.Update();
            }
            #endregion

        }
        #endregion

        #region kill
        public override void kill()
        {
            avatarFaceModel.MarkForDelete = true;
            avatarShirtModel.MarkForDelete = true;
            avatarPantsModel.MarkForDelete = true;
            avatarShoesModel.MarkForDelete = true;

            base.killChilds();
            

        }

        #endregion

        protected override void specialLoad(ref System.Xml.XmlTextReader reader, string type)
        {
            #region from PhysModel
            if (reader.Name.ToLower() == "materials" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Materials = GenericMethods.StringListFromString(reader.Value);
            }

            if (reader.Name.ToLower() == "meshes" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                Meshes = GenericMethods.StringListFromString(reader.Value);
            }

            if (reader.Name.ToLower() == "pboxes" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                PhysBoxes = GenericMethods.StringListFromString(reader.Value);
            }

            if (reader.Name.ToLower() == "isstatic")
                IsStatic = true;

            if (reader.Name.ToLower() == "noshadow")
                CastShadows = false;
            #endregion

            #region animations overrides
          //if (reader.Name.ToLower() == "sequences" && reader.HasAttributes && reader.NodeType != XmlNodeType.EndElement)
            if (reader.Name.ToLower() == "animation" && reader.HasAttributes && reader.NodeType != XmlNodeType.EndElement)
            {
                AnimationSettings setting = new AnimationSettings();

                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name.ToLower() == "name")
                        setting.Name = reader.Value;
                    if (reader.Name.ToLower() == "playback")
                        setting.Playback = reader.Value;
                    if (reader.Name.ToLower() == "speed")
                        setting.Speed = GenericMethods.FloatFromString(reader.Value);
                    if (reader.Name.ToLower() == "transition")
                        setting.Transition = reader.Value;
                }

                AnimationOverrideSettings.Add(setting);
            }
            #endregion

            #region Known Vertices
            if (reader.Name.ToLower() == "vertex" && reader.HasAttributes && reader.NodeType != XmlNodeType.EndElement)
            {
                string key = string.Empty;
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name.ToLower() == "name")
                        key = reader.Value;
                    if (reader.Name.ToLower() == "vertex")
                        if (key != string.Empty)
                            this.KnownVertices.GetOrAdd(key, GenericMethods.Vector3FromString(reader.Value));
                }
            }
            #endregion

            #region Gender
            if (reader.Name.ToLower() == "gender" && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                if(reader.Value.ToLower() == "male")
                    AvatarSex = enuAvatarSex.Male;
                if(reader.Value.ToLower() == "female")
                    AvatarSex = enuAvatarSex.Female;
            }
            #endregion

            // TODO:: Drop - Attachment instead

            #region Apparel (Never tested)
            if (reader.Name.ToLower() == "apparel" && reader.NodeType != XmlNodeType.EndElement)
            {
                while (reader.Read())
                {
                    #region Shoes
                    if(reader.Name.ToLower() == "shoes")
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name.ToLower() == "source")
                                this.avatarShoesModel.meshes[0] = ApplicationBase.Instance.MeshLoader.GetMeshByName(reader.Value.ToString());
                            if(reader.Name.ToLower() == "idle")
                                this.avatarShoesModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name)); 
                            if(reader.Name.ToLower() == "walk")
                                this.avatarShoesModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if(reader.Name.ToLower() == "sit")
                                this.avatarShoesModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if(reader.Name.ToLower() == "stand")
                                this.avatarShoesModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if(reader.Name.ToLower() == "sidle")
                                this.avatarShoesModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if(reader.Name.ToLower() == "wave")
                                this.avatarShoesModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                        }
                    #endregion

                    #region Shirt
                    if (reader.Name.ToLower() == "shirt")
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name.ToLower() == "source")
                                this.avatarShirtModel.meshes[0] = ApplicationBase.Instance.MeshLoader.GetMeshByName(reader.Value.ToString());
                            if (reader.Name.ToLower() == "idle")
                                this.avatarShirtModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if (reader.Name.ToLower() == "walk")
                                this.avatarShirtModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if (reader.Name.ToLower() == "sit")
                                this.avatarShirtModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if (reader.Name.ToLower() == "stand")
                                this.avatarShirtModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if (reader.Name.ToLower() == "sidle")
                                this.avatarShirtModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if (reader.Name.ToLower() == "wave")
                                this.avatarShirtModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                        }
                    #endregion

                    #region Pants
                    if (reader.Name.ToLower() == "pants")
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name.ToLower() == "source")
                                this.avatarPantsModel.meshes[0] = ApplicationBase.Instance.MeshLoader.GetMeshByName(reader.Value.ToString());
                            if (reader.Name.ToLower() == "idle")
                                this.avatarPantsModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if (reader.Name.ToLower() == "walk")
                                this.avatarPantsModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if (reader.Name.ToLower() == "sit")
                                this.avatarPantsModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if (reader.Name.ToLower() == "stand")
                                this.avatarPantsModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if (reader.Name.ToLower() == "sidle")
                                this.avatarPantsModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                            if (reader.Name.ToLower() == "wave")
                                this.avatarPantsModel.meshes[0].AnimationData.Add(ApplicationBase.Instance.AnimationLoader.FromName(reader.Name));
                        }
                    #endregion
                }

            }
            #endregion
        }

        protected override void setSpecialUniforms(ref Shader curShader, ref MeshVbo CurMesh)  // Added Mar-12-18
        {
            base.setSpecialUniforms(ref curShader, ref CurMesh);

            if (curShader.Loaded && CurMesh.Name.ToLower().Contains("body"))
            {
                float r = (SkinColor.R / 255f);
                float g = (SkinColor.G / 255f);
                float b = (SkinColor.B / 255f);
                float a = 1f;

                Vector4 skinColor = new Vector4(r, g, b, a);
                
                curShader.InsertUniform(Uniform.in_skinColor, ref skinColor);

            }
        }
    }
}
