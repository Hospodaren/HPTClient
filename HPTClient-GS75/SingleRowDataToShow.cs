using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;

namespace HPTClient
{
    public class HPTSingleRowDataToShow : HPTDataToShow
    {
        #region Combination settings

        private bool showRowNumber;
        [HorseDataToShow("Radnummer", "ShowRowNumber", DataToShowUsage.Everywhere, 1)]
        [DataMember]
        public bool ShowRowNumber
        {
            get
            {
                return showRowNumber;
            }
            set
            {
                showRowNumber = value;
                OnPropertyChanged("ShowRowNumber");
            }
        }

        private bool showRowValue;
        [HorseDataToShow("Radvärde", "ShowRowValue", DataToShowUsage.Everywhere, 2)]
        [DataMember]
        public bool ShowRowValue
        {
            get
            {
                return showRowValue;
            }
            set
            {
                showRowValue = value;
                OnPropertyChanged("ShowRowValue");
            }
        }

        private bool showV6;
        [HorseDataToShow("V6/V7/V8-val", "ShowV6", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowV6
        {
            get
            {
                return showV6;
            }
            set
            {
                showV6 = value;
                OnPropertyChanged("ShowV6");
            }
        }

        private bool showRowValueV6;
        //[HorseDataToShow("Radvärde V6/V7/V8", "ShowRowValueV6", DataToShowUsage.Everywhere, 4)]
        [HorseDataToShow("Radvärde V6", "ShowRowValueV6", DataToShowUsage.Everywhere, 4)]
        [DataMember]
        public bool ShowRowValueV6
        {
            get
            {
                return showRowValueV6;
            }
            set
            {
                showRowValueV6 = value;
                OnPropertyChanged("ShowRowValueV6");
            }
        }

        private bool showBetMultiplier;
        [HorseDataToShow("Flerbongsval", "ShowBetMultiplier", DataToShowUsage.Everywhere, 5)]
        [DataMember]
        public bool ShowBetMultiplier
        {
            get
            {
                return showBetMultiplier;
            }
            set
            {
                showBetMultiplier = value;
                OnPropertyChanged("ShowBetMultiplier");
            }
        }

        private bool showRowValueBetMultiplier;
        [HorseDataToShow("Radvärde flerbong", "ShowRowValueBetMultiplier", DataToShowUsage.Everywhere, 6)]
        [DataMember]
        public bool ShowRowValueBetMultiplier
        {
            get
            {
                return showRowValueBetMultiplier;
            }
            set
            {
                showRowValueBetMultiplier = value;
                OnPropertyChanged("ShowRowValueBetMultiplier");
            }
        }

        private bool showRowValue1And2Errors;
        [HorseDataToShow("Radvärde 1/2 fel", "ShowRowValue1And2Errors", DataToShowUsage.Everywhere, 7)]
        [DataMember]
        public bool ShowRowValue1And2Errors
        {
            get
            {
                return showRowValue1And2Errors;
            }
            set
            {
                showRowValue1And2Errors = value;
                OnPropertyChanged("ShowRowValue1And2Errors");
            }
        }

        private bool showStakeShareSum;
        [HorseDataToShow("Insatsfördelningssumma", "ShowStakeShareSum", DataToShowUsage.Everywhere, 8)]
        [DataMember]
        public bool ShowStakeShareSum
        {
            get
            {
                return showStakeShareSum;
            }
            set
            {
                showStakeShareSum = value;
                OnPropertyChanged("ShowStakeShareSum");
            }
        }

        private bool showStartNumberSum;
        [HorseDataToShow("Startnummersumma", "ShowStartNumberSum", DataToShowUsage.Everywhere, 9)]
        [DataMember]
        public bool ShowStartNumberSum
        {
            get
            {
                return showStartNumberSum;
            }
            set
            {
                showStartNumberSum = value;
                OnPropertyChanged("ShowStartNumberSum");
            }
        }

        private bool showRankSum;
        [HorseDataToShow("Ranksumma", "ShowRankSum", DataToShowUsage.Everywhere, 10)]
        [DataMember]
        public bool ShowRankSum
        {
            get
            {
                return showRankSum;
            }
            set
            {
                showRankSum = value;
                OnPropertyChanged("ShowRankSum");
            }
        }

        private bool showHorses;
        [HorseDataToShow("Hästar", "ShowHorses", DataToShowUsage.None, 11)]
        [DataMember]
        public bool ShowHorses
        {
            get
            {
                return showHorses;
            }
            set
            {
                showHorses = value;
                OnPropertyChanged("ShowHorses");
            }
        }

        private bool showOwnProbability;
        [HorseDataToShow("Chansvärderingskvot", "ShowOwnProbability", DataToShowUsage.Everywhere, 12)]
        [DataMember]
        public bool ShowOwnProbability
        {
            get
            {
                return showOwnProbability;
            }
            set
            {
                showOwnProbability = value;
                OnPropertyChanged("ShowOwnProbability");
            }
        }

        #endregion
    }
}
