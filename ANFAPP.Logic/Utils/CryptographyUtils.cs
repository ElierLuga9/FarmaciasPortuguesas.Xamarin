
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ANFAPP.Logic.Utils
{
	public class CryptographyUtils
	{

		/// <summary>
		/// Encrypt the referenced text using RSA with the referenced public key.
		/// </summary>
		/// <param name="encryptionKey"></param>
		/// <param name="textToEncrypt"></param>
		/// <returns></returns>
		public static string RSAEncrypt(string textToEncrypt, string modulus, string exponent)
		{
			// Process public key
			RSAParameters rsaKey = new RSAParameters();
			rsaKey.Modulus = Encoding.Unicode.GetBytes(modulus);
			rsaKey.Exponent = Encoding.Unicode.GetBytes(exponent);

			// Import Public key
			var csp = new RSACryptoServiceProvider();
			csp.ImportParameters(rsaKey);

			// Encrypt text
			var textBytes = Encoding.Unicode.GetBytes(textToEncrypt);
			var encryptedBytes = csp.Encrypt(textBytes, false);

			// Convert the encrypted bytes into a Base64 string
			return Convert.ToBase64String(encryptedBytes);
		}

	}
}
