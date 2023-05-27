using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using QuestionAndAnswerMongoDb.Configurations;
using QuestionAndAnswerMongoDb.DTOs;
using QuestionAndAnswerMongoDb.Models;

namespace QuestionAndAnswerMongoDb.Services
{
    public class QuestionsService
    {
        private readonly IMongoCollection<Question> _Question;

        public QuestionsService(IOptions<DatabaseSettings> DBSettings)
        {
            var Client = new MongoClient(DBSettings.Value.ConnectionString);
            var DB = Client.GetDatabase(DBSettings.Value.DatabaseName);
            _Question = DB.GetCollection<Question>("Questions");
        }

        public void AddQuestion(string Text,string UserID)
        {
            Question NewQuestion = new()
            {
                Text = Text,
                UserId = UserID
            };
            _Question.InsertOne(NewQuestion);
        }

        public Question GetQuestionById(ObjectId QuestionId)
        {
            var Question = _Question.Find(Q => Q.Id == QuestionId).SingleOrDefault();
            return Question;
        }

        public Answer GetAnswerById(ObjectId QuestionId, ObjectId AnswerId) 
        {
            var Answer = _Question.Find(Q => Q.Id == QuestionId).SingleOrDefault().Answers!.Where(A => A.Id == AnswerId).SingleOrDefault();
            if (Answer != null)
            {
                return Answer;
            }
            return null!;
        }

        public bool CheckUseridOfQuestionIsSameThisUserID(ObjectId QuestionId,string UserId)
        {
            var Question = GetQuestionById(QuestionId);
            if (Question.UserId == UserId)
            {
                return true;
            }
            return false;
        }

        public bool CheckUseridOfAnswerIsSameThisUserID(ObjectId QuestionId, ObjectId AnswerId, string UserId)
        {
            var Answer = GetAnswerById(QuestionId, AnswerId);
            if (Answer.UserId == UserId)
            {
                return true;
            }
            return false;
        }

        public void DeleteQuestion(ObjectId QuestionId)
        {
            _Question.DeleteOne(Q => Q.Id == QuestionId);
        }

        public bool AddAnswer(string questionId, AnswerDTO answer)
        {
            Answer NewAnswer = new()
            {
                Text = answer.AnswerText,
                UserId = answer.UserId!

            };

            var filter = Builders<Question>.Filter.Eq(q => q.Id, new ObjectId(questionId));
            var question = _Question.Find(filter).FirstOrDefault();

            var update = Builders<Question>.Update.Push(A => A.Answers, NewAnswer);

            var Result = _Question.UpdateOne(filter, update);
            return Result.ModifiedCount > 0;
        }

        public bool DeleteAnswer(string questionId,string AnswerId)
        {
            var filter = Builders<Question>.Filter.Eq(q => q.Id, new ObjectId(questionId));

            var update = Builders<Question>.Update.PullFilter(q => q.Answers, Builders<Answer>.Filter.Eq(a => a.Id, new ObjectId(AnswerId)));

            var Result = _Question.UpdateOne(filter, update);

            return Result.ModifiedCount > 0;
        }

        private bool _ChangeVoteValue(string QuestionId, string AnswerId,bool IsUpVote,bool IsIncrease = true)
        {
            var filter = Builders<Question>.Filter.And(
                Builders<Question>.Filter.Eq("_id", ObjectId.Parse(QuestionId)),
                Builders<Question>.Filter.Eq("Answers._id", ObjectId.Parse(AnswerId))
            );

            UpdateDefinition<Question> update;

            int Value = IsIncrease ? 1 : -1; // If IsIncrease true Will Increase 1 If False Will Decrease 1

            if (IsUpVote)
            {
                update = Builders<Question>.Update.Inc("Answers.$.UpVote", Value);
            }
            else
            {
                update = Builders<Question>.Update.Inc("Answers.$.DownVote", Value);
            }    

            var Result = _Question.UpdateOne(filter, update);

            return Result.ModifiedCount > 0;
        }

        private bool _IncreaseVoteValue(string QuestionId, string AnswerId, bool IsUpVote)
        {
            return _ChangeVoteValue(QuestionId, AnswerId, IsUpVote);
        }

