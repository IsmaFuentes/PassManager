using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace PassManager.MAUI.Helpers
{
  public static class StringCipher
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
#if WINDOWS
        var encryptedBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(content), null, DataProtectionScope.CurrentUser);
        stream.Write(encryptedBytes, 0, encryptedBytes.Length);
#elif ANDROID

#endif
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

#if WINDOWS
      var outputBytes = ProtectedData.Unprotect(File.ReadAllBytes(inputFile), null, DataProtectionScope.CurrentUser);
      return Encoding.UTF8.GetString(outputBytes);
#elif ANDROID
      
#endif
      return string.Empty;
    }
  }
}
