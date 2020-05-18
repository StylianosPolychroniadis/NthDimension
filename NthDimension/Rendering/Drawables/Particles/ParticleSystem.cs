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
using NthDimension.Rendering.GameViews;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Shaders;

namespace NthDimension.Rendering.Drawables.Particles
{
    using System;

    using NthDimension.Rasterizer;
    //using NthDimension.OpenGL.GLSL.API3x;

    using NthDimension.Algebra;
    using Rendering.Drawables.Models;
    using Rendering.Geometry;
    using Rendering.Shaders;
    using System.Collections.Generic;

#if _WINDOWS_
    //using NthDimension.OpenGL.Windows.GLSL.API3x;
#endif


    public class ParticleSystem : Model
    {
        new public static string nodename = "particles";

        Random rnd = new Random();

        private int spreadRadius;
        private int maxParticles = 1700;

        protected Particle[] particles;

        private int particlesPerFrame = 130;
        private float particleLifeTime = 7;
        private float particleSize = 1f;

        public List<ParticleAffector> ParticleControllers = new List<ParticleAffector>();

        public ParticleSystem(ApplicationObject parent)
            : base(parent)
        {
            spreadRadius = 30;
            particles = new Particle[maxParticles];
        }

        public void generateParticles(int amnt)
        {
            int curPos = 0;

            for (int i = 0; i < amnt; i++)
            {
                bool placed = false;

                while (!placed)
                {
                    if (!particles[curPos].alive)
                    {
                        Vector3 randomPos = new Vector3((float)(0.5 - rnd.NextDouble()), (float)(rnd.NextDouble() * 0.5f), (float)(0.5 - rnd.NextDouble())) * spreadRadius * 2;
                        Particle curPat = new Particle(randomPos);

                        curPat.rendertype = rnd.Next(meshes.Length);
                        curPat.size = particleSize;
                        curPat.spawnTime = ApplicationBase.Instance.VAR_FrameTime;
                        curPat.lifeTime = particleLifeTime;
                        curPat.alive = true;

                        particles[curPos] = curPat;

                        placed = true;
                    }

                    curPos++;

                    if (curPos == particles.Length)
                    {
                        return;
                    }
                }
            }
        }

        public override void Update()
        {
            float frametime = ApplicationBase.Instance.VAR_FrameTime;
            float lastFrameDur = ApplicationBase.Instance.VAR_FrameTime_Last;

            generateParticles(particlesPerFrame);

            
            foreach (var affector in this.ParticleControllers)
            {
                affector.affect(ref particles);
            }

            for (int i = 0; i < maxParticles; i++)
            {
                particles[i].position += particles[i].vector;
            }

            updateChilds();

        }

        public override void draw(ViewInfo curView, bool targetLayer)
        {
            if (vaoHandle != null && IsVisible)
            {
                //Vector4 screenpos;
                for (int i = 0; i < vaoHandle.Length; i++)
                {
                    ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Draw Model {0}:{1} Shade: {2}", this.GetType(), Name,  meshes[i].Name));

                    int matIdx = 0;

                    if (vaoHandle.Length == materials.Length)
                        matIdx = i;

                    if (materials[matIdx].propertys.useAlpha == targetLayer)
                    {
                        //ConsoleUtil.log("drawing: " + mMesh[i].name);

                        Material curMat = materials[matIdx];
                        Shaders.Shader shader = activateMaterial(ref curMat);
                        MeshVbo curMesh = meshes[i];

                        if (shader.Loaded)
                        {
                            //if (ApplicationBase.Instance.VAR_ScreenSize_Virtual.X < ApplicationBase.Instance.VAR_ScreenSize_Current.X ||
                            //    ApplicationBase.Instance.VAR_ScreenSize_Virtual.Y < ApplicationBase.Instance.VAR_ScreenSize_Current.Y)
                            //    ApplicationBase.Instance.VAR_ScreenSize_Virtual = ApplicationBase.Instance.VAR_ScreenSize_Current;

                            shader.InsertUniform(Uniform.in_screensize, ref ApplicationBase.Instance.VAR_ScreenSize_Virtual);
                            shader.InsertUniform(Uniform.in_rendersize, ref ApplicationBase.Instance.VAR_ScreenSize_Current);

                            shader.InsertUniform(Uniform.in_time, ref ApplicationBase.Instance.VAR_FrameTime);
                            shader.InsertUniform(Uniform.in_color, ref color);

                            //Game.Instance.Renderer.Uniform1(curShader.nearLocation, 1, ref mGame.mPlayer.zNear);
                            //Game.Instance.Renderer.Uniform1(curShader.farLocation, 1, ref mGame.mPlayer.zFar);

                            if (Scene != null)
                            {
                                SetupMatrices(ref curView, ref shader, ref curMesh);
                            }

                            setSpecialUniforms(ref shader, ref curMesh);

                            ApplicationBase.Instance.Renderer.BindVertexArray(vaoHandle[i]);

                            for (int j = 0; j < particles.Length; j++)
                            {
                                Particle curPat = particles[j];
                                if (curPat.alive && curPat.rendertype == i)
                                {
                                    shader.InsertUniform(Uniform.in_particlepos, ref curPat.position);
                                    shader.InsertUniform(Uniform.in_particlesize, ref curPat.size);
                                    ApplicationBase.Instance.Renderer.DrawElements(PrimitiveType, curMesh.MeshData.Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
                                }
                            }
                            //Game.Instance.Renderer.BindVertexArray(0);

                            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Draw Model {0}:{1} Shade: {2}", this.GetType(), Name, meshes[i].Name));
                        }
                    }
                }
            }
        }

        public override void save(ref System.Text.StringBuilder sb, int level)
        {
            //// converting object information to strings
            //string myposition = GenericMethods.StringFromVector3(Position);
            //string direction = GenericMethods.StringFromVector3(PointingDirection);
            //string stringColor = GenericMethods.StringFromVector3(colorRgb);
            //string mparent = Parent.Name;


            //string tab = GenericMethods.tabify(level - 1);
            //string tab2 = GenericMethods.tabify(level);

            //sb.AppendLine(tab + "<lamp name='" + Name + "'>");
            //sb.AppendLine(tab2 + "<position>" + myposition + "</position>");
            //sb.AppendLine(tab2 + "<direction>" + direction + "</direction>");
            //sb.AppendLine(tab2 + "<color>" + stringColor + "</color>");
            ////sb.AppendLine(tab2 + "<parent>" + mparent + "'/>");

            //if (Texture != null)
            //    sb.AppendLine(tab2 + "<texture>" + Texture + "</texture>");

            //// output saving message
            //Utilities.ConsoleUtil.log(string.Format("@ Saving Light: '{0}'", Name));

            //sb.AppendLine(tab + "</lamp>");

            //// save childs
            //saveChilds(ref sb, level);
        }

        protected override void specialLoad(ref System.Xml.XmlTextReader reader, string type)
        {
            ////if (reader.Name == "texture" && reader.NodeType != System.Xml.XmlNodeType.EndElement)
            ////{
            ////    reader.Read();
            ////    Texture = reader.Value;
            ////}

            //throw new NotImplementedException();
        }
    }
}
