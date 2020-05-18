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

    public class Remover : FirstPersonViewController
    {
        private Model muzzleModel;
        float muzzleFading = 0.9f;

        public Remover(ApplicationUser parent, ApplicationUserInput input)
            : base(parent, input)
        {
            icon.setMaterial("hud\\remover_icon.xmf");
            createMuzzleModel();
        }

        protected override void fireDown()
        {
            RigidBody body; JVector normal; float frac;

            bool result = Scene.CollisionRaycast(GenericMethods.FromOpenTKVector(Position), GenericMethods.FromOpenTKVector(PointingDirection),
                raycastCallback, out body, out normal, out frac);

            muzzleModel.Color = new Vector4(0.8f, 0.3f, 0.8f, 1.0f) * 2;

            if (body != null)
            {
                PhysModel selectedMod = (PhysModel)body.Tag;

                if (selectedMod != null)
                    selectedMod.dissolve();
            }
        }

        private void createMuzzleModel()
        {
            muzzleModel = new Model(Parent);
            muzzleModel.setMaterial("firstperson\\toolgun_muzzle.xmf");
            muzzleModel.setMesh("firstperson\\toolgun_muzzle.obj");

            muzzleModel.Color = Vector4.Zero;
            muzzleModel.IsVisible = true;

            muzzleModel.Renderlayer = Drawable.RenderLayer.Transparent;

            muzzleModel.Scene = Scene;
        }

        public override void Update()
        {
            base.Update();

            if (Parent.FirstPersonView == this)
            {
                muzzleModel.IsVisible = true;
                muzzleModel.Color = muzzleFading * muzzleModel.Color;
                muzzleModel.Orientation = weaponModel.Orientation;
                muzzleModel.Position = weaponModel.Position;
            }
            else
            {
                muzzleModel.IsVisible = false;
            }
        }
    }
}
