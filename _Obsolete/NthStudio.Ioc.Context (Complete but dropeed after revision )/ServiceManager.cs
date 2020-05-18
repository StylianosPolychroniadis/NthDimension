using System;
using System.Collections.Generic;
using System.Threading;

#if log4net
using log4net;
using log4net.Config;
#endif

namespace NthStudio.IoC.Context
{
    /// <summary>
    ///   Manages the starting and stopping of services configured in the system.
    /// </summary>
    public class ServiceManager : IConfigurable, IRunnable
    {
#region Singleton Design Pattern

        private static ServiceManager _singleInstance;

        /// <summary>
        ///   Gets the sole instance of the ServiceManager class.
        /// </summary>
        public static ServiceManager SingleInstance
        {
            get { return _singleInstance ?? (_singleInstance = new ServiceManager()); }
        }

#endregion

#region Public Properties

        /// <summary>
        ///   Signals to all services that they should stop working.
        /// </summary>
        public bool IsStopRequested { get; private set; }

#if log4net
        /// <summary>
        ///   Gets the logger of the site manager.
        /// </summary>
        public ILog Log { get; private set; }
#endif

        /// <summary>
        ///   Gets the list of services.
        /// </summary>
        public List<IService> Services { get; set; }

#endregion

#region Private Properties

        private int _configuredServiceCount;

#endregion

#region Implementation of IConfigurable

        public bool IsConfigured { get; set; }

        public void Configure()
        {
            if (IsConfigured)
                throw new ServiceManagerBaseException("ServiceManager already configured. Subsequent configuration calls in the same application domain is not supported.\n");

            //
            // Check if Services is null.
            //

            if (Services == null)
                throw new ServiceManagerBaseException("Property \"Services\" must be set before calling configure method.\n");

            //

            StartDate = DateTime.MinValue;
            StopDate = DateTime.MinValue;

#if log4net
            //
            // Configure log4net.
            //

            XmlConfigurator.Configure();

            LoggerTool.CreateLogger("ServiceManager");
            Log = LogManager.GetLogger("ServiceManager");
#endif
            //
            // Configure services
            //

            foreach (var service in Services)
            {
                try
                {
#if log4net
                    Log.Debug(String.Format("Trying to configure service \"{0}\"...", service));
#endif

                    service.Configure();
#if log4net
                    Log.Debug(service.IsConfigured
                                  ? String.Format("Service \"{0}\" is configured successfully.", service)
                                  : String.Format("Service \"{0}\" is not configured. See related services log entries.", service));
#endif
                    if (service.IsConfigured)
                        _configuredServiceCount++;
                }
                catch (Exception exception)
                {
#if log4net
                    Log.Error(String.Format("Failed to configure service \"{0}\"", service), exception);
#endif
                }
            }
#if log4net
            Log.Debug(String.Format("{0:n0} service(s) configured.", _configuredServiceCount));
#endif

            IsConfigured = true;
        }

#endregion

#region Implementation of IRunnable

        public bool IsRunning { get; private set; }

        public void Start()
        {
            if (!IsConfigured)
            {
                const string msg = "ServiceManager can not be started without calling the \"Configure\" method first.";
#if log4net
                Log.Fatal(msg);
#endif
                throw new ServiceManagerBaseException(msg);
            }

            if (IsRunning)
            {
#if log4net
                Log.Warn("Attempt to start an already started ServiceManager.");
#endif
                return;
            }

            if (_configuredServiceCount == 0)
            {
                const string msg = "No services are configured; ServiceManager will not start.";
#if log4net
                Log.Fatal(msg);
#endif
                throw new ServiceManagerBaseException(msg);
            }

#if log4net
            Log.Debug(String.Format("{0:n0} configured service(s) found. Starting services...", _configuredServiceCount));
#endif

            var startedServiceCount = 0;

            foreach (var service in Services)
            {
                if (!service.IsConfigured)
                {
#if log4net
                    Log.Warn(String.Format("Service \"{0}\" is not configured. Resuming to start other services.", service));
#endif
                    continue;
                }

                try
                {
#if log4net
                    Log.Debug(String.Format("Starting service \"{0}\"...", service));
#endif

                    service.Start();
                    startedServiceCount++;
#if log4net
                    Log.Debug(String.Format("Service \"{0}\" started.", service));
#endif
                }
                catch (Exception exception)
                {
#if log4net
                    Log.Error(String.Format("Could not start service \"{0}\".", service), exception);
#endif
                }
            }

            IsStopRequested = false;
            IsRunning = true;
            StartDate = DateTime.Now;

#if log4net
            Log.Info(String.Format("ServiceManager started successfully on host \"{0}\" with {1:n0} service(s).", Environment.MachineName, startedServiceCount));
#endif
        }

        public void Stop()
        {
            if (!IsRunning)
            {
#if log4net
                Log.Warn("Attempt to stop an already stopped ServiceManager.");
#endif
                return;
            }
#if log4net
            Log.Debug("Stopping ServiceManager...");
#endif

            IsStopRequested = true;

            foreach (var service in Services)
            {
                try
                {
                    if (service.IsRunning)
                    {
#if log4net
                        Log.Debug(String.Format("Stopping service \"{0}\"...", service));
#endif

                        service.Stop();

#if log4net
                        Log.Debug(String.Format("Service \"{0}\" stopped.", service));
#endif
                    }
                    else
                    {
#if log4net
                        Log.Debug(String.Format("Service \"{0}\" is not running. Resuming to stop other services.", service));
#endif
                    }
                }
                catch (ThreadAbortException) { }
                catch (StopRequestedException) { }
                catch (Exception ex)
                {
                    var msg = String.Format("Stop attempt failed for service \"{0}\". Resuming to stop of other services.", service);
#if log4net
                    Log.Fatal(msg, ex);
#endif
                }
            }

            IsRunning = false;
            StopDate = DateTime.Now;
#if log4net
            Log.Info(String.Format("ServiceManager stopped successfully on host \"{0}\".", Environment.MachineName));
#endif
        }

        public DateTime StartDate { get; private set; }

        public DateTime StopDate { get; private set; }
        public object XmlConfigurator { get; private set; }

#endregion
    }
}
