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



using NthDimension.Rendering.GameViews;
using NthDimension.Rendering.Utilities;

namespace NthDimension.Rendering.Drawables.Models
{
    using System;

    using NthDimension.Rasterizer;
    //using NthDimension.OpenGL.GLSL.API3x;
    using Rendering.Geometry;
    using Rendering.Shaders;

#if _WINDOWS_
    //using NthDimension.OpenGL.Windows.GLSL.API3x;
#endif

    public class Model : Drawable
    {
        public Model()
        {
            Renderlayer = RenderLayer.Solid;
        }

        public Model(ApplicationObject parent)
        {
            Parent = parent;
            Scene = parent.Scene;
        }

        public virtual void makeUnstatic()
        {
        }

        protected void updateSelection()
        {
            SelectedSmooth = Selected * 0.05f + SelectedSmooth * 0.95f;
        }


        #region drawing
        //private static int matIdxPrev = -1;
        private static string currentMaterialName           = string.Empty;

        public override void draw(ViewInfo curView, bool renderlayer)
        {
            try
            {
                if (vaoHandle != null  && IsVisible)
                {
                    int matBaseIdxPrev = -1;
                    int matIdx = 0;
                    Shaders.Shader shader = new Shader();

                    for (int i = 0; i < vaoHandle.Length; i++)
                    {
                        if (materials.Length == 0) continue;

                        if (vaoHandle.Length == materials.Length)
                            matIdx = i;

                        if (matIdx < 0) continue;

                        System.Diagnostics.Stopwatch drawTime = new System.Diagnostics.Stopwatch();
                        drawTime.Start();

                        MeshVbo curMesh = meshes[i];

                        if (!ApplicationBase.Instance.Scene.VisibleDrawables.ContainsKey(this)) continue;
                        if (!ApplicationBase.Instance.Scene.VisibleDrawables[this].Contains(curMesh)) continue;

                        Material curMaterial = materials[matIdx];
//#if DEBUG
                        if (!ApplicationBase.Instance.DrawnMeshes.Contains(curMesh))       // USED FOR DEBUG/DIAGNOSTICS
                            ApplicationBase.Instance.DrawnMeshes.Add(curMesh);
//#endif

                        ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Draw Model {0}:{1}:{2} Shader: {3}", this.GetType(), Name, curMesh.Name, shader.Name));


                        if (renderlayer == curMaterial.propertys.useAlpha)
                        {
                            if (matBaseIdxPrev != matIdx)
                            {
                                shader = activateMaterial(ref curMaterial);
                                currentMaterialName = curMaterial.name;
                            }

                            if (shader.Loaded)
                            {

                                Factories.ShaderLoader.UpdateShaderDebug(shader);

                                //if (ApplicationBase.Instance.VAR_ScreenSize_Virtual.X < ApplicationBase.Instance.VAR_ScreenSize_Current.X ||
                                //    ApplicationBase.Instance.VAR_ScreenSize_Virtual.Y < ApplicationBase.Instance.VAR_ScreenSize_Current.Y)
                                //    ApplicationBase.Instance.VAR_ScreenSize_Virtual = ApplicationBase.Instance.VAR_ScreenSize_Current;

                                shader.InsertUniform(Uniform.in_screensize, ref ApplicationBase.Instance.VAR_ScreenSize_Virtual);
                                shader.InsertUniform(Uniform.in_rendersize, ref ApplicationBase.Instance.VAR_ScreenSize_Current);
                                shader.InsertUniform(Uniform.in_time, ref ApplicationBase.Instance.VAR_FrameTime);
                                shader.InsertUniform(Uniform.in_color, ref color);
                                shader.InsertUniform(Uniform.in_alpha, ref this.Alpha);

                                if (Scene != null)
                                    SetupMatrices(ref curView, ref shader, ref curMesh);

                                if (null == curMesh.MeshData.Indices)
                                    ConsoleUtil.errorlog("draw() ", string.Format("{0} indices Vbo data is Null", curMesh.Name));
                                else
                                {
                                    setSpecialUniforms(ref shader, ref curMesh);

                                    ApplicationBase.Instance.Renderer.BindVertexArray(vaoHandle[i]);
                                    ApplicationBase.Instance.Renderer.DrawElements(PrimitiveType, curMesh.MeshData.Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero); //Game.Instance.Renderer.DrawArrays(PrimitiveType.Triangles, 0, curMesh.indicesVboData.Length);

                                    //if (Scene.AllOffsets.Count == 0)
                                    //    Scene.AllOffsets.Add(0);
                                    //else
                                    //    Scene.AllOffsets.Add(Scene.AllOffsets[Scene.AllOffsets.Count - 1] + curMesh.MeshData.Indices.Length);

                                    //if (!Scene.AllIndicesLengths.ContainsKey(curMesh.Name))
                                    //    Scene.AllIndicesLengths.Add(curMesh.Name, curMesh.MeshData.Indices.Length);
                                    //else
                                    //    Scene.AllIndicesLengths[curMesh.Name] = curMesh.MeshData.Indices.Length;

                                    //if (!Scene.AllEBOs.ContainsKey(curMesh.Name))
                                    //    Scene.AllEBOs.Add(curMesh.Name, curMesh.MeshData.EboHandle);
                                    //else
                                    //    Scene.AllEBOs[curMesh.Name] = curMesh.MeshData.EboHandle;
                                }

                                //matIdxPrev = matIdx;
                                curMesh.DrawTimeAllPasses += drawTime.ElapsedTicks;
                                curMesh.DrawCalls++;
                                ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Draw Model {0}:{1}:{2} Shader: {3}", this.GetType(), Name, curMesh.Name, shader.Name));
                            }
                        }

                        matBaseIdxPrev = matIdx;
                        ApplicationBase.Instance.Scene.DrawCallTotal++;
                    }
                }
            }
            catch (Exception dE)
            {
                if(NthDimension.Settings.Instance.game.diagnostics)
                    ConsoleUtil.errorlog("Model.draw() ", $"{dE.Message} {dE.StackTrace}");
            }
        }

