namespace QuestionAndAnswerMongoDb.DTOs
{
    public class VoteDTO
    {
        public string QuestionId { get; set; } = null!;
        public string AnswerId { get; set; } = null!;
        public bool IsUpVote { get; set; }
    }
}
