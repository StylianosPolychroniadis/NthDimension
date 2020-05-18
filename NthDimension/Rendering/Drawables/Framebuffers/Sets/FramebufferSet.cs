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

using NthDimension.Rendering.Configuration;

namespace NthDimension.Rendering.Drawables.Framebuffers
{
    using NthDimension.Algebra;
    using NthDimension.Rasterizer;
    using Rendering.Configuration;
    public class FramebufferSet : ApplicationObject
    {
        // Output
        public Framebuffer          OutputFb;                       
        // Scene
        public Framebuffer          SceneFramebuffer;                       // v
        public Framebuffer          SceneBackdropFb;                        // v
        public Framebuffer          SceneDefInfoFb;
        // Light
        public Framebuffer          LightFramebuffer;                       // v
        public Framebuffer          LightBlurFramebuffer;                   // v
        // Reflection
        public Framebuffer          ReflectionFramebuffer;                  // v
        // Selection
        public Framebuffer          SelectionFb;                            // v
        public Framebuffer          SelectionblurFb;                        // v
        public Framebuffer          SelectionblurFb2;                       // v
        // Screen Normal (ssn)
        public Framebuffer          ScreenSpaceNormalFb;                    // v
        // Ambient Occlusion (ao)
        public Framebuffer          AoFramebuffer;                          // v
        public Framebuffer          AoPreFramebuffer;                       // v
        public Framebuffer          AoBlurFramebuffer;                      // v
        public Framebuffer          AoBlurFramebuffer2;                     // v
        // Bloom
        public Framebuffer          BloomFramebuffer;                       // v
        public Framebuffer          BloomFramebuffer2;                      // v
        // Depth of Field (dof)
        public Framebuffer          DofFramebuffer;                         // v
        public Framebuffer          DofPreFramebuffer;                      // v
        public Framebuffer          DofFramebuffer2;                        // v
        // Editor 
        public Framebuffer          EditorFramebuffer;

        public RenderOptions        renderOptions;

        #region Ctor
        public FramebufferSet(FramebufferCreator mFramebufferCreator, Vector2 size, Framebuffer outputFb)
        {
            RenderOptions mOptions = new RenderOptions(size);

            createFramebufferSet(mFramebufferCreator, outputFb, mOptions);
        }
        public FramebufferSet(FramebufferCreator mFramebufferCreator, Framebuffer outputFb, RenderOptions mOptions)
        {

            createFramebufferSet(mFramebufferCreator, outputFb, mOptions);
        }
        #endregion

