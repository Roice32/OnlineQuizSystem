using Carter;
using MediatR;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features;

public class CreateActiveQuiz
{
    public record QuizCreation(Guid QuizId, Guid TakenBy) : IRequest <Result<ActiveQuiz>>;
    
    internal sealed class Handler : IRequestHandler<QuizCreation, Result<ActiveQuiz>>
    {
        private readonly ApplicationDBContext _context;
        
        public Handler(ApplicationDBContext context)
        {
            _context = context;
     
        }
        public async Task<Result<ActiveQuiz>> Handle(QuizCreation request, CancellationToken cancellationToken)
        {
            // Verific daca quizId,userId exista in baza de date
            var quiz = await _context.Quizzes.FindAsync(request.QuizId);
            var user = await _context.Users.FindAsync(request.TakenBy);
            if (quiz == null)
            {
                return Result.Failure<ActiveQuiz>(new Error(400, "Invalid Quiz Id"));
            }
            if (user == null)
            {
                return Result.Failure<ActiveQuiz>(new Error(400, "Invalid User Id"));
            }
            
            var activeQuiz = new ActiveQuiz
            {
                Id = Guid.NewGuid(),
                Quiz = quiz,
                User = user,
                StartedAt = DateTime.UtcNow 
            };

            _context.ActiveQuizzes.Add(activeQuiz);
            await _context.SaveChangesAsync();

            return Result.Success<ActiveQuiz>(activeQuiz);
        }
        
    }
}

public class CreateActiveQuizEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/active-quiz",async(CreateActiveQuizRequest request ,ISender sender) =>
        {
            var quizCreation = new CreateActiveQuiz.QuizCreation(request.quizId, request.takenBy);
            var result = await sender.Send(quizCreation);
            return result;
        });
    }
}   