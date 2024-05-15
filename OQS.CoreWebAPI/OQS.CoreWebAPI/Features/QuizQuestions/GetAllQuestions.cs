using Carter;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.QuizQuestions;
using OQS.CoreWebAPI.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Features.QuizQuestions
{
    public static class GetAllQuestions
    {
        public record Query(Guid QuizId, int Limit, int Offset) : IRequest<Result<List<QuestionResponse>>>;

        internal sealed class Handler : IRequestHandler<Query, Result<List<QuestionResponse>>>
        {
            private readonly ApplicationDBContext context;

            public Handler(ApplicationDBContext context)
            {
                this.context = context;
            }

            public async Task<Result<List<QuestionResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
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
                var query = new GetAllQuestions.Query(id, Limit: 10, Offset: 1);
                return await sender.Send(query);
            });
        }
    }

