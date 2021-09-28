using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ELODIE.Common;
using System;
using System.Collections.Generic;

namespace ELODIE.Entities
{
    public class RequestHistory<T> : DataEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public long RequestId { get; set; }
        public DateTime SavedAt { get; set; }
        public T OldContent { get; set; }
        public T Content { get; set; }
        public Dictionary<string, object> DisplayFields { get; set; }
        public long? AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
