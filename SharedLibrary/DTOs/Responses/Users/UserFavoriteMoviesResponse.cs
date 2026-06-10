namespace SharedLibrary.DTOs.Responses.Users;

public record UserFavoriteMovieResponse(
    int Id,
    int UserId,
    int MovieId
);

