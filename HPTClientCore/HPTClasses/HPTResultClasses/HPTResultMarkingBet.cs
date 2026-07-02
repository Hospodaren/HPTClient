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

        private string[] winnerStrings;
        [XmlIgnore]
        public string[] WinnerStrings
        {
            get
            {
                return winnerStrings;
            }
            set
            {
                winnerStrings = value;
                OnPropertyChanged();
            }
        }

        private int? value;
        [DataMember]
        public int? Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }

        private decimal? systemsLeft;
        [DataMember]
        public decimal? SystemsLeft
        {
            get
            {
                return systemsLeft;
            }
            set
            {
                systemsLeft = value;
                OnPropertyChanged();
            }
        }

        private bool hasResult;
        [DataMember]
        public bool HasResult
        {
            get
            {
                return hasResult;
            }
            set
            {
                hasResult = value;
                OnPropertyChanged();
            }
        }

        private HPTHorse[] winnerList;
        [XmlIgnore]
        public HPTHorse[] WinnerList
        {
            get
            {
                return winnerList;
            }
            set
            {
                winnerList = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string LegNrString { get; set; }
    }

    [DataContract]
    public class HPTPayOut : Notifier
    {
        private int numberOfCorrect;
        [DataMember]
        public int NumberOfCorrect
        {
            get
            {
                return numberOfCorrect;
            }
            set
            {
                numberOfCorrect = value;
                OnPropertyChanged();
            }
        }

        private decimal payOutAmount;
        [DataMember]
        public decimal PayOutAmount
        {
            get
            {
                return payOutAmount;
            }
            set
            {
                payOutAmount = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int NumberOfSystems { get; set; }

        //[DataMember]
        //public int TotalAmount { get; set; }

        private int totalAmount;
        [DataMember]
        public int TotalAmount
        {
            get
            {
                return totalAmount;
            }
            set
            {
                totalAmount = value;
                OnPropertyChanged();
            }
        }

        private int numberOfWinningRows;
        [DataMember]
        public int NumberOfWinningRows
        {
            get
            {
                return numberOfWinningRows;
            }
            set
            {
                numberOfWinningRows = value;
                OnPropertyChanged();
            }
        }

        private int minRowValue;
        public int MinRowValue
        {
            get
            {
                return minRowValue;
            }
            set
            {
                minRowValue = value;
                OnPropertyChanged();
            }
        }

        private int maxRowValue;
        public int MaxRowValue
        {
            get
            {
                return maxRowValue;
            }
            set
            {
                maxRowValue = value;
                OnPropertyChanged();
            }
        }

        public int OwnWinnings { get; set; }
    }
}
