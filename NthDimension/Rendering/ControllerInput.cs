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

using System.Drawing;
using NthDimension.Rendering.Scenegraph;

namespace NthDimension.Rendering
{

    // TODO:: Heavy refactor. Implement NthDimension KeyboardState from OpenTK and decouple from Scene

    public class PlayerControllerInput : ApplicationObject
    {
        public Point MouseDelta;
        public bool MouseButtonLeft;
        public bool MouseButtonRight;
        public bool MouseButton3;
        public bool CursorLock = false;
        

        public bool MOVEFORWARD     = false;        //.keyboard[OpenTK.Input.Key.W])
        public bool MOVEBACKWARD    = false;        //.keyboard[OpenTK.Input.Key.S])
        public bool RUN             = false;        //.keyboard[OpenTK.Input.Key.W])


        public bool STRAFELEFT      = false;        //.keyboard[OpenTK.Input.Key.A])
        public bool STRAFERIGHT     = false;        //.keyboard[OpenTK.Input.Key.D])
        public bool JUMP            = false;        //.keyboard[OpenTK.Input.Key.Space])
        public bool CROUCH          = false;
        public bool FIRE            = false;        //.mouse[OpenTK.Input.MouseButton.Left];
        public bool INTERACT        = false;        //.keyboard[OpenTK.Input.Key.E];
        public bool ROTATE          = false;        //.keyboard[Key.Q])
        public bool TOOL01          = false;        //.keyboard[Key.Number1])
        public bool TOOL02          = false;        //.keyboard[Key.Number2])
        public bool TOOL03          = false;        //.keyboard[Key.Number3])
        public bool TOOL04          = false;        //.keyboard[Key.Number4])
        public bool TOOL05          = false;        //.keyboard[Key.Number5])
        public bool CHANGECAMERA      = false;

