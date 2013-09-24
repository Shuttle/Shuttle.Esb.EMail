using Shuttle.EMail.Messages;

namespace Shuttle.EMail
{
	public interface IEMailService
	{
		string AddAttachment(SendEMailCommand command, string path);
	}
}