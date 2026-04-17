using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Service
{
    public class EmailService : IEmailService
    {
        public async Task SendGiftCardEmailAsync(GiftCardEmailRequest request)
        {
            var fromAddress = new MailAddress("eli7p654321@gmail.com", "Pandora Gift-Card");
            var toAddress = new MailAddress(request.RecipientEmail, request.RecipientName);

            const string fromPassword = "XXXXX";
           

            string subject = $"הפתעה! מתנה מ{request.SenderName} מחכה לך בפנדורה 🎁";

            string body = $@"
<div style='background-color: #f9f9f9; padding: 40px 0; font-family: Assistant, ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; direction: rtl;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.05); border: 1px solid #f0f0f0; text-align: center;'>
        
        <div style='background-color: #ffffff; padding: 30px 0; text-align: center;'>
            <img src='https://i.postimg.cc/bNGHmmfn/LOgo.webp' 
                 alt='Pandora Logo' style='width: 180px; height: auto; display: inline-block;'>
        </div>

        <div style='padding: 0 40px 30px 40px; text-align: center;'>
            <h1 style='color: #000000; font-size: 28px; font-weight: 300; margin-bottom: 20px;'>יש לך מתנה חדשה! 🎁</h1>
            
            <p style='font-size: 18px; color: #555; line-height: 1.6;'>
                היי {request.RecipientName}, <br>
                איזה כיף! <strong>{request.SenderName}</strong> בחר/ה להפתיע אותך עם כרטיס מתנה דיגיטלי לפנדורה.
            </p>

            <div style='background-color: #fff5f7; border: 1px dashed #ffb6c1; border-radius: 12px; padding: 30px; margin: 30px 0;'>
                <span style='display: block; font-size: 14px; text-transform: uppercase; letter-spacing: 1px; color: #ffb6c1; margin-bottom: 10px;'>סכום המתנה</span>
                <span style='display: block; font-size: 48px; font-weight: bold; color: #000000;'>₪{request.Amount}</span>
                
                <div style='margin-top: 20px; padding-top: 20px; border-top: 1px solid #ffdae1;'>
                    <p style='font-size: 20px; font-style: italic; color: #333; margin: 0;'>""{request.Greeting}""</p>
                </div>
            </div>

            <p style='font-size: 16px; color: #888; margin-bottom: 30px;'>
                הכרטיס ממתין לך באתר. אפשר להתחיל להתחדש בצ'ארם הבא שלך כבר עכשיו!
            </p>

            <a href='http://localhost:4200' style='background-color: #000000; color: #ffffff; text-decoration: none; padding: 15px 35px; border-radius: 0; font-weight: bold; display: inline-block; font-size: 16px; margin-bottom: 20px;'>
                לכניסה לאתר
            </a>

            <div style='background-color: #fcf8e3; color: #8a6d3b; border: 1px solid #faebcc; border-radius: 8px; padding: 15px; text-align: right; margin: 20px 0;'>
                <p style='margin: 0; font-size: 14px; line-height: 1.6;'>
                    💡 <strong>הערת מערכת:</strong> מאחר והאתר שלנו נמצא כרגע בסביבת פיתוח, הכפתור למעלה יעבוד רק במחשב שבו נבנה הפרויקט. כדי לממש את המתנה באמת - תצטרכי לבוא למחשב של אלישבע וזהבי. 😉<br><br>
                    בינתיים, את יכולה ללכת לבזבז כסף אמיתי ב<a href='https://www.pandora-shop.co.il/' target='_blank' style='color: #8a6d3b; font-weight: bold; text-decoration: underline;'>אתר הרשמי של פנדורה</a> ולראות בעצמך איך האתר שלהם פחות טוב <a href='https://www.jumbomail.me/j/vhQdUi7YpU2bzhQ' target='_blank' style='color: #8a6d3b; font-weight: bold; text-decoration: underline;'>משלנו</a>... 😜
                </p>
            </div>
        </div>

        <div style='padding: 20px; text-align: center; color: #a9a9a9; border-top: 1px solid #f0f0f0;'>
            <p style='font-size: 13px; margin: 0;'>
                Pandora Israel | Developed by Elisheva Polsky & Zehavi Kornvasser 2026 ©
            </p>
        </div>
    </div>
</div>";

            using var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}
