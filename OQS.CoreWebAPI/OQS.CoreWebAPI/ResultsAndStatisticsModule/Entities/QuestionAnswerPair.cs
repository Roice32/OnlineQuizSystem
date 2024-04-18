using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuestionAnswerPair
    {
        private Guid questionId{ get; set; }

        private List<System.Object> answer { get; set; }

        private QuizSubmission quizSubmission { get; set; }

        public QuestionAnswerPair (Guid questionId, List<System.Object> answer, QuizSubmission quizSubmission)
        {
            this.questionId = questionId;
            this.answer = answer;
            this.quizSubmission = quizSubmission;
        }      
        
        public QuestionAnswerPair (QuestionAnswerPair questionAnswerPair)
        {
            this.questionId = questionAnswerPair.questionId;    
            this.answer = questionAnswerPair.answer;    
            this.quizSubmission = questionAnswerPair.quizSubmission;    
        }
    }
}