        public bool ShiftLeft   = false; // 1,
        public bool LShift = false; // 1,
        public bool ShiftRight = false; // 2,
        public bool RShift = false; // 2,
        public bool ControlLeft = false; // 3,
        public bool LControl = false; // 3,
        public bool ControlRight = false; // 4,
        public bool RControl = false; // 4,
        public bool AltLeft = false; // 5,
        public bool LAlt = false; // 5,
        public bool AltRight = false; // 6,
        public bool RAlt = false; // 6,
        public bool WinLeft = false; // 7,
        public bool LWin = false; // 7,
        public bool WinRight = false; // 8,
        public bool RWin = false; // 8,
        public bool Menu = false; // 9,
        public bool F1 = false; // 10,
        public bool F2 = false; // 11,
        public bool F3 = false; // 12,
        public bool F4 = false; // 13,
        public bool F5 = false; // 14,
        public bool F6 = false; // 15,
        public bool F7 = false; // 16,
        public bool F8 = false; // 17,
        public bool F9 = false; // 18,
        public bool F10 = false; // 19,
        public bool F11 = false; // 20,
        public bool F12 = false; // 21,
        public bool F13 = false; // 22,
        public bool F14 = false; // 23,
        public bool F15 = false; // 24,
        public bool F16 = false; // 25,
        public bool F17 = false; // 26,
        public bool F18 = false; // 27,
        public bool F19 = false; // 28,
        public bool F20 = false; // 29,
        public bool F21 = false; // 30,
        public bool F22 = false; // 31,
        public bool F23 = false; // 32,
        public bool F24 = false; // 33,
        public bool F25 = false; // 34,
        public bool F26 = false; // 35,
        public bool F27 = false; // 36,
        public bool F28 = false; // 37,
        public bool F29 = false; // 38,
        public bool F30 = false; // 39,
        public bool F31 = false; // 40,
        public bool F32 = false; // 41,
        public bool F33 = false; // 42,
        public bool F34 = false; // 43,
        public bool F35 = false; // 44,
        public bool Up = false; // 45,
        public bool Down = false; // 46,
        public bool Left = false; // 47,
        public bool Right = false; // 48,
        public bool Enter = false; // 49,
        public bool Escape = false; // 50,
        public bool Space = false; // 51,
        public bool Tab = false; // 52,
        public bool BackSpace = false; // 53,
        public bool Back = false; // 53,
        public bool Insert = false; // 54,
        public bool Delete = false; // 55,
        public bool PageUp = false; // 56,
        public bool PageDown = false; // 57,
        public bool Home = false; // 58,
        public bool End = false; // 59,
        public bool CapsLock = false; // 60,
        public bool ScrollLock = false; // 61,
        public bool PrintScreen = false; // 62,
        public bool Pause = false; // 63,
        public bool NumLock = false; // 64,
        public bool Clear = false; // 65,
        public bool Sleep = false; // 66,
        public bool Keypad0 = false; // 67,
        public bool Keypad1 = false; // 68,
        public bool Keypad2 = false; // 69,
        public bool Keypad3 = false; // 70,
        public bool Keypad4 = false; // 71,
        public bool Keypad5 = false; // 72,
        public bool Keypad6 = false; // 73,
        public bool Keypad7 = false; // 74,
        public bool Keypad8 = false; // 75,
        public bool Keypad9 = false; // 76,
        public bool KeypadDivide = false; // 77,
        public bool KeypadMultiply = false; // 78,
        public bool KeypadSubtract = false; // 79,
        public bool KeypadMinus = false; // 79,
        public bool KeypadAdd = false; // 80,
        public bool KeypadPlus = false; // 80,
        public bool KeypadDecimal = false; // 81,
        public bool KeypadEnter = false; // 82,
        public bool A = false; // 83,
        public bool B = false; // 84,
        public bool C = false; // 85,
        public bool D = false; // 86,
        public bool E = false; // 87,
        public bool F = false; // 88,
        public bool G = false; // 89,
        public bool H = false; // 90,
        public bool I = false; // 91,
        public bool J = false; // 92,
        public bool K = false; // 93,
        public bool L = false; // 94,
        public bool M = false; // 95,
        public bool N = false; // 96,
        public bool O = false; // 97,
        public bool P = false; // 98,
        public bool Q = false; // 99,
        public bool R = false; // 100,
        public bool S = false; // 101,
        public bool T = false; // 102,
        public bool U = false; // 103,
        public bool V = false; // 104,
        public bool W = false; // 105,
        public bool X = false; // 106,
        public bool Y = false; // 107,
        public bool Z = false; // 108,
        public bool Number0 = false; // 109,
        public bool Number1 = false; // 110,
        public bool Number2 = false; // 111,
        public bool Number3 = false; // 112,
        public bool Number4 = false; // 113,
        public bool Number5 = false; // 114,
        public bool Number6 = false; // 115,
        public bool Number7 = false; // 116,
        public bool Number8 = false; // 117,
        public bool Number9 = false; // 118,
        public bool Tilde = false; // 119,
        public bool Minus = false; // 120,
        public bool Plus = false; // 121,
        public bool BracketLeft = false; // 122,
        public bool LBracket = false; // 122,
        public bool BracketRight = false; // 123,
        public bool RBracket = false; // 123,
        public bool Semicolon = false; // 124,
        public bool Quote = false; // 125,
        public bool Comma = false; // 126,
        public bool Period = false; // 127,
        public bool Slash = false; // 128,
        public bool BackSlash = false; // 129,
        public bool LastKey = false; // 130

