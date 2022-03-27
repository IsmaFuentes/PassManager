using System;

namespace PassManager.Models
{
    public class Credentials
    {
        public Credentials(string description, string userName, string password)
        {
            _id = Guid.NewGuid();

            this.userName = userName;
            this.password = password;
            this.description = description;
        }

        public Guid _id { get; }
        public string description { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
    }
}
