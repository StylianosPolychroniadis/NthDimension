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

using System.Collections.Generic;


using NthDimension.Algebra;
using NthDimension.Rasterizer;
using NthDimension.Network;
using NthDimension.Rendering.Drawables.Gui;
using NthDimension.Rendering.Drawables.Tools;
using NthDimension.Rendering.GameViews;
using NthDimension.Rendering.Scenegraph;
using NthDimension.Rendering.ViewControllers;
using NthDimension.Physics.Dynamics.Constraints;
using NthDimension.Physics.Collision.Shapes;
using NthDimension.Physics.Dynamics;
using NthDimension.Physics.Dynamics.Constraints.SingleBody;
using NthDimension.Physics.LinearMath;


namespace NthDimension.Rendering
{
    public class ApplicationUser : ApplicationObject
    {
        public enum enuPlayerViewMode
        {
            FirstPerson,
            FirstPersonVehicle,
            ThirdPerson,
            ThirdPersonVR
        }
        protected NthDimension.Physics.Dynamics.Constraints.SingleBody.FixedAngle mConstraint;
        protected RigidBody                         _body;
        protected RendererBaseGL3                      _renderer;

        private ApplicationUserInput                       gameInput;
        private bool                                prevC_KeyPress;

        #region Properties

        public RendererBaseGL3 Renderer {
            get { return _renderer; }
        }
        public enuPlayerViewMode                    PlayerViewMode      = enuPlayerViewMode.ThirdPerson;
        public override RigidBody                   AvatarBody
                                                    { get { return _body; } set { _body = value; forceUpdate(); } }
        public Hud                                  Hud;
        public Vector3                              RotationRaw;
        public Vector3                              Rotation;
        public Vector3                              VectorUp;
        public Vector3                              VectorFwd;
        public Vector3                              VectorRight;
        //public Vector4                              Color   = new Vector4(0.8f, 0.3f, 0.8f, 1.0f) * 0.3f;

        public ViewInfo                             ViewInfo;   // TODO:: Refactor -> Move to both FirstPersonView and ThirdPersonView

        #region REFACTORING POINT
        // TODO:: Refactor to enable multiple player-controllers. 
        // TODO:: I currently employ First and Third Person View
        //public FirstPersonViewController            FirstPersonView;
        //public ThirdPersonViewController            ThirdPersonView;
        // May-16-2020 1st step toward decoupling ViewControllers from ApplicationUser
        //             Next step, make variables private and rename accordingly
        public /*private*/ ApplicationUserView FirstPersonView;
        public /*private*/ ApplicationUserView ThirdPersonView;
        // TODO:: This should best be part of FirstPersonViewController
        public List<FirstPersonViewController>      FirstPersonTools    = new List<FirstPersonViewController> { };
        // TODO:: See what can be done with this
        public AvatarInfoDesc                       AvatarInfo;
        #endregion

        /// <summary>
        /// Retrieves an array of all models consisting the user avatar setup
        /// </summary>
        public Drawables.Drawable[]                 AvatarModels
        //public Drawables.Models.PlayerModel         AvatarModel 
                                                    {  
                                                        //get { return ThirdPersonView.Avatar; } 
                                                        get { return ThirdPersonView.GetAvatarModels(); }
                                                    }
        #endregion

