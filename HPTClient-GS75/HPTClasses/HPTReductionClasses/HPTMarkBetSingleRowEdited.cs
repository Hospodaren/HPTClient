using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTMarkBetSingleRowEdited
    {
        [DataMember]
        public string UniqueCode { get; set; }

        [DataMember]
        public bool V6 { get; set; }

        [DataMember]
        public int BetMultiplier { get; set; }
    }
}
