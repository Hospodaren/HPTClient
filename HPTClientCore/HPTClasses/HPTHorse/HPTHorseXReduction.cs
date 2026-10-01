using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseXReduction : Notifier
    {
        [XmlIgnore]
        public HPTHorse Horse
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public HPTPrio Prio
        {
            get;
            set
            {
                field = value;
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
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool Selectable
        {
            get;
            set
            {
                field = value;
                if (!value && Selected)
                {
                    Selected = false;
                }
                OnPropertyChanged();
            }
        }
    }
}
