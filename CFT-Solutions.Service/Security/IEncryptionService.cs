using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Service.Security
{
    public interface IEncryptionService
    {
        /// <summary>
        /// EncryptString
        /// </summary>
        /// <param name="plainSourceStringToEncrypt"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        string EncryptString(string plainSourceStringToEncrypt, string passPhrase);
        /// <summary>
        /// DecryptString
        /// </summary>
        /// <param name="base64StringToDecrypt"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        string DecryptString(string base64StringToDecrypt, string passphrase);

        /// <summary>
        /// Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        string CreateSaltKey(int size);

        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">{assword</param>
        /// <param name="saltkey">Salk key</param>
        /// <param name="passwordFormat">Password format (hash algorithm)</param>
        /// <returns>Password hash</returns>
        string CreatePasswordHash(string password, string saltkey, string passwordFormat = "SHA1");

        /// <summary>
        /// Encrypt text
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Encrypted text</returns>
        string EncryptText(string plainText, string encryptionPrivateKey = "");

        /// <summary>
        /// Decrypt text
        /// </summary>
        /// <param name="cipherText">Text to decrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Decrypted text</returns>
        string DecryptText(string cipherText, string encryptionPrivateKey = "");
    }
}
