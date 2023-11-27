using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
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

        private bool isEditing { get; set; }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (DataGrid.SelectedItems.Count > 0)
            {
                if(isEditing)
                {
                  DataGrid.CommitEdit(System.Windows.Controls.DataGridEditingUnit.Row, true);
                }

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

            isEditing = false;
        }

        private void DataGrid_BeginningEdit(object sender, System.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            isEditing = true;
        }

        private void RefreshList(List<Credentials> newCreds)
        {
            try
            {
                DataGrid.ItemsSource = newCreds;
                DataGrid.Items.Refresh();

                this.InvalidateVisual();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Create(object sender, RoutedEventArgs e)
        {
            if (isEditing)
            {
                return;
            }

            _manager.AddCredentials(new Credentials("description", "username", "password"));

            RefreshList(_manager.CredentialsList);

            // Last inserted item selection
            DataGrid.SelectedItem = DataGrid.Items[^1];
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
                System.Windows.MessageBox.Show(ex.Message);    
            }
        }

        private void Button_Export(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var openFolderDialog = new FolderBrowserDialog()
                {
                    ShowNewFolderButton = true,
                    SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                })
                {
                    if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _manager.ExportCredentials(openFolderDialog.SelectedPath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void Button_Import(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        using (var stream = File.Open(openFileDialog.FileName, FileMode.Open))
                        {
                            _manager.ImportCredentials(openFileDialog.FileName);
                        }

                        RefreshList(_manager.CredentialsList);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
