using System;
using System.Configuration;
using Shuttle.Core.Configuration;

namespace Shuttle.Esb.EMail.Server
{
    public class EMailSection : ConfigurationSection
    {
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host => (string)this["host"];

        [ConfigurationProperty("port", IsRequired = true)]
        public int Port => (int)this["port"];

        [ConfigurationProperty("useDefaultCredentials", IsRequired = false, DefaultValue = true)]
        public bool UseDefaultCredentials => (bool)this["useDefaultCredentials"];

        [ConfigurationProperty("enableSsl", IsRequired = false, DefaultValue = false)]
        public bool EnableSsl => (bool)this["enabledSsl"];

        [ConfigurationProperty("domain", IsRequired = false, DefaultValue = "")]
        public string Domain => (string)this["domain"];

        [ConfigurationProperty("username", IsRequired = false, DefaultValue = "")]
        public string Username => (string)this["username"];

        [ConfigurationProperty("password", IsRequired = false, DefaultValue = "")]
        public string Password => (string)this["password"];

        [ConfigurationProperty("trackerExpiryInterval", IsRequired = false, DefaultValue = "00:00:15")]
        public TimeSpan TrackerExpiryInterval => (TimeSpan)this["trackerExpiryInterval"];

        [ConfigurationProperty("trackerExpiryDuration", IsRequired = false, DefaultValue = "08:00:00")]
        public TimeSpan TrackerExpiryDuration => (TimeSpan)this["trackerExpiryDuration"];

        public static IEMailConfiguration GetConfiguration()
        {
            var section = ConfigurationSectionProvider.Open<EMailSection>("email");

            if (section == null)
            {
                throw new ApplicationException(
                    "Could not get the 'email' section from the application configuration file.");
            }

            var result = new EMailConfiguration(section.Host, section.Port);

            if (!section.UseDefaultCredentials)
            {
                result.WithCredentials(section.Username, section.Password, section.Domain);
            }

            result.WithTrackerConfiguration(section.TrackerExpiryInterval, section.TrackerExpiryDuration);

            return result;
        }
    }
}