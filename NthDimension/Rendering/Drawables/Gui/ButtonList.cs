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

using NthDimension.Algebra;

namespace NthDimension.Rendering.Drawables.Gui
{
    public class ButtonList : GuiElement
    {
        public ButtonList(Gui parent)
            : base(parent)
        {
        }

        public override void draw()
        {
            drawChilds();
        }

        public override int addChild(ApplicationObject newChild)
        {
            childs.Add(newChild);
            calculateChildPos();

            return childs.Count - 1;
        }

        private void calculateChildPos()
        {
            Vector2 direction = new Vector2(0, Size.Y);

            Vector2 from = Position - direction;
            Vector2 to = Position + direction;

            for (int i = 0; i < childs.Count; i++)
            {
                Button gChild = (Button)childs[i];

                float iRel = (float)i / (childs.Count - 1);

                gChild.Position = from * iRel + to * (1 - iRel);

                //childs[i] = gChild;
            }
        }

        public override void removeChild(ApplicationObject reChild)
        {
            childs.Remove(reChild);
        }
    }
}
