using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Enhance;
using UnityEngine;

namespace Arcript.Aspt.RawArcVNScripts
{
	#region Enums
	public enum RawArcVNScriptCmdType
	{
		[Description("<invalid>")]
		Unknown = -1,

		[Description("play")]
		PlayAudio = 0,

		[Description("stop")]
		StopAudio = 1,

		[Description("say")]
		ShowLegacyText = 2,

		[Description("show")]
		ShowPicture = 3,

		[Description("hide")]
		HidePicture = 4,

		[Description("move")]
		MovePicture = 5,

		[Description("wait")]
		WaitOrSleep = 6,

		[Description("say+")]
		ShowTextv2 = 7,

		[Description("say+set")]
		SetTextv2Settings = 8,

		[Description("selectSay")]
		ShowBranchSelection = 9,

		[Description("var")]
		VarDefine = 10,

		[Description("set")]
		VarSet = 11,

		[Description("if")]
		IfGotoCheck = 12,

		[Description("label")]
		LabelDefine = 13,

		FunctionAsArgument = 100,
		SingleOptionItem = 101,

		#region Arcript+ Commands (下述命令针对纯视觉小说游戏(如Galgame)的Script)
		// 不再针对Arcaea版本的VN脚本兼容Arcript+的指令(迁移至Aspt)
		//[Description("videoFSPlay")]
		//VideoFullScreenPlay = 1000, // 全屏(这里的全屏指的是填满窗口的窗口全屏)播放视频
		//							// 通常用于播放OP和ED等情况下

		//[Description("anchor")]
		//ScriptAnchor = 1001, // Script Anchor(锚点)，用于标记Script的某个位置，以便于在Script中跳转到该位置
		//					 // 部分支持的视觉小说可使用该指令的方式增加对"BackLog"中指定内容的"Jump"支持

		//[Description("hScene")]
		//HSceneStartTag = 1002, // [必须和ScriptAnchor配合使用] HScene的开始标记(可使用该标记在Save & Load时将截图模糊化处理[可选])

		//[Description("hSceneEnd")]
		//HSceneEndTag = 1003, // 必须和ScriptAnchor配合使用！！
		#endregion
	}

	public enum VNSuperPositionType
	{
		[Description("normal")]
		Normal = 0,

		[Description("overlay")]
		Overlay = 1,

		[Description("overlayplus")]
		OverlayPlus = 2,
	}

	public enum VNCurveType
	{
		[Description("linear")]
		Linear = 0, // l

		[Description("sineout")]
		SineOut = 1, // so

		[Description("sineinout")]
		SineInOut = 2, // sio

		[Description("cubicout")]
		CubicOut = 3, // qo
	}
	#endregion

	public class FuncArgument
	{
		public string FuncName { get; set; }

		public List<object> Arguments { get; set; }
	}
	
	public abstract class ArcVNScriptCmdBase
	{
		public abstract RawArcVNScriptCmdType Type { get; }

		public abstract List<string> Arguments { get; }

		/// <summary>
		/// 执行当前命令。
		/// </summary>
		/// <returns>
		/// 如果执行的命令需要玩家操作才能继续(见下)，则返回 <see langword="false"/>； 否则返回 <see langword="true"/>。 <br />
		/// <para>
		/// "需要玩家操作才能继续"的命令通常情况下表现为该命令执行后阻塞之后所有命令的执行，直到玩家进行了某些操作才能继续执行。<br />
		/// 下面是一些例子：
		/// <list type="bullet">
		/// <item>
		/// 显示文本 - <see cref="TextLegacyShowCmd"/> (<c>say</c>) 或 <see cref="TextShowCmd"/> (<c>say+</c>) <br />
		/// 文本显示完毕后，需要玩家单击才能继续执行Script。
		/// </item>
		/// <item>
		/// 选择支 - <see cref="BranchSelectCmd"/> (<c>selectSay</c>) <br />
		/// 显示完毕所有选项后，玩家需要选择给出的多个选项中的一个(通常是通过"单击"选项按钮)才能继续执行Script。
		/// </item>
		/// </list>
		/// </para>
		/// </returns>
		public abstract bool Execute();
	}

