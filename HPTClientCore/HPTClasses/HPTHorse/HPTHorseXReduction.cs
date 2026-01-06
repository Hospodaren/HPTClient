using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseXReduction : Notifier
    {
        private HPTHorse horse;
        [XmlIgnore]
        public HPTHorse Horse
        {
            get
            {
                return horse;
            }
            set
            {
                horse = value;
                OnPropertyChanged("Horse");
            }
        }

        private HPTPrio prio;
        [DataMember]
        public HPTPrio Prio
        {
            get
            {
                return prio;
            }
            set
            {
                prio = value;
                OnPropertyChanged("Prio");
            }
        }

        private bool selected;
        [DataMember]
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                if (Horse != null)
                {
                    Horse.HandlePrioChange(this);

                    //if (value)
                    //{
                    //    this.Horse.Prio = this.Prio;
                    //    this.Horse.PrioString = this.Prio.ToString();
                    //    if (!this.Horse.Selected)
                    //    {
                    //        this.Horse.Selected = true;
                    //    }
                    //    else if (this.horse.ParentRace != null && this.horse.ParentRace.ParentRaceDayInfo != null)
                    //    {
                    //        this.horse.ParentRace.ParentRaceDayInfo.ActivateABCDChanged();
                    //    }
                    //}
                }
                OnPropertyChanged("Selected");
            }
        }

        private bool selectable;
        [DataMember]
        public bool Selectable
        {
            get
            {
                return selectable;
            }
            set
            {
                selectable = value;
                if (!value && Selected)
                {
                    Selected = false;
                }
                OnPropertyChanged("Selectable");
            }
        }
    }
}
