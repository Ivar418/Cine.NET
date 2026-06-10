using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses.Users;

namespace API.Mappers;

public static class FavoriteMappers {
    public static UserFavoriteMoviesListResponse ToFavoriteListResponse(User user) {
        return new UserFavoriteMoviesListResponse(UserId: user.Id,
            FavoriteMovies: user.FavoriteMovies.Select(fm => new UserFavoriteMovieResponse(
                fm.Id,
                fm.UserId,
                fm.MovieId
            )));
    }
}