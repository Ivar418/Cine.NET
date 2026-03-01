using System.Net.Http.Headers;
using System.Text.Json;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses.TMDB;
using DotNetEnv;

namespace API.Repositories.Implementations;

public class MovieRepository : IMovieRepository
{
    private readonly ApiDbContext _db;

    public MovieRepository(ApiDbContext db)
    {
        _db = db;
    }

    public async Task<Movie?> GetMovieAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Movie>> GetMoviesAsync()
    {
        throw new NotImplementedException();
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

    public async Task<Movie> UpdateMovieAsync(Movie movie)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteMovieAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<TmdbMovieDetailsResponse> GetTmdbMovieDetailsAsync(int id)
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

            // Make the GET request
            var url = $"https://api.themoviedb.org/3/movie/{id}";
            var response = await client.GetAsync(url);

            // Ensure success status code
            response.EnsureSuccessStatusCode();

            // Read response content
            // var content = await response.Content.ReadAsStringAsync();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TmdbMovieDetailsResponse>(content) ??
                   throw new Exception("Could not deserialize movie details");
        }
        catch (HttpRequestException e)
        {
            throw new Exception("Error fetching movie details from TMDB", e);
        }
        catch (JsonException e)
        {
            throw new Exception("Error parsing movie details", e);
        }
        catch (InvalidOperationException e)
        {
            // API key missing
            throw;
        }
        catch (Exception e)
        {
            // Fallback for unexpected exceptions
            throw new Exception("An unexpected error occurred while getting movie details", e);
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

    public async Task<MovieSearchResultListDto> GetMovieSearchResultsAsync(string query)
    {
        throw new NotImplementedException();
    }
}