using MediatR;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Contracts.LiveQuizzes;

public class ConnectionRequest : IRequest<Result<bool>>
{
    public Guid UserId { get; set; }
    public string Code { get; set; }
}