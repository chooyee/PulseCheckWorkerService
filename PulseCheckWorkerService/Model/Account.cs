using MongoDB.Bson.Serialization.Attributes;
using Factory.DB;
using Util;
using MongoDB.Bson;

namespace Model
{

    [Table("PulseChecker_Account")]
    public class Account: AccountBase
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Status { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; }

        public static explicit operator Account(AccountSignup v)
        {
            var acc = new Model.Account();
            acc.LogPath = v.LogPath;
            acc.WorkingDirectory = v.WorkingDirectory;
            acc.FileName= v.FileName;
            acc.AccountName = v.AccountName;
            acc.Frequency = v.Frequency;
            acc.Status = EnumHelper.AccountStatus.Active.ToString();
            return acc;
        }

    }
}