        #region Ctor
        public ApplicationUser(SceneGame parent, AvatarInfoDesc avatar, Vector3 viewDir, ApplicationUserInput gameInput)
        {
            Parent                  = parent;
            this.gameInput          = gameInput;
            this.AvatarInfo         = avatar;

            if(null == this.AvatarInfo)
                this.AvatarInfo = avatar = AvatarPresets.MaleFit_Generic_0;

            Color                   = new Vector4(0.1f, 0.1f, 0.8f, 1.0f) * 0.3f;           // TODO:: Get from user settings


            //create hud renderer
            Hud                     = new Hud(this);
            parent.Guis.Add(Hud);

            if(null == Scene.SpawnAt_REFACTOR_TODO)
                Position                = GenericMethods.Vector3FromString(avatar.SpawnAt); //  spawnPos;
            else
                Position                = Scene.SpawnAt_REFACTOR_TODO;

            PointingDirection       = viewDir;

            VectorUp                = new Vector3(0, 1, 0);

            Shape boxShape          = new BoxShape(new JVector(0.5f, 2, 0.5f));

            //Shape boxShape = new BoxShape(new JVector(0.5f, 4, 0.5f));

            AvatarBody                    = new RigidBody(boxShape);
            AvatarBody.Position           = new JVector(Position.X, Position.Y, Position.Z);
            AvatarBody.AllowDeactivation  = false;

            //JMatrix mMatrix         = JMatrix.Identity;
            //Body.SetMassProperties(mMatrix, 2,false);

            mConstraint = new NthDimension.Physics.Dynamics.Constraints.SingleBody.FixedAngle(AvatarBody);

            parent.AddConstraint(mConstraint);
            parent.AddRigidBody(AvatarBody);
            AvatarBody.Tag = this;

            ViewInfo                = new ViewInfo(this);
            ViewInfo.aspect         = (float)ApplicationBase.Instance.Width / (float)ApplicationBase.Instance.Height;
            //ViewInfo.zNear = 0.1f;
            //ViewInfo.zFar = 4000f; // 2000 was working ok
            ViewInfo.UpdateProjectionMatrix();

#if !_DEVUI_
            FirstPersonTools.Add(new GameMenu(this,     gameInput));
            FirstPersonTools.Add(new Spawner( this,      gameInput));
            FirstPersonTools.Add(new VoxelGun(this,     gameInput));
            FirstPersonTools.Add(new Grabber( this,      gameInput));
            FirstPersonTools.Add(new Remover( this,      gameInput));

            FirstPersonView         = FirstPersonTools[1];
#endif

            ApplicationBase.Instance.setLoadingProgress(.8f);

            ThirdPersonView         = new ThirdPersonViewController(this, avatar, gameInput);

            ApplicationBase.Instance.setLoadingProgress(1f);

        }
#endregion

        public void ResetScene(SceneGame scene, string spawnPos)
        {
            Parent = scene;

            if (null != mConstraint)
                ((SceneGame)Parent).RemoveConstraint(mConstraint);
            if (null != AvatarBody)
                ((SceneGame)Parent).RemoveRigidBody(AvatarBody);


            Position = GenericMethods.Vector3FromString(spawnPos); //  spawnPos;
            PointingDirection = new Vector3(-1, 0,0);

            VectorUp = new Vector3(0, 1, 0);

            Shape boxShape = new BoxShape(new JVector(0.5f, 2, 0.5f));

            AvatarBody = new RigidBody(boxShape);
            AvatarBody.Position = new JVector(Position.X, Position.Y, Position.Z);
            AvatarBody.AllowDeactivation = false;

            FirstPersonView.Position = Position;
            ThirdPersonView.Position = Position;

            mConstraint = new NthDimension.Physics.Dynamics.Constraints.SingleBody.FixedAngle(AvatarBody);

            ((SceneGame)Parent).AddConstraint(mConstraint);
            ((SceneGame)Parent).AddRigidBody(AvatarBody);
            AvatarBody.Tag = this;

            ViewInfo = new ViewInfo(this);
            //ViewInfo.aspect = (float)ApplicationBase.Instance.Width / (float)ApplicationBase.Instance.Height;
            ViewInfo.UpdateProjectionMatrix();
        }
        public void OnResize()
        {
            ViewInfo = new ViewInfo(this);
            ViewInfo.aspect = (float)ApplicationBase.Instance.Width / (float)ApplicationBase.Instance.Height;
            //ViewInfo.zNear = 0.1f;
            //ViewInfo.zFar = 4000f; // 2000 was working ok
            ViewInfo.UpdateProjectionMatrix();
        }
        public Vector3 GetFrontVec()
        {
            return Vector3.Normalize(new Vector3(PointingDirection.X, 0, PointingDirection.Z));
        }
        public Vector3 GetRightVec()
        {
            return Vector3.Normalize(Vector3.Cross(Vector3.Normalize(PointingDirection), VectorUp));
        }
        public Vector3 GetUpVec()
        {
            return Vector3.Normalize(Vector3.Cross(GetFrontVec(), GetRightVec()));
        }
#region actions
        public Vector3 SetPosition(Vector3 newPosition)
        {
            Vector3 pos     = new Vector3(newPosition);

            if (PlayerViewMode == enuPlayerViewMode.FirstPerson)
                FirstPersonView.EnterView(pos);
            if (PlayerViewMode == enuPlayerViewMode.ThirdPerson)
                ThirdPersonView.EnterView(pos);

            return new Vector3(AvatarBody.Position);
        }

        

#endregion actions

