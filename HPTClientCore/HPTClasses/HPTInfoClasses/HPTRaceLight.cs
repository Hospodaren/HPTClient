using System.Collections.Generic;
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

        private HPTHorseLightSelectable selectedHorse;
        [XmlIgnore]
        public HPTHorseLightSelectable SelectedHorse
        {
            get
            {
                return this.selectedHorse;
            }
            set
            {
                this.selectedHorse = value;
                OnPropertyChanged("SelectedHorse");
            }
        }

        [DataMember]
        public List<HPTHorseLightSelectable> HorseList { get; set; }
    }
}
