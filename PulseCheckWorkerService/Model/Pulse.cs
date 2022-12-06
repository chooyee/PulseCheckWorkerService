using MongoDB.Bson.Serialization.Attributes;
using Factory.DB;

namespace Model
{
    [Table("PulseChecker_Pulse")]
    public class Pulse:BaseMongo
    {
        public string AccountName { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; }
    }
}
