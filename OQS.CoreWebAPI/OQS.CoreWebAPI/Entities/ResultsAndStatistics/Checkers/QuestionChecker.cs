using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Chat;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionAnswerPairs;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Shared;
using Model = OpenAI_API.Models.Model;

namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics.Checkers
{
    public interface IQuestionCheckerStrategy
    {
        QuestionType GetQuestionType { get; }

        QuestionResultBase CheckQuestion(Guid userId, QuestionAnswerPairBase qaPair, QuestionBase questionFromDb);
    }

    public class TrueFalseQuestionChecker : IQuestionCheckerStrategy
    {
        public QuestionType GetQuestionType => QuestionType.TrueFalse;

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
        public QuestionType GetQuestionType => QuestionType.MultipleChoice;

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
        public QuestionType GetQuestionType => QuestionType.SingleChoice;

        public QuestionResultBase CheckQuestion(Guid userId, QuestionAnswerPairBase qaPair, QuestionBase questionFromDb)
        {
            if (qaPair is null)
            {
                return new ChoiceQuestionResult(userId, questionFromDb.Id, 0, JsonConvert.SerializeObject(new Dictionary<string, AnswerResult>()));
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
        public QuestionType GetQuestionType => QuestionType.WrittenAnswer;

        public QuestionResultBase CheckQuestion(Guid userId, QuestionAnswerPairBase qaPair, QuestionBase questionFromDb)
        {
            if (qaPair is null)
            {
                return new WrittenAnswerQuestionResult(userId, questionFromDb.Id, 0, "", AnswerResult.NotAnswered);
            }

            if (((WrittenAnswerQuestion)questionFromDb).WrittenAcceptedAnswers
                               .Contains(((WrittenQAPair)qaPair).WrittenAnswer))
            {
                return new WrittenAnswerQuestionResult(userId, qaPair.QuestionId, questionFromDb.AllocatedPoints, ((WrittenQAPair)qaPair).WrittenAnswer, AnswerResult.Correct);
            }

            return new WrittenAnswerQuestionResult(userId, qaPair.QuestionId, 0, ((WrittenQAPair)qaPair).WrittenAnswer, AnswerResult.Wrong);
        }
    }

    public class ReviewNeededQuestionChecker : IQuestionCheckerStrategy
    {
        public QuestionType GetQuestionType => QuestionType.ReviewNeeded;

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
        public static async Task<Result<AskLLMForReviewResponse>> AskLLMForReviewAsync(ReviewNeededQuestion question, string answer)
        {
            var openAI = new OpenAIAPI("APIKeyGoesHere");

            if (question == null)
            {
                Console.WriteLine("Error: The question sent was null");
                return Result.Failure<AskLLMForReviewResponse>(
                    new Error("AskLLMForReview.Error",
                        "The question sent was null"));
            }

            var chatRequest = new ChatRequest
            {
                Model = Model.ChatGPTTurbo,
                Messages = new[]
                {
                    new ChatMessage(ChatMessageRole.System, "You are a helpful assistant."),
                    new ChatMessage(ChatMessageRole.User, "Question: " + question.Text +
                        "\nAnswer: " + answer +
                        "\nMax Possible Score: " + question.AllocatedPoints +
                        "\nPlease review and grade the answer based on the provided criteria, and return the response in the following JSON format: " +
                        "{ \"review\" : \"textReview\", \"grade\" : floatValueBetween0AndMaxPossibleScore }")
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
            catch (Exception e)
            {
                Console.WriteLine($"HttpRequestException caught: {e.Message}");
                return Result.Failure<AskLLMForReviewResponse>(
                    new Error("AskLLMForReview.Error",
                        e.Message));
            }

            if (chatResponse is null)
            {
                Console.WriteLine("Error: OpenAI API did not respond.");
                return Result.Failure<AskLLMForReviewResponse>(
                    new Error("AskLLMForReview.Error",
                        "OpenAI API did not respond."));
            }

            try
            {
                AskLLMForReviewResponse askLLMForReviewResponse = JsonConvert
                    .DeserializeObject<AskLLMForReviewResponse>(chatResponse.Choices[0].Message.Content);
                return askLLMForReviewResponse;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught during deserialization: {e.Message}");
                return Result.Failure<AskLLMForReviewResponse>(
                        new Error("AskLLMForReview.Error",
                            "OpenAI API did not return a valid response."));
            }
        }
    }

    public class QuestionChecker
    {
        private static readonly Dictionary<QuestionType, IQuestionCheckerStrategy> _strategies = new();

        static QuestionChecker() { }

        public QuestionChecker(IEnumerable<IQuestionCheckerStrategy> strategies)
        {
            foreach (var strategy in strategies)
            {
                if (!_strategies.ContainsKey(strategy.GetQuestionType))
                {
                    _strategies[strategy.GetQuestionType] = strategy;
                }
            }
        }

        public static QuestionResultBase CheckQuestion(Guid userId, QuestionAnswerPairBase qaPair, QuestionBase questionFromDb)
        {
            if (_strategies.TryGetValue(questionFromDb.Type, out var strategy))
            {
                return strategy.CheckQuestion(userId, qaPair, questionFromDb);
            }

            throw new NotSupportedException($"Question type {questionFromDb.Type} is not supported.");
        }

        public static void AddStrategy(QuestionType questionType, IQuestionCheckerStrategy strategy)
        {
            if (!_strategies.TryAdd(questionType, strategy))
            {
                throw new ArgumentException($"A strategy for question type {questionType} has already been added.");
            }
        }
    }
}