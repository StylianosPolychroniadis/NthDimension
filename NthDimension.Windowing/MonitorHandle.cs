using System;

namespace NthDimension.Windowing
{
    /// <summary>
    /// Wrapper around an implementation-defined monitor struct.
    /// </summary>
    public struct MonitorHandle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorHandle"/> struct.
        /// </summary>
        /// <param name="ptr">A pointer to the underlying native Monitor.</param>
        public MonitorHandle(IntPtr ptr)
        {
            Pointer = ptr;
        }

        /// <summary>
        /// Gets a pointer to the underlying native Monitor.
        /// </summary>
        public IntPtr Pointer { get; }

        /// <summary>
        /// Converts the underlying <see cref="Pointer"/> to a unmanaged pointer.
        /// </summary>
        /// <typeparam name="T">The type of the object found at the <see cref="Pointer"/> memory address.</typeparam>
        /// <returns>A unmanaged pointer to the underlying native Monitor.</returns>
        public unsafe T* ToUnsafePtr<T>() where T : unmanaged
        {
            return (T*)Pointer;
        }
    }
}
