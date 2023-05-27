using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace QuestionAndAnswerMongoDb.Attrbutes
{
    public class EmailValidatorAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var ValueAsString = value as string;

            if (ValueAsString != null)
            {
                var Result = Regex.Match(ValueAsString, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                if (Result.Success)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
