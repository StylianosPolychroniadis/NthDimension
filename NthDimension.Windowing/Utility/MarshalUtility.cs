using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NthDimension.Windowing.Utility
{
    internal static class MarshalUtility
    {
        public static unsafe IntPtr StringToCoTaskMemUTF8(string str)
        {
#if NETCORE
            return Marshal.sStringToCoTaskMemUTF8(str);
#else
            return AllocConvertManagedStringToNativeUtf8(str);
#endif
        }

        /// <summary>
        ///     Converts a null-terminated UTF-8 string to a <see cref="string" />.
        /// </summary>
        /// <param name="ptr">The pointer to the null-terminated UTF-8 data.</param>
        /// <returns>The string.</returns>
        public static unsafe string PtrToStringUTF8(byte* ptr)
        {
#if NETCORE
            return Marshal.PtrToStringUTF8((IntPtr)ptr);
#else
            return MarshalNativeUtf8ToManagedString(ptr);
#endif
        }

#if !NETCORE
        private static unsafe IntPtr AllocConvertManagedStringToNativeUtf8(string input)
        {
            fixed (char* pInput = input)
            {
                var len = Encoding.UTF8.GetByteCount(pInput, input.Length);
                var pResult = (byte*)Marshal.AllocHGlobal(len + 1).ToPointer();
                var bytesWritten = Encoding.UTF8.GetBytes(pInput, input.Length, pResult, len);
                //Trace.Assert(len == bytesWritten);
                pResult[len] = 0;
                return (IntPtr)pResult;
            }
        }

        private static unsafe string MarshalNativeUtf8ToManagedString(IntPtr pStringUtf8)
            => MarshalNativeUtf8ToManagedString((byte*)pStringUtf8);

        private static unsafe string MarshalNativeUtf8ToManagedString(byte* pStringUtf8)
        {
            var len = 0;
            while (pStringUtf8[len] != 0) len++;
            return Encoding.UTF8.GetString(pStringUtf8, len);
        }
#endif
    }
}
