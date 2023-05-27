using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using QuestionAndAnswerMongoDb.Configurations;
using QuestionAndAnswerMongoDb.DTOs;
using QuestionAndAnswerMongoDb.Enums;
using QuestionAndAnswerMongoDb.Helpers;
using QuestionAndAnswerMongoDb.Models;
using QuestionAndAnswerBlazor.Helpers;
using System.Transactions;

namespace QuestionAndAnswerMongoDb.Services
{
    public class AccountsService
    {
        private readonly IMongoCollection<User> _User;

        public AccountsService(IOptions<DatabaseSettings> DBSettings)
        {
            var Client = new MongoClient(DBSettings.Value.ConnectionString);
            var DB = Client.GetDatabase(DBSettings.Value.DatabaseName);
            _User = DB.GetCollection<User>("Users");
        }

        private bool _CheckifUsernameIsExist(string Username)
        {
            var Result = _User.Find(U => U.Username == Username).SingleOrDefault();

            if (Result != null)
            {
                return true;
            }
            return false;
        }

        private bool _CheckifEmailIsExist(string Email)
        {
            var Result = _User.Find(U => U.Email == Email).SingleOrDefault();

            if (Result != null)
            {
                return true;
            }
            return false;
        }

        public RegisterResult Add(string Username,string Password,string Email)
        {
            User NewUser = new()
            {
                Username = Username.ToLower(),
                Password = PasswordHelper.HashPassword(Password),
                Email = Email.ToLower()
            };

            if (_CheckifUsernameIsExist(Username))
            {
                return RegisterResult.UserNameIsExist;
            }

            if (_CheckifEmailIsExist(Email))
            {
                return RegisterResult.EmailIsExist;
            }

            _User.InsertOne(NewUser);
            return RegisterResult.Success;
        }

        public string ValidateCredentials(string Username, string Password)
        {
            var Account = _User.Find(U => U.Username == Username).SingleOrDefault();
            if (Account != null) 
            {
                if (PasswordHelper.VerifyPassword(Password, Account.Password))
                {
                    return Account.Id.ToString();
                }
            }
            return null!;
        }

        public ChangePasswordResult ChangePassword(string Email,string NewPassword)
        {
            var NewPasswordHash = PasswordHelper.HashPassword(NewPassword);

            var Account = _User.Find(U => U.Email == Email).SingleOrDefault();
            if (Account != null)
            {
                if (PasswordHelper.VerifyPassword(NewPassword,Account.Password)) // if True Mean Old Password = New Password
                {
                    return ChangePasswordResult.OldPasswordSameNewPassword;
                }
                var filter = Builders<User>.Filter.Eq("_id", Account.Id);

                var update = Builders<User>.Update.Set("Password", NewPasswordHash);

                _User.UpdateOne(filter, update);

                return ChangePasswordResult.Success;
            }
            return ChangePasswordResult.UnknownError;
        }

        public bool ChangePassword(string Username, string OldPassword, string NewPassword)
        {
            //still not Tested
            var NewPasswordHash = PasswordHelper.HashPassword(NewPassword);

            var Account = _User.Find(U => U.Username == Username).SingleOrDefault();
            if (Account != null)
            {
                if (!PasswordHelper.VerifyPassword(OldPassword,Account.Password))
                {
                    return false;
                }

                var filter = Builders<User>.Filter.Eq("_id", Account.Id);

                var update = Builders<User>.Update.Set("Password", NewPasswordHash);

                _User.UpdateOne(filter, update);

                return true;
            }
            return false;
        }

        public bool AddPasswordResetToken(string Email,string Token)
        {
            var Account = _User.Find(U => U.Email == Email).SingleOrDefault();
            if (Account != null)
            {
                _RemoveResetToken(Account.Id); // Remove Exist Reset Token if there One
                var filter = Builders<User>.Filter.Eq("_id", Account.Id);

                var PasswordResetDTO = new PasswordReset();
                PasswordResetDTO.Token = Token;

                var update = Builders<User>.Update.Push("PasswordReset", PasswordResetDTO);

                _User.UpdateOne(filter, update);

                return true;
            }
            return false;
        }

        private void _RemoveResetToken(ObjectId AccountID)
        {
            var filter = Builders<User>.Filter.Eq("_id", AccountID);

            var update = Builders<User>.Update.Unset("PasswordReset");

            var result = _User.UpdateOne(filter, update);
        }

        public bool ValidatePasswordResetToken(string Email,string Token)
        {
            var Account = _User.Find(U => U.Email == Email).SingleOrDefault();
            if (Account.PasswordReset != null)
            {
                var ResetToken = Account.PasswordReset!.SingleOrDefault()!.Token;
                var isExpire = Account.PasswordReset!.SingleOrDefault()!.ExpireAt <= DateTime.UtcNow;
                if (!isExpire)
                {
                    if (Token == ResetToken)
                    {
                        _RemoveResetToken(Account.Id);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
