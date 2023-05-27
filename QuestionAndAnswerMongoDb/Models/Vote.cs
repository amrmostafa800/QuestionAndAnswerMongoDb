using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace QuestionAndAnswerMongoDb.Models
{
    public class Vote
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        public bool IsUpVote { get; set; }

        public ObjectId UserId { get; set; }
    }
}
