using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using QuestionAndAnswerMongoDb.DTOs;
using QuestionAndAnswerMongoDb.Models;
using QuestionAndAnswerMongoDb.Services;
using System.Security.Claims;

namespace QuestionAndAnswerMongoDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        QuestionsService _questions;

        public QuestionsController(QuestionsService questionsService)
        {
            _questions = questionsService;
        }

        private string _GetUserId()
        {
            return User.FindFirstValue("ID")!;
        }

        [HttpGet("GetQuestion")]
        [Authorize]
        public IActionResult GetQuestion(string QuestionID)
        {
            var Result = _questions.GetQuestionById(new ObjectId(QuestionID));
            if (Result != null)
            {
                return Ok(Result);
            }
            return BadRequest("This Question Not Exist Maybe Deleted");
        }

        [HttpPost("AddQuestion")]
        [Authorize]
        public IActionResult AddQuestion(QuestionDTO question)
        {
            question.UserId = _GetUserId()!;
            try
            {
                _questions.AddQuestion(question.Text,question.UserId);
            }
            catch (Exception)
            {
                return NotFound("Unkown Error");
            }
            return Ok("True");
        }

        [HttpPut("AddAnswer/{questionID:Length(24)}")]
        [Authorize]
        public IActionResult AddAnswer(string questionID, AnswerDTO Answer)
        {
            Answer.UserId = _GetUserId()!;
            try
            {
                var Result = _questions.AddAnswer(questionID, Answer);
                if (!Result)
                {
                    return NotFound("Unkown Error");
                }
            }
            catch (Exception)
            {
                return NotFound("Unkown Error");
            }
            return Ok("True");
        }

        [HttpDelete("DeleteQuestion/{QuestionId:Length(24)}")]
        [Authorize]
        public IActionResult DeleteQuestion(string QuestionId)
        {
            var Result = _questions.CheckUseridOfQuestionIsSameThisUserID(ObjectId.Parse(QuestionId), _GetUserId());
            if (Result)
            {
                try
                {
                    _questions.DeleteQuestion(ObjectId.Parse(QuestionId));
                    return Ok("true");
                }
                catch (Exception)
                {
                    return BadRequest("Unknown Error");
                }
            }
            return BadRequest("Not Allowed");
        }

        [HttpDelete("DeleteAnswer/{QuestionId:Length(24)}/{AnswerId:Length(24)}")]
        [Authorize]
        public IActionResult DeleteAnswer(string QuestionId, string AnswerId)
        {
            var Result = _questions.CheckUseridOfAnswerIsSameThisUserID(ObjectId.Parse(QuestionId), ObjectId.Parse(AnswerId), _GetUserId());
            if (Result)
            {
                try
                {
                    _questions.DeleteAnswer(QuestionId, AnswerId);
                    return Ok("true");
                }
                catch (Exception)
                {
                    return BadRequest("Unknown Error");
                }
            }
            return BadRequest("Not Allowed");
        }

        [HttpPut("AddVote")]
        [Authorize]
        public IActionResult Vote(VoteDTO Vote)
        {
            var Result = _questions.Vote(Vote.QuestionId, Vote.AnswerId,_GetUserId(), Vote.IsUpVote);

            if (Result) 
            {
                return Ok("true");
            }
            return Ok("False");
        }

        [HttpDelete("UnVote")]
        [Authorize]
        public IActionResult UnVote(VoteDTO Vote)
        {
            var Result = _questions.Unvote(Vote.QuestionId, Vote.AnswerId, _GetUserId(),Vote.IsUpVote);

            if (Result)
            {
                return Ok("true");
            }
            return Ok("False");
        }

        [HttpPost("AddComment")]
        [Authorize]
        public IActionResult AddComment(CommentDTO comment)
        {
            var Result = _questions.AddComment(comment.QuestionId, comment.AnswerId, _GetUserId(), comment.Comment ?? "");

            if (Result)
            {
                return Ok("true");
            }
            return Ok("False");
        }

        [HttpDelete("DeleteComment/{CommentID:Length(24)}")]
        [Authorize]
        public IActionResult DeleteComment(string CommentID,CommentDTO comment)
        {
            var Result = _questions.DeleteComment(comment.QuestionId, comment.AnswerId, _GetUserId(), CommentID);

            if (Result)
            {
                return Ok("true");
            }
            return Ok("False");
        }
    }
}
