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
    public class FirstPersonViewController : ApplicationUserView // GameObject
    {
        // FPS Camera
        private readonly Vector4        fpsEyePositionDelta         = new Vector4(0.3f, -0.3f, -0.7f, 1f); // Difference from EyePos
        private const float             fpsCameraSpeed              = 1.3f;     // 3f; // 10f slides like on ice // TODO:: Lerp instead
        private const float             fpsRotationSmoothing        = 0.01f;    // 0.7f;
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
        private Matrix4                 weaponRotationOffset        = Matrix4.Mult(Matrix4.CreateRotationX((float)Math.PI / 2), Matrix4.CreateRotationY((float)Math.PI));
        
        // KEYS
        private bool                    prevE;
        protected bool                  prevK;

        //public volatile bool            Crouch = false;
        //private Vector3 eyeBias           = new Vector3(0f, 3f, 0f);

        

        public FirstPersonViewController(ApplicationUser parent, ApplicationUserInput gameInput):base(parent, gameInput)
        {
            //PositionOffset       = new Vector3(0f,2.8f,0f);
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

            //ConsoleUtil.log("Raycast Callback");
          
            return (hitbody != Parent.AvatarBody);
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

        public override Drawable[] GetAvatarModels()
        {
            return new Drawable[] { this.weaponModel };
        }

        public override void EnterView(Vector3 pos)
        {
            //if (Parent.PlayerViewMode == Player.enuPlayerViewMode.ThirdPerson)
            //    return;

            GameInput.FirstPersonCursorLock(true);
            Parent.Position = /*Parent.ViewInfo.EyePosition =*/ Position = pos;
            Parent.AvatarBody.Position = GenericMethods.FromOpenTKVector(Parent.Position);
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
            if (Parent.PlayerViewMode == ApplicationUser.enuPlayerViewMode.ThirdPerson)
                return;
            //weaponModel.isVisible = true;
            PointingDirection = Parent.PointingDirection;

            float smoothness = 0.5f;
            Vector4 tmpToolFromEye = fpsEyePositionDelta;
            //tmpToolFromEye.Y += fpsCameraHeightOffset;
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
                moveVec.X = fpsCameraSpeed;

            if (GameInput.MOVEBACKWARD)
                moveVec.X = -fpsCameraSpeed;

            if (GameInput.STRAFERIGHT)
                moveVec.Z = fpsCameraSpeed;

            if (GameInput.STRAFELEFT)
                moveVec.Z = -fpsCameraSpeed;

            if (GameInput.JUMP)
                moveVec.Y = fpsCameraSpeed;
#endif

            moveWeapon(moveVec);
        }
        private void moveWeapon(Vector3 move)
        {
            RigidBody   hitbody;
            JVector     normal;
            float       frac;

            bool collided = Scene.CollisionRaycast(GenericMethods.FromOpenTKVector(Position),
                                                   //GenericMethods.FromOpenTKVector(new Vector3(0, -1f, 0)),                                // Y = -1f jumps ok (low gravity)
                                                   GenericMethods.FromOpenTKVector(PointingDirection) * 5,
                                                   mRaycastCallback,
                                                                     out hitbody,
                                                                     out normal,
                                                                     out frac);

            JVector linearVelocity  = new JVector();
            JVector force           = new JVector();

            RigidBody body = ((ApplicationUser)Parent).AvatarBody;

            if (collided && frac < 2.2f)
            {
                body.AddForce(GenericMethods.FromOpenTKVector(move.X * Parent.VectorFwd + Parent.VectorRight * move.Z));

                if (move.Y > 0)
                {
                    linearVelocity = new JVector(body.LinearVelocity.X, 5, body.LinearVelocity.Z);
                    body.LinearVelocity = linearVelocity;                    
                }
            }
            else
            {
                force = GenericMethods.FromOpenTKVector(move.X * Parent.VectorFwd + Parent.VectorRight * move.Z);               
                body.AddForce(force);
            }

            string colInfo = string.Format("Collision fraction {0} position {1}", frac, Position.ToString());
            ConsoleUtil.log(string.Format("Force {0}, LinearVelocity {1}: {2},  ", force.ToString(), 
                                                                           linearVelocity.ToString(), 
                                                                           collided? colInfo : "No Collision"), 
                                                                            true);

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

                //Parent.forceUpdate();
            }
        }

        #endregion
    }
}
