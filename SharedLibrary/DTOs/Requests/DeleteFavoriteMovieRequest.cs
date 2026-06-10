namespace SharedLibrary.DTOs.Requests;

public record DeleteFavoriteMovieRequest(
    int UserId,
    int MovieId);