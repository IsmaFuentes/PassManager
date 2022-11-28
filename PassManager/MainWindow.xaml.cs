using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
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

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (DataGrid.SelectedItems.Count > 0)
            {
                // Unselect everything when clicking outside the grid
                DataGrid.UnselectAll();
                DataGrid.UnselectAllCells();
                DataGrid.SelectedItem = null;
            }
        }

        private void DataGrid_RowEditEnding(object sender, System.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            var newValue = (Credentials)e.Row.DataContext;

            if (newValue != null)
            {
                _manager.UpdateCredentials(newValue._id, newValue);
            }
        }

        private void RefreshList(List<Credentials> newCreds)
        {
            DataGrid.ItemsSource = newCreds;
            DataGrid.Items.Refresh();

            this.InvalidateVisual();
        }

        private void Button_Create(object sender, RoutedEventArgs e)
        {
            // todo: Reimplementar
            _manager.AddCredentials(new Credentials(string.Empty, string.Empty, string.Empty));

            RefreshList(_manager.CredentialsList);

            DataGrid.SelectedItem = DataGrid.Items[DataGrid.Items.Count - 1];
        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                if(this.DataGrid.SelectedItems.Count > 0)
                {
                    foreach(var item in DataGrid.SelectedItems)
                    {
                        _manager.RemoveCredentials((Credentials)item);
                    }

                    RefreshList(_manager.CredentialsList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Button_Export(object sender, RoutedEventArgs e)
        {
            _manager.ExportCredentials();
        }

        private void Button_Import(object sender, RoutedEventArgs e)
        {
            _manager.ImportCredentials();

            RefreshList(_manager.CredentialsList);
        }
    }
}
