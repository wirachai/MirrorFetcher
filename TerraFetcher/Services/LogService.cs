using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TerraFetcher.Models;

namespace TerraFetcher.Services
{
    public class LogService
    {
        private const string filePath = "log.json";

        public List<LogModel> GetLogs()
        {
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<LogModel>>(text);
            }

            return new List<LogModel>();
        }

        public void WriteLogs(List<LogModel> logs)
        {
            var json = JsonConvert.SerializeObject(logs);
            File.WriteAllText(filePath, json);
        }
    }
}
