using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace HPTClient
{
    [DataContract]
    public class HPTBet : Notifier, IHorseListContainer
    {
        public HPTBet()
        {
            this.Config = HPTConfig.Config;
        }

        public HPTBet(HPTRaceDayInfo rdi, HPTBetType bt)
        {
            this.Config = HPTConfig.Config;
            this.TimeStamp = DateTime.Now;
            this.BetType = bt;
            this.RaceDayInfo = rdi;
            foreach (HPTRace race in rdi.RaceList)
            {
                switch (bt.Code)
                {
                    case "TV":
                        race.LegNrString = "Lopp " + race.LegNr.ToString();
                        break;
                    case "T":
                        race.LegNrString = "Trio" + "-" + race.LegNr.ToString();
                        break;
                    default:
                        race.LegNrString = bt.Code + "-" + race.LegNr.ToString();
                        break;
                }
                race.Reserv1GroupName = race.LegNr.ToString() + "-" + "1";
                race.Reserv2GroupName = race.LegNr.ToString() + "-" + "2";
            }
            string s = this.HorseRankVariableList.ToString();
        }

        private string systemName;
        [DataMember]
        public string SystemName
        {
            get
            {
                return this.systemName;
            }
            set
            {
                this.systemName = value;
                OnPropertyChanged("SystemName");
                this.HasSystemName = !string.IsNullOrEmpty(value);
            }
        }

        public virtual string SystemNameForIdentification
        {
            get
            {
                if (this.HasSystemName)
                {
                    return this.SystemName;
                }
                return string.Empty;
            }
        }
        private bool hasSystemName;
        public bool HasSystemName
        {
            get
            {
                return this.hasSystemName;
            }
            set
            {
                this.hasSystemName = value;
                OnPropertyChanged("HasSystemName");
                this.HasNoSystemName = !value;
            }
        }

        private bool hasNoSystemName = true;
        public bool HasNoSystemName
        {
            get
            {
                return this.hasNoSystemName;
            }
            set
            {
                this.hasNoSystemName = value;
                OnPropertyChanged("HasNoSystemName");
            }
        }

        private DateTime timeStamp;
        [DataMember]
        public DateTime TimeStamp
        {
            get
            {
                return this.timeStamp;
            }
            set
            {
                this.timeStamp = value;
                this.TimeStampString = "Uppdatering (" + this.timeStamp.ToShortTimeString() + ")";
                OnPropertyChanged("TimeStamp");
            }
        }

        private string timeStampString;
        [DataMember]
        public string TimeStampString
        {
            get
            {
                return this.timeStampString;
            }
            set
            {
                this.timeStampString = value;
                OnPropertyChanged("TimeStampString");
            }
        }

        [DataMember]
        public int Turnover { get; set; }

        [DataMember]
        public HPTRaceDayInfo RaceDayInfo { get; set; }

        [DataMember]
        public HPTBetType BetType { get; set; }

        [DataMember]
        public string SaveDirectory { get; set; }

        [DataMember]
        public string UniqueIdentifier { get; set; }

        public int RaceNumberToLoad { get; set; }

        public void UploadSystem()
        {
            this.UniqueIdentifier = Guid.NewGuid().ToString("N");   // 00000000000000000000000000000000
        }

        [XmlIgnore]
        public HPTConfig Config { get; set; }

        public decimal PoolShare
        {
            get
            {
                return this.BetType.PoolShare;
            }
        }

        [XmlIgnore]
        public bool IsDeserializing { get; set; }

        [XmlIgnore]
        public bool IsSimulated { get; set; }

        private string systemFilename;
        [DataMember]
        public string SystemFilename
        {
            get
            {
                return this.systemFilename;
            }
            set
            {
                this.systemFilename = value;
                OnPropertyChanged("SystemFilename");
            }
        }

        #region Rankvariable lists

        internal virtual void ApplyConfigRankVariables(HPTRankTemplate rankTemplate)
        {
            if (rankTemplate == null)
            {
                return;
            }
            if (this.HorseRankVariableList.Count > rankTemplate.HorseRankVariableList.Count)
            {
                var newPropertyNames = this.HorseRankVariableList
                    .Select(hrv => hrv.PropertyName)
                    .Except(rankTemplate.HorseRankVariableList.Select(hrv => hrv.PropertyName));

                foreach (var propertyName in newPropertyNames)
                {
                    var newRankVariable = this.HorseRankVariableList.First(hrv => hrv.PropertyName == propertyName);
                    var clonedRankVariable = new HPTHorseRankVariable()
                    {
                        Calculated = newRankVariable.Calculated,
                        Category = newRankVariable.Category,
                        CategoryText = newRankVariable.CategoryText,
                        Descending = newRankVariable.Descending,
                        HorseRankInfo = newRankVariable.HorseRankInfo,
                        IsStatic = newRankVariable.IsStatic,
                        PropertyName = newRankVariable.PropertyName,
                        Sort = newRankVariable.Sort,
                        Text = newRankVariable.Text,
                        Weight = 1M
                    };
                    rankTemplate.HorseRankVariableList.Add(clonedRankVariable);
                }
            }
            foreach (var hptHorseRankVariable in rankTemplate.HorseRankVariableList)
            {
                var hptHorseRankVariableToSet = this.HorseRankVariableList.FirstOrDefault(rv => rv.PropertyName == hptHorseRankVariable.PropertyName);
                if (hptHorseRankVariableToSet != null)
                {
                    hptHorseRankVariableToSet.Use = hptHorseRankVariable.Use;
                    hptHorseRankVariableToSet.Weight = hptHorseRankVariable.Weight;
                }
            }
        }

        internal List<HPTHorseRankVariable> RankVariablesToUse
        {
            get
            {
                List<HPTHorseRankVariable> variablesToUse = this.HorseRankVariableList.Where(v => v.Use).ToList();
                return variablesToUse;
            }
        }

        public void RecalculateAllRanks()
        {
            //if (this.IsDeserializing)
            //{
            //    return;
            //}
            foreach (HPTHorseRankVariable variable in this.HorseRankVariableList)
            {
                foreach (HPTRace race in this.RaceDayInfo.RaceList)
                {
                    try
                    {
                        variable.SortHorseList(race.HorseList.ToList());
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                }
            }
        }

        internal void SetRankForHorsesWithSameValue(string propertyName)
        {
            foreach (var race in this.RaceDayInfo.RaceList)
            {
                var horseRankList = race.HorseList.SelectMany(h => h.RankList).Where(hr => hr.Name == propertyName);
            }
        }

        public void RecalculateRank()
        {
            //if (this.IsDeserializing)
            //{
            //    return;
            //}
            List<HPTHorseRankVariable> variablesToUse = RankVariablesToUse;
            decimal weightSum = 0M;
            foreach (HPTHorseRankVariable variable in variablesToUse)
            {
                weightSum += variable.Weight;
                foreach (HPTRace race in this.RaceDayInfo.RaceList)
                {
                    variable.SortHorseList(race.HorseList.ToList());
                }
            }
            foreach (HPTRace race in this.RaceDayInfo.RaceList)
            {
                foreach (HPTHorse horse in race.HorseList)
                {
                    int totalRank = 0;
                    decimal totalRankWeighted = 0M;
                    if (horse.Scratched == false || horse.Scratched == null)
                    {
                        int variablesToExclude = 0;
                        decimal weightToRemove = 0M;
                        foreach (HPTHorseRankVariable variable in variablesToUse)
                        {
                            var horseRank = horse.RankList.First(r => r.Name == variable.PropertyName);
                            horseRank.Use = true;
                            if ((variable.PropertyName == "RecordWeighedTotal" && horse.RecordWeighedTotal == 300M) || (variable.PropertyName == "RecordTime" && horse.RecordTime == 300M))
                            {
                                variablesToExclude++;
                                weightToRemove -= variable.Weight;
                                horseRank.BackColor = new SolidColorBrush(Colors.LightBlue);
                            }
                            else
                            {
                                totalRank += horseRank.Rank;
                                totalRankWeighted += horseRank.RankWeighted;
                            }
                        }
                        if (variablesToUse.Count > 0)
                        {
                            int numberOfVariablesToUse = variablesToUse.Count - variablesToExclude;
                            decimal weightSumToUse = weightSum - weightToRemove;
                            if (numberOfVariablesToUse > 0)
                            {
                                horse.RankMean = Convert.ToDecimal(totalRank) / Convert.ToDecimal(numberOfVariablesToUse);
                                horse.RankWeighted = totalRankWeighted / weightSumToUse;
                            }
                            else
                            {
                                var horseRank = horse.RankList.First(r => r.Name == "StakeDistributionShare");
                                horse.RankMean = horseRank.Rank;
                                horse.RankWeighted = horseRank.Rank;
                            }
                        }
                    }
                    else
                    {
                        horse.RankMean = race.HorseList.Count;
                        horse.RankWeighted = race.HorseList.Count;
                    }
                }
            }
        }

        protected void CreateRankVariableList()
        {
            this.horseRankVariableListMarksAndOdds = CreateRankVariableList(HPTRankCategory.MarksAndOdds);
            this.horseRankVariableListRecords = CreateRankVariableList(HPTRankCategory.Record);
            this.horseRankVariableListWinning = CreateRankVariableList(HPTRankCategory.Winnings);
            this.horseRankVariableListPlace = CreateRankVariableList(HPTRankCategory.Place);
            this.horseRankVariableListTop3 = CreateRankVariableList(HPTRankCategory.Top3);
            this.horseRankVariableListRest = CreateRankVariableList(HPTRankCategory.Rest);
        }

        private List<HPTHorseRankVariable> horseRankVariableList;
        [DataMember]
        public List<HPTHorseRankVariable> HorseRankVariableList
        {
            get
            {
                if (this.horseRankVariableList == null && !this.IsDeserializing)
                {
                    this.horseRankVariableList = HPTHorseRankVariable.CreateVariableList();
                }
                return this.horseRankVariableList;
            }
            set
            {
                this.horseRankVariableList = value;
                OnPropertyChanged("HorseRankVariableList");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariablesPoints;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariablesPoints
        {
            get
            {
                if (this.horseRankVariablesPoints == null)
                {
                    this.horseRankVariablesPoints = new ObservableCollection<HPTHorseRankVariable>(
                        this.HorseRankVariableList
                        .Where(hrv => hrv.PropertyName == "RankABC" || hrv.PropertyName == "RankOwn" || hrv.PropertyName == "RankAlternate")
                        );
                }
                return this.horseRankVariablesPoints;
            }
            set
            {
                this.horseRankVariablesPoints = value;
                OnPropertyChanged("HorseRankVariablesToShowList");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariablesToShowList;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariablesToShowList
        {
            get
            {
                if (this.horseRankVariablesToShowList == null)
                {
                    this.horseRankVariablesToShowList = new ObservableCollection<HPTHorseRankVariable>();
                    foreach (var horseRankVariableBase in HPTConfig.Config.HorseRankVariablesToShow.Where(hra => hra.Show))
                    {
                        var horseRankVariable = this.HorseRankVariableList.FirstOrDefault(hra => hra.PropertyName == horseRankVariableBase.PropertyName);
                        if (horseRankVariable != null)
                        {
                            this.horseRankVariablesToShowList.Add(horseRankVariable);
                        }
                    }
                }
                return this.horseRankVariablesToShowList;
            }
            set
            {
                this.horseRankVariablesToShowList = value;
                OnPropertyChanged("HorseRankVariablesToShowList");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListMarksAndOdds;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListMarksAndOdds
        {
            get
            {
                if (this.horseRankVariableListMarksAndOdds == null)
                {
                    this.horseRankVariableListMarksAndOdds = CreateRankVariableList(HPTRankCategory.MarksAndOdds);
                }
                return this.horseRankVariableListMarksAndOdds;
            }
            set
            {
                this.horseRankVariableListMarksAndOdds = value;
                OnPropertyChanged("HorseRankVariableListMarksAndOdds");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListRecords;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListRecords
        {
            get
            {
                if (this.horseRankVariableListRecords == null)
                {
                    this.horseRankVariableListRecords = CreateRankVariableList(HPTRankCategory.Record);
                }
                return this.horseRankVariableListRecords;
            }
            set
            {
                this.horseRankVariableListRecords = value;
                OnPropertyChanged("HorseRankVariableListRecords");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListWinning;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListWinning
        {
            get
            {
                if (this.horseRankVariableListWinning == null)
                {
                    this.horseRankVariableListWinning = CreateRankVariableList(HPTRankCategory.Winnings);
                }
                return this.horseRankVariableListWinning;
            }
            set
            {
                this.horseRankVariableListWinning = value;
                OnPropertyChanged("HorseRankVariableListWinning");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListPlace;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListPlace
        {
            get
            {
                if (this.horseRankVariableListPlace == null)
                {
                    this.horseRankVariableListPlace = CreateRankVariableList(HPTRankCategory.Place);
                }
                return this.horseRankVariableListPlace;
            }
            set
            {
                this.horseRankVariableListPlace = value;
                OnPropertyChanged("HorseRankVariableListPlace");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListTop3;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListTop3
        {
            get
            {
                if (this.horseRankVariableListTop3 == null)
                {
                    this.horseRankVariableListTop3 = CreateRankVariableList(HPTRankCategory.Top3);
                }
                return this.horseRankVariableListTop3;
            }
            set
            {
                this.horseRankVariableListTop3 = value;
                OnPropertyChanged("HorseRankVariableListTop3");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListRest;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListRest
        {
            get
            {
                if (this.horseRankVariableListRest == null)
                {
                    this.horseRankVariableListRest = CreateRankVariableList(HPTRankCategory.Rest);
                }
                return this.horseRankVariableListRest;
            }
            set
            {
                this.horseRankVariableListRest = value;
                OnPropertyChanged("HorseRankVariableListRest");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> CreateRankVariableList(HPTRankCategory category)
        {
            if (this.HorseRankVariableList != null)
            {
                IEnumerable<HPTHorseRankVariable> tempList = this.HorseRankVariableList.Where(hrv => hrv.Category == category).OrderBy(rv => rv.Order);
                return new ObservableCollection<HPTHorseRankVariable>(tempList);
            }
            return new ObservableCollection<HPTHorseRankVariable>();
        }

        #endregion

        #region IHorseListContainer implementation

        [XmlIgnore]
        public ICollection<HPTHorse> HorseList { get; set; }

        [XmlIgnore]
        public HPTRaceDayInfo ParentRaceDayInfo { get; set; }

        #endregion

        internal IEnumerable<HPTBet> FindBetsWithOverlappingRaces()
        {
            var betList = HPTConfig.Config.AvailableBets
                .Where(ab => ab.RaceDayInfo.TrackId == this.RaceDayInfo.TrackId)
                .Where(ab => ab.RaceDayInfo.RaceDayDate.Date == this.RaceDayInfo.RaceDayDate.Date)
                .Where(ab => ab != this)
                .Where(ab => ab.RaceDayInfo.RaceNumberList.Intersect(this.RaceDayInfo.RaceNumberList).Count() > 0)
                .ToList();

            return betList;
        }

        internal void ApplyOwnRanks(HPTBet bet)
        {
            this.RaceDayInfo.RaceList.ForEach(r =>
            {
                var otherRace = bet.RaceDayInfo.RaceList.FirstOrDefault(or => or.RaceNr == r.RaceNr);
                if (otherRace != null)
                {
                    r.HorseList.ToList().ForEach(h =>
                    {
                        var otherHorse = otherRace.HorseList.First(oh => oh.StartNr == h.StartNr);
                        h.RankOwn = otherHorse.RankOwn;
                        h.RankAlternate = otherHorse.RankAlternate;
                    });
                }
            });
        }

        internal void ApplySelection(HPTBet bet)
        {
            this.RaceDayInfo.RaceList.ForEach(r =>
            {
                var otherRace = bet.RaceDayInfo.RaceList.FirstOrDefault(or => or.RaceNr == r.RaceNr);
                if (otherRace != null)
                {
                    r.HorseList.ToList().ForEach(h =>
                    {
                        var otherHorse = otherRace.HorseList.First(oh => oh.StartNr == h.StartNr);
                        h.Selected = otherHorse.Selected;
                    });
                }
            });
        }
    }
}
