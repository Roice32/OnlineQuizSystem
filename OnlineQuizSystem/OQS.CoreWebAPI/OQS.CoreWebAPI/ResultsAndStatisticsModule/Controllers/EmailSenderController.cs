using Microsoft.AspNetCore.Mvc;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Features;
using System;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailSenderController : ControllerBase
    {
        private readonly EmailSender.Handler _emailSender;

        public EmailSenderController(EmailSender.Handler emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmailAsync(string recipientEmail, string recipientFirstName, string Link)
        {
            try
            {
                var command = new EmailSender.Command
                {
                    RecipientEmail = recipientEmail,
                    RecipientFirstName = recipientFirstName,
                };

                var result = await _emailSender.Handle(command, default);
                if (result.IsSuccess)
                {
                    return Ok(result.Value.Message);
                }
                else
                {
                    return BadRequest(result.Error);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
