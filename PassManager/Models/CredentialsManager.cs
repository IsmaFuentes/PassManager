﻿using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace PassManager.Models
{
    public class CredentialsManager
    {
        private readonly string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PassManager");

        public CredentialsManager()
        {
            try
            {
                Directory.CreateDirectory(FolderPath);

                string decryptedString = Cryptography.Cipher.Decrypt(Path.Combine(FolderPath, "data.dat"));

                List<Credentials> store = Helpers.Json.Parse<List<Credentials>>(decryptedString);

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

        public List<Credentials> CredentialsList { get; private set; } = new List<Credentials>();

        private void SaveData()
        {
            string content = Helpers.Json.Stringify(CredentialsList);

            Cryptography.Cipher.Encrypt(content, Path.Combine(FolderPath, "data.dat"));
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

                        AddCredentials(new Credentials(values[0], values[1], values[2]));
                    }
                }
            }
        }
    }
}
