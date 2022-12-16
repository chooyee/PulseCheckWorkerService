
using MongoDB.Bson;
using MongoDB.Driver;
using Util;
using Serilog;
using System.Diagnostics;
using PulseCheckWorkerService;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using Global;

namespace Factory
{
    public class Pulse
    {
        protected readonly string TableName;
        protected readonly Account AccountFactory;
        protected readonly int DiffMinutes;

        public static Pulse Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly Lazy<Pulse> lazy = new Lazy<Pulse>();

        public Pulse()
        {
            TableName = ReflectionFactory.GetTableAttribute(typeof(Model.Pulse));
            AccountFactory = new Account();
            DiffMinutes = GlobalEnv.Instance.DiffMinutes;
        }


        public async ValueTask<Model.Pulse> RegisterPulse(string accountName)
        {  
            var acc = await AccountFactory.GetAccountInfo(accountName);
            if (acc == null) throw new ArgumentException($"{accountName} is not registered");

            try
            {
                var pulse = new Model.Pulse();
                pulse.AccountName = acc.AccountName;
                pulse.CreatedDate = DateTime.Now;

                return await GlobalDBService.Instance.MasterDBService.InsertOneAsync(TableName, pulse);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                await ExceptionHandler.Email(funcName, ex.Message);
                throw new Exception($"Failed to register pulse: AccountName:{accountName} Error Message: {ex.Message}");
            }
            
        }

        public async ValueTask<List<Model.PulseCheckResult>> CheckPulse()
        {
            var funcName = "CheckPulse";
            try
            {
                var accountList = await AccountFactory.GetAccountInfo(EnumHelper.AccountStatus.Active);
                
                var pulseCheckResult = new List<Model.PulseCheckResult>();

                Log.Information($"Check Pulse: Found {accountList.Count()} accounts");
                foreach (var acc in accountList)
                {
                    Log.Information($"Check Pulse for {acc.AccountName}");

                    //To prevent error when there is no pulse 
                    //Set the account create date as the last pulse date
                    var dateCreated = acc.CreatedDate;

                    try
                    {
                        //Get pulses ordered by date desc
                        var pulseList = await PullPulse(acc.AccountName);
                        
                        if (pulseList.Any())
                        {
                            //get last pulse date and time
                            dateCreated = pulseList.First().CreatedDate;
                        }

                        var timeDiff = (DateTime.Now - dateCreated);

                        Log.Information($"Last pulse time difference: {timeDiff.TotalMinutes} minutes");

                        //Save the pulse check result to master list
                        pulseCheckResult.Add(new Model.PulseCheckResult()
                        {
                            AccountName = acc.AccountName,
                            Frequency = acc.Frequency,
                            DiffInMinutes = timeDiff.TotalMinutes,
                            LastRegistered = dateCreated
                        });

                        //If pulse missed to record for more than 3 times, send notification
                        if (timeDiff.TotalMinutes >= (acc.Frequency * 3))
                        {
                            var content = @$"No pulse from {acc.AccountName} more than {timeDiff.TotalMinutes} minutes. Last recorded pulse is {dateCreated}.
                            <br><table>
                            <tr><td>Path</td><td>{acc.FileName}</td></tr>
                            <tr><td>Path</td><td>{acc.WorkingDirectory}</td></tr>
                            <tr><td>Log Path</td><td>{acc.LogPath}</td></tr>
                            </table>";
                            await ExceptionHandler.Email(funcName, acc.AccountName, content);

                            //Wake up the service
                            if (!acc.IsProcessRunning())
                            {
                                await StartProcess(acc);
                                var subject = $"{acc.AccountName} started at {string.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.Now)}";
                                var mailContent = $"{acc.AccountName} started at {string.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.Now)} {acc.WorkingDirectory}{acc.FileName}";
                                await ExceptionHandler.Email($"{funcName} : StartProcess", subject, mailContent);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await ExceptionHandler.Email(funcName, acc.AccountName, $"Error checking pulse for {acc.AccountName} : {ex.Message}");
                        continue;
                    }
                }
                return pulseCheckResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Start Process via CMD
        /// </summary>
        /// <param name="sentryMember"></param>
        internal async ValueTask StartProcess(Model.Account sentryMember)
        {
            var funcName = "StartProcess";
            Log.Information("Starting process for {name}", sentryMember.AccountName);


            ProcessStartInfo cmdsi = new ProcessStartInfo("cmd.exe");

            var workingDir = sentryMember.WorkingDirectory.EndsWith("\\") ? sentryMember.WorkingDirectory : sentryMember.WorkingDirectory + "\\";
            cmdsi.WorkingDirectory = sentryMember.WorkingDirectory;
            cmdsi.Arguments = string.Format("/K {0}{1}", workingDir, sentryMember.FileName);

            try
            {
                Process cmd = Process.Start(cmdsi);
                //cmd.WaitForExit();
                cmd.Close();

               
            }
            catch (Exception ex)
            {
                //var subject = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcNname}: {error}", funcName, ex.Message);
                throw;
            }
        }

        internal async ValueTask<List<Model.Pulse>> PullPulse(string accountName)
        {
            try
            {
                var sort = Builders<Model.Pulse>.Sort.Descending("_id");
                var builder = Builders<Model.Pulse>.Filter;
                var filter = builder.Eq(x => x.AccountName, accountName);

                var result = await GlobalDBService.Instance.MasterDBService.FindAsync(TableName, filter, sort);

                return result;
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);

                throw new Exception(string.Format("{0}:{1}", funcName, ex));
            }
        }
    }
}
