using System.Runtime.Serialization;
using System.Xml.Serialization;

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
                return numberOfWinners;
            }
            set
            {
                numberOfWinners = value;
                OnPropertyChanged();
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
                if (selected == value)
                {
                    return;
                }
                selected = value;
                OnPropertyChanged();
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
                if (!value && Selected)
                {
                    Selected = false;
                }
                if (selectable == value)
                {
                    return;
                }
                selectable = value;
                OnPropertyChanged();
            }
        }

        private decimal probability;
        public decimal Probability
        {
            get
            {
                return probability;
            }
            set
            {
                probability = value;
                OnPropertyChanged();
            }
        }

        private bool isSuperfluous;
        [XmlIgnore]
        public bool IsSuperfluous
        {
            get
            {
                return isSuperfluous;
            }
            set
            {
                isSuperfluous = value;
                OnPropertyChanged();
            }
        }
    }
}
