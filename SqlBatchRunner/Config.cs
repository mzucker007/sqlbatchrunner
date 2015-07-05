using System.Runtime.Serialization;

namespace SqlBatchRunner
{
    [DataContract]
    public class Config
    {
        [DataMember]
        public string ConnectionString { get; set; }

        [DataMember]
        public ConnectionStringPathAndAttribute[] ConnectionStringXmlSearch { get; set; }
    }

    [DataContract]
    public class ConnectionStringPathAndAttribute
    {
        [DataMember]
        public string NodePath { get; set; }

        [DataMember]
        public string Attribute { get; set; }
    }
}
