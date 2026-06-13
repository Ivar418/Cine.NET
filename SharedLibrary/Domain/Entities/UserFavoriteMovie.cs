namespace SharedLibrary.Domain.Entities;

/// <summary>
/// Represents a user's favorite movie.
/// </summary>
/// <remarks>
/// This entity is used to associate a user with a movie that they have marked as a favorite in the system.
/// </remarks>
public class UserFavoriteMovie {
    /// <summary>
    /// Gets or sets the unique identifier for the user favorite movie.
    /// </summary>
    public int Id { get; init; }


    /// <summary>
    /// Gets or sets the identifier of the user associated with the favorite movie.
    /// </summary>
    /// <remarks>
    /// This property represents the unique ID of the user to whom the favorite movie is linked.
    /// It acts as a foreign key relationship between the User and UserFavoriteMovie entities.
    /// </remarks>
    public required int UserId { get; init; }

    public User User { get; init; } = null!;

    /// <summary>
    /// Gets or sets the identifier of the movie associated with the user's favorite movie list.
    /// Represents a unique identifier for a specific movie.
    /// </summary>
    public required int MovieId { get; init; }

    public Movie Movie { get; init; } = null!;
}