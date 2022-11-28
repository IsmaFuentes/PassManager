using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using PassManager.Helpers;

namespace PassManager.Models
{
    public class CredentialsManager
    {
        public List<Credentials> CredentialsList = new List<Credentials>();

        public CredentialsManager()
        {
            try
            {
                var store = Json.ReadSync<List<Credentials>>("store");

                if (store != null)
                {
                    CredentialsList.AddRange(store);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SaveJson() => Json.ModifyJsonSync("store", CredentialsList);

        public void AddCredentials(Credentials c)
        {
            CredentialsList.Add(c);

            SaveJson();
        }

        public void RemoveCredentials(Credentials c)
        {
            CredentialsList.Remove(c);

            SaveJson();
        }

        public void UpdateCredentials(Guid id, Credentials newValue)
        {
            Credentials oldValue = CredentialsList.Where(c => c._id.Equals(id)).FirstOrDefault();

            if (oldValue != null)
            {
                if(oldValue != newValue)
                {
                    CredentialsList[CredentialsList.IndexOf(oldValue)] = newValue;
                }

                SaveJson();
            }
        }

        public void ExportCredentials()
        {
            try
            {
                var openFolderDialog = new FolderBrowserDialog()
                {
                    ShowNewFolderButton = true,
                    SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                };

                if (openFolderDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = File.Create(Path.Combine(openFolderDialog.SelectedPath, "credentials.csv")))
                    {
                        stream.Position = 0;

                        using (var streamWriter = new StreamWriter(stream))
                        {
                            foreach (var creds in CredentialsList)
                            {
                                streamWriter.WriteLine($"{creds.description};{creds.userName};{creds.password}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ImportCredentials()
        {
            try
            {
                var openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = File.Open(openFileDialog.FileName, FileMode.Open))
                    {
                        stream.Position = 0;

                        using (var streamReader = new StreamReader(stream))
                        {
                            while (!streamReader.EndOfStream)
                            {
                                string text = streamReader.ReadLine();

                                var values = text.Split(";");

                                this.AddCredentials(new Credentials(values[0], values[1], values[2]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
