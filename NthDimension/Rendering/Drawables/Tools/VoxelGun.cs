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
using NthDimension.Rendering.Loaders;
using NthDimension.Rendering.ViewControllers;

namespace NthDimension.Rendering.Drawables.Tools
{
    using NthDimension.Algebra;
    using Rendering.Drawables.Models;
    using Rendering.Loaders;
    using NthDimension.Physics.Dynamics;
    using NthDimension.Physics.LinearMath;

    class VoxelGun : FirstPersonViewController
    {
        GhostModel ghost;
        Template[] volumeTemplates = new Template[2];
        private Template template;
        private int tempId = 0;

        public VoxelGun(ApplicationUser player, ApplicationUserInput gameInput)
            : base(player, gameInput)
        {
            icon.setMaterial("hud\\terrain_icon.xmf");

            ensureType();

            generateGhost();
        }

        private void generateGhost()
        {
            if (ghost != null)
                ghost.kill();

            ghost = new GhostModel(this);

            ghost.Materials = template.materials;

            ghost.Meshes = template.meshes;


            ghost.Position = Position;
            ghost.Selected = 1;
            ghost.SelectedSmooth = 0;

            ghost.Scene = Scene;
        }

        protected override void fireDown()
        {
#if VOXELS
            VoxelModel curModel = new VoxelModel(Scene);

            curModel.Materials = template.materials;
            curModel.Meshes = template.meshes;
            curModel.PhysBoxes = template.pmeshes;
            curModel.volume.AffectionRadius = template.volumeRadius;

            curModel.Position = ghost.Position;

            curModel.Name = Scene.GetUniqueName();
#endif
        }

        protected override void interactDown()
        {
            stepTemplateId();

            ensureType();

            generateGhost();
        }

        private void ensureType()
        {
            while ((template = ApplicationBase.Instance.TemplateLoader.getTemplate(tempId)).useType != Template.UseType.Meta)
                stepTemplateId();
        }

        private void stepTemplateId()
        {
            tempId++;
            if (tempId >= ApplicationBase.Instance.TemplateLoader.templates.Count)
                tempId = 0;
        }

        public override void Update()
        {
            base.Update();

            if (Parent.FirstPersonView == this)
            {
                RigidBody body; JVector normal; float frac;

                Vector3 pos = ApplicationBase.Instance.Scene.EyePos;
                Vector3 dir = ApplicationBase.Instance.Player.PointingDirection;
                
                bool result = Scene.CollisionRaycast(GenericMethods.FromOpenTKVector(pos), GenericMethods.FromOpenTKVector(dir),
                                                                         raycastCallback, out body, out normal, out frac);

                Vector3 hitCoords = pos + dir * frac;

                if (result && ghost != null)
                {
                    float smoothness = 0.9f;

                    //Matrix4 newOri = Matrix4.Mult(Matrix4.CreateRotationX((float)Math.PI / 2), Conversion.MatrixFromVector(normal));
                    Matrix4 newOri = GenericMethods.MatrixFromVector(normal);
                    Vector3 newPos = hitCoords;

                    ghost.Position = smoothness * ghost.Position + (1 - smoothness) * newPos;
                }

                ghost.IsVisible = true;
            }
            else
            {
                ghost.IsVisible = false;
            }
        }
    }
}
