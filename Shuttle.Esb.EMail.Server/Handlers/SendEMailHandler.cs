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
		private readonly IEMailConfiguration _configuration;
        private readonly IEMailAttachmentService _attachmentService;
        private readonly IEMailTracker _tracker;
        private readonly ILog _log;

		public SendEMailHandler(IEMailConfiguration configuration, IEMailAttachmentService attachmentService, IEMailTracker tracker)
		{
			Guard.AgainstNull(configuration, nameof(configuration));
			Guard.AgainstNull(attachmentService, nameof(attachmentService));
			Guard.AgainstNull(tracker, nameof(tracker));

			_configuration = configuration;
            _attachmentService = attachmentService;
            _tracker = tracker;

            _log = Log.For(this);
		}

		public void ProcessMessage(IHandlerContext<SendEMailCommand> context)
		{
            Guard.AgainstNull(context, nameof(context));

            var message = context.Message;

            if (!_tracker.HasBeenSent(message))
            {
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

                    foreach (var attachment in message.Attachments)
                    {
                        var path = _attachmentService.GetAttachmentPath(message.Id, attachment.Id, attachment.OriginalFileName);

                        if (File.Exists(path))
                        {
                            mail.Attachments.Add(new Attachment(new FileStream(path, FileMode.Open), attachment.OriginalFileName));

                            attachmentsBuilder.AppendFormat("{0}{1}", attachmentsBuilder.Length > 0 ? ";" : string.Empty, attachment);
                        }
                        else
                        {
                            var stream = new MemoryStream();
                            var writer = new StreamWriter(stream);

                            writer.WriteLine(Resources.MissingAttachmentMessage, attachment.OriginalFileName);
                            writer.WriteLine();
                            writer.WriteLine(Resources.MissingAttachmentApology);
                            writer.Flush();

                            stream.Position = 0;     

                            mail.Attachments.Add(new Attachment(stream, $"({Resources.Missing})-{attachment.OriginalFileName}.txt", MediaTypeNames.Text.Plain));

                            _log.Warning($"[attachment not found] : id = '{message.Id}' / attachment id = '{attachment.Id}' / original file name = '{attachment.OriginalFileName}'");
                        }
                    }

                    attachments = attachmentsBuilder.ToString();
                }

                if (Log.IsDebugEnabled)
                {
                    _log.Debug(
                        $"[sending mail] : id = '{message.Id}' / from = {message.From} / to = {message.To} / subject = {message.Subject} / attachments = {attachments}");
                }

                using (var client = new SmtpClient(_configuration.Host, _configuration.Port)
                {
                    UseDefaultCredentials = _configuration.UseDefaultCredentials,
                    EnableSsl = _configuration.EnableSsl
                })
                {
                    if (_configuration.HasCredentials())
                    {
                        client.Credentials = _configuration.HasDomain()
                            ? new NetworkCredential(_configuration.Username, _configuration.Password,
                                _configuration.Domain)
                            : new NetworkCredential(_configuration.Username, _configuration.Password);
                    }

                    client.Send(mail);
                }

                _tracker.Sent(message);

                mail.Dispose();

                if (Log.IsDebugEnabled)
                {
                    _log.Debug($"[sent] : id = '{message.Id}'");
                }
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