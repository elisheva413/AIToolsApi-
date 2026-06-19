using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using DTOs;
using Service;

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-giftcard")]
        [Authorize]
        public async Task<IActionResult> SendGiftCardEmail([FromBody] GiftCardEmailRequest request)
        {
            try
            {
                await _emailService.SendGiftCardEmailAsync(request);
                return Ok(new { message = "המייל נשלח בהצלחה!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "שגיאה בשליחת המייל", error = ex.Message });
            }
        }
    }
}