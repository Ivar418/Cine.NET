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