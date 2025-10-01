using System.Collections.ObjectModel;
using PassManager.MAUI.Models;
using PassManager.MAUI.Services;

namespace PassManager.MAUI.ViewModels
{
  public interface ICredentialsViewModel
  {
    public ObservableCollection<PasswordCredential> DataSource { get; }
    public void Load();
    public Task Save();
    public void Insert();
    public void Remove(PasswordCredential item);
  }

  public class CredentialsViewModel : ICredentialsViewModel
  {
    private readonly IJsonParser _parser;

    public CredentialsViewModel(IJsonParser parser)
    {
      _parser = parser;
    }

    public void Load()
    {
      // System.IO.File.Delete(Path.Combine(FolderPath, "data.dat"));
      Directory.CreateDirectory(FolderPath);
      var credentials = _parser.Parse<List<PasswordCredential>>(Helpers.StringCipher.Decrypt(Path.Combine(FolderPath, "data.dat"))) ?? [];
      foreach(var item in credentials)
        DataSource.Add(item);
    }

    private string FolderPath
    {
      get
      {
#if WINDOWS
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PassManager");
#elif ANDROID
        return Path.Combine(FileSystem.AppDataDirectory, "PassManger");
#endif
        return string.Empty;
      }
    }

    public ObservableCollection<PasswordCredential> DataSource { get; private set; } = new();

    public async Task Save()
    {
      await Task.Run(() => Helpers.StringCipher.Encrypt(_parser.Stringify(DataSource), Path.Combine(FolderPath, "data.dat")));
    }

    public void Insert()
    {
      DataSource.Add(new PasswordCredential());
    }

    public void Remove(PasswordCredential item)
    {
      DataSource.Remove(item);
    }
  }
}
