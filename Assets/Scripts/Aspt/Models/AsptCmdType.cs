using System.ComponentModel;

namespace Arcript.Aspt
{
	public enum AsptCmdType
	{
		// 标注有"Sub"的命令是子命令，不能单独使用，必须作为父命令的子命令使用
		[Description("<invalid>")] Unknown = -1,

		#region Audios
		[Description("play")] PlayAudio = 0,

		[Description("stop")] StopAudio = 1,
		#endregion

		#region Pictures
		[Description("show")] ShowPicture = 10,

		[Description("hide")] HidePicture = 11,

		[Description("move")] MovePicture = 12,
		#endregion

		#region Texts
		[Description("say")] ShowTextLegacy = 20,
		[Description("say+")] [ArcriptPlus] ShowTextv2 = 21,
		[Description("say+set")] [ArcriptPlus] SetTextv2Settings = 22,
		#endregion

		[Description("wait")] WaitOrSleep = 30,

		#region Branch Selections
		[Description("selectSay")] [ArcriptPlus] ShowBranchSelection = 40,

		[Description("select")] [ArcriptPlus] SubBranchOptionItem = 41,
		#endregion

		#region Variable & Label & If Checks
		[Description("var")] [ArcriptPlus] VarDefine = 50,
		
		[Description("varSet")] [ArcriptPlus] VarSet = 51,

		[Description("varCheck")][ArcriptPlus] SubVarCheck = 52,

		[Description("if")] [ArcriptPlus] IfCheck = 53,

		[Description("label")] [ArcriptPlus] LabelDefine = 54,
		#endregion

		[Description("videoFSPlay")] [ArcriptPlus] VideoFullScreenPlay = 60,

		#region Anchor Tags
		[Description("anchor")] [ArcriptPlus] ScriptAnchor = 70,

		[Description("anchorHSStart")] [ArcriptPlus] HSceneStartAnchorTag = 71,

		[Description("anchorHSEnd")] [ArcriptPlus] HSceneEndAnchorTag = 72,
		#endregion

		[Description("qFunc")][ArcriptPlus] SubQuickFunction = 80,
	}
}
