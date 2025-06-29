using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

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
                return this.horse;
            }
            set
            {
                this.horse = value;
                OnPropertyChanged("Horse");
            }
        }

        private HPTPrio prio;
        [DataMember]
        public HPTPrio Prio
        {
            get
            {
                return this.prio;
            }
            set
            {
                this.prio = value;
                OnPropertyChanged("Prio");
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
                if (this.Horse != null)
                {
                    this.Horse.HandlePrioChange(this);

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
                return this.selectable;
            }
            set
            {
                this.selectable = value;
                if (!value && this.Selected)
                {
                    this.Selected = false;
                }
                OnPropertyChanged("Selectable");
            }
        }
    }
}
