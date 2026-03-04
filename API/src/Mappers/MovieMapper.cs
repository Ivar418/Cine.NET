using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Responses;

namespace API.Mappers;

public static class MovieMapper
{
    public static MovieResponse ToResponse(Movie movie)
    {
        return new MovieResponse
        {
            Id                        = movie.Id,
            Title                     = movie.Title,
            TmdbId                    = movie.TmdbId,
            Language                  = movie.Language,
            PosterUrl                 = movie.PosterUrl,
            Runtime                   = movie.Runtime,
            ImdbId                    = movie.ImdbId,
            ReleaseDate               = movie.ReleaseDate,
            About                     = movie.About,
            AgeIndication             = movie.AgeIndication,
            SpokenLanguageName        = movie.SpokenLanguageName,
            SpokenLanguageCodeIso6391 = movie.SpokenLanguageCodeIso6391,
            GenreIds                  = movie.GenresIds,
            RowCreatedTimestampUtc    = movie.RowCreatedTimestampUtc,
            RowUpdatedTimestampUtc    = movie.RowUpdatedTimestampUtc
        };
    }

    public static IEnumerable<MovieResponse> ToResponses(IEnumerable<Movie> movies)
    {
        return movies.Select(ToResponse);
    }
}