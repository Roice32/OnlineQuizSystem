
﻿using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Profile
{
    public static class GetUserDetails
    {
        public class Query : IRequest<Result<UserDetailsModel>>
        {
            public Guid Id { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, Result<UserDetailsModel>>
        {
            private readonly UserManager<User> userManager;

            public Handler(UserManager<User> userManager)
            {
                this.userManager = userManager;
            }

            public async Task<Result<UserDetailsModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await userManager.FindByIdAsync(request.Id.ToString());
                if (user == null)
                {
                    return Result.Failure<UserDetailsModel>(
                                               new Error("Profile.Handler", "User doesn't exists."));
                }

                var userDetails = new UserDetailsModel
                {
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                };

                return userDetails;
            }
        }
       
    }
}
public class GetUserDetailsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/profile/{id}", async (Guid id, ISender sender) =>
        {
            var query = new OQS.CoreWebAPI.Features.Profile.GetUserDetails.Query
            {
                Id = id
            };
            var result = await sender.Send(query);
            if(result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }
            return Results.Ok(result.Value);
        }
        );
    }
}
