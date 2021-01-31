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
