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

        [DataMember]
        public ObservableCollection<HPTNumberOfWinners> NumberOfWinnersList
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public bool IncludeRow(HPTMarkBetSingleRow singleRow, string propertyName)
        {
            var horsesInInterval = 0;
            foreach (var horse in singleRow.HorseList)
            {
                var horseValue = Convert.ToDecimal(horse.GetType().GetProperty(propertyName).GetValue(horse, null));
                horsesInInterval += IsInInterval(horseValue) ? 1 : 0;
            }
            return NumberOfWinnersList.First(now => now.NumberOfWinners == horsesInInterval).Selected;
        }

        [DataMember]
        public bool Use
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public decimal LowerBoundary
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public decimal UpperBoundary
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public bool IsInInterval(decimal Value)
        {
            return (Value >= LowerBoundary && Value <= UpperBoundary);
        }
    }
}
