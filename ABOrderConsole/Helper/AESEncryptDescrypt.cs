using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ABOrderConsole.Helper
{
    public class AESEncryptDescrypt
    {
        //public static byte[] EncryptAesManaged(string original)
        //{
        //    byte[] encrypted = null;
        //    try
        //    {
        //        using (Aes myAes = Aes.Create())
        //        {
        //            var key = System.Convert.FromBase64String(System.Configuration.ConfigurationManager.AppSettings["AESKey"]);
        //            var iv = System.Convert.FromBase64String(System.Configuration.ConfigurationManager.AppSettings["AESIv"]);

        //            myAes.Key = key;
        //            myAes.IV = iv;
        //            //string key = System.Text.Encoding.UTF8.GetString(myAes.Key);
        //            //string IV = System.Text.Encoding.UTF8.GetString(myAes.IV);

        //            //var keyString = System.Convert.ToBase64String(myAes.Key, 0, myAes.Key.Length);
        //            //var ivString = System.Convert.ToBase64String(myAes.IV, 0, myAes.IV.Length);
                    
        //            //var ivBytes = System.Convert.FromBase64String(ivString);

        //            // Encrypt the string to an array of bytes.
        //            encrypted = EncryptStringToBytes_Aes(original, myAes.Key, myAes.IV);

        //            // Decrypt the bytes to a string.
        //            string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);
        //            //byte[] keyTobyte = key.Split(new[] { ',' }).Select(s => Convert.ToByte(s, 32))
        //            //                 .ToArray();
        //            //byte[] ivto = IV.Split(new[] { ',' }).Select(s => Convert.ToByte(s, 32))
        //            //                 .ToArray();
        //            //string det = DecryptStringFromBytes_Aes(encrypted, keyBytes, ivBytes);

        //            //if (roundtrip == det)
        //            //{
        //            //    var res = true;
        //            //}
        //            //Display the original data and the decrypted data.
        //            //Console.WriteLine("Original:   {0}", original);
        //            //Console.WriteLine("Round Trip: {0}", roundtrip);
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        Console.WriteLine(exp.Message);
        //    }

        //    return encrypted;
        //    //Console.ReadKey();
        //}

        //public static string DecryptAesManaged(byte[] encryptedText)
        //{
        //    string decryptedText = null;
        //    try
        //    {
        //        using (Aes myAes = Aes.Create())
        //        {
        //            var key = System.Convert.FromBase64String(System.Configuration.ConfigurationManager.AppSettings["AESKey"]);
        //            var iv = System.Convert.FromBase64String(System.Configuration.ConfigurationManager.AppSettings["AESIv"]);

        //            myAes.Key = key;
        //            myAes.IV = iv;
        //            //string key = System.Text.Encoding.UTF8.GetString(myAes.Key);
        //            //string IV = System.Text.Encoding.UTF8.GetString(myAes.IV);

        //            //var keyString = System.Convert.ToBase64String(myAes.Key, 0, myAes.Key.Length);
        //            //var ivString = System.Convert.ToBase64String(myAes.IV, 0, myAes.IV.Length);

        //            //var ivBytes = System.Convert.FromBase64String(ivString);

        //            // Encrypt the string to an array of bytes.
        //            //encrypted = EncryptStringToBytes_Aes(original, myAes.Key, myAes.IV);

        //            // Decrypt the bytes to a string.
        //            decryptedText = DecryptStringFromBytes_Aes(encryptedText, myAes.Key, myAes.IV);
        //            //byte[] keyTobyte = key.Split(new[] { ',' }).Select(s => Convert.ToByte(s, 32))
        //            //                 .ToArray();
        //            //byte[] ivto = IV.Split(new[] { ',' }).Select(s => Convert.ToByte(s, 32))
        //            //                 .ToArray();
        //            //string det = DecryptStringFromBytes_Aes(encrypted, keyBytes, ivBytes);

        //            //if (roundtrip == det)
        //            //{
        //            //    var res = true;
        //            //}
        //            //Display the original data and the decrypted data.
        //            //Console.WriteLine("Original:   {0}", original);
        //            //Console.WriteLine("Round Trip: {0}", roundtrip);
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        Console.WriteLine(exp.Message);
        //    }

        //    return decryptedText;
        //}

        //static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        //{
        //    // Check arguments.
        //    if (plainText == null || plainText.Length <= 0)
        //        throw new ArgumentNullException("plainText");
        //    if (Key == null || Key.Length <= 0)
        //        throw new ArgumentNullException("Key");
        //    if (IV == null || IV.Length <= 0)
        //        throw new ArgumentNullException("IV");
        //    byte[] encrypted;

        //    // Create an Aes object
        //    // with the specified key and IV.
        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        aesAlg.Key = Key;
        //        aesAlg.IV = IV;

        //        // Create an encryptor to perform the stream transform.
        //        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        //        // Create the streams used for encryption.
        //        using (MemoryStream msEncrypt = new MemoryStream())
        //        {
        //            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        //                {
        //                    //Write all data to the stream.
        //                    swEncrypt.Write(plainText);
        //                }
        //                encrypted = msEncrypt.ToArray();
        //            }
        //        }
        //    }

        //    // Return the encrypted bytes from the memory stream.
        //    return encrypted;
        //}

        //static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        //{
        //    // Check arguments.
        //    if (cipherText == null || cipherText.Length <= 0)
        //        throw new ArgumentNullException("cipherText");
        //    if (Key == null || Key.Length <= 0)
        //        throw new ArgumentNullException("Key");
        //    if (IV == null || IV.Length <= 0)
        //        throw new ArgumentNullException("IV");

        //    // Declare the string used to hold
        //    // the decrypted text.
        //    string plaintext = null;

        //    // Create an Aes object
        //    // with the specified key and IV.
        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        aesAlg.Key = Key;
        //        aesAlg.IV = IV;

        //        // Create a decryptor to perform the stream transform.
        //        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        //        // Create the streams used for decryption.
        //        using (MemoryStream msDecrypt = new MemoryStream(cipherText))
        //        {
        //            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
        //                {

        //                    // Read the decrypted bytes from the decrypting stream
        //                    // and place them in a string.
        //                    plaintext = srDecrypt.ReadToEnd();
        //                }
        //            }
        //        }
        //    }

        //    return plaintext;
        //}
    }
}
