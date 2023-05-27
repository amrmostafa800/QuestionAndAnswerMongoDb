using BCryptNet = BCrypt.Net.BCrypt;

namespace QuestionAndAnswerBlazor.Helpers
{
    public class PasswordHelper
    {
        // Generate a new salt for each password hash
        private const int SaltWorkFactor = 12;

        public static string HashPassword(string password)
        {
            // Generate a new salt
            string salt = BCryptNet.GenerateSalt(SaltWorkFactor);

            // Hash the password using the salt
            string hashedPassword = BCryptNet.HashPassword(password, salt);

            return hashedPassword;
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Verify the password using the hashed password
            bool isMatch = BCryptNet.Verify(password, hashedPassword);

            return isMatch;
        }
    }
}
