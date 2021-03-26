using ASC.Web.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Service
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string name, string subject, string message);
    }

    public class AuthMessageSender : IEmailSender
    {
        private IOptions<ApplicationSettings> _settings;

        public AuthMessageSender(IOptions<ApplicationSettings> settings)
        {
            _settings = settings;
        }

        public async Task SendEmailAsync(string email, string name, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();
            // BOOK: .netcore1.1
            // emailMessage.From.Add(new MailboxAddress(_settings.Value.SMTPAccount));
            // emailMessage.To.Add(new MailboxAddress(email));
            // UPGRADE: .netcore3.1
            emailMessage.From.Add(new MailboxAddress("Automobile Service Center", _settings.Value.SMTPAccount));
            emailMessage.To.Add(new MailboxAddress(name, email));
            //end
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = htmlMessage };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_settings.Value.SMTPServer, _settings.Value.SMTPPort, false);
                await client.AuthenticateAsync(_settings.Value.SMTPAccount, _settings.Value.SMTPPassword);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
