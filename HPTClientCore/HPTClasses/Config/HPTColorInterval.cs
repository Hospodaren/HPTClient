using System.Runtime.Serialization;
using System.Windows.Media;

namespace HPTClient
{
    [DataContract]
    public class HPTColorInterval : Notifier
    {
        [DataMember]
        public decimal LowerBoundary
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public decimal UpperBoundary
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public Color LowColor
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public Color MediumColor
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public Color HighColor
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
