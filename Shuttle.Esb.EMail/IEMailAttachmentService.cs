namespace Shuttle.Esb.EMail
{
	public interface IEMailAttachmentService
	{
		string AddAttachment(SendEMailCommand command, string path);
	}
}