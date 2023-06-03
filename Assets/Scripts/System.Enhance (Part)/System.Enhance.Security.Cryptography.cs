using System.Text;

namespace System.Enhance.Security.Cryptography
{
	/// <summary>
	/// RC4加密算法类(RC4 Cryptography Helper)
	/// </summary>
	public static class AESCryptographyHelper
    {
        /// <summary>
        /// 使用RC4算法加密或解密指定数据。
        /// </summary>
        /// <param name="byte1">要加密或解密的数据。</param>
        /// <param name="str2">加密或解密使用的密钥内容。</param>
        /// <returns>加密或解密结果。</returns>
        public static byte[] Encrypt(byte[] byte1, string str2)
        {
            if (byte1 == null || str2 == null) return null;
            byte[] output = new byte[byte1.Length];
            long i = 0;
            long j = 0;
            byte[] mBox = GetKey(Encoding.UTF8.GetBytes(str2), 256);
            for (long offset = 0; offset < byte1.Length; offset++)
            {
                i = (i + 1) % mBox.Length;
                j = (j + mBox[i]) % mBox.Length;
                byte temp = mBox[i];
                mBox[i] = mBox[j];
                mBox[j] = temp;
                byte a = byte1[offset];
                byte b = mBox[(mBox[i] + mBox[j]) % mBox.Length];
                output[offset] = (byte)((int)a ^ (int)b);
            }
            return output;
        }

        /// <summary>
        /// 打乱密码
        /// </summary>
        /// <param name="pass">密码</param>
        /// <param name="kLen">密码箱长度</param>
        /// <returns>打乱后的密码</returns>
        private static byte[] GetKey(byte[] pass, int kLen)
        {
			byte[] mBox = new byte[kLen];
            for (long i = 0; i < kLen; i++)
            {
                mBox[i] = (byte)i;
            }
			long j = 0;
            for (long i = 0; i < kLen; i++)
            {
                j = (j + mBox[i] + pass[i % pass.Length]) % kLen;
				byte temp = mBox[i];
                mBox[i] = mBox[j];
                mBox[j] = temp;
            }
            return mBox;
        }
    }
}