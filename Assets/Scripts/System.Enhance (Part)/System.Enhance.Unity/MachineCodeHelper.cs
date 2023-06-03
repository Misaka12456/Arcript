using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace System.Enhance.Unity
{
	public static class MachineCodeHelper
	{
		public static string GetMachineCode()
		{
			string machineCode = SystemInfo.deviceUniqueIdentifier.ToString();
			return machineCode.CalculateMD5();
		}

		private static string CalculateMD5(this string dataStr)
		{
			var md5 = MD5.Create();
			byte[] r = md5.ComputeHash(Encoding.UTF8.GetBytes(dataStr));
			return BitConverter.ToString(r).Replace("-",string.Empty).ToLower();
		}
	}
}
