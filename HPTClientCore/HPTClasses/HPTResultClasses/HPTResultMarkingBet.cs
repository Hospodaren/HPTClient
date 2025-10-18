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
                return this.winnerStrings;
            }
            set
            {
                this.winnerStrings = value;
                OnPropertyChanged("WinnerStrings");
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
                OnPropertyChanged("Value");
            }
        }

        private int systemsLeft;
        [DataMember]
        public int SystemsLeft
        {
            get
            {
                return this.systemsLeft;
            }
            set
            {
                this.systemsLeft = value;
                OnPropertyChanged("SystemsLeft");
            }
        }

        private bool hasResult;
        [DataMember]
        public bool HasResult
        {
            get
            {
                return this.hasResult;
            }
            set
            {
                this.hasResult = value;
                OnPropertyChanged("HasResult");
            }
        }

        private HPTHorse[] winnerList;
        [XmlIgnore]
        public HPTHorse[] WinnerList
        {
            get
            {
                return this.winnerList;
            }
            set
            {
                this.winnerList = value;
                OnPropertyChanged("WinnerList");
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
                return this.numberOfCorrect;
            }
            set
            {
                this.numberOfCorrect = value;
                OnPropertyChanged("NumberOfCorrect");
            }
        }

        private int payOutAmount;
        [DataMember]
        public int PayOutAmount
        {
            get
            {
                return this.payOutAmount;
            }
            set
            {
                this.payOutAmount = value;
                OnPropertyChanged("PayOutAmount");
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
                return this.totalAmount;
            }
            set
            {
                this.totalAmount = value;
                OnPropertyChanged("TotalAmount");
            }
        }

        private int numberOfWinningRows;
        [DataMember]
        public int NumberOfWinningRows
        {
            get
            {
                return this.numberOfWinningRows;
            }
            set
            {
                this.numberOfWinningRows = value;
                OnPropertyChanged("NumberOfWinningRows");
            }
        }

        private int minRowValue;
        public int MinRowValue
        {
            get
            {
                return this.minRowValue;
            }
            set
            {
                this.minRowValue = value;
                OnPropertyChanged("MinRowValue");
            }
        }

        private int maxRowValue;
        public int MaxRowValue
        {
            get
            {
                return this.maxRowValue;
            }
            set
            {
                this.maxRowValue = value;
                OnPropertyChanged("MaxRowValue");
            }
        }

        public int OwnWinnings { get; set; }
    }
}