	#region Basic Commands
	public class AudioPlayCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.PlayAudio;
		public override List<string> Arguments
		{
			get
			{
				var r = new List<string>()
				{
					{ AudioPath }, { Volume.ToString() }
				};
				if (IsLoop) r.Add("loop");
				return r;
			}
		}
		#endregion

		public string AudioPath { get; set; }

		public float Volume { get; set; }

		public bool IsLoop { get; set; } = false;

		public override bool Execute() => throw new NotImplementedException();
	}

	public class AudioStopCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.StopAudio;
		public override List<string> Arguments => new List<string>() { { AudioPath }, { Duration.ToString() } };
		#endregion

		public string AudioPath { get; set; }

		public float Duration { get; set; }

		public override bool Execute() => throw new NotImplementedException();
	}

	public class TextLegacyShowCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.ShowLegacyText;
		public override List<string> Arguments => new List<string>() { { Text } };
		#endregion

		public string Text { get; set; }

		public override bool Execute() => throw new NotImplementedException();
	}

	public class PictureShowCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.ShowPicture;
		public override List<string> Arguments
		{
			get
			{
				var r = new List<string>()
				{
					PicturePath,
					$"{PictureAnchorPos.x}:{PictureAnchorPos.y}",
					$"{ScreenAnchorPos.x}:{ScreenAnchorPos.y}",
					$"{Scale.x}:{Scale.y}"
				};
				if (Transition != null)
				{
					// funcName(args...)
					r.Add($"{Transition.FuncName}({string.Join(",", Transition.Arguments)})");
				}
				else
				{
					r.Add("none");
				}
				r.Add(SuperPosition.GetDescription());
				if (EnableFreeScaling) r.Add("scale");
				return r;
			}
		}
		#endregion

		public string PicturePath { get; set; }

		public Vector2 PictureAnchorPos { get; set; }

		public Vector2 ScreenAnchorPos { get; set; }

		public Vector2 Scale { get; set; }

		public FuncArgument Transition { get; set; }

		public VNSuperPositionType SuperPosition { get; set; }

		public bool EnableFreeScaling { get; set; } = false;

		public override bool Execute() => throw new NotImplementedException();
	}

	public class PictureHideCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.HidePicture;
		public override List<string> Arguments
		{
			get
			{
				var r = new List<string>()
				{
					PicturePath
				};
				if (Transition != null)
				{
					// funcName(args...)
					r.Add($"{Transition.FuncName}({string.Join(",", Transition.Arguments)})");
				}
				else
				{
					r.Add("none");
				}
				return r;
			}
		}
		#endregion

		public string PicturePath { get; set; }

		public FuncArgument Transition { get; set; } = null;

		public override bool Execute() => throw new NotImplementedException();
	}

	public class PictureMoveCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.MovePicture;

		public override List<string> Arguments => new List<string>()
		{
			PicturePath,
			$"{MovePosDelta.x}:{MovePosDelta.y}",
			$"{Duration}",
			Curve.GetDescription()
		};
		#endregion

		public string PicturePath { get; set; }

		public Vector2 MovePosDelta { get; set; } // 注意这里的坐标单位是int不是float

		public float Duration { get; set; }

		public VNCurveType Curve { get; set; }

		public override bool Execute() => throw new NotImplementedException();
	}

	public class GeneralWaitCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.WaitOrSleep;
		public override List<string> Arguments => new List<string>() { { Duration.ToString() } };
		#endregion

		public float Duration { get; set; }

		public override bool Execute() => throw new NotImplementedException();
	}
	#endregion

	#region Advanced Commands
	public class TextShowCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.ShowTextv2;
		public override List<string> Arguments
		{
			get
			{
				var r = new List<string>()
				{
					{ Speaker }, { Text }
				};
				if (TreatAsFallbackLegacyText)
				{
					r.Add("fallback");
				}
				return r;
			}
		}
		#endregion

		public string Speaker { get; set; }

		public string Text { get; set; }

		public bool TreatAsFallbackLegacyText { get; set; } = false;

		public override bool Execute() => throw new NotImplementedException();
	}

	public class TextShowSettingsCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.SetTextv2Settings;
		public override List<string> Arguments => new List<string>() { { SpeakerBoxImagePath }, { MessageBoxImagePath } };
		#endregion

		public string SpeakerBoxImagePath { get; set; }

		public string MessageBoxImagePath { get; set; }

		public override bool Execute() => throw new NotImplementedException();
	}

	public class SubOptionItemCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.SingleOptionItem;
		public override List<string> Arguments
		{
			get
			{
				var r = new List<string>()
				{
					{ ShownOptionText },
					{ "*Fixed=," }, // Fixed表明从上到下序列化时此处为固定的字符串(这里是",")(下同)
					{ OptionId },
					{ "*Fixed=," },
					{ OptionAction.FuncName },
					{ "*Fixed=," },
					{ GotoLabel },
					{ "*Fixed=;" } // 指定结束符(不使用默认的string.Empty)
				};
				if (SeletableRequirement != null)
				{
					r.Add($"{SeletableRequirement.FuncName}({string.Join(",", SeletableRequirement.Arguments)})");
				}
				return r;
			}
		}
		#endregion

		public string ShownOptionText { get; set; }

		public string OptionId { get; set; }

		public FuncArgument OptionAction { get; set; }

		public string GotoLabel { get; set; }

		public FuncArgument SeletableRequirement { get; set; } = null;

		public override bool Execute() => throw new NotImplementedException();
	}

	public class BranchSelectCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.ShowBranchSelection;
		public override List<string> Arguments
		{
			get
			{
				var r = new List<string>()
				{
					{ BranchSelectTip },
					{ OptionButtonImagePath },
					{ "*Fixed={" }
				};
				foreach (var option in Options)
				{
					r.AddRange(option.Arguments);
				}
				r.Add("*Fixed=}");
				return r;
			}
		}
		#endregion

		public string BranchSelectTip { get; set; }

		public string OptionButtonImagePath { get; set; }

		public List<SubOptionItemCmd> Options { get; set; } = new List<SubOptionItemCmd>();

		public override bool Execute() => throw new NotImplementedException();
	}

	public class VarDefineCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.VarDefine;
		public override List<string> Arguments => new List<string>()
		{
			{ VariableName },
			{ "*Fixed==" },
			{ InitialValue.ToString() }
		};
		#endregion

		public string VariableName { get; set; } = $"myVariable{Guid.NewGuid():N}";

		public int InitialValue { get; set; } = 0;

		public override bool Execute() => throw new NotImplementedException();
	}

	public class VarSetCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.VarSet;
		public override List<string> Arguments => new List<string>()
		{
			{ TargetVariableName },
			{ "*Fixed==" },
			{ TargetValue.ToString() }
		};
		#endregion

		public string TargetVariableName { get; set; }

		public int TargetValue { get; set; }

		public override bool Execute() => throw new NotImplementedException();
	}

	public class IfGotoCheckCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.IfGotoCheck;
		public override List<string> Arguments => new List<string>()
		{
			{ $"{Condition.FuncName}({string.Join(",", Condition.Arguments)})" },
			{ "*Fixed=goto" },
			{ GotoLabel }
		};
		#endregion

		public FuncArgument Condition { get; set; }

		public string GotoLabel { get; set; }

		public override bool Execute() => throw new NotImplementedException();
	}

	public class LabelDefineCmd : ArcVNScriptCmdBase
	{
		#region Overrided Properties
		public override RawArcVNScriptCmdType Type => RawArcVNScriptCmdType.LabelDefine;
		public override List<string> Arguments => new List<string>()
		{
			{ LabelName },
			{ "*Fixed=:" }
		}; // 即 $"{LabelName}:"
		#endregion

		public string LabelName { get; set; }

		public override bool Execute() => throw new NotImplementedException();
	}
	#endregion

	public class RawArcVNScript : IEnumerable, IEnumerable<ArcVNScriptCmdBase>
	{
		public List<ArcVNScriptCmdBase> Commands { get; set; } = new List<ArcVNScriptCmdBase>();

		public IEnumerator<ArcVNScriptCmdBase> GetEnumerator()
		{
			return ((IEnumerable<ArcVNScriptCmdBase>)Commands).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)Commands).GetEnumerator();
		}

		public ArcVNScriptCmdBase this[int index]
		{
			get => Commands[index];
			set => Commands[index] = value;
		}
	}
}