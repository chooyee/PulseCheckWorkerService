using MongoDB.Bson.Serialization.Attributes;
using Factory.DB;
using Util;

namespace Model
{

    [Table("PulseChecker_Account")]
    public class Account:BaseMongo
    {
        public string AccountName { get; set; }
        public int Frequency { get; set; }
       
        public string Status { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; }

        public static explicit operator Account(AccountSignup v)
        {
            var acc = new Model.Account();
            acc.AccountName = v.AccountName;
            acc.Frequency = v.Frequency;
            acc.Status = EnumHelper.AccountStatus.Active.ToString();
            return acc;
        }

    }
}
