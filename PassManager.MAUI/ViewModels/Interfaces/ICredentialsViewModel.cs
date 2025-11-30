using System.Collections.ObjectModel;

namespace PassManager.MAUI.ViewModels.Interfaces
{
  public interface ICredentialsViewModel
  {
    public ObservableCollection<Models.PasswordCredential> DataSource { get; }
    public Task Load();
    public Task Save();
    public void Insert();
    public void Remove(Models.PasswordCredential item);
    public void DeleteRepository();
  }
}
