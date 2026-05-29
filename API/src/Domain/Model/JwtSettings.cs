using System.Text;

namespace API.Domain.Model;

public class JwtSettings {
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }

    public void Validate() {
        if (string.IsNullOrWhiteSpace(Key))
            throw new InvalidOperationException(
                "JwtSettings:Key is missing."
            );

        if (Encoding.UTF8.GetByteCount(Key) < 32)
            throw new InvalidOperationException(
                "JwtSettings:Key must be at least 32 bytes long."
            );

        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException(
                "JwtSettings:Issuer is missing."
            );

        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException(
                "JwtSettings:Audience is missing."
            );
    }
}