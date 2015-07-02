using System;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Runtime.Serialization.Json;

namespace SqlBatchRunner
{
    public class ConfigScanner
    {
        private string xmlFile;

        public ConfigScanner(string xmlFileName)
        {
            this.xmlFile = xmlFileName;
        }

        private void ProcessSingleDirectory(string directoryName)
        {
            var filename = Path.Combine(directoryName, "config.json");
            if (File.Exists(filename))
            {
                var connectionString = GetConnectionString(directoryName);

                var runner = new SqlRunner(connectionString);
                runner.Run(directoryName);
            }
        }

        public void ProcessDirectory(string directoryName)
        {
            ProcessSingleDirectory(directoryName);

            foreach (var dir in Directory.EnumerateDirectories(directoryName, "*", SearchOption.AllDirectories))
            {
                ProcessSingleDirectory(dir);
            }
        }

        Config GetConfig(string dirName)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Config));

            var stream = File.OpenRead(Path.Combine(dirName, "config.json"));
            var config = (Config)serializer.ReadObject(stream);

            return config;
        }

        public string GetConnectionString(string dirName)
        {
            var config = GetConfig(dirName);

            if (!string.IsNullOrWhiteSpace(config.SettingsXMLPath))
            {
                return GetConnectionStringFromXML(xmlFile, config.SettingsXMLPath);
            }
            else
            {
                return config.ConnectionString;
            }
        }

        public string GetConnectionStringFromXML(string xmlFile, string parameterName)
        {
            var xml = XDocument.Load(xmlFile);

            var node = xml.Descendants("setParameter").Where(n => n.Attribute("name").Value.Equals(parameterName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            return node.Attribute("value").Value;
        }
    }
}
