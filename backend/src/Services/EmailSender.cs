using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Database.Services;

public static partial class Log
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Debug,
        Message = "About to send email to `{Recipient}` with subject `{Subject}` and body `{Body}`")]
    public static partial void AboutToSendEmail(
        this ILogger logger,
        (string name, string address) Recipient,
        string Subject,
        string Body
    );
}

public sealed class EmailSender
    : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;

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
        (string name, string address) recipient,
        string subject,
        string body
    )
    {
        _logger.AboutToSendEmail(recipient, subject, body);
        var message = new MimeMessage();
        message.From.Add(
            new MailboxAddress(
                "Database",
                "database@solarbuildingenvelopes.com"
            )
        );
        message.To.Add(
            new MailboxAddress(
                recipient.name,
                recipient.address
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
            client.Disconnect(true);
        }

        return Task.FromResult(0);
    }
}