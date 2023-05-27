using System.Text.Json;
using System.Text;
using QuestionAndAnswerMongoDb.DTOs;
using MongoDB.Driver.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Net;

namespace QuestionAndAnswerMongoDb.Helpers
{
    public static class EmailSender
    {
        public static void SendValidationCodeEmailAsync(string To,string Code)
        {
            MailMessage Email = new MailMessage("no-reply@devspace0.ml", To)
            {
                Subject = "Validation Code",
                Body = $"You Code : {Code}"
            };

            SmtpClient SmtpClient = new SmtpClient("smtp-relay.sendinblue.com", 587);
            SmtpClient.Credentials = new NetworkCredential("Smtp Email", "Smtp Password");
            SmtpClient.EnableSsl = true;

            try
            {
                SmtpClient.Send(Email);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while sending the email: " + ex.Message);
            }
            finally
            {
                SmtpClient.Dispose();
                Email.Dispose();
            }
        }
    }
}
