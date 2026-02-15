namespace WebApi_PocV1.DTOs.Requests;

public record PhotoRequest
{
    public required string PhotoType { get; init; }
    public required string CreatorId { get; init; }
}