using System.Threading;

using NthDimension.Windowing.Framework;

namespace NthDimension.Windowing.Desktop
{
    /// <summary>
    /// Singleton providing easy GLFW implementation access.
    /// </summary>
    internal static class GLFWProvider
    {
        private static readonly GLFWCallbacks.ErrorCallback ErrorCallback =
            (errorCode, description) => throw new GLFWException(description, errorCode);

        // NOTE: This assumption about "main thread" is flat wrong.
        // We literally can't tell whether this is actually the main thread.
        // We keep it around as a tiny safe guard in case the first window IS done correctly and further ones aren't.
        private static Thread _mainThread;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Thread.CurrentThread"/> is the same as the GLFW main thread.
        /// </summary>
        public static bool IsOnMainThread => _mainThread == Thread.CurrentThread;

        public static void EnsureInitialized()
        {
            if (_mainThread != null)
            {
                return;
            }

            _mainThread = Thread.CurrentThread;
            GLFW.Init();
            GLFW.SetErrorCallback(ErrorCallback);
        }
    }
}
