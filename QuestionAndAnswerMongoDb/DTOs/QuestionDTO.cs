using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace QuestionAndAnswerMongoDb.DTOs
{
    public class QuestionDTO
    {
        public string Text { get; set; } = null!;

        [JsonIgnore]
        public string? UserId { get; set; } = null!;
    }
}
