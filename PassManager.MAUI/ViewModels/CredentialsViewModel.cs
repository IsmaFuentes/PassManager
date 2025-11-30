using PassManager.MAUI.Models;
using PassManager.MAUI.Services;
using System.Collections.ObjectModel;

namespace PassManager.MAUI.ViewModels
{
  public class CredentialsViewModel : Interfaces.ICredentialsViewModel
  {
    private readonly IJsonParser _parser;
    private readonly IFileEncryptor _encryptor;

    public CredentialsViewModel(IJsonParser parser, IFileEncryptor encryptor)
    {
      _parser = parser;
      _encryptor = encryptor;
    }

    public async Task Load()
    {
      DataSource.Clear();
      // System.IO.File.Delete(Path.Combine(FolderPath, "data.dat"));
      Directory.CreateDirectory(FolderPath);
      string decryptedString = await _encryptor.DecryptFromFile(RepositoryPath);
      var credentials = _parser.Parse<List<PasswordCredential>>(decryptedString) ?? [];
      foreach(var item in credentials)
      {
        DataSource.Add(item);
      }
    }

    private string FolderPath
    {
      get
      {
#if WINDOWS
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PassManager");
#elif ANDROID
        return Path.Combine(FileSystem.AppDataDirectory, "PassManager");
#endif
        return string.Empty;
      }
    }

    private string RepositoryPath
    {
      get => Path.Combine(FolderPath, "data.dat");
    }

    public ObservableCollection<PasswordCredential> DataSource { get; private set; } = new();

    public async Task Save()
    {
      string json = _parser.Stringify(DataSource);
      await _encryptor.EncryptToFile(json, RepositoryPath);
    }

    public void Insert()
    {
      DataSource.Add(new PasswordCredential());
    }

    public void Remove(PasswordCredential item)
    {
      DataSource.Remove(item);
    }

    public void DeleteRepository()
    {
      if (System.IO.File.Exists(RepositoryPath))
      {
        System.IO.File.Delete(RepositoryPath);
      }

#if ANDROID
      //--- Nueva key
      _encryptor.DeleteStoredKey();
#endif
    }
  }
}
