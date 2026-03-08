using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses.TMDB;
using SharedLibrary.DTOs.Responses.TMDB.MovieReleaseDatesAndInfo;

namespace API.Repositories.Interfaces;

public interface IMovieRepository
{
    Task<ResultOf<Movie>> GetMovieAsync(int id);
    Task<ResultOf<ICollection<Movie>>> GetMoviesAsync();
    Task<Movie> AddMovieAsync(TmdbMovieDetailsResponse movie);
    Task<ResultOf<Movie>> AddMovieFromTmdbAsync(int tmdbId, string language = "nl");
    Task<Movie> UpdateMovieAsync(Movie movie);
    Task<ResultOf<Movie>> DeleteMovieAsync(int tmdbId);
    Task<TmdbMovieDetailsResponse?> GetTmdbMovieDetailsAsync(int id, string language);
    Task<MovieReleaseDatesDto> GetMovieReleaseDatesAllCountriesAsync(int id);
    Task<ReleaseInformationDto?> GetDutchMovieReleaseDatesAsync(int id);
    Task<MovieSearchResultListDto> GetMovieTmdbSearchResultsAsync(string query, string? primary_release_year, int? page, bool include_adult, string language);
    Task<List<MovieSearchItemDto>> GetMovieSearchResultsAsync(string query);

}