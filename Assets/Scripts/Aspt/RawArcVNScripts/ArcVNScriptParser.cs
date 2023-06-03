using System;
using System.Collections.Generic;
using System.Enhance;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Arcript.Aspt.RawArcVNScripts
{
	public class ArcVNScriptParser
	{
		public static ArcVNScriptParser S { get; } = new ArcVNScriptParser();

		public Dictionary<(int, int), string> Warnings { get; private set; } = new Dictionary<(int, int), string>();

		private StringParser parser;

		public void Load(string line)
		{
			parser = new StringParser(line.Trim());
		}

		public ArcVNScriptCmdBase Parse(int line, out RawArcVNScriptCmdType type)
		{
			string cmd = parser.ReadString(ternimator: StringParser.Space);
			if (string.IsNullOrEmpty(cmd)) throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: 0);
			switch (cmd)
			{
				#region Basic Commands (7)
				case "play": // 播放音频
					type = RawArcVNScriptCmdType.PlayAudio;
					return ParseAudioPlay(line);
				case "stop": // 停止音频
					type = RawArcVNScriptCmdType.StopAudio;
					return ParseAudioStop(line);
				case "say": // 显示/隐藏文本(旧版)
					type = RawArcVNScriptCmdType.ShowLegacyText;
					return ParseTextLegacyShow(line);
				case "show": // 显示图片
					type = RawArcVNScriptCmdType.ShowPicture;
					return ParsePictureShow(line);
				case "hide": // 隐藏图片
					type = RawArcVNScriptCmdType.HidePicture;
					return ParsePictureHide(line);
				case "move": // 移动图片
					type = RawArcVNScriptCmdType.MovePicture;
					return ParsePictureMove(line);
				case "wait": // 等待/暂时暂停执行(一段时间)
					type = RawArcVNScriptCmdType.WaitOrSleep;
					return ParseWait(line);
				#endregion

				#region Advanced Commands (7)
				case "say+": // 显示/隐藏文本(支持显示说话者(Speaker))
					type = RawArcVNScriptCmdType.ShowTextv2;
					return ParseTextShow(line);
				case "say+set": // 设置新版文本显示的图片资源(说话者名称框和文本框)
					type = RawArcVNScriptCmdType.SetTextv2Settings;
					return ParseTextShowSettings(line);
				case "selectSay": // 选择支(包括内嵌的选项的"select"指令)
					type = RawArcVNScriptCmdType.ShowBranchSelection;
					return ParseBranchSelect(line);
				case "var": // 定义变量
					type = RawArcVNScriptCmdType.VarDefine;
					return ParseVarDefine(line);
				case "set": // 设置变量的值
					type = RawArcVNScriptCmdType.VarSet;
					return ParseVarSet(line);
				case "if": // 如果跳转(If-Goto)条件判断
					type = RawArcVNScriptCmdType.IfGotoCheck;
					return ParseIfGotoCheck(line);
				case "label": // 定义标签(显式声明)
					type = RawArcVNScriptCmdType.LabelDefine;
					return ParseLabelDefine(line);
				#endregion
				default:
					return ParseFallback(line, out type);
			}
		}

		private ArcVNScriptCmdBase ParseFallback(int line, out RawArcVNScriptCmdType type)
		{
			#region Fallback Commands
			#region Label / 定义标签(隐式声明)
			var simpleLabelRegex = new Regex(@"^\s*(?<labelName>[a-zA-Z_]([a-zA-Z0-9_]*))\s*:\s*$");
			// 必须按照"abcdefg:"的格式，且a前面和:后面除了空格外不能有其他字符
			// 即最常见的如"Kano_Branch02:"的标签格式
			if (simpleLabelRegex.IsMatch(parser.FullStr))
			{
				var groups = simpleLabelRegex.Match(parser.FullStr).Groups;
				string labelName = groups["labelName"].Value;
				Warnings.Add((line, 0), $"Parsing Label: {labelName} as Simple Label Format...");
				type = RawArcVNScriptCmdType.LabelDefine;
				return ParseLabelDefine(line);
			}
			#endregion
			#endregion

			#region True/Final Default
			// fallback default
			string cmdTypeStr;
			try
			{
				cmdTypeStr = parser.ReadString(ternimator: StringParser.Space);
			}
			catch
			{
				cmdTypeStr = "<invalid token>";
			}
			cmdTypeStr = cmdTypeStr.Replace("'", @"\'"); // 转义单引号，防止与警告信息的单引号混淆
			Warnings.Add((line, 0), $"Script command type '{cmdTypeStr}' is not known by us, so it will be ignored.");
			type = RawArcVNScriptCmdType.Unknown;
			return null;
			#endregion
		}

		#region Parsing
		#region Basic Commands
		private ArcVNScriptCmdBase ParseAudioPlay(int line)
		{
			try
			{
				// play <audioPath> <volume> [loop]
				parser.Skip(StringParser.Space);
				parser.Skip("\"");
				string audioPath = parser.ReadString(ternimator: "\"");
				parser.Skip("\"");
				parser.Skip(StringParser.Space);
				if (!parser.TryReadFloat(out float volume, ternimator: StringParser.Space))
				{
					volume = parser.ReadInt(ternimator: StringParser.Space);
				}
				bool isLoop = false;
				try
				{
					string loopSign = parser.ReadString(ternimator: StringParser.Space).ToLower();
					if (loopSign == "loop")
					{
						isLoop = true;
						parser.Skip(StringParser.Space);
					}
				}
				catch
				{
					isLoop = false;
				}
				return new AudioPlayCmd()
				{
					AudioPath = audioPath,
					Volume = volume,
					IsLoop = isLoop,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParseAudioStop(int line)
		{
			try
			{
				// stop <audioPath> <duration>
				parser.Skip(StringParser.Space);
				parser.Skip("\"");
				string audioPath = parser.ReadString(ternimator: "\"");
				parser.Skip("\"");
				parser.Skip(StringParser.Space);
				if (!parser.TryReadFloat(out float duration, ternimator: StringParser.Space))
				{
					duration = parser.ReadInt(ternimator: StringParser.Space);
				}
				if (duration <= 0)
				{
					duration = 0;
					Warnings.Add((line, parser.Pos), "Duration <= 0 will be treated as 'immediately'.");
				}
				return new AudioStopCmd()
				{
					AudioPath = audioPath,
					Duration = duration,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParseTextLegacyShow(int line)
		{
			try
			{
				// say <text> (<text> can use \" to escape quote)
				parser.Skip(StringParser.Space);
				parser.Skip("\"");
				var textBuilder = new StringBuilder();
				for (; ; )
				{
					textBuilder.Append(parser.ReadString(ternimator: "\""));
					if (textBuilder[textBuilder.Length - 1] == '\\')
					{
						continue;
					}
					break;
				}
				string text = textBuilder.ToString();
				return new TextLegacyShowCmd()
				{
					Text = text,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParsePictureShow(int line)
		{
			try
			{
				// show <pic:stringB> <posX:float>:<posY:float> <anchorX:float>:<anchorY:float> <scaleX:float>:<scaleY:float> <transition:string> <superposition:string> [scale:string]
				// 'stringB' means the string is enclosed by double quotes (separated from 'string')
				// from this command, there are no-double-quote string (with only space to delimit), so we added the 'stringB' concept
				parser.Skip(StringParser.Space);

				#region <pic:stringB>
				parser.Skip("\"");
				string picPath = parser.ReadString(ternimator: "\"");
				parser.Skip("\"");
				#endregion

				parser.Skip(StringParser.Space);

				#region <posX:float>:<posY:float>
				// posX may exists as a int-like float, e.g. 1 (it is 1.0f actually)
				if (!parser.TryReadFloat(out float picAnchorPosX, ternimator: StringParser.Colon))
				{
					picAnchorPosX = parser.ReadInt(ternimator: StringParser.Colon);
				}
				// same as posY, anchorXY and scaleXY
				if (!parser.TryReadFloat(out float picAnchorPosY, ternimator: StringParser.Space))
				{
					picAnchorPosY = parser.ReadInt(ternimator: StringParser.Space);
				}

				var picAnchorPos = new Vector2(picAnchorPosX, picAnchorPosY);
				#endregion

				parser.Skip(StringParser.Space);

				#region <anchorX:float>:<anchorY:float>
				if (!parser.TryReadFloat(out float screenAnchorPosX, ternimator: StringParser.Colon))
				{
					screenAnchorPosX = parser.ReadInt(ternimator: StringParser.Colon);
				}
				if (!parser.TryReadFloat(out float screenAnchorPosY, ternimator: StringParser.Space))
				{
					screenAnchorPosY = parser.ReadInt(ternimator: StringParser.Space);
				}

				var screenAnchorPos = new Vector2(screenAnchorPosX, screenAnchorPosY);
				#endregion

				parser.Skip(StringParser.Space);

				#region <scaleX:float>:<scaleY:float>
				if (!parser.TryReadFloat(out float picScaleX, ternimator: StringParser.Colon))
				{
					picScaleX = parser.ReadInt(ternimator: StringParser.Colon);
				}
				if (!parser.TryReadFloat(out float picScaleY, ternimator: StringParser.Space))
				{
					picScaleY = parser.ReadInt(ternimator: StringParser.Space);
				}

				var picScaleRatio = new Vector2(picScaleX, picScaleY);
				#endregion

				parser.Skip(StringParser.Space);

				#region <transition:string>
				var transition = parser.ReadStringAsFuncArg(line: line, ternimator: StringParser.Space);
				#endregion

				parser.Skip(StringParser.Space);

				#region <superposition:string>
				string superposStr = parser.ReadString(ternimator: StringParser.Space);

				var superPosType = EnumsHelper.ParseByDescription<VNSuperPositionType>(description: superposStr);
				#endregion

				parser.Skip(StringParser.Space);

				#region [scale:string]
				bool isFreeScaleEnabled = false;
				if (!parser.EOFSpaceIgnored)
				{
					string freeScaleSign = parser.ReadString(ternimator: StringParser.Space).Trim().ToLower();
					isFreeScaleEnabled = freeScaleSign == "scale";
				}
				#endregion

				return new PictureShowCmd()
				{
					PicturePath = picPath,
					PictureAnchorPos = picAnchorPos,
					ScreenAnchorPos = screenAnchorPos,
					Scale = picScaleRatio,
					Transition = transition,
					SuperPosition = superPosType,
					EnableFreeScaling = isFreeScaleEnabled,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParsePictureHide(int line)
		{
			try
			{
				// hide <pic:stringB> [transition:string]
				parser.Skip(StringParser.Space);

				#region <pic:stringB>
				string picPath = parser.ReadQuotedString();
				#endregion

				parser.Skip(StringParser.Space);

				#region <transition:string>
				var transition = parser.ReadStringAsFuncArg(line: line, ternimator: StringParser.Space);
				#endregion

				return new PictureHideCmd()
				{
					PicturePath = picPath,
					Transition = transition,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParsePictureMove(int line)
		{
			try
			{
				// move <pic:stringB> <dx:int>:<dy:int> <duration:float> <curve:string>
				parser.Skip(StringParser.Space);

				#region <pic:stringB>
				string picPath = parser.ReadQuotedString();
				#endregion

				parser.Skip(StringParser.Space);

				#region <dx:int>:<dy:int>
				int dx = parser.ReadInt(ternimator: StringParser.Colon); // dx必须是int，因此这里不再添加fallback到float的逻辑
				int dy = parser.ReadInt(ternimator: StringParser.Space); // dy同理
				var movePosDelta = new Vector2(dx, dy);
				#endregion

				parser.Skip(StringParser.Space);

				#region <duration:float>
				if (!parser.TryReadFloat(out float duration, ternimator: StringParser.Space))
				{
					duration = parser.ReadInt(ternimator: StringParser.Space);
				}
				#endregion

				parser.Skip(StringParser.Space);

				#region <curve:string>
				string curveStr = parser.ReadString(ternimator: StringParser.Space);
				var curveType = EnumsHelper.ParseByDescription<VNCurveType>(description: curveStr);
				#endregion

				return new PictureMoveCmd()
				{
					PicturePath = picPath,
					MovePosDelta = movePosDelta,
					Duration = duration,
					Curve = curveType,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParseWait(int line)
		{
			try
			{
				// wait <duration:float>
				parser.Skip(StringParser.Space);

				#region <duration:float>
				if (!parser.TryReadFloat(out float duration, ternimator: StringParser.Space))
				{
					duration = parser.ReadInt(ternimator: StringParser.Space);
				}
				#endregion

				return new GeneralWaitCmd()
				{
					Duration = duration,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}
		#endregion

		#region Advanced Commands
		private ArcVNScriptCmdBase ParseTextShow(int line)
		{
			try
			{
				// say+ <speaker:stringB> <text:stringB> [fallback:string]
				parser.Skip(StringParser.Space);

				#region <speaker:stringB>
				string speaker = parser.ReadQuotedString();
				#endregion

				parser.Skip(StringParser.Space);

				#region <text:stringB>
				string text = parser.ReadQuotedString();
				#endregion

				parser.Skip(StringParser.Space);

				#region [fallback:string]
				bool isFallbackLegacy = false;
				if (!parser.EOFSpaceIgnored)
				{
					string fallbackSign = parser.ReadString(ternimator: StringParser.Space).ToLower().Trim();
					isFallbackLegacy = fallbackSign == "fallback";
				}
				#endregion

				return new TextShowCmd()
				{
					Speaker = speaker,
					Text = text,
					TreatAsFallbackLegacyText = isFallbackLegacy,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParseTextShowSettings(int line)
		{
			try
			{
				// say+set <speakerBgBoxImage:stringB> <msgBgBoxImage:stringB>
				parser.Skip(StringParser.Space);

				#region <speakerBgBoxImage:stringB>
				string speakerBgBoxPath = parser.ReadQuotedString();
				#endregion

				parser.Skip(StringParser.Space);

				#region <msgBgBoxImage:stringB>
				string msgBgBoxPath = parser.ReadQuotedString();
				#endregion

				return new TextShowSettingsCmd()
				{
					SpeakerBoxImagePath = speakerBgBoxPath,
					MessageBoxImagePath = msgBgBoxPath,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParseSubOptionItem(int line)
		{
			try
			{
				// select <shownCheck:stringB>, <id:string>, <action:string>, <gotoLabel:string> [, <requirement:string>];
				parser.Skip(StringParser.Space);

				#region <shownCheck:stringB>
				string shownCheck = parser.ReadQuotedString();
				#endregion

				parser.Skip(StringParser.Comma);
				parser.Skip(StringParser.Space);

				#region <id:string>
				string id = parser.ReadString(ternimator: StringParser.Comma);
				#endregion

				parser.Skip(StringParser.Comma);
				parser.Skip(StringParser.Space);

				#region <action:string>
				var action = parser.ReadStringAsFuncArg(line: line, ternimator: StringParser.Comma);
				#endregion

				parser.Skip(StringParser.Comma);
				parser.Skip(StringParser.Space);

				#region <gotoLabel:string>
				string gotoLabel = parser.ReadString();
				#endregion

				#region [, <requirement:string>]
				FuncArgument requirement = null;
				if (parser.Peek(1) == StringParser.Comma)
				{
					parser.Skip(StringParser.Comma);
					parser.Skip(StringParser.Space);
					
					requirement = parser.ReadStringAsFuncArg(line: line, ternimator: StringParser.Semicolon);
				}
				#endregion

				parser.Skip(StringParser.Semicolon);

				return new SubOptionItemCmd()
				{
					OptionId = id,
					ShownOptionText = shownCheck,
					OptionAction = action,
					GotoLabel = gotoLabel,
					SeletableRequirement = requirement,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParseBranchSelect(int line)
		{
			try
			{
				/* selectSay <selectionTip:stringB> <selectBtnImg:stringB> {
				 *		select <shownCheck:stringB>, <id:string>, <action:string>, <gotoLabel:string> [, <requirement:string>];
				 *		select <shownCheck:stringB>, <id:string>, <action:string>, <gotoLabel:string> [, <requirement:string>];
				 *		...
				 * }
				 */

				// a simplified format:
				// selectSay <selectionTip:stringB> <selectBtnImg:stringB> { <choice1:SubOptionItemCmd> <choice2:SubOptionItemCmd> ... }

				parser.Skip(StringParser.Space);

				#region <selectionTip:stringB>
				string selectionTip = parser.ReadQuotedString();
				#endregion

				parser.Skip(StringParser.Space);

				#region <selectBtnImg:stringB>
				string selectBtnImg = parser.ReadQuotedString();
				#endregion

				parser.Skip(StringParser.Space);

				parser.Skip(StringParser.LeftBrace);

				#region <choices:SubOptionItemCmd[]>
				var options = new List<SubOptionItemCmd>();
				while (parser.Peek(1) != StringParser.RightBrace)
				{
					parser.SkipWhitespaces();

					#region <choiceX:SubOptionItemCmd>
					var choice = ParseSubOptionItem(line);
					#endregion

					options.Add(choice as SubOptionItemCmd);

					parser.SkipWhitespaces();
				}
				#endregion

				return new BranchSelectCmd()
				{
					BranchSelectTip = selectionTip,
					OptionButtonImagePath = selectBtnImg,
					Options = options
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParseVarDefine(int line)
		{
			try
			{
				// var <varName:string> = <initValue:int>
				parser.SkipWhitespaces();

				#region <varName:string>
				string varName = parser.ReadString(ternimator: StringParser.Space);
				#endregion

				parser.SkipWhitespaces();
				parser.Skip(StringParser.Equal);
				parser.SkipWhitespaces();

				#region <initValue:int>
				int initValue = parser.ReadInt();
				#endregion

				return new VarDefineCmd()
				{
					VariableName = varName,
					InitialValue = initValue,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParseVarSet(int line)
		{
			try
			{
				// set <varName:string> = <newValue:int>
				parser.SkipWhitespaces();

				#region <varName:string>
				string varName = parser.ReadString(ternimator: StringParser.Space);
				#endregion

				parser.SkipWhitespaces();
				parser.Skip(StringParser.Equal);
				parser.SkipWhitespaces();

				#region <newValue:int>
				int newValue = parser.ReadInt();
				#endregion

				return new VarSetCmd()
				{
					TargetVariableName = varName,
					TargetValue = newValue,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParseIfGotoCheck(int line)
		{
			try
			{
				// if <condition:string> goto <targetLabel:string>
				parser.SkipWhitespaces();

				#region <condition:string>
				var condition = parser.ReadStringAsFuncArg(line: line);
				#endregion

				parser.SkipWhitespaces();
				parser.Skip("goto");
				parser.SkipWhitespaces();

				#region <targetLabel:string>
				string targetLabel = parser.ReadString(ternimator: StringParser.Space);
				#endregion

				return new IfGotoCheckCmd()
				{
					Condition = condition,
					GotoLabel = targetLabel,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}

		private ArcVNScriptCmdBase ParseLabelDefine(int line)
		{
			try
			{
				// LABEL <labelName:string>:
				// or
				// <labelName:string>: (check by ':'; most common type)
				parser.SkipWhitespaces();

				#region <labelName:string>
				string labelName = parser.ReadString(ternimator: StringParser.Colon);
				
				if (parser.Peek(1).Trim() != StringParser.Colon)
				{
					throw new ArcriptScriptParseException(tokenChar: parser.Peek(1)[0], line: line, column: parser.Pos + 1,
						$"{parser.Peek(1)} is a complete surprise to me");
				}
				#endregion
				
				parser.ReadString(StringParser.Colon);

				return new LabelDefineCmd()
				{
					LabelName = labelName,
				};
			}
			catch (ArcriptScriptParseException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ArcriptScriptParseException(tokenChar: parser.Current.Length > 0 ? parser.Current[0] : '\0', line: line, column: parser.Pos,
					innerException: ex);
			}
		}
		#endregion
		#endregion
	}
}