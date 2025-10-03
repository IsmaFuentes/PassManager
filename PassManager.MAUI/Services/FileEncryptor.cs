using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace PassManager.MAUI.Services
{
  public interface IFileEncryptor
  {
    public Task EncryptToFile(string content, string outputFile);
    public Task<string> DecryptFromFile(string inputFile);
#if ANDROID
    public Task DeleteStoredKey();
#endif
  }

  public class FileEncryptor : IFileEncryptor, IDisposable
  {
#if ANDROID
    private System.Threading.Timer _cleanupTimer;
    private DateTime _lastKeyUse = DateTime.Now;
    private string _storedKeyId => new(['P', 'a', 's', 's', 'K', 'e', 'y']);
    private byte[] _storedKey { get; set; } = [];
#endif

    public FileEncryptor() 
    {
#if ANDROID
      _cleanupTimer = new System.Threading.Timer((state) => 
      {
        try
        {
          var idleTime = DateTime.Now - _lastKeyUse;
          if (idleTime > TimeSpan.FromMinutes(5) && _storedKey.Length > 0)
          {
            Array.Clear(_storedKey, 0, _storedKey.Length);
            _storedKey = [];
          }
        }
        catch { /* No debería de entrar.. */}
      }, null, 0, 60000); //--> Cada minuto
#endif
    }

    public void Dispose()
    {
#if ANDROID
      if(_storedKey.Length > 0)
      {
        Array.Clear(_storedKey, 0, _storedKey.Length);
        _storedKey = [];
      }

      _cleanupTimer?.Dispose();
#endif
    }

    public async Task EncryptToFile(string content, string outputFile)
    {
      if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
      {
        throw new ArgumentNullException(nameof(content));
      }

      if (string.IsNullOrEmpty(outputFile) || string.IsNullOrWhiteSpace(outputFile))
      {
        throw new ArgumentNullException(nameof(outputFile));
      }

      using (var stream = new FileStream(outputFile, FileMode.Create))
      {
#if WINDOWS
        var encryptedBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(content), null, DataProtectionScope.CurrentUser);
        await stream.WriteAsync(encryptedBytes, 0, encryptedBytes.Length);
#elif ANDROID
        using (var aes = Aes.Create())
        {
          aes.Key = await GetKey();
          aes.Mode = CipherMode.CBC;
          aes.Padding = PaddingMode.PKCS7;
          aes.GenerateIV();

          stream.Write(aes.IV, 0, aes.IV.Length);

          using (var encryptor = aes.CreateEncryptor())
          {
            using (var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
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
    }

    public async Task<string> DecryptFromFile(string inputFile)
    {
      if (string.IsNullOrEmpty(inputFile) || string.IsNullOrWhiteSpace(inputFile))
      {
        throw new ArgumentNullException(nameof(inputFile));
      }

      if (!File.Exists(inputFile))
      {
        return string.Empty;
      }

      byte[] encryptedBytes = [];
#if WINDOWS
      encryptedBytes = await File.ReadAllBytesAsync(inputFile);
      var outputBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
      return await Task.FromResult(Encoding.UTF8.GetString(outputBytes));
#elif ANDROID
      encryptedBytes = await System.IO.File.ReadAllBytesAsync(inputFile);
      using (var aes = Aes.Create())
      {
        aes.Key = await GetKey();
        aes.IV = [.. encryptedBytes.Take(16)];
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using (var decryptor = aes.CreateDecryptor())
        {
          using (var stream = new MemoryStream([.. encryptedBytes.Skip(16)]))
          {
            using (var cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
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

#if ANDROID
    private async Task<byte[]> GetKey()
    {
      _lastKeyUse = DateTime.Now;
      if (_storedKey.Length == 0)
        _storedKey = await GetStoredKey();

      return _storedKey;
    }

    private async Task<byte[]> GetStoredKey()
    {
      string? key = await SecureStorage.GetAsync(_storedKeyId);

      if (string.IsNullOrEmpty(key))
      {
        using (var aes = Aes.Create())
        {
          aes.KeySize = 256;
          aes.GenerateKey();
          key = Convert.ToBase64String(aes.Key);

          await SecureStorage.SetAsync(_storedKeyId, key);
        }
      }

      var keyBytes = Convert.FromBase64String(key);
      if (keyBytes.Length != 32)
        throw new CryptographicException("Invalid key length!");

      return keyBytes;
    }

    public async Task DeleteStoredKey()
    {
      await Task.Run(() => SecureStorage.Remove(_storedKeyId));
    }
#endif
  }
}
