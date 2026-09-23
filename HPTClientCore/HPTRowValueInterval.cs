using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTRowValueInterval : Notifier
    {
        [DataMember]
        public int? LowerLimit
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int? UpperLimit
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int NumberOfRows
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public decimal PercentageOfRows
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    [DataContract]
    public class HPTRowValuePercentile : Notifier
    {
        [DataMember]
        public decimal Percentile
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string Description
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int RowValue
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
