using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Database.Services
{
    public sealed class EmailSender
      : IEmailSender
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(
            string smtpHost,
            int smtpPort,
            ILogger<EmailSender> logger
        )
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _logger = logger;
        }

        public Task SendAsync(
            (string name, string address) to,
            string subject,
            string body
        )
        {
            _logger.LogDebug("About to send email to `{To}` with subject `{Subject}` and body `{Body}`", to, subject, body);
            var message = new MimeMessage();
            message.From.Add(
                new MailboxAddress(
                    "Database",
                    "database@solarbuildingenvelopes.com"
                )
            );
            message.To.Add(
                new MailboxAddress(
                    to.name,
                    to.address
                )
            );
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = body
            };
            using (var client = new SmtpClient())
            {
                client.Connect(
                    _smtpHost,
                    _smtpPort,
                    SecureSocketOptions.StartTlsWhenAvailable
                );
                // client.Authenticate("joey", "password");
                client.Send(message);
                client.Disconnect(quit: true);
            }
            return Task.FromResult(0);
        }
    }
}