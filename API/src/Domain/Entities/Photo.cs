namespace WebApi_PocV1.Domain.Entities;

public record Photo
{
    public required string Id;
    public required string Url;
    public required string StorageKey;
    public required long Size;
    public required string ContentType;
    public required string creatorId;
}
