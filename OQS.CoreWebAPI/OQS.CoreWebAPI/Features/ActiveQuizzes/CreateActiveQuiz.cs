using System.Net;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features;

public class CreateActiveQuiz
{
    public record QuizCreation(Guid QuizId, Guid TakenBy) : IRequest <Result<ActiveQuiz>>;
   
        private readonly ApplicationDBContext _context;

        public CreateActiveQuiz(ApplicationDBContext context)
        {
            _context = context;
        }

        public class QuizCreationValidator : AbstractValidator<QuizCreation>
        {
           // private readonly ApplicationDBContext _dbContext;
           private readonly IServiceScopeFactory _serviceScopeFactory;
            public QuizCreationValidator(IServiceScopeFactory serviceScopeFactory)
            {
                _serviceScopeFactory = serviceScopeFactory;
                RuleFor(x => x.QuizId)
                    .MustAsync(QuizExists)
                    .WithMessage("Invalid Quiz Id");

                RuleFor(x => x.TakenBy)
                    .MustAsync(UserExists)
                    .WithMessage("Invalid User Id");
            }

            private async Task<bool> QuizExists(Guid quizId, CancellationToken cancellationToken)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                return await context.Quizzes.FindAsync(quizId) != null;
            }

            private async Task<bool> UserExists(Guid takenBy, CancellationToken cancellationToken)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                return await context.Users.FindAsync(takenBy) != null;
            }
        }


    internal sealed class Handler : IRequestHandler<QuizCreation, Result<ActiveQuiz>>
    {
        private readonly ApplicationDBContext _context;
        private readonly QuizCreationValidator _validator;
        
        public Handler(ApplicationDBContext context,QuizCreationValidator validator)
        {
            _context = context;
            _validator = validator;
        }
        
        public async Task<Result<ActiveQuiz>> Handle(QuizCreation request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<ActiveQuiz>(
                    new Error("CreateActiveQuiz.BadRequest", 
                        validationResult.ToString()));
            }

            var quiz = await _context.Quizzes.FindAsync(request.QuizId);
            var user = await _context.Users.FindAsync(request.TakenBy);
    
            var activeQuiz = new ActiveQuiz
            {
                Id = Guid.NewGuid(),
                Quiz = quiz,
                User = user,
                StartedAt = DateTime.UtcNow 
            };

            _context.ActiveQuizzes.Add(activeQuiz);
            await _context.SaveChangesAsync();

            return Result.Success(activeQuiz);
        }
        
    }
  
}

public class CreateActiveQuizEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/active-quiz",async(CreateActiveQuizRequest request ,ISender sender) =>
        {
            var quizCreation = new CreateActiveQuiz.QuizCreation(request.QuizId, request.TakenBy);
            var result = await sender.Send(quizCreation);
                
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }
            
            return Result.Success(result.Value.Id);
        });
    }
}
