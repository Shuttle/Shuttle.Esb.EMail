using System.Collections.Generic;
using System.Net.Mail;

namespace Shuttle.EMail.Messages
{
	public class SendEMailCommand 
	{
		public SendEMailCommand()
		{
			IsBodyHtml = false;
			Priority = MailPriority.Normal.ToString();
			Attachments = new List<string>();
		}

		public string From { get; set; }
		public string To { get; set; }
		public string CC { get; set; }
		public string Bcc { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public bool IsBodyHtml { get; set; }
		public string BodyEncoding { get; set; }
		public string SubjectEncoding { get; set; }
		public string Priority { get; set; }

		public List<string> Attachments { get; set; }
	}
}