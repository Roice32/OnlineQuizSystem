using System.Net;

namespace OQS.CoreWebAPI.Shared;

public record Error(HttpStatusCode Code, string Message)
{
    public static readonly Error None = new(HttpStatusCode.OK, string.Empty);
    public static readonly Error NullValue = new(HttpStatusCode.InternalServerError, "Value cannot be null.");
    public static readonly Error ConditionNotMet = new(HttpStatusCode.BadRequest, "Condition not met.");
}