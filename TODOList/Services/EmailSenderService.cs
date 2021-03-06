using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using TODOList.Configuration;
using TODOList.Models;

namespace TODOList.Services
    {
    public interface IEmailSenderService
        {
        Task SendEmailAsync (Message message);
        }

    public class EmailSenderService : IEmailSenderService
        {
        private readonly EmailConfiguration _emailConfig;
        public EmailSenderService (IOptions<EmailConfiguration> emailConfig)
            {
            _emailConfig = emailConfig.Value;
            }
        public async Task SendEmailAsync (Message message)
            {
            var emailMessage = CreateEmailMessage (message);
            await Send (emailMessage);
            }

        private MimeMessage CreateEmailMessage (Message message)
            {
            var emailMessage = new MimeMessage ();
            emailMessage.From.Add (new MailboxAddress (_emailConfig.From));
            emailMessage.To.AddRange (message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart (MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
            }
        private async Task Send (MimeMessage mailMessage)
            {
            using (var client = new SmtpClient ())
                {
                try
                    {
                    await client.ConnectAsync (_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove ("XOAUTH2");
                    await client.AuthenticateAsync (_emailConfig.UserName, _emailConfig.Password);
                    await client.SendAsync (mailMessage);
                    }
                catch
                    {
                    //log an error message or throw an exception or both.
                    throw;
                    }
                finally
                    {
                    await client.DisconnectAsync (true);
                    client.Dispose ();
                    }
                }
            }
        }
    }
