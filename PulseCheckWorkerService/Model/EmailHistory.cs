using MongoDB.Bson.Serialization.Attributes;
using PulseCheckWorkerService.Factory.DB;

namespace PulseCheckWorkerService.Model
{
    [Table("PulseChecker_EmailHistory")]
    public class EmailHistory:BaseMongo
    {
        public List<string> Recepients { get; set; }
        public string FuncName { get; set; }
        public string AccountName { get; set; }

        public string ErrorMsg { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; }

        public bool Status { get; set; }
    }
}
