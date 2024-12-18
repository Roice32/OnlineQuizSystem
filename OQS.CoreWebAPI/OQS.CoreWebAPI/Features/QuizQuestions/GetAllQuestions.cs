﻿using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.QuizQuestions;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Contracts.CRUD;

namespace OQS.CoreWebAPI.Features.QuizQuestions
{
    public static class GetAllQuestions
    {
        public record Query(Guid QuizId, int Limit, int Offset) : IRequest<Result<List<QuestionResponse>>>;

        internal sealed class Handler : IRequestHandler<Query, Result<List<QuestionResponse>>>
        {
            private readonly ApplicationDbContext context;

            public Handler(ApplicationDbContext context)
            {
                this.context = context;
            }

            public async Task<Result<List<QuestionResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var totalQuestions = await context.Questions
                    .CountAsync(question => question.QuizId == request.QuizId);
                Console.WriteLine($"Total questions for QuizId {request.QuizId}: {totalQuestions}");


                var questions = await context.Questions
                    .Where(question => question.QuizId == request.QuizId)
                    .Skip(request.Offset)
                    .Take(request.Limit)
                    .Select(question => new QuestionResponse(question))
                    .ToListAsync();

                return Result.Success<List<QuestionResponse>>(questions);
            }
        }
    }
}

public class GetAllQuestionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/quizzes/{id}/questions", async (Guid id, ISender sender) =>
        {
            var query = new GetAllQuestions.Query(id, Limit: 10, Offset: 0);
            return await sender.Send(query);
        });
    }
}