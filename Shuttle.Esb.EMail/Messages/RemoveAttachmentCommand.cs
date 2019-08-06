using System;

namespace Shuttle.Esb.EMail.Messages
{
	public class RemoveAttachmentCommand
	{
        public Guid EMailId { get; set; }
        public Guid AttachmentId { get; set; }
		public string OriginalFileName { get; set; }
	}
}