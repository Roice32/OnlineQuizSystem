namespace OQS.CoreWebAPI.Temp
{
    public class Quiz
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TimeLimitMinutes { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public Guid CreatorId { get; set; }
    }
}