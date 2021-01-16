using System;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.EMail
{
    public class EMailConfiguration : IEMailConfiguration
    {
        public EMailConfiguration WithHost(string host, int port)
        {
            Guard.AgainstNullOrEmptyString(host, nameof(host));

            Host = host;
            Port = port;

            return this;
        }

        public string EMailClientType { get; private set; }
        public string ApiKey { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool UseDefaultCredentials { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Domain { get; private set; }
        public bool EnableSsl { get; private set; }
        public TimeSpan TrackerExpiryInterval { get; private set; } = TimeSpan.FromSeconds(15);
        public TimeSpan TrackerExpiryDuration { get; private set; } = TimeSpan.FromHours(8);

        public EMailConfiguration WithCredentials(string username, string password, string domain)
        {
            UseDefaultCredentials = false;
            Username = username;
            Password = password;
            Domain = domain;

            return this;
        }

        public EMailConfiguration UseSsl()
        {
            EnableSsl = true;

            return this;
        }

        public EMailConfiguration WithTrackerConfiguration(TimeSpan trackerExpiryInterval, TimeSpan trackerExpiryDuration)
        {
            TrackerExpiryInterval = trackerExpiryInterval;
            TrackerExpiryDuration = trackerExpiryDuration;

            return this;
        }

        public EMailConfiguration WithEMailClientType(string emailClientType)
        {
            Guard.AgainstNullOrEmptyString(emailClientType, nameof(emailClientType));
            
            EMailClientType = emailClientType;
            
            return this;
        }
        public EMailConfiguration WithApiKey(string apiKey)
        {
            Guard.AgainstNullOrEmptyString(apiKey, nameof(apiKey));

            ApiKey = apiKey;
            
            return this;
        }
    }
}