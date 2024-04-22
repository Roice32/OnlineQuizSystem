using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Contracts
{
    public class CreateQuestionRequest
    {
        public Guid QuizId { get; set; }
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; }  

    }
}
