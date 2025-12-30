using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTIntervalGroup : Notifier
    {
        public HPTIntervalGroup()
        {
        }

        void now_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                OnPropertyChanged("NumberOfWinnersSelected");
            }
        }

        private ObservableCollection<HPTNumberOfWinners> numberOfWinnersList;
        [DataMember]
        public ObservableCollection<HPTNumberOfWinners> NumberOfWinnersList
        {
            get
            {
                return this.numberOfWinnersList;
            }
            set
            {
                this.numberOfWinnersList = value;
                OnPropertyChanged("NumberOfWinnersList");
            }
        }

        public bool IncludeRow(HPTMarkBetSingleRow singleRow, string propertyName)
        {
            int horsesInInterval = 0;
            foreach (HPTHorse horse in singleRow.HorseList)
            {
                decimal horseValue = Convert.ToDecimal(horse.GetType().GetProperty(propertyName).GetValue(horse, null));
                horsesInInterval += IsInInterval(horseValue) ? 1 : 0;
            }
            return this.NumberOfWinnersList.First(now => now.NumberOfWinners == horsesInInterval).Selected;
        }

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
                this.use = value;
                OnPropertyChanged("Use");
            }
        }

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
                this.lowerBoundary = value;
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
                this.upperBoundary = value;
                OnPropertyChanged("UpperBoundary");
            }
        }

        public bool IsInInterval(decimal Value)
        {
            return (Value >= this.LowerBoundary && Value <= this.UpperBoundary);
        }
    }
}
