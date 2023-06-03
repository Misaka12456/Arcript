using UnityEngine;

namespace System.Enhance.Unity
{
	public static class DeviceInfo
	{
		/// <summary>
		/// 获取设备电池剩余电量的百分比。
		/// Get the remaining device battery level percentage.
		/// </summary>
		/// <returns>
		/// 以百分比为单位的设备剩余电量。<br />
		/// 若当前设备不支持获取电池信息则返回 <see langword="-1" /> 。
		/// <para>
		/// Remaining Device Battery level, in percentage.<br />
		/// Will return <see langword="-1" /> as the device does not support getting battery info.
		/// </para>
		/// </returns>
		public static int GetBatteryLevel()
		{
			float level = SystemInfo.batteryLevel;
			if (level == -1)
			{
				return -1;
			}
			else
			{
				return Convert.ToInt32(level * 100);
			}
		}
	}
}
