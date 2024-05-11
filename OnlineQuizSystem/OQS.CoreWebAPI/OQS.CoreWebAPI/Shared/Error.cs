namespace OQS.CoreWebAPI.Shared
{
    public record Error(string Code, string Message)
    {
        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("Error.NullValue", "Value cannot be null.");
        public static readonly Error ConditionNotMet = new("Error.ConditionNotMet", "Condition not met.");
        public static readonly Error InvalidType = new("Error.InvalidType", "Invalid type.");
        public static readonly Error OutOfBoundsValue = new("Error.OutOfBoundsValue", "Value is out of bounds.");
        public static readonly Error DuplicateEntity = new("Error.DuplicateEntity", "Entity already exists.");
    }
}
