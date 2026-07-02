using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseDataToShow : HPTDataToShow
    {
        private bool showHorsePopup;
        [DataMember]
        public bool ShowHorsePopup
        {
            get
            {
                return showHorsePopup;
            }
            set
            {
                showHorsePopup = value;
                OnPropertyChanged();
            }
        }

        private bool showDriverPopup;
        [DataMember]
        public bool ShowDriverPopup
        {
            get
            {
                return showDriverPopup;
            }
            set
            {
                showDriverPopup = value;
                OnPropertyChanged();
            }
        }

        private bool showTrainerPopup;
        [DataMember]
        public bool ShowTrainerPopup
        {
            get
            {
                return showTrainerPopup;
            }
            set
            {
                showTrainerPopup = value;
                OnPropertyChanged();
            }
        }

        private bool showStartNr;
        //[HorseDataToShow("Avdelningsnummer", "ShowLegNr", DataToShowUsage.All, 1)]
        [DataMember]
        public bool ShowStartNr
        {
            get
            {
                return showStartNr;
            }
            set
            {
                showStartNr = value;
                OnPropertyChanged();
            }
        }

        private bool showPrio;
        [HorseDataToShow("ABCD-rank", "ShowPrio", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction | DataToShowUsage.Trio, 2)]
        [DataMember]
        public bool ShowPrio
        {
            get
            {
                return showPrio;
            }
            set
            {
                showPrio = value;
                OnPropertyChanged();
            }
        }

        private bool showName;
        [HorseDataToShow("Namn", "ShowName", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowName
        {
            get
            {
                return showName;
            }
            set
            {
                showName = value;
                OnPropertyChanged();
            }
        }

        private bool showSex;
        [HorseDataToShow("Kön", "ShowSex", DataToShowUsage.Everywhere, 4)]
        [DataMember]
        public bool ShowSex
        {
            get
            {
                return showSex;
            }
            set
            {
                showSex = value;
                OnPropertyChanged();
            }
        }

        private bool showAge;
        [HorseDataToShow("Ålder", "ShowAge", DataToShowUsage.Everywhere, 5)]
        [DataMember]
        public bool ShowAge
        {
            get
            {
                return showAge;
            }
            set
            {
                showAge = value;
                OnPropertyChanged();
            }
        }

        private bool showTrack;
        [HorseDataToShow("Bana", "ShowTrack", DataToShowUsage.Everywhere, 6)]
        [DataMember]
        public bool ShowTrack
        {
            get
            {
                return showTrack;
            }
            set
            {
                showTrack = value;
                OnPropertyChanged();
            }
        }

        private bool showATGTrend;
        [HorseDataToShow("ATG-Trend", "ShowATGTrend", DataToShowUsage.Vxx, 3)]
        [DataMember]
        public bool ShowATGTrend
        {
            get
            {
                return showATGTrend;
            }
            set
            {
                showATGTrend = value;
                OnPropertyChanged();
            }
        }

        private bool showTrends;
        [HorseDataToShow("Trender", "ShowTrends", DataToShowUsage.Vxx, 3)]
        [DataMember]
        public bool ShowTrends
        {
            get
            {
                return showTrends;
            }
            set
            {
                showTrends = value;
                OnPropertyChanged();
            }
        }

        private bool showDriver;
        [HorseDataToShow("Kusk", "ShowDriver", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowDriver
        {
            get
            {
                return showDriver;
            }
            set
            {
                showDriver = value;
                OnPropertyChanged();
            }
        }

        private bool showTrainer;
        [HorseDataToShow("Tränare", "ShowTrainer", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowTrainer
        {
            get
            {
                return showTrainer;
            }
            set
            {
                showTrainer = value;
                OnPropertyChanged();
            }
        }

        private bool showOwner;
        [HorseDataToShow("Ägare", "ShowOwner", DataToShowUsage.Everywhere, 3, true)]
        [DataMember]
        public bool ShowOwner
        {
            get
            {
                return showOwner;
            }
            set
            {
                showOwner = value;
                OnPropertyChanged();
            }
        }

        private bool showOwnProbability;
        [HorseDataToShow("Egen chansvärdering", "ShowOwnProbability", DataToShowUsage.Everywhere, 3, true)]
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
                OnPropertyChanged();
            }
        }

        private bool showBreeder;
        [HorseDataToShow("Uppfödare", "ShowBreeder", DataToShowUsage.Everywhere, 3, true)]
        [DataMember]
        public bool ShowBreeder
        {
            get
            {
                return showBreeder;
            }
            set
            {
                showBreeder = value;
                OnPropertyChanged();
            }
        }

        private bool showVinnarOdds;
        [HorseDataToShow("Vinnarodds", "ShowVinnarOdds", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowVinnarOdds
        {
            get
            {
                return showVinnarOdds;
            }
            set
            {
                showVinnarOdds = value;
                OnPropertyChanged();
            }
        }

        private bool showVinnarOddsShare;
        [HorseDataToShow("Vinnaroddsandel", "ShowVinnarOddsShare", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowVinnarOddsShare
        {
            get
            {
                return showVinnarOddsShare;
            }
            set
            {
                showVinnarOddsShare = value;
                OnPropertyChanged();
            }
        }

        private bool showVinnarOddsRelative;
        [HorseDataToShow("Relativt vinnarodds", "ShowVinnarOddsRelative", DataToShowUsage.Everywhere, 3, true)]
        [DataMember]
        public bool ShowVinnarOddsRelative
        {
            get
            {
                return showVinnarOddsRelative;
            }
            set
            {
                showVinnarOddsRelative = value;
                OnPropertyChanged();
            }
        }

        private bool showPlatsOdds;
        [HorseDataToShow("Platsodds", "ShowPlatsOdds", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowPlatsOdds
        {
            get
            {
                return showPlatsOdds;
            }
            set
            {
                showPlatsOdds = value;
                OnPropertyChanged();
            }
        }

        private bool showStakeDistributionPercent;
        [HorseDataToShow("Insatsfördelning", "ShowStakeDistributionPercent", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3)]
        [DataMember]
        public bool ShowStakeDistributionPercent
        {
            get
            {
                return showStakeDistributionPercent;
            }
            set
            {
                showStakeDistributionPercent = value;
                OnPropertyChanged();
            }
        }

        private bool showStakeDistributionShareAccumulated;
        [HorseDataToShow("Insatsförd. (ack)", "ShowStakeDistributionShareAccumulated", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3)]
        [DataMember]
        public bool ShowStakeDistributionShareAccumulated
        {
            get
            {
                return showStakeDistributionShareAccumulated;
            }
            set
            {
                showStakeDistributionShareAccumulated = value;
                OnPropertyChanged();
            }
        }

        private bool showMarkability;
        [HorseDataToShow("Streckbarhet", "ShowMarkability", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3, true)]
        [DataMember]
        public bool ShowMarkability
        {
            get
            {
                return showMarkability;
            }
            set
            {
                showMarkability = value;
                OnPropertyChanged();
            }
        }

        private bool showReserv;
        [HorseDataToShow("Reserv", "ShowReserv", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3, true)]
        [DataMember]
        public bool ShowReserv
        {
            get
            {
                return showReserv;
            }
            set
            {
                showReserv = value;
                OnPropertyChanged();
            }
        }

        private bool showRecord;
        [HorseDataToShow("Rekord", "ShowRecord", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowRecord
        {
            get
            {
                return showRecord;
            }
            set
            {
                showRecord = value;
                OnPropertyChanged();
            }
        }

        private bool showShape;
        //[HorseDataToShow("Namn", "ShowName", DataToShowUsage.All, 3)]
        [DataMember]
        public bool ShowShape
        {
            get
            {
                return showShape;
            }
            set
            {
                showShape = value;
                OnPropertyChanged();
            }
        }

        private bool showEarnings;
        [HorseDataToShow("Intjänat", "ShowEarnings", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowEarnings
        {
            get
            {
                return showEarnings;
            }
            set
            {
                showEarnings = value;
                OnPropertyChanged();
            }
        }

        private bool showEarningsMeanLast5;
        [HorseDataToShow("Intjänat senaste 5", "ShowEarningsMeanLast5", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowEarningsMeanLast5
        {
            get
            {
                return showEarningsMeanLast5;
            }
            set
            {
                showEarningsMeanLast5 = value;
                OnPropertyChanged();
            }
        }

        private bool showLocked;
        [HorseDataToShow("Lås", "ShowLocked", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3)]
        [DataMember]
        public bool ShowLocked
        {
            get
            {
                return showLocked;
            }
            set
            {
                showLocked = value;
                OnPropertyChanged();
            }
        }

        private bool showDaysSinceLastStart;
        //[HorseDataToShow("Senaste start", "ShowDaysSinceLastStart", DataToShowUsage.All, 3)]
        [DataMember]
        public bool ShowDaysSinceLastStart
        {
            get
            {
                return showDaysSinceLastStart;
            }
            set
            {
                showDaysSinceLastStart = value;
                OnPropertyChanged();
            }
        }

        private bool showDistance;
        [HorseDataToShow("Distans inklusive tillägg", "ShowDistance", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowDistance
        {
            get
            {
                return showDistance;
            }
            set
            {
                showDistance = value;
                OnPropertyChanged();
            }
        }

        private bool showShoeInfo;
        [HorseDataToShow("Skoinformation", "ShowShoeInfo", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowShoeInfo
        {
            get
            {
                return showShoeInfo;
            }
            set
            {
                showShoeInfo = value;
                OnPropertyChanged();
            }
        }

        private bool showSulkyInfo;
        [HorseDataToShow("Vagninformation", "ShowSulkyInfo", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowSulkyInfo
        {
            get
            {
                return showSulkyInfo;
            }
            set
            {
                showSulkyInfo = value;
                OnPropertyChanged();
            }
        }

        private bool showComments;
        [HorseDataToShow("Kommentarer", "ShowComments", DataToShowUsage.None, 3, true)]
        [DataMember]
        public bool ShowComments
        {
            get
            {
                return showComments;
            }
            set
            {
                showComments = value;
                OnPropertyChanged();
            }
        }

        private bool showLastStartDate;
        [HorseDataToShow("Senaste start", "ShowLastStartDate", DataToShowUsage.Everywhere, 3)]
        [DataMember]
        public bool ShowLastStartDate
        {
            get
            {
                return showLastStartDate;
            }
            set
            {
                showLastStartDate = value;
                OnPropertyChanged();
            }
        }

        private bool showResultRow;
        [HorseDataToShow("Resultatrad", "ShowResultRow", DataToShowUsage.Everywhere, 8)]
        [DataMember]
        public bool ShowResultRow
        {
            get
            {
                return showResultRow;
            }
            set
            {
                showResultRow = value;
                OnPropertyChanged();
            }
        }

        private bool showSystemCoverage;
        [HorseDataToShow("Täckning", "ShowSystemCoverage", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 9)]
        [DataMember]
        public bool ShowSystemCoverage
        {
            get
            {
                return showSystemCoverage;
            }
            set
            {
                showSystemCoverage = value;
                OnPropertyChanged();
            }
        }

        private bool showRankMean;
        [HorseDataToShow("Snittrank", "ShowRankMean", DataToShowUsage.Everywhere, 10)]
        [DataMember]
        public bool ShowRankMean
        {
            get
            {
                return showRankMean;
            }
            set
            {
                showRankMean = value;
                OnPropertyChanged();
            }
        }

        private bool showRankOwn;
        [HorseDataToShow("Egen rank", "ShowRankOwn", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowRankOwn
        {
            get
            {
                return showRankOwn;
            }
            set
            {
                showRankOwn = value;
                OnPropertyChanged();
            }
        }

        private bool showRankAlternate;
        [HorseDataToShow("Poäng", "ShowRankAlternate", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowRankAlternate
        {
            get
            {
                return showRankAlternate;
            }
            set
            {
                showRankAlternate = value;
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

        private bool showOwnInformation;
        [HorseDataToShow("Egen info", "ShowOwnInformation", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowOwnInformation
        {
            get
            {
                return showOwnInformation;
            }
            set
            {
                showOwnInformation = value;
                OnPropertyChanged();
            }
        }

        private bool showDoubleShare;
        [HorseDataToShow("DD/LD-andel", "ShowDoubleShare", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowDoubleShare
        {
            get
            {
                return showDoubleShare;
            }
            set
            {
                showDoubleShare = value;
                OnPropertyChanged();
            }
        }

        private bool showTvillingShare;
        [HorseDataToShow("Tvillingandel", "ShowTvillingShare", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowTvillingShare
        {
            get
            {
                return showTvillingShare;
            }
            set
            {
                showTvillingShare = value;
                OnPropertyChanged();
            }
        }

        private bool showTrioShare;
        [HorseDataToShow("Trio-andel", "ShowTrioShare", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowTrioShare
        {
            get
            {
                return showTrioShare;
            }
            set
            {
                showTrioShare = value;
                OnPropertyChanged();
            }
        }

        private bool showPlatsShare;
        [HorseDataToShow("Platsandel", "ShowPlatsShare", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowPlatsShare
        {
            get
            {
                return showPlatsShare;
            }
            set
            {
                showPlatsShare = value;
                OnPropertyChanged();
            }
        }

        private bool showStakeShare;
        //[HorseDataToShow("Alt. insatsfördelning", "ShowStakeShare", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 11, true)]
        [HorseDataToShow("Alt. insatsfördelning", "ShowStakeShare", DataToShowUsage.Everywhere, 11, true)]
        [DataMember]
        public bool ShowStakeShare
        {
            get
            {
                return showStakeShare;
            }
            set
            {
                showStakeShare = value;
                OnPropertyChanged();
            }
        }

        private bool showStakeShareRelativeToFavourite;
        [HorseDataToShow("Insatsfördelning relativt favoriten", "ShowStakeShareRelativeToFavourite", DataToShowUsage.Vxx, 11, true)]
        [DataMember]
        public bool ShowStakeShareRelativeToFavourite
        {
            get
            {
                return showStakeShareRelativeToFavourite;
            }
            set
            {
                showStakeShareRelativeToFavourite = value;
                OnPropertyChanged();
            }
        }

        private bool showStakeShareRelativeToNext;
        [HorseDataToShow("Insatsfördelning relativt föregående- och nästrankad", "ShowStakeShareRelativeToNext", DataToShowUsage.Vxx, 12, true)]
        [DataMember]
        public bool ShowStakeShareRelativeToNext
        {
            get
            {
                return showStakeShareRelativeToNext;
            }
            set
            {
                showStakeShareRelativeToNext = value;
                OnPropertyChanged();
            }
        }

        private bool showHeadToHead;
        [HorseDataToShow("Inbördes möten", "ShowHeadToHead", DataToShowUsage.Everywhere, 12, true)]
        [DataMember]
        public bool ShowHeadToHead
        {
            get
            {
                return showHeadToHead;
            }
            set
            {
                showHeadToHead = value;
                OnPropertyChanged();
            }
        }

        private bool showRelativeDifference;
        [HorseDataToShow("Insatsutveckling", "ShowRelativeDifference", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 12, true)]
        [DataMember]
        public bool ShowRelativeDifference
        {
            get
            {
                return showRelativeDifference;
            }
            set
            {
                showRelativeDifference = value;
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

        private bool showSTHorseLink;
        [HorseDataToShow("Hästinfo på ST", "ShowSTHorseLink", DataToShowUsage.Everywhere, 12, true)]
        [DataMember]
        public bool ShowSTHorseLink
        {
            get
            {
                return showSTHorseLink;
            }
            set
            {
                showSTHorseLink = value;
                OnPropertyChanged();
            }
        }

        private bool showResultInfo;
        [HorseDataToShow("Dagens resultat", "ShowResultInfo", DataToShowUsage.Everywhere, 12, true)]
        [DataMember]
        public bool ShowResultInfo
        {
            get
            {
                return showResultInfo;
            }
            set
            {
                showResultInfo = value;
                OnPropertyChanged();
            }
        }

        #region Specialfält som inte är konfigurerbara

        private bool showComplimentaryRuleSelect;
        [HorseDataToShow("Utgång", "ShowComplimentaryRuleSelect", DataToShowUsage.None, 2)]
        [DataMember]
        public bool ShowComplimentaryRuleSelect
        {
            get
            {
                return showComplimentaryRuleSelect;
            }
            set
            {
                showComplimentaryRuleSelect = value;
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

        private bool showMarksShare;
        //[HorseDataToShow("Streckandel", "ShowMarksShare", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3)]
        //[DataMember]
        public bool ShowMarksShare
        {
            get
            {
                return showMarksShare;
            }
            set
            {
                showMarksShare = value;
                OnPropertyChanged();
            }
        }

        private bool showMarksQuantity;
        //[HorseDataToShow("Streckantal", "ShowMarksQuantity", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3)]
        //[DataMember]
        public bool ShowMarksQuantity
        {
            get
            {
                return showMarksQuantity;
            }
            set
            {
                showMarksQuantity = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Obsolete ranks

        private bool showRankATG;
        [HorseDataToShow("ATG-Rank", "ShowRankATG", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 20)]
        [DataMember]
        public bool ShowRankATG
        {
            get
            {
                return showRankATG;
            }
            set
            {
                showRankATG = value;
                OnPropertyChanged();
            }
        }

        private bool showRankVinnarOdds;
        [DataMember]
        public bool ShowRankVinnarOdds
        {
            get
            {
                return showRankVinnarOdds;
            }
            set
            {
                showRankVinnarOdds = value;
                OnPropertyChanged();
            }
        }

        private bool showRankMarks;
        [DataMember]
        public bool ShowRankMarks
        {
            get
            {
                return showRankMarks;
            }
            set
            {
                showRankMarks = value;
                OnPropertyChanged();
            }
        }

        private bool showRankMarkability;
        [DataMember]
        public bool ShowRankMarkability
        {
            get
            {
                return showRankMarkability;
            }
            set
            {
                showRankMarkability = value;
                OnPropertyChanged();
            }
        }

        private bool showRankRecord;
        [DataMember]
        public bool ShowRankRecord
        {
            get
            {
                return showRankRecord;
            }
            set
            {
                showRankRecord = value;
                OnPropertyChanged();
            }
        }

        private bool showRankShape;
        [DataMember]
        public bool ShowRankShape
        {
            get
            {
                return showRankShape;
            }
            set
            {
                showRankShape = value;
                OnPropertyChanged();
            }
        }

        private bool showRankEarnings;
        [DataMember]
        public bool ShowRankEarnings
        {
            get
            {
                return showRankEarnings;
            }
            set
            {
                showRankEarnings = value;
                OnPropertyChanged();
            }
        }

        private bool showRankEarningsMeanLast5;
        [DataMember]
        public bool ShowRankEarningsMeanLast5
        {
            get
            {
                return showRankEarningsMeanLast5;
            }
            set
            {
                showRankEarningsMeanLast5 = value;
                OnPropertyChanged();
            }
        }

        private bool showRankRecordWeighedTotal;
        [DataMember]
        public bool ShowRankRecordWeighedTotal
        {
            get
            {
                return showRankRecordWeighedTotal;
            }
            set
            {
                showRankRecordWeighedTotal = value;
                OnPropertyChanged();
            }
        }

        private bool showRankRecordWeighedLast5;
        [DataMember]
        public bool ShowRankRecordWeighedLast5
        {
            get
            {
                return showRankRecordWeighedLast5;
            }
            set
            {
                showRankRecordWeighedLast5 = value;
                OnPropertyChanged();
            }
        }

        private bool showRankEarningsMeanThisYear;
        [DataMember]
        public bool ShowRankEarningsMeanThisYear
        {
            get
            {
                return showRankEarningsMeanThisYear;
            }
            set
            {
                showRankEarningsMeanThisYear = value;
                OnPropertyChanged();
            }
        }

        private bool showRankEarningsMeanLastYear;
        [DataMember]
        public bool ShowRankEarningsMeanLastYear
        {
            get
            {
                return showRankEarningsMeanLastYear;
            }
            set
            {
                showRankEarningsMeanLastYear = value;
                OnPropertyChanged();
            }
        }

        private bool showRankTotalEarningsMean;
        [DataMember]
        public bool ShowRankTotalEarningsMean
        {
            get
            {
                return showRankTotalEarningsMean;
            }
            set
            {
                showRankTotalEarningsMean = value;
                OnPropertyChanged();
            }
        }

        private bool showRankPlatsodds;
        [DataMember]
        public bool ShowRankPlatsodds
        {
            get
            {
                return showRankPlatsodds;
            }
            set
            {
                showRankPlatsodds = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
