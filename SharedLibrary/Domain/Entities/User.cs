using System.Net.Mail;

namespace SharedLibrary.Domain.Entities;

public class User {
    public int Id { get; private set; }

    public string UserName { get; private set; } = null!;

    public string? PhotoId { get; private set; }
    public Photo? Photo { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public ICollection<UserFavoriteMovie> FavoriteMovies { get; private set; } = new HashSet<UserFavoriteMovie>();
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }


    public User(
        string userName,
        string firstName,
        string lastName,
        string email,
        HashSet<string>? favoriteMovieIds = null
    ) {
        SetUserName(userName);
        SetFirstName(firstName);
        SetLastName(lastName);
        SetEmail(email);
        SetFavoriteMovies(favoriteMovieIds ?? []);
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    protected User() { }

    public void ChangeName(string firstName, string lastName) {
        var normalisedFirstName = firstName.Trim();
        var normalisedLastName = lastName.Trim();
        if (normalisedFirstName == FirstName && normalisedLastName == LastName) {
            return;
        }

        SetFirstName(firstName);
        SetLastName(lastName);
        RefreshUpdatedAt();
    }

    public void ChangeEmail(string email) {
        var normalised = new MailAddress(email).Address;
        if (normalised == Email) {
            return;
        }

        SetEmail(email);
        RefreshUpdatedAt();
    }

    public void ChangeUserName(string username) {
        var normalized = username.Trim();
        if (normalized == UserName) {
            return;
        }

        SetUserName(username);
        RefreshUpdatedAt();
    }

    public void AddFavoriteMovie(string movieId) {
        movieId = movieId.Trim();

        if (FavoriteMovies.Any(x => x.MovieId == movieId))
            return;

        FavoriteMovies.Add(new UserFavoriteMovie { UserId = Id, MovieId = movieId });
        RefreshUpdatedAt();
    }

    public void RemoveFavoriteMovie(string movieId) {
        movieId = movieId.Trim();

        var favorite = FavoriteMovies.FirstOrDefault(x => x.MovieId == movieId);

        if (favorite is null)
            return;

        FavoriteMovies.Remove(favorite);
        RefreshUpdatedAt();
    }

    public void ChangePhoto(Photo? photo) {
        if (PhotoId == photo?.Id)
            return;

        SetPhoto(photo);
        RefreshUpdatedAt();
    }

    private void SetFavoriteMovies(IEnumerable<string> favoriteMovieIds) {
        ArgumentNullException.ThrowIfNull(favoriteMovieIds);

        foreach (var movieId in favoriteMovieIds) {
            AddFavoriteMovie(
                movieId);
        }
    }

    private void SetUserName(string userName) {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username cannot be empty.");

        UserName = userName.Trim();
    }

    private void SetFirstName(string firstName) {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.");

        FirstName = firstName.Trim();
    }

    private void SetLastName(string lastName) {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.");

        LastName = lastName.Trim();
    }

    private void SetEmail(string email) {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.");
        try {
            var addr = new MailAddress(email);
            Email = addr.Address;
        }
        catch {
            throw new ArgumentException("Invalid email.");
        }
    }

    private void SetCreated() {
        CreatedAt = DateTimeOffset.UtcNow;
    }

    private void RefreshUpdatedAt() {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private void SetPhoto(Photo? photo) {
        Photo = photo;
        PhotoId = photo?.Id;
    }
}