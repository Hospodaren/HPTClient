using System.Runtime.Serialization;
using System.Windows.Media;

namespace HPTClient
{
    [DataContract]
    public class HPTColorInterval : Notifier
    {
        private decimal lowerBoundary;
        [DataMember]
        public decimal LowerBoundary
        {
            get
            {
                return lowerBoundary;
            }
            set
            {
                lowerBoundary = value;
                OnPropertyChanged("LowerBoundary");
            }
        }

        private decimal upperBoundary;
        [DataMember]
        public decimal UpperBoundary
        {
            get
            {
                return upperBoundary;
            }
            set
            {
                upperBoundary = value;
                OnPropertyChanged("UpperBoundary");
            }
        }

        private Color lowColor;
        [DataMember]
        public Color LowColor
        {
            get
            {
                return lowColor;
            }
            set
            {
                lowColor = value;
                OnPropertyChanged("LowColor");
            }
        }

        private Color mediumColor;
        [DataMember]
        public Color MediumColor
        {
            get
            {
                return mediumColor;
            }
            set
            {
                mediumColor = value;
                OnPropertyChanged("MediumColor");
            }
        }

        private Color highColor;
        [DataMember]
        public Color HighColor
        {
            get
            {
                return highColor;
            }
            set
            {
                highColor = value;
                OnPropertyChanged("HighColor");
            }
        }
    }
}
