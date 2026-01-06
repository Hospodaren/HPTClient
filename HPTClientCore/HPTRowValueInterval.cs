using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTRowValueInterval : Notifier
    {
        private int? lowerLimit;
        [DataMember]
        public int? LowerLimit
        {
            get
            {
                return lowerLimit;
            }
            set
            {
                lowerLimit = value;
                OnPropertyChanged("LowerLimit");
            }
        }

        private int? upperLimit;
        [DataMember]
        public int? UpperLimit
        {
            get
            {
                return upperLimit;
            }
            set
            {
                upperLimit = value;
                OnPropertyChanged("UpperLimit");
            }
        }

        private int numberOfRows;
        [DataMember]
        public int NumberOfRows
        {
            get
            {
                return numberOfRows;
            }
            set
            {
                numberOfRows = value;
                OnPropertyChanged("NumberOfRows");
            }
        }

        private decimal percentageOfRows;
        [DataMember]
        public decimal PercentageOfRows
        {
            get
            {
                return percentageOfRows;
            }
            set
            {
                percentageOfRows = value;
                OnPropertyChanged("PercentageOfRows");
            }
        }
    }

    [DataContract]
    public class HPTRowValuePercentile : Notifier
    {
        private decimal percentile;
        [DataMember]
        public decimal Percentile
        {
            get
            {
                return percentile;
            }
            set
            {
                percentile = value;
                OnPropertyChanged("Percentile");
            }
        }

        private string description;
        [DataMember]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        private int rowValue;
        [DataMember]
        public int RowValue
        {
            get
            {
                return rowValue;
            }
            set
            {
                rowValue = value;
                OnPropertyChanged("RowValue");
            }
        }
    }
}
