﻿
namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.ResultTypes
{
    public class TrueFalseQuestionResult : QuestionResultBase
    {
        public AnswerResult TrueFalseAnswerResult { get; set; }

        public TrueFalseQuestionResult(Guid userId, Guid questionId, float score, AnswerResult trueFalseAnswerResult):
            base(userId, questionId, score)
        {
            TrueFalseAnswerResult = trueFalseAnswerResult;
        }
    }
}
