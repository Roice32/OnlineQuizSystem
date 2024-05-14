namespace OQS.CoreWebAPI.Entities;

// temporary User class
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public UserType Type { get; set; }
    public DateTime CreatedAt { get; set; }
   // public string Email { get; set; }
}