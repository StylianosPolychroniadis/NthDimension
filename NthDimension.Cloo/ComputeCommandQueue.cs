#define TRACE
using NthDimension.Compute.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL command queue.
	/// </summary>
	/// <remarks> A command queue is an object that holds commands that will be executed on a specific device. The command queue is created on a specific device in a context. Commands to a command queue are queued in-order but may be executed in-order or out-of-order. </remarks>
	/// <seealso cref="T:NthDimension.Compute.ComputeContext" />
	/// <seealso cref="T:NthDimension.Compute.ComputeDevice" />
	public class ComputeCommandQueue : ComputeResource
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeContext context;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeDevice device;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool outOfOrderExec;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool profiling;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal IList<ComputeEventBase> Events;

		/// <summary>
		/// The handle of the <see cref="T:NthDimension.Compute.ComputeCommandQueue" />.
		/// </summary>
		public CLCommandQueueHandle Handle
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeContext" /> of the <see cref="T:NthDimension.Compute.ComputeCommandQueue" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeContext" /> of the <see cref="T:NthDimension.Compute.ComputeCommandQueue" />. </value>
		public ComputeContext Context
		{
			get
			{
				return context;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeDevice" /> of the <see cref="T:NthDimension.Compute.ComputeCommandQueue" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeDevice" /> of the <see cref="T:NthDimension.Compute.ComputeCommandQueue" />. </value>
		public ComputeDevice Device
		{
			get
			{
				return device;
			}
		}

		/// <summary>
		/// Gets the out-of-order execution mode of the commands in the <see cref="T:NthDimension.Compute.ComputeCommandQueue" />.
		/// </summary>
		/// <value> Is <c>true</c> if <see cref="T:NthDimension.Compute.ComputeCommandQueue" /> has out-of-order execution mode enabled and <c>false</c> otherwise. </value>
		public bool OutOfOrderExecution
		{
			get
			{
				return outOfOrderExec;
			}
		}

		/// <summary>
		/// Gets the profiling mode of the commands in the <see cref="T:NthDimension.Compute.ComputeCommandQueue" />.
		/// </summary>
		/// <value> Is <c>true</c> if <see cref="T:NthDimension.Compute.ComputeCommandQueue" /> has profiling enabled and <c>false</c> otherwise. </value>
		public bool Profiling
		{
			get
			{
				return profiling;
			}
		}

		/// <summary>
		/// Enqueues a command to copy data from a source buffer to a destination buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffers. </typeparam>
		/// <param name="source"> The buffer to copy from. </param>
		/// <param name="destination"> The buffer to copy to. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyBuffer<T>(ComputeBufferBase<T> source, ComputeBufferBase<T> destination, ICollection<ComputeEventBase> events) where T : struct
		{
			Copy(source, destination, 0L, 0L, source.Count, events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a source buffer to a destination buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffers. </typeparam>
		/// <param name="source"> The buffer to copy from. </param>
		/// <param name="destination"> The buffer to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyBuffer<T>(ComputeBufferBase<T> source, ComputeBufferBase<T> destination, long sourceOffset, long destinationOffset, long region, ICollection<ComputeEventBase> events) where T : struct
		{
			Copy(source, destination, sourceOffset, destinationOffset, region, events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a source buffer to a destination buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffers. </typeparam>
		/// <param name="source"> The buffer to copy from. </param>
		/// <param name="destination"> The buffer to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyBuffer<T>(ComputeBufferBase<T> source, ComputeBufferBase<T> destination, SysIntX2 sourceOffset, SysIntX2 destinationOffset, SysIntX2 region, ICollection<ComputeEventBase> events) where T : struct
		{
			Copy(source, destination, new SysIntX3(sourceOffset, 0L), new SysIntX3(destinationOffset, 0L), new SysIntX3(region, 1L), 0L, 0L, 0L, 0L, events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a source buffer to a destination buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffers. </typeparam>
		/// <param name="source"> The buffer to copy from. </param>
		/// <param name="destination"> The buffer to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyBuffer<T>(ComputeBufferBase<T> source, ComputeBufferBase<T> destination, SysIntX3 sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, ICollection<ComputeEventBase> events) where T : struct
		{
			Copy(source, destination, sourceOffset, destinationOffset, region, 0L, 0L, 0L, 0L, events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a source buffer to a destination buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffers. </typeparam>
		/// <param name="source"> The buffer to copy from. </param>
		/// <param name="destination"> The buffer to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="sourceRowPitch"> The size of a row of elements of <paramref name="source" /> in bytes. </param>
		/// <param name="destinationRowPitch"> The size of a row of elements of <paramref name="destination" /> in bytes. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyBuffer<T>(ComputeBufferBase<T> source, ComputeBufferBase<T> destination, SysIntX2 sourceOffset, SysIntX2 destinationOffset, SysIntX2 region, long sourceRowPitch, long destinationRowPitch, ICollection<ComputeEventBase> events) where T : struct
		{
			Copy(source, destination, new SysIntX3(sourceOffset, 0L), new SysIntX3(destinationOffset, 0L), new SysIntX3(region, 1L), sourceRowPitch, 0L, destinationRowPitch, 0L, events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a source buffer to a destination buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffers. </typeparam>
		/// <param name="source"> The buffer to copy from. </param>
		/// <param name="destination"> The buffer to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="sourceRowPitch"> The size of a row of elements of <paramref name="source" /> in bytes. </param>
		/// <param name="destinationRowPitch"> The size of a row of elements of <paramref name="destination" /> in bytes. </param>
		/// <param name="sourceSlicePitch"> The size of a 2D slice of elements of <paramref name="source" /> in bytes. </param>
		/// <param name="destinationSlicePitch"> The size of a 2D slice of elements of <paramref name="destination" /> in bytes. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyBuffer<T>(ComputeBufferBase<T> source, ComputeBufferBase<T> destination, SysIntX3 sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, long sourceRowPitch, long destinationRowPitch, long sourceSlicePitch, long destinationSlicePitch, ICollection<ComputeEventBase> events) where T : struct
		{
			Copy(source, destination, sourceOffset, destinationOffset, region, sourceRowPitch, sourceSlicePitch, destinationRowPitch, destinationSlicePitch, events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a buffer to an image.
		/// </summary>
		/// <typeparam name="T"> The type of data in <paramref name="source" />. </typeparam>
		/// <param name="source"> The buffer to copy from. </param>
		/// <param name="destination"> The image to copy to. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyBufferToImage<T>(ComputeBufferBase<T> source, ComputeImage destination, ICollection<ComputeEventBase> events) where T : struct
		{
			Copy(source, destination, 0L, default(SysIntX3), new SysIntX3(destination.Width, destination.Height, (destination.Depth == 0) ? 1 : destination.Depth), events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a buffer to an image.
		/// </summary>
		/// <typeparam name="T"> The type of data in <paramref name="source" />. </typeparam>
		/// <param name="source"> The buffer to copy from. </param>
		/// <param name="destination"> The image to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyBufferToImage<T>(ComputeBufferBase<T> source, ComputeImage2D destination, long sourceOffset, SysIntX2 destinationOffset, SysIntX2 region, ICollection<ComputeEventBase> events) where T : struct
		{
			Copy(source, destination, sourceOffset, new SysIntX3(destinationOffset, 0L), new SysIntX3(region, 1L), events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a buffer to an image.
		/// </summary>
		/// <typeparam name="T"> The type of data in <paramref name="source" />. </typeparam>
		/// <param name="source"> The buffer to copy from. </param>
		/// <param name="destination"> The image to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyBufferToImage<T>(ComputeBufferBase<T> source, ComputeImage3D destination, long sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, ICollection<ComputeEventBase> events) where T : struct
		{
			Copy(source, destination, sourceOffset, destinationOffset, region, events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a source image to a destination image.
		/// </summary>
		/// <param name="source"> The image to copy from. </param>
		/// <param name="destination"> The image to copy to. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyImage(ComputeImage source, ComputeImage destination, ICollection<ComputeEventBase> events)
		{
			Copy(source, destination, default(SysIntX3), default(SysIntX3), new SysIntX3(source.Width, source.Height, (source.Depth == 0) ? 1 : source.Depth), events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a source image to a destination image.
		/// </summary>
		/// <param name="source"> The image to copy from. </param>
		/// <param name="destination"> The image to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyImage(ComputeImage2D source, ComputeImage2D destination, SysIntX2 sourceOffset, SysIntX2 destinationOffset, SysIntX2 region, ICollection<ComputeEventBase> events)
		{
			Copy(source, destination, new SysIntX3(sourceOffset, 0L), new SysIntX3(destinationOffset, 0L), new SysIntX3(region, 1L), events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a source image to a destination image.
		/// </summary>
		/// <param name="source"> The image to copy from. </param>
		/// <param name="destination"> The image to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyImage(ComputeImage2D source, ComputeImage3D destination, SysIntX2 sourceOffset, SysIntX3 destinationOffset, SysIntX2 region, ICollection<ComputeEventBase> events)
		{
			Copy(source, destination, new SysIntX3(sourceOffset, 0L), destinationOffset, new SysIntX3(region, 1L), events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a source image to a destination image.
		/// </summary>
		/// <param name="source"> The image to copy from. </param>
		/// <param name="destination"> The image to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyImage(ComputeImage3D source, ComputeImage2D destination, SysIntX3 sourceOffset, SysIntX2 destinationOffset, SysIntX2 region, ICollection<ComputeEventBase> events)
		{
			Copy(source, destination, sourceOffset, new SysIntX3(destinationOffset, 0L), new SysIntX3(region, 1L), null);
		}

		/// <summary>
		/// Enqueues a command to copy data from a source image to a destination image.
		/// </summary>
		/// <param name="source"> The image to copy from. </param>
		/// <param name="destination"> The image to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyImage(ComputeImage3D source, ComputeImage3D destination, SysIntX3 sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, ICollection<ComputeEventBase> events)
		{
			Copy(source, destination, sourceOffset, destinationOffset, region, events);
		}

		/// <summary>
		/// Enqueues a command to copy data from an image to a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in <paramref name="destination" />. </typeparam>
		/// <param name="source"> The image to copy from. </param>
		/// <param name="destination"> The buffer to copy to. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyImageToBuffer<T>(ComputeImage source, ComputeBufferBase<T> destination, ICollection<ComputeEventBase> events) where T : struct
		{
			Copy(source, destination, default(SysIntX3), 0L, new SysIntX3(source.Width, source.Height, (source.Depth == 0) ? 1 : source.Depth), events);
		}

		/// <summary>
		/// Enqueues a command to copy data from an image to a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in <paramref name="destination" />. </typeparam>
		/// <param name="source"> The image to copy from. </param>
		/// <param name="destination"> The buffer to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyImageToBuffer<T>(ComputeImage2D source, ComputeBufferBase<T> destination, SysIntX2 sourceOffset, long destinationOffset, SysIntX2 region, ICollection<ComputeEventBase> events) where T : struct
		{
			Copy(source, destination, new SysIntX3(sourceOffset, 0L), destinationOffset, new SysIntX3(region, 1L), events);
		}

		/// <summary>
		/// Enqueues a command to copy data from a 3D image to a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in <paramref name="destination" />. </typeparam>
		/// <param name="source"> The image to copy from. </param>
		/// <param name="destination"> The buffer to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void CopyImageToBuffer<T>(ComputeImage3D source, ComputeBufferBase<T> destination, SysIntX3 sourceOffset, long destinationOffset, SysIntX3 region, ICollection<ComputeEventBase> events) where T : struct
		{
			Copy(source, destination, sourceOffset, destinationOffset, region, events);
		}

		/// <summary>
		/// Enqueues a command to read data from a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffer. </typeparam>
		/// <param name="source"> The buffer to read from. </param>
		/// <param name="destination"> The array to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void ReadFromBuffer<T>(ComputeBufferBase<T> source, ref T[] destination, bool blocking, IList<ComputeEventBase> events) where T : struct
		{
			ReadFromBuffer(source, ref destination, blocking, 0L, 0L, source.Count, events);
		}

		/// <summary>
		/// Enqueues a command to read data from a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffer. </typeparam>
		/// <param name="source"> The buffer to read from. </param>
		/// <param name="destination"> The array to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to read. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void ReadFromBuffer<T>(ComputeBufferBase<T> source, ref T[] destination, bool blocking, long sourceOffset, long destinationOffset, long region, IList<ComputeEventBase> events) where T : struct
		{
			GCHandle handle = GCHandle.Alloc(destination, GCHandleType.Pinned);
			IntPtr destination2 = Marshal.UnsafeAddrOfPinnedArrayElement(destination, (int)destinationOffset);
			if (blocking)
			{
				Read(source, blocking, sourceOffset, region, destination2, events);
				handle.Free();
				return;
			}
			IList<ComputeEventBase> list = (events != null && !events.IsReadOnly) ? events : Events;
			Read(source, blocking, sourceOffset, region, destination2, list);
			ComputeEvent computeEvent = (ComputeEvent)list[list.Count - 1];
			computeEvent.TrackGCHandle(handle);
		}

		/// <summary>
		/// Enqueues a command to read data from a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffer. </typeparam>
		/// <param name="source"> The buffer to read from. </param>
		/// <param name="destination"> The array to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to read. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void ReadFromBuffer<T>(ComputeBufferBase<T> source, ref T[,] destination, bool blocking, SysIntX2 sourceOffset, SysIntX2 destinationOffset, SysIntX2 region, IList<ComputeEventBase> events) where T : struct
		{
			ReadFromBuffer(source, ref destination, blocking, sourceOffset, destinationOffset, region, 0L, 0L, events);
		}

		/// <summary>
		/// Enqueues a command to read data from a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffer. </typeparam>
		/// <param name="source"> The buffer to read from. </param>
		/// <param name="destination"> The array to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to read. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void ReadFromBuffer<T>(ComputeBufferBase<T> source, ref T[,,] destination, bool blocking, SysIntX3 sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, IList<ComputeEventBase> events) where T : struct
		{
			ReadFromBuffer(source, ref destination, blocking, sourceOffset, destinationOffset, region, 0L, 0L, 0L, 0L, events);
		}

		/// <summary>
		/// Enqueues a command to read data from a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffer. </typeparam>
		/// <param name="source"> The buffer to read from. </param>
		/// <param name="destination"> The array to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to read. </param>
		/// <param name="sourceRowPitch"> The size of a row of elements of <paramref name="source" /> in bytes. </param>
		/// <param name="destinationRowPitch"> The size of a row of elements of <paramref name="destination" /> in bytes. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void ReadFromBuffer<T>(ComputeBufferBase<T> source, ref T[,] destination, bool blocking, SysIntX2 sourceOffset, SysIntX2 destinationOffset, SysIntX2 region, long sourceRowPitch, long destinationRowPitch, IList<ComputeEventBase> events) where T : struct
		{
			GCHandle handle = GCHandle.Alloc(destination, GCHandleType.Pinned);
			if (blocking)
			{
				Read(source, blocking, new SysIntX3(sourceOffset, 0L), new SysIntX3(destinationOffset, 0L), new SysIntX3(region, 1L), sourceRowPitch, 0L, destinationRowPitch, 0L, handle.AddrOfPinnedObject(), events);
				handle.Free();
				return;
			}
			IList<ComputeEventBase> list = (events != null && !events.IsReadOnly) ? events : Events;
			Read(source, blocking, new SysIntX3(sourceOffset, 0L), new SysIntX3(destinationOffset, 0L), new SysIntX3(region, 1L), sourceRowPitch, 0L, destinationRowPitch, 0L, handle.AddrOfPinnedObject(), list);
			ComputeEvent computeEvent = (ComputeEvent)list[list.Count - 1];
			computeEvent.TrackGCHandle(handle);
		}

		/// <summary>
		/// Enqueues a command to read data from a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffer. </typeparam>
		/// <param name="source"> The buffer to read from. </param>
		/// <param name="destination"> The array to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to read. </param>
		/// <param name="sourceRowPitch"> The size of a row of elements of <paramref name="source" /> in bytes. </param>
		/// <param name="destinationRowPitch"> The size of a row of elements of <paramref name="destination" /> in bytes. </param>
		/// <param name="sourceSlicePitch"> The size of a 2D slice of elements of <paramref name="source" /> in bytes. </param>
		/// <param name="destinationSlicePitch"> The size of a 2D slice of elements of <paramref name="destination" /> in bytes. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void ReadFromBuffer<T>(ComputeBufferBase<T> source, ref T[,,] destination, bool blocking, SysIntX3 sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, long sourceRowPitch, long destinationRowPitch, long sourceSlicePitch, long destinationSlicePitch, IList<ComputeEventBase> events) where T : struct
		{
			GCHandle handle = GCHandle.Alloc(destination, GCHandleType.Pinned);
			if (blocking)
			{
				Read(source, blocking, sourceOffset, destinationOffset, region, sourceRowPitch, sourceSlicePitch, destinationRowPitch, destinationSlicePitch, handle.AddrOfPinnedObject(), events);
				handle.Free();
				return;
			}
			IList<ComputeEventBase> list = (events != null && !events.IsReadOnly) ? events : Events;
			Read(source, blocking, sourceOffset, destinationOffset, region, sourceRowPitch, sourceSlicePitch, destinationRowPitch, destinationSlicePitch, handle.AddrOfPinnedObject(), list);
			ComputeEvent computeEvent = (ComputeEvent)list[list.Count - 1];
			computeEvent.TrackGCHandle(handle);
		}

		/// <summary>
		/// Enqueues a command to read data from an image.
		/// </summary>
		/// <param name="source"> The image to read from. </param>
		/// <param name="destination"> A valid pointer to a preallocated memory area to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void ReadFromImage(ComputeImage source, IntPtr destination, bool blocking, ICollection<ComputeEventBase> events)
		{
			Read(source, blocking, default(SysIntX3), new SysIntX3(source.Width, source.Height, (source.Depth == 0) ? 1 : source.Depth), 0L, 0L, destination, events);
		}

		/// <summary>
		/// Enqueues a command to read data from an image.
		/// </summary>
		/// <param name="source"> The image to read from. </param>
		/// <param name="destination"> A valid pointer to a preallocated memory area to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="region"> The region of elements to read. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void ReadFromImage(ComputeImage2D source, IntPtr destination, bool blocking, SysIntX2 sourceOffset, SysIntX2 region, ICollection<ComputeEventBase> events)
		{
			Read(source, blocking, new SysIntX3(sourceOffset, 0L), new SysIntX3(region, 1L), 0L, 0L, destination, events);
		}

		/// <summary>
		/// Enqueues a command to read data from an image.
		/// </summary>
		/// <param name="source"> The image to read from. </param>
		/// <param name="destination"> A valid pointer to a preallocated memory area to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="region"> The region of elements to read. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void ReadFromImage(ComputeImage3D source, IntPtr destination, bool blocking, SysIntX3 sourceOffset, SysIntX3 region, ICollection<ComputeEventBase> events)
		{
			Read(source, blocking, sourceOffset, region, 0L, 0L, destination, events);
		}

		/// <summary>
		/// Enqueues a command to read data from an image.
		/// </summary>
		/// <param name="source"> The image to read from. </param>
		/// <param name="destination"> A valid pointer to a preallocated memory area to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="region"> The region of elements to read. </param>
		/// <param name="sourceRowPitch"> The size of a row of pixels of <paramref name="destination" /> in bytes. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void ReadFromImage(ComputeImage2D source, IntPtr destination, bool blocking, SysIntX2 sourceOffset, SysIntX2 region, long sourceRowPitch, ICollection<ComputeEventBase> events)
		{
			Read(source, blocking, new SysIntX3(sourceOffset, 0L), new SysIntX3(region, 1L), sourceRowPitch, 0L, destination, events);
		}

		/// <summary>
		/// Enqueues a command to read data from an image.
		/// </summary>
		/// <param name="source"> The image to read from. </param>
		/// <param name="destination"> A valid pointer to a preallocated memory area to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="region"> The region of elements to read. </param>
		/// <param name="sourceRowPitch"> The size of a row of pixels of <paramref name="destination" /> in bytes. </param>
		/// <param name="sourceSlicePitch"> The size of a 2D slice of pixels of <paramref name="destination" /> in bytes. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void ReadFromImage(ComputeImage3D source, IntPtr destination, bool blocking, SysIntX3 sourceOffset, SysIntX3 region, long sourceRowPitch, long sourceSlicePitch, ICollection<ComputeEventBase> events)
		{
			Read(source, blocking, sourceOffset, region, sourceRowPitch, sourceSlicePitch, destination, events);
		}

		/// <summary>
		/// Enqueues a command to write data to a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffer. </typeparam>
		/// <param name="source"> The array to read from. </param>
		/// <param name="destination"> The buffer to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void WriteToBuffer<T>(T[] source, ComputeBufferBase<T> destination, bool blocking, IList<ComputeEventBase> events) where T : struct
		{
			WriteToBuffer(source, destination, blocking, 0L, 0L, destination.Count, events);
		}

		/// <summary>
		/// Enqueues a command to write data to a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffer. </typeparam>
		/// <param name="source"> The array to read from. </param>
		/// <param name="destination"> The buffer to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to write. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void WriteToBuffer<T>(T[] source, ComputeBufferBase<T> destination, bool blocking, long sourceOffset, long destinationOffset, long region, IList<ComputeEventBase> events) where T : struct
		{
			GCHandle handle = GCHandle.Alloc(source, GCHandleType.Pinned);
			IntPtr source2 = Marshal.UnsafeAddrOfPinnedArrayElement(source, (int)sourceOffset);
			if (blocking)
			{
				Write(destination, blocking, destinationOffset, region, source2, events);
				handle.Free();
				return;
			}
			IList<ComputeEventBase> list = (events != null && !events.IsReadOnly) ? events : Events;
			Write(destination, blocking, destinationOffset, region, source2, list);
			ComputeEvent computeEvent = (ComputeEvent)list[list.Count - 1];
			computeEvent.TrackGCHandle(handle);
		}

		/// <summary>
		/// Enqueues a command to write data to a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffer. </typeparam>
		/// <param name="source"> The array to read from. </param>
		/// <param name="destination"> The buffer to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to write. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void WriteToBuffer<T>(T[,] source, ComputeBufferBase<T> destination, bool blocking, SysIntX2 sourceOffset, SysIntX2 destinationOffset, SysIntX2 region, IList<ComputeEventBase> events) where T : struct
		{
			WriteToBuffer(source, destination, blocking, sourceOffset, destinationOffset, region, 0L, 0L, events);
		}

		/// <summary>
		/// Enqueues a command to write data to a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffer. </typeparam>
		/// <param name="source"> The array to read from. </param>
		/// <param name="destination"> The buffer to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to write. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void WriteToBuffer<T>(T[,,] source, ComputeBufferBase<T> destination, bool blocking, SysIntX3 sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, IList<ComputeEventBase> events) where T : struct
		{
			WriteToBuffer(source, destination, blocking, sourceOffset, destinationOffset, region, 0L, 0L, 0L, 0L, events);
		}

		/// <summary>
		/// Enqueues a command to write data to a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffer. </typeparam>
		/// <param name="source"> The array to read from. </param>
		/// <param name="destination"> The buffer to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to write. </param>
		/// <param name="sourceRowPitch"> The size of a row of elements of <paramref name="source" /> in bytes. </param>
		/// <param name="destinationRowPitch"> The size of a row of elements of <paramref name="destination" /> in bytes. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void WriteToBuffer<T>(T[,] source, ComputeBufferBase<T> destination, bool blocking, SysIntX2 sourceOffset, SysIntX2 destinationOffset, SysIntX2 region, long sourceRowPitch, long destinationRowPitch, IList<ComputeEventBase> events) where T : struct
		{
			GCHandle handle = GCHandle.Alloc(source, GCHandleType.Pinned);
			if (blocking)
			{
				Write(destination, blocking, new SysIntX3(sourceOffset, 0L), new SysIntX3(destinationOffset, 0L), new SysIntX3(region, 1L), sourceRowPitch, 0L, destinationRowPitch, 0L, handle.AddrOfPinnedObject(), events);
				handle.Free();
				return;
			}
			IList<ComputeEventBase> list = (events != null && !events.IsReadOnly) ? events : Events;
			Write(destination, blocking, new SysIntX3(sourceOffset, 0L), new SysIntX3(destinationOffset, 0L), new SysIntX3(region, 1L), sourceRowPitch, 0L, destinationRowPitch, 0L, handle.AddrOfPinnedObject(), list);
			ComputeEvent computeEvent = (ComputeEvent)list[list.Count - 1];
			computeEvent.TrackGCHandle(handle);
		}

		/// <summary>
		/// Enqueues a command to write data to a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffer. </typeparam>
		/// <param name="source"> The array to read from. </param>
		/// <param name="destination"> The buffer to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to write. </param>
		/// <param name="sourceRowPitch"> The size of a row of elements of <paramref name="source" /> in bytes. </param>
		/// <param name="destinationRowPitch"> The size of a row of elements of <paramref name="destination" /> in bytes. </param>
		/// <param name="sourceSlicePitch"> The size of a 2D slice of elements of <paramref name="source" /> in bytes. </param>
		/// <param name="destinationSlicePitch"> The size of a 2D slice of elements of <paramref name="destination" /> in bytes. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void WriteToBuffer<T>(T[,,] source, ComputeBufferBase<T> destination, bool blocking, SysIntX3 sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, long sourceRowPitch, long destinationRowPitch, long sourceSlicePitch, long destinationSlicePitch, IList<ComputeEventBase> events) where T : struct
		{
			GCHandle handle = GCHandle.Alloc(source, GCHandleType.Pinned);
			if (blocking)
			{
				Write(destination, blocking, sourceOffset, destinationOffset, region, sourceRowPitch, sourceSlicePitch, destinationRowPitch, destinationSlicePitch, handle.AddrOfPinnedObject(), events);
				handle.Free();
				return;
			}
			IList<ComputeEventBase> list = (events != null && !events.IsReadOnly) ? events : Events;
			Write(destination, blocking, sourceOffset, destinationOffset, region, sourceRowPitch, sourceSlicePitch, destinationRowPitch, destinationSlicePitch, handle.AddrOfPinnedObject(), list);
			ComputeEvent computeEvent = (ComputeEvent)list[list.Count - 1];
			computeEvent.TrackGCHandle(handle);
		}

		/// <summary>
		/// Enqueues a command to write data to an image.
		/// </summary>
		/// <param name="source"> A pointer to a memory area to read from. </param>
		/// <param name="destination"> The image to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void WriteToImage(IntPtr source, ComputeImage destination, bool blocking, ICollection<ComputeEventBase> events)
		{
			Write(destination, blocking, default(SysIntX3), new SysIntX3(destination.Width, destination.Height, (destination.Depth == 0) ? 1 : destination.Depth), 0L, 0L, source, events);
		}

		/// <summary>
		/// Enqueues a command to write data to an image.
		/// </summary>
		/// <param name="source"> A pointer to a memory area to read from. </param>
		/// <param name="destination"> The image to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to write. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void WriteToImage(IntPtr source, ComputeImage2D destination, bool blocking, SysIntX2 destinationOffset, SysIntX2 region, ICollection<ComputeEventBase> events)
		{
			Write(destination, blocking, new SysIntX3(destinationOffset, 0L), new SysIntX3(region, 1L), 0L, 0L, source, events);
		}

		/// <summary>
		/// Enqueues a command to write data to an image.
		/// </summary>
		/// <param name="source"> A pointer to a memory area to read from. </param>
		/// <param name="destination"> The image to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to write. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void WriteToImage(IntPtr source, ComputeImage3D destination, bool blocking, SysIntX3 destinationOffset, SysIntX3 region, ICollection<ComputeEventBase> events)
		{
			Write(destination, blocking, destinationOffset, region, 0L, 0L, source, events);
		}

		/// <summary>
		/// Enqueues a command to write data to an image.
		/// </summary>
		/// <param name="source"> A pointer to a memory area to read from. </param>
		/// <param name="destination"> The image to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to write. </param>
		/// <param name="destinationRowPitch"> The size of a row of pixels of <paramref name="destination" /> in bytes. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void WriteToImage(IntPtr source, ComputeImage2D destination, bool blocking, SysIntX2 destinationOffset, SysIntX2 region, long destinationRowPitch, ICollection<ComputeEventBase> events)
		{
			Write(destination, blocking, new SysIntX3(destinationOffset, 0L), new SysIntX3(region, 1L), destinationRowPitch, 0L, source, events);
		}

		/// <summary>
		/// Enqueues a command to write data to an image.
		/// </summary>
		/// <param name="source"> A pointer to a memory area to read from. </param>
		/// <param name="destination"> The image to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to write. </param>
		/// <param name="destinationRowPitch"> The size of a row of pixels of <paramref name="destination" /> in bytes. </param>
		/// <param name="destinationSlicePitch"> The size of a 2D slice of pixels of <paramref name="destination" /> in bytes. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void WriteToImage(IntPtr source, ComputeImage3D destination, bool blocking, SysIntX3 destinationOffset, SysIntX3 region, long destinationRowPitch, long destinationSlicePitch, ICollection<ComputeEventBase> events)
		{
			Write(destination, blocking, destinationOffset, region, destinationRowPitch, destinationSlicePitch, source, events);
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeCommandQueue" />.
		/// </summary>
		/// <param name="context"> A <see cref="T:NthDimension.Compute.ComputeContext" />. </param>
		/// <param name="device"> A <see cref="T:NthDimension.Compute.ComputeDevice" /> associated with the <paramref name="context" />. It can either be one of <see cref="P:Cloo.ComputeContext.Devices" /> or have the same <see cref="T:NthDimension.Compute.ComputeDeviceTypes" /> as the <paramref name="device" /> specified when the <paramref name="context" /> is created. </param>
		/// <param name="properties"> The properties for the <see cref="T:NthDimension.Compute.ComputeCommandQueue" />. </param>
		public ComputeCommandQueue(ComputeContext context, ComputeDevice device, ComputeCommandQueueFlags properties)
		{
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			Handle = CL10.CreateCommandQueue(context.Handle, device.Handle, properties, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			SetID(Handle.Value);
			this.device = device;
			this.context = context;
			outOfOrderExec = ((properties & ComputeCommandQueueFlags.OutOfOrderExecution) == ComputeCommandQueueFlags.OutOfOrderExecution);
			profiling = ((properties & ComputeCommandQueueFlags.Profiling) == ComputeCommandQueueFlags.Profiling);
			Events = new List<ComputeEventBase>();
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
		}

		/// <summary>
		/// Enqueues a command to acquire a collection of <see cref="T:NthDimension.Compute.ComputeMemory" />s that have been previously created from OpenGL objects.
		/// </summary>
		/// <param name="memObjs"> A collection of OpenCL memory objects that correspond to OpenGL objects. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		public void AcquireGLObjects(ICollection<ComputeMemory> memObjs, ICollection<ComputeEventBase> events)
		{
			int handleCount;
			CLMemoryHandle[] mem_objects = Tools.ExtractHandles(memObjs, out handleCount);
			int handleCount2;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount2);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode computeErrorCode = ComputeErrorCode.Success;
			computeErrorCode = CL10.EnqueueAcquireGLObjects(Handle, handleCount, mem_objects, handleCount2, event_wait_list, array);
			ComputeException.ThrowOnError(computeErrorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a barrier.
		/// </summary>
		/// <remarks> A barrier ensures that all queued commands have finished execution before the next batch of commands can begin execution. </remarks>
		public void AddBarrier()
		{
			ComputeErrorCode errorCode = CL10.EnqueueBarrier(Handle);
			ComputeException.ThrowOnError(errorCode);
		}

		/// <summary>
		/// Enqueues a marker.
		/// </summary>
		public ComputeEvent AddMarker()
		{
			CLEventHandle new_event;
			ComputeErrorCode errorCode = CL10.EnqueueMarker(Handle, out new_event);
			ComputeException.ThrowOnError(errorCode);
			return new ComputeEvent(new_event, this);
		}

		/// <summary>
		/// Enqueues a command to copy data between buffers.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffers. </typeparam>
		/// <param name="source"> The buffer to copy from. </param>
		/// <param name="destination"> The buffer to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
		public void Copy<T>(ComputeBufferBase<T> source, ComputeBufferBase<T> destination, long sourceOffset, long destinationOffset, long region, ICollection<ComputeEventBase> events) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL10.EnqueueCopyBuffer(Handle, source.Handle, destination.Handle, new IntPtr(sourceOffset * num), new IntPtr(destinationOffset * num), new IntPtr(region * num), handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a command to copy a 2D or 3D region of elements between two buffers.
		/// </summary>
		/// <typeparam name="T"> The type of data in the buffers. </typeparam>
		/// <param name="source"> The buffer to copy from. </param>
		/// <param name="destination"> The buffer to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="sourceRowPitch"> The size of the source buffer row in bytes. If set to zero then <paramref name="sourceRowPitch" /> equals <c>region.X * sizeof(T)</c>. </param>
		/// <param name="sourceSlicePitch"> The size of the source buffer 2D slice in bytes. If set to zero then <paramref name="sourceSlicePitch" /> equals <c>region.Y * sizeof(T) * sourceRowPitch</c>. </param>
		/// <param name="destinationRowPitch"> The size of the destination buffer row in bytes. If set to zero then <paramref name="destinationRowPitch" /> equals <c>region.X * sizeof(T)</c>. </param>
		/// <param name="destinationSlicePitch"> The size of the destination buffer 2D slice in bytes. If set to zero then <paramref name="destinationSlicePitch" /> equals <c>region.Y * sizeof(T) * destinationRowPitch</c>. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		/// <remarks> Requires OpenCL 1.1. </remarks>
		public void Copy<T>(ComputeBufferBase<T> source, ComputeBufferBase<T> destination, SysIntX3 sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, long sourceRowPitch, long sourceSlicePitch, long destinationRowPitch, long destinationSlicePitch, ICollection<ComputeEventBase> events) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			sourceOffset.X = new IntPtr(num * sourceOffset.X.ToInt64());
			destinationOffset.X = new IntPtr(num * destinationOffset.X.ToInt64());
			region.X = new IntPtr(num * region.X.ToInt64());
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL11.EnqueueCopyBufferRect(Handle, source.Handle, destination.Handle, ref sourceOffset, ref destinationOffset, ref region, new IntPtr(sourceRowPitch), new IntPtr(sourceSlicePitch), new IntPtr(destinationRowPitch), new IntPtr(destinationSlicePitch), handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a command to copy data from buffer to <see cref="T:NthDimension.Compute.ComputeImage" />.
		/// </summary>
		/// <typeparam name="T"> The type of data in <paramref name="source" />. </typeparam>
		/// <param name="source"> The buffer to copy from. </param>
		/// <param name="destination"> The image to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		public void Copy<T>(ComputeBufferBase<T> source, ComputeImage destination, long sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, ICollection<ComputeEventBase> events) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL10.EnqueueCopyBufferToImage(Handle, source.Handle, destination.Handle, new IntPtr(sourceOffset * num), ref destinationOffset, ref region, handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a command to copy data from <see cref="T:NthDimension.Compute.ComputeImage" /> to buffer.
		/// </summary>
		/// <param name="source"> The image to copy from. </param>
		/// <param name="destination"> The buffer to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		public void Copy<T>(ComputeImage source, ComputeBufferBase<T> destination, SysIntX3 sourceOffset, long destinationOffset, SysIntX3 region, ICollection<ComputeEventBase> events) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL10.EnqueueCopyImageToBuffer(Handle, source.Handle, destination.Handle, ref sourceOffset, ref region, new IntPtr(destinationOffset * num), handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a command to copy data between <see cref="T:NthDimension.Compute.ComputeImage" />s.
		/// </summary>
		/// <param name="source"> The <see cref="T:NthDimension.Compute.ComputeImage" /> to copy from. </param>
		/// <param name="destination"> The <see cref="T:NthDimension.Compute.ComputeImage" /> to copy to. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		public void Copy(ComputeImage source, ComputeImage destination, SysIntX3 sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, ICollection<ComputeEventBase> events)
		{
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL10.EnqueueCopyImage(Handle, source.Handle, destination.Handle, ref sourceOffset, ref destinationOffset, ref region, handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a command to execute a single <see cref="T:NthDimension.Compute.ComputeKernel" />.
		/// </summary>
		/// <param name="kernel"> The <see cref="T:NthDimension.Compute.ComputeKernel" /> to execute. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		public void ExecuteTask(ComputeKernel kernel, ICollection<ComputeEventBase> events)
		{
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL10.EnqueueTask(Handle, kernel.Handle, handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a command to execute a range of <see cref="T:NthDimension.Compute.ComputeKernel" />s in parallel.
		/// </summary>
		/// <param name="kernel"> The <see cref="T:NthDimension.Compute.ComputeKernel" /> to execute. </param>
		/// <param name="globalWorkOffset"> An array of values that describe the offset used to calculate the global ID of a work-item instead of having the global IDs always start at offset (0, 0,... 0). </param>
		/// <param name="globalWorkSize"> An array of values that describe the number of global work-items in dimensions that will execute the kernel function. The total number of global work-items is computed as global_work_size[0] *...* global_work_size[work_dim - 1]. </param>
		/// <param name="localWorkSize"> An array of values that describe the number of work-items that make up a work-group (also referred to as the size of the work-group) that will execute the <paramref name="kernel" />. The total number of work-items in a work-group is computed as local_work_size[0] *... * local_work_size[work_dim - 1]. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		public void Execute(ComputeKernel kernel, long[] globalWorkOffset, long[] globalWorkSize, long[] localWorkSize, ICollection<ComputeEventBase> events)
		{
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL10.EnqueueNDRangeKernel(Handle, kernel.Handle, globalWorkSize.Length, Tools.ConvertArray(globalWorkOffset), Tools.ConvertArray(globalWorkSize), Tools.ConvertArray(localWorkSize), handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Blocks until all previously enqueued commands are issued to the <see cref="P:Cloo.ComputeCommandQueue.Device" /> and have completed.
		/// </summary>
		public void Finish()
		{
			ComputeErrorCode errorCode = CL10.Finish(Handle);
			ComputeException.ThrowOnError(errorCode);
		}

		/// <summary>
		/// Issues all previously enqueued commands to the <see cref="P:Cloo.ComputeCommandQueue.Device" />.
		/// </summary>
		/// <remarks> This method only guarantees that all previously enqueued commands get issued to the OpenCL device. There is no guarantee that they will be complete after this method returns. </remarks>
		public void Flush()
		{
			ComputeErrorCode errorCode = CL10.Flush(Handle);
			ComputeException.ThrowOnError(errorCode);
		}

		/// <summary>
		/// Enqueues a command to map a part of a buffer into the host address space.
		/// </summary>
		/// <param name="buffer"> The buffer to map. </param>
		/// <param name="blocking">  The mode of operation of this call. </param>
		/// <param name="flags"> A list of properties for the mapping mode. </param>
		/// <param name="offset"> The <paramref name="buffer" /> element position where mapping starts. </param>
		/// <param name="region"> The region of elements to map. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		/// <remarks> If <paramref name="blocking" /> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking" /> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
		public IntPtr Map<T>(ComputeBufferBase<T> buffer, bool blocking, ComputeMemoryMappingFlags flags, long offset, long region, ICollection<ComputeEventBase> events) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			IntPtr zero = IntPtr.Zero;
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			zero = CL10.EnqueueMapBuffer(Handle, buffer.Handle, blocking, flags, new IntPtr(offset * num), new IntPtr(region * num), handleCount, event_wait_list, array, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
			return zero;
		}

		/// <summary>
		/// Enqueues a command to map a part of a <see cref="T:NthDimension.Compute.ComputeImage" /> into the host address space.
		/// </summary>
		/// <param name="image"> The <see cref="T:NthDimension.Compute.ComputeImage" /> to map. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="flags"> A list of properties for the mapping mode. </param>
		/// <param name="offset"> The <paramref name="image" /> element position where mapping starts. </param>
		/// <param name="region"> The region of elements to map. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		/// <remarks> If <paramref name="blocking" /> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking" /> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
		public IntPtr Map(ComputeImage image, bool blocking, ComputeMemoryMappingFlags flags, SysIntX3 offset, SysIntX3 region, ICollection<ComputeEventBase> events)
		{
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			IntPtr image_row_pitch;
			IntPtr image_slice_pitch;
			IntPtr result = CL10.EnqueueMapImage(Handle, image.Handle, blocking, flags, ref offset, ref region, out image_row_pitch, out image_slice_pitch, handleCount, event_wait_list, array, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
			return result;
		}

		/// <summary>
		/// Enqueues a command to read data from a buffer.
		/// </summary>
		/// <param name="source"> The buffer to read from. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="offset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="region"> The region of elements to read. </param>
		/// <param name="destination"> A pointer to a preallocated memory area to read the data into. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		/// <remarks> If <paramref name="blocking" /> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking" /> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
		public void Read<T>(ComputeBufferBase<T> source, bool blocking, long offset, long region, IntPtr destination, ICollection<ComputeEventBase> events) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL10.EnqueueReadBuffer(Handle, source.Handle, blocking, new IntPtr(offset * num), new IntPtr(region * num), destination, handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a command to read a 2D or 3D region of elements from a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of the buffer. </typeparam>
		/// <param name="source"> The buffer to read from. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="sourceRowPitch"> The size of the source buffer row in bytes. If set to zero then <paramref name="sourceRowPitch" /> equals <c>region.X * sizeof(T)</c>. </param>
		/// <param name="sourceSlicePitch"> The size of the source buffer 2D slice in bytes. If set to zero then <paramref name="sourceSlicePitch" /> equals <c>region.Y * sizeof(T) * sourceRowPitch</c>. </param>
		/// <param name="destinationRowPitch"> The size of the destination buffer row in bytes. If set to zero then <paramref name="destinationRowPitch" /> equals <c>region.X * sizeof(T)</c>. </param>
		/// <param name="destinationSlicePitch"> The size of the destination buffer 2D slice in bytes. If set to zero then <paramref name="destinationSlicePitch" /> equals <c>region.Y * sizeof(T) * destinationRowPitch</c>. </param>
		/// <param name="destination"> A pointer to a preallocated memory area to read the data into. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		/// <remarks> Requires OpenCL 1.1. </remarks>
		private void Read<T>(ComputeBufferBase<T> source, bool blocking, SysIntX3 sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, long sourceRowPitch, long sourceSlicePitch, long destinationRowPitch, long destinationSlicePitch, IntPtr destination, ICollection<ComputeEventBase> events) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			sourceOffset.X = new IntPtr(num * sourceOffset.X.ToInt64());
			destinationOffset.X = new IntPtr(num * destinationOffset.X.ToInt64());
			region.X = new IntPtr(num * region.X.ToInt64());
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL11.EnqueueReadBufferRect(Handle, source.Handle, blocking, ref sourceOffset, ref destinationOffset, ref region, new IntPtr(sourceRowPitch), new IntPtr(sourceSlicePitch), new IntPtr(destinationRowPitch), new IntPtr(destinationSlicePitch), destination, handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a command to read data from a <see cref="T:NthDimension.Compute.ComputeImage" />.
		/// </summary>
		/// <param name="source"> The <see cref="T:NthDimension.Compute.ComputeImage" /> to read from. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="offset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="region"> The region of elements to read. </param>
		/// <param name="rowPitch"> The <see cref="P:Cloo.ComputeImage.RowPitch" /> of <paramref name="source" /> or 0. </param>
		/// <param name="slicePitch"> The <see cref="P:Cloo.ComputeImage.SlicePitch" /> of <paramref name="source" /> or 0. </param>
		/// <param name="destination"> A pointer to a preallocated memory area to read the data into. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		/// <remarks> If <paramref name="blocking" /> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking" /> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
		public void Read(ComputeImage source, bool blocking, SysIntX3 offset, SysIntX3 region, long rowPitch, long slicePitch, IntPtr destination, ICollection<ComputeEventBase> events)
		{
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL10.EnqueueReadImage(Handle, source.Handle, blocking, ref offset, ref region, new IntPtr(rowPitch), new IntPtr(slicePitch), destination, handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a command to release <see cref="T:NthDimension.Compute.ComputeMemory" />s that have been created from OpenGL objects.
		/// </summary>
		/// <param name="memObjs"> A collection of <see cref="T:NthDimension.Compute.ComputeMemory" />s that correspond to OpenGL memory objects. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		public void ReleaseGLObjects(ICollection<ComputeMemory> memObjs, ICollection<ComputeEventBase> events)
		{
			int handleCount;
			CLMemoryHandle[] mem_objects = Tools.ExtractHandles(memObjs, out handleCount);
			int handleCount2;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount2);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL10.EnqueueReleaseGLObjects(Handle, handleCount, mem_objects, handleCount2, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a command to unmap a buffer or a <see cref="T:NthDimension.Compute.ComputeImage" /> from the host address space.
		/// </summary>
		/// <param name="memory"> The <see cref="T:NthDimension.Compute.ComputeMemory" />. </param>
		/// <param name="mappedPtr"> The host address returned by a previous call to <see cref="T:NthDimension.Compute.ComputeCommandQueue.Map(Cloo.ComputeImage,System.Boolean,Cloo.ComputeMemoryMappingFlags,Cloo.SysIntX3,Cloo.SysIntX3,System.Collections.Generic.ICollection{Cloo.ComputeEventBase})" />. This pointer is <c>IntPtr.Zero</c> after this method returns. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		public void Unmap(ComputeMemory memory, ref IntPtr mappedPtr, ICollection<ComputeEventBase> events)
		{
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL10.EnqueueUnmapMemObject(Handle, memory.Handle, mappedPtr, handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			mappedPtr = IntPtr.Zero;
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a wait command for a collection of <see cref="T:NthDimension.Compute.ComputeEvent" />s to complete before any future commands queued in the <see cref="T:NthDimension.Compute.ComputeCommandQueue" /> are executed.
		/// </summary>
		/// <param name="events"> The <see cref="T:NthDimension.Compute.ComputeEvent" />s that this command will wait for. </param>
		public void Wait(ICollection<ComputeEventBase> events)
		{
			int handleCount;
			CLEventHandle[] event_list = Tools.ExtractHandles(events, out handleCount);
			ComputeErrorCode errorCode = CL10.EnqueueWaitForEvents(Handle, handleCount, event_list);
			ComputeException.ThrowOnError(errorCode);
		}

		/// <summary>
		/// Enqueues a command to write data to a buffer.
		/// </summary>
		/// <param name="destination"> The buffer to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to write. </param>
		/// <param name="source"> The data written to the buffer. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		/// <remarks> If <paramref name="blocking" /> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking" /> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
		public void Write<T>(ComputeBufferBase<T> destination, bool blocking, long destinationOffset, long region, IntPtr source, ICollection<ComputeEventBase> events) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL10.EnqueueWriteBuffer(Handle, destination.Handle, blocking, new IntPtr(destinationOffset * num), new IntPtr(region * num), source, handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a command to write a 2D or 3D region of elements to a buffer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of the buffer. </typeparam>
		/// <param name="destination"> The buffer to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="sourceOffset"> The <paramref name="source" /> element position where reading starts. </param>
		/// <param name="region"> The region of elements to copy. </param>
		/// <param name="destinationRowPitch"> The size of the destination buffer row in bytes. If set to zero then <paramref name="destinationRowPitch" /> equals <c>region.X * sizeof(T)</c>. </param>
		/// <param name="destinationSlicePitch"> The size of the destination buffer 2D slice in bytes. If set to zero then <paramref name="destinationSlicePitch" /> equals <c>region.Y * sizeof(T) * destinationRowPitch</c>. </param>
		/// <param name="sourceRowPitch"> The size of the memory area row in bytes. If set to zero then <paramref name="sourceRowPitch" /> equals <c>region.X * sizeof(T)</c>. </param>
		/// <param name="sourceSlicePitch"> The size of the memory area 2D slice in bytes. If set to zero then <paramref name="sourceSlicePitch" /> equals <c>region.Y * sizeof(T) * sourceRowPitch</c>. </param>
		/// <param name="source"> The data written to the buffer. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		/// <remarks> Requires OpenCL 1.1. </remarks>
		private void Write<T>(ComputeBufferBase<T> destination, bool blocking, SysIntX3 sourceOffset, SysIntX3 destinationOffset, SysIntX3 region, long destinationRowPitch, long destinationSlicePitch, long sourceRowPitch, long sourceSlicePitch, IntPtr source, ICollection<ComputeEventBase> events) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			sourceOffset.X = new IntPtr(num * sourceOffset.X.ToInt64());
			destinationOffset.X = new IntPtr(num * destinationOffset.X.ToInt64());
			region.X = new IntPtr(num * region.X.ToInt64());
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL11.EnqueueWriteBufferRect(Handle, destination.Handle, blocking, ref destinationOffset, ref sourceOffset, ref region, new IntPtr(destinationRowPitch), new IntPtr(destinationSlicePitch), new IntPtr(sourceRowPitch), new IntPtr(sourceSlicePitch), source, handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Enqueues a command to write data to a <see cref="T:NthDimension.Compute.ComputeImage" />.
		/// </summary>
		/// <param name="destination"> The <see cref="T:NthDimension.Compute.ComputeImage" /> to write to. </param>
		/// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
		/// <param name="destinationOffset"> The <paramref name="destination" /> element position where writing starts. </param>
		/// <param name="region"> The region of elements to write. </param>
		/// <param name="rowPitch"> The <see cref="P:Cloo.ComputeImage.RowPitch" /> of <paramref name="destination" /> or 0. </param>
		/// <param name="slicePitch"> The <see cref="P:Cloo.ComputeImage.SlicePitch" /> of <paramref name="destination" /> or 0. </param>
		/// <param name="source"> The content written to the <see cref="T:NthDimension.Compute.ComputeImage" />. </param>
		/// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events" /> is not <c>null</c> or read-only a new <see cref="T:NthDimension.Compute.ComputeEvent" /> identifying this command is created and attached to the end of the collection. </param>
		/// <remarks> If <paramref name="blocking" /> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking" /> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
		public void Write(ComputeImage destination, bool blocking, SysIntX3 destinationOffset, SysIntX3 region, long rowPitch, long slicePitch, IntPtr source, ICollection<ComputeEventBase> events)
		{
			int handleCount;
			CLEventHandle[] event_wait_list = Tools.ExtractHandles(events, out handleCount);
			bool flag = events != null && !events.IsReadOnly;
			CLEventHandle[] array = flag ? new CLEventHandle[1] : null;
			ComputeErrorCode errorCode = CL10.EnqueueWriteImage(Handle, destination.Handle, blocking, ref destinationOffset, ref region, new IntPtr(rowPitch), new IntPtr(slicePitch), source, handleCount, event_wait_list, array);
			ComputeException.ThrowOnError(errorCode);
			if (flag)
			{
				events.Add(new ComputeEvent(array[0], this));
			}
		}

		/// <summary>
		/// Releases the associated OpenCL object.
		/// </summary>
		/// <param name="manual"> Specifies the operation mode of this method. </param>
		/// <remarks> <paramref name="manual" /> must be <c>true</c> if this method is invoked directly by the application. </remarks>
		protected override void Dispose(bool manual)
		{
			if (manual)
			{
			}
			if (Handle.IsValid)
			{
				Trace.WriteLine("Dispose " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
				CL10.ReleaseCommandQueue(Handle);
				Handle.Invalidate();
			}
		}
	}
}
