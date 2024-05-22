namespace OQS.CoreWebAPI.Shared;

public record Error(string Code, string Message)
{
    public static readonly Error None = new("No error", string.Empty);
    public static readonly Error NullValue = new("Internal Server Error", "Value cannot be null.");
    public static readonly Error ConditionNotMet = new("Condition not met", "Condition not met.");
    public static readonly Error LiveQuizCreatorNotFound = new("LiveQuizzes.NotFound", "Live Quiz Creator Not Found.");
}