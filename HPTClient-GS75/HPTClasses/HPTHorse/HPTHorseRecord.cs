using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseRecord
    {
        [DataMember]
        public string Date { get; set; }

        [DataMember]
        public int Distance { get; set; }

        [DataMember]
        public int Place { get; set; }

        [DataMember]
        public int RaceNr { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string RecordType { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Time { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string TrackCode { get; set; }

        [DataMember]
        public bool Winner { get; set; }

        [XmlIgnore]
        public decimal TimeWeighed { get; set; }
    }
}
