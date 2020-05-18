//using log4net;

namespace NthStudio.IoC.Context
{
    /// <summary>
    ///   Defines the interface for service classes.
    /// </summary>
    public interface IService : IConfigurable, IRunnable
    {
        /// <summary>
        ///   When implemented in a class, gets the logger for log operations.
        /// </summary>
#if log4net
        ILog Log { get; }
#endif

        /// <summary>
        ///   When implemented in a class, signals the implementer that it should execute its operations.
        /// </summary>
        void Execute();
    }
}
