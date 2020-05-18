

using NthDimension;
using NthDimension.Forms.Events;
using NthDimension.Rendering;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Scenegraph;
using NthDimension.Rasterizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINFORMS
//using System.Windows.Forms
#endif

namespace NthStudio
{
    public class GameInput : ApplicationUserInput
    {
        public delegate void KeyDownEvent(object sender, OpenTK.Input.KeyboardKeyEventArgs e);
        public event KeyDownEvent OnKeyDown;

        public OpenTK.Input.KeyboardState Keyboard
        {
            get { return OpenTK.Input.Keyboard.GetState(); }
        }
        public static OpenTK.Input.MouseState MouseState
        {
            get { return OpenTK.Input.Mouse.GetCursorState(); }
        }

        private Point mouseLock = new Point();
        private bool m_cursorVisible = true;

        public int WheelValue = 0;
        public float WheelValuePrecise = 0f;
        public int WheelDelta = 0;
        public float WheelDeltaPrecise = 0f;

        public bool CursorVisible
        {
            get { return m_cursorVisible; }
            set
            {
                if (m_cursorVisible != value)
                {
                    m_cursorVisible = value;
#if WINFORMS
                    //((StudioWindow)StudioWindow.Instance).SetCursorVisible(m_cursorVisible);
#endif
                }
            }
        }

        public bool CursorOverAvatarButton = false;

        public GameInput(SceneGame scene) : base(scene)
        {
            //_keyboard.KeyRepeat = true;
        }


        public override void FirstPersonCursorLock(bool enable)
        {
            if (enable)
            {
                mouseLock = new Point(
                    (ApplicationBase.Instance.Bounds.Left + ApplicationBase.Instance.Bounds.Right) / 2,
                    (ApplicationBase.Instance.Bounds.Top + ApplicationBase.Instance.Bounds.Bottom) / 2);
                this.CursorLock = true;
                update();
            }
            else
            {
                this.CursorLock = false;
                update();
            }
        }
        public override void ThirdPersonCursorLock(bool enable)
        {
            if (enable)
            {
                mouseLock = ApplicationBase.Instance.PointToScreen(new Point(((StudioWindow)StudioWindow.Instance).MousePosition.X,
                                                                  ((StudioWindow)StudioWindow.Instance).MousePosition.Y));
                this.CursorLock = true;
                update();
            }
            else
            {
                this.CursorLock = false;
                update();
            }
        }



