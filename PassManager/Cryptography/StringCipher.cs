using System;
using System.Text;
using System.Security.Cryptography;

namespace PassManager.Cryptography
{
    public static class StringCipher
    {
        public static string Encrypt(string text)
        {
            var encryptedBytes = ProtectedData.Protect(Encoding.Unicode.GetBytes(text), null, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string text)
        {
            var decryptedBytes = ProtectedData.Unprotect(Convert.FromBase64String(text), null, DataProtectionScope.CurrentUser);

            return Encoding.Unicode.GetString(decryptedBytes);
        }

    }
}
