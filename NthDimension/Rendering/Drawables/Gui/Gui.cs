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



namespace NthDimension.Rendering.Drawables.Gui
{
    using NthDimension.Algebra;
    using NthDimension.Rasterizer;
    public class Gui : Quad2d
    {
        public Vector2 sizePx;

        public static Vector4 colorA = new Vector4(0.8f, 0.3f, 0.8f, 1.0f);
        public static Vector4 colorB = new Vector4(0.6f, 0.7f, 1.0f, 1.0f);

        public Gui(ApplicationObject parent)
        {
            Parent = parent;
            sizePx = ApplicationBase.Instance.VAR_ScreenSize_Virtual;
        }

        public virtual void performClick(Vector2 pos)
        {
            clickChilds(pos);
        }

        public override void Update()
        {
            updateChilds();
        }

        public virtual void clickChilds(Vector2 pos)
        {
            foreach (var child in childs)
            {
                Gui gChild = (Gui)child;
                gChild.performClick(pos);
            }
        }

        public virtual void performHover(Vector2 pos)
        {
            hoverChilds(pos);
        }

        public virtual void hoverChilds(Vector2 pos)
        {
            foreach (var child in childs)
            {
                Gui gChild = (Gui)child;
                gChild.performHover(pos);
            }
        }

        public Gui() { }

        public override void draw()
        {
            if (IsVisible)
            {
                //GameBase.Instance.Renderer.Enable(EnableCap.Blend);
                ApplicationBase.Instance.Renderer.BlendEnabled = true;
                drawChilds();
            }

        }

        public void drawChilds()
        {
            foreach (Drawable child in childs)
            {
                child.draw();
            }
        }
    }
}
