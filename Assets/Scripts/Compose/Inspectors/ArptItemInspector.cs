using System.Enhance.Unity;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	public class ArptItemInspector : Singleton<ArptItemInspector>
	{
		[Header("File Info")]
		public Text labelFileName;
		public InputField inputFileTypeName;

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}

		public void SetFileInfo(string fileName, string typeName)
		{
			labelFileName.text = fileName;
			inputFileTypeName.text = typeName;
		}
	}
}
