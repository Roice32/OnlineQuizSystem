using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using OpenAI_API.Completions;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.Shared;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Model = OpenAI_API.Models.Model;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.Checkers
{
    public interface IQuestionCheckerStrategy
    {
        QuestionResultBase CheckQuestion(Guid userId, QuestionAnswerPairBase qaPair, QuestionBase questionFromDb);
    }

    public abstract class QuestionChecker
    {
        private static readonly Dictionary<QuestionType, IQuestionCheckerStrategy> strategies = new()
    {
        { QuestionType.TrueFalse, new TrueFalseQuestionChecker() },
        { QuestionType.MultipleChoice, new MultipleChoiceQuestionChecker() },
        { QuestionType.SingleChoice, new SingleChoiceQuestionChecker() },
        { QuestionType.WrittenAnswer, new WrittenAnswerQuestionChecker() },
        { QuestionType.ReviewNeeded, new ReviewNeededQuestionChecker() }
    };

        public class TrueFalseQuestionChecker : IQuestionCheckerStrategy
        {
            public QuestionResultBase CheckQuestion(Guid userId, QuestionAnswerPairBase qaPair, QuestionBase questionFromDb)
            {
                if (qaPair is null)
                {
                    return new TrueFalseQuestionResult(userId, questionFromDb.Id, 0, AnswerResult.NotAnswered);
                }

                if (((TrueFalseQuestion)questionFromDb).TrueFalseAnswer == ((TrueFalseQAPair)qaPair).TrueFalseAnswer)
                {
                    return new TrueFalseQuestionResult(userId, qaPair.QuestionId, questionFromDb.AllocatedPoints, AnswerResult.Correct);
                }

                return new TrueFalseQuestionResult(userId, qaPair.QuestionId, 0, AnswerResult.Wrong);
            }
        }

        public class MultipleChoiceQuestionChecker : IQuestionCheckerStrategy
        {

            public QuestionResultBase CheckQuestion(Guid userId, QuestionAnswerPairBase qaPair, QuestionBase questionFromDb)
            {
                if (qaPair is null)
                {
                    return new ChoiceQuestionResult(userId, questionFromDb.Id, 0, JsonConvert.SerializeObject(new Dictionary<string, AnswerResult>()));
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

                float scorePercentage = 1f * Math.Max(0, correctCount - wrongCount) / (correctCount + notPickedCount);

                string pseudoDictionaryChoicesResults = JsonConvert.SerializeObject(allChoicesResults);
                return new ChoiceQuestionResult(userId, qaPair.QuestionId, questionFromDb.AllocatedPoints * scorePercentage, pseudoDictionaryChoicesResults);
            }
        }
        public class SingleChoiceQuestionChecker : IQuestionCheckerStrategy
        {
            public QuestionResultBase CheckQuestion(Guid userId, QuestionAnswerPairBase qaPair, QuestionBase questionFromDb)
            {
                if (qaPair is null)
                {
                    return new ChoiceQuestionResult(userId, questionFromDb.Id, 0,JsonConvert.SerializeObject(new Dictionary<string, AnswerResult>()));
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
                    allChoicesResults.ContainsValue(AnswerResult.Correct) ? questionFromDb.AllocatedPoints : 0,
                    pseudoDictionaryChoicesResults);
            }
        }

        public class WrittenAnswerQuestionChecker : IQuestionCheckerStrategy
        {
            public QuestionResultBase CheckQuestion(Guid userId, QuestionAnswerPairBase qaPair, QuestionBase questionFromDb)
            {
                if (qaPair is null)
                {
                    return new WrittenAnswerQuestionResult(userId,questionFromDb.Id, 0, "",AnswerResult.NotAnswered);
                }

                if (((WrittenAnswerQuestion)questionFromDb).WrittenAcceptedAnswers
                                   .Contains(((WrittenQAPair)qaPair).WrittenAnswer))
                {
                    return new WrittenAnswerQuestionResult(userId, qaPair.QuestionId,questionFromDb.AllocatedPoints, ((WrittenQAPair)qaPair).WrittenAnswer,AnswerResult.Correct);
                }

                return new WrittenAnswerQuestionResult(userId, qaPair.QuestionId,0,((WrittenQAPair)qaPair).WrittenAnswer, AnswerResult.Wrong);
            }
        }

        public class ReviewNeededQuestionChecker : IQuestionCheckerStrategy
        {
            public QuestionResultBase CheckQuestion(Guid userId, QuestionAnswerPairBase qaPair, QuestionBase questionFromDb)
            {
                if (qaPair is null)
                {
                    return new ReviewNeededQuestionResult(userId,
                        questionFromDb.Id, 0, "", AnswerResult.NotAnswered);
                }

                ReviewNeededQuestionResult questionResult =
                    new ReviewNeededQuestionResult(userId, qaPair.QuestionId, 0, ((WrittenQAPair)qaPair).WrittenAnswer, AnswerResult.Pending);

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
        }
      
        public static QuestionResultBase CheckQuestion(Guid userId, QuestionAnswerPairBase qaPair, QuestionBase questionFromDb)
        {
            return strategies[questionFromDb.Type].CheckQuestion(userId, qaPair, questionFromDb);
        }



        public static async Task<Result<AskLLMForReviewResponse>> AskLLMForReviewAsync(ReviewNeededQuestion question, string answer)
        {
            var openAI = new OpenAIAPI("apikey");
           
            if(question == null)
            {
                return Result.Failure<AskLLMForReviewResponse>(
                    new Error("AskLLMForReview.Error",
                        "The question sent was null"));
            }
            
            var chatRequest = new ChatRequest
            {
                Model = Model.ChatGPTTurbo, // Use the appropriate model identifier for gpt-3.5-turbo
                Messages = new[]
                {
                    new ChatMessage(ChatMessageRole.System, "You are a helpful assistant."),
                    new ChatMessage(ChatMessageRole.User, "Question: " + question.Text +
                        "\nAnswer: " + answer +
                        "\nMax Possible Score: " + question.AllocatedPoints +
                        "\nReturn review & grade as JSON.")
                },
                MaxTokens = 100,
                Temperature = 0.5f,
                TopP = 1
            };


            ChatResult chatResponse = null;
            try
            {
                chatResponse = await openAI
                    .Chat
                    .CreateChatCompletionAsync(chatRequest);
            }
            catch (HttpRequestException e)
            {
                return Result.Failure<AskLLMForReviewResponse>(
                    new Error("AskLLMForReview.Error",
                        e.Message));
            }

            if (chatResponse is null)
            {
                return Result.Failure<AskLLMForReviewResponse>(
                    new Error("AskLLMForReview.Error",
                        "OpenAI API did not respond."));
            }

            AskLLMForReviewResponse askLLMForReviewResponse = JsonConvert
                .DeserializeObject<AskLLMForReviewResponse>(chatResponse.Choices[0].Message.Content);

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