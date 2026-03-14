using API.Domain.Common;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;
using SharedLibrary.DTOs.Responses.TMDB;

namespace API.Services.Interfaces;

public interface IMovieService
{
    Task<ResultOf<IEnumerable<Movie>>> AddMovieAsyncForEachSpecifiedLanguage(int TmdbId, IEnumerable<string>? informationLanguages);
    Task<ResultOf<Movie>> AddMovieAsync(int TmdbId, string InfomrationLanguage = TODO);
}