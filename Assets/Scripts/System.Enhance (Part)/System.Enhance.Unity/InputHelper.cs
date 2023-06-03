using System.Text;
using UnityEngine;

namespace System.Enhance.Unity
{
	public static class InputHelper
	{
		public static string ToFriendlyString(this KeyCode key)
		{
			var sb = new StringBuilder();
			if (key == KeyCode.None)
			{
				return "None";
			}
			sb.Append(key.ToString());
			sb = sb.Replace("Left", string.Empty).Replace("Right", string.Empty);
			sb = sb.Replace("Control", "Ctrl");
			sb = sb.Replace("Alpha", string.Empty);
			sb = sb.Replace("Keypad", "NumPad");
			sb = sb.Replace("Minus", "-");
			sb = sb.Replace("Plus", "+");
			sb = sb.Replace("Period", ".");
			sb = sb.Replace("Slash", "/");
			return sb.ToString();
		}
	}
}
