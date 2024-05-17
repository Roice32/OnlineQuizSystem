namespace OQS.CoreWebAPI.Entities;

public class LiveQuizz
{
    public string Code { get; set; }
    public Quiz Quiz { get; set; }
    public User CreatedBy { get; set; }
    public List<UserConnection> Connections { get; set; }
}