using System;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.MovePicture)]
	public class AsptPictureMoveCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.MovePicture;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "ImagePath")]
		public string ImagePath { get; set; }

		[YamlMember(Alias = "MoveDelta")]
		public Vector2 MoveDelta { get; set; }

		[YamlMember(Alias = "Curve", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public CurveType Curve { get; set; } = CurveType.Linear;

		[YamlMember(Alias = "Duration")]
		public float Duration { get; set; } = 0f; // 0s == immediately move (looks like 'flash')

		#region Events (Optional)
		[YamlIgnore]
		public override AsptCmdBase[] EventBeforeExec { get => EventBeforeMove; set => EventBeforeMove = value; }

		[YamlIgnore]
		public override AsptCmdBase[] EventAfterExec { get => EventAfterMove; set => EventAfterMove = value; }

		[YamlMember(Alias = "EBeforeMove", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public AsptCmdBase[] EventBeforeMove { get; set; } = Array.Empty<AsptCmdBase>();

		[YamlMember(Alias = "EAfterMove", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public AsptCmdBase[] EventAfterMove { get; set; } = Array.Empty<AsptCmdBase>();
		#endregion
	}
}
