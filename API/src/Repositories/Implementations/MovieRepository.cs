using API.Infrastructure.Database;
using API.Repositories.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses.TMDB;

namespace API.Repositories.Implementations;

public class MovieRepository(ApiDbContext db) : IMovieRepository
{
    private readonly ApiDbContext _db = db;

    public async Task<Movie?> GetMovieAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Movie>> GetMoviesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Movie> AddMovieAsync(Movie movie)
    {
        throw new NotImplementedException();
    }

    public async Task<Movie> UpdateMovieAsync(Movie movie)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteMovieAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<TmdbMovieDetailsResponse> GetImdbMovieDetailsAsync(int id)
    {
        throw new NotImplementedException();
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