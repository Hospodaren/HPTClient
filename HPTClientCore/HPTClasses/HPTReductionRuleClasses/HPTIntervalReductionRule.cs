using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTIntervalReductionRule : HPTReductionRule
    {
        [DataMember]
        public bool Use
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
        public int MinSum
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
        public int MaxSum
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
        public int MinPercentSum
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
        public int MaxPercentSum
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
        public int LowestSum
        {
            get;
            set
            {
                if (value == field)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int HighestSum
        {
            get;
            set
            {
                if (value == field)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int LowestIncludedSum
        {
            get;
            set
            {
                if (value == field)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int HighestIncludedSum
        {
            get;
            set
            {
                if (value == field)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int IncrementLower
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int IncrementUpper
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public override void SetReductionSpecificationString()
        {
            ReductionSpecificationString = $"Summa {MinSum} - {MaxSum}";
        }

    }
}
