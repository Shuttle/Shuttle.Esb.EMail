using System;

namespace Shuttle.EMail
{
	public class EMailException : Exception
	{
		public EMailException(string message) : base(message)
		{
		}
	}
}