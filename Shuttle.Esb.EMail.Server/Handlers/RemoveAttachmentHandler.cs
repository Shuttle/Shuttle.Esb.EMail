using System.IO;
using Shuttle.Core.Contract;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail.Server.Handlers
{
	public class RemoveAttachmentHandler : IMessageHandler<RemoveAttachmentCommand>
	{
        private readonly IEMailAttachmentService _attachmentService;

        public RemoveAttachmentHandler(IEMailAttachmentService attachmentService)
		{
            Guard.AgainstNull(attachmentService, nameof(attachmentService));

            _attachmentService = attachmentService;
		}

        public void ProcessMessage(IHandlerContext<RemoveAttachmentCommand> context)
		{
            Guard.AgainstNull(context,nameof(context));

            var message = context.Message;

            _attachmentService.RemoveAttachment(message.EMailId, message.AttachmentId, message.OriginalFileName);
		}
	}
}