using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTIntervalReductionRule : HPTReductionRule
    {
        private bool use;
        [DataMember]
        public bool Use
        {
            get
            {
                return use;
            }
            set
            {
                if (use == value)
                {
                    return;
                }
                use = value;
                OnPropertyChanged("Use");
            }
        }

        private int minSum;
        [DataMember]
        public int MinSum
        {
            get
            {
                return minSum;
            }
            set
            {
                if (minSum == value)
                {
                    return;
                }
                minSum = value;
                OnPropertyChanged("MinSum");
            }
        }

        private int maxSum;
        [DataMember]
        public int MaxSum
        {
            get
            {
                return maxSum;
            }
            set
            {
                if (maxSum == value)
                {
                    return;
                }
                maxSum = value;
                OnPropertyChanged("MaxSum");
            }
        }

        private int minPercentSum;
        [DataMember]
        public int MinPercentSum
        {
            get
            {
                return minPercentSum;
            }
            set
            {
                if (minPercentSum == value)
                {
                    return;
                }
                minPercentSum = value;
                OnPropertyChanged("MinPercentSum");
            }
        }

        private int maxPercentSum;
        [DataMember]
        public int MaxPercentSum
        {
            get
            {
                return maxPercentSum;
            }
            set
            {
                if (maxPercentSum == value)
                {
                    return;
                }
                maxPercentSum = value;
                OnPropertyChanged("MaxPercentSum");
            }
        }

        private int lowestSum;
        [DataMember]
        public int LowestSum
        {
            get
            {
                return lowestSum;
            }
            set
            {
                if (value == lowestSum)
                {
                    return;
                }
                lowestSum = value;
                OnPropertyChanged("LowestSum");
            }
        }

        private int highestSum;
        [DataMember]
        public int HighestSum
        {
            get
            {
                return highestSum;
            }
            set
            {
                if (value == highestSum)
                {
                    return;
                }
                highestSum = value;
                OnPropertyChanged("HighestSum");
            }
        }

        private int lowestIncludedSum;
        [DataMember]
        public int LowestIncludedSum
        {
            get
            {
                return lowestIncludedSum;
            }
            set
            {
                if (value == lowestIncludedSum)
                {
                    return;
                }
                lowestIncludedSum = value;
                OnPropertyChanged("LowestIncludedSum");
            }
        }

        private int highestIncludedSum;
        [DataMember]
        public int HighestIncludedSum
        {
            get
            {
                return highestIncludedSum;
            }
            set
            {
                if (value == highestIncludedSum)
                {
                    return;
                }
                highestIncludedSum = value;
                OnPropertyChanged("HighestIncludedSum");
            }
        }

        private int incrementLower;
        [DataMember]
        public int IncrementLower
        {
            get
            {
                return incrementLower;
            }
            set
            {
                incrementLower = value;
                OnPropertyChanged("IncrementLower");
            }
        }

        private int incrementUpper;
        [DataMember]
        public int IncrementUpper
        {
            get
            {
                return incrementUpper;
            }
            set
            {
                incrementUpper = value;
                OnPropertyChanged("IncrementUpper");
            }
        }

        public override void SetReductionSpecificationString()
        {
            ReductionSpecificationString = "Summa " + MinSum.ToString() + " - " + MaxSum.ToString();
        }

    }
}
