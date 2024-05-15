namespace OQS.CoreWebAPI.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}