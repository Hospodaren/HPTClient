using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTRaceLight : Notifier
    {
        [DataMember]
        public int LegNr { get; set; }

        [DataMember]
        public string LegNrString { get; set; }

        [XmlIgnore]
        public HPTHorseLightSelectable SelectedHorse
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public List<HPTHorseLightSelectable> HorseList { get; set; }
    }
}
