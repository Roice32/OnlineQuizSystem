namespace OQS.CoreWebAPI.Entities;
// temporary User class
public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; }=string.Empty;
    public string Email { get; set; }=string.Empty;
}