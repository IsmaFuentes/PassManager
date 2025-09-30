using System;
using System.IO;
using System.Text;

#if WINDOWS
using System.Security.Cryptography;
#endif

namespace PassManager.MAUI.Helpers
{
  public static class Cipher
  {
    public static void Encrypt(string content, string outputFile)
    {
      if(string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
      {
        throw new ArgumentNullException(nameof(content));
      }

      if(string.IsNullOrEmpty(outputFile) || string.IsNullOrWhiteSpace(outputFile))
      {
        throw new ArgumentNullException(nameof(outputFile));
      }

      using(var stream = new FileStream(outputFile, FileMode.OpenOrCreate))
      {
        byte[] encryptedBytes = [];
#if WINDOWS
        encryptedBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(content), null, DataProtectionScope.CurrentUser);
#endif
        stream.Write(encryptedBytes, 0, encryptedBytes.Length);
      }
    }

    public static string Decrypt(string inputFile)
    {
      if(string.IsNullOrEmpty(inputFile) || string.IsNullOrWhiteSpace(inputFile))
      {
        throw new ArgumentNullException(nameof(inputFile));
      }

      if(!File.Exists(inputFile))
      {
        return string.Empty;
      }

      byte[] outputBytes = [];

#if WINDOWS
      outputBytes = ProtectedData.Unprotect(File.ReadAllBytes(inputFile), null, DataProtectionScope.CurrentUser);
#endif

      return Encoding.UTF8.GetString(outputBytes);
    }

    public static string EncryptString(string text)
    {
      byte[] encryptedBytes = [];

#if WINDOWS
      encryptedBytes = ProtectedData.Protect(Encoding.Unicode.GetBytes(text), null, DataProtectionScope.CurrentUser);
#endif

      return Convert.ToBase64String(encryptedBytes);
    }

    public static string DecryptString(string text)
    {
      byte[] decryptedBytes = [];

#if WINDOWS
      decryptedBytes = ProtectedData.Unprotect(Convert.FromBase64String(text), null, DataProtectionScope.CurrentUser);
#endif

      return Encoding.Unicode.GetString(decryptedBytes);
    }
  }
}
