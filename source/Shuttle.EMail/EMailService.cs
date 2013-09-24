using System;
using System.IO;
using System.Text;
using Shuttle.Core.Infrastructure;
using Shuttle.EMail.Messages;

namespace Shuttle.EMail
{
	public class EMailService : IEMailService
	{
		private readonly IEMailConfiguration configuration;

		public EMailService(IEMailConfiguration configuration)
		{
			Guard.AgainstNull(configuration, "configuration");

			this.configuration = configuration;
		}

		public string AddAttachment(SendEMailCommand command, string path)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException(string.Format("File '{0}' does not exist.  Could not add e-mail attachment.", path));
			}

			var name = Guid.NewGuid().ToString("n");
			var file = string.Format("{0}{1}", name, Path.GetExtension(path));
			var targetPath = Path.Combine(configuration.AttachmentFolder, file);

			try
			{
				File.Copy(path, targetPath, true);
			}
			catch (Exception ex)
			{
				throw new EMailException(string.Format("File '{0}' not added as attachment.  Exception reported: {1}", path, ex.CompactMessages()));
			}

			var log = new StringBuilder();

			log.AppendFormat("Date created\t{0}", DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"));
			log.AppendLine();
			log.AppendFormat("Source file path:\t{0}", path);
			log.AppendLine();
			log.AppendFormat("Source machine\t{0}", Environment.MachineName);
			log.AppendLine();
			log.AppendFormat("Source identity\t{0}", string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName));
			log.AppendLine();

			File.WriteAllText(Path.Combine(configuration.AttachmentFolder, string.Format("{0}.log", name)), log.ToString());

			command.Attachments.Add(file);

			return file;
		}
	}
}