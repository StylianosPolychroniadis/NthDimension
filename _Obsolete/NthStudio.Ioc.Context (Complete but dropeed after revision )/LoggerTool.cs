#if log4net
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
#endif

namespace NthStudio.IoC.Context
{
#if log4net
    internal static class LoggerTool
    {
        public static void CreateLogger(string loggerName)
        {
            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%d [%t] %-5p %m%n"
            };
            patternLayout.ActivateOptions();

            var roller = new FileAppender
            {
                Layout = patternLayout,
                AppendToFile = true,
                File = "Log Files\\" + loggerName + ".log",
            };
            roller.ActivateOptions();

            var hierarchy = (Hierarchy)LogManager.GetRepository();
            var coreLogger = hierarchy.GetLogger(loggerName) as Logger;
            coreLogger.AddAppender(roller);
        }
    }
#endif
}
