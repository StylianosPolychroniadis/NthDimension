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
    using NthDimension.Algebra;
    using Rendering.Geometry;
    using Rendering.Shaders;

#if _WINDOWS_
    //using NthDimension.OpenGL.Windows.GLSL.API3x;
#endif   

    public class DissolvingModel : Model
    {
        private float originalState = 1f;
        private float state = 1f;
        private float factor = 0f;
        public DissolvingModel(ApplicationObject parent, Vector3 size) : base(parent)
        {
            color           = new Vector4(0.8f, 0.3f, 0.8f, 1.0f) * 0.2f;
            Renderlayer     = RenderLayer.Transparent;
            this.Size       = size;
            state           = originalState = this.Size.Y;
            factor          = 1 / state;

            IgnoreCulling   = true;
        }

        public override void Update()
        {
            state *= 0.99f;

            Vector3 oldSize = Size;

            Size = new Vector3(oldSize.X, state, oldSize.Z);

            if (state < (originalState * 0.005f))
            {
                //Scene.PhysicsWorld.RemoveBody(Body);

                MarkForDelete = true;
                kill();
            }

            updateChilds();
        }

#if _WINDOWS_
        protected override void setSpecialUniforms(ref Shaders.Shader shader, ref MeshVbo curMesh)
        {
            float shaderValue = factor * state;
            shader.InsertUniform(Uniform.in_mod, ref shaderValue);   // uniform should map from 0.0-1.0 //shader.InsertUniform(Uniform.in_mod, ref state);
        }
#endif
    }
}
