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

#define FBOSHADOW_DEBUG

namespace NthDimension.Rendering.Configuration
{
    using NthDimension.Algebra;
    public class VideoSettings
    {
        public int          windowWidth                 = 1366;
        public int          windowHeight                = 768;

        public int          virtualScreenWidth          = 1680;
        public int          virtualScreenHeight         = 1000;

        public int          targetFrameRate             = 60;

        public bool         fullScreen                  = false;
        public bool         highResolution              = false;
        public bool         useShadows                  = true;
#if FBOSHADOW_DEBUG
        //public bool         pssmShadows                 = false;
#endif
        public bool         postProcessing              = true;
        public bool         ssAmbientOccluison          = true;
        public bool         bloom                       = false;
        public bool         depthOfField                = false;
        public bool         lightmapSmoothing           = true;
        public bool         Particles                   = false;
        public float        gamma                       = 1.0f;         // 2.2 Bright Room, 2.4 Dark Room, 1.8 Apple, etc
        public float        effectsQuality              = 1f;           // [ 0.0 ...  1.0   ]         // Low 0.1 Mid 0.5 Hi 1.0+          [Performance Bottleneck]
        public float        effectsUpdateInterval       = 0.01f;        // [0.001 ... 0.01  ]         // Low +0.5 Mid 0.1 Hi 0.01         Time (smaller means faster updates)
        public int          shadowResolution            = 256;          // [  32 ...  2048  ]         // Low 32 Mid 128 Hi 512 Ultra 1024 (Extra Ultra 2048) Resolution in Pixels^2
        public float        shadowQuality               = 0.5f;         // [ 0.0 ...  10.0  ]         // Not a performance bottleneck. Leave at 1.0         
        public float        shadowUpdateInterval        = 0.5f;         // [0.01 ...  0.5   ]         // Low +0.5 Mid 0.1 Hi 0.01         Time (smaller means faster updates)
        public float        waterScreenPercentage       = 0.5f;
        

        public QualityLevel shadow                      = QualityLevel.High;
        public QualityLevel shader                      = QualityLevel.High;
        public QualityLevel lighting                    = QualityLevel.High;

        public enum Target { main, water, window };


        public NthDimension.Algebra.Vector2 CreateSizeVector(Target target)
        {
            switch (target)
            {
                case Target.main:
                    return new Vector2(virtualScreenWidth, virtualScreenHeight);
                case Target.water:
                    return new Vector2(virtualScreenWidth, virtualScreenHeight) * waterScreenPercentage;
                case Target.window:
                    return new Vector2(windowWidth, windowHeight);      // TODO:: What about OnResize()???
                default:
                    return Vector2.Zero;
            }
        }

        /// <summary>
        /// creates a RenderOptions instance for the chosen target.
        /// </summary>
        /// <returns></returns>
        public RenderOptions CreateRenderOptions(Target target)
        {
            RenderOptions result = new RenderOptions(CreateSizeVector(target));

            switch (target)
            {
                case Target.main:
                    result.postProcessing = postProcessing;
                    result.ssAmbientOcclusion = ssAmbientOccluison;
                    result.bloom = bloom;
                    result.depthOfField = depthOfField;

                    break;
                case Target.water:
                    result.postProcessing = postProcessing;
                    /*
			        result.ssAmbientOccluison = ssAmbientOccluison;
			        result.bloom = bloom;
			        result.depthOfField = depthOfField;
                    */

                    break;
                default:
                    break;
            }



            result.quality = effectsQuality;

            return result;
        }
    }
}
