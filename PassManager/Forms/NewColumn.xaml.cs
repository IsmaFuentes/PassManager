using System.Windows;

namespace PassManager.Forms
{
    /// <summary>
    /// Lógica de interacción para NewColumn.xaml
    /// </summary>
    public partial class NewColumn : Window
    {
        private string _description;
        private string _username;
        private string _password;

        public bool isSaved;

        public NewColumn(Window parentWindow)
        {
            InitializeComponent();

            this.Owner = parentWindow;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public string Description => _description;
        public string UserName => _username;
        public string Password => _password;

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            isSaved = true;

            _description = description.Text;
            _username = userName.Text;
            _password = password.Text;

            this.Close();
        }
    }
}
