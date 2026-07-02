using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTNumberOfWinners : Notifier
    {
        [DataMember]
        public int NumberOfWinners
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
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool Selectable
        {
            get;
            set
            {
                if (!value && Selected)
                {
                    Selected = false;
                }
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        public decimal Probability
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public bool IsSuperfluous
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }
}
