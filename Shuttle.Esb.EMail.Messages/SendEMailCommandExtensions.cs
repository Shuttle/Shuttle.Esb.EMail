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
			Guard.AgainstNull(command, nameof(command));
			Guard.AgainstNull(encoding, nameof(encoding));

			command.BodyEncoding = encoding.HeaderName;
		}

		public static bool HasBodyEncoding(this SendEMailCommand command)
		{
            Guard.AgainstNull(command, nameof(command));

			return !string.IsNullOrWhiteSpace(command.BodyEncoding);
		}

		public static Encoding GetBodyEncoding(this SendEMailCommand command)
		{
            Guard.AgainstNull(command, nameof(command));

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

        public static void SetHtmlBodyEncoding(this SendEMailCommand command, Encoding encoding)
        {
            Guard.AgainstNull(command, nameof(command));
            Guard.AgainstNull(encoding, nameof(encoding));

            command.HtmlBodyEncoding = encoding.HeaderName;
        }

        public static Encoding GetHtmlBodyEncoding(this SendEMailCommand command)
        {
            Guard.AgainstNull(command, nameof(command));

            if (NullEncoding(command.HtmlBodyEncoding))
            {
                return null;
            }

            try
            {
                return Encoding.GetEncoding(command.HtmlBodyEncoding);
            }
            catch
            {
                return null;
            }
        }
        
        public static bool HasHtmlBodyEncoding(this SendEMailCommand command)
        {
            Guard.AgainstNull(command, nameof(command));

            return !string.IsNullOrWhiteSpace(command.HtmlBodyEncoding);
        }

        public static Encoding GetDisplayNameEncoding(this SendEMailCommand.Address address)
        {
            Guard.AgainstNull(address, nameof(address));

            if (NullEncoding(address.DisplayNameEncoding))
            {
                return null;
            }

            try
            {
                return Encoding.GetEncoding(address.DisplayNameEncoding);
            }
            catch
            {
                return null;
            }
        }
        
        public static bool HasDisplayNameEncoding(this SendEMailCommand.Address address)
        {
            Guard.AgainstNull(address, nameof(address));

            return !string.IsNullOrWhiteSpace(address.DisplayNameEncoding);
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
            Guard.AgainstNull(command, nameof(command));
			Guard.AgainstNull(encoding, nameof(encoding));

			command.SubjectEncoding = encoding.HeaderName;
		}

		public static bool HasSubjectEncoding(this SendEMailCommand command)
		{
            Guard.AgainstNull(command, nameof(command));

			return command.GetSubjectEncoding() != null;
		}

		public static Encoding GetSubjectEncoding(this SendEMailCommand command)
		{
            Guard.AgainstNull(command, nameof(command));

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
            Guard.AgainstNull(command, nameof(command));

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
            Guard.AgainstNull(command, nameof(command));

			command.Priority = priority.ToString();
		}
		
        public static string GetDisplayNameOptional(this SendEMailCommand.Address address)
		{
            Guard.AgainstNull(address, nameof(address));

            return string.IsNullOrWhiteSpace(address.DisplayName) ? string.Empty : address.DisplayName;
        }
	}
}