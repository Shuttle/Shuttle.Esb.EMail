using System.Net.Mail;
using System.Text;
using Shuttle.Core.Contract;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail
{
    public static class AddressExtensions
    {
        public static MailAddress GetMailAddress(this SendEMailCommand.Address address)
        {
            Guard.AgainstNull(address, nameof(address));

            return new MailAddress(address.EMailAddress, address.GetDisplayNameOptional(),
                address.GetDisplayNameEncoding() ?? Encoding.ASCII);
        }
    }
}