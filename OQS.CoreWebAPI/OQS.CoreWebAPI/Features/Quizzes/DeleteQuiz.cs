﻿using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.Quizzes;
using OQS.CoreWebAPI.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Features.Quizzes
{
    public static class DeleteQuiz
    {
        public record Command : IRequest<Result>
        {
            public Guid Id { get; init; }
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
                var quiz = await _dbContext.Quizzes.FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);

                if (quiz == null)
                {
                    return Result.Failure(new Error("DeleteQuiz.NotFound", "Quiz not found."));
                }

                _dbContext.Quizzes.Remove(quiz);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}

public class DeleteQuizEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/quizzes/{id}", async (HttpRequest req, ISender sender, Guid id) =>
        {
            var command = new DeleteQuiz.Command { Id = id };
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok();
        });
    }
}