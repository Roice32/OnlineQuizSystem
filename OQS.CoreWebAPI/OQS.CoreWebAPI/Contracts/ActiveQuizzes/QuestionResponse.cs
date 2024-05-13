using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Contracts;

public class QuestionResponse
{
    public Guid Id { get; set; }
    public QuestionType Type { get; set; }
    public string Text { get; set; } 
    public List<string>? choices { get; set; }

    public QuestionResponse(QuestionBase question)
    {
        Id = question.Id;
        Type = question.Type;
        Text = question.Text;
        if (question is ChoiceQuestionBase choiceQuestion)
        {
            choices = choiceQuestion.Choices;
            Random rng = new Random();
            int n = choices.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (choices[k], choices[n]) = (choices[n], choices[k]);
            }
        }
    }

}