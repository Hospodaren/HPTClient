using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTCombinationDataToShow : HPTDataToShow
    {
        #region Combination settings

        [HorseDataToShow("Odds", "ShowCombinationOdds", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowCombinationOdds
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Oddsrank", "ShowCombinationOddsRank",
            DataToShowUsage.Tvilling | DataToShowUsage.Trio | DataToShowUsage.Double, 3)]
        [DataMember]
        public bool ShowCombinationOddsRank
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Multiplicerat odds", "ShowMultipliedOdds", DataToShowUsage.Tvilling | DataToShowUsage.Double,
            3)]
        [DataMember]
        public bool ShowMultipliedOdds
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Rank multiplicerat odds", "ShowMultipliedOddsRank",
            DataToShowUsage.Tvilling | DataToShowUsage.Double, 3)]
        [DataMember]
        public bool ShowMultipliedOddsRank
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Spelbarhet", "ShowPlayability",
            DataToShowUsage.Tvilling | DataToShowUsage.Trio | DataToShowUsage.Double, 3)]
        //[HorseDataToShow("Spelbarhet", "ShowPlayability", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowPlayability
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Oddskvot", "ShowOddsQuota", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowOddsQuota
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Vinst", "ShowProfit",
            DataToShowUsage.Tvilling | DataToShowUsage.Trio | DataToShowUsage.Double, 3)]
        [DataMember]
        public bool ShowProfit
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Vald", "ShowSelected", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowSelected
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Insats", "ShowStake", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowStake
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Häst 1", "ShowStartNr1", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowStartNr1
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Häst 2", "ShowStartNr2", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowStartNr2
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Häst 3", "ShowStartNr3", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowStartNr3
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Vxx-kvot", "ShowStakeQuota", DataToShowUsage.Double | DataToShowUsage.Tvilling, 3)]
        [DataMember]
        public bool ShowStakeQuota
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("VP-kvot", "ShowVPQuota", DataToShowUsage.Trio | DataToShowUsage.Tvilling, 3)]
        [DataMember]
        public bool ShowVPQuota
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("V-kvot", "ShowVQuota",
            DataToShowUsage.Trio | DataToShowUsage.Double | DataToShowUsage.Tvilling, 3)]
        [DataMember]
        public bool ShowVQuota
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("P-kvot", "ShowPQuota",
            DataToShowUsage.Trio | DataToShowUsage.Double | DataToShowUsage.Tvilling, 3)]
        [DataMember]
        public bool ShowPQuota
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Chansvärderingskvot", "ShowOPQuota", DataToShowUsage.Double | DataToShowUsage.Tvilling, 3)]
        [DataMember]
        public bool ShowOPQuota
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Tvilling-kvot", "ShowTVQuota", DataToShowUsage.Double, 3)]
        [DataMember]
        public bool ShowTVQuota
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("DD/LD-kvot", "ShowDQuota", DataToShowUsage.Tvilling, 3)]
        [DataMember]
        public bool ShowDQuota
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
