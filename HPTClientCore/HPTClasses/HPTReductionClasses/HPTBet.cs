using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTBet : Notifier, IHorseListContainer
    {
        public HPTBet()
        {
            Config = HPTConfig.Config;
        }

        public HPTBet(HPTRaceDayInfo rdi, HPTBetType bt)
        {
            Config = HPTConfig.Config;
            TimeStamp = DateTime.Now;
            BetType = bt;
            RaceDayInfo = rdi;
            foreach (var race in rdi.RaceList)
            {
                switch (bt.Code)
                {
                    case "TV":
                        race.LegNrString = $"Lopp {race.LegNr}";
                        break;
                    case "T":
                        race.LegNrString = $"Trio-{race.LegNr}";
                        break;
                    default:
                        race.LegNrString = $"{bt.Code}-{race.LegNr}";
                        break;
                }
                race.Reserv1GroupName = $"{race.LegNr}-1";
                race.Reserv2GroupName = $"{race.LegNr}-2";
            }
            var s = HorseRankVariableList.ToString();
        }

        [DataMember]
        public string SystemName
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
                HasSystemName = !string.IsNullOrEmpty(value);
            }
        }

        public virtual string SystemNameForIdentification
        {
            get
            {
                if (HasSystemName)
                {
                    return SystemName;
                }
                return string.Empty;
            }
        }

        public bool HasSystemName
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
                HasNoSystemName = !value;
            }
        }

        public bool HasNoSystemName
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = true;

        [DataMember]
        public DateTime TimeStamp
        {
            get;
            set
            {
                field = value;
                //TimeStampString = "Uppdatering (" + timeStamp.ToShortTimeString() + ")";
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string TimeStampString
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
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
            UniqueIdentifier = Guid.NewGuid().ToString("N");   // 00000000000000000000000000000000
        }

        [XmlIgnore]
        public HPTConfig Config { get; set; }

        public decimal PoolShare
        {
            get
            {
                return BetType.PoolShare;
            }
        }

        [XmlIgnore]
        public bool IsDeserializing { get; set; }

        [XmlIgnore]
        public bool IsSimulated { get; set; }

        [DataMember]
        public string SystemFilename
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        #region Rankvariable lists

        internal virtual void ApplyConfigRankVariables(HPTRankTemplate rankTemplate)
        {
            if (rankTemplate == null)
            {
                return;
            }
            if (HorseRankVariableList.Count > rankTemplate.HorseRankVariableList.Count)
            {
                var newPropertyNames = HorseRankVariableList
                    .Select(hrv => hrv.PropertyName)
                    .Except(rankTemplate.HorseRankVariableList.Select(hrv => hrv.PropertyName));

                foreach (var propertyName in newPropertyNames)
                {
                    var newRankVariable = HorseRankVariableList.First(hrv => hrv.PropertyName == propertyName);
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
                var hptHorseRankVariableToSet = HorseRankVariableList.FirstOrDefault(rv => rv.PropertyName == hptHorseRankVariable.PropertyName);
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
                var variablesToUse = HorseRankVariableList.Where(v => v.Use).ToList();
                return variablesToUse;
            }
        }

        public void RecalculateAllRanks()
        {
            //if (this.IsDeserializing)
            //{
            //    return;
            //}
            foreach (var variable in HorseRankVariableList)
            {
                foreach (var race in RaceDayInfo.RaceList)
                {
                    try
                    {
                        variable.SortHorseList(race.HorseList.ToList());
                    }
                    catch (Exception exc)
                    {
                        var s = exc.Message;
                    }
                }
            }
        }

        internal void SetRankForHorsesWithSameValue(string propertyName)
        {
            foreach (var race in RaceDayInfo.RaceList)
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
            var variablesToUse = RankVariablesToUse;
            var weightSum = 0M;
            foreach (var variable in variablesToUse)
            {
                weightSum += variable.Weight;
                foreach (var race in RaceDayInfo.RaceList)
                {
                    variable.SortHorseList(race.HorseList.ToList());
                }
            }
            foreach (var race in RaceDayInfo.RaceList)
            {
                foreach (var horse in race.HorseList)
                {
                    var totalRank = 0;
                    var totalRankWeighted = 0M;
                    if (horse.Scratched == false || horse.Scratched == null)
                    {
                        var variablesToExclude = 0;
                        var weightToRemove = 0M;
                        foreach (var variable in variablesToUse)
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
                            var numberOfVariablesToUse = variablesToUse.Count - variablesToExclude;
                            var weightSumToUse = weightSum - weightToRemove;
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
            horseRankVariableListMarksAndOdds = CreateRankVariableList(HPTRankCategory.MarksAndOdds);
            horseRankVariableListRecords = CreateRankVariableList(HPTRankCategory.Record);
            horseRankVariableListWinning = CreateRankVariableList(HPTRankCategory.Winnings);
            horseRankVariableListPlace = CreateRankVariableList(HPTRankCategory.Place);
            horseRankVariableListTop3 = CreateRankVariableList(HPTRankCategory.Top3);
            horseRankVariableListRest = CreateRankVariableList(HPTRankCategory.Rest);
        }

        [DataMember]
        public List<HPTHorseRankVariable> HorseRankVariableList
        {
            get
            {
                if (field == null && !IsDeserializing)
                {
                    field = HPTHorseRankVariable.CreateVariableList();
                }
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariablesPoints
        {
            get
            {
                if (field == null)
                {
                    field = new ObservableCollection<HPTHorseRankVariable>(
                        HorseRankVariableList
                        .Where(hrv => hrv.PropertyName == "RankABC" || hrv.PropertyName == "RankOwn" || hrv.PropertyName == "RankAlternate")
                        );
                }
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged("HorseRankVariablesToShowList");
            }
        }

        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariablesToShowList
        {
            get
            {
                if (field == null)
                {
                    field = new ObservableCollection<HPTHorseRankVariable>();
                    foreach (var horseRankVariableBase in HPTConfig.Config.HorseRankVariablesToShow.Where(hra => hra.Show))
                    {
                        var horseRankVariable = HorseRankVariableList.FirstOrDefault(hra => hra.PropertyName == horseRankVariableBase.PropertyName);
                        if (horseRankVariable != null)
                        {
                            field.Add(horseRankVariable);
                        }
                    }
                }
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListMarksAndOdds;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListMarksAndOdds
        {
            get
            {
                if (horseRankVariableListMarksAndOdds == null)
                {
                    horseRankVariableListMarksAndOdds = CreateRankVariableList(HPTRankCategory.MarksAndOdds);
                }
                return horseRankVariableListMarksAndOdds;
            }
            set
            {
                horseRankVariableListMarksAndOdds = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListRecords;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListRecords
        {
            get
            {
                if (horseRankVariableListRecords == null)
                {
                    horseRankVariableListRecords = CreateRankVariableList(HPTRankCategory.Record);
                }
                return horseRankVariableListRecords;
            }
            set
            {
                horseRankVariableListRecords = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListWinning;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListWinning
        {
            get
            {
                if (horseRankVariableListWinning == null)
                {
                    horseRankVariableListWinning = CreateRankVariableList(HPTRankCategory.Winnings);
                }
                return horseRankVariableListWinning;
            }
            set
            {
                horseRankVariableListWinning = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListPlace;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListPlace
        {
            get
            {
                if (horseRankVariableListPlace == null)
                {
                    horseRankVariableListPlace = CreateRankVariableList(HPTRankCategory.Place);
                }
                return horseRankVariableListPlace;
            }
            set
            {
                horseRankVariableListPlace = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListTop3;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListTop3
        {
            get
            {
                if (horseRankVariableListTop3 == null)
                {
                    horseRankVariableListTop3 = CreateRankVariableList(HPTRankCategory.Top3);
                }
                return horseRankVariableListTop3;
            }
            set
            {
                horseRankVariableListTop3 = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListRest;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListRest
        {
            get
            {
                if (horseRankVariableListRest == null)
                {
                    horseRankVariableListRest = CreateRankVariableList(HPTRankCategory.Rest);
                }
                return horseRankVariableListRest;
            }
            set
            {
                horseRankVariableListRest = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<HPTHorseRankVariable> CreateRankVariableList(HPTRankCategory category)
        {
            if (HorseRankVariableList != null)
            {
                IEnumerable<HPTHorseRankVariable> tempList = HorseRankVariableList.Where(hrv => hrv.Category == category).OrderBy(rv => rv.Order);
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
                .Where(ab => ab.RaceDayInfo.TrackId == RaceDayInfo.TrackId)
                .Where(ab => ab.RaceDayInfo.RaceDayDate.Date == RaceDayInfo.RaceDayDate.Date)
                .Where(ab => ab != this)
                .Where(ab => ab.RaceDayInfo.RaceNumberList.Intersect(RaceDayInfo.RaceNumberList).Count() > 0)
                .ToList();

            return betList;
        }

        internal void ApplyOwnRanks(HPTBet bet)
        {
            RaceDayInfo.RaceList.ForEach(r =>
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
            RaceDayInfo.RaceList.ForEach(r =>
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
