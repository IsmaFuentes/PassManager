using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PassManager.Helpers
{
    public class Json
    {
        private static readonly string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PassManager");

        private static string GetFileName(string name)
        {
            return string.Concat(name, ".json");
        }

        public static async Task ModifyJsonAsync<T>(string name, T content)
        {
            string fullPath = Path.Combine(FolderPath, GetFileName(name));

            string fileContent = JsonConvert.SerializeObject(content, Formatting.Indented);

            await File.WriteAllTextAsync(fullPath, fileContent);
        }

        public static void ModifyJsonSync<T>(string name, T content)
        {
            string fullPath = Path.Combine(FolderPath, GetFileName(name));

            string fileContent = JsonConvert.SerializeObject(content, Formatting.Indented);

            File.WriteAllText(fullPath, fileContent);
        }

        public static async Task<T> ReadAsync<T>(string name)
        {
            string fullPath = Path.Combine(FolderPath, GetFileName(name));

            if (!File.Exists(fullPath))
            {
                File.Create(fullPath);

                return default;
            }

            var file = await File.ReadAllTextAsync(fullPath);

            return JsonConvert.DeserializeObject<T>(file);
        }

        public static T ReadSync<T>(string name)
        {
            string fullPath = Path.Combine(FolderPath, GetFileName(name));

            if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);
            
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath).Close();

                return default;
            }

            var file = File.ReadAllText(fullPath);

            return JsonConvert.DeserializeObject<T>(file);
        }

        #region deprecated

        [Obsolete("Use ModifyJsonAsync instead")]
        public static async Task WriteAsync<T>(string name, T content)
        {
            string fullPath = Path.Combine(FolderPath, GetFileName(name));

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            using (var fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var StreamWriter = new StreamWriter(fileStream))
                {
                    var fileContent = JsonConvert.SerializeObject(content, Formatting.Indented);

                    await StreamWriter.WriteAsync(fileContent);
                }
            }
        }

        [Obsolete("Use ModifyJsonSync instead")]
        public static void WriteSync<T>(string name, T content)
        {
            string fullPath = Path.Combine(FolderPath, GetFileName(name));

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            using (var fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var StreamWriter = new StreamWriter(fileStream))
                {
                    var fileContent = JsonConvert.SerializeObject(content, Formatting.Indented);

                    StreamWriter.Write(fileContent);
                }
            }
        }

        #endregion
    }
}
