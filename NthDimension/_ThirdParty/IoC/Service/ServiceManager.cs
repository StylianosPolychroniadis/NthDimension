using System;
using System.Collections.Generic;
using System.Threading;

namespace NthDimension.Service
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

        /// <summary>
        ///   Gets the logger of the site manager.
        /// </summary>
        public ILog Log { get; private set; }

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

            //
            // Configure log4net.
            //

            //XmlConfigurator.Configure();
            //LoggerTool.CreateLogger("ServiceManager");
            //Log = LogManager.GetLogger("ServiceManager");

            //
            // Configure services
            //

            foreach (var service in Services)
            {
                try
                {
                    Log.Debug(String.Format("Trying to configure service \"{0}\"...", service));

                    service.Configure();

                    Log.Debug(service.IsConfigured
                                  ? String.Format("Service \"{0}\" is configured successfully.", service)
                                  : String.Format("Service \"{0}\" is not configured. See related services log entries.", service));

                    if (service.IsConfigured)
                        _configuredServiceCount++;
                }
                catch (Exception exception)
                {
                    Log.Error(String.Format("Failed to configure service \"{0}\"", service), exception);
                }
            }

            Log.Debug(String.Format("{0:n0} service(s) configured.", _configuredServiceCount));

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
                Log.Fatal(msg);
                throw new ServiceManagerBaseException(msg);
            }

            if (IsRunning)
            {
                Log.Warn("Attempt to start an already started ServiceManager.");
                return;
            }

            if (_configuredServiceCount == 0)
            {
                const string msg = "No services are configured; ServiceManager will not start.";
                Log.Fatal(msg);
                throw new ServiceManagerBaseException(msg);
            }

            Log.Debug(String.Format("{0:n0} configured service(s) found. Starting services...", _configuredServiceCount));

            var startedServiceCount = 0;

            foreach (var service in Services)
            {
                if (!service.IsConfigured)
                {
                    Log.Warn(String.Format("Service \"{0}\" is not configured. Resuming to start other services.", service));
                    continue;
                }

                try
                {
                    Log.Debug(String.Format("Starting service \"{0}\"...", service));

                    service.Start();
                    startedServiceCount++;

                    Log.Debug(String.Format("Service \"{0}\" started.", service));
                }
                catch (Exception exception)
                {
                    Log.Error(String.Format("Could not start service \"{0}\".", service), exception);
                }
            }

            IsStopRequested = false;
            IsRunning = true;
            StartDate = DateTime.Now;

            Log.Info(String.Format("ServiceManager started successfully on host \"{0}\" with {1:n0} service(s).", Environment.MachineName, startedServiceCount));
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                Log.Warn("Attempt to stop an already stopped ServiceManager.");
                return;
            }

            Log.Debug("Stopping ServiceManager...");

            IsStopRequested = true;

            foreach (var service in Services)
            {
                try
                {
                    if (service.IsRunning)
                    {
                        Log.Debug(String.Format("Stopping service \"{0}\"...", service));

                        service.Stop();

                        Log.Debug(String.Format("Service \"{0}\" stopped.", service));
                    }
                    else
                    {
                        Log.Debug(String.Format("Service \"{0}\" is not running. Resuming to stop other services.", service));
                    }
                }
                catch (ThreadAbortException) { }
                catch (StopRequestedException) { }
                catch (Exception ex)
                {
                    var msg = String.Format("Stop attempt failed for service \"{0}\". Resuming to stop of other services.", service);
                    Log.Fatal(msg, ex);
                }
            }

            IsRunning = false;
            StopDate = DateTime.Now;

            Log.Info(String.Format("ServiceManager stopped successfully on host \"{0}\".", Environment.MachineName));
        }

        public DateTime StartDate { get; private set; }

        public DateTime StopDate { get; private set; }

        #endregion
    }
}
