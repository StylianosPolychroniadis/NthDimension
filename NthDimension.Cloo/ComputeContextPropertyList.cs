using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents a list of <see cref="T:NthDimension.Compute.ComputeContextProperty" />s.
	/// </summary>
	/// <remarks> A <see cref="T:NthDimension.Compute.ComputeContextPropertyList" /> is used to specify the properties of a <see cref="T:NthDimension.Compute.ComputeContext" />. </remarks>
	/// <seealso cref="T:NthDimension.Compute.ComputeContext" />
	/// <seealso cref="T:NthDimension.Compute.ComputeContextProperty" />
	public class ComputeContextPropertyList : ICollection<ComputeContextProperty>, IEnumerable<ComputeContextProperty>, IEnumerable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<ComputeContextProperty> properties;

		/// <summary>
		///
		/// </summary>
		public int Count
		{
			get
			{
				return properties.Count;
			}
		}

		/// <summary>
		///
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeContextPropertyList" /> which contains a single item specifying a <see cref="T:NthDimension.Compute.ComputePlatform" />.
		/// </summary>
		/// <param name="platform"> A <see cref="T:NthDimension.Compute.ComputePlatform" />. </param>
		public ComputeContextPropertyList(ComputePlatform platform)
		{
			properties = new List<ComputeContextProperty>();
			properties.Add(new ComputeContextProperty(ComputeContextPropertyName.Platform, platform.Handle.Value));
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeContextPropertyList" /> which contains the specified <see cref="T:NthDimension.Compute.ComputeContextProperty" />s.
		/// </summary>
		/// <param name="properties"> An enumerable of <see cref="T:NthDimension.Compute.ComputeContextProperty" />'s. </param>
		public ComputeContextPropertyList(IEnumerable<ComputeContextProperty> properties)
		{
			this.properties = new List<ComputeContextProperty>(properties);
		}

		/// <summary>
		/// Gets a <see cref="T:NthDimension.Compute.ComputeContextProperty" /> of a specified <c>ComputeContextPropertyName</c>.
		/// </summary>
		/// <param name="name"> The <see cref="T:NthDimension.Compute.ComputeContextPropertyName" /> of the <see cref="T:NthDimension.Compute.ComputeContextProperty" />. </param>
		/// <returns> The requested <see cref="T:NthDimension.Compute.ComputeContextProperty" /> or <c>null</c> if no such <see cref="T:NthDimension.Compute.ComputeContextProperty" /> exists in the <see cref="T:NthDimension.Compute.ComputeContextPropertyList" />. </returns>
		public ComputeContextProperty GetByName(ComputeContextPropertyName name)
		{
			foreach (ComputeContextProperty property in properties)
			{
				if (property.Name == name)
				{
					return property;
				}
			}
			return null;
		}

		internal IntPtr[] ToIntPtrArray()
		{
			IntPtr[] array = new IntPtr[2 * properties.Count + 1];
			for (int i = 0; i < properties.Count; i++)
			{
				array[2 * i] = new IntPtr((int)properties[i].Name);
				array[2 * i + 1] = properties[i].Value;
			}
			array[array.Length - 1] = IntPtr.Zero;
			return array;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="item"></param>
		public void Add(ComputeContextProperty item)
		{
			properties.Add(item);
		}

		/// <summary>
		///
		/// </summary>
		public void Clear()
		{
			properties.Clear();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(ComputeContextProperty item)
		{
			return properties.Contains(item);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(ComputeContextProperty[] array, int arrayIndex)
		{
			properties.CopyTo(array, arrayIndex);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(ComputeContextProperty item)
		{
			return properties.Remove(item);
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ComputeContextProperty> GetEnumerator()
		{
			return properties.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)properties).GetEnumerator();
		}
	}
}
