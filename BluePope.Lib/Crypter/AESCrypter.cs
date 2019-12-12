using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;

namespace BluePope.Lib.Crypter
{
    public class AESCrypter : IDisposable
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
        public AESCrypter() : this (_aesKey, _aesIV)
        {
        }

        public AESCrypter(string key, string initialVector)
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

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                    _aes?.Dispose();
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                _utf8Encoding = null;

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~AESCrypter()
        // {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
