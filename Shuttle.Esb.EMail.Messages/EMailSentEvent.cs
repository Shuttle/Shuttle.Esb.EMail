using System;

namespace Shuttle.Esb.EMail.Messages
{
    public class EMailSentEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}