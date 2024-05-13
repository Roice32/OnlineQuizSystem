using OQS.CoreWebAPI.Entities;
using System;
using System.Collections.Generic;

namespace OQS.CoreWebAPI.Contracts
{
    public class QuestionResponse
    {
        public Guid Id { get; set; }
        public QuestionType Type { get; set; }
        public string Text { get; set; }

        public Guid QuizId { get; set; }
        public List<string>? Choices { get; set; }
        public bool? TrueFalseAnswer { get; set; }
        public List<string>? MultipleChoiceAnswers { get; set; }
        public string? SingleChoiceAnswer { get; set; }
        public List<string>? WrittenAcceptedAnswers { get; set; }

        public QuestionResponse(QuestionBase question)
        {
            Id = question.Id;
            Type = question.Type;
            Text = question.Text;
            QuizId = question.QuizId;
            if (question is ChoiceQuestionBase choiceQuestion)
            {
                Choices = choiceQuestion.Choices;
            }

            if (question is TrueFalseQuestion trueFalseQuestion)
            {
                TrueFalseAnswer = trueFalseQuestion.TrueFalseAnswer;
            }

            if (question is MultipleChoiceQuestion multipleChoiceQuestion)
            {
                MultipleChoiceAnswers = multipleChoiceQuestion.MultipleChoiceAnswers;
            }

            if (question is SingleChoiceQuestion singleChoiceQuestion)
            {
                SingleChoiceAnswer = singleChoiceQuestion.SingleChoiceAnswer;
            }

            if (question is WrittenAnswerQuestion writtenAnswerQuestion)
            {
                WrittenAcceptedAnswers = writtenAnswerQuestion.WrittenAcceptedAnswers;
            }
        }
    }
}