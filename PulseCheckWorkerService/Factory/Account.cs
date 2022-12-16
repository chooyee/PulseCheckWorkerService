using MongoDB.Driver;
using Model;
using Util;
using Serilog;
using System.Diagnostics;
using System.Transactions;
using PulseCheckWorkerService;

namespace Factory
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
            
            Model.Account newAcc = (Model.Account)newAccount;

            try
            {
                var existingAcc = await GetAccountInfo(accountName: newAccount.AccountName);
                if (existingAcc != null)
                {
                    //throw new Exception($"Account {newAccount.AccountName} already exists!");
                    var builder = Builders<Model.Account>.Filter;
                    var filter = builder.Eq(x => x.AccountName, newAcc.AccountName);
                    var updateBuilder = Builders<Model.Account>.Update
                    .Set(x => x.Frequency, newAcc.Frequency)
                    .Set(x => x.CreatedDate, newAcc.CreatedDate)                  
                    .Set(x => x.WorkingDirectory, newAcc.WorkingDirectory)
                    .Set(x => x.FileName, newAcc.FileName)
                    .Set(x => x.LogPath, newAcc.LogPath);
                    newAcc = await GlobalDBService.Instance.MasterDBService.FindOneAndUpdateAsync(TableName, filter, updateBuilder);
                    return Tuple.Create(true, EnumHelper.ResultStatus.Success.ToString());
                }
                else
                {
               
                       
                        newAcc.CreatedDate = DateTime.Now;
                        newAcc.Status = EnumHelper.AccountStatus.Active.ToString();
                        newAcc = await GlobalDBService.Instance.MasterDBService.InsertOneAsync(TableName, newAcc);
                        return Tuple.Create(true, EnumHelper.ResultStatus.Success.ToString());
                
                }
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

        internal async ValueTask<long> DeleteAccount(string accountName)
        {
            if (string.IsNullOrEmpty(accountName)) throw new ArgumentException(nameof(accountName));

            try
            {
                var builder = Builders<Model.Account>.Filter;
                var filter = builder.Eq(x => x.AccountName, accountName);
                return await GlobalDBService.Instance.MasterDBService.DeleteOneAsync(TableName, filter);

            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);

                throw new Exception(string.Format("{0}:{1}", funcName, ex));
            }
        }

        internal async ValueTask<bool> DeactivateAccount(string accountName)
        {
            if (string.IsNullOrEmpty(accountName)) throw new ArgumentException(nameof(accountName));

            try
            {
                return await UpdateAccountStatus(accountName, EnumHelper.AccountStatus.Deactivated);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);

                throw new Exception(string.Format("{0}:{1}", funcName, ex));
            }
        }

        internal async ValueTask<bool> ActivateAccount(string accountName)
        {
            if (string.IsNullOrEmpty(accountName)) throw new ArgumentException(nameof(accountName));

            try
            {
                return await UpdateAccountStatus(accountName, EnumHelper.AccountStatus.Active);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);

                throw new Exception(string.Format("{0}:{1}", funcName, ex));
            }
        }

        private async ValueTask<bool> UpdateAccountStatus(string accountName, EnumHelper.AccountStatus accountStatus)
        {
            if (string.IsNullOrEmpty(accountName)) throw new ArgumentException(nameof(accountName));

            try
            {
                var builder = Builders<Model.Account>.Filter;
                var filter = builder.Eq(x => x.AccountName, accountName);
                var updateBuilder = Builders<Model.Account>.Update
                    .Set(x => x.Status, accountStatus.ToString());
                var acc = await GlobalDBService.Instance.MasterDBService.FindOneAndUpdateAsync(TableName, filter, updateBuilder);
                if (acc.Status.Equals(EnumHelper.AccountStatus.Deactivated.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
