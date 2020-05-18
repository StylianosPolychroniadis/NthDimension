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

using NthDimension.Rendering.Drawables.Gui;
using NthDimension.Rendering.ViewControllers;

namespace NthDimension.Rendering.Drawables.Tools
{
    using Rendering.Drawables.Gui;
    public class GameMenu : FirstPersonViewController
    {
        private Menu MenuUi;

        public GameMenu(ApplicationUser parent, ApplicationUserInput input)
            : base(parent, input)
        {
            weaponModel.IsVisible = false;
            icon.IsVisible = false;

            MenuUi = new Menu(this);
            MenuUi.IsVisible = false;
            Scene.Guis.Add(MenuUi);
        }

        public override void Update()
        {
            if (Parent.FirstPersonView == this)
            {
                MenuUi.IsVisible = true;
                MenuUi.moveCursor(GameInput.MouseDelta);

                // fire player fire
                bool K = false;

#if _WINDOWS_
                //K = m_gameInput.mouse[OpenTK.Input.MouseButton.Left];
#endif
                K = GameInput.FIRE;

                if (K && !prevK)
                {
                    MenuUi.performClick();
                }
                else if (!K && prevK)
                {
                    //fireUp();
                }
                prevK = K;

                updateChilds();
            }
            else
            {
                MenuUi.IsVisible = false;
            }
        }
    }
}
