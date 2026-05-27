using System.Net.Mail;

namespace SharedLibrary.Domain.Entities;

public class User {
    public int Id { get; private set; } // EF Core can set this

    public string UserName { get; private set; }

    public string? PhotoId { get; private set; }
    public Photo? Photo { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }


    public User(
        string userName,
        string firstName,
        string lastName,
        string email) {
        SetUserName(userName);
        SetFirstName(firstName);
        SetLastName(lastName);
        SetEmail(email);
        CreatedAt = DateTimeOffset.UtcNow;
    }

    protected User() { }

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

    private void SetPhoto(Photo? photo) {
        PhotoId = photo.Id;
        Photo = photo;
    }
}