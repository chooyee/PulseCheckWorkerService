using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Model
{
    public class BaseMongo
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
