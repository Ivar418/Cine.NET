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
            try
            {
                var movie = await AddMovieFromTmdbAsync(tmdbId, informationLanguage);
                if (movie.IsSuccess)
                {
                    listOfAddedMovies.Add(movie.Value);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"[MovieService] Failed to add movie with TMDB ID {tmdbId} for language '{informationLanguage}': {ex.Message}");
            }
        }

        return ResultOf<IEnumerable<Movie>>.Success(listOfAddedMovies);
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

    public async Task<ResultOf<IEnumerable<Genre>>> FetchAllGenresForAllSpecifiedLanguagesAndSaveToDb(
        IEnumerable<string>? language = null)
    {
        language ??= ["en", "nl"];
        foreach (var languageItem in language)
        {
            var genreList = await _movieRepository.GetAllGenresFromTmdb(languageItem);
            var genres = genreList.Value.Genres.Select(genre => new Genre
                { TmdbId = genre.Id, Name = genre.Name, Language = languageItem });
            await _movieRepository.SaveGenres(genres);
        }

        var genresOnDb = await _movieRepository.GetAllGenresOnDb();
        if (genresOnDb.IsFailure) return ResultOf<IEnumerable<Genre>>.Failure(genresOnDb.Error);
        return ResultOf<IEnumerable<Genre>>.Success(genresOnDb.Value);
    }


    public async Task<ResultOf<Genre>> FetchGenreByLanguage(int tmdbGenreId, string language)
    {
        var genres = await _movieRepository.GetAllGenresOnDb();
        var result = genres.Value.FirstOrDefault(g => g.TmdbId == tmdbGenreId && g.Language == language);
        if (result == null)
        {
            var fetchResult = await _movieRepository.SaveGenreByTmdbGenreId(language, tmdbGenreId);
            if (fetchResult.IsFailure)
                return ResultOf<Genre>.Failure(fetchResult.Error);
            return ResultOf<Genre>.Success(fetchResult.Value.First());
        }
        return ResultOf<Genre>.Success(result);
    }
}