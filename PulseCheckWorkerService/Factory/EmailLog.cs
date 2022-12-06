using MongoDB.Driver;
using PulseCheckWorkerService;

namespace Factory
{
    public class EmailLog
    {
        public async static Task<Model.EmailHistory> Log(Model.EmailHistory history)
        {
            return await GlobalDBService.Instance.MasterDBService.InsertOneAsync(ReflectionFactory.GetTableAttribute(typeof(Model.EmailHistory)), history);
        }

        public async static Task<List<Model.EmailHistory>> GetLogByAccountName(string funcName, string accountName)
        {
            if (string.IsNullOrEmpty(funcName)) throw new ArgumentNullException(nameof(funcName));
            if (string.IsNullOrEmpty(accountName)) throw new ArgumentNullException(nameof(accountName));

            var sort = Builders<Model.EmailHistory>.Sort.Descending("_id");
            var builder = Builders<Model.EmailHistory>.Filter;
            var filter = builder.Eq(x => x.FuncName, funcName) & builder.Eq(x => x.AccountName, accountName);
            var tableName = ReflectionFactory.GetTableAttribute(typeof(Model.EmailHistory));
            return await GlobalDBService.Instance.MasterDBService.FindAsync(tableName, filter, sort);
        }

        public async static Task<List<Model.EmailHistory>> GetLog(string funcName, string? exceptionMsg = null)
        {
            if (string.IsNullOrEmpty(funcName)) throw new ArgumentNullException(nameof(funcName));

            var sort = Builders<Model.EmailHistory>.Sort.Descending("_id");
            var builder = Builders<Model.EmailHistory>.Filter;
            var filter = builder.Eq(x => x.FuncName, funcName);
            if (!string.IsNullOrEmpty(exceptionMsg))
            {
                filter &= builder.Eq(x => x.ErrorMsg, exceptionMsg);
            }
            var tableName = ReflectionFactory.GetTableAttribute(typeof(Model.EmailHistory));
            return await GlobalDBService.Instance.MasterDBService.FindAsync(tableName, filter, sort);
        }

    }
}
