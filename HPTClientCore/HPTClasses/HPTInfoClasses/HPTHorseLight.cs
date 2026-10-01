using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseLight : Notifier
    {
        [DataMember]
        public int LegNr { get; set; }

        [DataMember]
        public string LegNrString { get; set; }

        [DataMember]
        public int StartNr { get; set; }

        [DataMember]
        public string Name { get; set; }

        [XmlIgnore]
        public HPTHorse Horse { get; set; }
    }

    [DataContract]
    public class HPTHorseLightSelectable : HPTHorseLight
    {
        [XmlIgnore]
        public string GroupCode { get; set; }

        [DataMember]
        public bool Selectable
        {
            get;
            set
            {
                field = value;
                if (!field && Selected)
                {
                    Selected = false;
                }
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool Selected
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    [DataContract]
    public class HPTHorseLightAnalyzed : HPTHorseLight
    {
        [DataMember]
        public decimal RankvariableMean { get; set; }

        [DataMember]
        public decimal RankvariableSum { get; set; }

        [DataMember]
        public decimal RankvariableStDev { get; set; }
    }
}
