using Factory.EmailService;
using Model;
using PulseCheckWorkerService;
using Serilog;
using Global;

namespace Factory
{
    public class ExceptionHandler
    {
        /// <summary>
        /// Send Email
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="exceptionMsg"></param>
        public async static Task Email(string funcName, string exceptionMsg)
        {
            var recipients = GlobalEnv.Instance.SupportEmailRecipients;
            var subject = GlobalEnv.Instance.EmailSubject.Replace("<funcName>", funcName);
            var body = funcName + ":" + exceptionMsg;

            bool status = false;
            if (await SendEmailFrequencyCheck(funcName, exceptionMsg))
            {
                status = SmtpEmailService.SendMail(recipients, subject, body);
                await LogSentEmail(funcName, "", exceptionMsg, recipients, status);
            }

            Log.Error($"{funcName}:{exceptionMsg}");

            return;
        }

        /// <summary>
        /// Send Email
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="exceptionMsg"></param>
        public async static Task Email(string funcName, string accountName, string exceptionMsg)
        {
            var recipients = GlobalEnv.Instance.SupportEmailRecipients;
            var subject = $"{GlobalEnv.Instance.EmailSubject.Replace("<funcName>", funcName)} - {accountName}";
            var body = $"{subject} - {exceptionMsg}";

            bool status = false;
            if (await PulseEmailFrequencyCheck(funcName, accountName))
            {
                status = SmtpEmailService.SendMail(recipients, subject, body);
                await LogSentEmail(funcName, accountName, exceptionMsg, recipients, status);
                Log.Error($"{funcName} : {accountName} : {exceptionMsg}");
            }

            return;
        }

        private async static Task LogSentEmail(string funcName, string accountName, string exceptionMsg, List<string> recipients, bool status)
        {
            var history = new Model.EmailHistory()
            {
                CreatedDate = DateTime.Now,
                ErrorMsg = exceptionMsg,
                FuncName = funcName,
                AccountName = accountName,
                Recepients = recipients,
                Status = status
            };
            await EmailLog.Log(history);

            return;
        }


        private async static ValueTask<bool> SendEmailFrequencyCheck(string funcName, string exceptionMsg)
        {
            var emailLogs = await EmailLog.GetLog(funcName, exceptionMsg);
            if (!emailLogs.Any()) return true;

            var lastLog = emailLogs.First();

            //Check is the same exception
            if (lastLog.ErrorMsg == exceptionMsg)
            {
                var timeDiff = (DateTime.Now - lastLog.CreatedDate);
                if (timeDiff.TotalMinutes >= GlobalEnv.Instance.EmailFreqMinutes)
                {
                    return true;
                }
                else { return false; }
            }
            //Different exception
            else
                return true;
            
            
        }

        private async static ValueTask<bool> PulseEmailFrequencyCheck(string funcName, string accountName)
        {
            var emailLogs = await EmailLog.GetLogByAccountName(funcName, accountName);
            if (!emailLogs.Any()) return true;

            var lastLog = emailLogs.First();

            //Check is the same exception
            if (lastLog.AccountName.Equals(accountName,StringComparison.OrdinalIgnoreCase))
            {
                var timeDiff = (DateTime.Now - lastLog.CreatedDate);
                if (timeDiff.TotalMinutes >= GlobalEnv.Instance.EmailFreqMinutes)
                {
                    return true;
                }
                else { return false; }
            }
            //Different exception
            else
                return true;


        }

    }
}
