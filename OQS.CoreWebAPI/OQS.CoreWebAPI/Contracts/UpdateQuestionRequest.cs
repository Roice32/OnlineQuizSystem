using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Contracts
{
    public class UpdateQuestionRequest
    {
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public List<string>? choices { get; set; }
    }
}
