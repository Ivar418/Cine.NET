using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs.Requests;

public class EmailRequest
{
    [JsonPropertyName("subject")]
    public string Subject { get; set; }
    [JsonPropertyName("fromName")]
    public string FromName { get; set; }
    [JsonPropertyName("emailContent")]
    public string EmailContent { get; set; }
}