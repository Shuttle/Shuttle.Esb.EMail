using System;
using System.IO;
using Shuttle.Core.Contract;
using Shuttle.Core.Logging;
using Shuttle.Core.Reflection;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail
{
    public class EMailAttachmentService : IEMailAttachmentService
    {
        private readonly IEMailConfiguration _configuration;
        private readonly ILog _log;

        public EMailAttachmentService(IEMailConfiguration configuration)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            _configuration = configuration;

            _log = Log.For(this);
        }

        public string AddAttachment(SendEMailCommand command, string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(string.Format(Resources.AttachmentFileNotFound, path));
            }

            var name = Guid.NewGuid().ToString("n");
            var file = $"{name}{Path.GetExtension(path)}";
            var targetPath = Path.Combine(_configuration.AttachmentFolder, file);

            try
            {
                File.Copy(path, targetPath, true);
            }
            catch (Exception ex)
            {
                throw new EMailException(string.Format(Resources.AttachmentFileException, path, ex.AllMessages()));
            }

            command.Attachments.Add(file);

            return file;
        }
    }
}