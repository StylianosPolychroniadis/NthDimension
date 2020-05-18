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

using System.Linq;

using OpenTK;
using NthDimension.Rendering.Utilities;
using NthDimension.Rasterizer;

namespace NthDimension.Rendering.Drawables.Framebuffers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using NthDimension.Algebra;

    public class FramebufferCreator : ApplicationObject
    {
        public List<Framebuffer> Framebuffers = new List<Framebuffer> { };
        public Hashtable FramebufferNames = new Hashtable();

        public Framebuffer defaultFb;

        public FramebufferCreator(int width, int height)
        {
            defaultFb = new DefaultFramebuffer(new Vector2(width, height), this);
        }

        public void Delete()
        {
            foreach (Framebuffer fb in Framebuffers)
                fb.Delete();

            Framebuffers.Clear();
            FramebufferNames.Clear();

            defaultFb.Delete();
        }

        #region Randoms

        Random rnd = new Random();
        public const float rScale = 3f;

        /// <summary>Returns a random Float in the range [-0.5*scale..+0.5*scale]</summary>
        public float GetRandom()
        {
            return (float)(rnd.NextDouble() - 0.5) * rScale;
        }

        /// <summary>Returns a random Float in the range [0..1]</summary>
        public float GetRandom0to1()
        {
            return (float)rnd.NextDouble();
        }

        #endregion Randoms

        public Framebuffer createFrameBuffer(string name, int FboWidth, int FboHeight)
        {
            return createFrameBuffer(name, FboWidth, FboHeight, PixelInternalFormat.Rgba8, true);
        }

        public Framebuffer createFrameBuffer(int FboWidth, int FboHeight)
        {
            return createFrameBuffer(null, FboWidth, FboHeight, PixelInternalFormat.Rgba8, true);
        }

        public Framebuffer createFrameBuffer(int FboWidth, int FboHeight, PixelInternalFormat precision, bool multisampling)
        {
            return createFrameBuffer(null, FboWidth, FboHeight, precision, multisampling);
        }

        public Framebuffer createFrameBuffer(string name, int FboWidth, int FboHeight, PixelInternalFormat precision, bool multisampling)
        {
            int FboHandle, ColorTexture, DepthTexture;

            // Create Color Tex
            ApplicationBase.Instance.Renderer.GenTextures(1, out ColorTexture);
            ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, ColorTexture);
            ApplicationBase.Instance.Renderer.TexImage2D(TextureTarget.Texture2D, 0, precision, FboWidth, FboHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, multisampling ? (int)TextureMinFilter.Linear : (int)TextureMinFilter.Nearest);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, multisampling ? (int)TextureMinFilter.Linear : (int)TextureMinFilter.Nearest);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Create Depth Tex
            ApplicationBase.Instance.Renderer.GenTextures(1, out DepthTexture);
            ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, DepthTexture);
            ApplicationBase.Instance.Renderer.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, FboWidth, FboHeight, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero); // things go horribly wrong if DepthComponent's Bitcount does not match the main Framebuffer's Depth
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, multisampling ? (int)TextureMinFilter.Linear : (int)TextureMinFilter.Nearest);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, multisampling ? (int)TextureMinFilter.Linear : (int)TextureMinFilter.Nearest);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Create a FBO and attach the textures
            ApplicationBase.Instance.Renderer.GenFrameBuffers(1, out FboHandle);

            #region Test for Error

            string logString = string.Format("<> Creating framebuffer {0}", name);
            FramebufferErrorCode fbo_err = ApplicationBase.Instance.Renderer.CheckFrameBufferStatus(FramebufferTarget.FramebufferExt);

            switch (fbo_err)
            {
                case FramebufferErrorCode.FramebufferCompleteExt:
                {
                        string tabs = string.Empty;
                        string log = string.Empty;
                        while (log.Length + tabs.Length*5 < 51)
                        {
                            tabs += "\t";
                            log = string.Format(" <> Create FBO[{0}]: {1} Complete & Valid! ", name, tabs);
                        }
                        logString = log;
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
                    {
                        logString = (string.Format("FBO [{0}]: One or more attachment points are not complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.", name));
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
                    {
                        logString = (string.Format("FBO [{0}]:  There are no attachments.", name));
                        break;
                    }
                /* case  FramebufferErrorCode.GL_FRAMEBUFFER_INCOMPLETE_DUPLICATE_ATTACHMENT_EXT: 
                     {
                         ConsoleUtil.log("FBO: An object has been attached to more than one attachment point.");
                         break;
                     }*/
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                    {
                        logString = (string.Format("FBO [{0}]:  Attachments are of different size. All attachments must have the same width and height.", name));
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                    {
                        logString = (string.Format("FBO [{0}]:  The color attachments have different format. All color attachments must have the same format.", name));
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
                    {
                        logString = (string.Format("FBO [{0}]:  An attachment point referenced by Game.Instance.Renderer.DrawBuffers() doesn’t have an attachment.", name));
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
                    {
                        logString = (string.Format("FBO [{0}]:  The attachment point referenced by Game.Instance.Renderer.ReadBuffers() doesn’t have an attachment.", name));
                        break;
                    }
                case FramebufferErrorCode.FramebufferUnsupportedExt:
                    {
                        logString = (string.Format("FBO [{0}]:  This particular FBO configuration is not supported by the implementation.", name));
                        break;
                    }
                default:
                    {
                        logString = (string.Format("FBO [{0}]:  Status unknown. (yes, this is really bad.)", name));
                        break;
                    }
            }

            if (fbo_err == FramebufferErrorCode.FramebufferComplete ||
                fbo_err == FramebufferErrorCode.FramebufferCompleteExt)
                    ConsoleUtil.log(string.Format("{0} ID: {1} {2} {3}", logString, FboHandle, this.fboInfo(), Environment.NewLine));
            else
                    ConsoleUtil.errorlog(string.Empty, logString);

            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Create FrameBuffer() {0}:{1}", this.GetType(), Name));
            #endregion Test for Error

            Framebuffer framebuffer     = new Framebuffer(FboHandle, ColorTexture, DepthTexture, new Vector2(FboWidth, FboHeight), this);
            framebuffer.Name            = name;

            if (name != null)
                registerFramebuffer(framebuffer);

            return framebuffer;
        }

        private string fboInfo()
        {
            string ret = string.Empty;
            // using FBO might have changed states, e.g. the FBO might not support stereoscopic views or double buffering
            int[] queryinfo = new int[6];
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.MaxColorAttachmentsExt,       out queryinfo[0]);
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.AuxBuffers,                   out queryinfo[1]);
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.MaxDrawBuffers,               out queryinfo[2]);
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.Stereo,                       out queryinfo[3]);
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.Samples,                      out queryinfo[4]);
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.Doublebuffer,                 out queryinfo[5]);
            //ConsoleUtil.log("max. ColorBuffers: " + queryinfo[0] + " max. AuxBuffers: " + queryinfo[1] + " max. DrawBuffers: " + queryinfo[2] + "\nStereo: " + queryinfo[3] + " Samples: " + queryinfo[4] + " DoubleBuffer: " + queryinfo[5]);

            //ConsoleColor tmp = Console.ForegroundColor;
            //Console.Write("Max Clr Attcmns: ");
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.Write(queryinfo[0]);

            ret += string.Format("Colr {0} ", queryinfo[0]);

            //Console.ForegroundColor = tmp;
            //Console.Write(" Max Aux Buffers: ");
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.Write(queryinfo[1]);

            ret += string.Format("Aux {0} ", queryinfo[1]);

            //Console.ForegroundColor = tmp;
            //Console.Write(" Max Draw Buffers: ");
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.Write(queryinfo[2]);

            ret += string.Format("Draw {0} ", queryinfo[2]);

            //Console.ForegroundColor = tmp;
            //Console.Write(" Stereo: ");
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.Write(queryinfo[3]);

            ret += string.Format("Stereo {0} ", queryinfo[3]);

            //Console.ForegroundColor = tmp;
            //Console.Write(" Samples: ");
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.Write(queryinfo[4]);

            ret += string.Format("Sample {0} ", queryinfo[4]);


            //Console.ForegroundColor = tmp;
            //Console.Write(" Double buffer: ");
            //Console.ForegroundColor = ConsoleColor.Yellow;
            ret += string.Format("DblBuf {0} ", queryinfo[5]);
            
            //Console.WriteLine(queryinfo[5]);
            //Console.ForegroundColor = tmp;

            return ret;
        }

        private void registerFramebuffer(Framebuffer newFb)
        {
            Framebuffer fbo = Framebuffers.Where(m => m.Name == newFb.Name).FirstOrDefault();

            if (null != fbo)
                Framebuffers.Remove(fbo);

            Framebuffers.Add(newFb);

            int identifier = Framebuffers.Count;
            if (FramebufferNames.ContainsKey(newFb.Name))
                FramebufferNames.Remove(newFb.Name);

            FramebufferNames.Add(newFb.Name, identifier);

            ApplicationBase.Instance.TextureLoader.fromFramebuffer(newFb.Name + "color", newFb.ColorTexture);
        }

        public void createLightBuffer(string name, int size, bool multisampling)
        {
            ConsoleUtil.log(string.Format("Creating light framebuffer {0}", name));

            int FboHandle;
            int ColorTexture;
            int DepthTexture;

            int sampling = 0;
            if (multisampling)
                sampling = (int)TextureMinFilter.Linear;
            else
                sampling = (int)TextureMinFilter.Nearest;

            // Create Depth Tex
            ApplicationBase.Instance.Renderer.GenTextures(1, out DepthTexture);
            ApplicationBase.Instance.Renderer.BindTexture(TextureTarget.Texture2D, DepthTexture);
            ApplicationBase.Instance.Renderer.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, size, size, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
            // things go horribly wrong if DepthComponent's Bitcount does not match the main Framebuffer's Depth
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, sampling);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, sampling);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            ApplicationBase.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            //Game.Instance.Renderer.GenerateMipmap( GenerateMipmapTarget.Texture2D );

            // Create a FBO and attach the textures
            ApplicationBase.Instance.Renderer.GenFrameBuffers(1, out FboHandle);

            #region Test for Error

            switch (ApplicationBase.Instance.Renderer.CheckFrameBufferStatus(FramebufferTarget.FramebufferExt))
            {
                case FramebufferErrorCode.FramebufferCompleteExt:
                    {
                        Utilities.ConsoleUtil.log("FBO: The framebuffer is complete and valid for rendering.", false);
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
                    {
                        Utilities.ConsoleUtil.log("FBO: One or more attachment points are not complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.", false);
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
                    {
                        Utilities.ConsoleUtil.log("FBO: There are no attachments.", false);
                        break;
                    }
                /* case  FramebufferErrorCode.GL_FRAMEBUFFER_INCOMPLETE_DUPLICATE_ATTACHMENT_EXT: 
                     {
                         ConsoleUtil.log("FBO: An object has been attached to more than one attachment point.");
                         break;
                     }*/
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                    {
                        Utilities.ConsoleUtil.log("FBO: Attachments are of different size. All attachments must have the same width and height.", false);
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                    {
                        Utilities.ConsoleUtil.log("FBO: The color attachments have different format. All color attachments must have the same format.", false);
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
                    {
                        Utilities.ConsoleUtil.log("FBO: An attachment point referenced by Game.Instance.Renderer.DrawBuffers() doesn’t have an attachment.", false);
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
                    {
                        Utilities.ConsoleUtil.log("FBO: The attachment point referenced by Game.Instance.Renderer.ReadBuffers() doesn’t have an attachment.", false);
                        break;
                    }
                case FramebufferErrorCode.FramebufferUnsupportedExt:
                    {
                        Utilities.ConsoleUtil.log("FBO: This particular FBO configuration is not supported by the implementation.", false);
                        break;
                    }
                default:
                    {
                        Utilities.ConsoleUtil.log("FBO: Status unknown. (yes, this is really bad.)", false);
                        break;
                    }
            }

            // using FBO might have changed states, e.g. the FBO might not support stereoscopic views or double buffering
            int[] queryinfo = new int[6];
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.MaxColorAttachmentsExt, out queryinfo[0]);
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.AuxBuffers, out queryinfo[1]);
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.MaxDrawBuffers, out queryinfo[2]);
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.Stereo, out queryinfo[3]);
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.Samples, out queryinfo[4]);
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.Doublebuffer, out queryinfo[5]);
            //ConsoleUtil.log("max. ColorBuffers: " + queryinfo[0] + " max. AuxBuffers: " + queryinfo[1] + " max. DrawBuffers: " + queryinfo[2] + "\nStereo: " + queryinfo[3] + " Samples: " + queryinfo[4] + " DoubleBuffer: " + queryinfo[5]);

            //ConsoleUtil.log("Last GL Error: " + Game.Instance.Renderer.GetError(), false);

            #endregion Test for Error

            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Create FrameBuffer() {0}:{1}", this.GetType(), Name));

            int identifier = Framebuffers.Count;

            ColorTexture = 0;

            Framebuffer myFramebuffer = new Framebuffer(FboHandle, ColorTexture, DepthTexture, new Vector2(size, size), this);

            ApplicationBase.Instance.TextureLoader.fromFramebuffer(name + "light", myFramebuffer.DepthTexture);

            Framebuffers.Add(myFramebuffer);
            FramebufferNames.Add(name, identifier);

            //return new Framebuffer(FboHandle, ColorTexture, DepthTexture, new Vector2(FboWidth, FboHeight), mGameWindow);
        }

        public Framebuffer getFrameBuffer(string name)
        {
            int id = (int)FramebufferNames[name];
            return Framebuffers[id];
        }
    }
}
