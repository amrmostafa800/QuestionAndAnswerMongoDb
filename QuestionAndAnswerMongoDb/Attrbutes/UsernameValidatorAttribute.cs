using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace QuestionAndAnswerMongoDb.Attrbutes
{
    public class UsernameValidatorAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var ValueAsString = value as string;

            if (ValueAsString != null)
            {
                var Result = Regex.Match(ValueAsString, @"^[a-zA-Z][a-zA-Z0-9_-]{2,19}$");
                if (Result.Success)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
