using System;
using System.Diagnostics;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL context property.
	/// </summary>
	/// <remarks> An OpenCL context property is a (name, value) data pair. </remarks>
	public class ComputeContextProperty
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeContextPropertyName name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly IntPtr value;

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeContextPropertyName" /> of the <see cref="T:NthDimension.Compute.ComputeContextProperty" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeContextPropertyName" /> of the <see cref="T:NthDimension.Compute.ComputeContextProperty" />. </value>
		public ComputeContextPropertyName Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Gets the value of the <see cref="T:NthDimension.Compute.ComputeContextProperty" />.
		/// </summary>
		/// <value> The value of the <see cref="T:NthDimension.Compute.ComputeContextProperty" />. </value>
		public IntPtr Value
		{
			get
			{
				return value;
			}
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeContextProperty" />.
		/// </summary>
		/// <param name="name"> The name of the <see cref="T:NthDimension.Compute.ComputeContextProperty" />. </param>
		/// <param name="value"> The value of the created <see cref="T:NthDimension.Compute.ComputeContextProperty" />. </param>
		public ComputeContextProperty(ComputeContextPropertyName name, IntPtr value)
		{
			this.name = name;
			this.value = value;
		}

		/// <summary>
		/// Gets the string representation of the <see cref="T:NthDimension.Compute.ComputeContextProperty" />.
		/// </summary>
		/// <returns> The string representation of the <see cref="T:NthDimension.Compute.ComputeContextProperty" />. </returns>
		public override string ToString()
		{
			return GetType().Name + "(" + name + ", " + value + ")";
		}
	}
}
