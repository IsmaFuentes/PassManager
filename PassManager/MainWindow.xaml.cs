using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
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

        private void Button_Add(object sender, RoutedEventArgs e)
        {
            // BLANK ROW WITH FOCUS
            _manager.AddCredentials(new Credentials(string.Empty, string.Empty, string.Empty));
            RefreshList(_manager.CredentialsList);

            DataGrid.SelectedItem = DataGrid.Items[DataGrid.Items.Count - 1];
        }

        private void Button_Export(object sender, RoutedEvent e)
        {
            // todo..
        }

        private void Button_Import(object sender, RoutedEvent e)
        {
            // todo..
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
