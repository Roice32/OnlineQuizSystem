namespace OQS.CoreWebAPI.Shared;

public record Error(int Code, string Message)
{
    public static readonly Error None = new(StatusCodes.Status200OK, string.Empty);
    public static readonly Error NullValue = new(StatusCodes.Status500InternalServerError, "Value cannot be null.");
    public static readonly Error ConditionNotMet = new(StatusCodes.Status403Forbidden, "Condition not met.");
}