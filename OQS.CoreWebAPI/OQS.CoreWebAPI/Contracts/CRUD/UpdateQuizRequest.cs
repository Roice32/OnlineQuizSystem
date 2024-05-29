namespace OQS.CoreWebAPI.Contracts
{
    public class UpdateQuizRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string Language { get; set; }
        public int TimeLimitMinutes { get; set; }
    }
}
