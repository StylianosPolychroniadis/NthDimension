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

using System;
using NthDimension.Algebra;
using NthDimension.Rendering.Drawables;
using NthDimension.Rendering.Drawables.Gui;
using NthDimension.Rendering.Drawables.Models;
using NthDimension.Rendering.Utilities;
using NthDimension.Physics.Collision;
using NthDimension.Physics.Dynamics;
using NthDimension.Physics.LinearMath;

namespace NthDimension.Rendering.ViewControllers
{
    public class FirstPersonViewController : ViewController // GameObject
    {
        // GUI
        protected int                   slot;
        protected GuiElement            icon;
        protected Vector2               iconPos;
        protected Vector2               smoothIconPos;
        protected float                 iconWeight                  = 0.9f;
        protected float                 iconDist                    = 0.25f;
        // Models
        protected Model                 weaponModel;
        protected bool                  weaponModelLoaded           = false;
        private Matrix4                 weaponRotationOffset        = Matrix4.Mult(Matrix4.CreateRotationX((float)Math.PI / 2), Matrix4.CreateRotationY((float)Math.PI));
        // FPS Camera
        public float                    fpsCameraRotSpeed           = 0.001f;
        float                           fpsRotationSmoothing        = 0.01f;  // 0.7f;
        private bool                    prevE;
        protected bool                  prevK;

        private Vector3 PositionOffset = new Vector3(0f, 1.6f, 0f);

        public FirstPersonViewController(Player parent, GameInputBase gameInput):base(parent, gameInput)
        {
            base.raycastCallback = new RaycastCallback(mRaycastCallback);

            slot                    = getSlot();

            icon                    = new GuiElement(this.Parent.Hud);
            icon.setSizeRel(new Vector2(256, 128));

            iconPos                 = new Vector2(-0.8f, 0.8f - slot * iconDist);
            smoothIconPos           = iconPos;
            icon.Position           = smoothIconPos;

            icon.setMaterial("hud\\blank_icon.xmf");

            weaponModelLoaded       = createWeaponModel();
        }

        public override void Update()
        {
            if (Parent.PlayerViewMode == Player.enuPlayerViewMode.ThirdPerson)
                return;

            GameInput.FirstPersonCursorLock(true);

            if (Parent.FirstPersonView == this)
            {
                iconPos             = new Vector2(-0.8f, 0.8f - slot*iconDist);
                icon.Color          = new Vector4(0.8f, 0.3f, 0.8f, 1.0f);
                
                Position            = Scene.EyePos /*= weaponModel.Position*/ = Parent.Position = Parent.ViewInfo.Position = Parent.Position + PositionOffset;//Position = Parent.Position;
                
                PointingDirection   = Parent.PointingDirection;

                weaponModel.IsVisible = true;
                this.updateFirstPersonView();
            }
            else
            {
                iconPos = new Vector2(-0.85f, 0.8f - slot*iconDist);
                icon.Color = new Vector4(0.1f, 0.12f, 0.2f, 1.0f);

                weaponModel.IsVisible = false;
                wasActive = false;
            }

            smoothIconPos = smoothIconPos*iconWeight + iconPos*(1 - iconWeight);
            icon.Position = smoothIconPos;
        }

        protected new bool mRaycastCallback(RigidBody hitbody, JVector normal, float frac)
        {
            return (hitbody != Parent.Body);
        }
        protected int getSlot()
        {
            return Parent.FirstPersonTools.Count;
        }


        private bool createWeaponModel()
        {
            if (weaponModelLoaded)
                return true;

            weaponModel             = new Model(Parent);
            weaponModel.setMaterial("toolgun.xmf");
            weaponModel.setMesh("firstperson\\toolgun.obj");
            weaponModel.IsVisible   = false;
            weaponModel.Scene       = Scene;
            weaponModelLoaded       = true;

            return true;
        }

        //public void fromFirstPerson()
        //{
        //    // Switching from 1st person view to 3rd person view
        //    wasActive1stPerson = false;
        //    weaponModel.isVisible = false;
        //    avatarModel.isVisible = true;
        //    avatarPosition = Position;
        //    //avatarModel.PointingDirection = weaponModel.PointingDirection;
        //    Parent.PlayerMode = Player.PlayerViewMode.ThirdPerson;
        //    startUsing();
        //}

        //public void fromThirdPerson()
        //{
        //    // Switching from 3rd person view to 1st person view
        //    wasActive3rdPerson = false;
        //    weaponModel.isVisible = true;
        //    avatarModel.isVisible = false;
        //    //weaponModel.Position = avatarModel.Position;
        //    weaponModel.PointingDirection = PointingDirection;
        //    Parent.PlayerMode = Player.PlayerViewMode.FirstPerson;
        //    startUsing();
        //}

        public override void EnterView(Vector3 pos)
        {
            //if (Parent.PlayerViewMode == Player.enuPlayerViewMode.ThirdPerson)
            //    return;

            GameInput.FirstPersonCursorLock(true);
            Parent.Position = /*Parent.ViewInfo.EyePosition =*/ Position = pos;
            Parent.Body.Position = GenericMethods.FromOpenTKVector(Parent.Position);
            weaponModel.IsVisible = true;
            weaponModel.Position = Position;
            weaponModel.PointingDirection = PointingDirection;
            Parent.ViewInfo.GenerateModelViewMatrix();
            Parent.ViewInfo.GenerateViewProjectionMatrix();

        }
        public override void LeaveView(ref Vector3 pos)
        {
            pos = Position;
            wasActive = false;
            weaponModel.IsVisible = false;
        }

