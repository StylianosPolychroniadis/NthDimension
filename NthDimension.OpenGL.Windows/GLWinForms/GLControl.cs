using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace OpenTK
{
	/// <summary>
	/// OpenGL-aware WinForms control.
	/// The WinForms designer will always call the default constructor.
	/// Inherit from this class and call one of its specialized constructors
	/// to enable antialiasing or custom <see cref="P:OpenTK.GLControl.GraphicsMode" />s.
	/// </summary>
	public class GLControl : UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private IContainer components;

		private IGraphicsContext context;

		private IGLControl implementation;

		private GraphicsMode format;

		private int major;

		private int minor;

		private GraphicsContextFlags flags;

		private bool? initial_vsync_value;

		private bool resize_event_suppressed;

		private readonly bool design_mode;

		private IGLControl Implementation
		{
			get
			{
				ValidateState();
				return implementation;
			}
		}

		/// <summary>
		/// Gets the <c>CreateParams</c> instance for this <c>GLControl</c>
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				if (Configuration.RunningOnWindows)
				{
					createParams.ClassStyle |= 35;
					createParams.Style |= 100663296;
				}
				return createParams;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current thread contains pending system messages.
		/// </summary>
		[Browsable(false)]
		public bool IsIdle
		{
			get
			{
				ValidateState();
				return Implementation.IsIdle;
			}
		}

		/// <summary>
		/// Gets an interface to the underlying GraphicsContext used by this GLControl.
		/// </summary>
		[Browsable(false)]
		public IGraphicsContext Context
		{
			get
			{
				ValidateState();
				return context;
			}
			private set
			{
				context = value;
			}
		}

		/// <summary>
		/// Gets the aspect ratio of this GLControl.
		/// </summary>
		[Description("The aspect ratio of the client area of this GLControl.")]
		public float AspectRatio
		{
			get
			{
				ValidateState();
				return (float)base.ClientSize.Width / (float)base.ClientSize.Height;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether vsync is active for this GLControl.
		/// </summary>
		[Description("Indicates whether GLControl updates are synced to the monitor's refresh rate.")]
		public bool VSync
		{
			get
			{
				if (!base.IsHandleCreated)
				{
					return false;
				}
				ValidateState();
				return Context.VSync;
			}
			set
			{
				if (!base.IsHandleCreated)
				{
					initial_vsync_value = value;
					return;
				}
				ValidateState();
				Context.VSync = value;
			}
		}

		/// <summary>
		/// Gets the GraphicsMode of the GraphicsContext attached to this GLControl.
		/// </summary>
		/// <remarks>
		/// To change the GraphicsMode, you must destroy and recreate the GLControl.
		/// </remarks>
		public GraphicsMode GraphicsMode
		{
			get
			{
				ValidateState();
				return Context.GraphicsMode;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:OpenTK.Platform.IWindowInfo" /> for this instance.
		/// </summary>
		public IWindowInfo WindowInfo
		{
			get
			{
				return implementation.WindowInfo;
			}
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			BackColor = System.Drawing.Color.Black;
			base.Name = "NewGLControl";
			ResumeLayout(false);
		}

		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		public GLControl()
			: this(GraphicsMode.Default)
		{
		}

		/// <summary>
		/// Constructs a new instance with the specified GraphicsMode.
		/// </summary>
		/// <param name="mode">The OpenTK.Graphics.GraphicsMode of the control.</param>
		public GLControl(GraphicsMode mode)
			: this(mode, 1, 0, GraphicsContextFlags.Default)
		{
		}

		/// <summary>
		/// Constructs a new instance with the specified GraphicsMode.
		/// </summary>
		/// <param name="mode">The OpenTK.Graphics.GraphicsMode of the control.</param>
		/// <param name="major">The major version for the OpenGL GraphicsContext.</param>
		/// <param name="minor">The minor version for the OpenGL GraphicsContext.</param>
		/// <param name="flags">The GraphicsContextFlags for the OpenGL GraphicsContext.</param>
		public GLControl(GraphicsMode mode, int major, int minor, GraphicsContextFlags flags)
		{
			if (mode == null)
			{
				throw new ArgumentNullException("mode");
			}
			Toolkit.Init(new ToolkitOptions
			{
				Backend = PlatformBackend.PreferNative
			});
			SetStyle(ControlStyles.Opaque, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			DoubleBuffered = false;
			format = mode;
			this.major = major;
			this.minor = minor;
			this.flags = flags;
			design_mode = (base.DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime);
			InitializeComponent();
		}

		private void ValidateState()
		{
			if (base.IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
			if (!base.IsHandleCreated)
			{
				CreateControl();
			}
			if (implementation == null || context == null || context.IsDisposed)
			{
				RecreateHandle();
			}
		}

		/// <summary>Raises the HandleCreated event.</summary>
		/// <param name="e">Not used.</param>
		protected override void OnHandleCreated(EventArgs e)
		{
			if (context != null)
			{
				context.Dispose();
			}
			if (implementation != null)
			{
				implementation.WindowInfo.Dispose();
			}
			if (design_mode)
			{
				implementation = new DummyGLControl();
			}
			else
			{
				implementation = new GLControlFactory().CreateGLControl(format, this);
			}
			context = implementation.CreateContext(major, minor, flags);
			MakeCurrent();
			if (!design_mode)
			{
				((IGraphicsContextInternal)Context).LoadAll();
			}
			if (initial_vsync_value.HasValue)
			{
				Context.SwapInterval = (initial_vsync_value.Value ? 1 : 0);
				initial_vsync_value = null;
			}
			base.OnHandleCreated(e);
			if (resize_event_suppressed)
			{
				OnResize(EventArgs.Empty);
				resize_event_suppressed = false;
			}
		}

		/// <summary>Raises the HandleDestroyed event.</summary>
		/// <param name="e">Not used.</param>
		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (context != null)
			{
				context.Dispose();
				context = null;
			}
			if (implementation != null)
			{
				implementation.WindowInfo.Dispose();
				implementation = null;
			}
			base.OnHandleDestroyed(e);
		}

		/// <summary>
		/// Raises the System.Windows.Forms.Control.Paint event.
		/// </summary>
		/// <param name="e">A System.Windows.Forms.PaintEventArgs that contains the event data.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			ValidateState();
			if (design_mode)
			{
				e.Graphics.Clear(BackColor);
			}
			base.OnPaint(e);
		}

		/// <summary>
		/// Raises the Resize event.
		/// Note: this method may be called before the OpenGL context is ready.
		/// Check that IsHandleCreated is true before using any OpenGL methods.
		/// </summary>
		/// <param name="e">A System.EventArgs that contains the event data.</param>
		protected override void OnResize(EventArgs e)
		{
			if (!base.IsHandleCreated)
			{
				resize_event_suppressed = true;
				return;
			}
			if (context != null)
			{
				context.Update(Implementation.WindowInfo);
			}
			base.OnResize(e);
		}

		/// <summary>
		/// Raises the ParentChanged event.
		/// </summary>
		/// <param name="e">A System.EventArgs that contains the event data.</param>
		protected override void OnParentChanged(EventArgs e)
		{
			if (context != null)
			{
				context.Update(Implementation.WindowInfo);
			}
			base.OnParentChanged(e);
		}

		/// <summary>
		/// Swaps the front and back buffers, presenting the rendered scene to the screen.
		/// </summary>
		public void SwapBuffers()
		{
			ValidateState();
			Context.SwapBuffers();
		}

		/// <summary>
		/// Makes the underlying this GLControl current in the calling thread.
		/// All OpenGL commands issued are hereafter interpreted by this GLControl.
		/// </summary>
		public void MakeCurrent()
		{
			ValidateState();
			Context.MakeCurrent(Implementation.WindowInfo);
		}

		/// <summary>Grabs a screenshot of the frontbuffer contents.</summary>
		/// <returns>A System.Drawing.Bitmap, containing the contents of the frontbuffer.</returns>
		/// <exception cref="T:OpenTK.Graphics.GraphicsContextException">
		/// Occurs when no OpenTK.Graphics.GraphicsContext is current in the calling thread.
		/// </exception>
		[Obsolete("This method will not work correctly with OpenGL|ES. Please use GL.ReadPixels to capture the contents of the framebuffer (refer to http://www.opentk.com/doc/graphics/save-opengl-rendering-to-disk for more information).")]
		public Bitmap GrabScreenshot()
		{
			ValidateState();
			Bitmap bitmap = new Bitmap(base.ClientSize.Width, base.ClientSize.Height);
			BitmapData bitmapData = bitmap.LockBits(base.ClientRectangle, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			GL.ReadPixels(0, 0, base.ClientSize.Width, base.ClientSize.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, bitmapData.Scan0);
			bitmap.UnlockBits(bitmapData);
			bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
			return bitmap;
		}
	}
}
