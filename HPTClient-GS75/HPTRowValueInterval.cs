using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                return this.lowerLimit;
            }
            set
            {
                this.lowerLimit = value;
                OnPropertyChanged("LowerLimit");
            }
        }

        private int? upperLimit;
        [DataMember]
        public int? UpperLimit
        {
            get
            {
                return this.upperLimit;
            }
            set
            {
                this.upperLimit = value;
                OnPropertyChanged("UpperLimit");
            }
        }

        private int numberOfRows;
        [DataMember]
        public int NumberOfRows
        {
            get
            {
                return this.numberOfRows;
            }
            set
            {
                this.numberOfRows = value;
                OnPropertyChanged("NumberOfRows");
            }
        }

        private decimal percentageOfRows;
        [DataMember]
        public decimal PercentageOfRows
        {
            get
            {
                return this.percentageOfRows;
            }
            set
            {
                this.percentageOfRows = value;
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
                return this.percentile;
            }
            set
            {
                this.percentile = value;
                OnPropertyChanged("Percentile");
            }
        }

        private string description;
        [DataMember]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
                OnPropertyChanged("Description");
            }
        }

        private int rowValue;
        [DataMember]
        public int RowValue
        {
            get
            {
                return this.rowValue;
            }
            set
            {
                this.rowValue = value;
                OnPropertyChanged("RowValue");
            }
        }
    }
}
