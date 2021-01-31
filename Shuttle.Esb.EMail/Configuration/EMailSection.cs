using System;
using System.Configuration;
using Shuttle.Core.Configuration;

namespace Shuttle.Esb.EMail
{
    public class EMailSection : ConfigurationSection
    {
        [ConfigurationProperty("emailClientType", IsRequired = false)]
        public string EMailClientType => (string)this["emailClientType"];

        [ConfigurationProperty("apiKey", IsRequired = false)]
        public string ApiKey => (string)this["apiKey"];

        [ConfigurationProperty("host", IsRequired = false)]
        public string Host => (string)this["host"];

        [ConfigurationProperty("port", IsRequired = false, DefaultValue = 25)]
        public int Port => (int)this["port"];

        [ConfigurationProperty("useDefaultCredentials", IsRequired = false, DefaultValue = true)]
        public bool UseDefaultCredentials => (bool)this["useDefaultCredentials"];

        [ConfigurationProperty("enableSsl", IsRequired = false, DefaultValue = false)]
        public bool EnableSsl => (bool)this["enableSsl"];

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

            var result = new EMailConfiguration();

            if (!string.IsNullOrWhiteSpace(section.EMailClientType))
            {
                result.WithEMailClientType(section.EMailClientType);
            }

            if (!string.IsNullOrWhiteSpace(section.Host))
            {
                result.WithHost(section.Host, section.Port);
            }

            if (section.EnableSsl)
            {
                result.UseSsl();
            }
            
            if (!section.UseDefaultCredentials)
            {
                result.WithCredentials(section.Username, section.Password, section.Domain);
            }

            result.WithTrackerConfiguration(section.TrackerExpiryInterval, section.TrackerExpiryDuration);

            return result;
        }
    }
}