using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace PCAssembly.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Здесь реализуйте логику отправки email
            // Например, можно использовать SMTP, MailKit или сторонние сервисы (SendGrid, Amazon SES и т.д.)
            Console.WriteLine($"Email: {email}\nSubject: {subject}\nMessage: {htmlMessage}");
            return Task.CompletedTask; // Пока возвращаем задачу-заглушку
        }
    }
}
