using SendGrid;
using SendGrid.Helpers.Mail;
using Shuttle.Core.Configuration;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail.SendGrid
{
    public class EMailClient : IEMailClient
    {
        private readonly string _apiKey = ConfigurationItem<string>.ReadSetting("SendGridApiKey").GetValue();

        public void Send(SendEMailCommand message)
        {
            new SendGridClient(_apiKey).SendEmailAsync(MailHelper.CreateSingleEmail(
                new EmailAddress(message.SenderEMailAddress, message.GetSenderDisplayNameOptional()),
                new EmailAddress(message.RecipientEMailAddress, message.GetRecipientDisplayNameOptional()),
                message.Subject, message.Body, message.HtmlBody)).Wait();
        }
    }
}
