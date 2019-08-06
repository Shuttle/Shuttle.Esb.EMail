using Shuttle.Core.Contract;

namespace Shuttle.Esb.EMail.Server
{
    public static class EMailConfigurationExtensions
    {
        public static bool HasCredentials(this IEMailConfiguration configuration)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return !string.IsNullOrEmpty(configuration.Username);
        }

        public static bool HasDomain(this IEMailConfiguration configuration)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return !string.IsNullOrEmpty(configuration.Domain);
        }
    }
}