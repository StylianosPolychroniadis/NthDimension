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

namespace NthDimension.Rendering.ViewControllers
{
    using System;
    using System.Drawing;
    using NthDimension.Algebra;
    using NthDimension.Rendering.Drawables;
    using NthDimension.Rendering.Drawables.Gui;
    using NthDimension.Rendering.Drawables.Models;
    using NthDimension.Rendering.Utilities;
    using NthDimension.Physics.Collision;
    using NthDimension.Physics.Dynamics;
    using NthDimension.Physics.LinearMath;
    using NthDimension.Network;

    public enum PlayerLogicStates
    {
        Wait,
        Persecution,
        ToTargetPosition,
        InTargetPosition
    }

    public class ThirdPersonViewController : ApplicationUserView
    {
        public new ApplicationUser               Parent
        {
            get { return (ApplicationUser)base.Parent; }
            set
            {
                base.Parent = value;

                if (value != null)
                {
                    Parent = value;
                }

                //Parent.forceUpdate();
            }
        }

        // New Method
        public PlayerModel              Avatar                      = new PlayerModel(ApplicationBase.Instance.Scene);
        
        public string                   AvatarName                  = string.Empty;
        public float                    MouseSensitivity
        {
            get { return m_fineAdjustment; }
            set { m_fineAdjustment = value; }
        }

        public Vector3 Eye {  get { return eye; } }
        // Old method
        // Models
        //protected AnimatedModel         avatarBodyModel;
        //protected Model avatarBodyModel;
        //protected Model                 avatarFaceModel;
        protected bool                  avatarModelLoaded           = false;
        private Model                   avatarGoto;
        private bool                    avatarGotoAlwaysVisible     = false;
        // Avatar Camera
        private Vector3                 eye                         = new Vector3();
        private Vector3                 ava                         = new Vector3();
        private float                   m_theta                     = 15f;
        private float                   m_phi                       = -15f;
        private float                   m_rho                       = 10f;
        private float                   m_lastGoodRho               = 0f;
        private const float             m_phi_minimum               = 0.4f; // 0.8f
        private const float             m_phi_maximum               = 1.84f;//1.64f;
        private Vector3                 m_lastGoodEye               = new Vector3();
        private Matrix4                 m_lastGoodLookAt            = new Matrix4();
        private int                     m_mousePosX                 = 0;
        private int                     m_mousePosY                 = 0;
        private int                     m_oldMousePosX              = 0;
        private int                     m_oldMousePosY              = 0;
        private float                   m_fineAdjustment            = 0.03f;//0.08f;
        private int                     m_prevWheel                 = 0;
        private const float             m_camDistance_minimum       = 2f;
        private const float             m_camDistance_maximum       = 50f; //250f; //100f; // 500f; // 100f
        public const float              m_gotoDistance_maximum      = 6.7f; //5f;
        private Template                template;
        // Avatar Animation
        private PlayerLogicStates       avatarLogicState            = PlayerLogicStates.Wait;
        private PlayerLogicStates       avatarLogicStateOld         = PlayerLogicStates.Wait;
        private Vector3                 avatarGotoPosition          = new Vector3();

        // Avatar Configuration Meta-File
        //  - Offsets
        //private Matrix4                 m_avatarBodyRotationOffset;                                     // From Avatar Model Configuration TODO 
        //private Matrix4                 m_avatarFaceRotationOffset;                                     // From Avatar Model Configuration TODO 
        //private Vector3                 m_avatarScaleOffsetVec      = new Vector3(.02f, .02f, .02f);    // From Avatar Model Configuration TODO 
        private readonly float          m_avatarCameraHeightOffset    = 2.0f;//3.5f; // 3.5 for .025 scale   // From Avatar Model Configuration TODO 
        //private readonly float          m_avatarGroundYOffset       = 1f;                               // From Avatar Model Configuration TODO 
        private readonly float          m_avatarHeadToBodyOffset    = 0.0f;
        // - Mesh & Material Files
        //private readonly string         avatarFaceMaterialFile      = "test_avatarFace.xmf";                                            // From Avatar Model Configuration TODO 
        //private readonly string         avatarBodyMaterialFile      = "test_avatarBody.xmf";                                            // From Avatar Model Configuration TODO 
        //private readonly string         avatarFaceMeshFile          = "characters\\test_face.obj";                                      // From Avatar Model Configuration TODO 
        //private readonly string         avatarBodyMeshFile          = "characters\\models\\male\\male_fat_185_generic.xmd";          // From Avatar Model Configuration TODO 



