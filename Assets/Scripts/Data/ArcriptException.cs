using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcript.Data
{
	public class ArcriptException : SystemException
	{
		public ArcriptException() : base("An unexpected Arcript error has occurred.")
		{

		}

		public ArcriptException(string message) : base(message)
		{

		}

		public ArcriptException(Exception innerException) : base("An unexpected Arcript error has occurred.", innerException)
		{

		}

		public ArcriptException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
}
