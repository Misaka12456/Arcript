using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Enhance.Unity
{
	/// <summary>
	/// Provides easing functions for use in animations (in Unity).
	/// </summary>
	public static class EasingHelper
	{
		/// <summary>
		/// Linear easing. <br />
		/// Formula(with min, max, x): (max - min) * x + min;
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <param name="progress">The progress of the animation.</param>
		/// <returns>The eased value.</returns>
		public static float Linear(float min, float max, float progress) => (max - min) * progress + min;

		/// <summary>
		/// InSine easing (a.k.a. sinein). <br />
		/// Formula(with min, max, x): -1 * (max - min) * cos(x * PI / 2) + (max - min) + min;
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <param name="progress">The progress of the animation.</param>
		/// <returns>The eased value.</returns>
		public static float InSine(float min, float max, float progress) => -(max - min) * (float)Math.Cos(progress * (Math.PI / 2)) + max;

		/// <summary>
		/// OutSine easing (a.k.a. sineout). <br />
		/// Formula(with min, max, x): (max - min) * sin(x * PI / 2) + min;
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <param name="progress">The progress of the animation.</param>
		/// <returns>The eased value.</returns>
		public static float OutSine(float min, float max, float progress) => (max - min) * (float)Math.Sin(progress * (Math.PI / 2)) + min;

		/// <summary>
		/// InOutSine easing (a.k.a. sineinout). <br />
		/// Formula(with min, max, x): -1 * (max - min) / 2 * (cos(PI * x) - 1) + min;
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <param name="progress">The progress of the animation.</param>
		/// <returns>The eased value.</returns>
		public static float InOutSine(float min, float max, float progress) => -(max - min) / 2 * ((float)Math.Cos(Math.PI * progress) - 1) + min;

		public static float InCubic(float min, float max, float progress) => (max - min) * (float)Math.Pow(progress, 3) + min;

		public static float OutCubic(float min, float max, float progress) => (max - min) * (float)Math.Pow(progress - 1, 3) + max;

		public static float InOutCubic(float min, float max, float progress) => (max - min) * (float)Math.Pow(progress * 2, 3) / 2 + min;
	}
}
