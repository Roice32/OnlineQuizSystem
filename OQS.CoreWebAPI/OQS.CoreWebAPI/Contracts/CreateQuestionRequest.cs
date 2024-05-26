using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Contracts
{
    public class CreateQuestionRequest
    {
        public Guid QuizId { get; set; }
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public int AlocatedPoints { get; set; }
        public int TimeLimit { get; set; }
        public List<string>? Choices { get; set; }
        public bool? TrueFalseAnswer { get; set; }
        public List<string>? MultipleChoiceAnswers { get; set; }
        public string? SingleChoiceAnswer { get; set; }
        public List<string>? WrittenAcceptedAnswers { get; set; }
    }
}
