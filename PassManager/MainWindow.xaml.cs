using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

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

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.DataGrid.SelectedIndex != -1)
                {
                    var item = (Credentials)this.DataGrid.Items[this.DataGrid.SelectedIndex];

                    _manager.RemoveCredentials(item);

                    RefreshList(_manager.CredentialsList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        private void btn_Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFolderDialog = new FolderBrowserDialog()
                {
                    ShowNewFolderButton = true,
                    SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                };

                if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (var stream = File.Create(Path.Combine(openFolderDialog.SelectedPath, "credentials.csv")))
                    {
                        stream.Position = 0;

                        using (var streamWriter = new StreamWriter(stream))
                        {
                            foreach (var creds in _manager.CredentialsList)
                            {
                                var contentBytes = Encoding.Unicode.GetBytes($"{creds.description};{creds.userName};{creds.password}");

                                streamWriter.WriteLine(Convert.ToBase64String(contentBytes));
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (var stream = File.Open(openFileDialog.FileName, FileMode.Open))
                    {
                        stream.Position = 0;

                        using (var streamReader = new StreamReader(stream))
                        {
                            while (!streamReader.EndOfStream)
                            {
                                string text = Encoding.Unicode.GetString(Convert.FromBase64String(streamReader.ReadLine()));

                                var values = text.Split(";");

                                _manager.AddCredentials(new Credentials(values[0], values[1], values[2]));
                            }

                            RefreshList(_manager.CredentialsList);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