        #region Ctor
        public ThirdPersonViewController(ApplicationUser player, AvatarInfoDesc avatar,  ApplicationUserInput gameInput) : base(player, gameInput)
        {
#if _DEVUI_
            return;
#endif

            base.raycastCallback = new RaycastCallback(mRaycastCallback);

            avatarModelLoaded = createAvatarModel(avatar);
            //avatarModel.Position = spawnOffset;

            template = ApplicationBase.Instance.TemplateLoader.getTemplate(0);
        }
        #endregion

        public override void Update()
        {
#if _DEVUI_
            return;
#endif

            if (Parent.PlayerViewMode == ApplicationUser.enuPlayerViewMode.FirstPerson)
                return;

            Position = Parent.Position;

            this.updateThirdPersonView();

            switch (avatarLogicState)
            {
                case PlayerLogicStates.Wait:

                    break;
                case PlayerLogicStates.ToTargetPosition:
                    if (TargetPositionReached())
                        ChangeLogicStateTo(PlayerLogicStates.InTargetPosition);

                    break;
            }

        }

        private bool createAvatarModel(AvatarInfoDesc avatarInfo)
        {
#if _DEVUI_
            return false;
#endif

            if (avatarModelLoaded)
                return true;

            AvatarName = avatarInfo.AvatarName;

            Avatar.UserId = avatarInfo.UserId;
            Avatar.AvatarSex = avatarInfo.AvatarSex;
            Avatar.AvatarBodyType = avatarInfo.BodyType;
            Avatar.AvatarFaceType = avatarInfo.FaceType;

            Avatar.SetBodyMesh(avatarInfo.BodyMesh,
                                avatarInfo.BodyMaterial,
                                avatarInfo.SpawnAt,
                                avatarInfo.BodyRotation,
                                avatarInfo.BodySize);
            Avatar.BodyRotationOffset = GenericMethods.Matrix4FromString(avatarInfo.BodyRotation);

            #region Face - Index 0
            if (null != avatarInfo.Attachments[0])
            {
                Avatar.SetFaceMesh(avatarInfo.Attachments[0].Mesh,
                    avatarInfo.Attachments[0].Material,
                    avatarInfo.Attachments[0].Matrix,
                    avatarInfo.Attachments[0].Vertex,
                    avatarInfo.Attachments[0].Offset,
                    avatarInfo.Attachments[0].Size,
                    avatarInfo.Attachments[0].Orientation);
                Avatar.FaceRotationOffset = GenericMethods.Matrix4FromString(avatarInfo.Attachments[0].Orientation);
            }
            #endregion

            #region Hair - Index 1
            if (null != avatarInfo.Attachments[1])
            {
                Avatar.SetHairMesh(avatarInfo.Attachments[1].Mesh,
                    avatarInfo.Attachments[1].Material,
                    avatarInfo.Attachments[1].Matrix,
                    avatarInfo.Attachments[1].Vertex,
                    avatarInfo.Attachments[1].Offset,
                    avatarInfo.Attachments[1].Size,
                    avatarInfo.Attachments[1].Orientation);
                Avatar.HairRotationOffset = GenericMethods.Matrix4FromString(avatarInfo.Attachments[1].Orientation);
            }
            #endregion

            #region Shirt - Index 2
            if (null != avatarInfo.Attachments[2])
            {
                Avatar.SetShirtMesh(avatarInfo.Attachments[2].Mesh,
                    avatarInfo.Attachments[2].Material,
                    avatarInfo.Attachments[2].Matrix,
                    avatarInfo.Attachments[2].Vertex,
                    avatarInfo.Attachments[2].Offset,
                    avatarInfo.Attachments[2].Size,
                    avatarInfo.Attachments[2].Orientation);
                Avatar.ShirtRotationOffset = GenericMethods.Matrix4FromString(avatarInfo.Attachments[2].Orientation);
            }
            #endregion

            #region Pants - Index 3
            if (null != avatarInfo.Attachments[3])
            {
                Avatar.SetPantsMesh(avatarInfo.Attachments[3].Mesh,
                    avatarInfo.Attachments[3].Material,
                    avatarInfo.Attachments[3].Matrix,
                    avatarInfo.Attachments[3].Vertex,
                    avatarInfo.Attachments[3].Offset,
                    avatarInfo.Attachments[3].Size,
                    avatarInfo.Attachments[3].Orientation);
                Avatar.PantsRotationOffset = GenericMethods.Matrix4FromString(avatarInfo.Attachments[3].Orientation);
            }
            #endregion

            #region Shoes - Index 4
            if (null != avatarInfo.Attachments[4])
            {
                Avatar.SetShoesMesh(avatarInfo.Attachments[4].Mesh,
                    avatarInfo.Attachments[4].Material,
                    avatarInfo.Attachments[4].Matrix,
                    avatarInfo.Attachments[4].Vertex,
                    avatarInfo.Attachments[4].Offset,
                    avatarInfo.Attachments[4].Size,
                    avatarInfo.Attachments[4].Orientation);
                Avatar.HairRotationOffset = GenericMethods.Matrix4FromString(avatarInfo.Attachments[4].Orientation);
            }
            #endregion



            Avatar.SetAnimationByName(string.Format("{0}_{1}_{2}_idle", Avatar.AvatarSex == enuAvatarSex.Male ? "male" : "female",
                                                                        Avatar.AvatarBodyType,
                                                                        Avatar.AvatarFaceType));

            avatarGoto = new Model(this);
            avatarGoto.setMaterial("firstperson\\goto.xmf");
            avatarGoto.setMesh("base\\grid.obj");
            avatarGoto.Color = Parent.Color;           // TODO:: Player Color
            avatarGoto.Position = Position;
            avatarGoto.Scene = Scene;
            avatarGoto.Renderlayer = Drawable.RenderLayer.Transparent;
            avatarGoto.Parent = this.Parent;

            m_theta = 1.63f;
            m_phi = 1.22f;

            avatarModelLoaded = true;
            return true;
        }

