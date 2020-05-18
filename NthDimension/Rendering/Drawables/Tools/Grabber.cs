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

using NthDimension.Rendering.Drawables.Models;
using NthDimension.Rendering.ViewControllers;
namespace NthDimension.Rendering.Drawables.Tools
{
    using NthDimension.Algebra;
    using Rendering.Drawables.Models;
    using NthDimension.Physics.Dynamics;
    using NthDimension.Physics.LinearMath;

#if _WINDOWS_
    //using OpenTK.Input;
#endif


    public class Grabber : FirstPersonViewController
    {
        private float grabDist;
        private PhysModel selectedMod;
        private RigidBody selectedBody;
        private NthDimension.Physics.Dynamics.Constraints.SingleBody.PointOnPoint mConst;
        JVector selModRelPos;

        private Model muzzleModel;

        ElectricArcModel arcModel;
        private Vector3 weaponLocalHitCoords;
        private Vector4 modelLocalHitCoords;

        public Grabber(ApplicationUser player, ApplicationUserInput gameInput) : base(player, gameInput)
        {
            createArcModel();
            createMuzzleModel();

            icon.setMaterial("hud\\grabber_icon.xmf");
        }

        private void createMuzzleModel()
        {
            muzzleModel = new Model(this);
            muzzleModel.setMaterial("firstperson\\grabber_muzzle.xmf");
            muzzleModel.setMesh("firstperson\\toolgun_muzzle.obj");

            muzzleModel.Color = new Vector4(0.6f, 0.7f, 1.0f, 1);
            muzzleModel.IsVisible = false;
            muzzleModel.IgnoreCulling = true;
            muzzleModel.Renderlayer = Drawable.RenderLayer.Transparent;

            muzzleModel.Scene = Scene;
        }

        private void createArcModel()
        {
            arcModel = new ElectricArcModel(this);
            arcModel.setMaterial("firstperson\\toolgun_arc.xmf");
            arcModel.setMesh("firstperson\\toolgun_arc.obj");

            arcModel.Color = new Vector4(0.6f, 0.7f, 1.0f, 1) * 0.2f;
            arcModel.IsVisible = false;
            arcModel.IgnoreCulling = true;
            arcModel.Renderlayer = Drawable.RenderLayer.Transparent;

            arcModel.Scene = Scene;

        }

        protected override void fireDown()
        {
            RigidBody body; JVector normal; float frac;

            bool result = Scene.CollisionRaycast(GenericMethods.FromOpenTKVector(Position), 
                                                 GenericMethods.FromOpenTKVector(PointingDirection),
                                                 raycastCallback, 
                                                 out body, 
                                                 out normal, 
                                                 out frac);

            Vector4 gpos = new Vector4(Position + PointingDirection * frac, 1);

            JVector hitCoords = GenericMethods.FromOpenTKVector(gpos.Xyz);
            weaponLocalHitCoords = GenericMethods.Mult(gpos, Matrix4.Invert(weaponModel.ModelMatrix)).Xyz;
            arcModel.Orientation2 = Matrix4.CreateTranslation(weaponLocalHitCoords - new Vector3(0, 0, -5));

            muzzleModel.IsVisible = true;

            if (result && body.Tag != null)
            {
                if (!(body.Tag is PhysModel)) return;

                PhysModel curMod = (PhysModel)body.Tag;

                if (curMod.grabable)
                {

                    bool debugArc = arcModel.IsVisible;

                    arcModel.IsVisible = true;

                    grabDist = frac;

                    selectedBody = body;
                    selectedMod = (PhysModel)body.Tag;
                    selectedMod.Selected = 1;
                    selectedMod.Forceupdate = true;

                    Matrix4 localMaker = Matrix4.Invert(Matrix4.Mult(selectedMod.Orientation, selectedMod.ModelMatrix));
                    modelLocalHitCoords = GenericMethods.Mult(gpos, localMaker);

                    if (body.IsStatic)
                    {
                        selModRelPos = body.Position - hitCoords;
                    }
                    else
                    {
                        JVector lanchor = hitCoords - body.Position;
                        lanchor = JVector.Transform(lanchor, JMatrix.Transpose(body.Orientation));

                        body.IsActive = true;

                        //body.SetMassProperties(JMatrix.Identity, 0.1f, false);
                        //body.AffectedByGravity = false;

                        mConst = new NthDimension.Physics.Dynamics.Constraints.SingleBody.PointOnPoint(body, lanchor);
                        mConst.Softness = 0.02f;
                        mConst.BiasFactor = 0.1f;
                        Scene.AddConstraint(mConst);
                    }

                    if (!debugArc)
                        Utilities.ConsoleUtil.log(string.Format("Enabled arc model: {0} distance {1}", curMod.Name, grabDist));

                }
            }
        }

