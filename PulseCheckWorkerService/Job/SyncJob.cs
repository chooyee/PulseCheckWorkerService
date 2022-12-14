using EasyCronJob.Abstractions;
using Factory;
using Global;
using Serilog;

namespace Job
{
    public class SyncJob : CronJobService
    {
        private readonly ILogger<SyncJob> logger;

        public SyncJob(ICronConfiguration<SyncJob> cronConfiguration, ILogger<SyncJob> logger)
            : base(cronConfiguration.CronExpression, cronConfiguration.TimeZoneInfo, cronConfiguration.CronFormat)
        {
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Start Job" + " Start Time : " + DateTime.UtcNow);
            //Service.Sync.StartSync();
            return base.StartAsync(cancellationToken);
        }


        protected override Task ScheduleJob(CancellationToken cancellationToken)
        {
            logger.LogInformation("Schedule Job" + " Start Time : " + DateTime.UtcNow);
            ///Service.Sync.StartSync();
            try
            {
                Log.Debug("Check Pulse running!");
                _ = Pulse.Instance.CheckPulse().Result;

                var housekeepTime = (DateTime.Now.TimeOfDay - GlobalEnv.Instance.HousekeepTime);
                if ((housekeepTime.TotalMinutes >= 0) && (housekeepTime.TotalMinutes < 6))
                {
                    Housekeep.CleanPulseHistory();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                //ExceptionHandler.Email("ScheduleJob", $"Pulse Check failed! {ex.Message}").Wait();
            }
            return base.ScheduleJob(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            logger.LogInformation("Working Job" + " Start Time : " + DateTime.UtcNow);
            return base.DoWork(cancellationToken);
        }

      
    }
}
