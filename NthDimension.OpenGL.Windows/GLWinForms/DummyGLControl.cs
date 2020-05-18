using OpenTK.Graphics;
using OpenTK.Platform;

namespace OpenTK
{
	internal class DummyGLControl : IGLControl
	{
		public bool IsIdle
		{
			get
			{
				return false;
			}
		}

		public IWindowInfo WindowInfo
		{
			get
			{
				return Utilities.CreateDummyWindowInfo();
			}
		}

		public IGraphicsContext CreateContext(int major, int minor, GraphicsContextFlags flags)
		{
			return new GraphicsContext(null, null);
		}
	}
}