        public override Drawable[] GetAvatarModels()
        {
            return new Drawable[] { this.Avatar };
        }

        #region Update View
        private void updateThirdPersonView()
        {
            Avatar.Visible = true;
            
            if (!wasActive)
                startUsing();

            if (this.GameInput != null)
            {

                if (!ApplicationBase.Instance.CursorOverGUI()) //if(true)
                {
                    #region Wheel

                    if (m_prevWheel != GameInput.GetMouseWheel())
                    {
                        if (m_prevWheel < GameInput.GetMouseWheel())
                            m_rho++;
                        if (m_prevWheel > GameInput.GetMouseWheel())
                            m_rho--;

                        m_prevWheel = GameInput.GetMouseWheel();

                        // TODO:: This brings up the need for callback mechanisms
                        //foreach (Drawables.Lights.LightSpot t in ApplicationBase.Instance.Scene.Drawables)
                        //    t.wasUpdated = true;
                    }

                    #endregion

                    #region (RIGHT) Mouse Button and θ, φ angles

                    if (GameInput.MouseButtonRight)
                    {
                        if (!GameInput.CursorLock)
                        {
                            GameInput.ThirdPersonCursorLock(true);
                            GameInput.MouseDelta.X = 0;
                            GameInput.MouseDelta.Y = 0;
                            m_mousePosX = 0;
                            m_mousePosY = 0;
                            m_oldMousePosX = 0;
                            m_oldMousePosY = 0;

                            while (GameInput.MouseDelta.X != 0 && GameInput.MouseDelta.Y != 0)
                            {
                                //Scene.DoF_Motion = true;
                                GameInput.ThirdPersonCursorLock(true);
                            }

                            return;
                        }

                        m_mousePosX += GameInput.MouseDelta.X;
                        m_mousePosY += GameInput.MouseDelta.Y;

                        //ConsoleUtil.log(string.Format("Mouse Delta X: {0} Y: {1} New X: {2} Y {3}", GameInput.MouseDelta.X, GameInput.MouseDelta.Y, m_mousePosX, m_mousePosY));

                        m_theta     += (float) Math.PI*((float) (m_mousePosX - m_oldMousePosX)/180f)*m_fineAdjustment;
                        m_phi       += (float) Math.PI*((float) (m_mousePosY - m_oldMousePosY)/180f)*m_fineAdjustment;
                        // TODO Settings.Game.InvertMouse

                        // No horizontal (θ) constraint
                        

                        // Vertical Constraint
                        if (m_phi < m_phi_minimum) m_phi = m_phi_minimum;
                        if (m_phi > m_phi_maximum) m_phi = m_phi_maximum;

                        if (!m_mousePosX.Equals(m_oldMousePosX))
                        {
                            m_oldMousePosX = m_mousePosX;
                            //Scene.DoF_Motion = true;
                        }

                        if (!m_mousePosY.Equals(m_oldMousePosY))
                        {
                            m_oldMousePosY = m_mousePosY;
                            //Scene.DoF_Motion = true;
                        }
                    }
                    else
                    {
                        if (GameInput.CursorLock)
                            GameInput.ThirdPersonCursorLock(false);

                        //Scene.DoF_Motion = false;   
                    }

                    #endregion
                }

                #region Avatar Model Position and LookAt
                ava = Avatar.BodyPosition;
                float avaY = ava.Y + ApplicationBase.Instance.Scene.AvatarYOffset + m_avatarCameraHeightOffset;
                ava.Y = avaY;

                Vector3 up = new Vector3(0f, 1f, 0f);
                eye = thirdPersonEyePosition(ava, m_rho, m_phi, m_theta);
                Matrix4 lookAt = Matrix4.LookAt(eye, ava, up);
                #endregion

                #region Camera Viewing Distance Constraint

                float distance = (eye - ava).LengthFast;

                if (distance > m_camDistance_minimum && distance < m_camDistance_maximum)
                {
                    m_lastGoodRho = m_rho;
                    m_lastGoodEye = eye;
                    m_lastGoodLookAt = lookAt;
                }
                else
                {
                    m_rho = m_lastGoodRho;
                    eye = m_lastGoodEye;
                    lookAt = m_lastGoodLookAt;
                }

                #endregion

                if (lookAt.ToString().Contains("NaN"))
                {
#if DEBUG
                    ConsoleUtil.errorlog("(!) lookAt matrix ",
                        string.Format("Matrix contains NaN value. Eye minus(-) avatar: {0}", eye - ava));
#endif
                    return;
                }

                //if (!ApplicationBase.Instance.CursorOverGUI())
                //if(true)
                {
                    // Avatar Goto Target
                    #region Physic World RayCast

                    Matrix4 mmodelview = lookAt;
                    Matrix4 mprojection = Parent.ViewInfo.projectionMatrix; //  Matrix4.CreatePerspectiveFieldOfView(((Player)Parent).ViewInfo.fovy, ((Player)Parent).ViewInfo.aspect, ((Player)Parent).ViewInfo.zNear, ((Player)Parent).ViewInfo.zFar);

                    Point p = new Point(GameInput.MouseX, GameInput.MouseY);
                    Point biasp = new Point(p.X,
                                            p.Y);   // TODO:: Y appears lower than cursor

                    Vector3 v1 = this.MousePickRayStart = RayHelper.UnProject(mprojection,
                                                        mmodelview,
                                                        new Size(ApplicationBase.Instance.Width, ApplicationBase.Instance.Height),
                                                        new Vector3(biasp.X, biasp.Y, -1.5f));

                    Vector3 v2 = this.MousePickRayEnd =  RayHelper.UnProject(mprojection,
                                                        mmodelview,
                                                        new Size(ApplicationBase.Instance.Width, ApplicationBase.Instance.Height),
                                                        new Vector3(biasp.X, biasp.Y, 1f));

                    #region Ray
                    RigidBody body; JVector normal; float frac;

                    bool result = Scene.CollisionRaycast(GenericMethods.FromOpenTKVector(v1), GenericMethods.FromOpenTKVector(v2),
                        raycastCallback, out body, out normal, out frac);

                    Vector3 hitCoords = v1 + v2 * frac;

                    #region MAX Distance Constraint
                    float length = 0f;
                    if((length=(Avatar.Position - hitCoords).LengthFast) > m_gotoDistance_maximum)
                    {
                        Vector3 diff = Avatar.Position - hitCoords;
                        diff.Normalize();
                        diff *= -1;
                        hitCoords = Avatar.Position + diff * m_gotoDistance_maximum;
                    }
                    #endregion MAX Distance Constraint

                    if (result && !ApplicationBase.Instance.CursorOverAvatar)
                    {
                        float smoothness = 0.9f;

                        Matrix4 newOri = GenericMethods.MatrixFromVector(normal);
                        Vector3 newPos = hitCoords + GenericMethods.ToOpenTKVector(normal) * template.positionOffset;

                        if (GameInput.MouseButtonLeft)
                        {
                            if (avatarGotoAlwaysVisible)
                                avatarGoto.Position = smoothness * avatarGoto.Position + (1 - smoothness) * hitCoords;
                            else
                                avatarGoto.Position = hitCoords;
                            avatarGoto.Orientation =
                                GenericMethods.BlendMatrix(avatarGoto.Orientation, newOri, smoothness);
                        }

                        //ConsoleUtil.log(string.Format("Cursor on {0} at {1}", body.Shape,  body.Position));
                    }

                    #endregion Ray


                    #endregion

                    //ConsoleUtil.log(string.Format("Distance eye to target {0}", (avatarGoto.Position - eye).Length));

                    #region Mouse Button Move
                    if (GameInput.MouseButtonLeft && !ApplicationBase.Instance.CursorOverAvatar)
                    {
                        mouseMove(avatarGoto.Position);

                        // TODO -MOVE OUTSIDE OF SCOPE
                        ApplicationBase.Instance.BroadcastTransformationChange(Avatar.Position, avatarGoto.Position, Avatar.Orientation);
#if DEBUG
                        //ConsoleUtil.log(string.Format("New point at {0} m", (avatarGoto.Position - Avatar.Position).LengthFast));
#endif
                    }
                    #endregion
                }


                if (Parent.PlayerViewMode != ApplicationUser.enuPlayerViewMode.ThirdPerson)
                {
                    Scene.EyePos                    = Position                          = Parent.Position = Parent.ViewInfo.Position = eye; // ava;
                }
                else
                {
                    Scene.EyePos                    = Parent.ViewInfo.Position          = eye;
                    Position                        = Parent.Position                   = ava;
                }
                PointingDirection                   = Parent.PointingDirection          = Parent.ViewInfo.PointingDirection = (ava - eye);
                Orientation                         = Parent.Orientation                = Matrix4.CreateFromQuaternion(lookAt.ExtractRotation());


                Parent.ViewInfo.modelviewMatrix     = lookAt;
                Parent.ViewInfo.invModelviewMatrix  = Matrix4.Invert(lookAt);

                Parent.ViewInfo.GenerateViewProjectionMatrix();

                Parent.ViewInfo.calculateVectors();
                
                //Parent.ViewInfo.wasUpdated = true;
            }
            wasActive = true;
            updateChilds();
        }

    
        private Vector3 thirdPersonEyePosition(Vector3 avatarPosition, float rho, float φ, float θ)
        {
            Vector3 ret = new Vector3();
            ret.X = avatarPosition.X + rho * (float) Math.Sin(φ) * (float)Math.Cos(θ);
            ret.Y = avatarPosition.Y + rho * ((float)Math.Cos(φ));
            ret.Z = avatarPosition.Z + rho * ((float)Math.Sin(φ) * (float)Math.Sin(θ));

            return ret;
        }