        private bool _DecreaseVoteValue(string QuestionId, string AnswerId, bool IsUpVote)
        {
            return _ChangeVoteValue(QuestionId, AnswerId, IsUpVote,false);
        }

        private bool _IsVoteExist(string QuestionId, string AnswerId, string UserId)
        {
            Vote? Test = _Question.Find(Q => Q.Id == new ObjectId(QuestionId)).SingleOrDefault().Answers!.Where(A => A.Id == new ObjectId(AnswerId)).SingleOrDefault()!.Votes!.Where(V => V.UserId == new ObjectId(UserId)).SingleOrDefault();
            if (Test == null)
            {
                return false;
            }

            return true;
        }

        public bool Vote(string QuestionId,string AnswerId,string UserId,bool IsUpVote)
        {
            if (_IsVoteExist(QuestionId,AnswerId,UserId))
            {
                return false;
            }

            var Vote = new Vote()
            {
                IsUpVote = IsUpVote,
                UserId = ObjectId.Parse(UserId)
            };

            var QuestionFilter = Builders<Question>.Filter.Eq(q => q.Id, new ObjectId(QuestionId));
            var AnswerFilter = Builders<Question>.Filter.Eq("Answers._id", new ObjectId(AnswerId));

            var update = Builders<Question>.Update.Push("Answers.$.Votes", Vote);

            var Result = _Question.UpdateOne(QuestionFilter & AnswerFilter, update);

            if (Result.ModifiedCount > 0)
            {
                _IncreaseVoteValue(QuestionId, AnswerId, IsUpVote);
                return true;
            }

            return false;
        }

        public bool Unvote(string QuestionId, string AnswerId, string UserId, bool IsUpVote)
        {
            var QuestionFilter = Builders<Question>.Filter.Eq(q => q.Id, new ObjectId(QuestionId));
            var AnswerFilter = Builders<Question>.Filter.Eq("Answers._id", new ObjectId(AnswerId));

            var update = Builders<Question>.Update.PullFilter("Answers.$.Votes", Builders<Vote>.Filter.Eq(V => V.UserId, new ObjectId(UserId)));

            var Result = _Question.UpdateOne(QuestionFilter & AnswerFilter, update);

            if (Result.ModifiedCount > 0)
            {
                _DecreaseVoteValue(QuestionId, AnswerId, IsUpVote);
                return true;
            }

            return false;
        }

        public bool AddComment(string QuestionId, string AnswerId, string UserId, string Comment)
        {
            var NewComment = new Comment()
            {
                Text = Comment,
                UserId = UserId
            };

            var QuestionFilter = Builders<Question>.Filter.Eq(q => q.Id, new ObjectId(QuestionId));
            var AnswerFilter = Builders<Question>.Filter.Eq("Answers._id", new ObjectId(AnswerId));

            var update = Builders<Question>.Update.Push("Answers.$.Comments", NewComment);

            var Result = _Question.UpdateOne(QuestionFilter & AnswerFilter, update);

            if (Result.ModifiedCount > 0)
            {
                return true;
            }

            return false;
        }

        public bool DeleteComment(string QuestionId, string AnswerId,string UserID,string CommentID)
        {
            var QuestionFilter = Builders<Question>.Filter.Eq(q => q.Id, new ObjectId(QuestionId));
            var AnswerFilter = Builders<Question>.Filter.Eq("Answers._id", new ObjectId(AnswerId));
            var UserFilter = Builders<Question>.Filter.Eq("Answers.$.Comments.$.UserId", new ObjectId(UserID)); // to Sure Comment Who User Try Delete His comment and not another comment

            var update = Builders<Question>.Update.PullFilter("Answers.$.Comments", Builders<Comment>.Filter.Eq(C => C.Id, new ObjectId(CommentID)) & Builders<Comment>.Filter.Eq(C => C.UserId, UserID));

            var Result = _Question.UpdateOne(QuestionFilter & AnswerFilter, update);

            if (Result.ModifiedCount > 0)
            {
                return true;
            }

            return false;
        }
    }
}