        protected override void fireUp()
        {
            arcModel.IsVisible = false;
            muzzleModel.IsVisible = false;
            if (selectedMod != null)
            {
                //mConst.Body1.AffectedByGravity = true;
                if (mConst != null)
                {
                    Scene.RemoveConstraint(mConst);
                }
                selectedBody = null;
                selectedMod.Selected = 0;
                selectedMod.Forceupdate = false;
                selectedMod = null;
                mConst = null;
            }
        }

        protected override void rotateFirstPersonView()
        {
            if (GameInput.ROTATE) //.keyboard[Key.Q])
            {
                if (selectedMod != null)// && mConst == null)
                {
                    JMatrix rotMatA = JMatrix.CreateRotationY(GameInput.MouseDelta.X * CameraRotSpeed);
                    JMatrix rotMatB = GenericMethods.FromOpenTKMatrix(Matrix4.CreateFromAxisAngle(-Parent.VectorRight, GameInput.MouseDelta.Y * CameraRotSpeed));

                    JMatrix rotMatFinal = JMatrix.Multiply(rotMatA, rotMatB);

                    selectedBody.Orientation = JMatrix.Multiply(selectedBody.Orientation, rotMatFinal);
                }
            }
            else
            {
                base.rotateFirstPersonView();
            }
        }

        protected override void interactDown()
        {
            if (selectedMod == null)
            {
                RigidBody body; JVector normal; float frac;

                bool result = Scene.CollisionRaycast(GenericMethods.FromOpenTKVector(Position), GenericMethods.FromOpenTKVector(PointingDirection),
                    raycastCallback, out body, out normal, out frac);

                if (result && body.Tag != null)
                {
                    PhysModel curMod = (PhysModel)body.Tag;

                    if (curMod.grabable)
                    {
                        curMod.IsStatic = !curMod.IsStatic;
                        curMod.SelectedSmooth = 1;
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();

            if (Parent.FirstPersonView == this)
            {
                arcModel.IsVisible = true;
                arcModel.Position = weaponModel.Position;
                //arcModel.Position2 = weaponModel.Position;
                muzzleModel.Position = weaponModel.Position;

                muzzleModel.Orientation = weaponModel.Orientation;
                arcModel.Orientation = weaponModel.Orientation;
                arcModel.Orientation2 = weaponModel.Orientation;

                // moving model

                if (selectedMod != null)
                {
                    JVector anchorCoords = GenericMethods.FromOpenTKVector(Position + PointingDirection * grabDist);
                    if (mConst != null)
                    {
                        mConst.Anchor = anchorCoords;
                    }
                    else if (selectedBody.IsStatic)
                    {
                        selectedBody.Position = anchorCoords + selModRelPos;
                    }

                    selectedMod.updateMatrix();
                    Matrix4 globalMaker = Matrix4.Mult(selectedMod.Orientation, selectedMod.ModelMatrix);
                    Vector4 gpos = GenericMethods.Mult(modelLocalHitCoords, globalMaker);

                    JVector hitCoords = GenericMethods.FromOpenTKVector(gpos.Xyz);
                    //weaponLocalHitCoords = GenericMethods.Mult(gpos, Matrix4.Invert(Parent.viewInfo.modelviewMatrix)).Xyz;

                    //arcModel.Position2 = GenericMethods.Mult(new Vector4(weaponLocalHitCoords - new Vector3(0, 0, 5), 1f), Matrix4.Invert(Parent.viewInfo.modelviewMatrix)).Xyz;
                    arcModel.Position2 = gpos.Xyz - 5 * Parent.ViewInfo.PointingDirection;
                    //arcModel.Orientation2 = Matrix4.CreateTranslation(weaponLocalHitCoords - new Vector3(0, 0, -5));
                }
            }
            else
            {
                arcModel.IsVisible = false;
                muzzleModel.IsVisible = false;
                if (selectedMod != null)
                {
                    //mConst.Body1.AffectedByGravity = true;
                    if (mConst != null)
                    {
                        Scene.RemoveConstraint(mConst);
                    }
                    selectedBody = null;
                    selectedMod.Selected = 0;
                    selectedMod.Forceupdate = false;
                    selectedMod = null;
                    mConst = null;
                }
            }
        }
        /*
        internal override void activate()
        {
            parent.FirstPersonView = (Grabber)this;
        }
         * */
    }
}
