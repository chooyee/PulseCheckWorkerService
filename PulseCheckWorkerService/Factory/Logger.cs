using Serilog;

namespace PulseCheckWorkerService.Factory
{
    public class LoggerService
    {
        public static void InitLogService(string LogFolder = "")
        {
            if (string.IsNullOrEmpty(LogFolder))
                LogFolder = AppContext.BaseDirectory;

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(string.Format("{0}/logs/log_{1:yyyy_MM_dd}.txt", LogFolder, DateTime.Today), rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .CreateLogger();
        }
    }
}
