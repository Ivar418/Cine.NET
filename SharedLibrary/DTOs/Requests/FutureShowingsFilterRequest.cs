namespace SharedLibrary.DTOs.Requests;

public record FutureShowingsFilterRequest(
    DateTimeOffset? From);