using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;
using SharedLibrary.DTOs.Responses.TMDB;

namespace API.Services.Interfaces;

public interface IMovieService
{
    Task<ResultOf<IEnumerable<Movie>>> AddMovieAsyncForEachSpecifiedLanguage(int tmdbId,
        IEnumerable<string>? informationLanguages = null);

    Task<ResultOf<Movie>> AddMovieAsync(TmdbMovieDetailsResponse movieDetails);
    Task<ResultOf<Movie>> AddMovieFromTmdbAsync(int tmdbId, string informationLanguage);
    Task<ResultOf<ICollection<Movie>>> GetMoviesAsync(string informationLanguage);
    Task<ResultOf<Movie>> DeleteMovieByTmdbIdAsync(int tmdbId);
    Task<ResultOf<Movie>> GetMovieAsync(int id);

    Task<MovieSearchResultListDto> GetMovieTmdbSearchResultsAsync(string query, string? primary_release_year, int? page,
        bool include_adult, string language);

    Task<ResultOf<IEnumerable<Genre>>> FetchAllGenresForAllSpecifiedLanguagesAndSaveToDb(
        IEnumerable<string>? informationLanguages = null);

    Task<ResultOf<IEnumerable<Genre>>> FetchGenreByLanguage(int tmdbGenreId, string language);
}