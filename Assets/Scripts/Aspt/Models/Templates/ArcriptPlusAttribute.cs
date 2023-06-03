using System;

namespace Arcript.Aspt
{
	/// <summary>
	/// Represents a Arcript+ feature only command.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
	public class ArcriptPlusAttribute : Attribute
	{
		public ArcriptPlusAttribute() : base()
		{
			
		}
	}
}
