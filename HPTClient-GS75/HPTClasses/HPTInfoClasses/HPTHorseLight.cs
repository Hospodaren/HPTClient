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

        private bool selectable;
        [DataMember]
        public bool Selectable
        {
            get
            {
                return this.selectable;
            }
            set
            {
                this.selectable = value;
                if (!this.selectable && this.Selected)
                {
                    this.Selected = false;
                }
                OnPropertyChanged("Selectable");
            }
        }

        private bool selected;
        [DataMember]
        public bool Selected
        {
            get
            {
                return this.selected;
            }
            set
            {
                this.selected = value;
                OnPropertyChanged("Selected");
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
