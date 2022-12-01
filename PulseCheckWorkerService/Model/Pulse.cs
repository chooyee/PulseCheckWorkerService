using MongoDB.Bson.Serialization.Attributes;
using PulseCheckWorkerService.Factory.DB;

namespace PulseCheckWorkerService.Model
{
    [Table("PulseChecker_Pulse")]
    public class Pulse:BaseMongo
    {
        public string AccountName { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; }
    }
}
