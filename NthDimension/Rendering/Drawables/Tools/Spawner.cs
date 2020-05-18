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

using NthDimension.Rendering.Drawables.Lights;
using NthDimension.Rendering.Drawables.Models;
using NthDimension.Rendering.Loaders;
using NthDimension.Rendering.ViewControllers;

namespace NthDimension.Rendering.Drawables.Tools
{
    using NthDimension.Algebra;
    using Rendering.Drawables.Lights;
    using Rendering.Drawables.Models;
    using Rendering.Loaders;
    using NthDimension.Physics.Dynamics;
    using NthDimension.Physics.LinearMath;

    public class Spawner : FirstPersonViewController
    {
        private Model ghost, grid;

        private Template template;

        int tempId = 0;

        public Spawner(ApplicationUser player, ApplicationUserInput gameInput) : base(player, gameInput)
        {
            ensureTempType();
            template = ApplicationBase.Instance.TemplateLoader.getTemplate(tempId);
            generateGhost();

            generateGrid();

            icon.setMaterial(   "hud\\spawner_icon.xmf");
        }

        private void generateGrid()
        {
            grid = new Model(this);
            grid.setMaterial(   "grid.xmf");
            grid.setMesh(       "base\\grid.obj");

            grid.Color = new Vector4(0.8f, 0.3f, 0.8f, 1.0f) * 0.3f;

            grid.Position = Position;
            grid.Scene = Scene;

            grid.Renderlayer = Drawable.RenderLayer.Transparent;
            grid.IgnoreCulling = true;
        }

        private void generateGhost()
        {
            if (ghost != null)
            {
                ghost.kill();
            }

            ghost = new GhostModel(this);

            ghost.Materials = template.materials;
            ghost.Meshes = template.meshes;
            ghost.Position = ghost.Position;

            ghost.Position = Position;
            ghost.Selected = 1;
            ghost.SelectedSmooth = 0;

            ghost.Scene = Scene;
            ghost.IgnoreCulling = true;
        }

        protected override void startUsing()
        {
            base.startUsing();
            ghost.SelectedSmooth = 0;
        }

        public override void Update()
        {
            base.Update();

            if (Parent.FirstPersonView == this)
            {
                ghost.IsVisible = true;
                //if(null != grid)
                grid.IsVisible = true;

                RigidBody body; JVector normal; float frac;

                bool result = Scene.CollisionRaycast(GenericMethods.FromOpenTKVector(Position), GenericMethods.FromOpenTKVector(PointingDirection),
                    raycastCallback, out body, out normal, out frac);

                Vector3 hitCoords = Position + PointingDirection * frac;

                if (result && ghost != null)
                {
                    float smoothness = 0.9f;

                    //Matrix4 newOri = Matrix4.Mult(Matrix4.CreateRotationX((float)Math.PI / 2), Conversion.MatrixFromVector(normal));
                    Matrix4 newOri = GenericMethods.MatrixFromVector(normal);
                    Vector3 newPos = hitCoords + GenericMethods.ToOpenTKVector(normal) * template.positionOffset;

                   // if (null != grid)
                    {
                        grid.Position = smoothness * grid.Position + (1 - smoothness) * hitCoords;
                        grid.Orientation = GenericMethods.BlendMatrix(grid.Orientation, newOri, smoothness);
                    }

                    ghost.Position = smoothness * ghost.Position + (1 - smoothness) * newPos;

                    if (template.normal)
                        ghost.Orientation = newOri;
                }
            }
            else
            {
                ghost.IsVisible = false;
                // if (null != grid)
                grid.IsVisible = false;
            }
        }

        protected override void interactDown()
        {
            stepTemplateId();

            ensureTempType();

            generateGhost();
        }

        private void ensureTempType()
        {
            bool foundNext = false;
            Template curTmp;
            while (!foundNext)
            {
                curTmp = ApplicationBase.Instance.TemplateLoader.getTemplate(tempId);

                if (curTmp.useType == Template.UseType.Animated ||
                    curTmp.useType == Template.UseType.Model)
                {
                    template = curTmp;
                    foundNext = true;
                }
                else
                {
                    stepTemplateId();
                }
            }
            Utilities.ConsoleUtil.log(string.Format("Spawner Template: {0}",  template.name));
        }

        private void stepTemplateId()
        {
            tempId++;
            if (tempId >= ApplicationBase.Instance.TemplateLoader.templates.Count)
                tempId = 0;
        }

        protected override void fireDown()
        {
            objectFromTemplate();
        }

        private void objectFromTemplate()
        {
            PhysModel curModel;
            switch (template.useType)
            {
                case Template.UseType.Animated:
                    curModel = new AnimatedModel(Scene);
                    break;
                default:
                    curModel = new PhysModel(Scene);
                    break;
            }

            curModel.Materials      = template.materials;
            curModel.Meshes         = template.meshes;
            curModel.Position       = ghost.Position;

            curModel.PhysBoxes      = template.pmeshes;

            string modelname = string.Empty;

            try { modelname = Scene.GetUniqueName(); }
            catch { }

            try
            {

                curModel.Name = string.Format(@"{0}\{1}", modelname);

                curModel.Orientation = ghost.Orientation;



                if (template.hasLight && Scene.LightCount < ShaderLoader.maxNoLights)
                {
                    Light mLight = new LightSpot(curModel);
                    mLight.Color = new Vector4(template.lightColor, 1);
                }

                curModel.IsStatic = template.isStatic;
                curModel.IgnoreCulling = true;
                curModel.IgnoreCulling = true;
            }
            catch /*(System.Exception c)*/
            {

            }
        }
    }
}
