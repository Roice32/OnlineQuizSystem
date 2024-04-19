using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public abstract class QuizChecker
    {
        public static void CheckQuiz(QuizSubmission toBeChecked)
        {
            Quiz quizFromDb = FetchQuizFromDB(toBeChecked.QuizId);
            QuizResultBody resultBody = BuildQuizResultBody(toBeChecked, quizFromDb.Questions);
            QuizResultHeader resultHeader = BuildQuizResultHeader(toBeChecked, resultBody);
            StoreQuizResult(resultHeader, resultBody);
        }
        private static Quiz FetchQuizFromDB(Guid QuizId)
        {
            // fetch quiz from DB
            // PLACEHOLDER
            return null;
        }
        private static QuizResultBody BuildQuizResultBody(QuizSubmission toBeChecked, List<QuestionBase> questionsFromDb)
        {
            QuizResultBody resultBody = new QuizResultBody(toBeChecked.QuizId, toBeChecked.TakenBy);
            foreach (var QAPair in toBeChecked.QuestionAnswerPairs)
            {
                QuestionBase questionFromDb = questionsFromDb.Find(q => q.Id == QAPair.QuestionId);
                resultBody.AddQuestionResult(CheckQuestion(toBeChecked, QAPair, questionFromDb));
            }

            return resultBody;
        }

        private static QuestionResult CheckQuestion(QuizSubmission toBeChecked, QuestionAnswerPair QAPair, QuestionBase questionFromDb)
        {
            QuestionResult questionResult = new QuestionResult(toBeChecked.TakenBy, QAPair.QuestionId, QAPair.Answer);

            questionResult.AnswersTypes
                .AddRange(QuestionChecker
                    .CheckQuestion(QAPair.Answer, questionFromDb));
            
            switch (questionFromDb.Type)
            {
                case QuestionType.TrueFalse:
                case QuestionType.SingleChoice:
                case QuestionType.WrittenAnswer:
                    if (questionResult.AnswersTypes[0] == AnswerResult.Correct)
                        questionResult.Score = questionFromDb.AllocatedPoints;
                    else
                        questionResult.Score = 0;
                    break;
                case QuestionType.MultipleChoice:
                    int correctAnswersCount = questionResult.AnswersTypes
                        .Count(a => a == AnswerResult.Correct);
                    int wrongAnswersCount = questionResult.AnswersTypes
                        .Count(a => a == AnswerResult.Wrong);
                    int expectedCorrectAnswersCount = ((MultipleChoiceQuestion)questionFromDb)
                        .MultipleChoiceAnswers.Count;
                    questionResult.Score = Math.Max(0, correctAnswersCount - wrongAnswersCount)
                        / expectedCorrectAnswersCount
                        * questionFromDb.AllocatedPoints;
                    break;
                case QuestionType.ReviewNeeded:
                    questionResult.Score = 0;
                    break;
            }

            return questionResult;
        }

        private static QuizResultHeader BuildQuizResultHeader(QuizSubmission toBeChecked, QuizResultBody resultBody)
        {
            QuizResultHeader resultHeader = new QuizResultHeader(toBeChecked.QuizId,
                toBeChecked.TakenBy, toBeChecked.TimeElapsed);
            int totalScore = 0;
            foreach (var questionResult in resultBody.QuestionResults)
            {
                totalScore += questionResult.Score;
                if (questionResult.AnswersTypes.Contains(AnswerResult.Pending))
                    resultHeader.ReviewPending = true;
            }
            resultHeader.Score = totalScore;
            return null;
        }

        private static void StoreQuizResult(QuizResultHeader resultHeader, QuizResultBody resultBody)
        {
            // store quiz result
        }

        ~QuizChecker()
        {
            // clean up
        }
    }
}