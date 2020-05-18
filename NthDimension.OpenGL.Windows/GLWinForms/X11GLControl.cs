using OpenTK.Graphics;
using OpenTK.Platform;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OpenTK
{
	internal class X11GLControl : IGLControl
	{
		private struct XVisualInfo
		{
			public IntPtr Visual;

			public IntPtr VisualID;

			public int Screen;

			public int Depth;

			public int Class;

			public long RedMask;

			public long GreenMask;

			public long blueMask;

			public int ColormapSize;

			public int BitsPerRgb;

			public override string ToString()
			{
				return string.Format("id ({0}), screen ({1}), depth ({2}), class ({3})", VisualID, Screen, Depth, Class);
			}
		}

		private GraphicsMode mode;

		private IWindowInfo window_info;

		private IntPtr display;

		public bool IsIdle
		{
			get
			{
				return XPending(display) == 0;
			}
		}

		public IWindowInfo WindowInfo
		{
			get
			{
				return window_info;
			}
		}

		[DllImport("libX11")]
		private static extern IntPtr XCreateColormap(IntPtr display, IntPtr window, IntPtr visual, int alloc);

		[DllImport("libX11", EntryPoint = "XGetVisualInfo")]
		private static extern IntPtr XGetVisualInfoInternal(IntPtr display, IntPtr vinfo_mask, ref XVisualInfo template, out int nitems);

		private static IntPtr XGetVisualInfo(IntPtr display, int vinfo_mask, ref XVisualInfo template, out int nitems)
		{
			return XGetVisualInfoInternal(display, (IntPtr)vinfo_mask, ref template, out nitems);
		}

		[DllImport("libX11")]
		private static extern int XPending(IntPtr diplay);

		internal X11GLControl(GraphicsMode mode, Control control)
		{
			if (mode == null)
			{
				throw new ArgumentNullException("mode");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (!mode.Index.HasValue)
			{
				throw new GraphicsModeException("Invalid or unsupported GraphicsMode.");
			}
			this.mode = mode;
			Type type = Type.GetType("System.Windows.Forms.XplatUIX11, System.Windows.Forms");
			if (type == null)
			{
				throw new PlatformNotSupportedException("System.Windows.Forms.XplatUIX11 missing. Unsupported platform or Mono runtime version, aborting.");
			}
			display = (IntPtr)GetStaticFieldValue(type, "DisplayHandle");
			IntPtr intPtr = (IntPtr)GetStaticFieldValue(type, "RootWindow");
			int screen = (int)GetStaticFieldValue(type, "ScreenNo");
			XVisualInfo template = new XVisualInfo
			{
				VisualID = mode.Index.Value
			};
			int nitems;
			IntPtr intPtr2 = XGetVisualInfo(display, 1, ref template, out nitems);
			template = (XVisualInfo)Marshal.PtrToStructure(intPtr2, typeof(XVisualInfo));
			SetStaticFieldValue(type, "CustomVisual", template.Visual);
			SetStaticFieldValue(type, "CustomColormap", XCreateColormap(display, intPtr, template.Visual, 0));
			window_info = Utilities.CreateX11WindowInfo(display, screen, control.Handle, intPtr, intPtr2);
		}

		public IGraphicsContext CreateContext(int major, int minor, GraphicsContextFlags flags)
		{
			return new GraphicsContext(mode, WindowInfo, major, minor, flags);
		}

		private static object GetStaticFieldValue(Type type, string fieldName)
		{
			return type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
		}

		private static void SetStaticFieldValue(Type type, string fieldName, object value)
		{
			type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, value);
		}
	}
}
