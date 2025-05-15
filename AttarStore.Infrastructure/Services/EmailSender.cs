using AttarStore.Application.Settings;
using Castle.Core.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _smtp;

        public EmailSender(IOptions<EmailSettings> options)
        {
            _smtp = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // 1) Guard against null/empty
            if (string.IsNullOrWhiteSpace(email)) return;

            // 2) Validate format
            if (!System.Net.Mail.MailAddress.TryCreate(email, out var toAddress))
                return;

            using var client = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
                EnableSsl = _smtp.EnableSsl
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_smtp.FromEmail, _smtp.FromName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mail.To.Add(toAddress);

            await client.SendMailAsync(mail);
        }
    }
}
