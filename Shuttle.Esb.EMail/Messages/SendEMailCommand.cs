using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Shuttle.Esb.EMail.Messages
{
	public class SendEMailCommand
	{
        public class Attachment
        {
            public Guid Id { get; set; }
            public string OriginalFileName { get; set; }
        }

        public Guid Id { get; set; } = Guid.NewGuid();
		public string SenderEMailAddress { get; set; }
		public string SenderDisplayName { get; set; }
		public string RecipientEMailAddress { get; set; }
		public string RecipientDisplayName { get; set; }
		public string CC { get; set; }
		public string Bcc { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public string BodyEncoding { get; set; }
		public string HtmlBody { get; set; }
		public string HtmlBodyEncoding { get; set; }
		public string SubjectEncoding { get; set; }
		public string Priority { get; set; } = MailPriority.Normal.ToString();
        public bool Reply { get; set; }

        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}