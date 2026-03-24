namespace API.Domain.Model;

public class EmailSubscription
{
    public int Id { get; init; }
    public required string Email { get; init; }
}