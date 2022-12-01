using MongoDB.Driver;
using PulseCheckWorkerService.Model;
using PulseCheckWorkerService.Util;
using Serilog;
using System.Diagnostics;
using System.Transactions;

namespace PulseCheckWorkerService.Factory
{
    public class Account
    {
        public static Account Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly Lazy<Account> lazy = new Lazy<Account>();

        protected readonly string TableName;

        public Account()
        {
            TableName = ReflectionFactory.GetTableAttribute(typeof(Model.Account));
        }

        internal async ValueTask<Tuple<bool, string>> CreateAccount(AccountSignup newAccount)
        {  
            if (string.IsNullOrEmpty(newAccount.AccountName)) throw new ArgumentNullException(nameof(newAccount.AccountName));

            var existingAcc = await GetAccountInfo(accountName: newAccount.AccountName);
            if (existingAcc != null) throw new Exception($"Account {newAccount.AccountName} already exists!");

            try
            {
                Model.Account newAcc = (Model.Account)newAccount;
                newAcc.CreatedDate = DateTime.Now;
                newAcc.Status = EnumHelper.AccountStatus.Active.ToString();
                newAcc = await GlobalDBService.Instance.MasterDBService.InsertOneAsync(TableName, newAcc);
                return Tuple.Create(true, EnumHelper.ResultStatus.Success.ToString());
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(string.Format("{0}:{1}", funcName, ex.Message));
            }


        }

        internal async ValueTask<Model.Account> GetAccountInfo(string accountName)
        {
            if (string.IsNullOrEmpty(accountName)) throw new ArgumentException(nameof(accountName));

            try
            {
                var builder = Builders<Model.Account>.Filter;
                var filter = builder.Eq(x => x.AccountName, accountName);
                var result = await GlobalDBService.Instance.MasterDBService.FindAsync(TableName, filter);

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
               
                throw new Exception(string.Format("{0}:{1}", funcName, ex));
            }
        }

        internal async ValueTask<List<Model.Account>> GetAccountInfo(EnumHelper.AccountStatus accountStatus)
        {
            try
            {
                var builder = Builders<Model.Account>.Filter;
                var filter = builder.Eq(x => x.Status, accountStatus.ToString());
                var result = await GlobalDBService.Instance.MasterDBService.FindAsync(TableName, filter);

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
