using Newtonsoft.Json;

namespace PassManager.MAUI.Models
{
  public class PasswordCredential : Observable.ObservableObject
  {
    public PasswordCredential()
    {
      Id = Guid.NewGuid();
    }

    [JsonProperty("_id")]
    public Guid Id { get; set; }

    private string? _title;

    [JsonProperty("description")]
    public string Title 
    { 
      get => _title ?? string.Empty; 
      set
      {
        _title = value;
        NotifyPropertyChanged(nameof(Title));
      } 
    }

    private string? _userName;

    [JsonProperty("username")]
    public string UserName
    {
      get => _userName ?? string.Empty;
      set
      {
        _userName = value;
        NotifyPropertyChanged(nameof(UserName));
      }
    }

    private string? _password;

    [JsonProperty("password")]
    public string Password
    {
      get => _password ?? string.Empty;
      set
      {
        _password = value;
        NotifyPropertyChanged(nameof(Password));
      }
    }

    private bool _isPasswordVisible;

    [JsonIgnore]
    public bool IsPasswordVisible
    {
      get => _isPasswordVisible;
      set
      {
        _isPasswordVisible = value;
        NotifyPropertyChanged(nameof(IsPasswordVisible));
      }
    }

    public override string ToString() => $"{Title} {UserName}";
  }
}
