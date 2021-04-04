using System;
using System.Runtime.InteropServices;

namespace NthDimension.Math
{
	public sealed class UnsafeBuffer : IDisposable
	{
		private readonly GCHandle _handle;

		private unsafe void* _ptr;

		private int _length;

		private Array _buffer;

		public unsafe void* Address
		{
			get
			{
				return _ptr;
			}
		}

		public int Length
		{
			get
			{
				return _length;
			}
		}

		private unsafe UnsafeBuffer(Array buffer, int realLength, bool aligned)
		{
			_buffer = buffer;
			_handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
			_ptr = (void*)_handle.AddrOfPinnedObject();
			if (aligned)
			{
				_ptr = (void*)(((long)_ptr + 15) & -16);
			}
			_length = realLength;
		}

		~UnsafeBuffer()
		{
			Dispose();
		}

		public unsafe void Dispose()
		{
			if (_handle.IsAllocated)
			{
				_handle.Free();
			}
			_buffer = null;
			_ptr = null;
			_length = 0;
			GC.SuppressFinalize(this);
		}

		public void Clear()
		{
			Array.Clear(_buffer, 0, _buffer.Length);
		}

		public unsafe static implicit operator void*(UnsafeBuffer unsafeBuffer)
		{
			return unsafeBuffer.Address;
		}

		public static UnsafeBuffer Create(int size)
		{
			return Create(size, 1, true);
		}

		public static UnsafeBuffer Create(int length, int sizeOfElement)
		{
			return Create(length, sizeOfElement, true);
		}

		public static UnsafeBuffer Create(int length, int sizeOfElement, bool aligned)
		{
			return new UnsafeBuffer(new byte[length * sizeOfElement + (aligned ? 16 : 0)], length, aligned);
		}

		public static UnsafeBuffer Create(Array buffer)
		{
			return new UnsafeBuffer(buffer, buffer.Length, false);
		}
	}
}
