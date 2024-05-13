namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp
{
    public class SingleChoiceQuestion : ChoiceQuestionBase
    {
        public string SingleChoiceAnswer { get; set; }
        public SingleChoiceQuestion(Guid id, string text, List<string> choices, string singleChoiceAnswer, int allocatedPoints, Guid quizId) :
            base(id, text, QuestionType.SingleChoice, choices, allocatedPoints, quizId)
        {
            SingleChoiceAnswer = singleChoiceAnswer;
        }

    }
}