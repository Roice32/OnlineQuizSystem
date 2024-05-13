namespace OQS.CoreWebAPI.Shared;

public class Pagination
{
    public int Offset { get; set; } = 0;
    public int Limit { get; set; } = 10;
    public int TotalRecords { get; set; } = 0;
}