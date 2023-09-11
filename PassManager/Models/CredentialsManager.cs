using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using PassManager.Helpers;
using PassManager.Cryptography;
using System.Windows;

namespace PassManager.Models
{
    public class CredentialsManager
    {
        private readonly string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PassManager");

        public List<Credentials> CredentialsList = new List<Credentials>();

        public CredentialsManager()
        {
            try
            {
                string decryptedString = Cipher.Decrypt(Path.Combine(FolderPath, "data.dat"));

                List<Credentials> store = Json.Parse<List<Credentials>>(decryptedString);

                if (store != null)
                {
                    CredentialsList.AddRange(store);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveData()
        {
            string content = Json.Stringify(CredentialsList);

            Cipher.Encrypt(content, Path.Combine(FolderPath, "data.dat"));
        }

        public void AddCredentials(Credentials c)
        {
            CredentialsList.Add(c);

            SaveData();
        }

        public void RemoveCredentials(Credentials c)
        {
            CredentialsList.Remove(c);

            SaveData();
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

                SaveData();
            }
        }

        public void ExportCredentials(string path)
        {
            using (var stream = File.Create(Path.Combine(path, "credentials.csv")))
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

        public void ImportCredentials(string path)
        {
            using (var stream = File.Open(path, FileMode.Open))
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
}
