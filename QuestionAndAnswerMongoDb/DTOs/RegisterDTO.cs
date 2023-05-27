using QuestionAndAnswerMongoDb.Attrbutes;

namespace QuestionAndAnswerMongoDb.DTOs
{
    public class RegisterDTO : LoginDTO
    {
        //[UsernameValidator(ErrorMessage = "UserName not Suitable with Requirement")]
        //public string Username { get; set; } = null!;

        //[PasswordValidator(ErrorMessage = "Password not Suitable with Requirement")]
        //public string Password { get; set; } = null!;

        [EmailValidator(ErrorMessage = "Email not Suitable with Requirement")]
        public string Email { get; set; } = null!;
    }
}