        /// <summary>
        /// Creates all framebuffers. (TODO:: This MUST be updated on GameWindow.Resize())
        /// </summary>
        /// <param name="mFramebufferCreator"></param>
        /// <param name="outputFb"></param>
        /// <param name="mOptions"></param>
        private void createFramebufferSet(FramebufferCreator mFramebufferCreator, Framebuffer outputFb, RenderOptions mOptions)
        {
            // This MUST be updated on GameWindow.Resize()

            Vector2                 size                        = mOptions.size;
            Vector2                 size2                       = size * mOptions.quality;
            Vector2                 size3                       = size2 * 0.3f;
            PixelInternalFormat     pixelFormatRgbaLo           = PixelInternalFormat.Rgba8;
            PixelInternalFormat     pixelFormatRgbaHi           = PixelInternalFormat.Rgba16f;
            bool                    multisample                 = !(ApplicationBase.Instance.DeviceVendor == DeviceVendor.INTEL);


            if (null != SceneBackdropFb) SceneBackdropFb.Delete();
            SceneBackdropFb = mFramebufferCreator.createFrameBuffer("SceneBackdropFb",
                                                                                        (int)size.X,
                                                                                        (int)size.Y,
                                                                                        pixelFormatRgbaLo,
                                                                                        multisample);                   // No multisampling           

            #region SSN
            if (null != ScreenSpaceNormalFb) ScreenSpaceNormalFb.Delete();
            ScreenSpaceNormalFb = mFramebufferCreator.createFrameBuffer((int)size2.X,
                                                                                        (int)size2.Y,
                                                                                        pixelFormatRgbaHi,
                                                                                        false);                        // No multisampling
            ScreenSpaceNormalFb.ClearColor = new Color4(0f, 0f, 0f, 100f);
            #endregion SSN

            #region Lightmap
            if (null != LightFramebuffer) LightFramebuffer.Delete();
            LightFramebuffer                    = mFramebufferCreator.createFrameBuffer("LightFramebuffer", 
                                                                                        (int)size.X, 
                                                                                        (int)size.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        false);                         // No multisampling
            LightFramebuffer.ClearColor             = new Color4(0f, 0f, 0f, 0f);

            if (null != LightBlurFramebuffer) LightBlurFramebuffer.Delete();
            LightBlurFramebuffer                = mFramebufferCreator.createFrameBuffer("LightBlurFramebuffer", 
                                                                                        (int)size.X, 
                                                                                        (int)size.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        false);                         // No multisampling
            #endregion Lightmap
            
            #region Reflection
            if (null != ReflectionFramebuffer) ReflectionFramebuffer.Delete();
            ReflectionFramebuffer               = mFramebufferCreator.createFrameBuffer("ReflectionFramebuffer", 
                                                                                        (int)size.X, 
                                                                                        (int)size.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        false);                         // No multisampling
            #endregion Reflection

            #region Scene
            if (null != SceneFramebuffer) SceneFramebuffer.Delete();
            SceneFramebuffer                    = mFramebufferCreator.createFrameBuffer("SceneFramebuffer", 
                                                                                        (int)size.X, 
                                                                                        (int)size.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        multisample);                   // No multisampling
            SceneFramebuffer.ClearColor             = ApplicationBase.Instance.VAR_ScreenColor.ToColor4();

           
            #endregion Scene

            #region Selection
            if (null != SelectionFb) SelectionFb.Delete();
            SelectionFb                         = mFramebufferCreator.createFrameBuffer("SelectionFb", 
                                                                                        (int)size.X, 
                                                                                        (int)size.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        multisample);                   // No multisampling

            if (null != SelectionblurFb) SelectionblurFb.Delete();
            SelectionblurFb                         = mFramebufferCreator.createFrameBuffer("SelectionblurFb", 
                                                                                        (int)size3.X, 
                                                                                        (int)size3.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        false);                          // No multisampling
            if (null != SelectionblurFb2) SelectionblurFb2.Delete();
            SelectionblurFb2                        = mFramebufferCreator.createFrameBuffer("SelectionblurFb2", 
                                                                                            (int)size3.X, 
                                                                                            (int)size3.Y,
                                                                                            pixelFormatRgbaLo, 
                                                                                            multisample);
            #endregion Selection

            #region DOF
            // DOF
            if (mOptions.depthOfField)
            {
                if (null != DofPreFramebuffer) DofPreFramebuffer.Delete();
                DofPreFramebuffer               = mFramebufferCreator.createFrameBuffer("DofPreFramebuffer", 
                                                                                        (int)size2.X, 
                                                                                        (int)size2.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        multisample);                   // No multisampling

                if (null != DofFramebuffer) DofFramebuffer.Delete();
                DofFramebuffer                  = mFramebufferCreator.createFrameBuffer("DofFramebuffer", 
                                                                                        (int)size2.X, 
                                                                                        (int)size2.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        multisample);                   // No multisampling                
            }

            if (null != DofFramebuffer2) DofFramebuffer2.Delete();
            DofFramebuffer2 = mFramebufferCreator.createFrameBuffer("DofFramebuffer2",
                                                                                        (int)size2.X,
                                                                                        (int)size2.Y,
                                                                                        pixelFormatRgbaLo,
                                                                                        multisample);
            #endregion DOF

            #region SSAO
            // SSAO
            if (null != AoBlurFramebuffer2) AoBlurFramebuffer2.Delete();
            AoBlurFramebuffer2                  = mFramebufferCreator.createFrameBuffer("AoBlurFramebuffer2", 
                                                                                        (int)size2.X, 
                                                                                        (int)size2.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        multisample);
            Color4 aoColor = ApplicationBase.Instance.VAR_ScreenColor.ToColor4(); aoColor.A = 0.5f;
            AoBlurFramebuffer2.ClearColor = aoColor; //new Color4(128f, 128f, 128f, 0.5f);

            if (mOptions.ssAmbientOcclusion)
            {
                
                if (null != AoPreFramebuffer) AoPreFramebuffer.Delete();
                AoPreFramebuffer                = mFramebufferCreator.createFrameBuffer("AoPreFramebuffer", 
                                                                                        (int)size2.X, 
                                                                                        (int)size2.Y,
                                                                                        pixelFormatRgbaHi, 
                                                                                        false);                             // No multisampling

                if (null != AoFramebuffer) AoFramebuffer.Delete();
                AoFramebuffer                   = mFramebufferCreator.createFrameBuffer("AoFramebuffer", 
                                                                                        (int)size2.X, 
                                                                                        (int)size2.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        false);                             // No multisampling

                if (null != AoBlurFramebuffer) AoBlurFramebuffer.Delete();
                AoBlurFramebuffer               = mFramebufferCreator.createFrameBuffer("AoBlurFramebuffer", 
                                                                                        (int)size2.X, 
                                                                                        (int)size2.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        false);                             // No multisampling
            }
            else
            {
                // if ao is set of make the buffer grey 
                //aoBlurFramebuffer2.enable();
            }
            #endregion SSAO

            #region Bloom
            // Bloom
            if (null != BloomFramebuffer) BloomFramebuffer.Delete();
            BloomFramebuffer                    = mFramebufferCreator.createFrameBuffer("BloomFramebuffer", 
                                                                                        (int) size3.X, 
                                                                                        (int) size3.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        multisample);                              // No multisampling
            if (null != BloomFramebuffer2) BloomFramebuffer2.Delete();
            BloomFramebuffer2                   = mFramebufferCreator.createFrameBuffer("BloomFramebuffer2", 
                                                                                        (int) size3.X, 
                                                                                        (int) size3.Y,
                                                                                        pixelFormatRgbaLo, 
                                                                                        multisample);                              // No multisampling
            #endregion Bloom

            #region Editor
            if (null != EditorFramebuffer) EditorFramebuffer.Delete();
            EditorFramebuffer = mFramebufferCreator.createFrameBuffer("Editor",
                                                                            (int)size.X,
                                                                            (int)size.Y,
                                                                            pixelFormatRgbaLo,
                                                                            multisample);                               // ?????    
            EditorFramebuffer.ClearColor = ApplicationBase.Instance.VAR_ScreenColor.ToColor4();
            #endregion Editor

            this.renderOptions              = mOptions;
            this.OutputFb                   = outputFb;
        }

