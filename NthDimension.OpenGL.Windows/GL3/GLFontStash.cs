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

using System;


using OpenTK.Graphics.OpenGL;
using NthDimension.Rasterizer.NanoVG.FontStashNet;

namespace NthDimension.Rasterizer.Windows
{
    public static class GlFontStash
    {
        public static FONScontext glfonsCreate(int width, int height, FONSflags flags)
        {
            FONSparams fparams;
            GLFONScontext gl = new GLFONScontext();

            fparams.width = width;
            fparams.height = height;
            fparams.flags = flags;
            fparams.renderCreate = glfons__renderCreate;
            fparams.renderResize = glfons__renderResize;
            fparams.renderUpdate = glfons__renderUpdate;
            fparams.renderDraw = glfons__renderDraw;
            fparams.renderDelete = glfons__renderDelete;
            fparams.userPtr = gl;

            return FontStash.fonsCreateInternal(ref fparams);
        }

        public static void glfonsDelete(FONScontext ctx)
        {
            FontStash.fonsDeleteInternal(ctx);
        }

        public static int glfons__renderCreate(object userPtr, int width, int height)
        {
            GLFONScontext gl = userPtr as GLFONScontext;

            // Create may be called multiple times, delete existing texture.
            if (gl.tex != 0)
            {
                GL.DeleteTextures(1, ref gl.tex);
                gl.tex = 0;
            }
            GL.GenTextures(1, out gl.tex);
            if (!(gl.tex != 0))
                return 0;
            gl.width = width;
            gl.height = height;
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, gl.tex);
            GL.TexImage2D(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelInternalFormat.Alpha,
                gl.width, gl.height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Alpha, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, OpenTK.Graphics.OpenGL.TextureParameterName.TextureMinFilter,
                (int)OpenTK.Graphics.OpenGL.TextureMinFilter.Linear);
            return 1;
        }

        public static int glfons__renderResize(object uptr, int width, int height)
        {
            // Reuse create to resize too.
            return glfons__renderCreate(uptr, width, height);
        }

        public static void glfons__renderUpdate(object uptr, ref int[] rect, byte[] data)
        {
            GLFONScontext gl = (GLFONScontext)uptr;
            int w = rect[2] - rect[0];
            int h = rect[3] - rect[1];

            if (gl.tex == 0)
                return;
            GL.PushClientAttrib(ClientAttribMask.ClientPixelStoreBit);
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, gl.tex);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.PixelStore(PixelStoreParameter.UnpackRowLength, gl.width);
            GL.PixelStore(PixelStoreParameter.UnpackSkipPixels, rect[0]);
            GL.PixelStore(PixelStoreParameter.UnpackSkipRows, rect[1]);
            GL.TexSubImage2D(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, 0, rect[0], rect[1], w, h,
                OpenTK.Graphics.OpenGL.PixelFormat.Alpha, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, data);
            GL.PopClientAttrib();
        }

        public static void glfons__renderDraw(object userPtr, float[] verts, float[] tcoords,
                                              uint[] colors, int nverts)
        {
            GLFONScontext gl = (GLFONScontext)userPtr;
            if (gl.tex == 0)
                return;
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, gl.tex);
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            //GL.VertexPointer(2, VertexPointerType.Float, sizeof(float)*2, verts);
            //GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeof(float)*2, tcoords);
            //GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(uint), colors);

            GL.VertexPointer(2, VertexPointerType.Float, 0, verts);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, tcoords);
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, 0, colors);

            GL.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles, 0, nverts);

            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.ColorArray);
        }

        public static void glfons__renderDelete(object userPtr)
        {
            GLFONScontext gl = (GLFONScontext)userPtr;
            if (gl.tex != 0)
                GL.DeleteTextures(1, ref gl.tex);
            gl.tex = 0;
            //free(gl);
        }

        public static uint glfonsRGBA(byte r, byte g, byte b, byte a)
        {
            return (uint)((r) | (g << 8) | (b << 16) | (a << 24));
        }
    }

    public class GLFONScontext
    {
        public uint tex;
        public int width, height;
    }
}
