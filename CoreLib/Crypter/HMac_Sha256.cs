using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace CoreLib.Crypter
{
    public class HMacSha256
    {
        /// <summary>
        /// Hash the given string with sha256
        /// </summary>
        /// <param name="password">the string to hash</param> 

        /// <returns>The hex representation of the hash</returns>
        static string GetSha256(string password)
        {
            SHA256Managed crypt = new SHA256Managed();
            StringBuilder hash = new StringBuilder();
            
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(password), 0, Encoding.ASCII.GetByteCount(password));
            foreach (byte bit in crypto)
            {
                hash.Append(bit.ToString("x2"));
            }
            return hash.ToString();
        }


        /// <summary>
        /// XOR every byte in data with the specified byte
        /// </summary>
        /// <param name="data">the data to XOR</param>
        /// <param name="xor">the byte to XOR with</param>

        /// <returns>the XORed byte array</returns>
        static byte[] GetXOR(byte[] data, byte xor)
        {
            byte[] buffer = new Byte[data.Length];

            for (int i = 0; i < data.Length; i++)
                buffer[i] = Convert.ToByte(Convert.ToInt32(data[i]) ^ Convert.ToInt32(xor));

            return buffer;
        }

        /// <summary> 
        /// This function creates the proper HMAC-SHA256 response
        /// </summary>
        /// <param name="password">the password</param>
        /// <param name="challenge">the challenge</param >
        /// <returns>the hmac-sha256 response to send to InspIRCd</returns> 

        public static string GetHMac(string password, string challenge)
        {
            byte[] pass = Encoding.ASCII.GetBytes(password);
            string xor1 = Encoding.ASCII.GetString(GetXOR(pass, (byte)0x5C));
            string xor2 = Encoding.ASCII.GetString(GetXOR(pass, (byte)0x36));
            string sha2 = GetSha256(xor2 + challenge);
            string sha1 = GetSha256(xor1 + sha2);
            return sha1;
        }
    }
}
