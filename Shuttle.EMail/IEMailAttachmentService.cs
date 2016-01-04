namespace Shuttle.EMail
{
	public interface IEMailAttachmentService
	{
		string AddAttachment(SendEMailCommand command, string path);
	}
}