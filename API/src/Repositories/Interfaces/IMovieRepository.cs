using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses.TMDB;

namespace API.Repositories.Interfaces;

public interface IMovieRepository
{
    Task<ResultOf<Movie>> GetMovieAsync(int id);
    Task<ResultOf<List<Movie>>> GetMoviesAsync();
    Task<Movie> AddMovieAsync(TmdbMovieDetailsResponse movie);
    Task<ResultOf<Movie>> AddMovieFromTmdbAsync(int tmdbId);
    Task<Movie> UpdateMovieAsync(Movie movie);
    Task DeleteMovieAsync(int id);
    Task<TmdbMovieDetailsResponse?> GetTmdbMovieDetailsAsync(int id);
    Task<IEnumerable<ReleaseInformationPerCountryDto>> GetMovieReleaseDatesAllCountriesAsync(int id);
    Task<ReleaseInformationDto> GetMovieReleaseDatesAsync(int id);
    Task<MovieSearchResultListDto> GetMovieTmdbSearchResultsAsync(string query);
    Task<List<MovieSearchItemDto>> GetMovieSearchResultsAsync(string query);

}