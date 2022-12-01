using PulseCheckWorkerService.Factory.EmailService;
using ConfigurationManager = System.Configuration.ConfigurationManager;
namespace PulseCheckWorkerService
{
    public sealed class GlobalEnv
    {
        public static GlobalEnv Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly Lazy<GlobalEnv> lazy = new Lazy<GlobalEnv>();
        private readonly string env;
        private readonly string appName;
        private readonly string issuer;
        private readonly int randomStringLength;
        private readonly string key;

        private readonly string[] emailServerIPs;
        private readonly ImapConfig imapConfig;
        private readonly SmtpConfig smtpConfig;

        private readonly List<string> supportEmailRecipients;
        private readonly string emailSubject;

        private readonly int diffMinutes = 60;
        private readonly int emailFreqMinutes = 10;

        private readonly string cronJob;
        public GlobalEnv()
        {          
            env = ConfigurationManager.AppSettings["env"] ?? "uat";
            appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            issuer = ConfigurationManager.AppSettings["issuer"] ?? "chooyee.co";
            randomStringLength = int.Parse(ConfigurationManager.AppSettings["randomStringLength"] ?? "6") ;
            key = ConfigurationManager.AppSettings["key"]?? "";

            emailServerIPs = ConfigurationManager.AppSettings[$"{env}.email.ip"].Split(',');

            imapConfig = new ImapConfig();
            imapConfig.Port = int.Parse(ConfigurationManager.AppSettings[$"{env}.imap.port"]);
            imapConfig.Ssl = bool.Parse(ConfigurationManager.AppSettings[$"{env}.imap.ssl"]);

            smtpConfig = new SmtpConfig();
            smtpConfig.Port = int.Parse(ConfigurationManager.AppSettings[$"{env}.smtp.port"]);
            smtpConfig.Ssl = bool.Parse(ConfigurationManager.AppSettings[$"{env}.smtp.ssl"]);
            smtpConfig.Sender = ConfigurationManager.AppSettings["smtp.sender"];

            supportEmailRecipients = ConfigurationManager.AppSettings[$"{env}.email.support"].Split(',').ToList();
            emailSubject = ConfigurationManager.AppSettings["email.subject"];

            diffMinutes = int.Parse(ConfigurationManager.AppSettings["diff_minutes"] ?? "60");
            emailFreqMinutes = int.Parse(ConfigurationManager.AppSettings["email_freq_minutes"] ?? "10");

            cronJob = ConfigurationManager.AppSettings[$"cronjob"]?? "*/10 * * * * ";

        }


        public string Environment => env;
        public string AppName => appName;
        public string Issuer => issuer;
        public int RandomStringLength => randomStringLength;
        public string Key => key;

        public ImapConfig ImapConfig => imapConfig;
        public SmtpConfig SmtpConfig => smtpConfig;
        public string[] EmailServerIPs => emailServerIPs;

        public List<string> SupportEmailRecipients => supportEmailRecipients;
        public string EmailSubject => emailSubject;
        public int DiffMinutes => diffMinutes;
        public int EmailFreqMinutes => emailFreqMinutes;

        public string CronJob => cronJob;
    }
}
