using System.IO;
using UnityEngine;

namespace System.Enhance.Unity
{
	public static class ApplicationHelper
	{
		public static string GetVersion(int fieldCount = 2, bool includeSuffix = true)
		{
			string version = Application.version;
			var mainVersion = Version.Parse(version.Split('-')[0]);
			bool is3_0Beta = version.StartsWith("2.9");
			if (is3_0Beta)
			{
				int thirdNum = int.Parse(version.Split('-')[0].Split('.')[2]);
				thirdNum++;
				version = $"3.0 Beta {thirdNum}";
				return version;
			}
			if (version.Split('-').Length > 1 && includeSuffix)
			{
				string suffix = version.Split(new[] { '-' }, count: 2)[1];
				suffix = suffix.Replace('-', ' ');
				return string.Concat(mainVersion.ToString(fieldCount), " ", suffix);
			}
			else
			{
				return mainVersion.ToString(fieldCount);
			}
		}

		public static string BaseDirectory
		{
			get
			{
#if UNITY_EDITOR
				string dataPath = Application.dataPath; // 编辑器模式下为$"{UnityProjPath}/Assets"
				return new DirectoryInfo(dataPath).Parent.FullName;
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
				string dataPath = Application.dataPath; // 为$"{UnityBuildPath}/Arcript_Data"
				return new DirectoryInfo(dataPath).Parent.FullName;
#else
				return Application.persistentDataPath;
#endif
			}
		}
    }
}
