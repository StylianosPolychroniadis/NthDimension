using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#if log4net
using log4net;
#endif

namespace NthStudio.IoC.Context
{
    // ORIGINALLY ServiceSkeleton

    /// <summary>
    ///   This is the base of all service classes. All service classes must implement this class.
    /// </summary>
    public abstract class ServiceContext : IService
    {
        private readonly Thread _workerThread;

        protected ServiceContext()
        {
            _workerThread = new Thread(Execute);
        }

        protected virtual string GetLoggerName()
        {
            return GetType().Name;
        }

#region Abstract Methods

        /// <summary>
        ///   Gets the sleep time between service classes operations.
        /// </summary>
        /// <returns>Seconds to sleep.</returns>
        public abstract int SleepTime();

        /// <summary>
        ///   Signals the implementer class that it should do its own process.
        /// </summary>
        public abstract void ExecuteInternal();

        /// <summary>
        ///   Signals the implementer class to configure itsefl.
        /// </summary>
        /// <remarks>
        ///   After a success configuration, implementer class should set "IsConfigured" property to "true".
        /// </remarks>
        public abstract void ConfigureInternal();

#endregion

#region Implementation of IConfigurable

        public bool IsConfigured { get; set; }

        public void Configure()
        {
            StartDate = DateTime.MinValue;
            StopDate = DateTime.MinValue;

#if log4net
            LoggerTool.CreateLogger(GetLoggerName());
            Log = LogManager.GetLogger(GetLoggerName());
#endif

            //

            ConfigureInternal();
        }

#endregion

#region Implementation of IRunnable

        public bool IsRunning { get; private set; }

        public void Start()
        {
            _workerThread.Start();

            IsRunning = true;
            StartDate = DateTime.Now;
        }

        public void Stop()
        {
            if (IsRunning && !_workerThread.Join(TimeSpan.FromSeconds(30)))
                _workerThread.Abort();

            IsRunning = false;
            StopDate = DateTime.Now;
        }

        public DateTime StartDate { get; private set; }

        public DateTime StopDate { get; private set; }

#endregion

#region Implementation of IService

#if log4net
        public ILog Log { get; private set; }
#endif

        public void Execute()
        {
#if log4net
            Log.Debug(String.Format("Service \"{0}\" entered the execute loop.", this));
#endif

            while (!ServiceManager.SingleInstance.IsStopRequested)
            {
                try
                {
                    ExecuteInternal();
                }
                catch (ThreadAbortException) { }
                catch (StopRequestedException) { }
                catch (Exception ex)
                {
#if log4net
                    Log.Error("Error on internal executing process.", ex);
#endif
                }

#if log4net
                Log.Debug(String.Format("Sleeping for {0:n0} second(s).", SleepTime()));
#endif

                for (var i = 0; i < SleepTime(); i++)
                {
                    Thread.Sleep(1000);

                    if (ServiceManager.SingleInstance.IsStopRequested)
                        break;
                }
            }

#if log4net
            Log.Debug(String.Format("Service \"{0}\" exited from the execute loop.", this));
#endif
        }

#endregion
    }
}
