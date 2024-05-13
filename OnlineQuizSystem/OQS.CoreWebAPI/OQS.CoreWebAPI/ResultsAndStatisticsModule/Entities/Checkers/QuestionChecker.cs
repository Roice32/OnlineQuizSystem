using Newtonsoft.Json;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.Checkers
{
    public abstract class QuestionChecker
    {
        public static QuestionResultBase CheckQuestion(Guid userId,
            QuestionAnswerPairBase qaPair,
            QuestionBase questionFromDb)
        {
            return questionFromDb.Type switch
            {
                QuestionType.TrueFalse => CheckTrueFalseQuestion(userId, qaPair, questionFromDb),
                QuestionType.MultipleChoice => CheckMultipleChoiceQuestion(userId, qaPair, questionFromDb),
                QuestionType.SingleChoice => CheckSingleChoiceQuestion(userId, qaPair, questionFromDb),
                QuestionType.WrittenAnswer => CheckWrittenAnswerQuestion(userId, qaPair, questionFromDb),
                _ => CheckReviewNeededQuestion(userId, qaPair, questionFromDb),
            };
        }

        private static TrueFalseQuestionResult CheckTrueFalseQuestion(Guid userId,
            QuestionAnswerPairBase qaPair,
            QuestionBase questionFromDb)
        {
            if (qaPair is null)
            {
                return new TrueFalseQuestionResult(userId,
                    questionFromDb.Id,
                    0,
                    AnswerResult.NotAnswered);
            }

            if (((TrueFalseQuestion)questionFromDb).TrueFalseAnswer ==
                ((TrueFalseQAPair)qaPair).TrueFalseAnswer)
                return new TrueFalseQuestionResult(userId,
                    qaPair.QuestionId,
                    questionFromDb.AllocatedPoints,
                    AnswerResult.Correct);
            return new TrueFalseQuestionResult(userId,
                qaPair.QuestionId,
                0,
                AnswerResult.Wrong);
        }

        private static ChoiceQuestionResult CheckMultipleChoiceQuestion(Guid userId,
            QuestionAnswerPairBase qaPair,
            QuestionBase questionFromDb)
        {
            if (qaPair is null)
            {
                return new ChoiceQuestionResult(userId,
                questionFromDb.Id,
                0,
                JsonConvert.SerializeObject(new Dictionary<string, AnswerResult>()));
            }

            Dictionary<string, AnswerResult> allChoicesResults = new();
            int correctCount = 0;
            int wrongCount = 0;
            int notPickedCount = 0;
            foreach (var choice in ((ChoiceQuestionBase)questionFromDb).Choices)
            {
                if (((MultipleChoiceQuestion)questionFromDb).MultipleChoiceAnswers.Contains(choice))
                {
                    if (((MultipleChoiceQAPair)qaPair).MultipleChoiceAnswers.Contains(choice))
                    {
                        allChoicesResults.Add(choice, AnswerResult.Correct);
                        correctCount++;
                    }
                    else
                    {
                        allChoicesResults.Add(choice, AnswerResult.CorrectNotPicked);
                        notPickedCount++;
                    }
                }
                else
                {
                    if (((MultipleChoiceQAPair)qaPair).MultipleChoiceAnswers.Contains(choice))
                    {
                        allChoicesResults.Add(choice, AnswerResult.Wrong);
                        wrongCount++;
                    }
                    else
                    {
                        allChoicesResults.Add(choice, AnswerResult.Other);
                    }
                }
            }

            float scorePercentage = 1f * Math.Max(0, correctCount - wrongCount) /
                (correctCount + notPickedCount);

            string pseudoDictionaryChoicesResults = JsonConvert.SerializeObject(allChoicesResults);
            return new ChoiceQuestionResult(userId,
                qaPair.QuestionId,
            questionFromDb.AllocatedPoints * scorePercentage,
            pseudoDictionaryChoicesResults);
        }

        private static ChoiceQuestionResult CheckSingleChoiceQuestion(Guid userId,
            QuestionAnswerPairBase qaPair,
            QuestionBase questionFromDb)
        {
            if (qaPair is null)
            {
                return new ChoiceQuestionResult(userId,
                    questionFromDb.Id,
                    0,
                    JsonConvert.SerializeObject(new Dictionary<string, AnswerResult>()));
            }

            Dictionary<string, AnswerResult> allChoicesResults = new();
            foreach (var choice in ((ChoiceQuestionBase)questionFromDb).Choices)
            {
                if (((SingleChoiceQuestion)questionFromDb).SingleChoiceAnswer == choice)
                {
                    if (((SingleChoiceQAPair)qaPair).SingleChoiceAnswer == choice)
                    {
                        allChoicesResults.Add(choice, AnswerResult.Correct);
                    }
                    else
                    {
                        allChoicesResults.Add(choice, AnswerResult.CorrectNotPicked);
                    }
                }
                else
                {
                    if (((SingleChoiceQAPair)qaPair).SingleChoiceAnswer == choice)
                    {
                        allChoicesResults.Add(choice, AnswerResult.Wrong);
                    }
                    else
                    {
                        allChoicesResults.Add(choice, AnswerResult.Other);
                    }
                }
            }

            string pseudoDictionaryChoicesResults = JsonConvert.SerializeObject(allChoicesResults);
            return new ChoiceQuestionResult(userId,
                qaPair.QuestionId,
                allChoicesResults.ContainsValue(AnswerResult.Correct) ?
                    questionFromDb.AllocatedPoints :
                    0,
                pseudoDictionaryChoicesResults);
        }

        private static WrittenAnswerQuestionResult CheckWrittenAnswerQuestion(Guid userId,
            QuestionAnswerPairBase qaPair,
            QuestionBase questionFromDb)
        {
            if (qaPair is null)
            {
                return new WrittenAnswerQuestionResult(userId,
                    questionFromDb.Id,
                    0,
                    "",
                    AnswerResult.NotAnswered);
            }

            if (((WrittenAnswerQuestion)questionFromDb).WrittenAcceptedAnswers
                .Contains(((WrittenQAPair)qaPair).WrittenAnswer))
                return new WrittenAnswerQuestionResult(userId,
                    qaPair.QuestionId,
                    questionFromDb.AllocatedPoints,
                    ((WrittenQAPair)qaPair).WrittenAnswer,
                    AnswerResult.Correct);
            return new WrittenAnswerQuestionResult(userId,
                qaPair.QuestionId,
                0,
                ((WrittenQAPair)qaPair).WrittenAnswer,
                AnswerResult.Wrong);
        }

        private static ReviewNeededQuestionResult CheckReviewNeededQuestion(Guid userId,
            QuestionAnswerPairBase qaPair,
            QuestionBase questionFromDb)
        {
            if (qaPair is null)
            {
                return new ReviewNeededQuestionResult(userId,
                    questionFromDb.Id,
                    0,
                    "",
                    AnswerResult.NotAnswered);
            }

            // Later on, implement asking an LLM for a temporary review.
            return new ReviewNeededQuestionResult(userId,
                qaPair.QuestionId,
                0,
                ((WrittenQAPair)qaPair).WrittenAnswer,
                AnswerResult.Pending);
        }
    }
}