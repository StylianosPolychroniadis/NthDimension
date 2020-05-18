using NthDimension;
using NthDimension.Algebra;
using NthDimension.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio
{
    public static class Extensions
    {
        public static NthDimension.Rasterizer.NanoVG.NVGcolor ToNVGColor(this System.Drawing.Color color)
        {
            NthDimension.Rasterizer.NanoVG.NVGcolor ret;
            ret.r = color.R / 255f;
            ret.g = color.G / 255f;
            ret.b = color.B / 255f;
            ret.a = color.A / 255f;
            return ret;
        }

        public static MouseButton ToNth(this OpenTK.Input.MouseButton src)
        {
            MouseButton ret = new MouseButton();
            ret = MouseButton.None;

            switch (src)
            {
                case OpenTK.Input.MouseButton.Left:
                    ret = MouseButton.Left;
                    break;
                case OpenTK.Input.MouseButton.Right:
                    ret = MouseButton.Right;
                    break;
                case OpenTK.Input.MouseButton.Middle:
                    ret = MouseButton.Middle;
                    break;
            }
            return ret;
        }

        public static Keys ToNth(this OpenTK.Input.Key src)
        {
            Keys ret = new Keys();

            switch (src)
            {
                case OpenTK.Input.Key.A:
                    ret = Keys.A;
                    break;
                case (OpenTK.Input.Key.AltLeft | OpenTK.Input.Key.LAlt):
                    ret = Keys.Alt;
                    break;
                case (OpenTK.Input.Key.AltRight | OpenTK.Input.Key.RAlt):
                    ret = Keys.Alt;
                    break;
                case OpenTK.Input.Key.B:
                    ret = Keys.B;
                    break;
                case OpenTK.Input.Key.BackSlash:
                    ret = Keys.OemBackslash;
                    break;
                //case OpenTK.Input.Key.Back:
                case OpenTK.Input.Key.BackSpace:
                    ret = Keys.Back;
                    break;
                case (OpenTK.Input.Key.BracketLeft | OpenTK.Input.Key.LBracket):
                    ret = Keys.OemOpenBrackets;
                    break;
                case (OpenTK.Input.Key.BracketRight | OpenTK.Input.Key.RBracket):
                    ret = Keys.OemCloseBrackets;
                    break;
                case OpenTK.Input.Key.C:
                    ret = Keys.C;
                    break;
                case OpenTK.Input.Key.CapsLock:
                    ret = Keys.CapsLock;
                    break;
                case OpenTK.Input.Key.Clear:
                    ret = Keys.Clear;
                    break;
                case OpenTK.Input.Key.Comma:
                    ret = Keys.Oemcomma;
                    break;
                case (OpenTK.Input.Key.ControlLeft | OpenTK.Input.Key.LControl):
                    ret = Keys.ControlKey;
                    break;
                case (OpenTK.Input.Key.ControlRight | OpenTK.Input.Key.RControl):
                    ret = Keys.ControlKey;
                    break;
                case OpenTK.Input.Key.D:
                    ret = Keys.D;
                    break;
                case OpenTK.Input.Key.Delete:
                    ret = Keys.Delete;
                    break;
                case OpenTK.Input.Key.Down:
                    ret = Keys.Down;
                    break;
                case OpenTK.Input.Key.E:
                    ret = Keys.E;
                    break;
                case OpenTK.Input.Key.End:
                    ret = Keys.End;
                    break;
                case OpenTK.Input.Key.Enter:
                    ret = Keys.Enter;
                    break;
                case OpenTK.Input.Key.Escape:
                    ret = Keys.Escape;
                    break;
                case OpenTK.Input.Key.F:
                    ret = Keys.F;
                    break;
                case OpenTK.Input.Key.F1:
                    ret = Keys.F1;
                    break;
                case OpenTK.Input.Key.F2:
                    ret = Keys.F2;
                    break;
                case OpenTK.Input.Key.F3:
                    ret = Keys.F3;
                    break;
                case OpenTK.Input.Key.F4:
                    ret = Keys.F4;
                    break;
                case OpenTK.Input.Key.F5:
                    ret = Keys.F5;
                    break;
                case OpenTK.Input.Key.F6:
                    ret = Keys.F6;
                    break;
                case OpenTK.Input.Key.F7:
                    ret = Keys.F7;
                    break;
                case OpenTK.Input.Key.F8:
                    ret = Keys.F8;
                    break;
                case OpenTK.Input.Key.F9:
                    ret = Keys.F9;
                    break;
                case OpenTK.Input.Key.F10:
                    ret = Keys.F10;
                    break;
                case OpenTK.Input.Key.F11:
                    ret = Keys.F11;
                    break;
                case OpenTK.Input.Key.F12:
                    ret = Keys.F12;
                    break;
                case OpenTK.Input.Key.F13:
                    ret = Keys.F13;
                    break;
                case OpenTK.Input.Key.F14:
                    ret = Keys.F14;
                    break;
                case OpenTK.Input.Key.F15:
                    ret = Keys.F15;
                    break;
                case OpenTK.Input.Key.F16:
                    ret = Keys.F16;
                    break;
                case OpenTK.Input.Key.F17:
                    ret = Keys.F17;
                    break;
                case OpenTK.Input.Key.F18:
                    ret = Keys.F18;
                    break;
                case OpenTK.Input.Key.F19:
                    ret = Keys.F19;
                    break;
                case OpenTK.Input.Key.G:
                    ret = Keys.G;
                    break;
                case OpenTK.Input.Key.H:
                    ret = Keys.H;
                    break;
                case OpenTK.Input.Key.Home:
                    ret = Keys.Home;
                    break;
                case OpenTK.Input.Key.I:
                    ret = Keys.I;
                    break;
                case OpenTK.Input.Key.Insert:
                    ret = Keys.Insert;
                    break;
                case OpenTK.Input.Key.J:
                    ret = Keys.J;
                    break;
                case OpenTK.Input.Key.K:
                    ret = Keys.K;
                    break;
                case OpenTK.Input.Key.Keypad0:
                    ret = Keys.NumPad0;
                    break;
                case OpenTK.Input.Key.Keypad1:
                    ret = Keys.NumPad1;
                    break;
                case OpenTK.Input.Key.Keypad2:
                    ret = Keys.NumPad2;
                    break;
                case OpenTK.Input.Key.Keypad3:
                    ret = Keys.NumPad3;
                    break;
                case OpenTK.Input.Key.Keypad4:
                    ret = Keys.NumPad4;
                    break;
                case OpenTK.Input.Key.Keypad5:
                    ret = Keys.NumPad5;
                    break;
                case OpenTK.Input.Key.Keypad6:
                    ret = Keys.NumPad6;
                    break;
                case OpenTK.Input.Key.Keypad7:
                    ret = Keys.NumPad7;
                    break;
                case OpenTK.Input.Key.Keypad8:
                    ret = Keys.NumPad8;
                    break;
                case OpenTK.Input.Key.Keypad9:
                    ret = Keys.NumPad9;
                    break;
                case (OpenTK.Input.Key.KeypadAdd | OpenTK.Input.Key.KeypadPlus):
                    ret = Keys.Add;
                    break;
                case (OpenTK.Input.Key.KeypadDecimal | OpenTK.Input.Key.KeypadPeriod):
                    ret = Keys.Separator;
                    break;
                case OpenTK.Input.Key.KeypadDivide:
                    ret = Keys.Divide;
                    break;
                case OpenTK.Input.Key.KeypadEnter:
                    ret = Keys.Enter;
                    break;
                case OpenTK.Input.Key.KeypadMinus:
                    ret = Keys.OemMinus;
                    break;
                case OpenTK.Input.Key.KeypadMultiply:
                    ret = Keys.Multiply;
                    break;
                case OpenTK.Input.Key.L:
                    ret = Keys.L;
                    break;
                //case OpenTK.Input.Key.LastKey:
                    //ret = Keys;
                    //break;
                case OpenTK.Input.Key.Left:
                    ret = Keys.Left;
                    break;
                case OpenTK.Input.Key.M:
                    ret = Keys.M;
                    break;
                case OpenTK.Input.Key.Menu:
                    ret = Keys.Menu;
                    break;
                case OpenTK.Input.Key.Minus:
                    ret = Keys.Subtract;
                    break;
                case OpenTK.Input.Key.N:
                    ret = Keys.N;
                    break;
                case OpenTK.Input.Key.NonUSBackSlash:
                    ret = Keys.OemBackslash;
                    break;
                case OpenTK.Input.Key.Number0:
                    ret = Keys.D0;
                    break;
                case OpenTK.Input.Key.Number1:
                    ret = Keys.D1;
                    break;
                case OpenTK.Input.Key.Number2:
                    ret = Keys.D2;
                    break;
                case OpenTK.Input.Key.Number3:
                    ret = Keys.D3;
                    break;
                case OpenTK.Input.Key.Number4:
                    ret = Keys.D4;
                    break;
                case OpenTK.Input.Key.Number5:
                    ret = Keys.D5;
                    break;
                case OpenTK.Input.Key.Number6:
                    ret = Keys.D6;
                    break;
                case OpenTK.Input.Key.Number7:
                    ret = Keys.D7;
                    break;
                case OpenTK.Input.Key.Number8:
                    ret = Keys.D8;
                    break;
                case OpenTK.Input.Key.Number9:
                    ret = Keys.D9;
                    break;
                case OpenTK.Input.Key.NumLock:
                    ret = Keys.NumLock;
                    break;
                case OpenTK.Input.Key.O:
                    ret = Keys.O;
                    break;
                case OpenTK.Input.Key.P:
                    ret = Keys.P;
                    break;
                case OpenTK.Input.Key.PageDown:
                    ret = Keys.PageDown;
                    break;
                case OpenTK.Input.Key.PageUp:
                    ret = Keys.PageUp;
                    break;
                case OpenTK.Input.Key.Pause:
                    ret = Keys.Pause;
                    break;
                case OpenTK.Input.Key.Period:
                    ret = Keys.OemPeriod;
                    break;
                case OpenTK.Input.Key.Plus:
                    ret = Keys.Oemplus;
                    break;
                case OpenTK.Input.Key.PrintScreen:
                    ret = Keys.PrintScreen;
                    break;
                case OpenTK.Input.Key.Q:
                    ret = Keys.Q;
                    break;
                case OpenTK.Input.Key.Quote:
                    ret = Keys.OemQuotes;
                    break;
                case OpenTK.Input.Key.R:
                    ret = Keys.R;
                    break;
                case OpenTK.Input.Key.Right:
                    ret = Keys.Right;
                    break;
                case OpenTK.Input.Key.S:
                    ret = Keys.S;
                    break;
                case OpenTK.Input.Key.ScrollLock:
                    ret = Keys.Scroll;
                    break;
                case OpenTK.Input.Key.Semicolon:
                    ret = Keys.OemSemicolon;
                    break;
                //case OpenTK.Input.Key.LShift:
                case OpenTK.Input.Key.ShiftLeft:
                    ret = Keys.LShiftKey;
                    break;
                // case OpenTK.Input.Key.RShift:
                case OpenTK.Input.Key.ShiftRight:
                    ret = Keys.RShiftKey;
                    break;
                case OpenTK.Input.Key.Slash:
                    ret = Keys.OemPipe;
                    break;
                case OpenTK.Input.Key.Sleep:
                    ret = Keys.Sleep;
                    break;
                case OpenTK.Input.Key.Space:
                    ret = Keys.Space;
                    break;
                case OpenTK.Input.Key.T:
                    ret = Keys.T;
                    break;
                case OpenTK.Input.Key.Tab:
                    ret = Keys.Tab;
                    break;
                case (OpenTK.Input.Key.Tilde | OpenTK.Input.Key.Grave):
                    ret = Keys.Oemtilde;
                    break;
                case OpenTK.Input.Key.U:
                    ret = Keys.U;
                    break;
                case OpenTK.Input.Key.Up:
                    ret = Keys.Up;
                    break;
                case OpenTK.Input.Key.V:
                    ret = Keys.V;
                    break;
                case OpenTK.Input.Key.W:
                    ret = Keys.W;
                    break;
                //case OpenTK.Input.Key.LWin:
                case OpenTK.Input.Key.WinLeft:
                    ret = Keys.LWin;
                    break;
                //case OpenTK.Input.Key.RWin:
                case OpenTK.Input.Key.WinRight:
                    ret = Keys.RWin;
                    break;
                case OpenTK.Input.Key.X:
                    ret = Keys.X;
                    break;
                case OpenTK.Input.Key.Y:
                    ret = Keys.Y;
                    break;
                case OpenTK.Input.Key.Z:
                    ret = Keys.Z;
                    break;

            }

            return ret;
        }
    }

    #region Algebra
    public class structV4
    {
        public Vector4 right { get; set; }
        public Vector4 up { get; set; }
        public Vector4 dir { get; set; }
        public Vector4 position { get; set; }

        public structV4() { }

        public structV4(Vector4 vRight, Vector4 vUp, Vector4 vDirection, Vector4 vPosition)
        {
            right = vRight;
            up = vUp;
            dir = vDirection;
            position = vPosition;
        }
    }
    public static class Matrix4_Extensions
    {
        public static structV4 GetVector(this Matrix4 m)
        {
            return new structV4(new Vector4(m.M11, m.M12, m.M13, m.M14),     // right
                                new Vector4(m.M21, m.M22, m.M23, m.M24),     // up
                                new Vector4(m.M31, m.M32, m.M33, m.M34),     // direction
                                new Vector4(m.M41, m.M42, m.M43, m.M44));    // position
            //m[0][0] = right.x;
            //m[0][1] = right.y;
            //m[0][2] = right.z;
            //m[0][3] = 0;

            //m[1][0] = up.x;
            //m[1][1] = up.y;
            //m[1][2] = up.z;
            //m[1][3] = 0;

            //m[2][0] = dir.x;
            //m[2][1] = dir.y;
            //m[2][2] = dir.z;
            //m[2][3] = 0;

            //m[3][0] = enuDock.x;
            //m[3][1] = enuDock.y;
            //m[3][2] = enuDock.z;
            //m[3][3] = 1;
        }
        public static Matrix4 FromArray(this Matrix4 m, float[] array)
        {
            m.M11 = array[0];
            m.M12 = array[1];
            m.M13 = array[2];
            m.M14 = array[3];
            m.M21 = array[4];
            m.M22 = array[5];
            m.M23 = array[6];
            m.M24 = array[7];
            m.M31 = array[8];
            m.M32 = array[9];
            m.M33 = array[10];
            m.M34 = array[11];
            m.M41 = array[12];
            m.M42 = array[13];
            m.M43 = array[14];
            m.M44 = array[15];

            return m;
        }
        public static Matrix4 FromTable(this Matrix4 m, float[,] array)
        {
            //Matrix4 mout = new Matrix4();
            m.M11 = array[0, 0];
            m.M12 = array[0, 1];
            m.M13 = array[0, 2];
            m.M14 = array[0, 3];
            m.M21 = array[1, 0];
            m.M22 = array[1, 1];
            m.M23 = array[1, 2];
            m.M24 = array[1, 3];
            m.M31 = array[2, 0];
            m.M32 = array[2, 1];
            m.M33 = array[2, 2];
            m.M34 = array[2, 3];
            m.M41 = array[3, 0];
            m.M42 = array[3, 1];
            m.M43 = array[3, 2];
            m.M44 = array[3, 3];

            return m;
        }
        public static float GetDeterminant(this Matrix4 m)
        {
            return m.M11 * m.M22 * m.M33 + m.M12 * m.M23 * m.M31 + m.M13 * m.M21 * m.M32 -
                     m.M13 * m.M22 * m.M31 - m.M12 * m.M21 * m.M33 - m.M11 * m.M23 * m.M32;
        }
        public static Vector3 GetTranslation(this Matrix4 m)
        {
            return new Vector3(m.M41, m.M42, m.M43);
        }
        public static void Identity(this Matrix4 m)
        {
            m.Set(1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1);
        }
        /// <summary>
        /// [OBSOLETE:: Drop, used only in ApplyTransformation in GIZMOs
        /// </summary>
        /// <param name="m"></param>
        /// <param name="srcMatrix"></param>
        /// <param name="affine"></param>
        /// <returns></returns>
        public static Matrix4 Inverse(this Matrix4 m, Matrix4 srcMatrix, bool affine = false)
        {
            float det = 0; // Not really used
            Matrix4 mout = new Matrix4();

            if (affine)
            {
                Matrix4 tmp = new Matrix4();
                det = srcMatrix.GetDeterminant();
                float s = 1 / det;
                tmp.M11 = (srcMatrix.M22 * srcMatrix.M33 - srcMatrix.M23 * srcMatrix.M32) * s;
                tmp.M12 = (srcMatrix.M32 * srcMatrix.M13 - srcMatrix.M33 * srcMatrix.M12) * s;
                tmp.M13 = (srcMatrix.M12 * srcMatrix.M23 - srcMatrix.M13 * srcMatrix.M22) * s;
                tmp.M21 = (srcMatrix.M23 * srcMatrix.M31 - srcMatrix.M21 * srcMatrix.M33) * s;
                tmp.M22 = (srcMatrix.M33 * srcMatrix.M11 - srcMatrix.M31 * srcMatrix.M13) * s;
                tmp.M23 = (srcMatrix.M13 * srcMatrix.M21 - srcMatrix.M11 * srcMatrix.M23) * s;
                tmp.M31 = (srcMatrix.M21 * srcMatrix.M32 - srcMatrix.M22 * srcMatrix.M31) * s;
                tmp.M32 = (srcMatrix.M31 * srcMatrix.M12 - srcMatrix.M32 * srcMatrix.M11) * s;
                tmp.M33 = (srcMatrix.M11 * srcMatrix.M22 - srcMatrix.M12 * srcMatrix.M21) * s;
                tmp.M41 = -(tmp.M11 * srcMatrix.M41 + tmp.M21 * srcMatrix.M42 + tmp.M31 * srcMatrix.M43);
                tmp.M42 = -(tmp.M12 * srcMatrix.M41 + tmp.M22 * srcMatrix.M42 + tmp.M32 * srcMatrix.M43);
                tmp.M43 = -(tmp.M13 * srcMatrix.M41 + tmp.M23 * srcMatrix.M42 + tmp.M33 * srcMatrix.M43);

                mout = tmp;
            }
            else
            {
                // transpose matrix
                float[] src = new float[16];
                float[] srcm = new float[16];

                srcm = srcMatrix.ToArray();

                for (int i = 0; i < 4; ++i)
                {
                    src[i] = srcm[i * 4];
                    src[i + 4] = srcm[i * 4 + 1];
                    src[i + 8] = srcm[i * 4 + 2];
                    src[i + 12] = srcm[i * 4 + 3];
                }

                // calculate pairs for first 8 elements (cofactors)
                float[] tmp = new float[12]; // temp array for pairs
                tmp[0] = src[10] * src[15];
                tmp[1] = src[11] * src[14];
                tmp[2] = src[9] * src[15];
                tmp[3] = src[11] * src[13];
                tmp[4] = src[9] * src[14];
                tmp[5] = src[10] * src[13];
                tmp[6] = src[8] * src[15];
                tmp[7] = src[11] * src[12];
                tmp[8] = src[8] * src[14];
                tmp[9] = src[10] * src[12];
                tmp[10] = src[8] * src[13];
                tmp[11] = src[9] * src[12];

                // calculate first 8 elements (cofactors)
                srcMatrix.M11 = (tmp[0] * src[5] + tmp[3] * src[6] + tmp[4] * src[7]) - (tmp[1] * src[5] + tmp[2] * src[6] + tmp[5] * src[7]);
                srcMatrix.M12 = (tmp[1] * src[4] + tmp[6] * src[6] + tmp[9] * src[7]) - (tmp[0] * src[4] + tmp[7] * src[6] + tmp[8] * src[7]);
                srcMatrix.M13 = (tmp[2] * src[4] + tmp[7] * src[5] + tmp[10] * src[7]) - (tmp[3] * src[4] + tmp[6] * src[5] + tmp[11] * src[7]);
                srcMatrix.M14 = (tmp[5] * src[4] + tmp[8] * src[5] + tmp[11] * src[6]) - (tmp[4] * src[4] + tmp[9] * src[5] + tmp[10] * src[6]);
                srcMatrix.M21 = (tmp[1] * src[1] + tmp[2] * src[2] + tmp[5] * src[3]) - (tmp[0] * src[1] + tmp[3] * src[2] + tmp[4] * src[3]);
                srcMatrix.M22 = (tmp[0] * src[0] + tmp[7] * src[2] + tmp[8] * src[3]) - (tmp[1] * src[0] + tmp[6] * src[2] + tmp[9] * src[3]);
                srcMatrix.M23 = (tmp[3] * src[0] + tmp[6] * src[1] + tmp[11] * src[3]) - (tmp[2] * src[0] + tmp[7] * src[1] + tmp[10] * src[3]);
                srcMatrix.M24 = (tmp[4] * src[0] + tmp[9] * src[1] + tmp[10] * src[2]) - (tmp[5] * src[0] + tmp[8] * src[1] + tmp[11] * src[2]);

                // calculate pairs for second 8 elements (cofactors)
                tmp[0] = src[2] * src[7];
                tmp[1] = src[3] * src[6];
                tmp[2] = src[1] * src[7];
                tmp[3] = src[3] * src[5];
                tmp[4] = src[1] * src[6];
                tmp[5] = src[2] * src[5];
                tmp[6] = src[0] * src[7];
                tmp[7] = src[3] * src[4];
                tmp[8] = src[0] * src[6];
                tmp[9] = src[2] * src[4];
                tmp[10] = src[0] * src[5];
                tmp[11] = src[1] * src[4];

                // calculate second 8 elements (cofactors)
                srcMatrix.M31 = (tmp[0] * src[13] + tmp[3] * src[14] + tmp[4] * src[15]) - (tmp[1] * src[13] + tmp[2] * src[14] + tmp[5] * src[15]);
                srcMatrix.M32 = (tmp[1] * src[12] + tmp[6] * src[14] + tmp[9] * src[15]) - (tmp[0] * src[12] + tmp[7] * src[14] + tmp[8] * src[15]);
                srcMatrix.M33 = (tmp[2] * src[12] + tmp[7] * src[13] + tmp[10] * src[15]) - (tmp[3] * src[12] + tmp[6] * src[13] + tmp[11] * src[15]);
                srcMatrix.M34 = (tmp[5] * src[12] + tmp[8] * src[13] + tmp[11] * src[14]) - (tmp[4] * src[12] + tmp[9] * src[13] + tmp[10] * src[14]);
                srcMatrix.M41 = (tmp[2] * src[10] + tmp[5] * src[11] + tmp[1] * src[9]) - (tmp[4] * src[11] + tmp[0] * src[9] + tmp[3] * src[10]);
                srcMatrix.M42 = (tmp[8] * src[11] + tmp[0] * src[8] + tmp[7] * src[10]) - (tmp[6] * src[10] + tmp[9] * src[11] + tmp[1] * src[8]);
                srcMatrix.M43 = (tmp[6] * src[9] + tmp[11] * src[11] + tmp[3] * src[8]) - (tmp[10] * src[11] + tmp[2] * src[8] + tmp[7] * src[9]);
                srcMatrix.M44 = (tmp[10] * src[10] + tmp[4] * src[8] + tmp[9] * src[9]) - (tmp[8] * src[9] + tmp[11] * src[10] + tmp[5] * src[8]);

                // calculate determinant
                //float det = src[0]*m16[0]+src[1]*m16[1]+src[2]*m16[2]+src[3]*m16[3];
                det = src[0] * srcMatrix.M11 + src[1] * srcMatrix.M12 + src[2] * srcMatrix.M13 + src[3] * srcMatrix.M14;

                // calculate matrix inverse
                float invdet = 1 / det;
                for (int j = 0; j < 16; ++j)
                {
                    srcm[j] *= invdet;
                }
                mout = new Matrix4().FromArray(srcm);
            }

            m = mout;

            return mout;
        }

        /// <summary>
        /// For use with OpenGL (Left-Hand)
        /// </summary>
        /// <param name="m"></param>
        /// <param name="eye"></param>
        /// <param name="at"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Matrix4 LookAtLH(this Matrix4 m, Vector3 eye, Vector3 at, Vector3 up)
        {
            Vector3 X = new Vector3();
            Vector3 Y = new Vector3();
            Vector3 Z = new Vector3();
            Vector3 tmp = new Vector3();

            Z = at - eye;
            Z.Normalize();

            Y = up;
            Y.Normalize();

            tmp = Vector3.Cross(Y, Z);

            X = tmp;
            X.Normalize();

            tmp = Vector3.Cross(Z, X);
            Y = tmp;
            Y.Normalize();

            m.M11 = X.X;
            m.M12 = Y.X;
            m.M13 = Z.X;
            m.M14 = 0.0f;

            m.M21 = X.Y;
            m.M22 = Y.Y;
            m.M23 = Z.Y;
            m.M24 = 0.0f;

            m.M31 = X.Z;
            m.M32 = Y.Z;
            m.M33 = Z.Z;
            m.M34 = 0.0f;

            m.M41 = -X.Dot(eye);
            m.M42 = -Y.Dot(eye);
            m.M43 = -Z.Dot(eye);
            m.M44 = 1.0f;

            return m;
        }
        /// <summary>
        /// For use with DirectX (Right-Hand)
        /// </summary>
        /// <param name="m"></param>
        /// <param name="eye"></param>
        /// <param name="at"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Matrix4 LookAtRH(this Matrix4 m, Vector3 eye, Vector3 at, Vector3 up)
        {
            Vector3 X = new Vector3();
            Vector3 Y = new Vector3();
            Vector3 Z = new Vector3();
            Vector3 tmp = new Vector3();

            Z = eye - at;
            Z.Normalize();

            Y = up;
            Y.Normalize();

            tmp = Vector3.Cross(Y, Z);
            X = tmp;
            X.Normalize();

            tmp = Vector3.Cross(Z, X);
            Y = tmp;
            Y.Normalize();

            m.M11 = X.X;
            m.M12 = Y.X;
            m.M13 = Z.X;
            m.M14 = 0.0f;

            m.M21 = X.Y;
            m.M22 = Y.Y;
            m.M23 = Z.Y;
            m.M24 = 0.0f;

            m.M31 = X.Z;
            m.M32 = Y.Z;
            m.M33 = Z.Z;
            m.M34 = 0.0f;

            m.M41 = -X.Dot(eye);
            m.M42 = -Y.Dot(eye);
            m.M43 = -Z.Dot(eye);
            m.M44 = 1.0f;

            return m;
        }

        public static Matrix4 Mult(this Matrix4 m, float scalar)
        {
            Matrix4 mout = m;

            mout.M11 = mout.M11 * scalar;
            mout.M12 = mout.M12 * scalar;
            mout.M13 = mout.M13 * scalar;
            mout.M14 = mout.M14 * scalar;

            mout.M21 = mout.M21 * scalar;
            mout.M22 = mout.M22 * scalar;
            mout.M23 = mout.M23 * scalar;
            mout.M24 = mout.M24 * scalar;

            mout.M31 = mout.M31 * scalar;
            mout.M32 = mout.M32 * scalar;
            mout.M33 = mout.M33 * scalar;
            mout.M34 = mout.M34 * scalar;

            mout.M31 = mout.M41 * scalar;
            mout.M32 = mout.M42 * scalar;
            mout.M33 = mout.M43 * scalar;
            mout.M34 = mout.M44 * scalar;

            m = mout;
            return mout;
        }
        public static Matrix4 Multiply(this Matrix4 m, float scalar)
        {
            return Mult(m, scalar);
        }

        /// <summary>
        /// [Deprecated] Use Matrix4.CreateFromRotationAxis instead
        /// </summary>
        /// <param name="m"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Matrix4 RotationAxis(this Matrix4 m, Vector3 axis, float angle)
        {
            float length2 = axis.LengthSquared;
            if (length2 == 0)
            {
                m.Identity();
                return m;
            }

            Vector3 n = axis / (float)Math.Sqrt(length2);
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);
            float k = 1 - c;

            float xx = n.X * n.X * k + c;
            float yy = n.Y * n.Y * k + c;
            float zz = n.Z * n.Z * k + c;
            float xy = n.X * n.Y * k;
            float yz = n.Y * n.Z * k;
            float zx = n.Z * n.X * k;
            float xs = n.X * s;
            float ys = n.Y * s;
            float zs = n.Z * s;

            m.M11 = xx;
            m.M12 = xy + zs;
            m.M13 = zx - ys;
            m.M14 = 0;
            m.M21 = xy - zs;
            m.M22 = yy;
            m.M23 = yz + xs;
            m.M24 = 0;
            m.M31 = zx + ys;
            m.M32 = yz - xs;
            m.M33 = zz;
            m.M34 = 0;
            m.M41 = 0;
            m.M42 = 0;
            m.M43 = 0;
            m.M44 = 1;

            return m;
        }

        public static void Set(this Matrix4 m,
                          float m11, float m12, float m13, float m14,
                          float m21, float m22, float m23, float m24,
                          float m31, float m32, float m33, float m34,
                          float m41, float m42, float m43, float m44)
        {
            m.M11 = m11; m.M12 = m12; m.M13 = m13; m.M14 = m14;
            m.M21 = m21; m.M22 = m22; m.M23 = m23; m.M24 = m24;
            m.M31 = m31; m.M32 = m32; m.M33 = m33; m.M34 = m34;
            m.M41 = m41; m.M42 = m42; m.M43 = m43; m.M44 = m44;
        }

        public static void SetLine(this Matrix4 m, int line, Vector3 v)
        {
            float[,] arr = new float[4, 4];
            arr = m.ToTable();

            arr[line, 0] = v.X;
            arr[line, 1] = v.Y;
            arr[line, 2] = v.Z;
            arr[line, 3] = 0f;

            m.FromTable(arr);
        }
        public static void SetColumn(this Matrix4 m, int column, Vector3 v)
        {
            float[,] arr = new float[4, 4];
            arr = m.ToTable();

            arr[0, column] = v.X;
            arr[1, column] = v.Y;
            arr[2, column] = v.Z;
            arr[3, column] = 0f;

            m.FromTable(arr);
        }

        public static void TranslationClear(this Matrix4 m)
        {
            m.M41 = 0f;
            m.M42 = 0f;
            m.M43 = 0f;
        }

        public static float[] ToArray(this Matrix4 m)
        {
            float[] arr = new float[16];
            arr[0] = m.M11;
            arr[1] = m.M12;
            arr[2] = m.M13;
            arr[3] = m.M14;
            arr[4] = m.M21;
            arr[5] = m.M22;
            arr[6] = m.M23;
            arr[7] = m.M24;
            arr[8] = m.M31;
            arr[9] = m.M32;
            arr[10] = m.M33;
            arr[11] = m.M34;
            arr[12] = m.M41;
            arr[13] = m.M42;
            arr[14] = m.M43;
            arr[15] = m.M44;

            return arr;
        }
        public static float[,] ToTable(this Matrix4 m)
        {
            float[,] arr = new float[4, 4];
            arr[0, 0] = m.M11;
            arr[0, 1] = m.M12;
            arr[0, 2] = m.M13;
            arr[0, 3] = m.M14;
            arr[1, 0] = m.M21;
            arr[1, 1] = m.M22;
            arr[1, 2] = m.M23;
            arr[1, 3] = m.M24;
            arr[2, 0] = m.M31;
            arr[2, 1] = m.M32;
            arr[2, 2] = m.M33;
            arr[2, 3] = m.M34;
            arr[3, 0] = m.M41;
            arr[3, 1] = m.M42;
            arr[3, 2] = m.M43;
            arr[3, 3] = m.M44;

            return arr;
        }


        //public static void TransformaVector(ref Vector3 pV)
        //{
        //    float auxX, auxY, auxZ;

        //    float inverseW = 1.0f / (values[3] + values[7] + values[11] + values[15]);
        //    auxX = ((values[0] * pV.X) + (values[4] * pV.Y) + (values[8] * pV.Z) + values[12]) * inverseW;
        //    auxY = ((values[1] * pV.X) + (values[5] * pV.Y) + (values[9] * pV.Z) + values[13]) * inverseW;
        //    auxZ = ((values[2] * pV.X) + (values[6] * pV.Y) + (values[10] * pV.Z) + values[14]) * inverseW;
        //    float[] arrayMat = mat.ToArray();
        //    float inverseW = 1.0f / (arrayMat[3] + arrayMat[7] + arrayMat[11] + arrayMat[15]);
        //    auxX = ((arrayMat[0] * pV.X) + (arrayMat[4] * pV.Y) + (arrayMat[8] * pV.Z) + arrayMat[12]) * inverseW;
        //    auxY = ((arrayMat[1] * pV.X) + (arrayMat[5] * pV.Y) + (arrayMat[9] * pV.Z) + arrayMat[13]) * inverseW;
        //    auxZ = ((arrayMat[2] * pV.X) + (arrayMat[6] * pV.Y) + (arrayMat[10] * pV.Z) + arrayMat[14]) * inverseW;

        //    pV.X = auxX;
        //    pV.Y = auxY;
        //    pV.Z = auxZ;
        //}
        /// <summary>
        /// pV = this * pV
        /// </summary>
        /// <param name="pV"></param>
        public static void TransformVector(this Matrix4 mat, ref Vector3 pV)
        {
            /* See this implementation also
             * 
             * func (m *Matrix3) TransformVector3(v *Vector3) *Vector3 {
	            newVec := &Vector3{}
	            newVec[0] = v[0]*m[0] + v[1]*m[1] + v[2] + m[2]
	            newVec[1] = v[0]*m[3] + v[1]*m[4] + v[2] + m[5]
	            newVec[2] = v[0]*m[6] + v[1]*m[7] + v[2] + m[8]
	            return newVec 
             * }
             */


            float auxX, auxY, auxZ;
            float[] arrayMat = mat.ToArray();

            float inverseW = 1.0f / (arrayMat[3] + arrayMat[7] + arrayMat[11] + arrayMat[15]);
            auxX = ((arrayMat[0] * pV.X) + (arrayMat[4] * pV.Y) + (arrayMat[8] * pV.Z) + arrayMat[12]) * inverseW;
            auxY = ((arrayMat[1] * pV.X) + (arrayMat[5] * pV.Y) + (arrayMat[9] * pV.Z) + arrayMat[13]) * inverseW;
            auxZ = ((arrayMat[2] * pV.X) + (arrayMat[6] * pV.Y) + (arrayMat[10] * pV.Z) + arrayMat[14]) * inverseW;

            pV.X = auxX;
            pV.Y = auxY;
            pV.Z = auxZ;
        }
        /// <summary>
        /// [Deprecated] Use Matrix4.CreateTranslation instead
        /// </summary>
        /// <param name="m"></param>
        /// <param name="v"></param>
        public static void Translation(this Matrix4 m, Vector3 v)
        {
            Set(m, 1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                v.X, v.Y, v.Z, 1);
        }
        public static Vector3 Translation3(this Matrix4 m)
        {
            Vector3 tv = new Vector3(m.M41, m.M42, m.M43);
            return tv;
        }
        /// <summary>
        /// [Deprecated] Use Matrix4.Scale instead
        /// </summary>
        /// <param name="m"></param>
        /// <param name="sc"></param>
        public static void Scale(this Matrix4 m, Vector3 sc)
        {
            m.Set(sc.X, 0, 0, 0,
                  0, sc.Y, 0, 0,
                  0, 0, sc.Z, 0,
                  0, 0, 0, 1);
        }

        #region Unit Vectors
        ///// <summary>
        ///// <para>Get: Devuelve un nuevo Vector3 cuyas widgets equivalen al vector unitario ancho de la matriz.</para>
        ///// <para>Set: Establece la porción de la matriz que corresponde al vector unitario ancho, segun (ancho,alto,z) del vector.</para>
        ///// </summary>
        //public static Vector3 Unit3X(this Matrix4 m)
        //{
        //    return new Vector3(m.M11, m.M12, m.M13); 
        //}
        // <summary>
        // <para>Get: Devuelve un nuevo Vector4 cuyas widgets equivalen al vector unitario ancho de la matriz.</para>
        // <para>Set: Establece la porción de la matriz que corresponde al vector unitario ancho, segun (ancho,alto,z) del vector.</para>
        // </summary>
        //public Vector4 Unit4X
        //{
        //    get { return new Vector4(arrayMat[0], arrayMat[1], arrayMat[2], arrayMat[3]); }
        //    set { SetUnitX(value.x, value.y, value.z, value.w); }
        //}
        /// <summary>
        /// Establece la porción de la matriz que corresponde al vector unitario ancho, segun (ancho,alto,z).
        /// </summary>
        /// <param name="ancho"></param>
        /// <param name="alto"></param>
        /// <param name="z"></param>
        public static void SetUnitX(this Matrix4 m, float x, float y, float z)
        {
            m.M11 = x;
            m.M12 = y;
            m.M13 = z;
            m.M14 = 0f;
            //arrayMat[0] = x;
            //arrayMat[1] = y;
            //arrayMat[2] = z;
            //arrayMat[3] = 0;
        }
        /// <summary>
        /// Establece la porción de la matriz que corresponde al vector unitario ancho, segun (ancho,alto,z,w). 
        /// </summary>
        /// <param name="ancho"></param>
        /// <param name="alto"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public static void SetUnitX(this Matrix4 m, float x, float y, float z, float w)
        {
            //arrayMat[0] = x;
            //arrayMat[1] = y;
            //arrayMat[2] = z;
            //arrayMat[3] = w;
            m.M11 = x;
            m.M12 = y;
            m.M13 = z;
            m.M14 = w;
        }
        /// <summary>
        /// Establece las componetes (ancho,alto,z) del vector a los valores (ancho,alto,z) del vector unitario ancho de la matriz.
        /// </summary>
        /// <param name="uniX"></param>
        public static void GetUnitX(this Matrix4 m, ref Vector3 uniX)
        {
            uniX.X = m.M11;
            uniX.Y = m.M12;
            uniX.Z = m.M13;
        }
        /// <summary>
        ///  Establece las componetes (ancho,alto,z,w) del vector a los valores (ancho,alto,z,w) del vector unitario ancho de la matriz.
        /// </summary>
        /// <param name="uniX"></param>
        public static void GetUnitX(this Matrix4 m, ref Vector4 uniX)
        {
            //uniX.x = arrayMat[0];
            //uniX.y = arrayMat[1];
            //uniX.z = arrayMat[2];
            //uniX.w = arrayMat[3];
            uniX.X = m.M11;
            uniX.Y = m.M12;
            uniX.Z = m.M13;
            uniX.W = m.M14;
        }
        ///// <summary>
        ///// <para>Get: Devuelve un nuevo Vector3 cuyas widgets equivalen al vector unitario alto de la matriz.</para>
        ///// <para>Set: Establece la porción de la matriz que corresponde al vector unitario alto, segun (ancho,alto,z) del vector.</para>
        ///// </summary>
        //public Vector3 Unit3Y
        //{
        //    get { return new Vector3(arrayMat[4], arrayMat[5], arrayMat[6]); }
        //    set { SetUnitY(value.X, value.Y, value.Z); }
        //}
        ///// <summary>
        ///// <para>Get: Devuelve un nuevo Vector4 cuyas widgets equivalen al vector unitario alto de la matriz.</para>
        ///// <para>Set: Establece la porción de la matriz que corresponde al vector unitario alto, segun (ancho,alto,z) del vector.</para>
        ///// </summary>
        //public Vector4 Unit4Y
        //{
        //    get { return new Vector4(arrayMat[4], arrayMat[5], arrayMat[6], arrayMat[7]); }
        //    set { SetUnitY(value.x, value.y, value.z, value.w); }
        //}
        /// <summary>
        ///  Establece la porción de la matriz que corresponde al vector unitario alto, segun (ancho,alto,z). 
        /// </summary>
        /// <param name="ancho"></param>
        /// <param name="alto"></param>
        /// <param name="z"></param>
        public static void SetUnitY(this Matrix4 m, float x, float y, float z)
        {
            //arrayMat[4] = x;
            //arrayMat[5] = y;
            //arrayMat[6] = z;
            //arrayMat[7] = 0;
            m.M21 = x;
            m.M21 = y;
            m.M21 = z;
            m.M21 = 0f;
        }
        /// <summary>
        ///  Establece la porción de la matriz que corresponde al vector unitario alto, segun (ancho,alto,z,w). 
        /// </summary>
        /// <param name="ancho"></param>
        /// <param name="alto"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public static void SetUnitY(this Matrix4 m, float x, float y, float z, float w)
        {
            m.M21 = x;
            m.M21 = y;
            m.M21 = z;
            m.M21 = w;
        }
        /// <summary>
        ///   Establece las componetes (ancho,alto,z) del vector a los valores (ancho,alto,z) del vector unitario alto de la matriz.
        /// </summary>
        /// <param name="uniY"></param>
        public static void GetUnitY(this Matrix4 m, ref Vector3 uniY)
        {
            //uniY.X = arrayMat[4];
            //uniY.Y = arrayMat[5];
            //uniY.Z = arrayMat[6];
            uniY.X = m.M21;
            uniY.Y = m.M22;
            uniY.Z = m.M23;
        }
        /// <summary>
        ///   Establece las componetes (ancho,alto,z,w) del vector a los valores (ancho,alto,z,w) del vector unitario alto de la matriz.
        /// </summary>
        /// <param name="uniY"></param>
        public static void GetUnitY(this Matrix4 m, ref Vector4 uniY)
        {
            //uniY.x = arrayMat[4];
            //uniY.y = arrayMat[5];
            //uniY.z = arrayMat[6];
            //uniY.w = arrayMat[7];
            uniY.X = m.M21;
            uniY.Y = m.M22;
            uniY.Z = m.M23;
            uniY.W = m.M24;
        }
        ///// <summary>
        ///// <para>Get: Devuelve un nuevo Vector3 cuyas widgets equivalen al vector unitario z de la matriz.</para>
        ///// <para>Set: Establece la porción de la matriz que corresponde al vector unitario z, segun (ancho,alto,z) del vector.</para>
        ///// </summary>
        //public Vector3 Unit3Z
        //{
        //    get { return new Vector3(arrayMat[8], arrayMat[9], arrayMat[10]); }
        //    set { SetUnitZ(value.X, value.Y, value.Z); }
        //}
        ///// <summary>
        ///// <para>Get: Devuelve un nuevo Vector4 cuyas widgets equivalen al vector unitario z de la matriz.</para>
        ///// <para>Set: Establece la porción de la matriz que corresponde al vector unitario z, segun (ancho,alto,z) del vector.</para>
        ///// </summary>
        //public Vector4 Unit4Z
        //{
        //    get { return new Vector4(arrayMat[8], arrayMat[8], arrayMat[10], arrayMat[11]); }
        //    set { SetUnitZ(value.x, value.y, value.z, value.w); }
        //}
        /// <summary>
        ///   Establece la porción de la matriz que corresponde al vector unitario z, segun (ancho,alto,z). 
        /// </summary>
        /// <param name="ancho"></param>
        /// <param name="alto"></param>
        /// <param name="z"></param>
        public static void SetUnitZ(this Matrix4 m, float x, float y, float z)
        {
            //arrayMat[8] = x;
            //arrayMat[9] = y;
            //arrayMat[10] = z;
            //arrayMat[11] = 0;
            m.M31 = x;
            m.M32 = y;
            m.M33 = z;
            m.M34 = 0f;
        }
        /// <summary>
        ///   Establece la porción de la matriz que corresponde al vector unitario z, segun (ancho,alto,z,w). 
        /// </summary>
        /// <param name="ancho"></param>
        /// <param name="alto"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public static void SetUnitZ(this Matrix4 m, float x, float y, float z, float w)
        {
            //arrayMat[8] = x;
            //arrayMat[9] = y;
            //arrayMat[10] = z;
            //arrayMat[11] = w;
            m.M31 = x;
            m.M32 = y;
            m.M33 = z;
            m.M34 = w;

        }
        /// <summary>
        ///    Establece las componetes (ancho,alto,z) del vector a los valores (ancho,alto,z) del vector unitario z de la matriz.
        /// </summary>
        /// <param name="uniZ"></param>
        public static void GetUnitZ(this Matrix4 m, ref Vector3 uniZ)
        {
            //uniZ.X = arrayMat[8];
            //uniZ.Y = arrayMat[9];
            //uniZ.Z = arrayMat[10];
            uniZ.X = m.M31;
            uniZ.Y = m.M32;
            uniZ.Z = m.M33;
        }
        /// <summary>
        ///    Establece las componetes (ancho,alto,z,w) del vector a los valores (ancho,alto,z,w) del vector unitario z de la matriz.
        /// </summary>
        /// <param name="uniZ"></param>
        public static void GetUnitZ(this Matrix4 m, ref Vector4 uniZ)
        {
            //uniZ.x = arrayMat[8];
            //uniZ.y = arrayMat[9];
            //uniZ.z = arrayMat[10];
            //uniZ.w = arrayMat[11];
            uniZ.X = m.M31;
            uniZ.Y = m.M32;
            uniZ.Z = m.M33;
            uniZ.W = m.M34;
        }
        #endregion
    }
    public static class Matrox4d_Extension
    {
        public static Matrix4 ToHalfFloat(this Matrix4d m)
        {
            Vector4 row0 = new Vector4((float)m.Row0.X,
                                        (float)m.Row0.Y,
                                        (float)m.Row0.Z,
                                        (float)m.Row0.W);
            Vector4 row1 = new Vector4((float)m.Row1.X,
                                        (float)m.Row1.Y,
                                        (float)m.Row1.Z,
                                        (float)m.Row1.W);
            Vector4 row2 = new Vector4((float)m.Row2.X,
                                        (float)m.Row2.Y,
                                        (float)m.Row2.Z,
                                        (float)m.Row2.W);
            Vector4 row3 = new Vector4((float)m.Row3.X,
                                        (float)m.Row3.Y,
                                        (float)m.Row3.Z,
                                        (float)m.Row3.W);

            return new Matrix4(row0, row1, row2, row3);
        }
    }
    public static class Vector3_Extensions
    {
        public static Vector3 Cross(Vector3 left, Vector3 right)
        {
            Vector3 cross = new Vector3();

            cross.X = (left.Y * right.Z) - (left.Z * right.Y);
            cross.Y = (left.Z * right.X) - (left.X * right.Z);
            cross.Z = (left.X * right.Y) - (left.Y * right.X);

            return cross;
        }
        public static Vector3 Cross(this Vector3 v, Vector3 vLeft, Vector3 vRight)
        {
            Vector3 vout = new Vector3();
            float x = vLeft.Y * vRight.Z - vLeft.Z * vRight.Y;
            float y = vLeft.Z * vRight.X - vLeft.X * vRight.Z;
            float z = vLeft.X * vRight.Y - vLeft.Y * vRight.X;
            vout = new Vector3(x, y, z);
            v = vout;
            return vout;
        }
        public static void Cross(Vector3 left, Vector3 right, ref Vector3 result)
        {
            result.X = left.Y * right.Z - left.Z * right.Y;
            result.Y = left.Z * right.X - left.X * right.Z;
            result.Z = left.X * right.Y - left.Y * right.X;

        }
        public static double Distance(this Vector3 v1, Vector3 v2)
        {
            return
            (
               System.Math.Sqrt
               (
                   (v1.X - v2.X) * (v1.X - v2.X) +
                   (v1.Y - v2.Y) * (v1.Y - v2.Y) +
                   (v1.Z - v2.Z) * (v1.Z - v2.Z)
               )
            );
        }
        public static float Dot(this Vector3 v, Vector3 v1)
        {
            return (v.X * v1.X) + (v.Y * v1.Y) + (v.Z * v1.Z);
        }
        public static Vector3 Multiply(this Vector3 v1, Vector3 v2)
        {
            Vector3 vout = new Vector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
            v1 = vout;
            return vout;
        }
        public static Vector3 Normal(Vector3 point0, Vector3 point1, Vector3 point2)
        {
            Vector3 auxNormal = new Vector3();
            Vector3 edge1 = point1 - point0;
            Vector3 edge2 = point2 - point0;

            Cross(edge1, edge2, ref auxNormal);
            auxNormal.Normalize();
            return auxNormal;
        }
        public static void Normal(Vector3 point0, Vector3 point1, Vector3 point2, ref Vector3 result)
        {
            Vector3 edge1 = point1 - point0;
            Vector3 edge2 = point2 - point0;
            Cross(edge1, edge2, ref result);
            result.Normalize();
        }
        //public static void Normaliza(this Vector3 v)
        //{
        //    // Check whether this function is implemented the same as OpenTK.Vector3.Normilize()
        //    float length = (float)System.Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);

        //    // Will also work for zero-sized vectors, but will change nothing
        //    if (length > float.Epsilon)
        //    {
        //        float inverseLength = 1.0f / length;

        //        v.X *= inverseLength;
        //        v.Y *= inverseLength;
        //        v.Z *= inverseLength;
        //    }

        //    //return length;
        //}

        /// <summary>
        /// [OBSOLETE] Use OpenTK.Vector3.TransformPosition instead
        /// </summary>
        /// <param name="v"></param>
        /// <param name="v1"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3 TransformPoint(this Vector3 v, Matrix4 matrix)
        {
            Vector3 vout = new Vector3();
            float x = v.X * matrix.M11 + v.Y * matrix.M21 + v.Z * matrix.M31 + matrix.M41;
            float y = v.X * matrix.M12 + v.Y * matrix.M22 + v.Z * matrix.M32 + matrix.M42;
            float z = v.X * matrix.M13 + v.Y * matrix.M23 + v.Z * matrix.M33 + matrix.M43;
            vout = new Vector3(x, y, z);
            v = vout;
            return vout;
        }
        /// <summary>
        /// [OBSOLETE] Use OpenTK.Vector3.TransformPosition instead
        /// </summary>
        /// <param name="v"></param>
        /// <param name="v1"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3 TransformPoint(this Vector3 v, Vector3 v1, Matrix4 matrix)
        {
            Vector3 vout = new Vector3();
            float x = v1.X * matrix.M11 + v1.Y * matrix.M21 + v1.Z * matrix.M31 + matrix.M41;
            float y = v1.X * matrix.M12 + v1.Y * matrix.M22 + v1.Z * matrix.M32 + matrix.M42;
            float z = v1.X * matrix.M13 + v1.Y * matrix.M23 + v1.Z * matrix.M33 + matrix.M43;
            vout = new Vector3(x, y, z);
            v = vout;
            return vout;
        }
        //public static Vector3 TransformVector(this Vector3 v, Matrix4 matrix)
        //{
        //    Vector3 vout;

        //    vout.X = v.X * matrix.M11 + v.Y * matrix.M21 + v.Z * matrix.M31;
        //    vout.Y = v.X * matrix.M12 + v.Y * matrix.M22 + v.Z * matrix.M32;
        //    vout.Z = v.X * matrix.M13 + v.Y * matrix.M23 + v.Z * matrix.M33;

        //    v = new Vector3(vout.X, vout.Y, vout.Z);
        //    return new Vector3(vout.X, vout.Y, vout.Z);
        //}

        //public static Vector3 TransformVector(this Vector3 vec, Matrix4 mat)
        //{
        //    /// <summary>Transform a direction vector by the given Matrix
        //    /// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
        //    /// </summary>
        //    /// <param name="vec">The vector to transform</param>
        //    /// <param name="mat">The desired transformation</param>
        //    /// <returns>The transformed vector</returns>
        //    Vector3 v = new Vector3();
        //    Vector3 v1 = new Vector3(mat.Column0.X, mat.Column0.Y, mat.Column0.Z);
        //    Vector3 v2 = new Vector3(mat.Column1.X, mat.Column1.Y, mat.Column1.Z);
        //    Vector3 v3 = new Vector3(mat.Column2.X, mat.Column2.Y, mat.Column2.Z);
        //    v.X = Vector3.Dot(vec, v1);
        //    v.Y = Vector3.Dot(vec, v2);
        //    v.Z = Vector3.Dot(vec, v3);
        //    return v;
        //}
        public static Vector3 TransformVector(this Vector3 vec, Matrix4 mat)
        {
            // TODO:: This function is added for debug. Remove if implementation matches above
            // Nov-02-14 Modified to use ref Vector3 

            /// <summary>Transform a direction vector by the given Matrix
            /// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
            /// </summary>
            /// <param name="vec">The vector to transform</param>
            /// <param name="mat">The desired transformation</param>
            /// <returns>The transformed vector</returns>
            //Vector3 v = new Vector3();
            vec.X = vec.X * mat.M11 + vec.Y * mat.M21 + vec.Z * mat.M31;
            vec.Y = vec.X * mat.M12 + vec.Y * mat.M22 + vec.Z * mat.M32;
            vec.Z = vec.X * mat.M13 + vec.Y * mat.M23 + vec.Z * mat.M33;
            //vec = v;
            return vec;
        }
        public static void TransformVector(ref Vector3 vec, ref Matrix4 mat, out Vector3 result)
        {
            /// <summary>Transform a direction vector by the given Matrix
            /// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
            /// </summary>
            /// <param name="vec">The vector to transform</param>
            /// <param name="mat">The desired transformation</param>
            /// <param name="result">The transformed vector</param>
            result = new Vector3();
            result.X = vec.X * mat.Row0.X +
                       vec.Y * mat.Row1.X +
                       vec.Z * mat.Row2.X;

            result.Y = vec.X * mat.Row0.Y +
                       vec.Y * mat.Row1.Y +
                       vec.Z * mat.Row2.Y;

            result.Z = vec.X * mat.Row0.Z +
                       vec.Y * mat.Row1.Z +
                       vec.Z * mat.Row2.Z;

            vec = result;
        }
        public static Vector3 TransformNormal(Vector3 norm, Matrix4 mat)
        {
            /// <summary>Transform a Normal by the given Matrix</summary>
            /// <remarks>
            /// This calculates the inverse of the given matrix, use TransformNormalInverse if you
            /// already have the inverse to avoid this extra calculation
            /// </remarks>
            /// <param name="norm">The normal to transform</param>
            /// <param name="mat">The desired transformation</param>
            /// <returns>The transformed normal</returns>
            //mat.Invert();
            Matrix4 mat_out = new Matrix4();
            mat_out = mat;
            mat_out.Invert();
            //mat.InvertMatrix(ref mat_out);
            return TransformNormalInverse(norm, mat_out);
        }
        public static void TransformNormal(ref Vector3 norm, ref Matrix4 mat, out Vector3 result)
        {
            /// <summary>Transform a Normal by the given Matrix</summary>
            /// <remarks>
            /// This calculates the inverse of the given matrix, use TransformNormalInverse if you
            /// already have the inverse to avoid this extra calculation
            /// </remarks>
            /// <param name="norm">The normal to transform</param>
            /// <param name="mat">The desired transformation</param>
            /// <param name="result">The transformed normal</param>
            Matrix4 Inverse = new Matrix4();
            //Matrix4.Invert(mat);
            Inverse = mat;
            Inverse.Invert();
            //mat.InvertMatrix(ref Inverse);

            Vector3.TransformNormalInverse(ref norm, ref Inverse, out result);
        }
        public static Vector3 TransformNormalInverse(Vector3 norm, Matrix4 invMat)
        {
            /// <summary>Transform a Normal by the (transpose of the) given Matrix</summary>
            /// <remarks>
            /// This version doesn't calculate the inverse matrix.
            /// Use this version if you already have the inverse of the desired transform to hand
            /// </remarks>
            /// <param name="norm">The normal to transform</param>
            /// <param name="invMat">The inverse of the desired transformation</param>
            /// <returns>The transformed normal</returns>
            Vector3 n = new Vector3();
            Vector3 v1 = new Vector3(invMat.Row0.X, invMat.Row0.Y, invMat.Row0.Z);
            Vector3 v2 = new Vector3(invMat.Row1.X, invMat.Row1.Y, invMat.Row1.Z);
            Vector3 v3 = new Vector3(invMat.Row2.X, invMat.Row2.Y, invMat.Row2.Z);
            n.X = Vector3.Dot(norm, v1);
            n.Y = Vector3.Dot(norm, v2);
            n.Z = Vector3.Dot(norm, v3);
            return n;
        }
        public static void TransformNormalInverse(ref Vector3 norm, ref Matrix4 invMat, out Vector3 result)
        {
            /// <summary>Transform a Normal by the (transpose of the) given Matrix</summary>
            /// <remarks>
            /// This version doesn't calculate the inverse matrix.
            /// Use this version if you already have the inverse of the desired transform to hand
            /// </remarks>
            /// <param name="norm">The normal to transform</param>
            /// <param name="invMat">The inverse of the desired transformation</param>
            /// <param name="result">The transformed normal</param>
            result = new Vector3();
            result.X = norm.X * invMat.Row0.X +
                       norm.Y * invMat.Row0.Y +
                       norm.Z * invMat.Row0.Z;

            result.Y = norm.X * invMat.Row1.X +
                       norm.Y * invMat.Row1.Y +
                       norm.Z * invMat.Row1.Z;

            result.Z = norm.X * invMat.Row2.X +
                       norm.Y * invMat.Row2.Y +
                       norm.Z * invMat.Row2.Z;
        }
        public static Vector3 TransformPosition(Vector3 pos, Matrix4 mat)
        {
            /// <summary>Transform a Position by the given Matrix</summary>
            /// <param name="enuDock">The position to transform</param>
            /// <param name="mat">The desired transformation</param>
            /// <returns>The transformed position</returns>
            Vector3 p = new Vector3();
            Vector3 v1 = new Vector3(mat.Column0.X, mat.Column0.Y, mat.Column0.Z);
            Vector3 v2 = new Vector3(mat.Column1.X, mat.Column1.Y, mat.Column1.Z);
            Vector3 v3 = new Vector3(mat.Column2.X, mat.Column2.Y, mat.Column2.Z);
            p.X = Vector3.Dot(pos, v1) + mat.Row3.X;
            p.Y = Vector3.Dot(pos, v2) + mat.Row3.Y;
            p.Z = Vector3.Dot(pos, v3) + mat.Row3.Z;
            return p;
        }
        public static void TransformPosition(ref Vector3 pos, ref Matrix4 mat, out Vector3 result)
        {
            /// <summary>Transform a Position by the given Matrix</summary>
            /// <param name="enuDock">The position to transform</param>
            /// <param name="mat">The desired transformation</param>
            /// <param name="result">The transformed position</param>
            result = new Vector3();
            result.X = pos.X * mat.Row0.X +
                       pos.Y * mat.Row1.X +
                       pos.Z * mat.Row2.X +
                       mat.Row3.X;

            result.Y = pos.X * mat.Row0.Y +
                       pos.Y * mat.Row1.Y +
                       pos.Z * mat.Row2.Y +
                       mat.Row3.Y;

            result.Z = pos.X * mat.Row0.Z +
                       pos.Y * mat.Row1.Z +
                       pos.Z * mat.Row2.Z +
                       mat.Row3.Z;
        }
        public static Vector3 Transform(Vector3 vec, Matrix4 mat)
        {
            /// <summary>Transform a Vector by the given Matrix</summary>
            /// <param name="vec">The vector to transform</param>
            /// <param name="mat">The desired transformation</param>
            /// <returns>The transformed vector</returns>
            Vector3 result;
            Transform(ref vec, ref mat, out result);
            return result;
        }
        public static void Transform(ref Vector3 vec, ref Matrix4 mat, out Vector3 result)
        {
            /// <summary>Transform a Vector by the given Matrix</summary>
            /// <param name="vec">The vector to transform</param>
            /// <param name="mat">The desired transformation</param>
            /// <param name="result">The transformed vector</param>
            Vector4 v4 = new Vector4(vec.X, vec.Y, vec.Z, 1.0f);
            Vector4.Transform(ref v4, ref mat, out v4);
            result = new Vector3(v4.X, v4.Y, v4.Z);
        }
        public static Vector3 Transform(Vector3 vec, Quaternion quat)
        {
            /// <summary>
            /// Transforms a vector by a quaternion rotation.
            /// </summary>
            /// <param name="vec">The vector to transform.</param>
            /// <param name="quat">The quaternion to rotate the vector by.</param>
            /// <returns>The result of the operation.</returns>
            Vector3 result = new Vector3();
            Transform(ref vec, ref quat, out result);
            return result;
        }
        public static void Transform(ref Vector3 vec, ref Quaternion quat, out Vector3 result)
        {
            /// <summary>
            /// Transforms a vector by a quaternion rotation.
            /// </summary>
            /// <param name="vec">The vector to transform.</param>
            /// <param name="quat">The quaternion to rotate the vector by.</param>
            /// <param name="result">The result of the operation.</param>

            // Since vec.w == 0, we can optimize quat * vec * quat^-1 as follows:
            // vec + 2.0 * cross(quat.xyz, cross(quat.xyz, vec) + quat.angle * vec)
            Vector3 xyz = new Vector3(quat.X, quat.Y, quat.Z), temp = new Vector3(), temp2;

            temp = Vector3.Cross(xyz, vec); //Vector3.Cross(xyz, vec, ref temp);

            Vector3.Multiply(ref vec, quat.W, out temp2);
            Vector3.Add(ref temp, ref temp2, out temp);

            temp = Vector3.Cross(xyz, temp); //Vector3.Cross(xyz, temp, ref temp);

            Vector3.Multiply(ref temp, 2, out temp);
            Vector3.Add(ref vec, ref temp, out result);
        }
        public static Vector3 TransformPerspective(Vector3 vec, Matrix4 mat)
        {
            /// <summary>Transform a Vector3 by the given Matrix, and project the resulting Vector4 back to a Vector3</summary>
            /// <param name="vec">The vector to transform</param>
            /// <param name="mat">The desired transformation</param>
            /// <returns>The transformed vector</returns>
            Vector3 result;
            TransformPerspective(ref vec, ref mat, out result);
            return result;
        }
        public static void TransformPerspective(ref Vector3 vec, ref Matrix4 mat, out Vector3 result)
        {
            /// <summary>Transform a Vector3 by the given Matrix, and project the resulting Vector4 back to a Vector3</summary>
            /// <param name="vec">The vector to transform</param>
            /// <param name="mat">The desired transformation</param>
            /// <param name="result">The transformed vector</param>
            Vector4 v = new Vector4(vec.X, vec.Y, vec.Z, 1);
            Vector4.Transform(ref v, ref mat, out v);
            result = new Vector3();
            result.X = v.X / v.W;
            result.Y = v.Y / v.W;
            result.Z = v.Z / v.W;
        }
        public static float[] vertex(this Vector3 v)
        {
            float[] fout = new float[] { v.X, v.Y, v.Z };
            return fout;
        }
        public static float vertex(this Vector3 v, int index)
        {
            float fout = 0f;

            if (index == 0)
                fout = v.X;
            if (index == 1)
                fout = v.Y;
            if (index == 2)
                fout = v.Z;


            return fout;
        }


    }
    public static class Vector4_Extensions
    {
        public static float DotNormal(this Vector4 v4, Vector3 v3)
        {
            return (v4.X * v3.X) + (v4.Y * v3.Y) + (v4.Z * v3.Z);
        }
        public static Vector4 FromVector3(this Vector4 v, Vector3 p_point1, Vector3 p_normal)
        {
            Vector4 ret = new Vector4();
            Vector3 normal = new Vector3();

            normal = p_normal;//.Normalize();
            normal.Normalize();
            ret.W = normal.Dot(p_point1);

            ret.X = normal.X;
            ret.Y = normal.Y;
            ret.Z = normal.Z;
            v = ret;
            return ret;
        }
        /// <summary>
        /// Ray Intersect
        /// </summary>
        /// <param name="v"></param>
        /// <param name="interPoint"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static bool RayIntersect(this Vector4 v, ref Vector3 interPoint, Vector3 position, Vector3 direction)
        {
            float den = new Vector3(v.X, v.Y, v.Z).Dot(direction);

            if (Math.Abs(den) < 0.00001)
            {
                return false;
            }

            Vector3 tmp = new Vector3(v.X, v.Y, v.Z) * v.W - position;
            interPoint = position + (new Vector3(v.X, v.Y, v.Z).Dot(tmp) / den) * direction;

            return true;
        }
    }
    public static class Quaternion_Extensions
    {
        public static Quaternion FromMatrix2(this Quaternion quat, Matrix4 mat)
        {
            Quaternion q = new Quaternion();
            float T = mat.M11 + mat.M22 + mat.M33;
            if (T <= 0)
            {
                if ((mat.M11 > mat.M22) && (mat.M11 > mat.M33))
                {
                    float S = (float)Math.Sqrt(1 + mat.M11 - mat.M22 - mat.M33) * 2;
                    q.X = 1.0f / (2 * S);
                    S = 1.0f / S;
                    q.Y = (mat.M12 - mat.M21) * S;
                    q.Z = (mat.M13 - mat.M31) * S;
                    q.W = (mat.M23 - mat.M32) * S;
                }
                else if ((mat.M22 > mat.M11) && (mat.M22 > mat.M33))
                {
                    float S = (float)Math.Sqrt(1 - mat.M11 + mat.M22 - mat.M33) * 2;
                    q.Y = 1.0f / (2 * S);
                    S = 1.0f / S;
                    q.X = (mat.M12 - mat.M21) * S;
                    q.Z = (mat.M23 - mat.M31) * S;
                    q.W = (mat.M13 - mat.M31) * S;
                }
                else
                {
                    float S = (float)Math.Sqrt(1 - mat.M11 - mat.M22 + mat.M33) * 2;
                    q.Z = 1.0f / (2 * S);
                    S = 1.0f / S;
                    q.X = (mat.M13 - mat.M31) * S;
                    q.Y = (mat.M23 - mat.M32) * S;
                    q.W = (mat.M12 - mat.M21) * S;
                }
            }
            else
            {
                float S = 1.0f / (2 * (float)Math.Sqrt(T));
                q.X = (mat.M32 - mat.M23) * S;
                q.Y = (mat.M13 - mat.M31) * S;
                q.Z = (mat.M21 - mat.M12) * S;
                q.W = 1.0f / (4 * S);
            }

            quat = q;
            return q;
        }
        public static Quaternion FromMatrix(this Quaternion quat, Matrix4 m)
        {
            double trace = 1 + m.M11 + m.M22 + m.M33;
            double S = 0;
            double X = 0;
            double Y = 0;
            double Z = 0;
            double W = 0;

            if (trace >= 0.0000001)
            {
                S = (double)Math.Sqrt(trace) * 2;
                X = (m.M23 - m.M32) / S;
                Y = (m.M31 - m.M13) / S;
                Z = (m.M12 - m.M21) / S;
                W = 0.25d * S;
            }
            else
            {
                if (m.M11 > m.M22 && m.M11 > m.M33)
                {
                    // Column 0: 
                    S = (double)Math.Sqrt(1.0 + m.M11 - m.M22 - m.M33) * 2;
                    X = 0.25d * S;
                    Y = (m.M12 + m.M21) / S;
                    Z = (m.M31 + m.M13) / S;
                    W = (m.M23 - m.M32) / S;
                }
                else if (m.M22 > m.M33)
                {
                    // Column 1: 
                    S = (double)Math.Sqrt(1.0 + m.M22 - m.M11 - m.M33) * 2;
                    X = (m.M12 + m.M21) / S;
                    Y = 0.25d * S;
                    Z = (m.M23 + m.M32) / S;
                    W = (m.M31 - m.M13) / S;
                }
                else
                {
                    // Column 2:
                    S = (double)Math.Sqrt(1.0 + m.M33 - m.M11 - m.M22) * 2;
                    X = (m.M31 + m.M13) / S;
                    Y = (m.M23 + m.M32) / S;
                    Z = 0.25d * S;
                    W = (m.M12 - m.M21) / S;
                }
            }
            Quaternion q = new Quaternion((float)X, (float)Y, (float)Z, (float)W);
            quat = q;
            return q;
        }
        public static Quaternion UnitUninverse(this Quaternion quat)
        {
            Quaternion qout = new Quaternion();
            //x = -x;
            //y = -y;
            //z = -z;
            //w = w;
            qout.X = -quat.X;
            qout.Y = -quat.Y;
            qout.Z = -quat.Z;
            qout.W = quat.W;

            quat = qout;
            return qout;
        }
    }
    #endregion Algebra Extensions
}
