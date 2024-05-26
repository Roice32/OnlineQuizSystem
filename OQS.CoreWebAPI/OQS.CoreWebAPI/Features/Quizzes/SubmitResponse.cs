﻿using Carter;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Quizzes;
public class SubmitResponse : ICarterModule
{
    private List<SubmitResponseRequest> result = new List<SubmitResponseRequest>();
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/active-quiz/{activeQuizId}", async (SubmitResponseRequest request) =>
        {
            if (request == null)
                return Result.Failure(new Error(400, "Invalid request format."));
            var data = new SubmitResponseRequest { ActiveQuizId = request.ActiveQuizId, Answers = request.Answers };
            result.Add(data);
            return Result.Success("Submission successful.");
        });
    }
}