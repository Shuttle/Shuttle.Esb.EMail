using System;
using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Contract;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail
{
    public class DefaultEMailTracker : IEMailTracker
    {
        private readonly object _lock = new object();
        private readonly Dictionary<Guid, DateTime> _sent = new Dictionary<Guid, DateTime>();

        public bool HasBeenSent(SendEMailCommand command)
        {
            Guard.AgainstNull(command, nameof(command));

            lock (_lock)
            {
                return _sent.ContainsKey(command.Id);
            }
        }

        public void Sent(SendEMailCommand command)
        {
            Guard.AgainstNull(command, nameof(command));

            lock (_lock)
            {
                if (_sent.ContainsKey(command.Id))
                {
                    return;
                }

                _sent.Add(command.Id, DateTime.Now);
            }
        }

        public void Expire(TimeSpan duration)
        {
            lock (_lock)
            {
                var now = DateTime.Now;

                foreach (var key in _sent.Where(entry => (now - entry.Value) > duration).Select(entry => entry.Key).ToList())
                {
                    _sent.Remove(key);
                }
            }
        }
    }
}