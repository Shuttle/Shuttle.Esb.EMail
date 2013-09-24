using System;
using System.Net.Mail;
using System.Text;
using Shuttle.Core.Infrastructure;

namespace Shuttle.EMail.Messages
{
	public static class SendEMailCommandExtensions
	{
		public static void SetBodyEncoding(this SendEMailCommand command, Encoding encoding)
		{
			Guard.AgainstNull(command, "command");
			Guard.AgainstNull(encoding, "encoding");

			command.BodyEncoding = encoding.HeaderName;
		}

		public static bool HasBodyEncoding(this SendEMailCommand command)
		{
			Guard.AgainstNull(command, "command");

			return command.BodyEncoding != null;
		}

		public static Encoding GetBodyEncoding(this SendEMailCommand command)
		{
			Guard.AgainstNull(command, "command");

			if (string.IsNullOrEmpty(command.BodyEncoding)
				||
				command.BodyEncoding.Equals("default", StringComparison.InvariantCultureIgnoreCase)
				||
				command.BodyEncoding.Equals("none", StringComparison.InvariantCultureIgnoreCase)
				||
				command.BodyEncoding.Equals("empty", StringComparison.InvariantCultureIgnoreCase)
				||
				command.BodyEncoding.Equals("unknown", StringComparison.InvariantCultureIgnoreCase))
			{
				return null;
			}

			try
			{
				return Encoding.GetEncoding(command.BodyEncoding);
			}
			catch
			{
				return null;
			}
		}

		public static void SetSubjectEncoding(this SendEMailCommand command, Encoding encoding)
		{
			Guard.AgainstNull(command, "command");
			Guard.AgainstNull(encoding, "encoding");

			command.SubjectEncoding = encoding.HeaderName;
		}

		public static bool HasSubjectEncoding(this SendEMailCommand command)
		{
			Guard.AgainstNull(command, "command");

			return command.GetSubjectEncoding() != null;
		}

		public static Encoding GetSubjectEncoding(this SendEMailCommand command)
		{
			Guard.AgainstNull(command, "command");

			if (string.IsNullOrEmpty(command.SubjectEncoding)
				||
				command.SubjectEncoding.Equals("default", StringComparison.InvariantCultureIgnoreCase)
				||
				command.SubjectEncoding.Equals("none", StringComparison.InvariantCultureIgnoreCase)
				||
				command.SubjectEncoding.Equals("empty", StringComparison.InvariantCultureIgnoreCase)
				||
				command.SubjectEncoding.Equals("unknown", StringComparison.InvariantCultureIgnoreCase))
			{
				return null;
			}

			try
			{
				return Encoding.GetEncoding(command.SubjectEncoding);
			}
			catch
			{
				return null;
			}
		}

		public static MailPriority GetMailPriority(this SendEMailCommand command)
		{
			Guard.AgainstNull(command, "command");

			try
			{
				return (MailPriority) Enum.Parse(typeof (MailPriority), command.Priority);
			}
			catch (Exception)
			{
				return MailPriority.Normal;
			}
		}

		public static void SetMailPriority(this SendEMailCommand command, MailPriority priority)
		{
			Guard.AgainstNull(command, "command");

			command.Priority = priority.ToString();
		}
	}
}