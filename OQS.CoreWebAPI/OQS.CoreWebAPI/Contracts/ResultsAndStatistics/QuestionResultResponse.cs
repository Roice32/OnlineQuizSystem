using OQS.CoreWebAPI.Entities.ResultsAndStatistics;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Temp;

namespace OQS.CoreWebAPI.Contracts.ResultsAndStatistics
{
    public class QuestionResultResponse
    {
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        public float Score { get; set; }
        public QuestionType Type { get; set; }
        public AnswerResult? TrueFalseAnswerResult { get; set; }
        public string? PseudoDictionaryChoicesResults { get; set; } = string.Empty;
        public string? WrittenAnswer { get; set; } = string.Empty;
        public AnswerResult? WrittenAnswerResult { get; set; }
        public string? ReviewNeededAnswer { get; set; } = string.Empty;
        public string? LLMReview { get; set; } = string.Empty;
        public AnswerResult? ReviewNeededResult { get; set; }

        public QuestionResultResponse() { }

        public QuestionResultResponse(QuestionResultBase questionResult)
        {
            UserId = questionResult.UserId;
            QuestionId = questionResult.QuestionId;
            Score = questionResult.Score;
            if (questionResult is TrueFalseQuestionResult trueFalseQuestionResult)
            {
                Type = QuestionType.TrueFalse;
                TrueFalseAnswerResult = trueFalseQuestionResult.TrueFalseAnswerResult;
            }
            if (questionResult is ChoiceQuestionResult multipleChoiceQuestionResult)
            {
                Type = QuestionType.MultipleChoice;
                PseudoDictionaryChoicesResults = multipleChoiceQuestionResult.PseudoDictionaryChoicesResults;
            }
            if (questionResult is WrittenAnswerQuestionResult writtenAnswerQuestionResult)
            {
                Type = QuestionType.WrittenAnswer;
                WrittenAnswer = writtenAnswerQuestionResult.WrittenAnswer;
                WrittenAnswerResult = writtenAnswerQuestionResult.WrittenAnswerResult;
            }
            if (questionResult is ReviewNeededQuestionResult reviewNeededQuestionResult)
            {
                Type = QuestionType.ReviewNeeded;
                ReviewNeededAnswer = reviewNeededQuestionResult.ReviewNeededAnswer;
                LLMReview = reviewNeededQuestionResult.LLMReview;
                ReviewNeededResult = reviewNeededQuestionResult.ReviewNeededResult;
            }
        }
    }
}
