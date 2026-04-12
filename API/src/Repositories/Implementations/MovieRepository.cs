using System.Net.Http.Headers;
using System.Text.Json;
using API.Domain.Common;
using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses.TMDB;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.DTOs.Responses.TMDB.Genre;
using SharedLibrary.DTOs.Responses.TMDB.MovieReleaseDatesAndInfo;
using SharedLibrary.DTOs.Responses.TMDB.Videos;

namespace API.Repositories.Implementations;

public class MovieRepository : IMovieRepository
{
    private readonly ApiDbContext _db;

    public MovieRepository(ApiDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves a movie by internal identifier.
    /// </summary>
    /// <param name="id">The movie identifier.</param>
    /// <returns>A success result with the movie, or a failure when no movie is found.</returns>
    public async Task<ResultOf<Movie>> GetMovieAsync(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        return movie == null ? ResultOf<Movie>.Failure("Movie not found") : ResultOf<Movie>.Success(movie);
    }

    /// <summary>
    /// Retrieves all stored movie records that share a TMDB identifier.
    /// </summary>
    /// <param name="tmdbId">The TMDB movie identifier.</param>
    /// <returns>A success result with matching movies, or a failure when none are found.</returns>
    public async Task<ResultOf<IEnumerable<Movie>>> GetMoviesByTmdbIdAsync(int tmdbId)
    {
        var movies = await _db.Movies.Where(m => m.TmdbId == tmdbId).ToListAsync();
        return movies.Count == 0
            ? ResultOf<IEnumerable<Movie>>.Failure("Movie not found")
            : ResultOf<IEnumerable<Movie>>.Success(movies);
    }

    /// <summary>
    /// Retrieves movies filtered by information language, or all movies when <c>all</c> is requested.
    /// </summary>
    /// <param name="informationLanguage">The information language filter value.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the filtered movie collection,
    /// or a failure result when retrieval fails.
    /// </returns>
    public async Task<ResultOf<ICollection<Movie>>> GetMoviesAsync(string informationLanguage)
    {
        try
        {
            List<Movie> movies;
            if (informationLanguage == "all")
            {
                movies = await _db.Movies.ToListAsync();
            }
            else
            {
                movies = await _db.Movies.Where(m => m.InformationLanguage == informationLanguage).ToListAsync();
            }

            return ResultOf<ICollection<Movie>>.Success(movies);
        }
        catch (Exception e)
        {
            return ResultOf<ICollection<Movie>>.Failure(e.Message);
        }
    }

    /// <summary>
    /// Creates and persists a movie entity from TMDB details and derived metadata.
    /// </summary>
    /// <param name="movie">The TMDB movie details payload.</param>
    /// <param name="informationLanguage">Optional language code used for stored metadata context.</param>
    /// <returns>The persisted <see cref="Movie"/> entity.</returns>
    public async Task<Movie> AddMovieAsync(TmdbMovieDetailsResponse movie, string? informationLanguage = null)
    {
        var firstLanguage = movie.SpokenLanguages?.FirstOrDefault();
        var dutchReleaseInfo = await GetDutchMovieReleaseDatesAsync(movie.Id);
        var dutchAgeIndication = dutchReleaseInfo?.Certification;
        var youtubeKey = await GetMovieYoutubeTrailerAsync(movie.Id);
        var result = await _db.Movies.AddAsync(new Movie
        {
            Title = movie.OriginalTitle,
            TmdbId = movie.Id,
            InformationLanguage = informationLanguage ?? "und",
            Language = movie.OriginalLanguage,
            PosterPath = movie.PosterPath,
            BackdropPath = movie.BackdropPath,
            YoutubeTrailerKey = youtubeKey.FirstOrDefault()?.Key,
            Runtime = movie.Runtime,
            ImdbId = movie.ImdbId,
            ReleaseDate = movie.ReleaseDate,
            About = movie.Overview,
            AgeIndication = dutchAgeIndication,
            SpokenLanguageName = firstLanguage?.EnglishName,
            SpokenLanguageCodeIso6391 = firstLanguage?.Iso_639_1,
            GenresIds = movie.Genres.Select(genreDto => genreDto.Id).ToList(),
            RowCreatedTimestampUtc = Movie.CurrentUtcTimestamp()
        });
        await _db.SaveChangesAsync();
        return result.Entity;
    }

    /// <summary>
    /// Adds a movie from TMDB for a given language when it does not already exist locally.
    /// </summary>
    /// <param name="tmdbId">The TMDB movie identifier.</param>
    /// <param name="language">The metadata language code.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the persisted movie,
    /// or a failure result when the movie exists already or cannot be fetched.
    /// </returns>
    public async Task<ResultOf<Movie>> AddMovieFromTmdbAsync(int tmdbId, string language = "und")
    {
        try
        {
            // Check if movie already exists
            var existingMovie =
                await _db.Movies.FirstOrDefaultAsync(m => m.TmdbId == tmdbId && m.InformationLanguage == language);
            if (existingMovie != null)
            {
                return ResultOf<Movie>.Failure("Movie already exists");
            }

            // Fetch details from TMDB
            var details = await GetTmdbMovieDetailsAsync(tmdbId, language);
            return details == null
                ? ResultOf<Movie>.Failure("Movie not found on TMDB")
                : ResultOf<Movie>.Success(await AddMovieAsync(details, language));
        }
        catch (HttpRequestException)
        {
            return ResultOf<Movie>.Failure("Movie not found on TMDB");
        }
    }

    /// <summary>
    /// Deletes all local movie records matching the provided TMDB identifier.
    /// </summary>
    /// <param name="tmdbId">The TMDB movie identifier.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing one of the deleted movie records,
    /// or a failure result when no matching movies are found.
    /// </returns>
    public async Task<ResultOf<Movie>> DeleteMovieByTmdbIdAsync(int tmdbId)
    {
        var movies = await GetMoviesByTmdbIdAsync(tmdbId);
        if (movies.IsFailure) return ResultOf<Movie>.Failure("Movie not found");
        foreach (var movie in movies.Value)
        {
            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();
        }

        return ResultOf<Movie>.Success(movies.Value.First());
    }

    /// <summary>
    /// Retrieves the TMDB movie genre list for a specific language.
    /// </summary>
    /// <param name="language">The language code used by TMDB for localized genre names.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the TMDB genre payload,
    /// or a failure result when the external request fails.
    /// </returns>
    public async Task<ResultOf<GenreResultList>> GetAllGenresFromTmdb(string language = "und")
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
            var url = $"https://api.themoviedb.org/3/genre/movie/list?language={language}";
            var response = await client.GetAsync(url);

            // Ensure success status code
            if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                return ResultOf<GenreResultList>.Failure("Failed to fetch genres from TMDB");
            }

