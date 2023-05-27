namespace QuestionAndAnswerMongoDb.Models
{
    public class PasswordReset
    {
        public string Token { get; set; } = null!;
        public DateTime ExpireAt { get; set; } = DateTime.UtcNow.AddHours(1);
    }
}
