using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace QuestionAndAnswerMongoDb.Models
{
    public class Answer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        public string Text { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;
        
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        
        public int? UpVote { get; set; } = 0;
        
        public int? DownVote { get; set; } = 0;

        public IEnumerable<Vote>? Votes { get; set; } = new List<Vote>();

        public IEnumerable<Comment>? Comments { get; set; } = new List<Comment>();

    }
}