        public override void drawSelection(ViewInfo curView)
        {
            try
            {
                if (vaoHandle != null && IsVisible)
                {
                    int matSelectionIdxPrev = -1;
                    int matIdx = 0;
                    Shaders.Shader curShader = new Shader();

                    for (int i = 0; i < vaoHandle.Length; i++)
                    {
                        if (materials.Length == 0) continue;

                        if (vaoHandle.Length == materials.Length)
                            matIdx = i;

                        if (matIdx < 0) continue;

                        System.Diagnostics.Stopwatch drawTime = new System.Diagnostics.Stopwatch();
                        drawTime.Start();

                        MeshVbo curMesh = meshes[i];

                        if (!ApplicationBase.Instance.Scene.VisibleDrawables.ContainsKey(this)) continue;
                        if (!ApplicationBase.Instance.Scene.VisibleDrawables[this].Contains(curMesh)) continue;

                        Material curMaterial = materials[matIdx];

                        if (matSelectionIdxPrev != matIdx)
                        //if (currentMaterialName != curMaterial.name)
                        {
                            curShader = activateMaterialSelection(curMaterial);
                            currentMaterialName = curMaterial.name;
                        }

                        if (curShader.Loaded)
                        {
                            Factories.ShaderLoader.UpdateShaderDebug(curShader);

                            if (Scene != null)
                                SetupMatrices(ref curView, ref curShader, ref curMesh);

                            if (null == curMesh.MeshData.Indices)
                                ConsoleUtil.errorlog("drawSelection() ", string.Format("{0} indices Vbo data is Null", curMesh.Name));
                            else
                            {
                                ApplicationBase.Instance.Renderer.Uniform1(ApplicationBase.Instance.Renderer.GetUniformLocation(curShader.Handle, "selected"), 1, ref SelectedSmooth);
                                ApplicationBase.Instance.Renderer.BindVertexArray(vaoHandle[i]);
                                //if (this is PlayerModel || (null != this.Parent && this.Parent is PlayerModel))
                                ApplicationBase.Instance.Renderer.DrawElements(PrimitiveType, curMesh.MeshData.Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
                                //else
                                //    GameBase.Instance.Renderer.DrawArrays(PrimitiveType.Triangles, 0, curMesh.MeshData.Indices.Length);

                                //GameBase.Instance.Renderer.BindVertexArray(0);

                                //if (Scene.AllOffsets.Count == 0)
                                //    Scene.AllOffsets.Add(0);
                                //else
                                //    Scene.AllOffsets.Add(Scene.AllOffsets[Scene.AllOffsets.Count - 1] + curMesh.MeshData.Indices.Length);

                                //if (!Scene.AllIndicesLengths.ContainsKey(this.Name))
                                //    Scene.AllIndicesLengths.Add(this.Name, curMesh.MeshData.Indices.Length);
                                //else
                                //    Scene.AllIndicesLengths[this.Name] = curMesh.MeshData.Indices.Length;

                                //if (!Scene.AllEBOs.ContainsKey(this.Name))
                                //    Scene.AllEBOs.Add(this.Name, curMesh.MeshData.EboHandle);
                                //else
                                //    Scene.AllEBOs[this.Name] = curMesh.MeshData.EboHandle;
                            }

                            matSelectionIdxPrev = matIdx;
                            ApplicationBase.Instance.Scene.DrawCallTotal++;
                        }

                        curMesh.DrawTimeAllPasses += drawTime.ElapsedTicks;
                    }
                }
            }
            catch (Exception dE)
            {
                ConsoleUtil.errorlog("Model.drawSelection() ", $"{dE.Message} {dE.StackTrace}");
            }
        }

        public override void drawNormal(ViewInfo curView)
        {
            try
            {
                if (vaoHandle != null && IsVisible)
                {
                    int matNormalIdxPrev = -1;
                    int matIdx = 0;
                    Shaders.Shader curShader = new Shader();

                    for (int i = 0; i < vaoHandle.Length; i++)
                    {
                        if (materials.Length == 0) continue;

                        if (vaoHandle.Length == materials.Length)
                            matIdx = i;

                        if (matIdx < 0) continue;

                        System.Diagnostics.Stopwatch drawTime = new System.Diagnostics.Stopwatch();
                        drawTime.Start();

                        MeshVbo curMesh = meshes[i];

                        if (!ApplicationBase.Instance.Scene.VisibleDrawables.ContainsKey(this)) continue;
                        if (!ApplicationBase.Instance.Scene.VisibleDrawables[this].Contains(curMesh)) continue;

                        Material curMaterial = materials[matIdx];

                        if (matNormalIdxPrev != matIdx)
                        //if (currentMaterialName != curMaterial.name)
                        {
                            curShader = activateMaterialNormal(curMaterial/*, this.Name*/);   // DEBUG LINE USE THE ONE ABOVE
                            currentMaterialName = curMaterial.name;
                        }

                        if (curShader.Loaded)
                        {
                            Factories.ShaderLoader.UpdateShaderDebug(curShader);

                            if (Scene != null)
                                SetupMatrices(ref curView, ref curShader, ref curMesh);

                            if (null == curMesh.MeshData.Indices)
                                ConsoleUtil.errorlog("drawNormal() ", string.Format("{0} indices Vbo data is Null", curMesh.Name));
                            else
                            {
                                ApplicationBase.Instance.Renderer.BindVertexArray(vaoHandle[i]);

                                //if (this is PlayerModel || (null != this.Parent && this.Parent is PlayerModel))
                                ApplicationBase.Instance.Renderer.DrawElements(PrimitiveType, curMesh.MeshData.Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
                                //else
                                //    GameBase.Instance.Renderer.DrawArrays(PrimitiveType.Triangles, 0, curMesh.MeshData.Indices.Length);


                                //GameBase.Instance.Renderer.BindVertexArray(0);
                                //if (Scene.AllOffsets.Count == 0)
                                //    Scene.AllOffsets.Add(0);
                                //else
                                //    Scene.AllOffsets.Add(Scene.AllOffsets[Scene.AllOffsets.Count - 1] + curMesh.MeshData.Indices.Length);

                                //if (!Scene.AllIndicesLengths.ContainsKey(this.Name))
                                //    Scene.AllIndicesLengths.Add(this.Name, curMesh.MeshData.Indices.Length);
                                //else
                                //    Scene.AllIndicesLengths[this.Name] = curMesh.MeshData.Indices.Length;

                                //if (!Scene.AllEBOs.ContainsKey(this.Name))
                                //    Scene.AllEBOs.Add(this.Name, curMesh.MeshData.EboHandle);
                                //else
                                //    Scene.AllEBOs[this.Name] = curMesh.MeshData.EboHandle;
                            }

                            matNormalIdxPrev = matIdx;
                            ApplicationBase.Instance.Scene.DrawCallTotal++;
                        }

                        curMesh.DrawTimeAllPasses += drawTime.ElapsedTicks;
                    }
                }
            }
            catch (Exception dE)
            {
                ConsoleUtil.errorlog("Model.drawNormal() ", $"{dE.Message} {dE.StackTrace}");
            }
        }

        public override void drawShadow(ViewInfo curView)
        {
            try
            {
                if (vaoHandle != null && IsVisible)
                {
                    int matShadowIdxPrev = -1;
                    int matIdx = 0;
                    Shaders.Shader shader = new Shader();

                    for (int i = 0; i < vaoHandle.Length; i++)
                    {
                        if (materials.Length == 0) continue;

                        if (vaoHandle.Length == materials.Length) matIdx = i;

                        if (matIdx < 0) continue;

                        System.Diagnostics.Stopwatch drawTime = new System.Diagnostics.Stopwatch();
                        drawTime.Start();

                        MeshVbo curMesh = meshes[i];

                        if (!ApplicationBase.Instance.Scene.VisibleDrawables.ContainsKey(this)) continue;
                        if (!ApplicationBase.Instance.Scene.VisibleDrawables[this].Contains(curMesh)) continue;

                        Material curMaterial = materials[matIdx];

                        if (matShadowIdxPrev != matIdx)
                        {
                            shader = activateMaterialShadow(curMaterial/*, this.Name*/);   // DEBUG LINE USE THE ONE ABOVE
                            currentMaterialName = curMaterial.name;
                        }

                        if (shader.Loaded)
                        {
                            Factories.ShaderLoader.UpdateShaderDebug(shader);

                            if (Scene != null)
                            {
                                SetupMatrices(ref curView, ref shader, ref curMesh);
                            }

                            shader.InsertUniform(Uniform.in_rendersize, ref ApplicationBase.Instance.VAR_ScreenSize_Current);

                            if (null == curMesh.MeshData.Indices)
                                ConsoleUtil.errorlog("drawShadow() ", string.Format("{0} indices Vbo data is Null", curMesh.Name));
                            else
                            {
                                ApplicationBase.Instance.Renderer.BindVertexArray(vaoHandle[i]);

                                //if (this is PlayerModel || (null != this.Parent && this.Parent is PlayerModel))
                                ApplicationBase.Instance.Renderer.DrawElements(PrimitiveType, curMesh.MeshData.Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
                                //else
                                //    GameBase.Instance.Renderer.DrawArrays(PrimitiveType.Triangles, 0, curMesh.MeshData.Indices.Length);

                                //GameBase.Instance.Renderer.BindVertexArray(0);

                                //if (Scene.AllOffsets.Count == 0)
                                //    Scene.AllOffsets.Add(0);
                                //else
                                //    Scene.AllOffsets.Add(Scene.AllOffsets[Scene.AllOffsets.Count - 1] + curMesh.MeshData.Indices.Length);

                                //if (!Scene.AllIndicesLengths.ContainsKey(this.Name))
                                //    Scene.AllIndicesLengths.Add(this.Name, curMesh.MeshData.Indices.Length);
                                //else
                                //    Scene.AllIndicesLengths[this.Name] = curMesh.MeshData.Indices.Length;

                                //if (!Scene.AllEBOs.ContainsKey(this.Name))
                                //    Scene.AllEBOs.Add(this.Name, curMesh.MeshData.EboHandle);
                                //else
                                //    Scene.AllEBOs[this.Name] = curMesh.MeshData.EboHandle;
                            }

                            matShadowIdxPrev = matIdx;
                            ApplicationBase.Instance.Scene.DrawCallTotal++;
                        }

                        curMesh.DrawTimeAllPasses += drawTime.ElapsedTicks;
                    }
                }
            }
            catch (Exception dE)
            {
                ConsoleUtil.errorlog("Model.drawShadow() ", $"{dE.Message} {dE.StackTrace}");
            }
        }

        #endregion drawing/shader

        #region update

        public override void Update()
        {
            updateChilds();

            // Bad Idea below for debug but cool for feature
            //if (meshes[0].CurrentLod == MeshVbo.MeshLod.Level3)
            //    this.Color = new Algebra.Vector4(0.8f, 0f, 0f, 1f) * 0.3f;

            //if (meshes[0].CurrentLod == MeshVbo.MeshLod.Level2)
            //    this.Color = new Algebra.Vector4(0f, 0f, 0.8f, 1f) * 0.3f;

            //if (meshes[0].CurrentLod == MeshVbo.MeshLod.Level1)
            //    this.Color = new Algebra.Vector4(0f, 0.8f, 0f, 1f) * 0.3f;

            //if (meshes[0].CurrentLod == MeshVbo.MeshLod.Level0)           // *! 
            //    this.Color = new Algebra.Vector4(0f, 0f, 0f, 0f);
        }

        #endregion update

        protected virtual void setSpecialUniforms(ref Shaders.Shader curShader, ref MeshVbo CurMesh)
        {
        }

        public override void kill()
        {
            this.MarkForDelete = true;

            if (this is PlayerModel)
                for (int i = 0; i < ((PlayerModel) this).AttachmentModels.Length; i++)
                    ((PlayerModel) this).AttachmentModels[i].MarkForDelete = true;


            for (int i = 0; i < vaoHandle.Length; i++)
            {
                //todo delete vaos
            };

            Parent = null;
            //parent.childNames.Remove(name);

            killChilds();
        }
    }
}