            // Read response content
            var content = await response.Content.ReadAsStringAsync();
            var genreList = JsonSerializer.Deserialize<GenreResultList>(content) ??
                            throw new Exception("Could not deserialize genre list");

            return ResultOf<GenreResultList>.Success(genreList);
        }
        catch (Exception e)
        {
            return ResultOf<GenreResultList>.Failure(e.Message);
        }
    }

    /// <summary>
    /// Persists a collection of genres to the database.
    /// </summary>
    /// <param name="genres">The genre entities to save.</param>
    /// <returns>A success result containing the saved genre collection.</returns>
    public async Task<ResultOf<IEnumerable<Genre>>> SaveGenres(IEnumerable<Genre> genres)
    {
        _db.Genres.AddRange(genres);
        await _db.SaveChangesAsync();
        return ResultOf<IEnumerable<Genre>>.Success(genres);
    }

    /// <summary>
    /// Fetches TMDB genres for a language, selects one genre by TMDB ID, and persists it.
    /// </summary>
    /// <param name="language">The language code used to fetch localized genre names.</param>
    /// <param name="tmdbGenreId">The TMDB genre identifier to persist.</param>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing the saved genre entity sequence,
    /// or a failure result when fetch, lookup, or save fails.
    /// </returns>
    public async Task<ResultOf<IEnumerable<Genre>>> SaveGenreByTmdbGenreId(string language, int tmdbGenreId)
    {
        var genres = await GetAllGenresFromTmdb(language);
        if (genres.IsFailure) return ResultOf<IEnumerable<Genre>>.Failure(genres.Error);
        var genre = genres.Value.Genres.FirstOrDefault(g => g.Id == tmdbGenreId);
        if (genre == null) return ResultOf<IEnumerable<Genre>>.Failure("Genre not found");
        var genreEntity = new Genre
        {
            TmdbId = genre.Id,
            Name = genre.Name,
            Language = language
        };
        var savedGenre = SaveGenres(new List<Genre> { genreEntity });
        if (savedGenre.Result.IsFailure) return ResultOf<IEnumerable<Genre>>.Failure(savedGenre.Result.Error);

        return ResultOf<IEnumerable<Genre>>.Success(savedGenre.Result.Value);
    }

    /// <summary>
    /// Retrieves a stored genre by TMDB genre identifier and language.
    /// </summary>
    /// <param name="tmdbGenreId">The TMDB genre identifier.</param>
    /// <param name="language">The language code of the stored genre record.</param>
    /// <returns>A success result with the genre, or a failure when no match exists.</returns>
    public async Task<ResultOf<Genre>> GetGenreByTmdbGenreId(int tmdbGenreId, string language)
    {
        var genreResult = await _db.Genres
            .FirstOrDefaultAsync(g => g.TmdbId == tmdbGenreId && g.Language == language);

        return genreResult != null ? ResultOf<Genre>.Success(genreResult) : ResultOf<Genre>.Failure("Genre not found");
    }

    /// <summary>
    /// Retrieves all genre records currently stored in the database.
    /// </summary>
    /// <returns>
    /// A <see cref="ResultOf{T}"/> containing all stored genres,
    /// or a failure result when retrieval fails.
    /// </returns>
    public async Task<ResultOf<IEnumerable<Genre>>> GetAllGenresOnDb()
    {
        try
        {
            var result = await _db.Genres.ToListAsync();
            return ResultOf<IEnumerable<Genre>>.Success(result);
        }
        catch (Exception e)
        {
            return ResultOf<IEnumerable<Genre>>.Failure(e.Message);
        }
    }

    /// <summary>
    /// Retrieves detailed movie metadata from TMDB.
    /// </summary>
    /// <param name="id">The TMDB movie identifier.</param>
    /// <param name="language">The language code for localized metadata.</param>
    /// <returns>
    /// The deserialized TMDB movie details response, or <c>null</c> when TMDB returns a non-success status.
    /// </returns>
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

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TmdbMovieDetailsResponse>(content) ??
                   throw new Exception("Could not deserialize movie details");
        }
        catch (JsonException e)
        {
            throw new Exception("Error parsing movie details", e);
        }
    }

    /// <summary>
    /// Retrieves movie release-date metadata for all countries from TMDB.
    /// </summary>
    /// <param name="id">The TMDB movie identifier.</param>
    /// <returns>The deserialized release-date payload, or <c>null</c> on non-success responses.</returns>
    public async Task<MovieReleaseDatesDto> GetMovieReleaseDatesAllCountriesAsync(int id)
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
            // Make the GET request
            var url = $"https://api.themoviedb.org/3/movie/{id}/release_dates";
            var response = await client.GetAsync(url);

            // Ensure success status code
            if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                return null;
            }

            // Read response content
            // var content = await response.Content.ReadAsStringAsync();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MovieReleaseDatesDto>(content);
            return result;
        }
        catch (JsonException e)
        {
            throw new Exception("Error parsing movie details", e);
        }
    }

    /// <summary>
    /// Retrieves the first Dutch release information entry for a TMDB movie.
    /// </summary>
    /// <param name="id">The TMDB movie identifier.</param>
    /// <returns>The first Dutch release information record, or <c>null</c> when unavailable.</returns>
    public async Task<ReleaseInformationDto?> GetDutchMovieReleaseDatesAsync(int id)
    {
        var allReleaseInformation = await GetMovieReleaseDatesAllCountriesAsync(id);
        var dutchReleaseInformation =
            allReleaseInformation.Results.FirstOrDefault(releaseInfo => releaseInfo.CountryOfRelease == "NL");
        if (dutchReleaseInformation != null)
        {
            return dutchReleaseInformation.release_dates.FirstOrDefault();
        }

        return null;
    }

    /// <summary>
    /// Retrieves YouTube trailer videos for a TMDB movie, prioritized by official and publication date.
    /// </summary>
    /// <param name="tmdbId">The TMDB movie identifier.</param>
    /// <returns>A sequence of filtered trailer video items, or an empty sequence when retrieval fails.</returns>
    public async Task<IEnumerable<VideoResultItem>> GetMovieYoutubeTrailerAsync(int tmdbId)
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
            var url = $"https://api.themoviedb.org/3/movie/{tmdbId}/videos";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var items = JsonSerializer.Deserialize<VideoResultList>(content)
                        ?? throw new Exception("Could not deserialize movie details");

            var youtubeTrailers = items.Results
                .Where(v => v.Type == "Trailer" && v.Site == "YouTube");

            return youtubeTrailers
                .OrderByDescending(v => v.Official) // official first
                .ThenByDescending(v => v.PublishedAt); // newest second
        }
        catch (Exception e)
        {
            return new List<VideoResultItem>();
        }
    }

    /// <summary>
    /// Searches TMDB movies using query filters and returns the deserialized search result payload.
    /// </summary>
    /// <param name="query">The free-text search query.</param>
    /// <param name="primary_release_year">Optional primary release year filter.</param>
    /// <param name="page">Optional result page number.</param>
    /// <param name="include_adult">Whether adult titles should be included.</param>
    /// <param name="language">Optional language code for localized results.</param>
    /// <returns>The TMDB search response payload.</returns>
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

        var parameters = new List<string>();

        parameters.Add($"query={Uri.EscapeDataString(query)}");
        parameters.Add($"include_adult={include_adult}");

        if (!string.IsNullOrEmpty(primary_release_year))
        {
            parameters.Add($"primary_release_year={primary_release_year}");
        }

        if (page.HasValue)
        {
            parameters.Add($"page={page}");
        }

        if (!string.IsNullOrEmpty(language))
        {
            parameters.Add($"language={language}");
        }


        var url = $"https://api.themoviedb.org/3/search/movie?{string.Join("&", parameters)}";
        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MovieSearchResultListDto>(content) ??
               throw new Exception("Could not deserialize movie details");
    }
}