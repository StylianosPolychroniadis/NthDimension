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

namespace NthDimension.Rendering.Drawables.Models
{
    public class GhostModel : Model
    {
        public GhostModel(ApplicationObject parent) : base(parent)
        {
            this.IgnoreCulling = true;
        }

        public override void draw(ViewInfo curView, bool targetLayer)
        {
        }

        public override void drawShadow(ViewInfo curView)
        {
        }

        public override void drawNormal(ViewInfo curView)
        {
        }

        public override void drawDefInfo(ViewInfo curView)
        {
        }

        public override void Update()
        {
            updateSelection();
            updateChilds();
        }
    }
}
