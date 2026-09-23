using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTLegResult : Notifier
    {
        [DataMember]
        public int LegNr { get; set; }

        [DataMember]
        public int[] Winners { get; set; }

        [XmlIgnore]
        public string[] WinnerStrings
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int? Value
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public decimal? SystemsLeft
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool HasResult
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public HPTHorse[] WinnerList
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string LegNrString { get; set; }
    }

    [DataContract]
    public class HPTPayOut : Notifier
    {
        [DataMember]
        public int NumberOfCorrect
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public decimal PayOutAmount
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int NumberOfSystems { get; set; }

        //[DataMember]
        //public int TotalAmount { get; set; }

        [DataMember]
        public int TotalAmount
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int NumberOfWinningRows
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int MinRowValue
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int MaxRowValue
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int OwnWinnings { get; set; }
    }
}
