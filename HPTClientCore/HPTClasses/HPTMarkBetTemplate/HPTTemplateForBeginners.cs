namespace HPTClient
{
    public class HPTTemplateForBeginners : Notifier
    {
        public List<HPTHorseRankVariable> HorseRankVariableList { get; set; }

        private int stake;
        public int Stake
        {
            get
            {
                return stake;
            }
            set
            {
                stake = value;
                OnPropertyChanged();
            }
        }

        private int numberOfSpikes;
        public int NumberOfSpikes
        {
            get
            {
                return numberOfSpikes;
            }
            set
            {
                numberOfSpikes = value;
                OnPropertyChanged();
            }
        }

        private HPTReductionRisk reductionRisk;
        public HPTReductionRisk ReductionRisk
        {
            get
            {
                return reductionRisk;
            }
            set
            {
                reductionRisk = value;
                OnPropertyChanged();
            }
        }

        private HPTDesiredProfit desiredProfit;
        public HPTDesiredProfit DesiredProfit
        {
            get
            {
                return desiredProfit;
            }
            set
            {
                desiredProfit = value;
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
