using System.Security.Cryptography;
using System.Text;

namespace QuestionAndAnswerMongoDb.Helpers
{
    public class TokenHelper
    {
        public static string GenerateRandomToken(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            byte[] randomBytes = new byte[length];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            StringBuilder tokenBuilder = new StringBuilder(length);
            foreach (byte b in randomBytes)
            {
                tokenBuilder.Append(chars[b % chars.Length]);
            }

            return tokenBuilder.ToString();
        }
    }
}
