using System;

namespace Arcript.Data
{
	public class ArcriptGameplayException : ArcriptException
	{
		public ArcriptGameplayException() : base("An unexpected Arcript gameplay error has occurred.")
		{

		}

		public ArcriptGameplayException(string message) : base(message)
		{

		}

		public ArcriptGameplayException(Exception innerException) : base("An unexpected Arcript gameplay error has occurred.", innerException)
		{

		}

		public ArcriptGameplayException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
}
