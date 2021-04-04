using System.Reflection;

namespace System.Runtime.InteropServices
{
    public static partial class NativeLibrary
    {
        private const string kernel32 = "Kernel32";
        const int ERROR_BAD_FORMAT = 11;
        const int ERROR_BAD_EXE_FORMAT = 193;

        [DllImport(kernel32, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport(kernel32, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr LoadLibraryEx(string lpLibFileName, IntPtr hFile, uint dwFlags);

        [DllImport(kernel32, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Shim4DotNetFramework.SafeHandles.SafeLibraryHandle LoadLibrarySafely(string lpLibFileName);

        internal static IntPtr LoadLibraryByName(string libraryName, Assembly _, DllImportSearchPath? searchPath, bool throwOnError)
        {
            var handle = LoadLibraryEx(libraryName, default, (uint)(searchPath ?? 0));
            int lastError = Marshal.GetLastWin32Error();
            if (handle == default && throwOnError)
            {
                switch (lastError)
                {
                    case ERROR_BAD_EXE_FORMAT:
                    case ERROR_BAD_FORMAT:
                        throw new BadImageFormatException();
                    default:
                        throw new DllNotFoundException();
                }
            }
            return handle;
        }

        internal static IntPtr LoadFromPath(string libraryName, bool throwOnError)
        {
            IntPtr handle = LoadLibrary(libraryName);
            int lastError = Marshal.GetLastWin32Error();

            if (handle == default && throwOnError)
            {
                switch (lastError)
                {
                    case ERROR_BAD_EXE_FORMAT:
                    case ERROR_BAD_FORMAT:
                        throw new BadImageFormatException();
                    default:
                        throw new DllNotFoundException();
                }
            }

            return handle;
        }

        [DllImport(kernel32, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr handle);

        [DllImport(kernel32, CharSet = CharSet.Ansi, SetLastError = true, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        internal static IntPtr GetSymbol(IntPtr handle, string symbolName, bool throwOnError)
        {
            IntPtr fpProc = GetProcAddress(handle, symbolName);
            return fpProc == default && throwOnError ? throw new EntryPointNotFoundException() : fpProc;
        }
    }
}
namespace Shim4DotNetFramework.SafeHandles
{
    using System.Security;
    using Microsoft.Win32.SafeHandles;

    [SecurityCritical]
    public sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeLibraryHandle()
            : base(ownsHandle: true)
        {
        }

        [SecurityCritical]
        protected override bool ReleaseHandle() => System.Runtime.InteropServices.NativeLibrary.FreeLibrary(handle);
    }

}