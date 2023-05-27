namespace QuestionAndAnswerMongoDb.DTOs
{
    public class CommentDTO
    {
        public string QuestionId { get; set; } = null!;
        public string AnswerId { get; set; } = null!;
        public string? Comment { get; set; } = null!;
    }
}
