using System;
using System.IO;
using System.Threading;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.EMail.Server
{
    public class EMailConfiguration : IEMailConfiguration
    {
        public EMailConfiguration(string host, int port)
        {
            Guard.AgainstNullOrEmptyString(host, nameof(host));

            Host = host;
            Port = port;
        }

        public string Host { get; }
        public int Port { get; }
        public bool UseDefaultCredentials { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Domain { get; private set; }
        public bool EnableSsl { get; private set; }
        public TimeSpan TrackerExpiryInterval { get; private set; } = TimeSpan.FromSeconds(15);
        public TimeSpan TrackerExpiryDuration { get; private set; } = TimeSpan.FromHours(8);

        public void WithCredentials(string username, string password, string domain)
        {
            UseDefaultCredentials = false;
            Username = username;
            Password = password;
            Domain = domain;
        }

        public void UseSsl()
        {
            EnableSsl = true;
        }

        public void WithTrackerConfiguration(TimeSpan trackerExpiryInterval, TimeSpan trackerExpiryDuration)
        {
            TrackerExpiryInterval = trackerExpiryInterval;
            TrackerExpiryDuration = trackerExpiryDuration;
        }
    }
}