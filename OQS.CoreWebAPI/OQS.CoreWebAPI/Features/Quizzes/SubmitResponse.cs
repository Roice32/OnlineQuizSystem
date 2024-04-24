using Carter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Entities;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Features.Quizzes;
public class SubmiResponse : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/active-quiz/{activeQuizId}", async (HttpContext httpContext) =>
        {
            var activeQuizId = (string)httpContext.GetRouteValue("activeQuizId");

            var request = await httpContext.Request.ReadFromJsonAsync<SubmitResponseRequest>();

            if (request == null)
                return Result.Failure(new Error(400, "Invalid request format."));
            var formattedData = FormatData(request);
            return Result.Success("Submission successful.");
        });
    }
    private List<object> FormatData(SubmitResponseRequest request)
    {
        var formattedData = new List<object>();

        foreach (var answer in request.Answers)
        {
            var formattedAnswer = new
            {
                questionId = answer.QuestionId,
                questionType = answer.Type,
                trueFalseAnswer = answer.Type == QuestionType.TrueFalse ? bool.Parse(answer.Value) : (bool?)null,
                multipleChoiceAnswers = answer.Type == QuestionType.MultipleChoice ? answer.Value.Split(',') : null,
                singleChoiceAnswer = answer.Type == QuestionType.SingleChoice ? answer.Value : null,
                writeAnswer = answer.Type == QuestionType.WriteAnswer ? answer.Value : null,
                reviewNeeded = answer.Type == QuestionType.ReviewNeeded ? answer.Value : null
            };

            formattedData.Add(formattedAnswer);
        }

        return formattedData;
    }

}