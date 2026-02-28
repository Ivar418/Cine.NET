using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses.TMDB;

namespace API.Repositories.Interfaces;

public interface IMovieRepository
{
    Task<Movie?> GetMovieAsync(int id);
    Task<IEnumerable<Movie>> GetMoviesAsync();
    Task<Movie> AddMovieAsync(Movie movie);
    Task<Movie> UpdateMovieAsync(Movie movie);
    Task DeleteMovieAsync(int id);
    Task<TmdbMovieDetailsResponse>GetTmdbMovieDetailsAsync(int id);
    Task<IEnumerable<ReleaseInformationPerCountryDto>> GetMovieReleaseDatesAllCountriesAsync(int id);
    Task<ReleaseInformationDto> GetMovieReleaseDatesAsync(int id);
    Task<MovieSearchResultListDto> GetMovieSearchResultsAsync(string query);
}