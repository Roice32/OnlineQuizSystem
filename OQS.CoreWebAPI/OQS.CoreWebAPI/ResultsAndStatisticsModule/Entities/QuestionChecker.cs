using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using System.Collections;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public abstract class QuestionChecker
    {
        public static List<AnswerResult> CheckQuestion(List<Object> submittedAnswers,
            QuestionBase questionFromDb)
        {
            List<AnswerResult> answersType = new();
            switch(questionFromDb.Type)
            {
                case QuestionType.TrueFalse:
                    answersType.Add(CheckTrueFalseQuestion(submittedAnswers, questionFromDb));
                    break;
                case QuestionType.MultipleChoice:
                    answersType.AddRange(CheckMultipleChoiceQuestion(submittedAnswers, questionFromDb));
                    break;
                case QuestionType.SingleChoice:
                    answersType.Add(CheckSingleChoiceQuestion(submittedAnswers, questionFromDb));
                    break;
                case QuestionType.WrittenAnswer:
                    answersType.Add(CheckWrittenAnswerQuestion(submittedAnswers, questionFromDb));
                    break;
                case QuestionType.ReviewNeeded:
                    answersType.Add(CheckReviewNeededQuestion(submittedAnswers, questionFromDb));
                    break;
            }
            return answersType;
        }

        private static AnswerResult CheckTrueFalseQuestion(List<Object> submittedAnswers,
                       QuestionBase questionFromDB)
        {
            /*if (questionFromDb.TrueFalseAnswer == Convert.ToBoolean(submittedAnswers[0]))
                return AnswerResult.Correct;*/
            return AnswerResult.Wrong;
        }

        private static List<AnswerResult> CheckMultipleChoiceQuestion(List<Object> submittedAnswers,
                                  QuestionBase questionFromDB)
        {
            List<AnswerResult> answersType = new();
            /*foreach (var submittedAnswer in submittedAnswers)
            {
                if (questionFromDb.MultipleChoiceAnswers.Contains(submittedAnswer))
                    answersType.Add(AnswerResult.Correct);
                else
                    answersType.Add(AnswerResult.Wrong);
            }*/
            return answersType;
        }

        private static AnswerResult CheckSingleChoiceQuestion(List<Object> submittedAnswers,
                                  QuestionBase questionFromDB)
        {
            /*if (questionFromDb.SingleChoiceAnswer == Convert.ToInt32(submittedAnswers[0])
                return AnswerResult.Correct;*/
            return AnswerResult.Wrong;
        }

        private static AnswerResult CheckWrittenAnswerQuestion(List<Object> submittedAnswers,
                                             QuestionBase questionFromDB)
        {
            /*if (questionFromDb.WrittenAcceptedAnswers.Contains(Convert.ToString(submittedAnswers[0]))
                return AnswerResult.Correct;*/
            return AnswerResult.Wrong;
        }

        private static AnswerResult CheckReviewNeededQuestion(List<Object> submittedAnswers,
                                                        QuestionBase questionFromDB)
        {
            // Later on, implement asking an LLM for a temporary review.
            return AnswerResult.Pending;
        }
    }
}
