using System;
using System.Net.Mail;
using System.Text;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.EMail.Messages
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

			if (NullEncoding(command.BodyEncoding))
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

		private static bool NullEncoding(string encoding)
		{
			return string.IsNullOrEmpty(encoding)
			       ||
			       encoding.Equals("default", StringComparison.InvariantCultureIgnoreCase)
			       ||
			       encoding.Equals("none", StringComparison.InvariantCultureIgnoreCase)
			       ||
			       encoding.Equals("empty", StringComparison.InvariantCultureIgnoreCase)
			       ||
			       encoding.Equals("unknown", StringComparison.InvariantCultureIgnoreCase);
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

			if (NullEncoding(command.SubjectEncoding))
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