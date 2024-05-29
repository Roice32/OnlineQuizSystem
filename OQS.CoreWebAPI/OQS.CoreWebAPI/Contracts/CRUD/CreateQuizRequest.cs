namespace OQS.CoreWebAPI.Contracts
{
    public class CreateQuizRequest
    {

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string Language { get; set; }
        public Guid CreatorId { get; set; }
        public int TimeLimitMinutes { get; set; }

    }
}
