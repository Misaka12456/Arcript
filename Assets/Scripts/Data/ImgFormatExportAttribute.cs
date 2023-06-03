using System;

namespace Arcript.Data
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ImgFormatExportAttribute : Attribute
	{
		public string FindExtensionPattern { get; }

		public int Priority { get; } = 0; // 若存在多个相同的匹配模式则按照优先级顺序使用(TryLoadxxx)方法测试(只要有一个测试通过就会使用)

		public ImgFormatExportAttribute(string pattern, int priority = 0)
		{
			FindExtensionPattern = pattern;
			Priority = priority;
		}
	}
}
