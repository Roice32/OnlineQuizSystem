﻿namespace OQS.CoreWebAPI.Contracts.ResultsAndStatistics
{
    public class FetchQuizResultHeaderResponse
    {
        public Guid ResultId { get; set; }
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SubmittedAtUtc { get; set; }
        public float Score { get; set; }
        public bool ReviewPending { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string QuizName { get; set; } = string.Empty;
    }
}