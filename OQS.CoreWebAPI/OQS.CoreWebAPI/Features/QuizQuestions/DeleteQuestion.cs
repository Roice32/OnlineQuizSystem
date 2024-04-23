﻿using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.QuizQuestions;
using OQS.CoreWebAPI.Features.Quizzes;
using OQS.CoreWebAPI.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Features.QuizQuestions
{
    public static class DeleteQuestion
    {
        public record Command : IRequest<Result>
        {
            public Guid QuizId { get; init; }
            public Guid QuestionId { get; init; }
        }

        internal sealed class Handler : IRequestHandler<Command, Result>
        {
            private readonly ApplicationDBContext _dbContext;

            public Handler(ApplicationDBContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var quiz = await _dbContext.Quizzes.FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken);

                if (quiz == null)
                {
                    return Result.Failure(new Error("DeleteQuiz.NotFound", "Quiz not found."));
                }

                var question = await _dbContext.Questions.FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

                if (question == null)
                {
                    return Result.Failure(new Error("DeleteQuiz.NotFound", "Question not found."));
                }

                _dbContext.Questions.Remove(question);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}

public class DeleteQuestionEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/quizzes/{quiz_id}/questions/{question_id}", async (HttpRequest req, ISender sender, Guid quiz_id, Guid question_id) =>
        {
            var command = new DeleteQuestion.Command { QuizId = quiz_id, QuestionId=question_id };
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok();
        });
    }
}