using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ApiJWT.Service
{

	public class CriptografiaService
	{
		private readonly byte[] _key;

		public CriptografiaService(string apiKey)
		{
			using var sha256 = SHA256.Create();
			_key = sha256.ComputeHash(Encoding.UTF8.GetBytes(apiKey)); // Deriva uma chave de 256 bits
		}
		
		public string Criptografar(string plainText)
		{
			using var aes = Aes.Create();
			aes.Key = _key;
			aes.GenerateIV(); // Gera um IV aleatório para cada criptografia

			using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
			using var ms = new MemoryStream();

			// Primeiro, grava o IV no início do stream
			ms.Write(aes.IV, 0, aes.IV.Length);

			using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
			using var sw = new StreamWriter(cs);

			sw.Write(plainText);
			sw.Close();

			return Convert.ToBase64String(ms.ToArray());
		}

		public string Descriptografar(string cipherText)
		{
			var cipherBytes = Convert.FromBase64String(cipherText);

			using var aes = Aes.Create();
			aes.Key = _key;

			using var ms = new MemoryStream(cipherBytes);

			// Lê o IV armazenado no início do texto criptografado
			byte[] iv = new byte[16];
			ms.Read(iv, 0, iv.Length);
			aes.IV = iv;

			using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
			using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
			using var sr = new StreamReader(cs);

			return sr.ReadToEnd();
		}
	}

}
