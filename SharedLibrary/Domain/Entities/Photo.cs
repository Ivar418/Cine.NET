namespace SharedLibrary.Domain.Entities;

public class Photo
{
    public required string Id { set; get; }
    public required string Url { set; get; }
    public required string StorageKey { set; get; }
    public required long Size { set; get; }
    public required string ContentType { set; get; }
    public required int EntityId { get; set; }

}