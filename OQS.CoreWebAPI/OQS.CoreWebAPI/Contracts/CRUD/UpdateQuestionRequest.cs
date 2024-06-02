using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Contracts
{
    public class UpdateQuestionRequest
    {
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public int AllocatedPoints { get; set; }
        public int TimeLimit { get; set; }
        public List<string>? Choices { get; set; }
        public bool? TrueFalseAnswer { get; set; }
        public List<string>? MultipleChoiceAnswers { get; set; }
        public string? SingleChoiceAnswer { get; set; }
        public List<string>? WrittenAcceptedAnswers { get; set; }
    }
}
