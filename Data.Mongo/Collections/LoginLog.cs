using Data.Mongo.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Data.Mongo.Collections
{
    [BsonCollection("loginlogs")]
    [BsonIgnoreExtraElements]
    public class LoginLog : MongoBaseDocument
    {
        public string UserName { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LoginTime { get; set; }
    }
}
