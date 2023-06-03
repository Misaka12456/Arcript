using UnityEngine;
using UnityEngine.UI;

namespace System.Enhance.Unity
{
	public class PositiveNumberHelper : MonoBehaviour
	{
		public InputField InputField;

		public bool IsZeroStartPreserve = false;

		private uint LastValue = 0;

		private void Start()
		{
			if (InputField != null && uint.TryParse(InputField.text, out uint r))
			{
				LastValue = r;
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (InputField != null)
			{
				if (IsZeroStartPreserve)
				{
					if (LastValue < 10 && InputField.text == "0" + LastValue.ToString()) return;
				}
				else
				{
					if (InputField.text == LastValue.ToString()) return;
				}
				if (uint.TryParse(InputField.text, out uint r))
				{
					LastValue = r;
					if (r < 10 && IsZeroStartPreserve)
					{
						InputField.text = $"0{r}";
					}
					else
					{
						InputField.text = LastValue.ToString();
					}
				}
				else if (string.IsNullOrWhiteSpace(InputField.text))
				{
					LastValue = 0;
					if (IsZeroStartPreserve)
					{
						InputField.text = "00";
					}
					else
					{
						InputField.text = "0";
					}
				}
				InputField.text = LastValue.ToString();
			}
		}
	}
}