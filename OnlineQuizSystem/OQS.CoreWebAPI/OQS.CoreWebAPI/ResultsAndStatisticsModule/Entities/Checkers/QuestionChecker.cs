using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Completions;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.Shared;

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

            ReviewNeededQuestionResult questionResult =
                new ReviewNeededQuestionResult(userId,
                    qaPair.QuestionId,
                    0,
                    ((WrittenQAPair)qaPair).WrittenAnswer,
                    AnswerResult.Pending);

            Result<AskLLMForReviewResponse> askLLMForReviewResponse =
                AskLLMForReviewAsync((ReviewNeededQuestion)questionFromDb,
                    ((WrittenQAPair)qaPair).WrittenAnswer).GetAwaiter().GetResult();
        
            if (askLLMForReviewResponse.IsSuccess)
            {
                questionResult.LLMReview = askLLMForReviewResponse.Value.Review;
                questionResult.Score = askLLMForReviewResponse.Value.Grade;
            }

            return questionResult;
        }

        private static async Task<Result<AskLLMForReviewResponse>> AskLLMForReviewAsync(ReviewNeededQuestion question, string answer)
        {
            var openAI = new OpenAIAPI("sk-proj-QSvrIDvvcYVwtOOcjMi8T3BlbkFJPqlYoGxnr47RgxH4DEKH");
            CompletionRequest completionRequest = new()
            {
                Model = "gpt-3.5-turbo",
                MaxTokens = 100,
                Temperature = 0.5f,
                TopP = 1,
                Prompt = "Question: " + question.Text +
                    "\nAnswer: " + answer +
                    "\nMax Possible Score: " + question.AllocatedPoints +
                    "\nReturn review & grade as JSON."
            };

            CompletionResult completionResponse = null;
            try
            {
                completionResponse = await openAI
                    .Completions
                    .CreateCompletionAsync(completionRequest);
            }
            catch (HttpRequestException e)
            {
                return Result.Failure<AskLLMForReviewResponse>(
                    new Error("AskLLMForReview.Error",
                        e.Message));
            }

            if (completionResponse is null)
            {
                return Result.Failure<AskLLMForReviewResponse>(
                    new Error("AskLLMForReview.Error",
                        "OpenAI API did not respond."));
            }

            AskLLMForReviewResponse askLLMForReviewResponse = JsonConvert
                .DeserializeObject<AskLLMForReviewResponse>(completionResponse.Completions[0].Text);

            if (askLLMForReviewResponse is null)
            {
                return Result.Failure<AskLLMForReviewResponse>(
                    new Error("AskLLMForReview.Error",
                        "OpenAI API did not return a valid response."));
            }

            return askLLMForReviewResponse;
        }
    }
}