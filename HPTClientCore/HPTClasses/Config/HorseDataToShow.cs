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
                OnPropertyChanged("ShowHorsePopup");
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
                OnPropertyChanged("ShowDriverPopup");
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
                OnPropertyChanged("ShowTrainerPopup");
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
                OnPropertyChanged("ShowStartNr");
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
                OnPropertyChanged("ShowPrio");
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
                OnPropertyChanged("ShowName");
            }
        }

        private bool showSex;
        [HorseDataToShow("Kön", "ShowSex", DataToShowUsage.Everywhere, 4)]
        [DataMember]
        public bool ShowSex
        {
            get
            {
                return this.showSex;
            }
            set
            {
                this.showSex = value;
                OnPropertyChanged("ShowSex");
            }
        }

        private bool showAge;
        [HorseDataToShow("Ålder", "ShowAge", DataToShowUsage.Everywhere, 5)]
        [DataMember]
        public bool ShowAge
        {
            get
            {
                return this.showAge;
            }
            set
            {
                this.showAge = value;
                OnPropertyChanged("ShowAge");
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
                OnPropertyChanged("ShowTrack");
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
                OnPropertyChanged("ShowDriver");
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
                OnPropertyChanged("ShowTrainer");
            }
        }

        private bool showOwner;
        [HorseDataToShow("Ägare", "ShowOwner", DataToShowUsage.Everywhere, 3, true)]
        [DataMember]
        public bool ShowOwner
        {
            get
            {
                return this.showOwner;
            }
            set
            {
                this.showOwner = value;
                OnPropertyChanged("ShowOwner");
            }
        }

        private bool showOwnProbability;
        [HorseDataToShow("Egen chansvärdering", "ShowOwnProbability", DataToShowUsage.Everywhere, 3, true)]
        [DataMember]
        public bool ShowOwnProbability
        {
            get
            {
                return this.showOwnProbability;
            }
            set
            {
                this.showOwnProbability = value;
                OnPropertyChanged("ShowOwnProbability");
            }
        }

        private bool showBreeder;
        [HorseDataToShow("Uppfödare", "ShowBreeder", DataToShowUsage.Everywhere, 3, true)]
        [DataMember]
        public bool ShowBreeder
        {
            get
            {
                return this.showBreeder;
            }
            set
            {
                this.showBreeder = value;
                OnPropertyChanged("ShowBreeder");
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
                OnPropertyChanged("ShowVinnarOdds");
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
                OnPropertyChanged("ShowVinnarOddsShare");
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
                OnPropertyChanged("ShowVinnarOddsRelative");
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
                OnPropertyChanged("ShowPlatsOdds");
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
                OnPropertyChanged("ShowStakeDistributionPercent");
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
                OnPropertyChanged("ShowStakeDistributionShareAccumulated");
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
                OnPropertyChanged("ShowMarkability");
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
                OnPropertyChanged("ShowReserv");
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
                OnPropertyChanged("ShowRecord");
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
                OnPropertyChanged("ShowShape");
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
                OnPropertyChanged("ShowEarnings");
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
                OnPropertyChanged("ShowEarningsMeanLast5");
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
                OnPropertyChanged("ShowLocked");
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
                OnPropertyChanged("ShowDaysSinceLastStart");
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
                OnPropertyChanged("ShowDistance");
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
                OnPropertyChanged("ShowShoeInfo");
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
                OnPropertyChanged("ShowSulkyInfo");
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
                OnPropertyChanged("ShowComments");
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
                OnPropertyChanged("ShowLastStartDate");
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
                OnPropertyChanged("ShowResultRow");
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
                OnPropertyChanged("ShowSystemCoverage");
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
                OnPropertyChanged("ShowRankMean");
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
                OnPropertyChanged("ShowRankOwn");
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
                OnPropertyChanged("ShowRankAlternate");
            }
        }

        private bool showRankTip;
        [HorseDataToShow("Tipsrank", "ShowRankTip", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 12, true)]
        [DataMember]
        public bool ShowRankTip
        {
            get
            {
                return showRankTip;
            }
            set
            {
                showRankTip = value;
                OnPropertyChanged("ShowRankTip");
            }
        }

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
                OnPropertyChanged("ShowOwnInformation");
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
                OnPropertyChanged("ShowDoubleShare");
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
                OnPropertyChanged("ShowTvillingShare");
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
                OnPropertyChanged("ShowTrioShare");
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
                OnPropertyChanged("ShowPlatsShare");
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
                OnPropertyChanged("ShowStakeShare");
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
                OnPropertyChanged("ShowHeadToHead");
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
                OnPropertyChanged("ShowRelativeDifference");
            }
        }

        private bool showRelativeDifferenceVinnare;
        [HorseDataToShow("Insatsutveckling (V)", "ShowRelativeDifferenceVinnare", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 12, true)]
        [DataMember]
        public bool ShowRelativeDifferenceVinnare
        {
            get
            {
                return showRelativeDifferenceVinnare;
            }
            set
            {
                showRelativeDifferenceVinnare = value;
                OnPropertyChanged("ShowRelativeDifferenceVinnare");
            }
        }

        private bool showRelativeDifferencePlats;
        [HorseDataToShow("Insatsutveckling (P)", "ShowRelativeDifferencePlats", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 12, true)]
        [DataMember]
        public bool ShowRelativeDifferencePlats
        {
            get
            {
                return showRelativeDifferencePlats;
            }
            set
            {
                showRelativeDifferencePlats = value;
                OnPropertyChanged("ShowRelativeDifferencePlats");
            }
        }

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
                OnPropertyChanged("ShowSTHorseLink");
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
                OnPropertyChanged("ShowResultInfo");
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
                OnPropertyChanged("ShowComplimentaryRuleSelect");
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

        private bool showMarksPercent;
        //[HorseDataToShow("Streckprocent", "ShowMarksPercent", DataToShowUsage.Vxx | DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction, 3)]
        //[DataMember]
        public bool ShowMarksPercent
        {
            get
            {
                return showMarksPercent;
            }
            set
            {
                showMarksPercent = value;
                OnPropertyChanged("ShowMarksPercent");
            }
        }

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
                OnPropertyChanged("ShowMarksShare");
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
                OnPropertyChanged("ShowMarksQuantity");
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
                OnPropertyChanged("ShowRankATG");
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
                OnPropertyChanged("ShowRankVinnarOdds");
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
                OnPropertyChanged("ShowRankMarks");
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
                OnPropertyChanged("ShowRankMarkability");
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
                OnPropertyChanged("ShowRankRecord");
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
                OnPropertyChanged("ShowRankShape");
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
                OnPropertyChanged("ShowRankEarnings");
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
                OnPropertyChanged("ShowRankEarningsMeanLast5");
            }
        }

        private bool showRankRecordWeighedTotal;
        [DataMember]
        public bool ShowRankRecordWeighedTotal
        {
            get
            {
                return this.showRankRecordWeighedTotal;
            }
            set
            {
                this.showRankRecordWeighedTotal = value;
                OnPropertyChanged("ShowRankRecordWeighedTotal");
            }
        }

        private bool showRankRecordWeighedLast5;
        [DataMember]
        public bool ShowRankRecordWeighedLast5
        {
            get
            {
                return this.showRankRecordWeighedLast5;
            }
            set
            {
                this.showRankRecordWeighedLast5 = value;
                OnPropertyChanged("ShowRankRecordWeighedLast5");
            }
        }

        private bool showRankEarningsMeanThisYear;
        [DataMember]
        public bool ShowRankEarningsMeanThisYear
        {
            get
            {
                return this.showRankEarningsMeanThisYear;
            }
            set
            {
                this.showRankEarningsMeanThisYear = value;
                OnPropertyChanged("ShowRankEarningsMeanThisYear");
            }
        }

        private bool showRankEarningsMeanLastYear;
        [DataMember]
        public bool ShowRankEarningsMeanLastYear
        {
            get
            {
                return this.showRankEarningsMeanLastYear;
            }
            set
            {
                this.showRankEarningsMeanLastYear = value;
                OnPropertyChanged("ShowRankEarningsMeanLastYear");
            }
        }

        private bool showRankTotalEarningsMean;
        [DataMember]
        public bool ShowRankTotalEarningsMean
        {
            get
            {
                return this.showRankTotalEarningsMean;
            }
            set
            {
                this.showRankTotalEarningsMean = value;
                OnPropertyChanged("ShowRankTotalEarningsMean");
            }
        }

        private bool showRankPlatsodds;
        [DataMember]
        public bool ShowRankPlatsodds
        {
            get
            {
                return this.showRankPlatsodds;
            }
            set
            {
                this.showRankPlatsodds = value;
                OnPropertyChanged("ShowRankPlatsodds");
            }
        }

        #endregion
    }
}
