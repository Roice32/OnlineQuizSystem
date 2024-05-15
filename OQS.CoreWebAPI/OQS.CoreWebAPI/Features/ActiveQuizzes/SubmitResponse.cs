using System.Net;
using Carter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Entities;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Features.Quizzes;
public class SubmitResponse : ICarterModule
{
    private List<SubmitResponseRequest> result = new List<SubmitResponseRequest>();
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/active-quiz/{activeQuizId}", async (SubmitResponseRequest request) =>
        {
            if (request == null)
                return Result.Failure(new Error("SubmitActiveQuiz.BadRequest", "Invalid request format."));
            var data = new SubmitResponseRequest { ActiveQuizId = request.ActiveQuizId, Answers = request.Answers };
            result.Add(data);
            return Result.Success("Submission successful.");
        });
    }
}