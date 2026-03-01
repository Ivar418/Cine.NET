namespace SharedLibrary.DTOs.Responses;

public record PhotoResponse
{
    public required string Id { get; init; }
    public required string Url { get; init; }
    public required long Size { get; init; }
    public required string ContentType { get; init; }
    public required int EntityId { get; set; }
}