using Arcript.Data;
using Arcript.I18n;
using System;
using System.Enhance.Unity;
using System.Enhance.Unity.UI;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MsgBoxType = System.Enhance.Unity.UI.MsgBoxDialog.MsgBoxType;

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
			Application.logMessageReceived += Arcript_LogMessageReceived;
		}

		private void Arcript_LogMessageReceived(string condition, string stackTrace, LogType type)
		{
			if (type == LogType.Exception)
			{
				// 如果来源异常不是继承自或为ArcriptException，则直接忽略
				if (stackTrace.IndexOf(nameof(ArcriptException)) < 0) return;
				if (!ArcriptConfig.S.EnableGlobalErrorMsgBoxPopup) return;
				MsgBoxDialog.Show(condition, I.S["compose.dialogs.error.title"].value, MsgBoxType.OKOnly | MsgBoxType.IconError);
			}
		}

		public void SetTitle(string projName = "", string scriptName = "", bool pendingModify = false)
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
					//sb.Append($" - {scriptName}");
					sb.Append(" - ");
					if (pendingModify)
					{
						sb.Append("*");
					}
					sb.Append(scriptName);
				}
			}
			labelAppTitle.text = sb.ToString();
		}
	}
}