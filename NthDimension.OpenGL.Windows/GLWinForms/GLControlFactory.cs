using OpenTK.Graphics;
using System;
using System.Windows.Forms;

namespace OpenTK
{
	internal class GLControlFactory
	{
		public IGLControl CreateGLControl(GraphicsMode mode, Control control)
		{
			if (mode == null)
			{
				throw new ArgumentNullException("mode");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (Configuration.RunningOnSdl2)
			{
				return new Sdl2GLControl(mode, control);
			}
			if (Configuration.RunningOnWindows)
			{
				return new WinGLControl(mode, control);
			}
			if (Configuration.RunningOnMacOS)
			{
				return new CarbonGLControl(mode, control);
			}
			if (Configuration.RunningOnX11)
			{
				return new X11GLControl(mode, control);
			}
			throw new PlatformNotSupportedException();
		}
	}
}
