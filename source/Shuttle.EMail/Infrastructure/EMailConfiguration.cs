using System.IO;
using Shuttle.Core.Infrastructure;

namespace Shuttle.EMail
{
	public class EMailConfiguration : IEMailConfiguration
	{
		private readonly ConfigurationItem<string> attachmentFolder;

		public EMailConfiguration()
		{
			attachmentFolder = ConfigurationItem<string>.ReadSetting("AttachmentFolder");

			if (!Directory.Exists(AttachmentFolder))
			{
				var message = string.Format("Attachment folder '{0}' does not exist.", AttachmentFolder);

				Log.Error(message);

				throw new DirectoryNotFoundException(message);
			}
		}

		public string AttachmentFolder
		{
			get { return attachmentFolder.GetValue(); }
		}
	}
}