        public bool ShiftLeft_Previous = false; // 1,
        public bool LShift_Previous = false; // 1,
        public bool ShiftRight_Previous = false; // 2,
        public bool RShift_Previous = false; // 2,
        public bool ControlLeft_Previous = false; // 3,
        public bool LControl_Previous = false; // 3,
        public bool ControlRight_Previous = false; // 4,
        public bool RControl_Previous = false; // 4,
        public bool AltLeft_Previous = false; // 5,
        public bool LAlt_Previous = false; // 5,
        public bool AltRight_Previous = false; // 6,
        public bool RAlt_Previous = false; // 6,
        public bool WinLeft_Previous = false; // 7,
        public bool LWin_Previous = false; // 7,
        public bool WinRight_Previous = false; // 8,
        public bool RWin_Previous = false; // 8,
        public bool Menu_Previous = false; // 9,
        public bool F1_Previous = false; // 10,
        public bool F2_Previous = false; // 11,
        public bool F3_Previous = false; // 12,
        public bool F4_Previous = false; // 13,
        public bool F5_Previous = false; // 14,
        public bool F6_Previous = false; // 15,
        public bool F7_Previous = false; // 16,
        public bool F8_Previous = false; // 17,
        public bool F9_Previous = false; // 18,
        public bool F10_Previous = false; // 19,
        public bool F11_Previous = false; // 20,
        public bool F12_Previous = false; // 21,
        public bool F13_Previous = false; // 22,
        public bool F14_Previous = false; // 23,
        public bool F15_Previous = false; // 24,
        public bool F16_Previous = false; // 25,
        public bool F17_Previous = false; // 26,
        public bool F18_Previous = false; // 27,
        public bool F19_Previous = false; // 28,
        public bool F20_Previous = false; // 29,
        public bool F21_Previous = false; // 30,
        public bool F22_Previous = false; // 31,
        public bool F23_Previous = false; // 32,
        public bool F24_Previous = false; // 33,
        public bool F25_Previous = false; // 34,
        public bool F26_Previous = false; // 35,
        public bool F27_Previous = false; // 36,
        public bool F28_Previous = false; // 37,
        public bool F29_Previous = false; // 38,
        public bool F30_Previous = false; // 39,
        public bool F31_Previous = false; // 40,
        public bool F32_Previous = false; // 41,
        public bool F33_Previous = false; // 42,
        public bool F34_Previous = false; // 43,
        public bool F35_Previous = false; // 44,
        public bool Up_Previous = false; // 45,
        public bool Down_Previous = false; // 46,
        public bool Left_Previous = false; // 47,
        public bool Right_Previous = false; // 48,
        public bool Enter_Previous = false; // 49,
        public bool Escape_Previous = false; // 50,
        public bool Space_Previous = false; // 51,
        public bool Tab_Previous = false; // 52,
        public bool BackSpace_Previous = false; // 53,
        public bool Back_Previous = false; // 53,
        public bool Insert_Previous = false; // 54,
        public bool Delete_Previous = false; // 55,
        public bool PageUp_Previous = false; // 56,
        public bool PageDown_Previous = false; // 57,
        public bool Home_Previous = false; // 58,
        public bool End_Previous = false; // 59,
        public bool CapsLock_Previous = false; // 60,
        public bool ScrollLock_Previous = false; // 61,
        public bool PrintScreen_Previous = false; // 62,
        public bool Pause_Previous = false; // 63,
        public bool NumLock_Previous = false; // 64,
        public bool Clear_Previous = false; // 65,
        public bool Sleep_Previous = false; // 66,
        public bool Keypad0_Previous = false; // 67,
        public bool Keypad1_Previous = false; // 68,
        public bool Keypad2_Previous = false; // 69,
        public bool Keypad3_Previous = false; // 70,
        public bool Keypad4_Previous = false; // 71,
        public bool Keypad5_Previous = false; // 72,
        public bool Keypad6_Previous = false; // 73,
        public bool Keypad7_Previous = false; // 74,
        public bool Keypad8_Previous = false; // 75,
        public bool Keypad9_Previous = false; // 76,
        public bool KeypadDivide_Previous = false; // 77,
        public bool KeypadMultiply_Previous = false; // 78,
        public bool KeypadSubtract_Previous = false; // 79,
        public bool KeypadMinus_Previous = false; // 79,
        public bool KeypadAdd_Previous = false; // 80,
        public bool KeypadPlus_Previous = false; // 80,
        public bool KeypadDecimal_Previous = false; // 81,
        public bool KeypadEnter_Previous = false; // 82,
        public bool A_Previous = false; // 83,
        public bool B_Previous = false; // 84,
        public bool C_Previous = false; // 85,
        public bool D_Previous = false; // 86,
        public bool E_Previous = false; // 87,
        public bool F_Previous = false; // 88,
        public bool G_Previous = false; // 89,
        public bool H_Previous = false; // 90,
        public bool I_Previous = false; // 91,
        public bool J_Previous = false; // 92,
        public bool K_Previous = false; // 93,
        public bool L_Previous = false; // 94,
        public bool M_Previous = false; // 95,
        public bool N_Previous = false; // 96,
        public bool O_Previous = false; // 97,
        public bool P_Previous = false; // 98,
        public bool Q_Previous = false; // 99,
        public bool R_Previous = false; // 100,
        public bool S_Previous = false; // 101,
        public bool T_Previous = false; // 102,
        public bool U_Previous = false; // 103,
        public bool V_Previous = false; // 104,
        public bool W_Previous = false; // 105,
        public bool X_Previous = false; // 106,
        public bool Y_Previous = false; // 107,
        public bool Z_Previous = false; // 108,
        public bool Number0_Previous = false; // 109,
        public bool Number1_Previous = false; // 110,
        public bool Number2_Previous = false; // 111,
        public bool Number3_Previous = false; // 112,
        public bool Number4_Previous = false; // 113,
        public bool Number5_Previous = false; // 114,
        public bool Number6_Previous = false; // 115,
        public bool Number7_Previous = false; // 116,
        public bool Number8_Previous = false; // 117,
        public bool Number9_Previous = false; // 118,
        public bool Tilde_Previous = false; // 119,
        public bool Minus_Previous = false; // 120,
        public bool Plus_Previous = false; // 121,
        public bool BracketLeft_Previous = false; // 122,
        public bool LBracket_Previous = false; // 122,
        public bool BracketRight_Previous = false; // 123,
        public bool RBracket_Previous = false; // 123,
        public bool Semicolon_Previous = false; // 124,
        public bool Quote_Previous = false; // 125,
        public bool Comma_Previous = false; // 126,
        public bool Period_Previous = false; // 127,
        public bool Slash_Previous = false; // 128,
        public bool BackSlash_Previous = false; // 129,
        public bool LastKey_Previous = false; // 130

