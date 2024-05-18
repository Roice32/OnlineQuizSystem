using Carter;
using FluentValidation;
using MediatR;
using OQS.CoreWebAPI.Contracts.LiveQuizzes;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.LiveQuizzes;

public class CreateLiveQuiz
{
    public record Query(Guid QuizId, Guid UserId) : IRequest <Result<LiveQuizz>>;

    public class Validator : AbstractValidator<Query>
    {
       private readonly IServiceScopeFactory _serviceScopeFactory;
        public Validator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            RuleFor(x => x.QuizId)
                .MustAsync(QuizExists)
                .WithMessage("Invalid Quiz Id");

            RuleFor(x => x.UserId)
                .MustAsync(UserExists)
                .WithMessage("Invalid User Id");
        }

        private async Task<bool> QuizExists(Guid quizId, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            return await context.Quizzes.FindAsync(quizId, cancellationToken) != null;
        }

        private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            return await context.Users.FindAsync(userId, cancellationToken) != null;
        }
    }


    internal sealed class Handler : IRequestHandler<Query, Result<LiveQuizz>>
    {
        private readonly ApplicationDBContext _context;
        private readonly IValidator<Query> _validator;
        
        public Handler(ApplicationDBContext context,IValidator<Query> validator)
        {
            _context = context;
            _validator = validator;
        }
        
        public async Task<Result<LiveQuizz>> Handle(Query request, CancellationToken cancellationToken)
        {
            var validatorResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validatorResult.IsValid)
            {
                return Result.Failure<LiveQuizz>(new Error("CreateLiveQuiz.BadRequest",
                    validatorResult.ToString()));
            }

            var quiz = await _context.Quizzes.FindAsync(request.QuizId, cancellationToken);
            var user = await _context.Users.FindAsync(request.UserId, cancellationToken);
            
            if (quiz == null || user == null)
            {
                return Result.Failure<LiveQuizz>(new Error("CreateLiveQuiz.NotFound",
                    "Quiz or User not found"));
            }
    
            var liveQuiz = new LiveQuizz
            {
                Code = LiveQuizCode.Generate(),
                Quiz = quiz,
                CreatedBy = user,
            };

            _context.LiveQuizzes.Add(liveQuiz);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(liveQuiz);
        }
        
    }
  
}

public class CreateLiveQuizEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/live-quizzes",async(CreateLiveQuizRequest request, ISender sender) =>
        {
            var liveQuizCreation = new CreateLiveQuiz.Query(request.QuizId, request.UserId);
            var result = await sender.Send(liveQuizCreation);
                
            return result.IsFailure ? (Result)Result.Failure<LiveQuizz>(result.Error) : Result.Success(result.Value.Code);
        });
    }
}