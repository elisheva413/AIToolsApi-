using DTOs;

namespace Service
{
    public interface IEmailService
    {
        Task SendGiftCardEmailAsync(GiftCardEmailRequest request);
    }
}