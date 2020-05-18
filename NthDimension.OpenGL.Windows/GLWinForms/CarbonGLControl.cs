using OpenTK.Graphics;
using OpenTK.Platform;
using System.Windows.Forms;

namespace OpenTK
{
	internal class CarbonGLControl : IGLControl
	{
		private GraphicsMode mode;

		private Control control;

		private IWindowInfo window_info;

		private bool lastIsIdle;

		public bool IsIdle
		{
			get
			{
				lastIsIdle = !lastIsIdle;
				return lastIsIdle;
			}
		}

		public IWindowInfo WindowInfo
		{
			get
			{
				return window_info;
			}
		}

		internal CarbonGLControl(GraphicsMode mode, Control owner)
		{
			this.mode = mode;
			control = owner;
			window_info = Utilities.CreateMacOSCarbonWindowInfo(control.Handle, false, true);
		}

		public IGraphicsContext CreateContext(int major, int minor, GraphicsContextFlags flags)
		{
			return new GraphicsContext(mode, WindowInfo, major, minor, flags);
		}
	}
}
