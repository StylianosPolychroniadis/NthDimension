using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL object.
	/// </summary>
	/// <remarks> An OpenCL object is an object that is identified by its handle in the OpenCL environment. </remarks>
	public abstract class ComputeObject : IEquatable<ComputeObject>
	{
		/// <summary>
		///
		/// </summary>
		/// <typeparam name="HandleType"></typeparam>
		/// <typeparam name="InfoType"></typeparam>
		/// <param name="objectHandle"></param>
		/// <param name="paramName"></param>
		/// <param name="paramValueSize"></param>
		/// <param name="paramValue"></param>
		/// <param name="paramValueSizeRet"></param>
		/// <returns></returns>
		protected delegate ComputeErrorCode GetInfoDelegate<HandleType, InfoType>(HandleType objectHandle, InfoType paramName, IntPtr paramValueSize, IntPtr paramValue, out IntPtr paramValueSizeRet);

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="MainHandleType"></typeparam>
		/// <typeparam name="SecondHandleType"></typeparam>
		/// <typeparam name="InfoType"></typeparam>
		/// <param name="mainObjectHandle"></param>
		/// <param name="secondaryObjectHandle"></param>
		/// <param name="paramName"></param>
		/// <param name="paramValueSize"></param>
		/// <param name="paramValue"></param>
		/// <param name="paramValueSizeRet"></param>
		/// <returns></returns>
		protected delegate ComputeErrorCode GetInfoDelegateEx<MainHandleType, SecondHandleType, InfoType>(MainHandleType mainObjectHandle, SecondHandleType secondaryObjectHandle, InfoType paramName, IntPtr paramValueSize, IntPtr paramValue, out IntPtr paramValueSizeRet);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IntPtr handle;

		/// <summary>
		/// Checks if two <c>object</c>s are equal. These <c>object</c>s must be cast from <see cref="T:NthDimension.Compute.ComputeObject" />s.
		/// </summary>
		/// <param name="objA"> The first <c>object</c> to compare. </param>
		/// <param name="objB"> The second <c>object</c> to compare. </param>
		/// <returns> <c>true</c> if the <c>object</c>s are equal otherwise <c>false</c>. </returns>
		public new static bool Equals(object objA, object objB)
		{
			if (objA == objB)
			{
				return true;
			}
			if (objA == null || objB == null)
			{
				return false;
			}
			return objA.Equals(objB);
		}

		/// <summary>
		/// Checks if the <see cref="T:NthDimension.Compute.ComputeObject" /> is equal to a specified <see cref="T:NthDimension.Compute.ComputeObject" /> cast to an <c>object</c>.
		/// </summary>
		/// <param name="obj"> The specified <c>object</c> to compare the <see cref="T:NthDimension.Compute.ComputeObject" /> with. </param>
		/// <returns> <c>true</c> if the <see cref="T:NthDimension.Compute.ComputeObject" /> is equal with <paramref name="obj" /> otherwise <c>false</c>. </returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (!(obj is ComputeObject))
			{
				return false;
			}
			return Equals(obj as ComputeObject);
		}

		/// <summary>
		/// Checks if the <see cref="T:NthDimension.Compute.ComputeObject" /> is equal to a specified <see cref="T:NthDimension.Compute.ComputeObject" />.
		/// </summary>
		/// <param name="obj"> The specified <see cref="T:NthDimension.Compute.ComputeObject" /> to compare the <see cref="T:NthDimension.Compute.ComputeObject" /> with. </param>
		/// <returns> <c>true</c> if the <see cref="T:NthDimension.Compute.ComputeObject" /> is equal with <paramref name="obj" /> otherwise <c>false</c>. </returns>
		public bool Equals(ComputeObject obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (!handle.Equals(obj.handle))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Gets the hash code of the <see cref="T:NthDimension.Compute.ComputeObject" />.
		/// </summary>
		/// <returns> The hash code of the <see cref="T:NthDimension.Compute.ComputeObject" />. </returns>
		public override int GetHashCode()
		{
			return handle.GetHashCode();
		}

		/// <summary>
		/// Gets the string representation of the <see cref="T:NthDimension.Compute.ComputeObject" />.
		/// </summary>
		/// <returns> The string representation of the <see cref="T:NthDimension.Compute.ComputeObject" />. </returns>
		public override string ToString()
		{
			return GetType().Name + "(" + handle.ToString() + ")";
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="HandleType"></typeparam>
		/// <typeparam name="InfoType"></typeparam>
		/// <typeparam name="QueriedType"></typeparam>
		/// <param name="handle"></param>
		/// <param name="paramName"></param>
		/// <param name="getInfoDelegate"></param>
		/// <returns></returns>
		protected QueriedType[] GetArrayInfo<HandleType, InfoType, QueriedType>(HandleType handle, InfoType paramName, GetInfoDelegate<HandleType, InfoType> getInfoDelegate)
		{
			IntPtr paramValueSizeRet;
			ComputeErrorCode errorCode = getInfoDelegate(handle, paramName, IntPtr.Zero, IntPtr.Zero, out paramValueSizeRet);
			ComputeException.ThrowOnError(errorCode);
			QueriedType[] array = new QueriedType[paramValueSizeRet.ToInt64() / Marshal.SizeOf(typeof(QueriedType))];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			try
			{
				errorCode = getInfoDelegate(handle, paramName, paramValueSizeRet, gCHandle.AddrOfPinnedObject(), out paramValueSizeRet);
				ComputeException.ThrowOnError(errorCode);
			}
			finally
			{
				gCHandle.Free();
			}
			return array;
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="MainHandleType"></typeparam>
		/// <typeparam name="SecondHandleType"></typeparam>
		/// <typeparam name="InfoType"></typeparam>
		/// <typeparam name="QueriedType"></typeparam>
		/// <param name="mainHandle"></param>
		/// <param name="secondHandle"></param>
		/// <param name="paramName"></param>
		/// <param name="getInfoDelegate"></param>
		/// <returns></returns>
		protected QueriedType[] GetArrayInfo<MainHandleType, SecondHandleType, InfoType, QueriedType>(MainHandleType mainHandle, SecondHandleType secondHandle, InfoType paramName, GetInfoDelegateEx<MainHandleType, SecondHandleType, InfoType> getInfoDelegate)
		{
			IntPtr paramValueSizeRet;
			ComputeErrorCode errorCode = getInfoDelegate(mainHandle, secondHandle, paramName, IntPtr.Zero, IntPtr.Zero, out paramValueSizeRet);
			ComputeException.ThrowOnError(errorCode);
			QueriedType[] array = new QueriedType[paramValueSizeRet.ToInt64() / Marshal.SizeOf(typeof(QueriedType))];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			try
			{
				errorCode = getInfoDelegate(mainHandle, secondHandle, paramName, paramValueSizeRet, gCHandle.AddrOfPinnedObject(), out paramValueSizeRet);
				ComputeException.ThrowOnError(errorCode);
			}
			finally
			{
				gCHandle.Free();
			}
			return array;
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="HandleType"></typeparam>
		/// <typeparam name="InfoType"></typeparam>
		/// <param name="handle"></param>
		/// <param name="paramName"></param>
		/// <param name="getInfoDelegate"></param>
		/// <returns></returns>
		protected bool GetBoolInfo<HandleType, InfoType>(HandleType handle, InfoType paramName, GetInfoDelegate<HandleType, InfoType> getInfoDelegate)
		{
			int info = GetInfo<HandleType, InfoType, int>(handle, paramName, getInfoDelegate);
			return info == 1;
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="HandleType"></typeparam>
		/// <typeparam name="InfoType"></typeparam>
		/// <typeparam name="QueriedType"></typeparam>
		/// <param name="handle"></param>
		/// <param name="paramName"></param>
		/// <param name="getInfoDelegate"></param>
		/// <returns></returns>
		protected QueriedType GetInfo<HandleType, InfoType, QueriedType>(HandleType handle, InfoType paramName, GetInfoDelegate<HandleType, InfoType> getInfoDelegate) where QueriedType : struct
		{
			QueriedType val = default(QueriedType);
			GCHandle gCHandle = GCHandle.Alloc(val, GCHandleType.Pinned);
			try
			{
				IntPtr paramValueSizeRet;
				ComputeErrorCode errorCode = getInfoDelegate(handle, paramName, (IntPtr)Marshal.SizeOf(val), gCHandle.AddrOfPinnedObject(), out paramValueSizeRet);
				ComputeException.ThrowOnError(errorCode);
			}
			finally
			{
				val = (QueriedType)gCHandle.Target;
				gCHandle.Free();
			}
			return val;
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="MainHandleType"></typeparam>
		/// <typeparam name="SecondHandleType"></typeparam>
		/// <typeparam name="InfoType"></typeparam>
		/// <typeparam name="QueriedType"></typeparam>
		/// <param name="mainHandle"></param>
		/// <param name="secondHandle"></param>
		/// <param name="paramName"></param>
		/// <param name="getInfoDelegate"></param>
		/// <returns></returns>
		protected QueriedType GetInfo<MainHandleType, SecondHandleType, InfoType, QueriedType>(MainHandleType mainHandle, SecondHandleType secondHandle, InfoType paramName, GetInfoDelegateEx<MainHandleType, SecondHandleType, InfoType> getInfoDelegate) where QueriedType : struct
		{
			QueriedType val = default(QueriedType);
			GCHandle gCHandle = GCHandle.Alloc(val, GCHandleType.Pinned);
			try
			{
				IntPtr paramValueSizeRet;
				ComputeErrorCode errorCode = getInfoDelegate(mainHandle, secondHandle, paramName, new IntPtr(Marshal.SizeOf(val)), gCHandle.AddrOfPinnedObject(), out paramValueSizeRet);
				ComputeException.ThrowOnError(errorCode);
			}
			finally
			{
				val = (QueriedType)gCHandle.Target;
				gCHandle.Free();
			}
			return val;
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="HandleType"></typeparam>
		/// <typeparam name="InfoType"></typeparam>
		/// <param name="handle"></param>
		/// <param name="paramName"></param>
		/// <param name="getInfoDelegate"></param>
		/// <returns></returns>
		protected string GetStringInfo<HandleType, InfoType>(HandleType handle, InfoType paramName, GetInfoDelegate<HandleType, InfoType> getInfoDelegate)
		{
			byte[] arrayInfo = GetArrayInfo<HandleType, InfoType, byte>(handle, paramName, getInfoDelegate);
			char[] chars = Encoding.ASCII.GetChars(arrayInfo, 0, arrayInfo.Length);
			string text = new string(chars);
			char[] trimChars = new char[1];
			return text.TrimEnd(trimChars);
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="MainHandleType"></typeparam>
		/// <typeparam name="SecondHandleType"></typeparam>
		/// <typeparam name="InfoType"></typeparam>
		/// <param name="mainHandle"></param>
		/// <param name="secondHandle"></param>
		/// <param name="paramName"></param>
		/// <param name="getInfoDelegate"></param>
		/// <returns></returns>
		protected string GetStringInfo<MainHandleType, SecondHandleType, InfoType>(MainHandleType mainHandle, SecondHandleType secondHandle, InfoType paramName, GetInfoDelegateEx<MainHandleType, SecondHandleType, InfoType> getInfoDelegate)
		{
			byte[] arrayInfo = GetArrayInfo<MainHandleType, SecondHandleType, InfoType, byte>(mainHandle, secondHandle, paramName, getInfoDelegate);
			char[] chars = Encoding.ASCII.GetChars(arrayInfo, 0, arrayInfo.Length);
			string text = new string(chars);
			char[] trimChars = new char[1];
			return text.TrimEnd(trimChars);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="id"></param>
		protected void SetID(IntPtr id)
		{
			handle = id;
		}
	}
}
