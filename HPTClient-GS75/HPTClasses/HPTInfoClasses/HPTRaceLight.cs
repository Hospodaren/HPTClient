using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

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
