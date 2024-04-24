using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Contracts
{
    public class SubmitUserResponseRequest
    {
        public Guid UserId { get; set; }
        public List<QuestionBase> Questions { get; set; }
    }
}