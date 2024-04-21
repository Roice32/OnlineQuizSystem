using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Routing;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using OQS.CoreWebAPI.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Features.Quizzes;

public class SubmitUserResponseEndPoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/active_quiz/{active_quiz_id}",async (SubmitUserResponseRequest request,ISender sender,ApplicationDBContext dbContext) =>
        {
            var activeQuizId= active_quiz_id;
            
            //verificam daca exista active quiz-ul
            var activeQuiz= await dbContext.ActiveQuizzes.FindAsync(activeQuizId);
            if(activeQuiz==null)
            {
                return Result.Failure(activeQuiz.Error ?? new Error(400,"Active quiz not found."));
            }

            //verificam daca userul care da submit este acelasi user care a inceput quiz-ul
            if(activeQuiz.User.Id!=request.UserId)
            {
                return Result.Failure(new Error(400,"Unauthorized. User does not match the one who started the quiz."));
            }

            //aici updatez active quiz-ul cu raspunsurile userului
            activeQuiz.UserResponses = new QuizResponse(activeQuiz.Quiz);
            activeQuiz.UserResponses.Questions = request.Questions.Select(q => new QuestionResponse(q)).ToList();
            activeQuiz.SubmissionTime = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            var verificationData = FormatVerificationData(activeQuiz);

            return Result.Success(verificationData);
        });
    }
    private object FormatVerificationData(ActiveQuiz activeQuiz)
    {
        return new
        {
            ActiveQuizId=activeQuiz.Id,
            User=activeQuiz.User.UserName,
            SubmissionTime=activeQuiz.SubmissionTime,
            Quiz= new QuizResponse(activeQuiz.Quiz),
            Questions = activeQuiz.UserResponses.Questions.Select(q => new 
            {
                Id = q.Id,
                QuestionType = q.Type,
                Text = q.Text,
                Choices = q.Choices
            }).ToList()
        };
    }
}