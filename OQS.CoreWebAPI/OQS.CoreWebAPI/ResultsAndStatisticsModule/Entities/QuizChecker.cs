using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public abstract class QuizChecker
    {
        public static void CheckQuiz(QuizSubmission toBeChecked)
        {
            /*Quiz quizFromDb = FetchQuizFromDB(toBeChecked.QuizId);
            QuizResultBody resultBody = BuildQuizResultBody(toBeChecked, quizFromDb.Questions);
            QuizResultHeader resultHeader = BuildQuizResultHeader(toBeChecked, resultBody);
            StoreQuizResult(resultHeader, resultBody);*/
        }

        private static Quiz FetchQuizFromDB(Guid QuizId)
        {
            // fetch quiz from DB
            // PLACEHOLDER
            return null;
        }

        private static QuizResultBody BuildQuizResultBody(QuizSubmission toBeChecked, List<QuestionBase> questionsFromDb)
        {
            /*QuizResultBody resultBody = new QuizResultBody(toBeChecked.QuizId, toBeChecked.TakenBy);
            foreach (var QAPair in toBeChecked.QuestionAnswerPairs)
            {
                QuestionBase questionFromDb = questionsFromDb.Find(q => q.Id == QAPair.QuestionId);
                resultBody.AddQuestionResult(CheckQuestion(QAPair, questionFromDb));
            }*/
            // PLACEHOLDER
            return null;
        }

        private static QuestionResult CheckQuestion(QuestionAnswerPair QAPair, QuestionBase questionFromDb)
        {
            /*QuestionResult questionResult = new QuestionResult(QAPair.QuestionId, QAPair.Answers)
            
            questionResult.AnswersType
                .AddRange(QuestionChecker::CheckQuestion
                    (QuestionResult.SubmittedAnswers,
                    questionFromDB));
            
            switch (questionFromDb.Type)
            {
                case QuestionType.TrueFalse:
                case QuestionType.SingleChoice:
                case QuestionType.WrittenAnswer:
                    if (questionResult.AnswersType[0] == AnswerResult.Correct)
                        questionResult.Score = QuestionFromDB.AllocatedPoints;
                    else
                        questionResult.Score = 0;
                    break;
                case QuestionType.MultipleChoice:
                    int correctAnswersCount = questionResult.AnswersType
                        .Count(a => a == AnswerResult.Correct);
                    int wrongAnswersCount = questionResult.AnswersType
                        .Count(a => a == AnswerResult.Wrong);
                    int expectedCorrectAnswersCount = QuestionFromDB.CorrectAnswersCount;
                    questionResult.Score = Math.Max(0, correctAnswersCount - wrongAnswersCount)
                        / expectedCorrectAnswersCount
                        * QuestionFromDB.AllocatedPoints;
                    break;
                case QuestionType.ReviewNeeded:
                    questionResult.Score = 0;
                    break;
            }*/

            return null;
        }

        private static QuizResultHeader BuildQuizResultHeader(QuizSubmission toBeChecked, QuizResultBody resultBody)
        {
            /*QuizResultHeader resultHeader = new QuizResultHeader();
            resultHeader.QuizID = toBeChecked.QuizID;
            resultHeader.UserId = toBeChecked.TakenBy;
            resultHeader.SubmittedAt = DateTime.Now;
            int totalScore = 0;
            foreach (var questionResult in resultBody.QuestionResults)
            {
                totalScore += questionResult.Score;
                if (questionResult.AnswersType.contains(AnswerType.Pending))
                    resultHeader.ReviewPending = true;
            }
            resultHeader.TotalScore = totalScore;*/
            return null;
        }

        private static void storeQuizResult(QuizResultHeader resultHeader, QuizResultBody resultBody)
        {
            // store quiz result
        }

        ~QuizChecker()
        {
            // clean up
        }
    }
}
