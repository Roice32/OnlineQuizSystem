using OQS.CoreWebAPI.Entities;
namespace OQS.CoreWebAPI.Contracts
{
    public class TagResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedOnUtc { get; set; }


        public TagResponse(Tag tag)
        {
            Id = tag.Id;
            Name = tag.Name;
            CreatedOnUtc = tag.CreatedOnUtc;
        }
    }
}
