
using MongoDB.Driver;
using Serilog;
using Global;

namespace Factory
{
    internal static class Housekeep
    {
        internal static Tuple<long,string> CleanPulseHistory()
        {
            try
            {
                Log.Information("CleanPulseHistory start");
                var dateFilter = DateTime.Now.AddDays(-30);
                var pulseTable = ReflectionFactory.GetTableAttribute(typeof(Model.Pulse));
                var builder = Builders<Model.Account>.Filter;
                var filter = builder.Lt(x => x.CreatedDate, dateFilter);

                var deleteResult = GlobalDBService.Instance.MasterDBService.DeleteMany(pulseTable, filter);
                Log.Information($"Delete Pulse Log Count: {deleteResult}");
                var compactResult = GlobalDBService.Instance.MasterDBService.Compact(pulseTable);
                Log.Information("CleanPulseHistory: " + compactResult);

                return Tuple.Create(deleteResult, compactResult);
            }
            catch (Exception ex)
            {
                Log.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name}: {ex.Message}");
                throw;
            }
        }
    }
}