        public void Delete()
        {
            if (null != OutputFb)                   OutputFb.Delete();
            if (null != SceneFramebuffer)           SceneFramebuffer.Delete();
            if (null != SceneBackdropFb)            SceneBackdropFb.Delete();
            if (null != SceneDefInfoFb)             SceneDefInfoFb.Delete();
            if (null != LightFramebuffer)           LightFramebuffer.Delete();
            if (null != LightBlurFramebuffer)       LightBlurFramebuffer.Delete();
            if (null != ReflectionFramebuffer)      ReflectionFramebuffer.Delete();
            if (null != SelectionFb)                SelectionFb.Delete();
            if (null != SelectionblurFb)            SelectionblurFb.Delete();
            if (null != SelectionblurFb2)           SelectionblurFb2.Delete();
            if (null != ScreenSpaceNormalFb)        ScreenSpaceNormalFb.Delete();
            if (null != AoPreFramebuffer)           AoPreFramebuffer.Delete();
            if (null != AoFramebuffer)              AoFramebuffer.Delete();
            if (null != AoBlurFramebuffer)          AoBlurFramebuffer.Delete();
            if (null != AoBlurFramebuffer2)         AoBlurFramebuffer2.Delete();
            if (null != BloomFramebuffer)           BloomFramebuffer.Delete();
            if (null != BloomFramebuffer2)          BloomFramebuffer2.Delete();
            if (null != DofPreFramebuffer)          DofPreFramebuffer.Delete();
            if (null != DofFramebuffer)             DofFramebuffer.Delete();
            if (null != DofFramebuffer2)            DofFramebuffer2.Delete();
            if (null != EditorFramebuffer)          EditorFramebuffer.Delete();

        }
    }
}
