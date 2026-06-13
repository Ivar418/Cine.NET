namespace SharedLibrary.DTOs.Responses.Users;

public record UserFavoriteMoviesListResponse(
    int UserId,
    IEnumerable<UserFavoriteMovieResponse> FavoriteMovies);