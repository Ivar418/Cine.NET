using API.Services.Interfaces;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SharedLibrary.DTOs.Requests;

namespace API.Controllers;

[ApiController]
[Route("api/mail/subscription")]
public class MailSubscriptionController : ControllerBase
{
    private readonly ILocalMailService _mailService;

    public MailSubscriptionController(ILocalMailService mailService)
    {
        _mailService = mailService;
    }

    /// <summary>
    /// Subscribes an email address to the mailing list.
    /// </summary>
    /// <param name="email">The email address to subscribe.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating whether the subscription succeeded,
    /// failed validation, or failed due to a server error.
    /// </returns>
    [Route("subscribe")]
    [HttpPost]
    public async Task<IActionResult> Subscribe([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required.");
        try
        {
            if (await _mailService.ExistsAsync(email))
            {
                return BadRequest("Email is already subscribed.");
            }

            await _mailService.AddAsync(email);
            return Ok("Subscription successful.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    /// <summary>
    /// Unsubscribes an email address from the mailing list.
    /// </summary>
    /// <param name="email">The email address to unsubscribe.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating whether the unsubscription succeeded,
    /// failed validation, or failed due to a server error.
    /// </returns>
    [Route("unsubscribe")]
    [HttpDelete]
    public async Task<IActionResult> Unsubscribe([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required.");

        try
        {
            var removed = await _mailService.RemoveAsync(email);
            if (!removed) return BadRequest("Email is not subscribed.");

            return Ok("Unsubscription successful.");
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    /// <summary>
    /// Sends an email message to all subscribed recipients.
    /// </summary>
    /// <param name="request">The email payload, including sender name, subject, and content.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating whether the email dispatch completed successfully
    /// or failed due to a server error.
    /// </returns>
    [Route("send")]
    [HttpPost]
    public async Task<IActionResult> SendEmailToSubscribers([FromBody] EmailRequest request)
    {
        try
        {
            var textPart = new TextPart("plain")
            {
                Text = request.EmailContent
            };

            await _mailService.SendEmailToSubscribersAsync(textPart, request.FromName, request.Subject);
            return Ok("Emails sent successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}