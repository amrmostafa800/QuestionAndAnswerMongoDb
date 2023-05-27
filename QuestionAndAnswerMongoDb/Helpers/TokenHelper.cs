namespace QuestionAndAnswerMongoDb.Helpers
{
    public class TokenHelper
    {
        public static string GenerateRandomToken(int Length)
        {
            Random random = new Random();

            const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] Token = new char[Length];

            for (int i = 0; i < Length; i++)
            {
                Token[i] = Chars[random.Next(Chars.Length)];
            }

            return new string(Token);
        }
    }
}
