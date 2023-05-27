using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace QuestionAndAnswerMongoDb.Attrbutes
{
    public class PasswordValidatorAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var ValueAsString = value as string;

            if (ValueAsString != null)
            {
                var Result = Regex.Match(ValueAsString, @"^(?=.*[a-z])(?=.*[A-Z]).{8,}$");
                if (Result.Success)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
