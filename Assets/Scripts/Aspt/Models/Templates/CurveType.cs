using System.ComponentModel;

namespace Arcript.Aspt
{
	public enum CurveType
	{
		[Description("linear")] Linear = 0,
		[Description("sinein")] SineIn = 1,
		[Description("sineout")] SineOut = 2,
		[Description("sineinout")] SineInOut = 3,
		[Description("cubicin")] CubicIn = 4,
		[Description("cubicout")] CubicOut = 5,
		[Description("cubicinout")] CubicInOut = 6,
		[Description("bezier")] Bezier = 7,
	}
}
