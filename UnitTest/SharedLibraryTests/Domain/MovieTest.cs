using SharedLibrary.Domain.Entities;
using Xunit;

namespace UnitTest;

public class MovieTests
{
    private static Movie BuildValidMovie() => new Movie
    {
        Id = 1,
        Title = "Inception",
        TmdbId = 27205,
        RowCreatedTimestampUtc = "2026-01-01T00:00:00.0000000+00:00"
    };

    [Fact]
    public void Movie_DefaultInformationLanguage_IsUnd()
    {
        var movie = new Movie();

        Assert.Equal("und", movie.InformationLanguage);
    }

    [Fact]
    public void Movie_DefaultTitle_IsEmptyString()
    {
        var movie = new Movie();

        Assert.Equal(string.Empty, movie.Title);
    }

    [Fact]
    public void Movie_DefaultTmdbId_IsZero()
    {
        var movie = new Movie();

        Assert.Equal(0, movie.TmdbId);
    }

    [Fact]
    public void RowCreatedTimestampUtc_ValidTimestamp_SetsSuccessfully()
    {
        var movie = BuildValidMovie();

        Assert.Equal("2026-01-01T00:00:00.0000000+00:00", movie.RowCreatedTimestampUtc);
    }

    [Fact]
    public void RowUpdatedTimestampUtc_ValidTimestamp_SetsSuccessfully()
    {
        var movie = BuildValidMovie();
        movie.RowUpdatedTimestampUtc = "2025-06-15T12:30:00.0000000+00:00";

        Assert.Equal("2025-06-15T12:30:00.0000000+00:00", movie.RowUpdatedTimestampUtc);
    }

    [Fact]
    public void RowUpdatedTimestampUtc_Null_IsAllowed()
    {
        var movie = BuildValidMovie();
        movie.RowUpdatedTimestampUtc = null;

        Assert.Null(movie.RowUpdatedTimestampUtc);
    }

    [Fact]
    public void RowCreatedTimestampUtc_InvalidTimestamp_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Movie
        {
            RowCreatedTimestampUtc = "not-a-timestamp"
        });
    }

    [Fact]
    public void RowUpdatedTimestampUtc_InvalidTimestamp_ThrowsArgumentException()
    {
        var movie = BuildValidMovie();

        Assert.Throws<ArgumentException>(() =>
        {
            movie.RowUpdatedTimestampUtc = "2026-01-01T00:00:00";  // Missing offset
        });
    }

    [Fact]
    public void RowCreatedTimestampUtc_NonUtcOffset_ThrowsArgumentException()
    {
        // Valid timestamp format but NOT +00:00 — should be rejected
        Assert.Throws<ArgumentException>(() => new Movie
        {
            RowCreatedTimestampUtc = "2026-01-01T00:00:00.0000000+02:00"
        });
    }

    [Fact]
    public void CurrentUtcTimestamp_ReturnsValidUtcString()
    {
        var timestamp = Movie.CurrentUtcTimestamp();

        var parsed = DateTimeOffset.Parse(timestamp);
        Assert.Equal(TimeSpan.Zero, parsed.Offset);
    }

    [Fact]
    public void CurrentUtcTimestamp_IsCloseToNow()
    {
        var before = DateTimeOffset.UtcNow;
        var timestamp = Movie.CurrentUtcTimestamp();
        var after = DateTimeOffset.UtcNow;

        var parsed = DateTimeOffset.Parse(timestamp);

        Assert.InRange(parsed, before, after);
    }

    [Theory]
    [InlineData("not-a-date")]
    [InlineData("2026-01-01")]
    [InlineData("2026-01-01T00:00:00")]
    [InlineData("2026-01-01T00:00:00.0000000+02:00")]
    public void RowUpdatedTimestampUtc_VariousInvalidFormats_AllThrow(string invalidTimestamp)
    {
        var movie = BuildValidMovie();

        Assert.Throws<ArgumentException>(() =>
        {
            movie.RowUpdatedTimestampUtc = invalidTimestamp;
        });
    }
}