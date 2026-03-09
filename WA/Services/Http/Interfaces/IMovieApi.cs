using SharedLibrary.DTOs.Responses;

namespace WA.Services.Http.Interfaces;

public interface IMovieApiClient
{
    Task<List<MovieResponse>?> GetAllMoviesAsync();
    Task<MovieResponse?> GetMovieByIdAsync(int id);
    Task<bool> DeleteMovieAsync(int id);
}