using System.Security.Cryptography;
using SharedLibrary.Domain.Entities;

namespace API.Domain.Model;

public class RefreshToken {
    public int Id { get; private set; }

    public string Token { get; private set; } = null!;

    public int UserId { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public bool Revoked { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public User User { get; private set; } = null!;


    protected RefreshToken() { }

    public RefreshToken(
        string token,
        DateTimeOffset expiresAt,
        User user) {
        SetToken(token);
        SetExpiry(expiresAt);

        User = user ?? throw new ArgumentNullException(nameof(user));
        UserId = user.Id;

        Revoked = false;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public bool IsExpired() {
        return DateTimeOffset.UtcNow >= ExpiresAt;
    }

    public bool IsActive() {
        return !Revoked && !IsExpired();
    }

    public void Revoke() {
        if (Revoked)
            return;

        Revoked = true;
        RevokedAt = DateTimeOffset.UtcNow;
    }

    public void Rotate(string newToken, DateTimeOffset newExpiry) {
        if (Revoked)
            throw new InvalidOperationException(
                "Cannot rotate a revoked token.");

        SetToken(newToken);
        SetExpiry(newExpiry);
    }

    private void SetToken(string token) {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException(
                "Refresh token cannot be empty.");

        Token = token.Trim();
    }

    private void SetExpiry(DateTimeOffset expiresAt) {
        if (expiresAt <= DateTimeOffset.UtcNow)
            throw new ArgumentException(
                "Expiry must be in the future.");

        ExpiresAt = expiresAt;
    }
}