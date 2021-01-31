using SendGrid.Helpers.Mail;
using Shuttle.Core.Contract;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail.SendGrid
{
    public static class AddressExtensions
    {
        public static EmailAddress GetEmailAddress(this SendEMailCommand.Address address)
        {
            Guard.AgainstNull(address, nameof(address));

            return new EmailAddress(address.EMailAddress, address.DisplayName);
        }
    }
}