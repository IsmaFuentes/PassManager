using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace PassManager.MAUI.Helpers
{
  public static class StringCipher
  {
    private static byte[] StoredKey = [];
    private static string KeyName => new(['P','a','s','s','K','e','y']);

    public static async Task Encrypt(string content, string outputFile)
    {
      if(string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
      {
        throw new ArgumentNullException(nameof(content));
      }

      if(string.IsNullOrEmpty(outputFile) || string.IsNullOrWhiteSpace(outputFile))
      {
        throw new ArgumentNullException(nameof(outputFile));
      }

      using (var stream = new FileStream(outputFile, FileMode.Create))
      {
#if WINDOWS
        var encryptedBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(content), null, DataProtectionScope.CurrentUser);
        stream.Write(encryptedBytes, 0, encryptedBytes.Length);
#elif ANDROID
        if (StoredKey.Length == 0)
        {
          StoredKey = await GetStoredKey();
        }

        using (var aes = Aes.Create())
        {
          aes.Key = StoredKey;
          aes.Mode = CipherMode.CBC;
          aes.Padding = PaddingMode.PKCS7;
          aes.GenerateIV();

          stream.Write(aes.IV, 0, aes.IV.Length);

          using (var encryptor = aes.CreateEncryptor()) 
          {
            using(var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
              using (var writer = new StreamWriter(cryptoStream)) 
              {
                writer.Write(content);
                await writer.FlushAsync();
              }
            }
          }
        }
#endif
      }

      await Task.CompletedTask;
    }

    public static async Task<string> Decrypt(string inputFile)
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
      return await Task.FromResult(Encoding.UTF8.GetString(outputBytes));
#elif ANDROID
      byte[] encryptedBytes = System.IO.File.ReadAllBytes(inputFile);

      if (StoredKey.Length == 0)
      {
        StoredKey = await GetStoredKey();
      }

      using (var aes = Aes.Create())
      {
        aes.Key = StoredKey;
        aes.IV = [..encryptedBytes.Take(16)];
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using (var decryptor = aes.CreateDecryptor()) 
        {
          using(var stream = new MemoryStream([..encryptedBytes.Skip(16)]))
          {
            using(var cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
            {
              using (var reader = new StreamReader(cryptoStream)) 
              {
                return reader.ReadToEnd();
              }
            }
          }
        }
      }
#endif
      return await Task.FromResult(string.Empty);
    }

    private static async Task<byte[]> GetStoredKey()
    {
      string? key = await SecureStorage.GetAsync(KeyName);

      if (string.IsNullOrEmpty(key))
      {
        using(var aes = Aes.Create())
        {
          aes.KeySize = 256;
          aes.GenerateKey();
          key = Convert.ToBase64String(aes.Key);

          await SecureStorage.SetAsync(KeyName, key);
        }
      }

      return Convert.FromBase64String(key);
    }

    private static async Task DeleteStoredKey()
    {
      await Task.Run(() => SecureStorage.Remove(KeyName));
    }
  }
}
