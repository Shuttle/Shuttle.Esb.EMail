using System;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail
{
    public interface IEMailTracker
    {
        bool HasBeenSent(SendEMailCommand command);
        void Sent(SendEMailCommand command);
        void Expire(TimeSpan duration);
    }
}