using System;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail
{
    public interface IEMailAttachmentService
    {
        SendEMailCommand.Attachment AddAttachment(SendEMailCommand command, string path);
        void RemoveAttachment(Guid emailId, Guid attachmentId, string originalFileName);
        string GetAttachmentPath(Guid emailId, Guid attachmentId, string originalFileName);
        string GetAttachmentFileName(Guid emailId, Guid attachmentId, string originalFileName);
    }
}