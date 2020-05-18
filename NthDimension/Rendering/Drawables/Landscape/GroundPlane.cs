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

using NthDimension.Rendering.Scenegraph;

namespace NthDimension.Rendering.Drawables.Models
{
    using System.Text;
    using Rendering.Scenegraph;
    using Rasterizer;

    class GroundPlane : Model
    {
        public GroundPlane(SceneGame mScene) : base(mScene)
        {
        }

        //public override void draw(ViewInfo curView, bool renderlayer)
        //{
        //    //Game.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, 100);
        //    //Game.Instance.Renderer.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, 100);
        //    base.draw(curView, renderlayer);
        //}

        public override void save(ref StringBuilder sb, int level)
        {
            saveChilds(ref sb, level);
        }

        public void CreateNavMesh()
        {

        }
        
    }
}