        #region Third Person Move

        private void mouseMove(Vector3 pos)
        {
            if (!avatarGotoAlwaysVisible)
            {
                if (GameInput.MouseButtonLeft)
                    avatarGotoPosition = avatarGoto.Position;
            }
            else
                avatarGotoPosition = avatarGoto.Position;

            float angle = (float)GenericMethods.AngleXZ(Avatar.Position, avatarGotoPosition);
            Avatar.Orientation = Avatar.BodyRotationOffset * Matrix4.CreateRotationY(-angle);
            //Avatar.avatarFaceModel.Orientation = Avatar.Orientation * Avatar.FaceRotationOffset * Matrix4.CreateRotationY(-angle);
            ChangeLogicStateTo(PlayerLogicStates.ToTargetPosition);
        }
        private void keyboardMove(Matrix4 lookat)
        {
            float smoothness = 0.3f;
            float RightZ = lookat.M13;
            float UpZ = lookat.M23;
            float BackZ = lookat.M33;
            float RightX = lookat.M11;
            float UpX = lookat.M21;
            float BackX = lookat.M31;
#pragma warning disable CS0219
            bool keyPressed = false;

            float cameraSpeed = 3f;
            Vector3 moveVec = new Vector3();

            if (GameInput.MOVEFORWARD)
            {
                keyPressed = true;
                Avatar.BodyPosition += new Vector3(-RightZ * smoothness, ApplicationBase.Instance.Scene.AvatarYOffset, -BackZ * smoothness);
                moveVec.X = cameraSpeed;
            }
            if (GameInput.MOVEBACKWARD)
            {
                keyPressed = true;
                Avatar.BodyPosition += new Vector3(RightZ * smoothness, ApplicationBase.Instance.Scene.AvatarYOffset, BackZ * smoothness);
                moveVec.X = -cameraSpeed;
            }
            if (GameInput.STRAFELEFT)
            {
                keyPressed = true;
                Avatar.BodyPosition += new Vector3(-RightX * smoothness, ApplicationBase.Instance.Scene.AvatarYOffset, -BackX * smoothness);
                moveVec.Z = -cameraSpeed;
            }
            if (GameInput.STRAFERIGHT)
            {
                keyPressed = true;
                Avatar.BodyPosition += new Vector3(RightX * smoothness, ApplicationBase.Instance.Scene.AvatarYOffset, BackX * smoothness);
                moveVec.Z = cameraSpeed;
            }
            if (GameInput.JUMP)
            {
                moveVec.Y = cameraSpeed;
            }

            

            //if (keyPressed)
            //{
                //Quaternion qa = Avatar.Orientation.ExtractRotation();
                //Quaternion qb = lookat.ExtractRotation();

                //Vector3 va = GenericMethods.GetPitchYawRollDeg(qa);
                //Vector3 vb = GenericMethods.GetPitchYawRollDeg(qb);

                //Avatar.Orientation = Avatar.BodyRotationOffset * Matrix4.CreateRotationY(-(clampAngle(vb.Y, 0, 360) - clampAngle(va.Y, 0, 360)));
                //Avatar.avatarFaceModel.Orientation = Avatar.FaceRotationOffset * Matrix4.CreateRotationY(-(clampAngle(vb.Y, 0, 360) - clampAngle(va.Y, 0, 360)));

                ////ConsoleUtil.log(string.Format("Y1: {0} - Y2: {1} = {2}", clampAngle(vb.Y, 0, 360), clampAngle(va.Y, 0, 360), clampAngle(vb.Y, 0, 360) - clampAngle(va.Y, 0, 360)));
                ////
                //// moveAvatar(moveVec);

                ////Vector3 addVec = new Vector3(parent.Body.Position.X, parent.Body.Position.Y, parent.Body.Position.Z) * (1 - smoothness);
                ////Quaternion dirq = lookat.Inverted().ExtractRotation();
                ////Vector3 dirvec = GenericMethods.GetPitchYawRollDeg(dirq);
                ////Matrix4 mrot = Matrix4.CreateRotationY(dirvec.Y);

                ////Vector3 mvec = GenericMethods.Mult(mrot, new Vector4(addVec.X, addVec.Y, addVec.Z, 1f)).Xyz;

                ////avatarPosition = avatarPosition * smoothness + mvec;

           // }
        }
        private void moveAvatar(Vector3 move)
        {
            //RigidBody hitbody;
            //JVector normal;
            //float frac;

            //bool result = Scene.world.CollisionSystem.Raycast(GenericMethods.FromOpenTKVector(Position),
            //                                                         new JVector(0, -1f, 0),
            //                                                         mRaycastCallback,
            //                                                         out hitbody,
            //                                                         out normal,
            //                                                         out frac);
            //if (result && frac < 2.2f)
            //{
            //    Parent.Body.AddForce(GenericMethods.FromOpenTKVector(move.X * Parent.VectorFwd + parent.VectorRight * move.Z));
            //    if (move.Y > 0)
            //        parent.Body.LinearVelocity = new JVector(parent.Body.LinearVelocity.X, 5, parent.Body.LinearVelocity.Z);

            //}
            //else
            //    parent.Body.AddForce(GenericMethods.FromOpenTKVector(move.X * parent.VectorFwd + parent.VectorRight * move.Z));

        }

