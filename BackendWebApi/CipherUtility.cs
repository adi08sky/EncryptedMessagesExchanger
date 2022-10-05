using System;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace BackendWebApi
{
    public static class CipherUtility
    {
        public static string Encrypt(ref byte[] key, string massage)
        {
            try
            {
                byte[] encrypted;
                using (MemoryStream memStream = new MemoryStream())
                {
                    using (Aes aes = Aes.Create())
                    {
                        key = aes.Key;
                        byte[] iv = aes.IV;
                        memStream.Write(iv, 0, iv.Length);

                        using (CryptoStream cryptoStream = new(
                                   memStream,
                                   aes.CreateEncryptor(),
                                   CryptoStreamMode.Write))
                        {
                            using (StreamWriter encryptWriter = new(cryptoStream))
                            {
                                encryptWriter.Write(massage);
                            }
                            encrypted = memStream.ToArray();
                        }
                    }
                }
                string result = Convert.ToBase64String(encrypted);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static string Decrypt(string keyString, string encryptedStream)
        {
            try
            {
                byte[] key = Convert.FromBase64String(keyString);
                byte[] cipherText = Convert.FromBase64String(encryptedStream);
                string decryptedMessage = null;
                using (MemoryStream fileStream = new MemoryStream(cipherText))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] iv = new byte[aes.IV.Length];
                        int numBytesToRead = aes.IV.Length;
                        int numBytesRead = 0;
                        while (numBytesToRead > 0)
                        {
                            int n = fileStream.Read(iv, numBytesRead, numBytesToRead);
                            if (n == 0) break;

                            numBytesRead += n;
                            numBytesToRead -= n;
                        }

                        using (CryptoStream cryptoStream = new(
                                   fileStream,
                                   aes.CreateDecryptor(key, iv),
                                   CryptoStreamMode.Read))
                        {
                            using (StreamReader decryptReader = new(cryptoStream))
                            {
                                decryptedMessage = decryptReader.ReadToEnd();
                            }
                        }
                    }
                }
                return decryptedMessage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}