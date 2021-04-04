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
    public class FirstPersonViewController : ApplicationUserView 
    {
        // FPS Camera
        private readonly Vector4        fpsEyePositionDelta         = new Vector4(0.3f, -0.3f, -0.7f, 1f);          // Note: Eye to Weapon Offset
        private const float             fpsCameraSpeedNormal        = 5f;
        
        private const float             fpsRotationSmoothing        = 0.75f;                                        // TODO:: Move to settings (was .7)
        private const float             fpsCameraHeightOffset       = 2f;

        public float                    CameraRotSpeed              = 0.001f;

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
        private Matrix4                 weaponRotationOffset        = Matrix4.Mult(Matrix4.CreateRotationX(MathFunc.PiOver2), Matrix4.CreateRotationY(MathFunc.Pi));
        
        // KEYS
        private bool                    prevE;
        protected bool                  prevK;

        //public volatile bool            Crouch = false;
        

        public FirstPersonViewController(ApplicationUser parent, ApplicationUserInput gameInput):base(parent, gameInput)
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
            // This should not get called in other view modes -> modify per viewmode

            if (Parent.PlayerViewMode == ApplicationUser.enuPlayerViewMode.ThirdPerson)
                return;

            GameInput.FirstPersonCursorLock(true);

            if (Parent.FirstPersonView == this)
            {
                iconPos                 = new Vector2(-0.8f, 0.8f - slot*iconDist);
                icon.Color              = new Vector4(0.8f, 0.3f, 0.8f, 1.0f);
                            
                PointingDirection       = Parent.PointingDirection;

                weaponModel.IsVisible   = (Parent.PlayerViewMode == ApplicationUser.enuPlayerViewMode.FirstPerson);

                this.updateFirstPersonView();
                Vector3 eyePos = position;
                eyePos.Y += fpsCameraHeightOffset;
                Scene.EyePos            = eyePos;
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
            return (hitbody != Parent.RigidBody);
        }
        protected int getSlot()
        {
            return Parent.FirstPersonTools.Count;
        }


        private bool createWeaponModel()
        {
            if (weaponModelLoaded)
                return true;

            weaponModel             = new Model(Parent);                            // OK
            weaponModel.setMaterial("firstperson\\toolgun.xmf");                    // OK
            weaponModel.setMesh("firstperson\\toolgun.obj");                        // OK
            weaponModel.IsVisible   = false;                                    
            weaponModel.Scene       = Scene;

            weaponModelLoaded       = true;                         

            return true;
        }

        public override Drawable[] GetAvatarModels()
        {
            return new Drawable[] { this.weaponModel };
        }

        public override void EnterView(Vector3 pos)
        {
            GameInput.FirstPersonCursorLock(true);
            Parent.Position = /*Parent.ViewInfo.EyePosition =*/ Position = pos;
            Parent.RigidBody.Position = GenericMethods.FromOpenTKVector(Parent.Position);
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
            weaponModel.Position        = Position;
            weaponModel.Orientation     = Matrix4.Mult(weaponRotationOffset,
                                                       GenericMethods.MatrixFromVector(PointingDirection));
        }

        #region First Person View

        private void updateFirstPersonView()
        {
            if (Parent.PlayerViewMode == ApplicationUser.enuPlayerViewMode.ThirdPerson)
                return;

            PointingDirection = Parent.PointingDirection;

            float smoothness = 0.5f;
            Vector4 tmpToolFromEye = fpsEyePositionDelta;
            Vector3 newPos = GenericMethods.Mult(tmpToolFromEye, Matrix4.Invert(Parent.ViewInfo.modelviewMatrix)).Xyz;
            Matrix4 newOri = Matrix4.Mult(weaponRotationOffset, GenericMethods.MatrixFromVector(PointingDirection));

            weaponModel.Position    = Position  = weaponModel.Position * smoothness + newPos * (1 - smoothness);
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
            Vector3 moveVec = new Vector3();

          
#if _WINDOWS_

            if (GameInput.MOVEFORWARD)
                moveVec.X = fpsCameraSpeedNormal;

            if (GameInput.MOVEBACKWARD)
                moveVec.X = -fpsCameraSpeedNormal;

            if (GameInput.STRAFERIGHT)
                moveVec.Z = fpsCameraSpeedNormal;

            if (GameInput.STRAFELEFT)
                moveVec.Z = -fpsCameraSpeedNormal;

            if (GameInput.JUMP)
                moveVec.Y = fpsCameraSpeedNormal;

            if (GameInput.RUN)
            {
                moveVec.X *= 1.8f;
                moveVec.Z *= 1.8f;
            }

#endif

                moveBody(moveVec);
        }
       
        private void moveBody(Vector3 move)
        {
            RigidBody   hitbody;
            JVector     normal;
            float       frac;

            

            RigidBody body = ((ApplicationUser)Parent).RigidBody;

            bool collisionWithGround = Scene.CollisionRaycast(GenericMethods.FromOpenTKVector(Position),
                                                              new JVector(Position.X, Position.Y - body.BoundingBox.Max.Y / 2 - 2, Position.Z),
                                                                     mRaycastCallback,
                                                                     out hitbody,
                                                                     out normal,
                                                                     out frac);

            body.LinearVelocity = new JVector(move.X * Parent.VectorFwd + Parent.VectorRight * move.Z);

            if (collisionWithGround && frac < 3f && move.Y > 0)
                body.LinearVelocity = new JVector(body.LinearVelocity.X, move.Y * 2, body.LinearVelocity.Z);

#if DEBUG
            if (NthDimension.Settings.Instance.game.diagnostics)
            {
                string colInfo = string.Format("Collision fraction {0} position {1}", frac, Position.ToString());
                ConsoleUtil.log(string.Format("LinearVelocity {0}: {1}, Mass: {2}",
                                                                               body.LinearVelocity.ToString(),
                                                                               collisionWithGround ? colInfo : "No Collision", 
                                                                               body.Mass), false);
            }
#endif

        }
#endregion

#region First Person Rotate 
        protected virtual void rotateFirstPersonView()
        {
            rotateWeapon(GameInput.MouseDelta.Y * CameraRotSpeed, GameInput.MouseDelta.X * CameraRotSpeed, 0);
        }

        private void rotateWeapon(float pitch, float yaw, float roll)
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
        public new ApplicationUser Parent
        {
            get { return (ApplicationUser)base.Parent; }
            set
            {
                base.Parent = value;

                if (value != null)
                {
                    Parent = value;
                }
            }
        }

#endregion
    }
}
