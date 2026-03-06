using SharedLibrary.DTOs.Responses;

namespace WA.Services.Http.Interfaces;

public interface IMovieApiClient
{
    Task<List<MovieResponse>?> GetAllMoviesAsync();
    Task<bool> DeleteMovieAsync(int id);
}