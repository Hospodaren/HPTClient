using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseDataToShow : HPTDataToShow
    {
        [DataMember]
        public bool ShowHorsePopup
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowDriverPopup
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowTrainerPopup
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        //[HorseDataToShow("Avdelningsnummer", "ShowLegNr", DataToShowUsage.All, 1)]
        [DataMember]
        public bool ShowStartNr
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("ABCD-rank", "ShowPrio",
            DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction | DataToShowUsage.Trio,
            2)]
        [DataMember]
        public bool ShowPrio
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Namn", "ShowName", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowName
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Kön", "ShowSex", DataToShowUsage.Everywhere, 4)]
        [DataMember]
        public bool ShowSex
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Ålder", "ShowAge", DataToShowUsage.Everywhere, 5)]
        [DataMember]
        public bool ShowAge
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Bana", "ShowTrack", DataToShowUsage.Everywhere, 6)]
        [DataMember]
        public bool ShowTrack
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("ATG-Trend", "ShowATGTrend", DataToShowUsage.Vxx, 3)]
        [DataMember]
        public bool ShowATGTrend
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Trender", "ShowTrends", DataToShowUsage.Vxx, 3)]
        [DataMember]
        public bool ShowTrends
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Kusk", "ShowDriver", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowDriver
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Tränare", "ShowTrainer", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowTrainer
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Ägare", "ShowOwner", DataToShowUsage.Everywhere, 3, true)]
        [DataMember]
        public bool ShowOwner
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Egen chansvärdering", "ShowOwnProbability", DataToShowUsage.Everywhere, 3, true)]
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

        [HorseDataToShow("Uppfödare", "ShowBreeder", DataToShowUsage.Everywhere, 3, true)]
        [DataMember]
        public bool ShowBreeder
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Vinnarodds", "ShowVinnarOdds", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowVinnarOdds
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Vinnaroddsandel", "ShowVinnarOddsShare", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowVinnarOddsShare
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Relativt vinnarodds", "ShowVinnarOddsRelative", DataToShowUsage.Everywhere, 3, true)]
        [DataMember]
        public bool ShowVinnarOddsRelative
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Platsodds", "ShowPlatsOdds", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowPlatsOdds
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Insatsfördelning", "ShowStakeDistributionPercent",
            DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3)]
        [DataMember]
        public bool ShowStakeDistributionPercent
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Insatsförd. (ack)", "ShowStakeDistributionShareAccumulated",
            DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3)]
        [DataMember]
        public bool ShowStakeDistributionShareAccumulated
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Streckbarhet", "ShowMarkability",
            DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3, true)]
        [DataMember]
        public bool ShowMarkability
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Reserv", "ShowReserv",
            DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3, true)]
        [DataMember]
        public bool ShowReserv
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Rekord", "ShowRecord", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowRecord
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        //[HorseDataToShow("Namn", "ShowName", DataToShowUsage.All, 3)]
        [DataMember]
        public bool ShowShape
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Intjänat", "ShowEarnings", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowEarnings
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Intjänat senaste 5", "ShowEarningsMeanLast5", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowEarningsMeanLast5
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Lås", "ShowLocked",
            DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3)]
        [DataMember]
        public bool ShowLocked
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        //[HorseDataToShow("Senaste start", "ShowDaysSinceLastStart", DataToShowUsage.All, 3)]
        [DataMember]
        public bool ShowDaysSinceLastStart
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Distans inklusive tillägg", "ShowDistance", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowDistance
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Skoinformation", "ShowShoeInfo", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowShoeInfo
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Vagninformation", "ShowSulkyInfo", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowSulkyInfo
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Kommentarer", "ShowComments", DataToShowUsage.None, 3, true)]
        [DataMember]
        public bool ShowComments
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Senaste start", "ShowLastStartDate", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowLastStartDate
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Resultatrad", "ShowResultRow", DataToShowUsage.Everywhere, 8)]
        [DataMember]
        public bool ShowResultRow
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Täckning", "ShowSystemCoverage",
            DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 9)]
        [DataMember]
        public bool ShowSystemCoverage
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Snittrank", "ShowRankMean", DataToShowUsage.Everywhere, 10)]
        [DataMember]
        public bool ShowRankMean
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Egen rank", "ShowRankOwn", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowRankOwn
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Poäng", "ShowRankAlternate", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowRankAlternate
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        //private bool showRankTip;
        //[HorseDataToShow("Tipsrank", "ShowRankTip", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 12, true)]
        //[DataMember]
        //public bool ShowRankTip
        //{
        //    get
        //    {
        //        return showRankTip;
        //    }
        //    set
        //    {
        //        showRankTip = value;
        //        OnPropertyChanged("ShowRankTip");
        //    }
        //}

        [HorseDataToShow("Egen info", "ShowOwnInformation", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowOwnInformation
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("DD/LD-andel", "ShowDoubleShare", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowDoubleShare
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Tvillingandel", "ShowTvillingShare", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowTvillingShare
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Trio-andel", "ShowTrioShare", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowTrioShare
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Platsandel", "ShowPlatsShare", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowPlatsShare
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        //[HorseDataToShow("Alt. insatsfördelning", "ShowStakeShare", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 11, true)]
        [HorseDataToShow("Alt. insatsfördelning", "ShowStakeShare", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowStakeShare
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Insatsfördelning relativt favoriten", "ShowStakeShareRelativeToFavourite",
            DataToShowUsage.Vxx, 11, true)]
        [DataMember]
        public bool ShowStakeShareRelativeToFavourite
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Insatsfördelning relativt föregående- och nästrankad", "ShowStakeShareRelativeToNext",
            DataToShowUsage.Vxx, 12, true)]
        [DataMember]
        public bool ShowStakeShareRelativeToNext
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Inbördes möten", "ShowHeadToHead", DataToShowUsage.Everywhere, 12, true)]
        [DataMember]
        public bool ShowHeadToHead
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Insatsutveckling", "ShowRelativeDifference",
            DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 12, true)]
        [DataMember]
        public bool ShowRelativeDifference
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        //private bool showRelativeDifferenceVinnare;
        //[HorseDataToShow("Insatsutveckling (V)", "ShowRelativeDifferenceVinnare", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 12, true)]
        //[DataMember]
        //public bool ShowRelativeDifferenceVinnare
        //{
        //    get
        //    {
        //        return showRelativeDifferenceVinnare;
        //    }
        //    set
        //    {
        //        showRelativeDifferenceVinnare = value;
        //        OnPropertyChanged("ShowRelativeDifferenceVinnare");
        //    }
        //}

        //private bool showRelativeDifferencePlats;
        //[HorseDataToShow("Insatsutveckling (P)", "ShowRelativeDifferencePlats", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 12, true)]
        //[DataMember]
        //public bool ShowRelativeDifferencePlats
        //{
        //    get
        //    {
        //        return showRelativeDifferencePlats;
        //    }
        //    set
        //    {
        //        showRelativeDifferencePlats = value;
        //        OnPropertyChanged("ShowRelativeDifferencePlats");
        //    }
        //}

        [HorseDataToShow("Hästinfo på ST", "ShowSTHorseLink", DataToShowUsage.Everywhere, 12, true)]
        [DataMember]
        public bool ShowSTHorseLink
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HorseDataToShow("Dagens resultat", "ShowResultInfo", DataToShowUsage.Everywhere, 12, true)]
        [DataMember]
        public bool ShowResultInfo
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        #region Specialfält som inte är konfigurerbara

        [HorseDataToShow("Utgång", "ShowComplimentaryRuleSelect", DataToShowUsage.None, 2)]
        [DataMember]
        public bool ShowComplimentaryRuleSelect
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowLegNrText { get; set; }

        [DataMember]
        public bool ShowTrio { get; set; }

        [DataMember]
        public bool ShowSystemsLeft { get; set; }

        [DataMember]
        public bool ShowSystemValue { get; set; }

        [DataMember]
        public bool ShowATGResultLink { get; set; }

        #endregion

        #region Obsolete

        //private bool showMarksPercent;
        ////[HorseDataToShow("Streckprocent", "ShowMarksPercent", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3)]
        ////[DataMember]
        //public bool ShowMarksPercent
        //{
        //    get
        //    {
        //        return showMarksPercent;
        //    }
        //    set
        //    {
        //        showMarksPercent = value;
        //        OnPropertyChanged("ShowMarksPercent");
        //    }
        //}

        //[HorseDataToShow("Streckandel", "ShowMarksShare", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3)]
        //[DataMember]
        public bool ShowMarksShare
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        //[HorseDataToShow("Streckantal", "ShowMarksQuantity", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3)]
        //[DataMember]
        public bool ShowMarksQuantity
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Obsolete ranks

        [HorseDataToShow("ATG-Rank", "ShowRankATG",
            DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 20)]
        [DataMember]
        public bool ShowRankATG
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankVinnarOdds
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankMarks
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankMarkability
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankRecord
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankShape
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankEarnings
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankEarningsMeanLast5
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankRecordWeighedTotal
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankRecordWeighedLast5
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankEarningsMeanThisYear
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankEarningsMeanLastYear
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankTotalEarningsMean
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRankPlatsodds
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
