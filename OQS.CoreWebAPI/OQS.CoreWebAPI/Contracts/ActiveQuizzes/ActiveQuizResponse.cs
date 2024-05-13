namespace OQS.CoreWebAPI.Contracts;

public class ActiveQuizResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public int TimeLimitMinutes { get; set; }
    public List<NakedQuestionResponse> Questions { get; set; } = new();  
    
    public ActiveQuizResponse(Entities.ActiveQuiz.ActiveQuiz activeQuiz)
    {
        Id = activeQuiz.Id;
        Name = activeQuiz.Quiz.Name;
        TimeLimitMinutes = activeQuiz.Quiz.TimeLimitMinutes;
        Questions = activeQuiz.Quiz.Questions.Select(q => new NakedQuestionResponse(q)).ToList();
        Random rng = new Random();
        int n = Questions.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (Questions[k], Questions[n]) = (Questions[n], Questions[k]);
        }
    }
}