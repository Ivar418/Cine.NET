using API.Domain.Model;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using DotNetEnv;
using MailKit.Net.Smtp;
using MimeKit;

namespace API.Services.Implementations;

public class LocalMailService : ILocalMailService
{
    private readonly string _serverUrl;
    private readonly int _serverPort;
    private readonly string _senderEmail;
    private readonly string _senderUsername;
    private readonly string _senderPassword;
    private readonly IMailSubscriptionRepository _repository;


    public LocalMailService(IMailSubscriptionRepository repository)
    {
        Env.Load();
        _serverUrl = Env.GetString("MAIL_SERVER_URL") ?? "";
        _serverPort = Env.GetInt("MAIL_SERVER_PORT");
        _senderEmail = Env.GetString("MAIL_SENDER_EMAIL") ?? "";
        _senderUsername = Env.GetString("MAIL_SENDER_USERNAME") ?? "";
        _senderPassword = Env.GetString("MAIL_SENDER_PASSWORD") ?? "";
        _repository = repository;
    }

    /// <summary>
    /// Sends the provided email content to all currently subscribed email addresses.
    /// </summary>
    /// <param name="emailContent">The message body to send.</param>
    /// <param name="fromName">The display name used as sender.</param>
    /// <param name="subject">The email subject line.</param>
    /// <returns>
    /// A task that returns <c>true</c> after iterating through subscribers and attempting delivery.
    /// </returns>
    public async Task<bool> SendEmailToSubscribersAsync(TextPart emailContent, string fromName, string subject)
    {
        var sendToList = await GetAllAsync();
        foreach (var email in sendToList)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, _senderEmail));
            message.To.Add(new MailboxAddress(email.Email, email.Email));
            message.Subject = subject;
            message.Body = emailContent;

            using var client = new SmtpClient();
            client.Connect(_serverUrl, _serverPort, false);

            // Note: only needed if the SMTP server requires authentication
            // client.Authenticate(_senderUsername, _senderPassword);

            client.Send(message);
            client.Disconnect(true);
        }

        return true;
    }

    public async Task<bool> AddAsync(string email)
    {
        return await _repository.AddAsync(email);
    }

    public async Task<bool> RemoveAsync(string email)
    {
        return await _repository.RemoveAsync(email);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _repository.ExistsAsync(email);
    }

    public async Task<IEnumerable<EmailSubscription>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }
}