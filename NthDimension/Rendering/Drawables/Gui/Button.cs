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
    public class Button : GuiElement
    {
        Vector2                     Min;
        Vector2                     Max;

        OnClick                     handlerClick;

        float                       clicked;

        private const string        defaultMaterialFile = "hud\\blank_icon.xmf";

        public Button(Gui parent)
            : base(parent)
        {
            setMaterial(defaultMaterialFile);
        }

        public OnClick HandlerClick { get { return handlerClick; } set { handlerClick = value; } }

        public override Vector2 Size
        {
            get { return screenSize; }
            set { screenSize = value; updateBB(); }
        }

        public override Vector2 Position
        {
            get { return screenPosition; }
            set { screenPosition = value; updateBB(); }
        }

        private void updateBB()
        {
            Min = Position - Size;
            Max = Position + Size;
        }

        public override void performHover(Vector2 pos)
        {
            hoverChilds(pos);

            if (pos.X > Min.X && pos.X < Max.X
                && pos.Y > Min.Y && pos.Y < Max.Y)
            {
                color = Gui.colorA;
            }
            else
            {
                color = Gui.colorB * 0.2f;
            }

            color = color + Vector4.One * clicked * 0.2f;
        }


        public override void performClick(Vector2 pos)
        {
            clickChilds(pos);

            if (pos.X > Min.X && pos.X < Max.X
                && pos.Y > Min.Y && pos.Y < Max.Y)
            {
                clicked = 1;
                if (handlerClick != null)
                    handlerClick(this);
            }
        }

        public override void Update()
        {
            clicked *= 0.8f;
        }
    }
}
