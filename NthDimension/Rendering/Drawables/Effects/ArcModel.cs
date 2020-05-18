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

using NthDimension.Algebra;
using NthDimension.Rendering.GameViews;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Shaders;

namespace NthDimension.Rendering.Drawables.Models
{
    //using NthDimension.OpenGL.GLSL.API3x;

#if _WINDOWS_
    //using NthDimension.OpenGL.Windows.GLSL.API3x;
#endif

    public class ElectricArcModel : Model
    {
        protected Matrix4 orientation2;
        protected Matrix4 modelMatrix2;

        public ElectricArcModel(ApplicationObject parent)
            : base(parent)
        {
        }

        public Matrix4 Orientation2
        {
            get { return orientation2; }
            set { orientation2 = value; }
        }

        public Matrix4 ModelMatrix2
        {
            get { return modelMatrix2; }
            set { modelMatrix2 = value; }
        }

        public Vector3 Position2
        {
            get { return position; }
            set
            {
                position = value;
                updateModelMatrix2();
            }
        }


        private void updateModelMatrix2()
        {
            //modelMatrix = Matrix4.Identity;
            Matrix4 scaleMatrix             = Matrix4.CreateScale(Size);
            Matrix4 translationMatrix       = Matrix4.CreateTranslation(position);

            //Matrix4.Mult(ref translationMatrix, ref modelMatrix, out modelMatrix);
            Matrix4.Mult(ref scaleMatrix, ref translationMatrix, out modelMatrix2);
            //Matrix4.Mult(ref orientation, ref modelMatrix, out modelMatrix);
        }

        protected override void SetupMatrices(ref ViewInfo curView, ref Shaders.Shader shader, ref MeshVbo curMesh)
        {
            base.SetupMatrices(ref curView, ref shader, ref curMesh);

            shader.InsertUniform(Uniform.rotation_matrix2, ref orientation2);
            shader.InsertUniform(Uniform.model_matrix2, ref modelMatrix2);
        }
    }
}
