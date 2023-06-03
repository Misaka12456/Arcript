using System;
using System.Enhance.Unity;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose
{
	public class ArcriptComposeManager : Singleton<ArcriptComposeManager>
	{
#if UNITY_EDITOR
		private static readonly DateTime ProjectCreateTime = new DateTime(2023, 5, 15); // Arcript VN Creator 项目创建时间
#endif

		[Header("General")]
		public Canvas canvasPreview;
		public RectTransform panelLeft, panelRight, panelBottom, panelTopBar;

		[Header("TopBar")]
		public Text labelAppTitle;

		[Header("Resources")]
		public TextAsset assetBuild;

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}

		public void SetTitle(string projName = "", string scriptName = "")
		{
			var sb = new StringBuilder($"Arcript VN Creator v{Application.version} (Build {assetBuild.text})");
#if UNITY_EDITOR
			int dayIndex = (DateTime.Now - ProjectCreateTime).Days + 1; // 开发进度的第Day x天(从Day 1开始)
			sb.Append($" - Day {dayIndex}");
#endif
			if (!string.IsNullOrWhiteSpace(projName))
			{
				sb.Append($" - {projName}");
				if (!string.IsNullOrWhiteSpace(scriptName))
				{
					sb.Append($" - {scriptName}");
				}
				else
				{
					sb.Append(" - <New Script>");
				}
			}
			labelAppTitle.text = sb.ToString();
		}
	}
}