using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;

namespace CoreLib.Crypter
{
    public class AesPortable
    {
        const string _aesKey = "testkey";
        const string _aesIV = "testiv";

        private UTF8Encoding _utf8Encoding;
        private AesManaged _aes;

        /// <summary>
        /// 암복호화 모듈 초기화
        /// </summary>
        /// <param name="key">A key string which is converted into UTF-8 and hashed by SHA256.
        /// Null or an empty string is not allowed.</param>
        /// <param name="initialVector">An initial vector string which is converted into UTF-8
        /// and hashed by SHA256. Null or an empty string is not allowed.</param>
        public AesPortable()
        {
            AesPortable_Init(_aesKey, _aesIV);
        }

        public AesPortable(string key, string initialVector)
        {
            AesPortable_Init(key, initialVector);
        }
        
        void AesPortable_Init(string key, string initialVector)
        {
            if (key == null || key == "")
                throw new ArgumentException("The key can not be null or an empty string.", "key");

            if (initialVector == null || initialVector == "")
                throw new ArgumentException("The initial vector can not be null or an empty string.", "initialVector");


            // This is an encoder which converts a string into a UTF-8 byte array.
            _utf8Encoding = new System.Text.UTF8Encoding();

            // Create a AES algorithm.
            _aes = new AesManaged();

            _aes.KeySize = 256;
            _aes.BlockSize = 128;

            // Initialize an encryption key and an initial vector.
            SHA256Managed sha256 = new SHA256Managed();
            _aes.Key = sha256.ComputeHash(_utf8Encoding.GetBytes(key));
            
            byte[] iv = sha256.ComputeHash(_utf8Encoding.GetBytes(initialVector));
            Array.Resize(ref iv, 16);
            _aes.IV = iv;
        }

        ~AesPortable()
        {
            _aes = null;

            _utf8Encoding = null;
        }

        /// <summary>
        /// 암호화 한다.
        /// </summary>
        /// <param name="message">암호화 대상 문자열</param>
        /// <returns></returns>
        public string Encrypt(string message)
        {
            // Get an encryptor interface.
            ICryptoTransform transform = _aes.CreateEncryptor();

            // Get a UTF-8 byte array from a unicode string.
            byte[] utf8Value = _utf8Encoding.GetBytes(message);

            // Encrypt the UTF-8 byte array.
            byte[] encryptedValue = transform.TransformFinalBlock(utf8Value, 0, utf8Value.Length);

            // Return a base64 encoded string of the encrypted byte array.
            return Convert.ToBase64String(encryptedValue);
        }

        /// <summary>
        /// 복호화한다.
        /// </summary>
        /// <param name="message">복호화 대상 문자열</param>
        /// <returns></returns>
        public string Decrypt(string message)
        {
            if (String.IsNullOrEmpty(message))
                throw new ArgumentException("복호화 대상 문자열이 존재 하지 않습니다.");

            // Get an decryptor interface.
            ICryptoTransform transform = _aes.CreateDecryptor();

            // Get an encrypted byte array from a base64 encoded string.
            byte[] encryptedValue = Convert.FromBase64String(message);

            // Decrypt the byte array.
            byte[] decryptedValue = transform.TransformFinalBlock(encryptedValue, 0, encryptedValue.Length);

            // Return a string converted from the UTF-8 byte array.
            return _utf8Encoding.GetString(decryptedValue, 0, decryptedValue.Length);
        }
    }
}
