using API.Domain.Common;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses.TMDB;

namespace API.Services.Implementations;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
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

    public async Task<ResultOf<Movie>> AddMovieAsync(TmdbMovieDetailsResponse movieDetails)
    {
        try
        {
            var movie = await _movieRepository.AddMovieAsync(movieDetails);

            return ResultOf<Movie>.Success(movie);
        }
        catch (Exception ex)
        {
            return ResultOf<Movie>.Failure(ex.Message);
        }
    }

    public async Task<ResultOf<Movie>> AddMovieFromTmdbAsync(int tmdbId, string informationLanguage)
    {
        return await _movieRepository.AddMovieFromTmdbAsync(tmdbId, informationLanguage);
    }

    public async Task<ResultOf<ICollection<Movie>>> GetMoviesAsync(string informationLanguage)
    {
        return await _movieRepository.GetMoviesAsync(informationLanguage);
    }

    public async Task<ResultOf<Movie>> DeleteMovieByTmdbIdAsync(int tmdbId)
    {
        return await _movieRepository.DeleteMovieByTmdbIdAsync(tmdbId);
    }

    public async Task<ResultOf<Movie>> GetMovieAsync(int id)
    {
        return await _movieRepository.GetMovieAsync(id);
    }

    public async Task<MovieSearchResultListDto> GetMovieTmdbSearchResultsAsync(string query,
        string? primary_release_year, int? page, bool include_adult,
        string language)
    {
        return await _movieRepository.GetMovieTmdbSearchResultsAsync(query, primary_release_year, page, include_adult,
            language);
    }
}