using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace PassManager.Cryptography
{
    // https://learn.microsoft.com/en-us/dotnet/standard/security/how-to-use-data-protection
    public static class Cipher
    {
        public static void Encrypt(string content, string outputFile)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (string.IsNullOrEmpty(outputFile) || string.IsNullOrWhiteSpace(outputFile))
            {
                throw new ArgumentNullException(nameof(outputFile));
            }

            using (var stream = new FileStream(outputFile, FileMode.OpenOrCreate))
            {
                var encryptedBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(content), null, DataProtectionScope.CurrentUser);

                stream.Write(encryptedBytes, 0, encryptedBytes.Length);
            }
        }

        public static string Decrypt(string inputFile)
        {
            if (string.IsNullOrEmpty(inputFile) || string.IsNullOrWhiteSpace(inputFile))
            {
                throw new ArgumentNullException(nameof(inputFile));
            }

            if (!File.Exists(inputFile))
            {
                return string.Empty;
            }

            byte[] outputBytes = ProtectedData.Unprotect(File.ReadAllBytes(inputFile), null, DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(outputBytes);
        }

        public static string EncryptString(string text)
        {
            var encryptedBytes = ProtectedData.Protect(Encoding.Unicode.GetBytes(text), null, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string DecryptString(string text)
        {
            var decryptedBytes = ProtectedData.Unprotect(Convert.FromBase64String(text), null, DataProtectionScope.CurrentUser);

            return Encoding.Unicode.GetString(decryptedBytes);
        }
    }
}
