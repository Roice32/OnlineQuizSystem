﻿namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp
{
    public abstract class QuestionBase
    {
        public Guid Id { get; set; }
        public QuestionType Type { get; set; }
        public string Text { get; set; }
        public int AllocatedPoints { get; set; }
        protected QuestionBase(Guid id, QuestionType type, string text, int allocatedPoints)
        {
            Id = id;
            Type = type;
            Text = text;
            AllocatedPoints = allocatedPoints;
        }
    }
}