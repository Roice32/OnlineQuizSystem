using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairClasses;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.Checkers
{
    public abstract class QuestionChecker
    {
        public static QuestionResultBase CheckQuestion(Guid userId,
            QuestionAnswerPairBase qaPair,
            QuestionBase questionFromDb)
        {
            switch (questionFromDb.Type)
            {
                case QuestionType.TrueFalse:
                    //return CheckTrueFalseQuestion(userId, qaPair, questionFromDb);
                case QuestionType.MultipleChoice:
                    //return CheckMultipleChoiceQuestion(userId, qaPair, questionFromDb);
                case QuestionType.SingleChoice:
                    //return CheckSingleChoiceQuestion(userId, qaPair, questionFromDb);
                case QuestionType.WrittenAnswer:
                    //return CheckWrittenAnswerQuestion(userId, qaPair, questionFromDb);
                default:
                    //return CheckReviewNeededQuestion(userId, qaPair, questionFromDb);
                    break;
            }
            // PLACEHOLDER
            return null;
        }

        /*private static TrueFalseQuestionResult CheckTrueFalseQuestion(Guid userId,
            QuestionAnswerPairBase qaPair,
            QuestionBase questionFromDb)
        {
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
        }*/

        /*private static MultipleChoiceQuestionResult CheckMultipleChoiceQuestion(Guid userId,
            QuestionAnswerPairBase qaPair,
            QuestionBase questionFromDb)
        {
            Dictionary<string, AnswerResult> allChoicesResults = new();
            foreach (var choice in ((ChoiceQuestionBase)questionFromDb).Choices)
            {
                if(((MultipleChoiceQuestion)questionFromDb).MultipleChoiceAnswers.Contains(choice))
                {
                    if (((MultipleChoiceQAPair)qaPair).MultipleChoiceAnswers.Contains(choice))
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
                    if (((MultipleChoiceQAPair)qaPair).MultipleChoiceAnswers.Contains(choice))
                    {
                        allChoicesResults.Add(choice, AnswerResult.Wrong);
                    }
                    else
                    {
                        allChoicesResults.Add(choice, AnswerResult.Other);
                    }
                }
            }

        int correctCount = allChoicesResults.Count(r => r.Value == AnswerResult.Correct);
        int wrongCount = allChoicesResults.Count(r => r.Value == AnswerResult.Wrong);
        float scorePercentage = Math.Max(0, correctCount - wrongCount) /
            ((MultipleChoiceQuestion)questionFromDb).MultipleChoiceAnswers.Count;

        return new MultipleChoiceQuestionResult(userId,
            qaPair.QuestionId,
            questionFromDb.AllocatedPoints * scorePercentage,
            allChoicesResults);
        }*/

        /*private static SingleChoiceQuestionResult CheckSingleChoiceQuestion(Guid userId,
            QuestionAnswerPairBase qaPair,
            QuestionBase questionFromDb)
        {
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
            return new SingleChoiceQuestionResult(userId,
                qaPair.QuestionId,
                questionFromDb.AllocatedPoints,
                allChoicesResults);
        }*/

        /*private static WrittenAnswerQuestionResult CheckWrittenAnswerQuestion(Guid userId,
            QuestionAnswerPairBase qaPair,
            QuestionBase questionFromDb)
        {
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
        }*/

        /*private static ReviewNeededQuestionResult CheckReviewNeededQuestion(Guid userId,
            QuestionAnswerPairBase qaPair,
            QuestionBase questionFromDb)
        {
            // Later on, implement asking an LLM for a temporary review.
            return new ReviewNeededQuestionResult(userId,
                qaPair.QuestionId,
                0,
                AnswerResult.Pending);
        }*/
    }
}