using API.Domain.Model;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using DotNetEnv;

namespace API.Services;

public class MailService
{
    private readonly string _serverUrl;
    private readonly string _serverPort;
    private readonly string _senderEmail;
    private readonly string _senderPassword;
    private readonly IEnumerable<EmailSubscription> _receiverEmail;
    

    public MailService()
    {
        Env.Load();
        _serverUrl = Env.GetString("MAIL_SERVER_URL") ?? "";
        _serverPort = Env.GetString("MAIL_SERVER_PORT") ?? "";
        _senderEmail = Env.GetString("MAIL_SENDER_EMAIL") ?? "";
        _senderPassword = Env.GetString("MAIL_SENDER_PASSWORD") ?? "";
        _receiverEmail = new List<EmailSubscription>();
    }

    public void SendEmail()
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Joey Tribbiani", "joey@friends.com"));
        message.To.Add(new MailboxAddress("Mrs. Chanandler Bong", "chandler@friends.com"));
        message.Subject = "How you doin'?";

        message.Body = new TextPart("plain")
        {
            Text = @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey"
        };

        using (var client = new SmtpClient())
        {
            client.Connect(_serverUrl, 587, false);

            // Note: only needed if the SMTP server requires authentication
            client.Authenticate("joey", "password");

            client.Send(message);
            client.Disconnect(true);
        }
    }
}