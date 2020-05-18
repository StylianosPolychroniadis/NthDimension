// NthDimension Application API - Application Entry Point

namespace [projectName]
{
	// Framework members
	using System;
	using System.IO;
	using System.Linq;
	using System.Drawing;
	using System.ComponentModel;	
	using System.Collections.Generic;
	// Numerical library (Vector, Matrix, Quaternion, Tensor)
	using NthDimension.Algebra;
	// Rendering library
	using NthDimension.Rendering;
	using NthDimension.Rendering.Configuration;
	using NthDimension.Rendering.Drawables;
	using NthDimension.Rendering.Drawables.Framebuffers;
	using NthDimension.Rendering.Drawables.Models;
	using NthDimension.Rendering.Drawables.Lights;
	using NthDimension.Rendering.GameViews;
	using NthDimension.Rendering.Geometry;
	using NthDimension.Rendering.Serialization;
	using NthDimension.Rendering.Sound;
	using NthDimension.Rendering.Utilities;
	// Rasterizer (Low-level GPU Access for UI) library
	using NthDimension.Rasterizer;
	using NthDimension.Rasterizer.NanoVG;
	using NthDimension.Rasterizer.Windows;
	// Forms Graphical User Interface library 
	using NthDimension.Forms.Events;
	using NthDimension.Forms;
	// Ethernet Server-Client API library
	using NthDimension.Network;
	// General Purpose Utilities library
	using NthDimension.Utilities;

	public partial class Application : ApplicationBase
	{

		#region Ctor
		public Application(ApplicationSettings settings)
		{
		
		}
		#endregion

		protected override void OnUpdateFrame(OpenTK.FrameEventArgs e)
		{
			base.OnUpdateFrame(e);
		}
		
		protected override void OnRenderFrame(OpenTK.FrameEventArgs e)
		{
			base.OnRenderFrame(e);
		}
	
	}

} 
