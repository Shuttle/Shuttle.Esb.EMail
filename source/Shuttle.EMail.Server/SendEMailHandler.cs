using System.IO;
using System.Net.Mail;
using System.Text;
using Shuttle.Core.Infrastructure;
using Shuttle.EMail.Messages;
using Shuttle.ESB.Core;

namespace Shuttle.EMail.Server
{
	public class SendEMailHandler : IMessageHandler<SendEMailCommand>
	{
		private readonly IEMailConfiguration _configuration;
		private readonly IEMailService _service;
		private readonly ILog _log;

		public SendEMailHandler(IEMailService service, IEMailConfiguration configuration)
		{
			Guard.AgainstNull(service, "service");
			Guard.AgainstNull(configuration, "configuration");

			_service = service;
			_configuration = configuration;

			_log = Log.For(this);
		}

		public void ProcessMessage(IHandlerContext<SendEMailCommand> context)
		{
			var message = context.Message;

			var body = message.Body;

			if (!message.IsBodyHtml && body.Contains("\n"))
			{
				body = body.Replace("\r", "").Replace("\n", "\r\n");
			}

			var mail = new MailMessage(message.From, message.To.Replace(';', ','), message.Subject, body)
			{
				IsBodyHtml = message.IsBodyHtml,
				Priority = message.GetMailPriority()
			};

			if (message.HasBodyEncoding())
			{
				mail.BodyEncoding = message.GetBodyEncoding();
			}

			if (message.HasSubjectEncoding())
			{
				mail.SubjectEncoding = message.GetSubjectEncoding();
			}

			if (!string.IsNullOrEmpty(message.CC))
			{
				mail.CC.Add(message.CC.Replace(';', ','));
			}

			if (!string.IsNullOrEmpty(message.Bcc))
			{
				mail.Bcc.Add(message.Bcc.Replace(';', ','));
			}

			var attachments = "(none)";

			if (message.Attachments.Count > 0)
			{
				var attachmentsBuilder = new StringBuilder();

				foreach (var file in message.Attachments)
				{
					var path = Path.Combine(_configuration.AttachmentFolder, file);

					if (!File.Exists(path))
					{
						throw new UnrecoverableHandlerException(string.Format("Cannot find attachment '{0}'.  The e-mail cannot be sent.",
							path));
					}

					mail.Attachments.Add(new Attachment(new FileStream(path, FileMode.Open), file));

					attachmentsBuilder.AppendFormat("{0}{1}", attachmentsBuilder.Length > 0 ? ";" : string.Empty, file);
				}

				attachments = attachmentsBuilder.ToString();
			}

			_log.Trace(string.Format("[sending mail] : from = {0} / to = {1} / subject = {2} / attachments = {3}", message.From,
				message.To, message.Subject, attachments));

			using (var client = new SmtpClient())
			{
				client.Send(mail);
			}

			foreach (var file in message.Attachments)
			{
				context.Send(new RemoveAttachmentCommand
				{
					File = file
				}, c => c.Local());
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}