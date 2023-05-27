using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuestionAndAnswerMongoDb.Models
{
    public class Question
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string Text { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public IEnumerable<Answer>? Answers { get; set; } = new List<Answer>();
    }
}
