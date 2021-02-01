using System.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using Shuttle.Core.Contract;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail.SendGrid
{
    public class EMailClient : IEMailClient
    {
        private readonly string _apiKey;

        public EMailClient(IEMailConfiguration configuration)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            _apiKey = configuration.ApiKey;

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new ConfigurationErrorsException(Resources.MissingApiKeyException);
            }
        }

        public void Send(SendEMailCommand message)
        {
            var mail = new SendGridMessage
            {
                Subject = message.Subject,
                HtmlContent = message.HtmlBody,
                PlainTextContent = message.Body,
                From = message.FromAddress.GetEmailAddress()
            };

            foreach (var address in message.ToAddresses)
            {
                mail.AddTo(address.GetEmailAddress());
            }

            foreach (var address in message.CCAddresses)
            {
                mail.AddCc(address.GetEmailAddress());
            }

            foreach (var address in message.BccAddresses)
            {
                mail.AddBcc(address.GetEmailAddress());
            }
            
            new SendGridClient(_apiKey).SendEmailAsync(mail).Wait();
        }
    }
}
