using System.Text;

namespace HPTClient
{
    public class HPTMarkBetSingleRow : Notifier
    {
        public HPTHorse[] HorseList { get; set; }

        public int[] PrioList { get; set; }

        public HPTMarkBetSingleRow(HPTHorse[] horseList)
        {
            HorseList = new HPTHorse[horseList.Length];
            PrioList = new int[7];
            horseList.CopyTo(HorseList, 0);
        }

        #region General properties

        public bool Selected { get; set; }

        public bool Edited { get; set; }

        private bool selectedForEditing;
        public bool SelectedForEditing
        {
            get
            {
                return selectedForEditing;
            }
            set
            {
                selectedForEditing = value;
                OnPropertyChanged("SelectedForEditing");
            }
        }

        public int RowNumber { get; set; }

        public int CouponNumber { get; set; }

        private bool v6;
        public bool V6
        {
            get
            {
                return v6;
            }
            set
            {
                v6 = value;
                OnPropertyChanged("V6");
            }
        }

        #endregion

        #region Row value

        public decimal RowShareStake { get; set; }

        // KOMMANDE
        public decimal RowShareStakeRounded { get; set; }

        // KOMMANDE
        public decimal RowShareOwnProbability { get; set; }

        // KOMMANDE
        public decimal OwnProbabilityQuota { get; set; }

        public decimal OwnProbabilityEV { get; set; }

        public decimal RowShareStakeWithoutScratchings { get; set; }

        public decimal RowShare { get; set; }

        // NY LÖSNING MED INSATSFÖRDELNING
        public int EstimateRowValue(HPTMarkBet markBet)
        {
            // Egen chansvärdering med hänsyn taget till jackpott och spelavdrag
            try
            {
                OwnProbabilityEV = OwnProbabilityQuota * markBet.RaceDayInfo.JackpotFactor * markBet.BetType.GamblerReturnPercentage;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }

            try
            {
                if (RowShareStake > 0)
                {
                    decimal result = markBet.RaceDayInfo.MaxPayOut / (RowShareStake * markBet.RaceDayInfo.NumberOfGambledRowsTotal);
                    if (result == 0M || markBet.RaceDayInfo.JackpotFactor > 2M)
                    {
                        result = markBet.BetType.PoolShare * markBet.BetType.RowCost / RowShareStake;
                        result *= markBet.RaceDayInfo.JackpotFactor;
                    }
                    result /= markBet.RaceDayInfo.V6Factor;
                    decimal resultV6 = result;
                    decimal resultWithoutScratchings = result * (RowShareStake / RowShareStakeWithoutScratchings);
                    if (V6 || markBet.V6)
                    {
                        resultV6 *= markBet.BetType.V6Factor;
                    }
                    if (markBet.RaceDayInfo.MaxPayOut > 0)
                    {
                        result = result > markBet.RaceDayInfo.MaxPayOut ? markBet.RaceDayInfo.MaxPayOut : result;
                        resultV6 = resultV6 > markBet.RaceDayInfo.MaxPayOut ? markBet.RaceDayInfo.MaxPayOut : resultV6;
                        resultWithoutScratchings = resultWithoutScratchings > markBet.RaceDayInfo.MaxPayOut ? markBet.RaceDayInfo.MaxPayOut : resultWithoutScratchings;
                    }
                    RowValue = Convert.ToInt32(Math.Floor(result));
                    RowValueV6 = Convert.ToInt32(Math.Floor(resultV6));
                    RowValueBetMultiplier = Convert.ToInt32(Math.Floor(resultV6 * BetMultiplier));
                    RowValueWithoutScratchings = Convert.ToInt32(Math.Floor(resultWithoutScratchings));
                    return RowValue;
                }
                RowValue = 0;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }

            return 0;
        }

