using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Shuttle.Core.Contract;
using Shuttle.Core.Logging;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail
{
    public class DefaultEMailClient : IEMailClient
    {
        private readonly IEMailConfiguration _configuration;
        private readonly IEMailAttachmentService _attachmentService;
        private ILog _log;

        public DefaultEMailClient(IEMailConfiguration configuration, IEMailAttachmentService attachmentService)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(attachmentService, nameof(attachmentService));

            _configuration = configuration;
            _attachmentService = attachmentService;

            _log = Log.For(this);
        }

        public void Send(SendEMailCommand message)
        {
            Guard.AgainstNull(message, nameof(message));
            Guard.AgainstNull(message.FromAddress, nameof(message.FromAddress));

            var sendHtml = !string.IsNullOrWhiteSpace(message.HtmlBody);
            var body = sendHtml ? message.HtmlBody : message.Body;

            if (!sendHtml && body.Contains("\n"))
            {
                body = body.Replace("\r", string.Empty).Replace("\n", "\r\n");
            }

            var mail = new MailMessage
            {
                From = message.FromAddress.GetMailAddress(),
                Subject = message.Subject,
                Body = body,
                IsBodyHtml = sendHtml,
                Priority = message.GetMailPriority()
            };

            foreach (var address in message.ToAddresses)
            {
                mail.To.Add(address.GetMailAddress());
            }

            foreach (var address in message.CCAddresses)
            {
                mail.CC.Add(address.GetMailAddress());
            }

            foreach (var address in message.BccAddresses)
            {
                mail.Bcc.Add(address.GetMailAddress());
            }

            if (!sendHtml && message.HasBodyEncoding())
            {
                mail.BodyEncoding = message.GetBodyEncoding();
            }

            if (sendHtml && message.HasHtmlBodyEncoding())
            {
                mail.BodyEncoding = message.GetHtmlBodyEncoding();
            }

            if (message.HasSubjectEncoding())
            {
                mail.SubjectEncoding = message.GetSubjectEncoding();
            }

            if (message.Attachments.Count > 0)
            {
                foreach (var attachment in message.Attachments)
                {
                    var path = _attachmentService.GetAttachmentPath(message.Id, attachment.Id, attachment.OriginalFileName);

                    if (File.Exists(path))
                    {
                        mail.Attachments.Add(new Attachment(new FileStream(path, FileMode.Open), attachment.OriginalFileName));
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

            mail.Dispose();
        }
    }
}