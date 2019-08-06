using System;

namespace Shuttle.Esb.EMail.Server
{
	public interface IEMailConfiguration
	{
		string Host { get; }
        int Port { get; }
        bool UseDefaultCredentials { get; }
        string Username { get; }
        string Password { get; }
        string Domain { get; }
        bool EnableSsl { get; }
        TimeSpan TrackerExpiryInterval { get; }
        TimeSpan TrackerExpiryDuration { get; }
    }
}