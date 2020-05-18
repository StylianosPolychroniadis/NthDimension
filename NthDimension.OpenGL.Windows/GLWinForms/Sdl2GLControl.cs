using OpenTK.Graphics;
using OpenTK.Platform;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OpenTK
{
	internal class Sdl2GLControl : IGLControl
	{
		private static class NativeMethods
		{
			[DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
			public static extern bool SDL_HasEvents(int minType, int maxType);
		}

		private IWindowInfo window_info;

		private GraphicsMode mode;

		public bool IsIdle
		{
			get
			{
				return NativeMethods.SDL_HasEvents(0, 65535);
			}
		}

		public IWindowInfo WindowInfo
		{
			get
			{
				return window_info;
			}
		}

		public Sdl2GLControl(GraphicsMode mode, Control control)
		{
			this.mode = mode;
			window_info = Utilities.CreateSdl2WindowInfo(control.Handle);
		}

		public IGraphicsContext CreateContext(int major, int minor, GraphicsContextFlags flags)
		{
			return new GraphicsContext(mode, window_info, major, minor, flags);
		}
	}
}
