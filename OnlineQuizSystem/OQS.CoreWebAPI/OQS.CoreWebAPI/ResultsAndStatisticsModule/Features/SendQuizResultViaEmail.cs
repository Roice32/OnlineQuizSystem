using MediatR;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Carter;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.Database;
using FluentValidation;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Features;
using System.Text;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using Newtonsoft.Json;
using System.Runtime.Intrinsics.X86;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public interface IQuizResultEmailSenderStrategy
    {
        bool CanHandle(QuestionType questionType);
        string GetCorrectAnswer(QuestionBase question);
        string GetUserAnswer( QuestionResultBase question);
    }
    

    
    public class SendQuizResultViaEmail
    {
        public class TrueFalseQuizResultEmailSender : IQuizResultEmailSenderStrategy
        {
            public bool CanHandle(QuestionType questionType) => questionType == QuestionType.TrueFalse;

            public string GetCorrectAnswer(QuestionBase question)
            {
                var trueFalseQuestion = question as TrueFalseQuestion;
                if (trueFalseQuestion != null)
                {
                    return trueFalseQuestion.TrueFalseAnswer ? "False" : "True";
                }
                return string.Empty;
            }

            public string GetUserAnswer(QuestionResultBase questionResult)
            {
                var trueFalseQuestionResult = questionResult as TrueFalseQuestionResult;
                if (trueFalseQuestionResult != null)
                {
                    return trueFalseQuestionResult.TrueFalseAnswerResult.ToString();
                }
                return string.Empty;
            }
        }

        public class MultipleChoiceQuizResultEmailSender : IQuizResultEmailSenderStrategy
        {
            public bool CanHandle(QuestionType questionType) => questionType == QuestionType.MultipleChoice;

            public string GetCorrectAnswer(QuestionBase question)
            {
                var multipleChoiceQuestion = question as MultipleChoiceQuestion;
                if (multipleChoiceQuestion != null)
                {
                    return string.Join(", ", multipleChoiceQuestion.MultipleChoiceAnswers);
                }
                return string.Empty;   
            }

            public string GetUserAnswer(QuestionResultBase questionResult)
            {
                var multipleChoiceResult = questionResult as ChoiceQuestionResult;
                if (multipleChoiceResult != null)
                {
                    return string.Join(", ", JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(multipleChoiceResult.PseudoDictionaryChoicesResults).Keys);
                }
                return string.Empty;
            }
        }

        public class SingleChoiceQuizResultEmailSender : IQuizResultEmailSenderStrategy
        {
            public bool CanHandle(QuestionType questionType) => questionType == QuestionType.SingleChoice;

            public string GetCorrectAnswer(QuestionBase question)
            {
                var singleChoiceQuestion = question as SingleChoiceQuestion;
                if (singleChoiceQuestion != null)
                {
                    return singleChoiceQuestion.SingleChoiceAnswer;
                }
                return string.Empty;
            }

            public string GetUserAnswer(QuestionResultBase questionResult)
            {
                var singleChoiceResult = questionResult as ChoiceQuestionResult;
                if (singleChoiceResult != null)
                {
                    return string.Join(", ", JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(singleChoiceResult.PseudoDictionaryChoicesResults).Keys);
                }
                return string.Empty;
            }
        }

        public class WrittenAnswerQuizResultEmailSender : IQuizResultEmailSenderStrategy
        {
            public bool CanHandle(QuestionType questionType) => questionType == QuestionType.WrittenAnswer;

            public string GetCorrectAnswer(QuestionBase question)
            {
                var writtenAnswerQuestion = question as WrittenAnswerQuestion;
                if (writtenAnswerQuestion != null)
                {
                    return string.Join(", ", writtenAnswerQuestion.WrittenAcceptedAnswers);
                }
                return string.Empty;
            }

            public string GetUserAnswer(QuestionResultBase questionResult)
            {
                var writtenAnswerQuestionResult = questionResult as WrittenAnswerQuestionResult;
                if (writtenAnswerQuestionResult != null)
                {
                    return writtenAnswerQuestionResult.WrittenAnswer;
                }
                return string.Empty;
            }
        }


        public class Command : IRequest<Result>
        {
            public string RecipientEmail { get; set; }
            public Guid QuizId { get; set; }
            public Guid UserId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.RecipientEmail)
                    .NotEmpty()
                    .WithMessage("RecipientEmail is required.");

                RuleFor(x => x.QuizId)
                    .NotEmpty()
                    .WithMessage("QuizId is required.");

                RuleFor(x => x.UserId)
                    .NotEmpty()
                    .WithMessage("UserId is required.");
            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IValidator<Command> validator;
            private readonly IMediator mediator;
            private readonly Dictionary<QuestionType, IQuizResultEmailSenderStrategy> emailSenders = new()
            {
                { QuestionType.TrueFalse, new TrueFalseQuizResultEmailSender() },
                { QuestionType.MultipleChoice, new MultipleChoiceQuizResultEmailSender() },
                { QuestionType.SingleChoice, new SingleChoiceQuizResultEmailSender() },
                { QuestionType.WrittenAnswer, new WrittenAnswerQuizResultEmailSender()}
            };
            public Handler(IValidator<Command> validator, IMediator mediator)
            {
                this.validator = validator;
                this.mediator = mediator;
            }

            public bool CanHandle(QuestionType questionType)
            {
                return emailSenders[questionType].CanHandle(questionType);
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure(new Error("EmailSender.Validator", validationResult.ToString()));
                }

                var quizResult = await mediator.Send(new GetQuizResult.Query { QuizId = request.QuizId, UserId = request.UserId }, cancellationToken);
                if (!quizResult.IsSuccess)
                {
                    return Result.Failure(new Error("QuizResultNotFound", "Quiz result not found for the given quiz and user."));
                }

                var emailBody = new StringBuilder($"Quiz: {quizResult.Value.QuizResultHeader.QuizName}\nScore: {quizResult.Value.QuizResultHeader.Score}\n");
                foreach (var question in quizResult.Value.QuizResultBody.Questions)
                {
                    var correspondingAnswerPair = quizResult.Value.QuizResultBody.Questions.FirstOrDefault(qap => qap.Id == question.Id);
                    var correspondingUserAnswer = quizResult.Value.QuizResultBody.QuestionResults.FirstOrDefault(qr => qr.QuestionId == question.Id);

                    var formatter = emailSenders.FirstOrDefault(f => f.Value.CanHandle(question.Type));

                    if (formatter.Value != null)
                    {
                        if (correspondingAnswerPair != null && correspondingUserAnswer != null)
                        {

                            var correctAnswer = formatter.Value.GetCorrectAnswer(correspondingAnswerPair);
                            var userAnswer = formatter.Value.GetUserAnswer(correspondingUserAnswer);
                      

                            emailBody.AppendLine($"Question: {question.Text}\nYour Answer: {userAnswer}\nCorrect Answer: {correctAnswer}");

                            if (question.AllocatedPoints == correspondingUserAnswer.Score)
                            {
                                emailBody.AppendLine($"Your answer is correct!");
                                emailBody.AppendLine($"{correspondingUserAnswer.Score} points out of {question.AllocatedPoints} \n");
                            }
                            else if (question.AllocatedPoints > correspondingUserAnswer.Score && correspondingUserAnswer.Score != 0)
                            {
                                emailBody.AppendLine($"Your answer is partially correct!");
                                emailBody.AppendLine($"{correspondingUserAnswer.Score} points out of {question.AllocatedPoints} \n");
                            }
                            else
                            {
                                emailBody.AppendLine($"Your answer is incorrect!");
                                emailBody.AppendLine($"{correspondingUserAnswer.Score} points out of {question.AllocatedPoints}\n ");
                            }
                        }
                    }
                }

                try
                {
                    var message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse("echipafacultate@yahoo.com"));
                    message.To.Add(MailboxAddress.Parse(request.RecipientEmail));
                    message.Subject = "Quiz Score";
                    message.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                    {
                        Text = emailBody.ToString() + "\nThank you for participating in the quiz!"
                    };

                    using var smtp = new SmtpClient();
                    smtp.Connect("smtp.mail.yahoo.com", 587, false);
                    smtp.Authenticate("echipafacultate@yahoo.com", "onhqcvgwqodblrtv");
                    smtp.Send(message);
                    smtp.Disconnect(true);

                    return Result.Success();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred while sending email: {ex}");
                    return Result.Failure(new Error("EmailSenderError", ex.Message));
                }
            }
        }
    }
    public class SendQuizResultViaEmailEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/email/sendQuizResultViaEmail", async (string recipientEmail, Guid quizId, Guid userId, ISender sender) =>
            {
                var command = new SendQuizResultViaEmail.Command
                {
                    RecipientEmail = recipientEmail,
                    QuizId = quizId,
                    UserId = userId,
                };
                var result = await sender.Send(command);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result);
            });
        }
    }
}
