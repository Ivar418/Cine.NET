using System.Net.Http.Headers;
using System.Text.Json;
using API.Domain.Common;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses.TMDB;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace API.Repositories.Implementations;

public class MovieRepository : IMovieRepository
{
    private readonly ApiDbContext _db;

    public MovieRepository(ApiDbContext db)
    {
        _db = db;
    }

    public async Task<ResultOf<Movie>> GetMovieAsync(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        return movie == null ? ResultOf<Movie>.Failure("Movie not found") : ResultOf<Movie>.Success(movie);
    }

    public async Task<ResultOf<List<Movie>>> GetMoviesAsync()
    {
        try
        {
            var movies = await _db.Movies.ToListAsync();

            return ResultOf<List<Movie>>.Success(movies);
        }
        catch (Exception e)
        {
            return ResultOf<List<Movie>>.Failure(e.Message);
        }
    }

    public async Task<Movie> AddMovieAsync(TmdbMovieDetailsResponse movie)
    {
        Console.WriteLine($"Adding movie: {movie.OriginalTitle}");
        var firstLanguage = movie.SpokenLanguages?.FirstOrDefault();

        var result = await _db.Movies.AddAsync(new Movie
        {
            Title = movie.OriginalTitle,
            TmdbId = movie.Id,
            Language = movie.OriginalLanguage,
            PosterUrl = movie.PosterPath,
            Runtime = movie.Runtime,
            ImdbId = movie.ImdbId,
            ReleaseDate = movie.ReleaseDate,
            About = movie.Overview,
            AgeIndication = "PG-13",
            SpokenLanguageName = firstLanguage?.EnglishName,
            SpokenLanguageCodeIso6391 = firstLanguage?.Iso_639_1,
            GenresIds = movie.Genres.Select(genreDto => genreDto.Id).ToList(),
            RowCreatedTimestampUtc = Movie.CurrentUtcTimestamp()
        });
        await _db.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<ResultOf<Movie>> AddMovieFromTmdbAsync(int tmdbId, string language)
    {
        try
        {
            // Check if movie already exists
            var existingMovie = await _db.Movies.FirstOrDefaultAsync(m => m.TmdbId == tmdbId);
            if (existingMovie != null)
            {
                return ResultOf<Movie>.Failure("Movie already exists");
            }

            // Fetch details from TMDB
            var details = await GetTmdbMovieDetailsAsync(tmdbId, language);
            if (details == null)
            {
                return ResultOf<Movie>.Failure("Movie not found on TMDB");
            }

            return ResultOf<Movie>.Success(await AddMovieAsync(details));
        }
        catch (HttpRequestException)
        {
            return ResultOf<Movie>.Failure("Movie not found on TMDB");
        }
    }

    public async Task<Movie> UpdateMovieAsync(Movie movie)
    {
        throw new NotImplementedException();
    }

    public async Task<ResultOf<Movie>> DeleteMovieAsync(int id)
    {
        try
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null)
                return ResultOf<Movie>.Failure("Movie not found");

            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();

            return ResultOf<Movie>.Success(movie);
        }
        catch (Exception e)
        {
            return ResultOf<Movie>.Failure(e.Message);
        }
    }

    public async Task<TmdbMovieDetailsResponse?> GetTmdbMovieDetailsAsync(int id, string language)
    {
        try
        {
            Env.Load();
            // Get the API key from environment variables
            var apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY_READ_ONLY");
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("TMDB API key is not set in environment variables.");
            }

            using var client = new HttpClient();
            // Set the authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var parameters = new List<String>();
            parameters.Add($"{id}");
            parameters.Add($"?language={language}");
            // Make the GET request
            var url = $"https://api.themoviedb.org/3/movie/{parameters.Aggregate((key, value) => key + value)}";
            var response = await client.GetAsync(url);

            // Ensure success status code
            if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                return null;
            }

            // Read response content
            // var content = await response.Content.ReadAsStringAsync();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TmdbMovieDetailsResponse>(content) ??
                   throw new Exception("Could not deserialize movie details");
        }
        catch (JsonException e)
        {
            throw new Exception("Error parsing movie details", e);
        }
    }

    public async Task<IEnumerable<ReleaseInformationPerCountryDto>> GetMovieReleaseDatesAllCountriesAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ReleaseInformationDto> GetMovieReleaseDatesAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<MovieSearchResultListDto> GetMovieTmdbSearchResultsAsync(
        string query,
        string? primary_release_year,
        int? page,
        bool include_adult,
        string language)
    {
        Env.Load();
        // Get the API key from environment variables
        var apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY_READ_ONLY");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("TMDB API key is not set in environment variables.");
        }

        using var client = new HttpClient();
        // Set the authorization header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        // Make the GET request
        var parameters = new List<String>();
        parameters.Add($"query={query}");
        parameters.Add($"&include_adult={include_adult}");
        if (!string.IsNullOrEmpty(primary_release_year))
        {
            parameters.Add($"&primary_release_year={primary_release_year}");
        }

        if (page.HasValue)
        {
            parameters.Add($"&page={page}");
        }

        if (!string.IsNullOrEmpty(language))
        {
            parameters.Add($"&language={language}");
        }


        var url = $"https://api.themoviedb.org/3/search/movie?{parameters.Aggregate((key, value) => key + value)}";
        var response = await client.GetAsync(url);

        // Ensure success status code
        response.EnsureSuccessStatusCode();

        // Read response content
        // var content = await response.Content.ReadAsStringAsync();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MovieSearchResultListDto>(content) ??
               throw new Exception("Could not deserialize movie details");
    }

    public async Task<List<MovieSearchItemDto>> GetMovieSearchResultsAsync(string query)
    {
        throw new NotImplementedException();
    }
}