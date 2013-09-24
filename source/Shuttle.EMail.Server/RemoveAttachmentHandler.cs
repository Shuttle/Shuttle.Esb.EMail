using System.IO;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.EMail.Messages;

namespace Shuttle.EMail.Server
{
	public class RemoveAttachmentHandler : IMessageHandler<RemoveAttachmentCommand>
	{
		private readonly IEMailConfiguration configuration;

		public RemoveAttachmentHandler(IEMailConfiguration configuration)
		{
			Guard.AgainstNull(configuration, "configuration");

			this.configuration = configuration;
		}

		public void ProcessMessage(HandlerContext<RemoveAttachmentCommand> context)
		{
			var file = context.Message.File;

			var path = Path.Combine(configuration.AttachmentFolder, file);

			if (File.Exists(path))
			{
				File.Delete(path);
			}

			path = Path.Combine(configuration.AttachmentFolder, string.Format("{0}.log", Path.GetFileNameWithoutExtension(path)));

			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}