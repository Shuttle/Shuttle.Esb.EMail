using System;
using System.IO;
using Shuttle.Core.Configuration;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail
{
    public class DefaultEMailAttachmentService : IEMailAttachmentService
    {
        private readonly string _attachmentFolder;

        public DefaultEMailAttachmentService()
        {
            _attachmentFolder = ConfigurationItem<string>.ReadSetting("AttachmentFolder", string.Empty).GetValue();

            if (!HasAttachmentFolder || Directory.Exists(_attachmentFolder))
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(_attachmentFolder);
            }
            catch (Exception ex)
            {
                throw new DirectoryNotFoundException(
                    $"Could not find attachment folder '{_attachmentFolder}' and could not create it either (see inner exception).",
                    ex);
            }
        }

        public bool HasAttachmentFolder => !string.IsNullOrEmpty(_attachmentFolder);

        public SendEMailCommand.Attachment AddAttachment(SendEMailCommand command, string path)
        {
            Guard.AgainstNull(command, nameof(command));
            Guard.AgainstNullOrEmptyString(path, nameof(path));

            if (!HasAttachmentFolder)
            {
                throw new InvalidOperationException(Resources.AttachmentsNotSupportedException);
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(string.Format(Resources.AttachmentFileNotFound, path));
            }

            var originalFileName = Path.GetFileName(path);
            var result = new SendEMailCommand.Attachment
            {
                Id = Guid.NewGuid(),
                OriginalFileName = originalFileName
            };

            var targetPath = GetAttachmentPath(command.Id, result.Id, originalFileName);

            try
            {
                File.Copy(path, targetPath, true);
            }
            catch (Exception ex)
            {
                throw new EMailException(string.Format(Resources.AttachmentFileException, path, ex.AllMessages()));
            }

            command.Attachments.Add(result);

            return result;
        }

        public void RemoveAttachment(Guid emailId, Guid attachmentId, string originalFileName)
        {
            if (!HasAttachmentFolder)
            {
                return;
            }

            var path = GetAttachmentPath(emailId, attachmentId, originalFileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public string GetAttachmentPath(Guid emailId, Guid attachmentId, string originalFileName)
        {
            if (!HasAttachmentFolder)
            {
                throw new InvalidOperationException(Resources.AttachmentsNotSupportedException);
            }

            return Path.Combine(_attachmentFolder, GetAttachmentFileName(emailId, attachmentId, originalFileName));
        }

        public string GetAttachmentFileName(Guid emailId, Guid attachmentId, string originalFileName)
        {
            return $"{emailId:n}#{attachmentId:n}-{originalFileName}";
        }
    }
}