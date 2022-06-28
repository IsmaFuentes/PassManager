using System;
using System.Linq;
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
    }
}
