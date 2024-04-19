using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public abstract class QuestionChecker
    {
        public static List<AnswerResult> CheckQuestion(List<string> submittedAnswers,
            QuestionBase questionFromDb)
        {
            List<AnswerResult> answersType = new();
            switch (questionFromDb.Type)
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

        private static AnswerResult CheckTrueFalseQuestion(List<string> submittedAnswers,
                       QuestionBase questionFromDb)
        {
            if (((TrueFalseQuestion)questionFromDb).TrueFalseAnswer == Convert.ToBoolean(submittedAnswers[0]))
                return AnswerResult.Correct;
            return AnswerResult.Wrong;
        }

        private static List<AnswerResult> CheckMultipleChoiceQuestion(List<string> submittedAnswers,
                                  QuestionBase questionFromDb)
        {
            List<AnswerResult> answersType = new();
            foreach (var submittedAnswer in submittedAnswers)
            {
                if (((MultipleChoiceQuestion)questionFromDb).MultipleChoiceAnswers.Contains(submittedAnswer))
                    answersType.Add(AnswerResult.Correct);
                else
                    answersType.Add(AnswerResult.Wrong);
            }
            return answersType;
        }

        private static AnswerResult CheckSingleChoiceQuestion(List<string> submittedAnswers,
                                  QuestionBase questionFromDb)
        {
            if (((SingleChoiceQuestion)questionFromDb).SingleChoiceAnswer == Convert.ToInt32(submittedAnswers[0]))
                return AnswerResult.Correct;
            return AnswerResult.Wrong;
        }

        private static AnswerResult CheckWrittenAnswerQuestion(List<string> submittedAnswers,
                                             QuestionBase questionFromDb)
        {
            if (((WrittenAnswerQuestion)questionFromDb).WrittenAcceptedAnswers.Contains(Convert.ToString(submittedAnswers[0])))
                return AnswerResult.Correct;
            return AnswerResult.Wrong;
        }

        private static AnswerResult CheckReviewNeededQuestion(List<string> submittedAnswers,
                                                        QuestionBase questionFromDB)
        {
            // Later on, implement asking an LLM for a temporary review.
            return AnswerResult.Pending;
        }
    }
}