        protected virtual void startUsing()
        {
            weaponModel.Position = Position;
                //GenericMethods.Mult(new Vector4(0.3f, -0.7f, -0.7f, 1f),
                //    Matrix4.Invert(Parent.ViewInfo.modelviewMatrix)).Xyz;
            weaponModel.Orientation = Matrix4.Mult(weaponRotationOffset,
                GenericMethods.MatrixFromVector(PointingDirection));
        }

        #region First Person View

        private void updateFirstPersonView()
        {
            if (Parent.PlayerViewMode == Player.enuPlayerViewMode.ThirdPerson)
                return;
            //weaponModel.isVisible = true;
            PointingDirection = Parent.PointingDirection;

            float smoothness = 0.5f;
            //Vector3 newPos = GenericMethods.Mult(new Vector4(0.3f, -0.3f, -0.7f, 1f), Matrix4.Invert(Parent.ViewInfo.modelviewMatrix)).Xyz;
            Vector3 newPos = GenericMethods.Mult(new Vector4(0.3f, 0.6f, -0.7f, 1f), Matrix4.Invert(Parent.ViewInfo.modelviewMatrix)).Xyz;
            Matrix4 newOri = Matrix4.Mult(weaponRotationOffset, GenericMethods.MatrixFromVector(PointingDirection));

            weaponModel.Position = Position = (weaponModel.Position /*+ PositionOffset - new Vector3(0f,0.3f,0f)*/) * smoothness + newPos * (1 - smoothness);
            weaponModel.Orientation = GenericMethods.BlendMatrix(weaponModel.Orientation, newOri, smoothness);

            if (!wasActive)
                startUsing();


            if (GameInput != null)
            {
                rotateFirstPersonView();

                moveFirstPersonView();

                #region Fire
                // fire player fire
                bool K = false;

#if _WINDOWS_
                K = GameInput.FIRE; //.mouse[OpenTK.Input.MouseButton.Left];
#endif

                if (K && !prevK && wasActive)
                    fireDown();
                else if (!K && prevK)
                    fireUp();

                prevK = K;
                #endregion Fire

                #region Interact
                // fire player interact
                bool E = false;
#if _WINDOWS_
                E = GameInput.INTERACT;
#endif
                if (E && !prevE)
                    interactDown();
                else if (!E && prevE)
                    interactUp();
                prevE = E;
                #endregion Interact
            }

            wasActive = true;
            updateChilds();
        }

        #region First Person Move
        private void moveFirstPersonView()
        {
            float cameraSpeed = 3f; // 10f slides like on ice

            Vector3 moveVec = new Vector3();

#if _WINDOWS_
            if (GameInput.MOVEFORWARD)
                moveVec.X = cameraSpeed;

            if (GameInput.MOVEBACKWARD)
                moveVec.X = -cameraSpeed;

            if (GameInput.STRAFERIGHT)
                moveVec.Z = cameraSpeed;

            if (GameInput.STRAFELEFT)
                moveVec.Z = -cameraSpeed;

            if (GameInput.JUMP)
                moveVec.Y = cameraSpeed;
#endif

            moveWeapon(moveVec + PositionOffset);
        }
        private void moveWeapon(Vector3 move)
        {
            RigidBody hitbody;
            JVector normal;
            float frac;

            bool result = Scene.PhysicsWorld.CollisionSystem.Raycast(GenericMethods.FromOpenTKVector(Position),
                                                                     new JVector(0, -1f, 0),
                                                                     mRaycastCallback,
                                                                     out hitbody,
                                                                     out normal,
                                                                     out frac);
            if (result && frac < 2.2f)
            {
                Parent.Body.AddForce(GenericMethods.FromOpenTKVector(move.X * Parent.VectorFwd + Parent.VectorRight * move.Z));
                if (move.Y > 0)
                    Parent.Body.LinearVelocity = new JVector(Parent.Body.LinearVelocity.X, 5, Parent.Body.LinearVelocity.Z);

            }
            else
                Parent.Body.AddForce(GenericMethods.FromOpenTKVector(move.X * Parent.VectorFwd + Parent.VectorRight * move.Z));

        }
        #endregion

        #region First Person Rotate 
        protected virtual void rotateFirstPersonView()
        {
            rotateWeapon(GameInput.MouseDelta.Y * fpsCameraRotSpeed, GameInput.MouseDelta.X * fpsCameraRotSpeed, 0);
        }

        public void rotateWeapon(float pitch, float yaw, float roll)
        {
            Vector4 tmpView = new Vector4(0, 0, -1, 1);

            Parent.RotationRaw.X -= pitch;
            Parent.RotationRaw.Y += yaw;

            Parent.Rotation = Parent.RotationRaw * (1 - fpsRotationSmoothing) + Parent.Rotation * fpsRotationSmoothing;

            Matrix4 tmpA = Matrix4.CreateRotationX(Parent.Rotation.X);
            Matrix4 tmpb = Matrix4.CreateRotationY(Parent.Rotation.Y);

            tmpView = GenericMethods.Mult(tmpA, tmpView);
            tmpView = GenericMethods.Mult(tmpb, tmpView);

            PointingDirection = Parent.PointingDirection = tmpView.Xyz;

            Parent.VectorFwd = Parent.GetFrontVec();
            Parent.VectorRight = Parent.GetRightVec();
        }
        #endregion

        #region First Person View Events
        protected virtual void interactUp()
        {
        }

        protected virtual void interactDown()
        {
        }


        protected virtual void fireUp()
        {
        }

        protected virtual void fireDown()
        {
        }
        #endregion

        #endregion First Person View



        #region Properties
        public new Player Parent
        {
            get { return (Player)base.Parent; }
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

        #endregion
    }
}
