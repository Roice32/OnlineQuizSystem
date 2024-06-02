using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Contracts.CRUD
{
    public class QuestionResponse
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public QuestionType Type { get; set; }
        public string Text { get; set; }

        public int AllocatedPoints { get; set; }

        public int TimeLimit { get; set; }
        public List<string>? Choices { get; set; }
        public bool? TrueFalseAnswer { get; set; }
        public List<string>? MultipleChoiceAnswers { get; set; }
        public string? SingleChoiceAnswer { get; set; }
        public List<string>? WrittenAcceptedAnswers { get; set; }

        public QuestionResponse()
        {
        }

        public QuestionResponse(QuestionBase question)
        {
            this.Id = question.Id;
            this.QuizId = question.QuizId;
            this.Type = question.Type;
            this.Text = question.Text;
            this.TimeLimit = question.TimeLimit;
            this.AllocatedPoints = question.AllocatedPoints;
            if (question is ChoiceQuestionBase choiceQuestion)
            {
                this.Choices = choiceQuestion.Choices;
            }

            if (question is TrueFalseQuestion trueFalseQuestion)
            {
                this.TrueFalseAnswer = trueFalseQuestion.TrueFalseAnswer;
            }

            if (question is MultipleChoiceQuestion multipleChoiceQuestion)
            {
                this.MultipleChoiceAnswers = multipleChoiceQuestion.MultipleChoiceAnswers;
            }

            if (question is SingleChoiceQuestion singleChoiceQuestion)
            {
                this.SingleChoiceAnswer = singleChoiceQuestion.SingleChoiceAnswer;
            }

            if (question is WrittenAnswerQuestion writtenAnswerQuestion)
            {
                this.WrittenAcceptedAnswers = writtenAnswerQuestion.WrittenAcceptedAnswers;
            }
        }
    }
}