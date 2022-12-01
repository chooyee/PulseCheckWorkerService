
using MongoDB.Bson;
using MongoDB.Driver;
using PulseCheckWorkerService.Util;
using Serilog;
using System.Diagnostics;

namespace PulseCheckWorkerService.Factory
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
            var funcName = new StackFrame().GetMethod().DeclaringType.FullName;
            try
            {
                var accountList = await AccountFactory.GetAccountInfo(EnumHelper.AccountStatus.Active);
                var pulseCheckResult = new List<Model.PulseCheckResult>();

                Log.Information($"Check Pulse: Found {accountList.Count()} accounts");
                foreach (var acc in accountList)
                {
                    Log.Information($"Check Pulse for {acc.AccountName}");
                    
                    var dateCreated = acc.CreatedDate;

                    try
                    {
                        var pulseList = await PullPulse(acc.AccountName);
                        
                        if (pulseList.Any())
                        {
                            dateCreated = pulseList.First().CreatedDate;
                        }

                        var timeDiff = (DateTime.Now - dateCreated);

                        Log.Information($"Last pulse time difference: {timeDiff.TotalMinutes} minutes");

                        var pcr = new Model.PulseCheckResult()
                        {
                            AccountName = acc.AccountName,
                            Frequency = acc.Frequency,
                            DiffInMinutes = timeDiff.TotalMinutes,
                            LastRegistered = dateCreated
                        };

                        pulseCheckResult.Add(pcr);

                        if (timeDiff.TotalMinutes >= (acc.Frequency * 3))
                        {
                            await ExceptionHandler.Email(funcName, acc.AccountName, $"No pulse from {acc.AccountName} more than {timeDiff.TotalMinutes} minutes. Last recorded pulse is {dateCreated}");
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
