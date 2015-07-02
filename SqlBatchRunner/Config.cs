using System.Runtime.Serialization;

namespace SqlBatchRunner
{
    [DataContract]
    public class Config
    {
        [DataMember]
        public string ConnectionString { get; set; }

        [DataMember]
        public string SettingsXMLPath { get; set; }
    }
}