        public override void Update()
        {
#region Draw Frame Rate UI
            //if (Settings.Instance.game.debugMode)
            //    Hud.fpsCounter.setValue((float)Game.Instance.smoothframerate);
#endregion

            if (ApplicationBase.Instance.VAR_AppState == ApplicationState.Playing)
            {
#region Change View

                if (this.gameInput.CHANGECAMERA && !prevC_KeyPress)
                {
                    if(this.PlayerViewMode == enuPlayerViewMode.FirstPerson)
                        this.PlayerViewMode = enuPlayerViewMode.ThirdPerson;
                    else if(this.PlayerViewMode == enuPlayerViewMode.ThirdPerson)
                        this.PlayerViewMode = enuPlayerViewMode.ThirdPersonVR;
                    else if (this.PlayerViewMode == enuPlayerViewMode.ThirdPersonVR)
                        this.PlayerViewMode = enuPlayerViewMode.FirstPerson;

                    if (this.PlayerViewMode == enuPlayerViewMode.FirstPerson)
                    {
                        Vector3 xpos = new Vector3();
                        ThirdPersonView.LeaveView(ref xpos);
                        FirstPersonView.EnterView(xpos);
                    }
                    else if(this.PlayerViewMode == enuPlayerViewMode.ThirdPerson ||
                            this.PlayerViewMode == enuPlayerViewMode.ThirdPersonVR)
                    {
                        Vector3 xpos = new Vector3();
                        FirstPersonView.LeaveView(ref xpos);
                        ThirdPersonView.EnterView(xpos);
                    }
                }
                prevC_KeyPress = this.gameInput.CHANGECAMERA;
                #endregion

                if (PlayerViewMode == enuPlayerViewMode.FirstPerson)
                    updateFirstPersonView();

                if (PlayerViewMode == enuPlayerViewMode.ThirdPerson ||
                    PlayerViewMode == enuPlayerViewMode.ThirdPersonVR)
                    updateThirdPersonView();
            }

            //Game.Instance.Scene.EyePos = Position;
            updateChilds();
        }

        private void updateFirstPersonView()
        {
            #region First Person View 
            Position = GenericMethods.ToOpenTKVector(AvatarBody.Position);
            //Position += FirstPersonView.PositionOffset;
#if _WINDOWS_
            if (gameInput.TOOL01)
            {
                if (FirstPersonTools.Count > 1)
                    FirstPersonView = FirstPersonTools[1];
            }

            if (gameInput.TOOL02)
            {
                if (FirstPersonTools.Count > 2)
                    FirstPersonView = FirstPersonTools[2];
            }

            if (gameInput.TOOL03)
            {
                if (FirstPersonTools.Count > 3)
                    FirstPersonView = FirstPersonTools[3];
            }

            if (gameInput.TOOL04)
            {
                if (FirstPersonTools.Count > 4)
                    FirstPersonView = FirstPersonTools[4];
            }

            if (gameInput.TOOL05)
            {
                if (FirstPersonTools.Count > 5)
                    FirstPersonView = FirstPersonTools[5];
            }

            FirstPersonView.Update();
#endif

            #endregion
        }
        private void updateThirdPersonView()
        {
#region Third Person View
            ThirdPersonView.Update();
#endregion
        }

        
    }
}
