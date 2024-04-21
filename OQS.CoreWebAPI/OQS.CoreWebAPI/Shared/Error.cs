namespace OQS.CoreWebAPI.Shared;

public record Error(int Code, string Message)
{
    public static readonly Error None = new(0, string.Empty);
    public static readonly Error NullValue = new(500, "Value cannot be null.");
    public static readonly Error ConditionNotMet = new(500, "Condition not met.");
}