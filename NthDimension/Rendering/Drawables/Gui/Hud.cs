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

namespace NthDimension.Rendering.Drawables.Gui
{
    using NthDimension.Algebra;
    using Rendering.Configuration;


    public class Hud : Gui
    {
        public GuiElement crossHair;
        public HudNumber fpsCounter;

        public Hud(ApplicationObject parent)
            : base(parent)
        {
            crossHair = new GuiElement(this);
            crossHair.setSizeRel(new Vector2(100, 100));
            crossHair.setMaterial("firstperson\\crosshair.xmf");

            if (Settings.Instance.game.debugMode)
            {
                //fpsCounter = new HudNumber(this);
                //fpsCounter.Position = new Vector2(0, -0.8f);
                //fpsCounter.setSizeRel(new Vector2(80, 160));
                //fpsCounter.digits = 3;
            }
        }
    }
}
