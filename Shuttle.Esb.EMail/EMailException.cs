using System;

namespace Shuttle.Esb.EMail
{
	public class EMailException : Exception
	{
		public EMailException(string message) : base(message)
		{
		}
	}
}