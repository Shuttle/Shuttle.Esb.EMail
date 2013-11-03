using System.IO;
using System.Net.Mail;
using System.Text;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.EMail.Messages;

namespace Shuttle.EMail.Server
{
	public class SendEMailHandler : IMessageHandler<SendEMailCommand>
	{
		private readonly IEMailGateway gateway;
		private readonly IEMailConfiguration configuration;
		private readonly ILog log;

		public SendEMailHandler(IEMailGateway gateway, IEMailConfiguration configuration)
		{
			Guard.AgainstNull(gateway, "gateway");
			Guard.AgainstNull(configuration, "configuration");

			this.gateway = gateway;
			this.configuration = configuration;

			log = Log.For(this);
		}

		public void ProcessMessage(HandlerContext<SendEMailCommand> context)
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
					var path = Path.Combine(configuration.AttachmentFolder, file);

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

			log.Trace(string.Format("[sending mail] : from = {0} / to = {1} / subject = {2} / attachments = {3}", message.From, message.To, message.Subject, attachments));

			gateway.SendMail(mail);

			foreach (var file in message.Attachments)
			{
				context.Bus.SendLocal(new RemoveAttachmentCommand
										{
											File = file
										});
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}