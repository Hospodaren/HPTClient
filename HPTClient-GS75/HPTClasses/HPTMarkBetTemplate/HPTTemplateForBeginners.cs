using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                return this.stake;
            }
            set
            {
                this.stake = value;
                OnPropertyChanged("Stake");
            }
        }

        private int numberOfSpikes;
        public int NumberOfSpikes
        {
            get
            {
                return this.numberOfSpikes;
            }
            set
            {
                this.numberOfSpikes = value;
                OnPropertyChanged("NumberOfSpikes");
            }
        }

        private HPTReductionRisk reductionRisk;
        public HPTReductionRisk ReductionRisk
        {
            get
            {
                return this.reductionRisk;
            }
            set
            {
                this.reductionRisk = value;
                OnPropertyChanged("ReductionRisk");
            }
        }

        private HPTDesiredProfit desiredProfit;
        public HPTDesiredProfit DesiredProfit
        {
            get
            {
                return this.desiredProfit;
            }
            set
            {
                this.desiredProfit = value;
                OnPropertyChanged("DesiredProfit");
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
