using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTNumberOfWinners : Notifier
    {
        private int numberOfWinners;
        [DataMember]
        public int NumberOfWinners
        {
            get
            {
                return this.numberOfWinners;
            }
            set
            {
                this.numberOfWinners = value;
                OnPropertyChanged("NumberOfWinners");
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
                if (this.selected == value)
                {
                    return;
                }
                this.selected = value;
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
                if (!value && this.Selected)
                {
                    this.Selected = false;
                }
                if (this.selectable == value)
                {
                    return;
                }
                this.selectable = value;
                OnPropertyChanged("Selectable");
            }
        }

        private decimal probability;
        public decimal Probability
        {
            get
            {
                return this.probability;
            }
            set
            {
                this.probability = value;
                OnPropertyChanged("Probability");
            }
        }

        private bool isSuperfluous;
        [XmlIgnore]
        public bool IsSuperfluous
        {
            get
            {
                return this.isSuperfluous;
            }
            set
            {
                this.isSuperfluous = value;
                OnPropertyChanged("IsSuperfluous");
            }
        }
    }
}
