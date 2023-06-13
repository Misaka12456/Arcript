using Newtonsoft.Json;
using System;
using Arcript.Data;
using System.IO;
using UnityEngine;
using System.Text;

namespace Arcript.Compose
{
	[Serializable]
	public class ArcriptConfig
	{
		public static ArcriptConfig S { get; private set; }
		
		/// <summary>
		/// Arcript项目的默认路径
		/// </summary>
		[JsonProperty]
		public string DefaultProjectPath = string.Empty;

		/// <summary>
		/// 是否针对新项目默认启用Arcript+特性支持(启用Arcript+将会禁用"Arcaea纯文本脚本格式(.vns)"编译选项)
		/// </summary>
		[JsonProperty]
		public bool EnableArptPlusFeaturesForNewProject = false;

		/// <summary>
		/// 是否启用全局错误消息框弹出(仅针对 <see cref="ArcriptException" /> 类型的异常(包括继承自该类型的异常))
		/// </summary>
		[JsonProperty]
		public bool EnableGlobalErrorMsgBoxPopup = true;

		/// <summary>
		/// 是否在Arcript启动时检查更新
		/// </summary>
		[JsonProperty]
		public bool CheckUpdateWhenStartup = true;

		static ArcriptConfig()
		{
			S = LoadPreferences();
			SavePreferences(S);
		}

		private static ArcriptConfig LoadPreferences()
		{
			ArcriptConfig conf;
			try
			{
				conf = JsonConvert.DeserializeObject<ArcriptConfig>(File.ReadAllText(Path.Combine(Application.persistentDataPath, "config.dat"), Encoding.UTF8));
			}
			catch
			{
				conf = new ArcriptConfig();
			}
			conf ??= new ArcriptConfig();
			return conf;
		}

		private static void SavePreferences(ArcriptConfig conf)
		{
			try
			{
				File.WriteAllText(Path.Combine(Application.persistentDataPath, "config.dat"), JsonConvert.SerializeObject(conf, Formatting.Indented), Encoding.UTF8);
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"[ArcriptConfig] Cannot save config preferences: {ex}");
			}
		}

		~ArcriptConfig()
		{
			SavePreferences(this);
		}
	}
}
