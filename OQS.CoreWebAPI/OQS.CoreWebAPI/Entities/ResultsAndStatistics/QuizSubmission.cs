using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionAnswerPairs;

namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics
{
    public class QuizSubmission
    {
        public Guid QuizId { get; set; }
        public Guid TakenBy { get; set; }
        public List<QuestionAnswerPairBase> QuestionAnswerPairs { get; set; } = new();

        public QuizSubmission(Guid quizId, Guid takenBy, List<QuestionAnswerPairBase> questionAnswerPairs)
        {
            QuizId = quizId;
            TakenBy = takenBy;
            QuestionAnswerPairs.AddRange(questionAnswerPairs);
        }

        public QuizSubmission(Guid quizId, Guid userId, SubmitResponseRequest submittedResponse)
        {
            QuizId = quizId;
            TakenBy = userId;
            QuestionAnswerPairs = [];
            foreach (Answer answer in submittedResponse.Answers)
            {
                if (answer.Type == QuestionType.TrueFalse)
                {
                    QuestionAnswerPairs.Add(new TrueFalseQAPair(answer.QuestionId, (bool)answer.TrueFalseAnswer));
                    continue;
                }
                if (answer.Type == QuestionType.MultipleChoice)
                {
                    QuestionAnswerPairs.Add(new MultipleChoiceQAPair(answer.QuestionId, answer.MultipleChoiceAnswers.ToList()));
                    continue;
                }
                if (answer.Type == QuestionType.SingleChoice)
                {
                    QuestionAnswerPairs.Add(new SingleChoiceQAPair(answer.QuestionId, answer.SingleChoiceAnswer));
                    continue;
                }
                if (answer.Type == QuestionType.WrittenAnswer)
                {
                    QuestionAnswerPairs.Add(new WrittenQAPair(answer.QuestionId, answer.WriteAnswer));
                    continue;
                }
                if (answer.Type == QuestionType.ReviewNeeded)
                {
                    QuestionAnswerPairs.Add(new WrittenQAPair(answer.QuestionId, answer.WriteAnswer));
                    continue;
                }
                throw new Exception("Invalid Question Type");
            }
        }
    }
}
