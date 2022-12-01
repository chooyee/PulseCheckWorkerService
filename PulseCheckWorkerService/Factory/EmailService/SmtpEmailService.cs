using MailKit.Net.Smtp;
using MimeKit;
using PulseCheckWorkerService.Util;
using Serilog;

namespace PulseCheckWorkerService.Factory.EmailService
{
    public class SmtpEmailService:EmailService
    {
        private readonly string EmailSenderName = "Pulse Checker Server";

        public SmtpEmailService()
        {
            
        }


        public bool SendEmail(string smtpIp, List<string> recipients, string subject, string body, List<string> attachments=null, List<string> ccReceipients = null)
        {
            try
            {
                //LogHandler.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name, "Start Sending Email");
                MimeMessage message = new MimeKit.MimeMessage();
                message.From.Add(new MailboxAddress(EmailSenderName, smtpConfig.Sender));

                foreach (string recipient in recipients)
                {
                    message.To.Add(new MailboxAddress("", recipient));
                }

                if (ccReceipients != null)
                {
                    foreach (var cc in ccReceipients)
                    {
                        message.Cc.Add(new MailboxAddress("", cc));
                    }
                }

                message.Subject = $"[{GlobalEnv.Instance.Environment}] - {subject}";

                BodyBuilder bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };

                if (attachments != null)
                {
                    foreach (string attachment in attachments)
                    {
                        Console.WriteLine(attachment);
                        bodyBuilder.Attachments.Add(attachment);
                    }
                }

                message.Body = bodyBuilder.ToMessageBody();

                if (GlobalEnv.Instance.Environment==EnumHelper.Environment.Development)
                {
                    using (var client = new SmtpClient())
                    {
                        client.Connect(emailServerIPs[0], smtpConfig.Port, MailKit.Security.SecureSocketOptions.StartTls);

                        ////Note: only needed if the SMTP server requires authentication
                        client.Authenticate("chooyee8527@gmail.com", "Starbuck$1916");

                        client.Send(message);
                        client.Disconnect(true);
                    }
                }
                else
                {
                    using (var client = new SmtpClient())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.CheckCertificateRevocation = false;
                        client.Connect(smtpIp, smtpConfig.Port, MailKit.Security.SecureSocketOptions.None);
                        client.Send(message);
                        client.Disconnect(true);
                    }
                }

                //LogHandler.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name, "Sending Email Complete!");
                return true;
            }
            catch (Exception ex)
            {
                var logMsg = string.Format("SMTP IP : {0} : Port : {1} : SSL : {2} : Error : {3}", smtpIp, smtpConfig.Port, smtpConfig.Ssl, ex.Message);
                throw (new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + logMsg));
            }
        }

        public static bool SendMail(List<string> recipients, string subject, string body, List<string> attachments=null, List<string> ccReceipients = null)
        {
            try
            {
                if (GlobalEnv.Instance.Environment == EnumHelper.Environment.Development) return true;

                var result = false;
                using (var smtp = new SmtpEmailService())
                {
                    foreach (var smtpIP in smtp.emailServerIPs)
                    {
                        try
                        {
                            result = smtp.SendEmail(smtpIP, recipients, subject, body, attachments, ccReceipients);
                            if (result)
                                break;
                        }
                        catch (Exception ex)
                        {
                            Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + ": " + ex.Message);
                            continue;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + ": " + ex.Message);
                return false;
            }
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EmailService()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
