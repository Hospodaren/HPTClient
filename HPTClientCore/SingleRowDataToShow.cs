using System.Runtime.Serialization;

namespace HPTClient
{
    public class HPTSingleRowDataToShow : HPTDataToShow
    {
        #region Combination settings

        [HorseDataToShow("Radnummer", "ShowRowNumber", DataToShowUsage.Everywhere, 1)]
        [DataMember]
        public bool ShowRowNumber
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Radvärde", "ShowRowValue", DataToShowUsage.Everywhere, 2)]
        [DataMember]
        public bool ShowRowValue
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("V6/V7/V8-val", "ShowV6", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowV6
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        //[HorseDataToShow("Radvärde V6/V7/V8", "ShowRowValueV6", DataToShowUsage.Everywhere, 4)]
        [HorseDataToShow("Radvärde V6", "ShowRowValueV6", DataToShowUsage.Everywhere, 4)]
        [DataMember]
        public bool ShowRowValueV6
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Flerbongsval", "ShowBetMultiplier", DataToShowUsage.Everywhere, 5)]
        [DataMember]
        public bool ShowBetMultiplier
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Radvärde flerbong", "ShowRowValueBetMultiplier", DataToShowUsage.Everywhere, 6)]
        [DataMember]
        public bool ShowRowValueBetMultiplier
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Radvärde 1/2 fel", "ShowRowValue1And2Errors", DataToShowUsage.Everywhere, 7)]
        [DataMember]
        public bool ShowRowValue1And2Errors
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Insatsfördelningssumma", "ShowStakeShareSum", DataToShowUsage.Everywhere, 8)]
        [DataMember]
        public bool ShowStakeShareSum
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Startnummersumma", "ShowStartNumberSum", DataToShowUsage.Everywhere, 9)]
        [DataMember]
        public bool ShowStartNumberSum
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Ranksumma", "ShowRankSum", DataToShowUsage.Everywhere, 10)]
        [DataMember]
        public bool ShowRankSum
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Hästar", "ShowHorses", DataToShowUsage.None, 11)]
        [DataMember]
        public bool ShowHorses
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Chansvärderingskvot", "ShowOwnProbability", DataToShowUsage.Everywhere, 12)]
        [DataMember]
        public bool ShowOwnProbability
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
