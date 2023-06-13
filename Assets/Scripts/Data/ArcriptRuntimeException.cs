using System;

namespace Arcript.Data
{
	public class ArcriptRuntimeException : ArcriptException
	{
		public ArcriptRuntimeException() : base("An unexpected Arcript runtime error has occurred.")
		{

		}

		public ArcriptRuntimeException(string message) : base(message)
		{

		}

		public ArcriptRuntimeException(Exception innerException) : base("An unexpected Arcript runtime error has occurred.", innerException)
		{

		}

		public ArcriptRuntimeException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
}
