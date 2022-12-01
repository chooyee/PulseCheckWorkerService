using MongoDB.Driver;

namespace PulseCheckWorkerService.Factory
{
    public class EmailLog
    {
        public async static Task<Model.EmailHistory> Log(Model.EmailHistory history)
        {
            return await GlobalDBService.Instance.MasterDBService.InsertOneAsync(ReflectionFactory.GetTableAttribute(typeof(Model.EmailHistory)), history);
        }

        public async static Task<List<Model.EmailHistory>> GetLog(string funcName)
        {
            var sort = Builders<Model.EmailHistory>.Sort.Descending("_id");
            var builder = Builders<Model.EmailHistory>.Filter;
            var filter = builder.Eq(x => x.FuncName, funcName);
            var tableName = ReflectionFactory.GetTableAttribute(typeof(Model.EmailHistory));
            return await GlobalDBService.Instance.MasterDBService.FindAsync(tableName, filter, sort);
        }
    }
}
