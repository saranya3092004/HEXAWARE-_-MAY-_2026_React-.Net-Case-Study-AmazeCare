using log4net;
using log4net.Config;
using System.Reflection;

namespace AmazeCare.Server.Modules.Middlewares
{
    public static class Log4NetSetupExtensions
    {
        public static ILoggingBuilder AddAmazeCareLog4Net(this ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddLog4Net("log4net.config");
            return logging;
        }
    }
}
