using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;
using SharedLibrary.DTOs.Responses.TMDB;

namespace API.Services.Interfaces;

public interface IMovieService
{
    Task<ResultOf<IEnumerable<Movie>>> AddMovieAsyncForEachSpecifiedLanguage(int tmdbId, IEnumerable<string>? informationLanguages = null);
    Task<ResultOf<Movie>> AddMovieAsync(TmdbMovieDetailsResponse movieDetails);
    Task<ResultOf<Movie>> AddMovieFromTmdbAsync(int tmdbId, string informationLanguage);

}