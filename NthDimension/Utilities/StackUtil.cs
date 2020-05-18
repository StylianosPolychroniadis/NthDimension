using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NthDimension.Utilities
{
    // no-inlining seems to be broken on x64
    // https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=162364&wa=wsignin1.0

    public class WhoCalls
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string WhatsMyName()
        {
            StackFrame stackFrame = new StackFrame();
            MethodBase methodBase = stackFrame.GetMethod();
            return methodBase.Name;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string WhoCalledMe()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(2);
            MethodBase methodBase = stackFrame.GetMethod();

            return String.Format("{0}:{1}:{2}.{3}",
                stackFrame.GetFileName(),
                stackFrame.GetFileLineNumber(),
                methodBase.ReflectedType,
                methodBase.Name);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string StackTrace()
        {
            string ss = string.Empty;

            StackTrace stackTrace = new StackTrace();
            for (int x = 1; x < stackTrace.FrameCount; x++)
            {
                StackFrame stackFrame = stackTrace.GetFrame(x);
                var method = stackFrame.GetMethod();
                ss = ss + method.ReflectedType + "." + method.Name + ":" + stackFrame.GetFileName() + ":" + stackFrame.GetFileLineNumber() + " ";
            }


            return ss;
        }
    }
}
