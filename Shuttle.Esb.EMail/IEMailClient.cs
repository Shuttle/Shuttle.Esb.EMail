using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail
{
    public interface IEMailClient
    {
        void Send(SendEMailCommand message);
    }
}