        public override void update()
        {
            OpenTK.Input.MouseState _mouse = OpenTK.Input.Mouse.GetCursorState();
            OpenTK.Input.KeyboardState _keyboard = OpenTK.Input.Keyboard.GetState();

#region Cursor Lock/Unlock
            if (CursorLock)
            {
                Point mouse_current = ApplicationBase.Instance.PointToScreen(new Point(MouseX, MouseY));

#if WINFORMS
                System.Windows.Forms.Cursor.Position = mouseLock;
#else
                OpenTK.Input.Mouse.SetPosition(mouseLock.X, mouseLock.Y);
#endif

                Point mouse_delta = Settings.Instance.game.invertMouseY
                    ? new Point(
                        mouse_current.X - mouseLock.X,
                        mouse_current.Y - mouseLock.Y)
                    : new Point(
                        mouse_current.X - mouseLock.X,
                        -mouse_current.Y + mouseLock.Y);

                MouseDelta = mouse_delta;

                CursorVisible = false;
            }
            else
            {
                CursorVisible = true;
            }
#endregion Cursor Lock/Unlock

#region Keyboard Keys
            if (_mouse[OpenTK.Input.MouseButton.Left])
                FIRE = true;
            else
                FIRE = false;

            if (_keyboard[OpenTK.Input.Key.W])
                MOVEFORWARD = true;
            else
                MOVEFORWARD = false;

            if (_keyboard[OpenTK.Input.Key.S])
                MOVEBACKWARD = true;
            else
                MOVEBACKWARD = false;

            if (_keyboard[OpenTK.Input.Key.A])
                STRAFELEFT = true;
            else
                STRAFELEFT = false;

            if (_keyboard[OpenTK.Input.Key.D])
                STRAFERIGHT = true;
            else
                STRAFERIGHT = false;

            if (_keyboard[OpenTK.Input.Key.Space])
                JUMP = true;
            else
                JUMP = false;

            if (_keyboard[OpenTK.Input.Key.RControl] || _keyboard[OpenTK.Input.Key.LControl])
                CROUCH = true;
            else
                CROUCH = false;

            if (_keyboard[OpenTK.Input.Key.E])
                INTERACT = true;
            else
                INTERACT = false;

            if (_keyboard[OpenTK.Input.Key.Tab])
                ROTATE = true;
            else
                ROTATE = false;

            if (_keyboard[OpenTK.Input.Key.F1] && !(_keyboard[OpenTK.Input.Key.ControlLeft] || _keyboard[OpenTK.Input.Key.LControl]))
                TOOL01 = true;
            else
                TOOL01 = false;

            if (_keyboard[OpenTK.Input.Key.F2])
                TOOL02 = true;
            else
                TOOL02 = false;

            if (_keyboard[OpenTK.Input.Key.F3])
                TOOL03 = true;
            else
                TOOL03 = false;

            if (_keyboard[OpenTK.Input.Key.F4])
                TOOL04 = true;
            else
                TOOL04 = false;

            if (_keyboard[OpenTK.Input.Key.F5])
                TOOL05 = true;
            else
                TOOL05 = false;


            if (_keyboard[OpenTK.Input.Key.F8])
                CHANGECAMERA = true;
            else
                CHANGECAMERA = false;

            if (_keyboard[OpenTK.Input.Key.F9])
                F9 = true;
            else
                F9 = false;

            if (_keyboard[OpenTK.Input.Key.Home])
                Home = true;
            else
                Home = false;

            if ((_keyboard[OpenTK.Input.Key.LControl] || _keyboard[OpenTK.Input.Key.RControl]) &&
                (!_keyboard[OpenTK.Input.Key.LShift] && !_keyboard[OpenTK.Input.Key.RShift]) &&
                _keyboard[OpenTK.Input.Key.L])
                    {
                new Gui.FboViewer(@"Pssm 2/4 (near far)", StudioWindow.Instance.Scene.SunFrameBufferFar1.DepthTexture, true)
                    .Show(((StudioWindow)StudioWindow.Instance).Screen2D);
            }

            if ((_keyboard[OpenTK.Input.Key.LControl] || _keyboard[OpenTK.Input.Key.RControl]) &&
                (!_keyboard[OpenTK.Input.Key.LShift] && !_keyboard[OpenTK.Input.Key.RShift]) &&
                _keyboard[OpenTK.Input.Key.L])
            {
                new Gui.FboViewer(@"Pssm 1/4 (near)", StudioWindow.Instance.Scene.SunFrameBufferNear.DepthTexture, true)
                    .Show(((StudioWindow)StudioWindow.Instance).Screen2D);
            }


            #endregion

            #region Avatar Selection

            #endregion

        }

        public override int GetMouseWheel()
        {
            return WheelValue;
        }

        public void KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs args)
        {
            if (null != OnKeyDown)
            {               
                OnKeyDown(sender, args);            
            }
        }

        public void MouseDown(object sender, MouseButtonEventArgs args)
        {
            if (args.Button == MouseButton.Left)
                MouseButtonLeft = true;
            if (args.Button == MouseButton.Right)
                MouseButtonRight = true;
        }

        public void MouseUp(object sender, MouseButtonEventArgs args)
        {
            if (args.Button == MouseButton.Left)
                MouseButtonLeft = false;
            if (args.Button == MouseButton.Right)
                MouseButtonRight = false;
        }

        public void MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            MouseX = e.X;
            MouseY = e.Y;
        }
    }
}
