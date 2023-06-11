using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QuestionAndAnswerMongoDb.DTOs;
using QuestionAndAnswerMongoDb.Helpers;
using QuestionAndAnswerMongoDb.Models;
using QuestionAndAnswerMongoDb.Services;
using QuestionAndAnswerBlazor.Helpers;

namespace QuestionAndAnswerMongoDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        AccountsService _accountsService;
        private IConfiguration _configuration;
        readonly JwtHelper _JWT;

        public AccountsController(AccountsService accountsService, IConfiguration configuration)
        {
            _accountsService = accountsService;
            _configuration = configuration;
            _JWT = new JwtHelper(_configuration.GetSection("JWT:Key").Value ?? throw new InvalidOperationException("JWT 'Key' not found."));
        }

        [HttpPost("Register")]
        public IActionResult Register(RegisterDTO user)
        {
            try
            {
                var Result = _accountsService.Add(user.Username,user.Password,user.Email);

                switch (Result)
                {
                    case Enums.RegisterResult.Success:
                        return Ok("Registered Successfully");
                    case Enums.RegisterResult.UserNameIsExist:
                        return BadRequest("UserName Is Exist");
                    case Enums.RegisterResult.EmailIsExist:
                        return BadRequest("Email Is Exist");
                    default:
                        return BadRequest("Unknown Error");
                }
            }
            catch (Exception)
            {
                return NotFound("Unkown Error");
            }
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginDTO user)
        {
            var Result = _accountsService.ValidateCredentials(user.Username, user.Password);

            if (Result != null)
            {
                var JwtToken = _JWT.GenerateToken(Result);
                return Ok(JwtToken); 
            }
            return BadRequest("Username Or Password is Invalid");
        }

        [HttpPost("SendPasswordResetToken")]
        public IActionResult SendPasswordResetToken(SendPasswordResetTokenDTO passwordReset)
        {
            var PasswordResetToken = TokenHelper.GenerateRandomToken(8);
            _accountsService.AddPasswordResetToken(passwordReset.Email, PasswordResetToken);
            EmailSender.SendValidationCodeEmailAsync(passwordReset.Email, PasswordResetToken);
            return Ok("If your email exists, a confirmation code has been sent to your email");
        }

        [HttpPost("PasswordReset")]
        public IActionResult PasswordReset(PasswordResetDTO passwordReset)
        {
            var isValidResetToken = _accountsService.ValidatePasswordResetToken(passwordReset.Email, passwordReset.ResetToken);
            if (isValidResetToken)
            {
                var Result = _accountsService.ChangePassword(passwordReset.Email, passwordReset.NewPassword);
                switch (Result)
                {
                    case Enums.ChangePasswordResult.Success:
                        return Ok("Password Changed");
                    case Enums.ChangePasswordResult.OldPasswordSameNewPassword:
                        return BadRequest("This Is Same You Current Password - If You Still Want To Reset Click Forgot Password Again");
                    case Enums.ChangePasswordResult.UnknownError:
                        return BadRequest("Unknown Error Please Contact Admin");
                    default:
                        return BadRequest("Unknown Error Please Contact Admin");
                }
            }
            return BadRequest("Email Or Reset Code Not Valid");

        }
    }
}
