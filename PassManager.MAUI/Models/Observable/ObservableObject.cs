using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PassManager.MAUI.Models.Observable
{
  public class ObservableObject : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;
    public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
