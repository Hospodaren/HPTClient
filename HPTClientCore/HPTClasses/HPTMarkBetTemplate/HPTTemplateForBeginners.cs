namespace HPTClient
{
    public class HPTTemplateForBeginners : Notifier
    {
        public List<HPTHorseRankVariable> HorseRankVariableList { get; set; }

        public int Stake
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfSpikes
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public HPTReductionRisk ReductionRisk
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public HPTDesiredProfit DesiredProfit
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public enum HPTReductionRisk
    {
        Medium,
        Low,
        High
    }

    public enum HPTDesiredProfit
    {
        Medium,
        Low,
        High
    }
}
