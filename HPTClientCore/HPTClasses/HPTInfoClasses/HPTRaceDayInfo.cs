using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTRaceDayInfo : Notifier, IHorseListContainer
    {
        #region Egna events

        public event Action<object, EventArgs> ABCDChanged;

        public event Action ClearABCD;

        public event Action<HPTRankTemplate> RankTemplateChanged;

        #endregion

        public HPTRaceDayInfo()
        {
            this.ScratchedHorseInfo = new HPTScratchedHorsesInfo(this);
            this.ScratchedHorseInfo.HaveSelectedScratchedHorse = false;
            this.HorseListSelected = new ObservableCollection<HPTHorse>();
        }

        public void Merge(HPTService.HPTRaceDayInfo rdi)
        {
            try
            {
                this.MarksQuantity = rdi.MarksQuantity;
                if (rdi.Turnover != 0)
                {
                    this.Turnover = rdi.Turnover;
                }

                // Uppdatera storleken på vinstpotterna
                if (rdi.PayoutList != null)
                {
                    if (this.PayOutListATG == null)
                    {
                        HPTServiceToHPTHelper.CreatePayOutLists(rdi, this);
                    }
                    else
                    {
                        foreach (var payOut in rdi.PayoutList)
                        {
                            var payOutATG = this.PayOutListATG.FirstOrDefault(po => po.NumberOfCorrect == payOut.NumberOfCorrect);
                            if (payOutATG != null)
                            {
                                payOutATG.NumberOfSystems = payOut.NumberOfSystems;
                                payOutATG.PayOutAmount = payOut.PayOutAmount;
                                payOutATG.TotalAmount = payOut.TotalAmount;
                            }
                        }
                    }
                    //if (this.PayOutListATG != null && this.PayOutListATG.Count > 0 && this.PayOutListATG.First().TotalAmount == 0)
                    if (this.PayOutListATG != null && this.PayOutListATG.Count > 0)
                    {
                        if (this.BetType.Code == "V4" || this.BetType.Code == "V5")
                        {
                            this.PayOutListATG.First().TotalAmount += Convert.ToInt32(Convert.ToDecimal(this.Turnover) * this.BetType.PoolShare);
                        }

                        this.MaxPayOut = this.PayOutListATG
                            .OrderByDescending(po => po.NumberOfCorrect)
                            .First()
                            .TotalAmount;
                    }
                }
                foreach (HPTService.HPTRace race in rdi.LegList)
                {
                    HPTRace hptRace = null;
                    switch (this.BetType.TypeCategory)
                    {
                        case BetTypeCategory.None:
                            break;
                        case BetTypeCategory.V4:
                        case BetTypeCategory.V5:
                        case BetTypeCategory.V6X:
                        case BetTypeCategory.V75:
                        case BetTypeCategory.V86:
                        case BetTypeCategory.V85:
                        case BetTypeCategory.Double:
                            hptRace = this.RaceList.Where(hr => hr.LegNr == race.LegNr).FirstOrDefault();
                            break;
                        case BetTypeCategory.Trio:
                        case BetTypeCategory.Twin:
                            hptRace = this.RaceList.Where(hr => hr.RaceNr == race.SharedInfo.RaceNr).FirstOrDefault();
                            break;
                        default:
                            break;
                    }

                    if (hptRace != null)
                    {
                        hptRace.Merge(race);
                    }
                }

                // Hantera potterna för olika antal rätt
                if (rdi.PayoutList != null)
                {
                    IEnumerable<HPTPayOut> payOutList = rdi.PayoutList.Select(po => new HPTPayOut()
                    {
                        NumberOfCorrect = po.NumberOfCorrect,
                        NumberOfSystems = po.NumberOfSystems,
                        PayOutAmount = po.PayOutAmount,
                        TotalAmount = po.TotalAmount
                    });
                    this.PayOutListATG = new ObservableCollection<HPTPayOut>(payOutList);
                    if (this.Jackpot > 0)
                    {
                        var payOut = this.PayOutListATG.FirstOrDefault(po => po.NumberOfCorrect == this.RaceList.Count);
                        this.JackpotFactor = Convert.ToDecimal(payOut.TotalAmount) / Convert.ToDecimal(payOut.TotalAmount - this.Jackpot);
                        if (this.JackpotFactor > 2M && DateTime.Now < this.RaceList.First().PostTime)
                        {
                            this.JackpotFactor = 2M;
                        }
                    }
                    else
                    {
                        this.JackpotFactor = 1M;
                    }
                    if (this.BetType.Code == "V4" || this.BetType.Code == "V5")
                    {
                        if (this.PayOutListATG.First().TotalAmount == 0)
                        {
                            this.PayOutListATG.First().TotalAmount += Convert.ToInt32(Convert.ToDecimal(this.Turnover) * this.BetType.PoolShare);
                        }
                    }

                    SetV6Factor();
                }

                if (rdi.CombinationList != null)
                {
                    foreach (HPTService.HPTCombination comb in rdi.CombinationList)
                    {
                        string uniqueCode = GetUniqueCodeFromCombination(comb);
                        var hptComb = this.CombinationListInfoDouble.CombinationList
                            .FirstOrDefault(hc => hc.UniqueCode == uniqueCode);

                        if (hptComb != null)
                        {
                            if (hptComb.Horse1.Scratched == true || hptComb.Horse2.Scratched == true)
                            {
                                hptComb.Selected = false;
                                hptComb.Stake = null;
                            }
                            else
                            {
                                hptComb.CombinationOdds = comb.CombinationOdds;
                                hptComb.CombinationOddsExact = comb.CombinationOddsExact;
                                hptComb.CalculateQuotas(this.BetType.Code);
                            }
                        }
                    }
                    SortCombinationValues();
                }
            }
            catch (Exception exc)
            {
                HPTConfig.Config.AddToErrorLog(exc);
            }
        }

        internal void SetV6Factor()
        {
            try
            {
                int minPayOut = this.PayOutListATG
                        .OrderBy(po => po.NumberOfCorrect)
                        .First()
                        .TotalAmount;

                // Ta hänsyn till V6/V7/V8
                //decimal payoutWithoutJackpot = this.MaxPayOut - (decimal)this.Jackpot;
                //decimal v6Turnover = payoutWithoutJackpot - minPayOut;
                //decimal amountToAdd = this.BetType.V6Factor * v6Turnover;
                //decimal totalAmount = minPayOut + amountToAdd;
                //this.V6Factor = totalAmount / payoutWithoutJackpot;
                decimal payoutWithoutJackpot = this.MaxPayOut - (decimal)this.Jackpot;
                decimal v6Turnover = payoutWithoutJackpot - minPayOut;
                //decimal amountToAdd = this.BetType.V6Factor * v6Turnover;
                //decimal totalAmount = minPayOut + amountToAdd;
                this.V6Factor = 1M + v6Turnover / payoutWithoutJackpot;
                if (this.V6Factor < 1M)
                {
                    this.V6Factor = 1M;
                }
            }
            catch (Exception)
            {
                this.V6Factor = 1M;
            }
        }

        #region Combinations

        //public void SortTvillingCombinationValues()
        //{
        //    //foreach (HPTRace hptRace in this.LegListSorted.Values)
        //    foreach (HPTRace hptRace in this.RaceList)
        //    {
        //        if (hptRace.TvillingCombinationList != null)
        //        {
        //            hptRace.TvillingCombinationList.Sort(CompareCombinationOddsRank);
        //            for (int i = 0; i < this.CombinationList.Count; i++)
        //            {
        //                hptRace.TvillingCombinationList[i].CombinationOddsRank = i + 1;
        //            }
        //            hptRace.TvillingCombinationList.Sort(CompareMultipliedOddsRank);
        //            for (int i = 0; i < this.CombinationList.Count; i++)
        //            {
        //                hptRace.TvillingCombinationList[i].MultipliedOddsRank = i + 1;
        //            }
        //        }
        //    }
        //}

        public void SortCombinationValues()
        {
            this.CombinationListInfoDouble.CombinationList.Sort(CompareCombinationOddsRank);
            for (int i = 0; i < this.CombinationListInfoDouble.CombinationList.Count; i++)
            {
                this.CombinationListInfoDouble.CombinationList[i].CombinationOddsRank = i + 1;
            }

            this.CombinationListInfoDouble.CombinationList.Sort(CompareMultipliedOddsRank);
            for (int i = 0; i < this.CombinationListInfoDouble.CombinationList.Count; i++)
            {
                this.CombinationListInfoDouble.CombinationList[i].MultipliedOddsRank = i + 1;
            }
        }

        private int CompareCombinationOddsRank(HPTCombination c1, HPTCombination c2)
        {
            return (int)c1.CombinationOdds - (int)c2.CombinationOdds;
        }

        private int CompareMultipliedOddsRank(HPTCombination c1, HPTCombination c2)
        {
            return (int)c1.MultipliedOdds - (int)c2.MultipliedOdds;
        }

        #endregion

        private string GetHexCode(int number)
        {
            switch (number)
            {
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
                case 16:
                    return "G";
                case 17:
                    return "H";
                case 18:
                    return "I";
                case 19:
                    return "J";
                case 20:
                    return "K";
                default:
                    return number.ToString();
            }
        }

        private string GetUniqueCodeFromCombination(HPTService.HPTCombination comb)
        {
            return GetHexCode(comb.Horse1Nr) + GetHexCode(comb.Horse2Nr);
        }

        // Config property
        private HPTHorseDataToShow dataToShow;
        [XmlIgnore]
        public HPTHorseDataToShow DataToShow
        {
            get
            {
                return this.dataToShow;
            }
            set
            {
                this.dataToShow = value;
                OnPropertyChanged("DataToShow");
            }
        }

        //public HPTHorseDataToShow DataToShow { get; set; }

        #region Properties from ATG RaceDayInfo

        [DataMember]
        public string Trackname { get; set; }

        public string TracknameFile
        {
            get
            {
                return this.Trackname.Replace('/', '-');
            }
        }

        [DataMember]
        public string Trackcode { get; set; }

        [DataMember]
        public int TrackId { get; set; }

        private string trackCondition;
        [DataMember]
        public string TrackCondition
        {
            get
            {
                return this.trackCondition;
            }
            set
            {
                this.trackCondition = value;
                OnPropertyChanged("TrackCondition");
            }
        }

        private DateTime raceDayDate;
        [DataMember]
        public DateTime RaceDayDate
        {
            get
            {
                return raceDayDate;
            }
            set
            {
                raceDayDate = value;
                OnPropertyChanged("RaceDayDate");
                if (value == DateTime.MinValue)
                {
                    this.RaceDayDateString = string.Empty;
                    this.RaceDayDateShortString = string.Empty;
                }
                else
                {
                    this.RaceDayDateString = value.ToString("yyyy-MM-dd");
                    this.RaceDayDateShortString = value.ToString("d MMM");
                }
            }
        }

        private bool showInUI;
        [XmlIgnore]
        public bool ShowInUI
        {
            get
            {
                return this.showInUI;
            }
            set
            {
                this.showInUI = value;
                OnPropertyChanged("ShowInUI");
            }
        }

        [XmlIgnore]
        public string RaceDayDateString { get; set; }

        [XmlIgnore]
        public string RaceDayDateShortString { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTBetType BetType { get; set; }

        [DataMember]
        public string BetTypeString { get; set; }

        public bool HasMarksGame
        {
            get
            {
                return this.BetTypeList.Any(bt => bt.IsMarksGame);
            }
        }

        public bool IsSwedishTrack
        {
            get
            {
                return this.TrackId <= 50;
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTBetType> BetTypeList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<int> RaceNumberList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTRace> RaceList { get; set; }

        //[DataMember]
        //public int MarksQuantity { get; set; }

        private int marksQuantity;
        [DataMember]
        public int MarksQuantity
        {
            get
            {
                return this.marksQuantity;
            }
            set
            {
                this.marksQuantity = value;
                OnPropertyChanged("MarksQuantity");
            }
        }

        private int turnover;
        [DataMember]
        public int Turnover
        {
            get
            {
                return this.turnover;
            }
            set
            {
                this.turnover = value;
                OnPropertyChanged("Turnover");
                if (this.marksQuantity > 0)
                {
                    this.MeanSystemCost = Convert.ToDecimal(this.turnover) / Convert.ToDecimal(this.marksQuantity);
                }
                if (this.turnover > 0 && this.BetType.RowCost > 0M)
                {
                    this.NumberOfGambledRowsTotal = this.turnover / this.BetType.RowCost;
                }
            }
        }

        public decimal NumberOfGambledRowsTotal { get; set; }

        //private decimal numberOfGambledRows;
        //public decimal NumberOfGambledRows
        //{
        //    get
        //    {
        //        return this.numberOfGambledRows;
        //    }
        //    set
        //    {
        //        this.numberOfGambledRows = value;
        //        OnPropertyChanged("NumberOfGambledRows");
        //    }
        //}

        private decimal meanSystemCost;
        public decimal MeanSystemCost
        {
            get
            {
                return this.meanSystemCost;
            }
            set
            {
                this.meanSystemCost = value;
                OnPropertyChanged("MeanSystemCost");
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int? Jackpot { get; set; }

        [DataMember]
        public decimal JackpotFactor { get; set; }

        [XmlIgnore]
        public decimal V6Factor { get; set; }

        private decimal correlationFactor;
        [XmlIgnore]
        public decimal CorrelationFactor
        {
            get
            {
                if (this.correlationFactor == 0)
                {
                    this.correlationFactor = Convert.ToDecimal(Math.Pow(1.1D, this.RaceList.Count));
                }
                return this.correlationFactor;
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTCombinationListInfo CombinationListInfoDouble { get; set; }

        private HPTScratchedHorsesInfo scratchedHorseInfo;
        [XmlIgnore]
        public HPTScratchedHorsesInfo ScratchedHorseInfo
        {
            get
            {
                if (this.scratchedHorseInfo == null)
                {
                    this.scratchedHorseInfo = new HPTScratchedHorsesInfo(this);
                }
                return this.scratchedHorseInfo;
            }
            set
            {
                this.scratchedHorseInfo = value;
            }
        }

        #endregion

        #region Bettype logos

        [XmlIgnore]
        public HPTBetType BetTypeVxx
        {
            get
            {
                return this.BetTypeList.FirstOrDefault(bt => bt.Code == "V64" || bt.Code == "V85" || bt.Code == "V75" || bt.Code == "V86" || bt.Code == "GS75");
            }
        }

        [XmlIgnore]
        public HPTBetType BetTypeV4
        {
            get
            {
                return this.BetTypeList.FirstOrDefault(bt => bt.Code == "V4");
            }
        }

        [XmlIgnore]
        public HPTBetType BetTypeV5
        {
            get
            {
                return this.BetTypeList.FirstOrDefault(bt => bt.Code == "V5");
            }
        }

        [XmlIgnore]
        public HPTBetType BetTypeV65
        {
            get
            {
                return this.BetTypeList.FirstOrDefault(bt => bt.Code == "V65");
            }
        }

        [XmlIgnore]
        public HPTBetType BetTypeDouble
        {
            get
            {
                return this.BetTypeList.FirstOrDefault(bt => bt.Code == "DD");
            }
        }

        [XmlIgnore]
        public HPTBetType BetTypeLunchDouble
        {
            get
            {
                return this.BetTypeList.FirstOrDefault(bt => bt.Code == "LD");
            }
        }

        [XmlIgnore]
        public HPTBetType BetTypeTvilling
        {
            get
            {
                return this.BetTypeList.FirstOrDefault(bt => bt.Code == "TV");
            }
        }

        [XmlIgnore]
        public HPTBetType BetTypeTrio
        {
            get
            {
                return this.BetTypeList.FirstOrDefault(bt => bt.Code == "T");
            }
        }

        [XmlIgnore]
        public HPTBetType BetTypeV3
        {
            get
            {
                return this.BetTypeList.FirstOrDefault(bt => bt.Code == "V3");
            }
        }

        #endregion

        public string ToDateAndTrackString()
        {
            return this.RaceDayDateString + " " + this.TracknameFile;
        }

        internal void ActivateABCDChanged()
        {
            if (this.ABCDChanged != null)
            {
                this.ABCDChanged(new object(), new EventArgs());
            }
        }

        public void ClearABCDRaceDayInfo()
        {
            if (this.ClearABCD != null)
            {
                this.ClearABCD();
            }
        }

        public void SetRankTemplateChanged(HPTRankTemplate rankTemplate)
        {
            if (this.RankTemplateChanged != null)
            {
                this.RankTemplateChanged(rankTemplate);
            }
        }

        #region Result

        private bool hasResult;
        [DataMember]
        public bool HasResult
        {
            get
            {
                return this.hasResult;
            }
            set
            {
                this.hasResult = value;
                OnPropertyChanged("HasResult");
            }
        }

        private bool resultComplete;
        [DataMember]
        public bool ResultComplete
        {
            get
            {
                return this.resultComplete;
            }
            set
            {
                this.resultComplete = value;
                OnPropertyChanged("ResultComplete");
            }
        }

        internal bool AllResultsComplete
        {
            get
            {
                // Inte ens utdelning är inte klar
                if (this.PayOutList.Count == 0 || this.PayOutList.Sum(po => po.PayOutAmount) == 0)
                {
                    return false;
                }

                // Utdelning klar, men kanske inte samtliga resultat för loppet
                int numberOfHorsesWithResultInfoInLastRace = this.RaceList
                    .OrderByDescending(r => r.RaceNr)
                    .First()
                    .HorseList
                    .Count(h => h.HorseResultInfo != null && h.HorseResultInfo.FinishingPosition == 1);

                return numberOfHorsesWithResultInfoInLastRace > 0;
            }
        }

        internal bool AnyResultComplete
        {
            get
            {
                // Utdelning klar, men kanske inte samtliga resultat för loppet
                int numberOfHorsesWithResultInfo = this.RaceList
                    .OrderBy(r => r.RaceNr)
                    .First()
                    .HorseList
                    .Count(h => h.HorseResultInfo != null && h.HorseResultInfo.FinishingPosition == 1);

                return numberOfHorsesWithResultInfo > 0;
            }
        }

        private int numberOfFinishedRaces;
        [DataMember]
        public int NumberOfFinishedRaces
        {
            get
            {
                return this.numberOfFinishedRaces;
            }
            set
            {
                this.numberOfFinishedRaces = value;
                OnPropertyChanged("NumberOfFinishedRaces");
            }
        }

        internal void CalculateSimulatedRaceValues(HPTMarkBet markBet)
        {
            //decimal tempValue = 0M;
            decimal numberOfWinningSystems = this.MarksQuantity;
            var horseList = new HPTHorse[this.RaceList.Count];

            //decimal divisionFactor = 1M;
            //decimal percentageToAdd = 0.05M;
            for (int i = 0; i < this.RaceList.Count; i++)
            {
                var race = this.RaceList[i];
                if (race.LegResult != null && race.LegResult.WinnerList != null && race.LegResult.WinnerList.Count() > 0)
                {
                    race.LegResult.Value = null;

                    if (race.LegNr == this.RaceList.Count && !horseList.Any(h => h == null))
                    {
                        var singleRow = new HPTMarkBetSingleRow(horseList);
                        singleRow.CalculateValues();
                        singleRow.EstimateRowValue(markBet);
                        race.LegResult.Value = singleRow.RowValue;
                    }
                }
            }
        }

        internal ObservableCollection<HPTHorse> horseListSelected;
        [XmlIgnore]
        public ObservableCollection<HPTHorse> HorseListSelected
        {
            get
            {
                return this.horseListSelected;
            }
            set
            {
                this.horseListSelected = value;
                OnPropertyChanged("HorseListSelected");
            }
        }

        private int maxPayOut;
        [XmlIgnore]
        public int MaxPayOut
        {
            get
            {
                if (this.maxPayOut == 0)
                {
                    if (this.PayOutListATG != null && this.PayOutListATG.Count > 0)
                    {
                        this.maxPayOut = this.PayOutListATG
                            .OrderByDescending(po => po.NumberOfCorrect)
                            .First()
                            .TotalAmount;
                    }
                }
                return this.maxPayOut;
            }
            set
            {
                this.maxPayOut = value;
            }
        }

        private ObservableCollection<HPTPayOut> payOutListATG;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTPayOut> PayOutListATG
        {
            get
            {
                return this.payOutListATG;
            }
            set
            {
                this.payOutListATG = value;
                OnPropertyChanged("PayOutListATG");
            }
        }

        public bool HasAllPayOutInformation
        {
            get
            {
                if (this.PayOutList == null
                    || this.PayOutList.Count == 0
                    || this.PayOutList.Max(po => po.PayOutAmount) == 0
                    || this.PayOutList.Max(po => po.NumberOfWinningRows) == 0)
                {
                    return false;
                }
                return true;
            }
        }

        private ObservableCollection<HPTPayOut> payOutList;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTPayOut> PayOutList
        {
            get
            {
                return this.payOutList;
            }
            set
            {
                this.payOutList = value;
                OnPropertyChanged("PayOutList");
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTService.HPTVinstlista WinnerList { get; set; }

        [XmlIgnore]
        public HPTService.HPTResultMarkingBet ResultMarkingBet { get; set; }

        #endregion

        #region History

        public DateTime[] TimestampListMarkBetHistory { get; set; }

        public DateTime[] TimestampListVPHistory { get; set; }

        public HPTTurnoverHistory[] TurnoverHistoryList { get; set; }

        #endregion

        #region IHorseListContainer implementation

        [XmlIgnore]
        public ICollection<HPTHorse> HorseList { get; set; }

        [XmlIgnore]
        public HPTRaceDayInfo ParentRaceDayInfo { get; set; }

        #endregion
    }
}