        private float clampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return MathHelper.Clamp(angle, min, max);
        }
        #endregion



        #endregion

        #region EnterView
        public override void EnterView(Vector3 pos)
        {
            //if (((Player)Parent).PlayerViewMode == Player.enuPlayerViewMode.FirstPerson)
            //    return;

            GameInput.CursorLock      = false;
            Avatar.Visible              = true;
            Avatar.BodyPosition          = Position;
            Avatar.FacePosition          = new Vector3(Position.X, Position.Y + m_avatarHeadToBodyOffset, Position.Z);
            this.startUsing();
        }
        #endregion

        #region LeaveView
        public override void LeaveView(ref Vector3 pos)
        {
            pos                         = Position;
            pos.Y                       += 2f;
            wasActive                   = false;
            Avatar.Visible   = false;
        }
        #endregion
        protected void startUsing()
        {
            Avatar.BodyPosition = Position;
        }
        

        #region TargetPositionReached
        protected bool TargetPositionReached()
        {
            Vector3 vdiff = Vector3.Subtract(Avatar.Position, avatarGotoPosition);

            if (vdiff.LengthFast > .5f)
            {
                Avatar.Position = Vector3.Lerp(Avatar.Position, avatarGotoPosition, 0.005f); // Moved up - debugging positioning mismatch

                if (Avatar.CurrentAnimationName !=
                    (Avatar.AvatarSex == enuAvatarSex.Male
                        ? string.Format("male_{0}_{1}_walk", Avatar.AvatarBodyType, Avatar.AvatarFaceType)
                        : string.Format("female_{0}_{1}_walk", Avatar.AvatarBodyType, Avatar.AvatarFaceType)))
                {
                    Avatar.SetAnimationByName(Avatar.AvatarSex == enuAvatarSex.Male
                        ? string.Format("male_{0}_{1}_walk", Avatar.AvatarBodyType, Avatar.AvatarFaceType)
                        : string.Format("female_{0}_{1}_walk", Avatar.AvatarBodyType, Avatar.AvatarFaceType));

                    Avatar.SetShirtAnimation(Avatar.AvatarSex == enuAvatarSex.Male
                        ? string.Format("male_{0}_{1}_tshirt_walk", Avatar.AvatarBodyType, Avatar.AvatarFaceType)
                        : string.Format("female_{0}_{1}_tshirt_walk", Avatar.AvatarBodyType, Avatar.AvatarFaceType));

                    Avatar.SetPantsAnimation(Avatar.AvatarSex == enuAvatarSex.Male
                        ? string.Format("male_{0}_{1}_leggings_walk", Avatar.AvatarBodyType, Avatar.AvatarFaceType)
                        : string.Format("female_{0}_{1}_leggings_walk", Avatar.AvatarBodyType, Avatar.AvatarFaceType));

                    Avatar.SetShoesAnimation(Avatar.AvatarSex == enuAvatarSex.Male
                        ? string.Format("male_{0}_{1}_shoes_walk", Avatar.AvatarBodyType, Avatar.AvatarFaceType)
                        : string.Format("female_{0}_{1}_shoes_walk", Avatar.AvatarBodyType, Avatar.AvatarFaceType));

                }

                avatarGoto.IsVisible = true;

                return false;
            }
            else
            {
                //if (Avatar.AvatarSex == enuAvatarSex.Male)
                //    if (Avatar.CurrentAnimationName != string.Format("male_{0}_{1}_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType))
                //        Avatar.SetAnimationByName(string.Format("male_{0}_{1}_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType));


                //if(Avatar.AvatarSex == enuAvatarSex.Female)
                //    if (Avatar.CurrentAnimationName != string.Format("female_{0}_{1}_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType))
                //        Avatar.SetAnimationByName(string.Format("female_{0}_{1}_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType));
                if (Avatar.CurrentAnimationName !=
                    (Avatar.AvatarSex == enuAvatarSex.Male
                        ? string.Format("male_{0}_{1}_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType)
                        : string.Format("female_{0}_{1}_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType)))
                {
                    Avatar.SetAnimationByName(Avatar.AvatarSex == enuAvatarSex.Male
                        ? string.Format("male_{0}_{1}_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType)
                        : string.Format("female_{0}_{1}_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType));

                    Avatar.SetShirtAnimation(Avatar.AvatarSex == enuAvatarSex.Male
                        ? string.Format("male_{0}_{1}_tshirt_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType)
                        : string.Format("female_{0}_{1}_tshirt_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType));

                    Avatar.SetPantsAnimation(Avatar.AvatarSex == enuAvatarSex.Male
                        ? string.Format("male_{0}_{1}_leggings_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType)
                        : string.Format("female_{0}_{1}_leggings_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType));

                    Avatar.SetShoesAnimation(Avatar.AvatarSex == enuAvatarSex.Male
                        ? string.Format("male_{0}_{1}_shoes_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType)
                        : string.Format("female_{0}_{1}_shoes_idle", Avatar.AvatarBodyType, Avatar.AvatarFaceType));

                }

                avatarGoto.IsVisible = false;


            }
            return true;
        }
        #endregion

        protected void ChangeLogicStateTo(PlayerLogicStates logState)
        {
            avatarLogicStateOld = avatarLogicState;
            avatarLogicState = logState;
            if (avatarLogicStateOld != avatarLogicState)
                OnChangeLogicState();
        }
        public virtual void OnChangeLogicState() { }


        
      
    }
}
