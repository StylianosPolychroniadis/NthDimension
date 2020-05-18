using System;

namespace NthDimension.Service
{
    public interface IRunnable      // Name sucks Refactor to IExecutable???
    {
        /// <summary>
        ///   When implemented in a class, determines whether the execution has started.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        ///   When implemented in a class, gets the last time when this instance has completed
        ///   <see cref = "Start()" /> method. Must be equal to <see cref = "DateTime.MinValue" />
        ///   if this instance has never started.
        /// </summary>
        DateTime StartDate { get; }

        /// <summary>
        ///   When implemented in a class, gets the last time when this instance has completed
        ///   <see cref = "Stop()" /> method. Must be equal to <see cref = "DateTime.MinValue" />
        ///   if this instance has never stopped.
        /// </summary>
        DateTime StopDate { get; }

        /// <summary>
        ///   When implemented in a class, signals the object that it should start
        ///   execution. The object should start the execution only if it is not running.
        /// </summary>
        void Start();

        /// <summary>
        ///   When implemented in a class, signals the object that it should stop
        ///   execution and dispose all associated resources. The object should sopt
        ///   execution only if it is running.
        /// </summary>
        void Stop();
    }
}
