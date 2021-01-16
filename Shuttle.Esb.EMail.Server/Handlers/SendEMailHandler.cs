using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Shuttle.Core.Contract;
using Shuttle.Core.Logging;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail.Server.Handlers
{
	public class SendEMailHandler : IMessageHandler<SendEMailCommand>
	{
        private readonly IEMailClient _client;
        private readonly IEMailTracker _tracker;
        private readonly ILog _log;

		public SendEMailHandler( IEMailClient client, IEMailTracker tracker)
		{
			Guard.AgainstNull(client, nameof(client));
			Guard.AgainstNull(tracker, nameof(tracker));

            _client = client;
            _tracker = tracker;

            _log = Log.For(this);
		}

		public void ProcessMessage(IHandlerContext<SendEMailCommand> context)
		{
            Guard.AgainstNull(context, nameof(context));

            var message = context.Message;

            if (!_tracker.HasBeenSent(message))
            {
                if (Log.IsDebugEnabled)
                {
                    var attachments = new StringBuilder();

                    if (message.Attachments.Count > 0)
                    {
                        foreach (var attachment in message.Attachments)
                        {
                            attachments.AppendFormat("{0}{1}", attachments.Length > 0 ? ";" : string.Empty, attachment);

                        }
                    }
                    else
                    {
                        attachments.Append("(none)");
                    }

                    _log.Debug(
                        $"[sending mail] : id = '{message.Id}' / sender = {message.SenderEMailAddress}{(string.IsNullOrWhiteSpace(message.SenderEMailAddress) ? string.Empty : $" ({message.SenderEMailAddress})")}) / recipient = {message.RecipientEMailAddress}{(string.IsNullOrWhiteSpace(message.RecipientEMailAddress) ? string.Empty : $" ({message.RecipientEMailAddress})")}) / subject = {message.Subject} / attachments = {attachments}");
                }

                _client.Send(message);

                if (Log.IsDebugEnabled)
                {
                    _log.Debug($"[sent] : id = '{message.Id}'");
                }
                
                _tracker.Sent(message);
            }
            else
            {
                if (Log.IsDebugEnabled)
                {
                    _log.Debug($"[already sent] : id = '{message.Id}'");
                }
            }

            foreach (var attachment in message.Attachments)
			{
				context.Send(new RemoveAttachmentCommand
				{
                    EMailId = message.Id,
                    AttachmentId= attachment.Id,
					OriginalFileName = attachment.OriginalFileName
                }, c => c.Local());
			}

            if (message.Reply && context.TransportMessage.HasSenderInboxWorkQueueUri())
            {
                context.Send(new EMailSentEvent
                {
                    Id = message.Id
                }, c => c.Reply());
            }
        }
	}
}