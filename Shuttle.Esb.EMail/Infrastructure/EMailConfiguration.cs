using System.IO;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.EMail
{
	public class EMailConfiguration : IEMailConfiguration
	{
		public EMailConfiguration()
		{
			AttachmentFolder = ConfigurationItem<string>.ReadSetting("AttachmentFolder").GetValue();

			if (Directory.Exists(AttachmentFolder))
			{
				return;
			}

			var message = string.Format("Attachment folder '{0}' does not exist.", AttachmentFolder);

			Log.Error(message);

			throw new DirectoryNotFoundException(message);
		}

		public string AttachmentFolder { get; private set; }
	}
}