        public bool GUI_PerformanceCurves   = true;
        public bool GUI_ChatWindow          = false;

        public virtual int GetMouseWheel()
        {
            return 0;
        }
        //#if _WINDOWS_
        //        public KeyboardDevice keyboard;
        //        public MouseDevice mouse;
        //#endif

        public PlayerControllerInput(SceneGame scene //,
//#if _WINDOWS_
//            KeyboardDevice keyboard, MouseDevice mouse
//#endif
            )
        {
            Parent = scene;
        }

        public void Reset(SceneGame scene)
        {
            
        }

        public virtual void update()
        {
//#if _WINDOWS_
            
//            //// Calculates how far the mouse has moved since the last call to this method
//            //Point center = new Point(
//            //    (Game.Instance.Bounds.Left + Game.Instance.Bounds.Right) / 2,
//            //    (Game.Instance.Bounds.Top + Game.Instance.Bounds.Bottom) / 2);

//            //Point mouse_current = Game.Instance.PointToScreen(new Point(Game.Instance.Mouse.X, Game.Instance.Mouse.Y));
//            //Cursor.Position = center;

//            //Point mouse_delta = new Point(
//            //    mouse_current.X - center.X,
//            //    -mouse_current.Y + center.Y);

//            //MouseMove = mouse_delta;
//            ////return mouse_delta;
//#endif
        }

        public virtual void ThirdPersonCursorLock(bool @lock) { }
        public virtual void FirstPersonCursorLock(bool enable) { }

        public int MouseX { get; set; }
        public int MouseY { get; set; }

    }
}
