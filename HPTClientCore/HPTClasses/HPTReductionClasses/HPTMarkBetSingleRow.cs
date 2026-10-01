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

        public bool SelectedForEditing
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int RowNumber { get; set; }

        public int CouponNumber { get; set; }

        public bool V6
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
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

        public int EstimateRowValue(HPTMarkBet markBet)
        {
            //// TODO: Egen chansvärdering med hänsyn taget till jackpott och spelavdrag
            //try
            //{
            //    OwnProbabilityEV = OwnProbabilityQuota * markBet.RaceDayInfo.JackpotFactor * markBet.BetType.GamblerReturnPercentage;
            //}
            //catch (Exception exc)
            //{
            //    string s = exc.Message;
            //}

            try
            {
                if (RowShareStake > 0)
                {
                    //decimal result = markBet.RaceDayInfo.MaxPayOut / (RowShareStake * markBet.RaceDayInfo.NumberOfGambledRowsTotal);
                    var result = markBet.BetType.PoolShare * markBet.BetType.RowCost / RowShareStake;
                    //if (result == 0M || markBet.RaceDayInfo.JackpotFactor > 2M)
                    //{
                    //    result = markBet.BetType.PoolShare * markBet.BetType.RowCost / RowShareStake;
                    //    result *= markBet.RaceDayInfo.JackpotFactor;
                    //}
                    if (markBet.RaceDayInfo.JackpotFactor > 2M)
                    {
                        result *= markBet.RaceDayInfo.JackpotFactor;
                    }
                    result /= markBet.RaceDayInfo.V6Factor;
                    var resultV6 = result;
                    var resultWithoutScratchings = result * (RowShareStake / RowShareStakeWithoutScratchings);
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
                var s = exc.Message;
            }

            return 0;
        }

        public int EstimateRowValueFinalStakeShare(HPTMarkBet markBet)
        {
            try
            {
                var rowShareFinalStakeShare = HorseList
                        .Select(h => (decimal)h.StakeDistributionShareFinal)
                        .Aggregate((sd, next) => sd * next);

                var result = markBet.RaceDayInfo.MaxPayOut / (rowShareFinalStakeShare * markBet.RaceDayInfo.NumberOfGambledRowsTotal);
                if (result == 0M || markBet.RaceDayInfo.JackpotFactor > 2M)
                {
                    result = markBet.BetType.PoolShare * markBet.BetType.RowCost / rowShareFinalStakeShare;
                    result *= markBet.RaceDayInfo.JackpotFactor;
                }
                result /= markBet.RaceDayInfo.V6Factor;
                var resultV6 = result;
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
                var s = exc.Message;
            }
            return 0;
        }

        public int RowValue
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int RowValueWithoutScratchings
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int RowValueV6
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int RowValueBetMultiplier
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int RowValueFinalStakeShare { get; set; }

        public int RowValueOneErrorFinalStakeShare { get; set; }

        public int RowValueTwoErrorsFinalStakeShare { get; set; }

        public int RowValueThreeErrorsFinalStakeShare { get; set; }

        public int BetMultiplier
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
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
            var atgRankSum = 0;
            var ownRankSum = 0;
            var alternateRankSum = 0;
            var oddsSum = 0;
            var startNrSum = 0;
            var stakePercentSumExact = 0M;
            var rankSum = 0M;
            var stakePercentSum = 0;
            var rowShareStakeWithoutScratchings = 1M;
            var rowShareStake = 1M;
            var sbUniqueCode = new StringBuilder();
            var sbABCDRankCode = new StringBuilder();

            // KOMMANDE
            var rowShareStakeRounded = 1M;
            var rowShareOwnProbability = 1M;

            foreach (var horse in HorseList)
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
                //rowShareStakeRounded *= horse.StakeShareRounded;
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

        public int RowValueOneError
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int RowValueTwoErrors
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int RowValueThreeErrors
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int? RowValueOneErrorLower
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int? RowValueOneErrorUpper
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int? RowValueTwoErrorsLower
        {
            get;
            set

            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int? RowValueTwoErrorsUpper
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int? RowValueThreeErrorsLower
        {
            get;
            set

            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int? RowValueThreeErrorsUpper
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
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
            var v6RowValue = markBet.V6SingleRows && RowValue < markBet.V6UpperBoundary;
            var v6OwnRank = markBet.V6OwnRank && AlternateRankSum <= markBet.V6OwnRankMax;

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
                var exactMultiplier = Convert.ToDecimal(markBet.SingleRowTargetProfit) / Convert.ToDecimal(RowValueV6);
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
            var tempMultiplier = BetMultiplier;

            while (tempMultiplier > 0)
            {
                var partialMultiplier = markBet.BetType.BetMultiplierList.Where(bm => bm <= tempMultiplier).Max();
                BetMultiplierList.Add(partialMultiplier);
                tempMultiplier -= partialMultiplier;
            }
        }

        public string GroupingCode { get; set; }

        public void SetGroupingCode(params int[] racesToRemove)
        {
            var sb = new StringBuilder();
            foreach (var horse in HorseList)
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
            var sb = new StringBuilder();
            sb.Append(RowNumber);
            sb.Append(": ");
            for (var i = 0; i < HorseList.Length; i++)
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

        public int[] StartNrList
        {
            get
            {
                if (field == null && HorseList != null)
                {
                    field = HorseList.Select(h => h.StartNr).ToArray();
                }

                return field;
            }
            set;
        }

        internal bool HasRowDifference(HPTMarkBetSingleRow singleRow, int difference)
        {
            var numberOfDifferent = 0;
            for (var i = 0; i < StartNrList.Length; i++)
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
            var numberOfDifferent = 0;
            for (var i = 0; i < StartNrList.Length; i++)
            {
                numberOfDifferent += StartNrList[i] == singleRow.StartNrList[i] ? 0 : 1;
            }
            return numberOfDifferent;
        }

        internal int RowDifference(HPTMarkBetSingleRow singleRow, int max)
        {
            var numberOfDifferent = 0;
            for (var i = 0; i < StartNrList.Length; i++)
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
            var numberOfDifferent = 0;
            for (var i = 0; i < StartNrList.Length; i++)
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
