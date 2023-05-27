using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using QuestionAndAnswerMongoDb.Attrbutes;

namespace QuestionAndAnswerMongoDb.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public IEnumerable<PasswordReset>? PasswordReset { get; set; }

    }
}
