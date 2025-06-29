using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTCombinationDataToShow : HPTDataToShow
    {
        #region Combination settings

        private bool showCombinationOdds;
        [HorseDataToShow("Odds", "ShowCombinationOdds", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowCombinationOdds
        {
            get
            {
                return showCombinationOdds;
            }
            set
            {
                showCombinationOdds = value;
                OnPropertyChanged("ShowCombinationOdds");
            }
        }

        private bool showCombinationOddsRank;
        [HorseDataToShow("Oddsrank", "ShowCombinationOddsRank", DataToShowUsage.Tvilling | DataToShowUsage.Trio | DataToShowUsage.Double, 3)]
        [DataMember]
        public bool ShowCombinationOddsRank
        {
            get
            {
                return showCombinationOddsRank;
            }
            set
            {
                showCombinationOddsRank = value;
                OnPropertyChanged("ShowCombinationOddsRank");
            }
        }

        private bool showMultipliedOdds;
        [HorseDataToShow("Multiplicerat odds", "ShowMultipliedOdds", DataToShowUsage.Tvilling | DataToShowUsage.Double, 3)]
        [DataMember]
        public bool ShowMultipliedOdds
        {
            get
            {
                return showMultipliedOdds;
            }
            set
            {
                showMultipliedOdds = value;
                OnPropertyChanged("ShowMultipliedOdds");
            }
        }

        private bool showMultipliedOddsRank;
        [HorseDataToShow("Rank multiplicerat odds", "ShowMultipliedOddsRank", DataToShowUsage.Tvilling | DataToShowUsage.Double, 3)]
        [DataMember]
        public bool ShowMultipliedOddsRank
        {
            get
            {
                return showMultipliedOddsRank;
            }
            set
            {
                showMultipliedOddsRank = value;
                OnPropertyChanged("ShowMultipliedOddsRank");
            }
        }

        private bool showPlayability;
        [HorseDataToShow("Spelbarhet", "ShowPlayability", DataToShowUsage.Tvilling | DataToShowUsage.Trio | DataToShowUsage.Double, 3)]
        //[HorseDataToShow("Spelbarhet", "ShowPlayability", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowPlayability
        {
            get
            {
                return showPlayability;
            }
            set
            {
                showPlayability = value;
                OnPropertyChanged("ShowPlayability");
            }
        }

        private bool showOddsQuota;
        [HorseDataToShow("Oddskvot", "ShowOddsQuota", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowOddsQuota
        {
            get
            {
                return showOddsQuota;
            }
            set
            {
                showOddsQuota = value;
                OnPropertyChanged("ShowOddsQuota");
            }
        }

        private bool showProfit;
        [HorseDataToShow("Vinst", "ShowProfit", DataToShowUsage.Tvilling | DataToShowUsage.Trio | DataToShowUsage.Double, 3)]
        [DataMember]
        public bool ShowProfit
        {
            get
            {
                return showProfit;
            }
            set
            {
                showProfit = value;
                OnPropertyChanged("ShowProfit");
            }
        }

        private bool showSelected;
        [HorseDataToShow("Vald", "ShowSelected", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowSelected
        {
            get
            {
                return showSelected;
            }
            set
            {
                showSelected = value;
                OnPropertyChanged("ShowSelected");
            }
        }

        private bool showStake;
        [HorseDataToShow("Insats", "ShowStake", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowStake
        {
            get
            {
                return showStake;
            }
            set
            {
                showStake = value;
                OnPropertyChanged("ShowStake");
            }
        }

        private bool showStartNr1;
        [HorseDataToShow("Häst 1", "ShowStartNr1", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowStartNr1
        {
            get
            {
                return showStartNr1;
            }
            set
            {
                showStartNr1 = value;
                OnPropertyChanged("ShowStartNr1");
            }
        }

        private bool showStartNr2;
        [HorseDataToShow("Häst 2", "ShowStartNr2", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowStartNr2
        {
            get
            {
                return showStartNr2;
            }
            set
            {
                showStartNr2 = value;
                OnPropertyChanged("ShowStartNr2");
            }
        }

        private bool showStartNr3;
        [HorseDataToShow("Häst 3", "ShowStartNr3", DataToShowUsage.None, 3)]
        [DataMember]
        public bool ShowStartNr3
        {
            get
            {
                return showStartNr3;
            }
            set
            {
                showStartNr3 = value;
                OnPropertyChanged("ShowStartNr3");
            }
        }

        private bool showStakeQuota;
        [HorseDataToShow("Vxx-kvot", "ShowStakeQuota", DataToShowUsage.Double | DataToShowUsage.Tvilling, 3)]
        [DataMember]
        public bool ShowStakeQuota
        {
            get
            {
                return showStakeQuota;
            }
            set
            {
                showStakeQuota = value;
                OnPropertyChanged("ShowStakeQuota");
            }
        }

        private bool showVPQuota;
        [HorseDataToShow("VP-kvot", "ShowVPQuota", DataToShowUsage.Trio | DataToShowUsage.Tvilling, 3)]
        [DataMember]
        public bool ShowVPQuota
        {
            get
            {
                return showVPQuota;
            }
            set
            {
                showVPQuota = value;
                OnPropertyChanged("ShowVPQuota");
            }
        }

        private bool showVQuota;
        [HorseDataToShow("V-kvot", "ShowVQuota", DataToShowUsage.Trio | DataToShowUsage.Double | DataToShowUsage.Tvilling, 3)]
        [DataMember]
        public bool ShowVQuota
        {
            get
            {
                return showVQuota;
            }
            set
            {
                showVQuota = value;
                OnPropertyChanged("ShowVQuota");
            }
        }

        private bool showPQuota;
        [HorseDataToShow("P-kvot", "ShowPQuota", DataToShowUsage.Trio | DataToShowUsage.Double | DataToShowUsage.Tvilling, 3)]
        [DataMember]
        public bool ShowPQuota
        {
            get
            {
                return showPQuota;
            }
            set
            {
                showPQuota = value;
                OnPropertyChanged("ShowPQuota");
            }
        }

        private bool showOPQuota;
        [HorseDataToShow("Chansvärderingskvot", "ShowOPQuota", DataToShowUsage.Double | DataToShowUsage.Tvilling, 3)]
        [DataMember]
        public bool ShowOPQuota
        {
            get
            {
                return showOPQuota;
            }
            set
            {
                showOPQuota = value;
                OnPropertyChanged("ShowOPQuota");
            }
        }

        private bool showTVQuota;
        [HorseDataToShow("Tvilling-kvot", "ShowTVQuota", DataToShowUsage.Double, 3)]
        [DataMember]
        public bool ShowTVQuota
        {
            get
            {
                return showTVQuota;
            }
            set
            {
                showTVQuota = value;
                OnPropertyChanged("ShowTVQuota");
            }
        }

        private bool showDQuota;
        [HorseDataToShow("DD/LD-kvot", "ShowDQuota", DataToShowUsage.Tvilling, 3)]
        [DataMember]
        public bool ShowDQuota
        {
            get
            {
                return showDQuota;
            }
            set
            {
                showDQuota = value;
                OnPropertyChanged("ShowDQuota");
            }
        }

        #endregion
    }
}
