using System.Text.Json.Serialization;
using SharedLibrary.Domain.Entities;

namespace API.Domain.Model;

public class UserCredential {
    public int Id { get; private set; }
    public int UserId { get; private set; }
    [JsonIgnore] public string PasswordHash { get; private set; }

    public User User { get; private set; } = null!;

    protected UserCredential() { }

    public UserCredential(int userId, string passwordHash) {
        UserId = userId;
        PasswordHash = passwordHash;
    }
}