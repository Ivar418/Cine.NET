using SharedLibrary.Domain.Entities;

namespace API.Repositories.Interfaces;

public interface IMovieRepository
{
    Task<Movie?> GetMovieAsync(int id);
    Task<IEnumerable<Movie>> GetMoviesAsync();
    Task<Movie> AddMovieAsync(Movie movie);
}