﻿using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Quizzes;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Quizzes
{
    public static class CreateQuiz
    {
        public record Command : IRequest<Result<Guid>>
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; }

            public string ImageUrl { get; set; }

            public string  Language { get; set; }
            public Guid CreatorId { get; set; }
            public int TimeLimitMinutes { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty();

                RuleFor(x=>x.Language).NotEmpty();
             
                RuleFor(x => x.CreatorId).NotEmpty();

                RuleFor(x=>x.TimeLimitMinutes).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly ApplicationDBContext dbContext;
            private readonly IValidator<Command> validator;

            public Handler(ApplicationDBContext dbContext, IValidator<Command> validator)
            {
                this.dbContext = dbContext;
                this.validator = validator;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<Guid>(
                        new Error("CreateQuiz.Validator",
                        validationResult.ToString()));
                }
                var quiz = new Quiz
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    ImageUrl = request.ImageUrl,
                    Language = request.Language,
                    CreatorId=request.CreatorId,
                    TimeLimitMinutes = request.TimeLimitMinutes,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.Quizzes.Add(quiz);
                await dbContext.SaveChangesAsync(cancellationToken);

                return quiz.Id;
            }
        }
    }
}

public class CreateQuizEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
         app.MapPost("api/quizzes", async (CreateQuizRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateQuiz.Command>();
            var result = await sender.Send(command);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Ok($"/api/quizzes/{result.Value}");
        });
    }
}