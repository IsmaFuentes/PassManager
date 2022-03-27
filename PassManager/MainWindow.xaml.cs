using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using PassManager.Forms;
using PassManager.Models;

namespace PassManager
{
    /// <summary>
    /// https://github.com/Kinnara/ModernWpf
    /// </summary>
    public partial class MainWindow : Window
    {
        private CredentialsManager _manager = new CredentialsManager();

        public MainWindow()
        {
            InitializeComponent();
            
            // DataGrid
            DataGrid.ItemsSource = _manager.CredentialsList;
        }

        private void RefreshList(List<Credentials> newCreds)
        {
            DataGrid.ItemsSource = newCreds;
            DataGrid.Items.Refresh();

            this.InvalidateVisual();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewColumn newColumnWindow = new(GetWindow(this));

            newColumnWindow.ShowDialog();

            if (newColumnWindow.isSaved)
            {
                string description = newColumnWindow.Description;
                string username = newColumnWindow.UserName;
                string password = newColumnWindow.Password;

                _manager.AddCredentials(new Credentials(description, username, password));

                RefreshList(_manager.CredentialsList);
            }
        }

        private void DataGrid_RowEditEnding(object sender, System.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            var newValue = (Credentials)e.Row.DataContext;

            if(newValue != null)
            {
                _manager.UpdateCredentials(newValue._id, newValue);
            }
        }
    }
}
