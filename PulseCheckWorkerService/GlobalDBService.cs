using ConfigurationManager = System.Configuration.ConfigurationManager;
using Cryptolib2;
using MongoDB.Driver.Core.Configuration;
using Factory.DB;

namespace PulseCheckWorkerService
{
    public sealed class GlobalDBService
    {
        public static GlobalDBService Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly Lazy<GlobalDBService> lazy = new Lazy<GlobalDBService>();
        private readonly string _masterConStr;
        private readonly string _defaultDB;
        private readonly MongoDbService _masterDB;

        public GlobalDBService()
        {
            _defaultDB = ConfigurationManager.AppSettings["default_db"];
            _masterConStr = ConfigurationManager.ConnectionStrings["mgo"].ConnectionString;
            _masterDB = new MongoDbService(_masterConStr, _defaultDB);
            _masterDB.CreateConnection();
        }

        public string MasterConnectionString { get { return _masterConStr; } }
        public MongoDbService MasterDBService { get { return _masterDB; }}
        public string DefaultDB { get { return _defaultDB; } }
      
    }

   
}
