using Arcript.Aspt;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	public abstract class InspectCmdPanelBase<T> : InpectInfoPanelBase where T : AsptCmdBase
	{
		public override GameObject TemplatePrefab { get; }

		[Header("UI")]
		public Toggle toggleIsBlock;

		[HideInInspector] internal ArptScriptCmdItem parentItem;
		[HideInInspector] internal T cmd;

		protected virtual void Awake()
		{
			toggleIsBlock.onValueChanged.AddListener((value) =>
			{
				if (cmd == null) return;
				cmd.IsBlock = value;
				Apply();
			});
			InspectorAwake();
		}

		protected virtual void InspectorAwake()
		{

		}
		
		public override void SetInfo(string fileName, string typeName) => throw new NotSupportedException("Can't set file info to a command panel.");

		public virtual void SetInfo<C>(C command, ArptScriptCmdItem parentItem) where C : AsptCmdBase
		{
			// 获取传入的T的AsptCmdAttribute
			var cmdType = typeof(C);
			var attrs = cmdType.GetCustomAttributes(typeof(AsptCmdAttribute), false);
			if (attrs.Length == 0)
			{
				throw new NotSupportedException($"Cannot find AsptCmdAttribute on type {cmdType.Name}.");
			}
			var cmdAttr = attrs[0] as AsptCmdAttribute;

			// 获取继承本类的类的CmdInspectExportAttribute
			var subCmdType = GetType();
			var subAttrs = subCmdType.GetCustomAttributes(typeof(CmdInspectExportAttribute), false);
			if (subAttrs.Length == 0)
			{
				throw new NotSupportedException($"Cannot find CmdInspectExportAttribute on current type {subCmdType.Name}.");
			}

			var exportAttr = subAttrs[0] as CmdInspectExportAttribute;
			var modelType = exportAttr.CmdModelType;

			// 获取modelType的AsptCmdAttribute
			var modelAttrs = modelType.GetCustomAttributes(typeof(AsptCmdAttribute), false);
			if (modelAttrs.Length == 0)
			{
				throw new NotSupportedException($"Cannot find AsptCmdAttribute on model type {modelType.Name}.");
			}
			var modelAttr = modelAttrs[0] as AsptCmdAttribute;

			if (modelAttr.CmdType != cmdAttr.CmdType)
			{
				throw new NotSupportedException($"Command type mismatch. Supported command type is {modelAttr.CmdType}, but got {cmdAttr.CmdType}.");
			}

			this.parentItem = parentItem;

			// 将command转换为modelType
			cmd = command as T;

			toggleIsBlock.SetIsOnWithoutNotify(cmd.IsBlock);

			// 如果本类的继承类没有重写SetInfo<C>则输出一条警告
			if (GetType().GetMethod("SetInfo").DeclaringType == typeof(InspectCmdPanelBase<T>))
			{
				Debug.LogWarning($"The method SetInfo<C> of type {GetType().Name} is not overrided. This usually means you forgot to override it.\n" +
								  "The basic implementation of SetInfo<C> is only validate the command type and set the very-basic block status.\n" +
								  "You need to override it with base.SetInfo() to set the main inspector panel data.");
			}
		}

		public abstract void Apply(object tag = null);

		public abstract void InitNewInfo();
	}
}
