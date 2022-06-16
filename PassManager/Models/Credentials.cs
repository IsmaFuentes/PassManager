using System;
using Newtonsoft.Json;
using PassManager.Cryptography;

namespace PassManager.Models
{
    public class Credentials
    {
        public Credentials(string description, string userName, string password)
        {
            _id = Guid.NewGuid();

            this.userName = userName;
            this.storedPassword = !string.IsNullOrEmpty(password) ? StringCipher.Encrypt(password) : password;
            this.description = description;
        }

        public Guid _id { get; }
        public string description { get; set; }
        public string userName { get; set; }
        public string storedPassword { get; set; }

        [JsonIgnore]
        public string password
        {
            get => !string.IsNullOrEmpty(storedPassword) ? StringCipher.Decrypt(storedPassword) : storedPassword;
            set
            {
                if(!string.IsNullOrEmpty(value))
                {
                    value = StringCipher.Encrypt(value);
                }

                storedPassword = value;
            }
        }
    }
}
