﻿/* LICENSE
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Algebra;

namespace NthDimension.Rendering.Drawables.Gui
{
    public class Menu : Gui
    {
        private GuiElement          cursor;
        private float               speed                               = 1f;
        private Vector2             rawCursorPos                        = new Vector2();
        float                       cursorSmoothness                    = 0.5f;
        // TODO:: Refactor-Revise
        public Vector2              ButtonSize                          = new Vector2(100,50);

        public Menu(ApplicationObject parent)
            : base(parent)
        {

            GuiElement backGround = new GuiElement(this);
            backGround.setMaterial("hud\\background.xmf");

            cursor = new GuiElement(this);
            cursor.setSizeRel(new Vector2(100, 100));
            cursor.setMaterial("hud\\cursor.xmf");

            ButtonList bList = new ButtonList(backGround);
            bList.setSizeRel(new Vector2(200, 700));
            bList.Position = new Vector2(-0.85f, 0f);

            Button button = new Button(bList);
            button.setSizeRel(new Vector2(200, 100));
            button.setMaterial("hud\\resume_button.xmf");
            button.HandlerClick = Resume;

            Button button2 = new Button(bList);
            button2.setSizeRel(new Vector2(200, 100));
            button2.setMaterial("hud\\exit_button.xmf");
            button2.HandlerClick = Exit;


            Button button3 = new Button(bList);
            button3.setSizeRel(new Vector2(200, 100));
            //button3.setMaterial("hud\\resume_button.xmf");

            Button button4 = new Button(bList);
            button4.setSizeRel(new Vector2(200, 100));
            //button4.setMaterial("hud\\resume_button.xmf");

            Button button5 = new Button(bList);
            button5.setSizeRel(new Vector2(200, 100));
            //button5.setMaterial("hud\\resume_button.xmf");
        }

        public override void Update()
        {
            if (IsVisible)
            {
                performHover(cursor.Position);
                updateChilds();
            }
        }

        internal void moveCursor(System.Drawing.Point point)
        {
            rawCursorPos = rawCursorPos + Vector2.Divide(new Vector2(point.X, point.Y), sizePx) * speed;
            rawCursorPos.X = GenericMethods.Clamp(rawCursorPos.X, -1, 1);
            rawCursorPos.Y = GenericMethods.Clamp(rawCursorPos.Y, -1, 1);
            cursor.Position = cursorSmoothness * cursor.Position + rawCursorPos * (1 - cursorSmoothness);

        }

        internal void performClick()
        {
            performClick(cursor.Position);
        }

        public static void Resume(Button caller)
        {
            //caller.gameWindow.enterGame();
            ApplicationBase.Instance.enterGame();
        }

        public static void Exit(Button caller)
        {
            //caller.gameWindow.exitGame();
            ApplicationBase.Instance.exitGame();
        }
    }
}
