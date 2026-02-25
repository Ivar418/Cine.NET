using SharedLibrary.Domain.Entities;

namespace API.Storage;

public record PhotoSaveResult(string Id, string Url, string StorageKey, long Size, string ContentType, string creatorId);


public interface IPhotoStorage
{
    Task<PhotoSaveResult> SaveAsync(
        Stream stream,
        string contentType,
        string originalFileName,
        string folder,
        CancellationToken ct,
        string creatorId
    );
    Task <Photo> GetByCreatorAndType(string creatorId, string type);
}
