using API.Domain.Common;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;

namespace API.Services.Implementations;

public class MovieService : IMovieService
{
    private readonly MovieRepository _movieRepository;

    public MovieService(MovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<ResultOf<IEnumerable<Movie>>> AddMovieAsyncForEachSpecifiedLanguage(int tmdbId,
        IEnumerable<string>? informationLanguages = null)
    {
        var listOfAddedMovies = new List<Movie>();
        informationLanguages ??= ["en", "nl"];
        foreach (var informationLanguage in informationLanguages)
        {
            var movie = await AddMovieFromTmdbAsync(tmdbId, informationLanguage);
            if (movie.IsSuccess)
            {
                listOfAddedMovies.Add(movie.Value);
            }
        }

        return ResultOf<IEnumerable<Movie>>.Success(listOfAddedMovies);
    }

    public async Task<ResultOf<Movie>> AddMovieFromTmdbAsync(int tmdbId, string informationLanguage)
    {
        return await _movieRepository.AddMovieFromTmdbAsync(tmdbId, informationLanguage);
    }
}