using OpenTK.Graphics;
using OpenTK.Platform;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace OpenTK
{
	internal class WinGLControl : IGLControl
	{
		private struct MSG
		{
			public IntPtr HWnd;

			public uint Message;

			public IntPtr WParam;

			public IntPtr LParam;

			public uint Time;

			public POINT Point;

			public override string ToString()
			{
				return string.Format("msg=0x{0:x} ({1}) hwnd=0x{2:x} wparam=0x{3:x} lparam=0x{4:x} pt=0x{5:x}", (int)Message, Message.ToString(), HWnd.ToInt32(), WParam.ToInt32(), LParam.ToInt32(), Point);
			}
		}

		private struct POINT
		{
			public int X;

			public int Y;

			public POINT(int x, int y)
			{
				X = x;
				Y = y;
			}

			public Point ToPoint()
			{
				return new Point(X, Y);
			}

			public override string ToString()
			{
				return "Point {" + X + ", " + Y + ")";
			}
		}

		private MSG msg = default(MSG);

		private IWindowInfo window_info;

		private GraphicsMode mode;

		public bool IsIdle
		{
			get
			{
				return !PeekMessage(ref msg, IntPtr.Zero, 0, 0, 0);
			}
		}

		public IWindowInfo WindowInfo
		{
			get
			{
				return window_info;
			}
		}

		[DllImport("User32.dll")]
		[SuppressUnmanagedCodeSecurity]
		private static extern bool PeekMessage(ref MSG msg, IntPtr hWnd, int messageFilterMin, int messageFilterMax, int flags);

		public WinGLControl(GraphicsMode mode, Control control)
		{
			this.mode = mode;
			window_info = Utilities.CreateWindowsWindowInfo(control.Handle);
		}

		public IGraphicsContext CreateContext(int major, int minor, GraphicsContextFlags flags)
		{
			return new GraphicsContext(mode, window_info, major, minor, flags);
		}
	}
}
