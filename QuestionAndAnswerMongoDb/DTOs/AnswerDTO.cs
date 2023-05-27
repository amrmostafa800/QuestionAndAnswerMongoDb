using System.Text.Json.Serialization;

namespace QuestionAndAnswerMongoDb.DTOs
{
    public class AnswerDTO
    {
        public string AnswerText { get; set; } = null!;

        [JsonIgnore]
        public string? UserId { get; set; } = null!;
    }
}