        public int EstimateRowValueFinalStakeShare(HPTMarkBet markBet)
        {
            try
            {
                decimal rowShareFinalStakeShare = HorseList
                        .Select(h => (decimal)h.StakeDistributionShareFinal)
                        .Aggregate((sd, next) => sd * next);

                decimal result = markBet.RaceDayInfo.MaxPayOut / (rowShareFinalStakeShare * markBet.RaceDayInfo.NumberOfGambledRowsTotal);
                if (result == 0M || markBet.RaceDayInfo.JackpotFactor > 2M)
                {
                    result = markBet.BetType.PoolShare * markBet.BetType.RowCost / rowShareFinalStakeShare;
                    result *= markBet.RaceDayInfo.JackpotFactor;
                }
                result /= markBet.RaceDayInfo.V6Factor;
                decimal resultV6 = result;
                if (V6 || markBet.V6)
                {
                    resultV6 *= markBet.BetType.V6Factor;
                }
                if (markBet.RaceDayInfo.MaxPayOut > 0)
                {
                    result = result > int.MaxValue ? int.MaxValue : result;
                    resultV6 = resultV6 > int.MaxValue ? int.MaxValue : resultV6;
                }
                RowValueFinalStakeShare = Convert.ToInt32(Math.Floor(result));
                RowValueOneErrorFinalStakeShare = markBet.CouponCorrector.CalculatePayOutOneErrorFinalStakeShare(HorseList, markBet.BetType.PoolShareOneError * markBet.BetType.RowCost);
                RowValueTwoErrorsFinalStakeShare = markBet.CouponCorrector.CalculatePayOutTwoErrorsFinalStakeShare(HorseList, markBet.BetType.PoolShareTwoErrors * markBet.BetType.RowCost);
                RowValueThreeErrorsFinalStakeShare = markBet.CouponCorrector.CalculatePayOutThreeErrorsFinalStakeShare(HorseList, markBet.BetType.PoolShareThreeErrors * markBet.BetType.RowCost);

                return RowValueFinalStakeShare;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return 0;
        }

        private int rowValue;
        public int RowValue
        {
            get
            {
                return rowValue;
            }
            set
            {
                rowValue = value;
                OnPropertyChanged("RowValue");
            }
        }

        private int rowValueWithoutScratchings;
        public int RowValueWithoutScratchings
        {
            get
            {
                return rowValueWithoutScratchings;
            }
            set
            {
                rowValueWithoutScratchings = value;
                OnPropertyChanged("RowValueWithoutScratchings");
            }
        }

        private int rowValueV6;
        public int RowValueV6
        {
            get
            {
                return rowValueV6;
            }
            set
            {
                rowValueV6 = value;
                OnPropertyChanged("RowValueV6");
            }
        }

        private int rowValueBetMultiplier;
        public int RowValueBetMultiplier
        {
            get
            {
                return rowValueBetMultiplier;
            }
            set
            {
                rowValueBetMultiplier = value;
                OnPropertyChanged("RowValueBetMultiplier");
            }
        }

        public int RowValueFinalStakeShare { get; set; }

        public int RowValueOneErrorFinalStakeShare { get; set; }

        public int RowValueTwoErrorsFinalStakeShare { get; set; }

        public int RowValueThreeErrorsFinalStakeShare { get; set; }

        private int betMultiplier;
        public int BetMultiplier
        {
            get
            {
                return betMultiplier;
            }
            set
            {
                betMultiplier = value;
                OnPropertyChanged("BetMultiplier");
                RowValueBetMultiplier = RowValueV6 * value;
            }
        }

        #endregion

        #region Calculated Properties

        public string UniqueCode { get; set; }

        public string ABCDRankCode { get; set; }

        internal bool ValuesCalculated = false;
        internal void CalculateValues()
        {
            int atgRankSum = 0;
            int ownRankSum = 0;
            int alternateRankSum = 0;
            int oddsSum = 0;
            int startNrSum = 0;
            decimal stakePercentSumExact = 0M;
            decimal rankSum = 0M;
            int stakePercentSum = 0;
            decimal rowShareStakeWithoutScratchings = 1M;
            decimal rowShareStake = 1M;
            StringBuilder sbUniqueCode = new StringBuilder();
            StringBuilder sbABCDRankCode = new StringBuilder();

            // KOMMANDE
            decimal rowShareStakeRounded = 1M;
            decimal rowShareOwnProbability = 1M;

            foreach (HPTHorse horse in HorseList)
            {
                atgRankSum += horse.RankATG;
                ownRankSum += horse.RankOwn;
                alternateRankSum += horse.RankAlternate;
                oddsSum += horse.VinnarOdds;
                rankSum += horse.RankMean;
                stakePercentSum += horse.StakeDistributionPercent;
                stakePercentSumExact += horse.StakeDistributionShare;
                startNrSum += horse.StartNr;
                rowShareStake *= horse.StakeDistributionShare;
                rowShareStakeWithoutScratchings *= horse.StakeShareWithoutScratchings;
                sbUniqueCode.Append(horse.HexCode);
                sbABCDRankCode.Append(horse.Prio.ToString());
                PrioList[(int)horse.Prio] += 1;

                // KOMMANDE
                rowShareStakeRounded *= horse.StakeShareRounded;
                rowShareOwnProbability *= Convert.ToDecimal(horse.OwnProbability);
            }
            ATGRankSum = atgRankSum;
            OddsSum = oddsSum;
            OwnRankSum = ownRankSum;
            AlternateRankSum = alternateRankSum;
            StartNrSum = startNrSum;
            RankSum = rankSum;
            StakePercentSum = stakePercentSum;
            StakePercentSumExact = stakePercentSumExact * 100M;
            UniqueCode = sbUniqueCode.ToString();
            ABCDRankCode = sbABCDRankCode.ToString();
            RowShareStakeWithoutScratchings = rowShareStakeWithoutScratchings;
            RowShareStake = rowShareStake;

            // KOMMANDE
            if (rowShareStakeRounded > 0M)
            {
                RowShareStakeRounded = rowShareStakeRounded;
                RowShareOwnProbability = rowShareOwnProbability;
                OwnProbabilityQuota = RowShareOwnProbability / RowShareStakeRounded;
            }

            ValuesCalculated = true;
        }

        public int ATGRankSum { get; set; }

        public int OwnRankSum { get; set; }

        public int AlternateRankSum { get; set; }

        public int OddsSum { get; set; }

        public int StartNrSum { get; set; }

        public int PercentSum { get; set; }

        public decimal PercentSumExact { get; set; }

        public decimal StakePercentSumExact { get; set; }

        public decimal ShareSum { get; set; }

        public decimal RankSum { get; set; }

        public int StakePercentSum { get; set; }

        public decimal[] CurrentGroupIntervalValues { get; set; }

        private int rowValueOneError;
        public int RowValueOneError
        {
            get
            {
                return rowValueOneError;
            }
            set
            {
                rowValueOneError = value;
                OnPropertyChanged("RowValueOneError");
            }
        }

        private int rowValueTwoErrors;
        public int RowValueTwoErrors
        {
            get
            {
                return rowValueTwoErrors;
            }
            set
            {
                rowValueTwoErrors = value;
                OnPropertyChanged("RowValueTwoErrors");
            }
        }

        private int rowValueThreeErrors;
        public int RowValueThreeErrors
        {
            get
            {
                return rowValueThreeErrors;
            }
            set
            {
                rowValueThreeErrors = value;
                OnPropertyChanged("RowValueThreeErrors");
            }
        }

        private int? rowValueOneErrorLower;
        public int? RowValueOneErrorLower
        {
            get
            {
                return rowValueOneErrorLower;
            }
            set
            {
                rowValueOneErrorLower = value;
                OnPropertyChanged("RowValueOneErrorLower");
            }
        }

        private int? rowValueOneErrorUpper;
        public int? RowValueOneErrorUpper
        {
            get
            {
                return rowValueOneErrorUpper;
            }
            set
            {
                rowValueOneErrorUpper = value;
                OnPropertyChanged("RowValueOneErrorUpper");
            }
        }

        private int? rowValueTwoErrorsLower;
        public int? RowValueTwoErrorsLower
        {
            get
            {
                return rowValueTwoErrorsLower;
            }
            set

            {
                rowValueTwoErrorsLower = value;
                OnPropertyChanged("RowValueTwoErrorsLower");
            }
        }

        private int? rowValueTwoErrorsUpper;
        public int? RowValueTwoErrorsUpper
        {
            get
            {
                return rowValueTwoErrorsUpper;
            }
            set
            {
                rowValueTwoErrorsUpper = value;
                OnPropertyChanged("RowValueTwoErrorsUpper");
            }
        }

        private int? rowValueThreeErrorsLower;
        public int? RowValueThreeErrorsLower
        {
            get
            {
                return rowValueThreeErrorsLower;
            }
            set

            {
                rowValueThreeErrorsLower = value;
                OnPropertyChanged("RowValueThreeErrorsLower");
            }
        }

        private int? rowValueThreeErrorsUpper;
        public int? RowValueThreeErrorsUpper
        {
            get
            {
                return rowValueThreeErrorsUpper;
            }
            set
            {
                rowValueThreeErrorsUpper = value;
                OnPropertyChanged("RowValueThreeErrorsUpper");
            }
        }

        #endregion

        public List<int> BetMultiplierList { get; set; }

        public void SetV6BetMultiplier(HPTMarkBet markBet)
        {
            // Manuellt redigerad enkelrad
            if (markBet.SingleRowEditedList != null && markBet.SingleRowEditedList.Count > 0)
            {
                var srEdited = markBet.SingleRowEditedList.FirstOrDefault(sr => sr.UniqueCode == UniqueCode);
                if (srEdited != null)
                {
                    Edited = true;
                    V6 = srEdited.V6;
                    BetMultiplier = srEdited.BetMultiplier;
                    CreateBetMultiplierList(markBet);
                }
            }

            // Skippa resten av bearbetningen om raden är manuellt editerad
            if (Edited)
            {
                return;
            }

            // Sätt defaultvärden från markbet
            BetMultiplier = markBet.BetMultiplier == 0 ? 1 : markBet.BetMultiplier;
            V6 = markBet.V6;
            if (markBet.V6)
            {
                EstimateRowValue(markBet);
            }

            // kontrollera båda varianterna av V6-gräns
            bool v6RowValue = markBet.V6SingleRows && RowValue < markBet.V6UpperBoundary;
            bool v6OwnRank = markBet.V6OwnRank && AlternateRankSum <= markBet.V6OwnRankMax;

            // Värden från radvärdesregler i andra hand
            if (v6RowValue || v6OwnRank)
            {
                V6 = true;
                EstimateRowValue(markBet);
            }
            else if (!markBet.V6SingleRows && !markBet.V6OwnRank && RowValue != RowValueV6)
            {
                if (!markBet.V6)
                {
                    V6 = false;
                }
                EstimateRowValue(markBet);
            }

            if (markBet.SingleRowBetMultiplier && RowValueV6 > 0)
            {
                decimal exactMultiplier = Convert.ToDecimal(markBet.SingleRowTargetProfit) / Convert.ToDecimal(RowValueV6);
                BetMultiplier = Convert.ToInt32(Math.Ceiling(exactMultiplier));
                BetMultiplier = BetMultiplier == 0 ? 1 : BetMultiplier;
            }

            // Ändra värden om man satt andra explicita värden på V6/Flerbongsregler
            if (markBet.ReductionV6BetMultiplierRule)
            {
                foreach (var v6BetMultiplierRule in markBet.V6BetMultiplierRuleList.Where(r => r.Use && r.HorseList.Count > 0))
                {
                    v6BetMultiplierRule.NumberOfRowsAffected = 0;
                    if (HorseList.Intersect(v6BetMultiplierRule.HorseList).Count() == v6BetMultiplierRule.HorseList.Count)
                    {
                        V6 = v6BetMultiplierRule.V6;
                        BetMultiplier = v6BetMultiplierRule.BetMultiplier;
                        v6BetMultiplierRule.NumberOfRowsAffected++;
                    }
                }
            }

            // Öka BetMultiplier om man satt ett högre värde generellt för hela systemet
            if (markBet.BetMultiplier > BetMultiplier)
            {
                BetMultiplier = markBet.BetMultiplier;
            }

            // Skapa array med de flerbongsvärden som behövs
            CreateBetMultiplierList(markBet);
        }

        public void CreateBetMultiplierList(HPTMarkBet markBet)
        {
            BetMultiplierList = new List<int>();
            int tempMultiplier = BetMultiplier;

            while (tempMultiplier > 0)
            {
                int partialMultiplier = markBet.BetType.BetMultiplierList.Where(bm => bm <= tempMultiplier).Max();
                BetMultiplierList.Add(partialMultiplier);
                tempMultiplier -= partialMultiplier;
            }
        }

        public string GroupingCode { get; set; }

        public void SetGroupingCode(params int[] racesToRemove)
        {
            StringBuilder sb = new StringBuilder();
            foreach (HPTHorse horse in HorseList)
            {
                if (!racesToRemove.Contains(horse.ParentRace.LegNr))
                {
                    sb.Append(horse.HexCode);
                }
            }
            GroupingCode = sb.ToString();
        }

        internal void SetCurrentGroupIntervalValues(System.Reflection.PropertyInfo propertyInfo)
        {
            CurrentGroupIntervalValues = HorseList
                .Select(h => Convert.ToDecimal(propertyInfo.GetValue(h, null)))
                .ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(RowNumber);
            sb.Append(": ");
            for (int i = 0; i < HorseList.Length; i++)
            {
                sb.Append(HorseList[i].StartNr);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            if (V6 || BetMultiplier > 1)
            {
                sb.Append(" (");
                if (V6)
                {
                    switch (HorseList.First().ParentRace.ParentRaceDayInfo.BetType.TypeCategory)
                    {
                        case BetTypeCategory.V6X:
                            sb.Append("V6");
                            break;
                        case BetTypeCategory.V75:
                            sb.Append("V7");
                            break;
                        case BetTypeCategory.V86:
                            sb.Append("V8");
                            break;
                        default:
                            break;
                    }
                }
                if (V6 && BetMultiplier > 1)
                {
                    sb.Append(" och ");
                }
                if (BetMultiplier > 1)
                {
                    sb.Append(BetMultiplier);
                    sb.Append(" X Flerbong");
                }
                sb.Append(")");
            }

            return sb.ToString();
        }

        //public int[] StartNrList { get; set; }

        private int[] startNrList;
        public int[] StartNrList
        {
            get
            {
                if (startNrList == null && HorseList != null)
                {
                    startNrList = HorseList.Select(h => h.StartNr).ToArray();
                }
                return startNrList;
            }
            set
            {
                startNrList = value;
            }
        }

        internal bool HasRowDifference(HPTMarkBetSingleRow singleRow, int difference)
        {
            int numberOfDifferent = 0;
            for (int i = 0; i < StartNrList.Length; i++)
            {
                numberOfDifferent += StartNrList[i] == singleRow.StartNrList[i] ? 0 : 1;
                if (numberOfDifferent > difference)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool HasRowDifference(IEnumerable<HPTMarkBetSingleRow> singleRows, int difference)
        {
            foreach (var singleRow in singleRows)
            {
                if (!HasRowDifference(singleRow, difference))
                {
                    return false;
                }
            }
            return true;
        }

        internal int RowDifference(HPTMarkBetSingleRow singleRow)
        {
            int numberOfDifferent = 0;
            for (int i = 0; i < StartNrList.Length; i++)
            {
                numberOfDifferent += StartNrList[i] == singleRow.StartNrList[i] ? 0 : 1;
            }
            return numberOfDifferent;
        }

        internal int RowDifference(HPTMarkBetSingleRow singleRow, int max)
        {
            int numberOfDifferent = 0;
            for (int i = 0; i < StartNrList.Length; i++)
            {
                numberOfDifferent += StartNrList[i] == singleRow.StartNrList[i] ? 0 : 1;
                if (numberOfDifferent > max)
                {
                    return numberOfDifferent;
                }
            }
            return numberOfDifferent;
        }

        internal bool RowDifferenceInInterval(HPTMarkBetSingleRow singleRow, int min, int max)
        {
            int numberOfDifferent = 0;
            for (int i = 0; i < StartNrList.Length; i++)
            {
                numberOfDifferent += StartNrList[i] == singleRow.StartNrList[i] ? 0 : 1;
                if (numberOfDifferent > max)
                {
                    return false;
                }
            }
            return numberOfDifferent >= min;
        }

        #region För kupongkomprimering

        internal bool HasDuplicateBetMultipliers()
        {
            return BetMultiplierList.Distinct().Count() < BetMultiplierList.Count;
        }

        internal HPTMarkBetSingleRow Clone(int betMultiplier)
        {
            var singleRow = new HPTMarkBetSingleRow(HorseList)
            {
                BetMultiplier = betMultiplier,
                RowNumber = RowNumber,
                V6 = V6
                //UniqueCode = this.UniqueCode
            };
            return singleRow;
        }

        internal HPTMarkBetSingleRow Clone()
        {
            var singleRow = new HPTMarkBetSingleRow(HorseList)
            {
                BetMultiplier = BetMultiplier,
                RowNumber = RowNumber,
                V6 = V6,
                UniqueCode = UniqueCode
            };
            return singleRow;
        }

        internal List<HPTMarkBetSingleRow> GetUniqueList()
        {
            var singleRowList = BetMultiplierList
                .Distinct()
                .Select(bm =>
                    new HPTMarkBetSingleRow(HorseList)
                    {
                        BetMultiplier = bm,
                        RowNumber = RowNumber,
                        UniqueCode = UniqueCode,
                        V6 = V6
                    })
                .ToList();

            return singleRowList;
        }

        internal List<HPTMarkBetSingleRow> GetDuplicateList()
        {
            return GetDuplicateList(1);

            //var duplicateList = this.BetMultiplierList
            //    .GroupBy(bm => bm)
            //    .Where(g => g.Count() > 1)
            //    .Select(g => g.Key);

            //var duplicateRowList = duplicateList
            //    .Select(bm =>
            //        new HPTMarkBetSingleRow(this.HorseList)
            //        {
            //            BetMultiplier = bm,
            //            BetMultiplierList = this.BetMultiplierList.Where(bm2 => bm2 == bm).ToList(),
            //            RowNumber = this.RowNumber,
            //            UniqueCode = this.UniqueCode,
            //            V6 = this.V6
            //        })
            //    .ToList();

            //return duplicateRowList;
        }

        internal List<HPTMarkBetSingleRow> GetDuplicateList(int depth)
        {
            var duplicateList = BetMultiplierList
                .GroupBy(bm => bm)
                .Where(g => g.Count() > depth)
                .Select(g => g.Key);

            var duplicateRowList = duplicateList
                .Select(bm =>
                    new HPTMarkBetSingleRow(HorseList)
                    {
                        BetMultiplier = bm,
                        BetMultiplierList = BetMultiplierList.Where(bm2 => bm2 == bm).ToList(),
                        RowNumber = RowNumber,
                        UniqueCode = UniqueCode,
                        V6 = V6
                    })
                .ToList();

            return duplicateRowList;
        }

        #endregion
    }
}
