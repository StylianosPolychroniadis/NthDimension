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

namespace NthDimension.Rendering.Configuration
{
    using NthDimension.Algebra;

    public struct RenderOptions
    {
        public Vector2          size;
        public float            quality;
        public bool             postProcessing;
        public bool             ssAmbientOcclusion;
        public bool             depthOfField;
        public bool             bloom;

        public RenderOptions(Vector2 size)
        {
            this.size               = size;
            quality                 = Settings.Instance.video.effectsQuality; //0.5f;
            postProcessing          = Settings.Instance.video.postProcessing;
            ssAmbientOcclusion      = Settings.Instance.video.ssAmbientOccluison;
            depthOfField            = Settings.Instance.video.depthOfField;
            bloom                   = Settings.Instance.video.bloom;
        }
    }
}
