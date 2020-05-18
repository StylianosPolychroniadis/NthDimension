using NthDimension.Compute.Bindings;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents a list of OpenCL generated or user created events.
	/// </summary>
	/// <seealso cref="T:NthDimension.Compute.ComputeCommandQueue" />
	public class ComputeEventList : IList<ComputeEventBase>, ICollection<ComputeEventBase>, IEnumerable<ComputeEventBase>, IEnumerable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		private readonly List<ComputeEventBase> events;

		/// <summary>
		/// Gets the last <see cref="T:NthDimension.Compute.ComputeEventBase" /> on the list.
		/// </summary>
		/// <value> The last <see cref="T:NthDimension.Compute.ComputeEventBase" /> on the list. </value>
		public ComputeEventBase Last
		{
			get
			{
				return events[events.Count - 1];
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ComputeEventBase this[int index]
		{
			get
			{
				return events[index];
			}
			set
			{
				events[index] = value;
			}
		}

		/// <summary>
		///
		/// </summary>
		public int Count
		{
			get
			{
				return events.Count;
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
		/// Creates an empty <see cref="T:NthDimension.Compute.ComputeEventList" />.
		/// </summary>
		public ComputeEventList()
		{
			events = new List<ComputeEventBase>();
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeEventList" /> from an existing list of <see cref="T:NthDimension.Compute.ComputeEventBase" />s.
		/// </summary>
		/// <param name="events"> A list of <see cref="T:NthDimension.Compute.ComputeEventBase" />s. </param>
		public ComputeEventList(IList<ComputeEventBase> events)
		{
			events = new Collection<ComputeEventBase>(events);
		}

		/// <summary>
		/// Waits on the host thread for the specified events to complete.
		/// </summary>
		/// <param name="events"> The events to be waited for completition. </param>
		public static void Wait(ICollection<ComputeEventBase> events)
		{
			int handleCount;
			CLEventHandle[] event_list = Tools.ExtractHandles(events, out handleCount);
			ComputeErrorCode errorCode = CL10.WaitForEvents(handleCount, event_list);
			ComputeException.ThrowOnError(errorCode);
		}

		/// <summary>
		/// Waits on the host thread for the <see cref="T:NthDimension.Compute.ComputeEventBase" />s in the <see cref="T:NthDimension.Compute.ComputeEventList" /> to complete.
		/// </summary>
		public void Wait()
		{
			Wait(events);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int IndexOf(ComputeEventBase item)
		{
			return events.IndexOf(item);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		public void Insert(int index, ComputeEventBase item)
		{
			events.Insert(index, item);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			events.RemoveAt(index);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="item"></param>
		public void Add(ComputeEventBase item)
		{
			events.Add(item);
		}

		/// <summary>
		///
		/// </summary>
		public void Clear()
		{
			events.Clear();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(ComputeEventBase item)
		{
			return events.Contains(item);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(ComputeEventBase[] array, int arrayIndex)
		{
			events.CopyTo(array, arrayIndex);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(ComputeEventBase item)
		{
			return events.Remove(item);
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ComputeEventBase> GetEnumerator()
		{
			return ((IEnumerable<ComputeEventBase>)events).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)events).GetEnumerator();
		}
	}
}
