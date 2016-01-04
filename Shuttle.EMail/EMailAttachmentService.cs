using System;
using System.IO;
using System.Text;
using Shuttle.Core.Infrastructure;

namespace Shuttle.EMail
{
	public class EMailAttachmentService : IEMailAttachmentService
	{
		private readonly IEMailConfiguration _configuration;
		private readonly ILog _log;

		public EMailAttachmentService(IEMailConfiguration configuration)
		{
			Guard.AgainstNull(configuration, "configuration");

			_configuration = configuration;

			_log = Log.For(this);
		}

		public string AddAttachment(SendEMailCommand command, string path)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException(string.Format(EMailResources.AttachmentFileNotFound, path));
			}

			var name = Guid.NewGuid().ToString("n");
			var file = string.Format("{0}{1}", name, Path.GetExtension(path));
			var targetPath = Path.Combine(_configuration.AttachmentFolder, file);

			try
			{
				File.Copy(path, targetPath, true);
			}
			catch (Exception ex)
			{
				throw new EMailException(string.Format(EMailResources.AttachmentFileException, path, ex.AllMessages()));
			}

			if (Log.IsTraceEnabled)
			{
				var log = new StringBuilder();

				log.AppendFormat("Date created\t{0}", DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"));
				log.AppendLine();
				log.AppendFormat("Source file path:\t{0}", path);
				log.AppendLine();
				log.AppendFormat("Source machine\t{0}", Environment.MachineName);
				log.AppendLine();
				log.AppendFormat("Source identity\t{0}", string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName));
				log.AppendLine();

				var logPath = Path.Combine(_configuration.AttachmentFolder, string.Format("{0}.log", name));

				try
				{
					File.WriteAllText(logPath, log.ToString());
				}
				catch (Exception ex)
				{
					_log.Error(string.Format(EMailResources.LogFileException, logPath, ex.AllMessages()));
				}
			}

			command.Attachments.Add(file);

			return file;
		}
	}
}