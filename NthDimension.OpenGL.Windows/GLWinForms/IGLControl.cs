using OpenTK.Graphics;
using OpenTK.Platform;

namespace OpenTK
{
	internal interface IGLControl
	{
		bool IsIdle
		{
			get;
		}

		IWindowInfo WindowInfo
		{
			get;
		}

		IGraphicsContext CreateContext(int major, int minor, GraphicsContextFlags flags);
	}
}
