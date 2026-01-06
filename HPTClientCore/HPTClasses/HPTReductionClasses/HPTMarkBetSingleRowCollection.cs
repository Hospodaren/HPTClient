using System.Text;

namespace HPTClient
{
    public class HPTMarkBetSingleRowCollection : Notifier
    {
        private HPTHorse[] horseList;
        private HPTMarkBet markBet;

        public List<HPTMarkBetSingleRowCombination> CompressedCoupons { get; set; }
        private HPTMarkBetSingleRow[] AllRows;
        internal bool SaveAllRows;

        // Event för att meddelan när den flertrådade analysen är klar
        public event Action AnalyzingFinished;

        // Properties för att hantera trådad beräkning och komprimering
        public bool CompressionInProgress { get; set; }
        public bool CalculationInProgress { get; set; }
        public bool StopCalculation { get; set; }

        public HPTMarkBetSingleRowCollection(HPTMarkBet markBet)
        {
            this.markBet = markBet;
            horseList = new HPTHorse[markBet.NumberOfRaces];
            lastRequestedRecalculation = DateTime.Now;
            singleRows = new List<HPTMarkBetSingleRow>();
        }

        private List<HPTMarkBetSingleRow> singleRows;
        public List<HPTMarkBetSingleRow> SingleRows
        {
            get
            {
                return singleRows;
            }
            set
            {
                singleRows = value;
                OnPropertyChanged("SingleRowsObservable");
            }
        }

        public void ClearAll()
        {
            if (SingleRows != null)
            {
                SingleRows.Clear();
            }
            if (CompressedCoupons != null)
            {
                CompressedCoupons.Clear();
            }
            CoveredRowsShare = 0M;
        }

        private DateTime lastRequestedRecalculation;
        private int rowNumber = 1;
        public void UpdateRowCollection()
        {
            if (!CheckCalculationStatus())
            {
                return;
            }

            lock (this)
            {
                StopCalculation = false;
                CalculationInProgress = true;
                try
                {
                    InitForRecalculation();

                    // Skapa alla enkelrader
                    MakeSingleRowCollection(0);

                    // Kör resterande regler och avsluta bearbetningen
                    if (markBet.IsCalculatingTemplates)
                    {
                        markBet.ReducedSize = singleRows.Count;
                        return;
                    }
                    HandleAnalyzedRowCollection();
                    OnPropertyChanged("SingleRows");  // Tvinga fram uppdatering av enkelradslistan
                }
                catch (Exception exc)
                {
                    ResetValues();
                    throw exc;
                }
            }
        }

        private void ResetValues()
        {
            totalCouponSize = 0;
            markBet.TotalCouponSize = 0;
            AnalyzedRowsShare = 0M;
            NumberOfAnalyzedRows = 0;
            NumberOfCoveredRows = 0;
            CoveredRowsShare = 0M;
            CalculationInProgress = false;
        }

        internal bool CheckCalculationStatus()
        {
            if (markBet.IsCalculatingTemplates)
            {
                return true;
            }
            DateTime dt = DateTime.Now;
            if (dt > lastRequestedRecalculation)
            {
                lastRequestedRecalculation = dt;
            }

            // Fall där det är onödigt att beräkna reducering
            if (markBet.IsDeserializing || markBet.SystemSize == 0)
            {
                return false;
            }

            // Kolla om beräkning av antal rader pågår
            if (CalculationInProgress)
            {
                StopCalculation = true;
            }

            // Kolla om kupongkomprimering pågår
            if (CompressionInProgress)
            {
                StopCalculation = true;
            }

            int i = 0;
            while ((CalculationInProgress || CompressionInProgress)
                && i < 50)
            {
                if (dt < lastRequestedRecalculation)
                {
                    return false;
                }
                Thread.Sleep(100);
                i++;
            }
            if (dt < lastRequestedRecalculation)
            {
                return false;
            }
            return true;
        }

        private List<HPTHorse>[] orderedRaceHorseList;
        public void InitForRecalculation()
        {
            singleRows.Clear();

            orderedRaceHorseList = new List<HPTHorse>[markBet.RaceDayInfo.RaceList.Count];
            for (int i = 0; i < markBet.RaceDayInfo.RaceList.Count; i++)
            {
                if (markBet.ABCDEFReductionRule.Use || markBet.MultiABCDEFReductionRule.Use)
                {
                    orderedRaceHorseList[i] = markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.Prio)
                        .ToList();
                }
                else if (markBet.ReductionRank)
                {
                    orderedRaceHorseList[i] = markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.RankWeighted)
                        .ToList();
                }
                else if (markBet.StakePercentSumReductionRule.Use)
                {
                    orderedRaceHorseList[i] = markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.StakeDistributionPercent)
                        .ToList();
                }
                else if (markBet.StartNrSumReductionRule.Use)
                {
                    orderedRaceHorseList[i] = markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.StartNr)
                        .ToList();
                }
                else if (markBet.ATGRankSumReductionRule.Use)
                {
                    orderedRaceHorseList[i] = markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.RankATG)
                        .ToList();
                }
                else if (markBet.OwnRankSumReductionRule.Use)
                {
                    orderedRaceHorseList[i] = markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.RankOwn)
                        .ToList();
                }
                else if (markBet.OddsSumReductionRule.Use)
                {
                    orderedRaceHorseList[i] = markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.VinnarOdds)
                        .ToList();
                }
                else
                {
                    orderedRaceHorseList[i] = markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderBy(h => h.StakeDistributionPercent)
                        .ToList();
                }
            }

            NumberOfAnalyzedRows = 0;
            AnalyzedRowsShare = 0M;

            rowNumber = 1;
            reductionRulesToApply = markBet.ReductionRulesToApply;

            // Initiera variabel för totalt antal inlämnade rader med flerbong inkluderat
            totalCouponSize = 0;

            // Specialfall där vi behöver spara alla rader i ramen
            if (markBet.HasPercentageSumReduction || markBet.IsCalculatingTemplates)
            {
                SaveAllRows = true;
            }
            if (SaveAllRows)
            {
                AllRows = new HPTMarkBetSingleRow[markBet.SystemSize];
            }
        }

        internal void HandleAnalyzedRowCollection()
        {
            // Kör procentsummereglerna
            if (markBet.HasPercentageSumReduction)
            {
                ApplyRowValuePercentageRule();
                //ApplyPercentSumPercentageRule();
                ApplyRankSumPercentageRule();
                ApplyStakePercentSumPercentageRule();
                ApplyStartNumberSumPercentageRule();

                ResetNumberOfCoveredRows();
            }


            #region Test av algoritmer

            if (markBet.GuaranteeReduction && markBet.NumberOfToleratedErrors > 0)
            {
                if (markBet.FastGuaranteeReduction)
                {
                    RemoveWithRowDifference(); // Verkar funka som den ska
                }
                else
                {
                    DateTime dtSTart = DateTime.Now;
                    RemoveWithRowDifferenceSlow(); // Ny version att testa
                    TimeSpan ts = DateTime.Now - dtSTart;
                    string s = ts.TotalSeconds.ToString();
                    string n = markBet.ReducedSize.ToString();
                }
            }

            #endregion

            RemoveOwnProbabilityRowsToReachTarget();
            RemoveRandomRowsToReachTarget();
            SetBetMultiplierToReachTarget();

            HandleHighestAndLowestSums();
            HandleHighestAndLowestIncludedSums();

            //// Skapa kollektionen som visas i GUIt
            //this.SingleRowsObservable = new ObservableCollection<HPTMarkBetSingleRow>(this.SingleRows);


            markBet.ReducedSize = SingleRows.Count;
            markBet.TotalCouponSize = totalCouponSize;

            //CompressToCoupons();
            CalculationInProgress = false;
            StopCalculation = false;

            if (AnalyzingFinished != null)
            {
                AnalyzingFinished();
            }
        }

        public void RecalculateCurrentRows()
        {
            if (AllRows == null || AllRows.Length == 0)
            {
                SaveAllRows = true;
                UpdateRowCollection();
                return;
            }
            if (reductionRulesToApply == null || reductionRulesToApply.Count == 0)
            {
                reductionRulesToApply = markBet.ReductionRulesToApply;
            }
            SingleRows.Clear();
            //this.SingleRows = new List<HPTMarkBetSingleRow>();

            NumberOfAnalyzedRows = 0;
            AnalyzedRowsShare = 0M;

            rowNumber = 1;
            foreach (HPTMarkBetSingleRow singleRow in AllRows)
            {
                AnalyzeRow(singleRow);
            }
            //this.SingleRowsObservable = new ObservableCollection<HPTMarkBetSingleRow>(this.SingleRows);
            markBet.ReducedSize = SingleRows.Count;
            markBet.TotalCouponSize = totalCouponSize;
        }

        private List<HPTReductionRule> reductionRulesToApply;
        private void MakeSingleRowCollection(int raceNumber)
        {
            if (raceNumber == markBet.NumberOfRaces)   // Sista loppet
            {
                if (StopCalculation)
                {
                    throw new Exception();
                }
                HPTMarkBetSingleRow singleRow = new HPTMarkBetSingleRow(horseList);
                if (SaveAllRows)
                {
                    AllRows[numberOfAnalyzedRows] = singleRow;
                }
                AnalyzeRow(singleRow);
                return;
            }
            else if (raceNumber > 1 && !SaveAllRows)    // Andra loppet eller senare
            {
                if (StopCalculation)
                {
                    throw new Exception();
                }
                if (!AnalyzeRowInAdvance(raceNumber))    // raceNumber är index i array... :-(
                {
                    return;
                }
            }
            foreach (HPTHorse horse in orderedRaceHorseList[raceNumber])
            {
                horseList[raceNumber] = horse;
                MakeSingleRowCollection(raceNumber + 1);
            }
            return;
        }

        internal bool AnalyzeRowInAdvance(int numberOfRacesToTest)
        {
            foreach (var reductionRule in reductionRulesToApply)
            {
                bool include = reductionRule.IncludeRow(markBet, horseList, numberOfRacesToTest);
                if (!include)
                {
                    // Hur många rader slipper vi kontrollera tack vare tidigt avbrott
                    int numberOfRowsCalculated = markBet.RaceDayInfo.RaceList
                        .Where(r => r.LegNr > numberOfRacesToTest)
                        .Select(r => r.NumberOfSelectedHorses)
                        .Aggregate((numberOfChosen, next) => numberOfChosen * next);

                    // Uppdatera så att antalet analyserade rader stämmer
                    NumberOfAnalyzedRows += numberOfRowsCalculated;
                    AnalyzedRowsShare = Convert.ToDecimal(numberOfAnalyzedRows) / Convert.ToDecimal(markBet.SystemSize);

                    return include;
                }
            }
            return true;
        }

        #region Analysera enskild rad

        // Summan av antal inlämnade rader med hänsyn till flerbong
        private int totalCouponSize;
        private void AnalyzeRow(HPTMarkBetSingleRow singleRow)
        {
            lock (this)
            {
                numberOfAnalyzedRows++;
                if (numberOfAnalyzedRows % 739 == 0 || numberOfAnalyzedRows == markBet.SystemSize)
                {
                    NumberOfAnalyzedRows = numberOfAnalyzedRows;
                    AnalyzedRowsShare = Convert.ToDecimal(numberOfAnalyzedRows) /
                                             Convert.ToDecimal(markBet.SystemSize);
                }
            }

            // Kör igenom alla regler
            if (!TestRules(singleRow))
            {
                return;
            }

            // Add single row to collection
            lock (SingleRows)
            {
                // Gör beräkningar på enkelraden
                if (!singleRow.ValuesCalculated)
                {
                    singleRow.CalculateValues();
                }
                if (singleRow.RowValue == 0)
                {
                    singleRow.EstimateRowValue(markBet);
                }
                SingleRows.Add(singleRow);
            }
            singleRow.SetV6BetMultiplier(markBet);
            totalCouponSize += singleRow.BetMultiplier;

            // Lägg till radtäckning för varje häst
            foreach (HPTHorse horse in singleRow.HorseList)
            {
                lock (horse)
                {
                    horse.NumberOfCoveredRows++;
                }
            }

            // Räkna upp reducerad storlek
            markBet.ReducedSize = SingleRows.Count;
            markBet.ReductionQuota = 1M - Convert.ToDecimal(markBet.ReducedSize) / Convert.ToDecimal(SingleRows.Count);

            singleRow.Selected = true;
            singleRow.RowNumber = rowNumber;
            singleRow.CouponNumber = 0;
            rowNumber++;
        }

        private bool TestRules(HPTMarkBetSingleRow singleRow)
        {
            // Kör igenom alla regler
            foreach (HPTReductionRule rule in reductionRulesToApply)
            {
                if (!rule.IncludeRow(markBet, singleRow))
                {
                    return false;
                }
            }
            return true;
        }

        internal void HandleHighestAndLowestSums()
        {
            if (markBet.SystemSize == 0)
            {
                return;
            }
            bool recalculationPaused = markBet.pauseRecalculation;
            markBet.pauseRecalculation = true;

            List<HPTHorse> lowestRowValueHorseList = new List<HPTHorse>();
            List<HPTHorse> highestRowValueHorseList = new List<HPTHorse>();
            List<HPTHorse> lowestRankSumHorseList = new List<HPTHorse>();
            List<HPTHorse> highestRankSumHorseList = new List<HPTHorse>();
            //List<HPTHorse> lowestMarksPercentHorseList = new List<HPTHorse>();
            //List<HPTHorse> highestMarksPercentHorseList = new List<HPTHorse>();
            List<HPTHorse> lowestStartNumberSumHorseList = new List<HPTHorse>();
            List<HPTHorse> highestStartNumberSumHorseList = new List<HPTHorse>();
            List<HPTHorse> lowestOddsSumHorseList = new List<HPTHorse>();
            List<HPTHorse> highestOddsSumHorseList = new List<HPTHorse>();
            List<HPTHorse> lowestATGRankSumHorseList = new List<HPTHorse>();
            List<HPTHorse> highestATGRankSumHorseList = new List<HPTHorse>();
            List<HPTHorse> lowestOwnRankSumHorseList = new List<HPTHorse>();
            List<HPTHorse> highestOwnRankSumHorseList = new List<HPTHorse>();

            foreach (var race in markBet.RaceDayInfo.RaceList)
            {
                if (race.HorseListSelected.Count > 0)
                {
                    IOrderedEnumerable<HPTHorse> horseListByStakeShare = race.HorseListSelected.OrderByDescending(h => h.StakeDistributionShare);
                    lowestRowValueHorseList.Add(horseListByStakeShare.First());
                    highestRowValueHorseList.Add(horseListByStakeShare.Last());

                    //IOrderedEnumerable<HPTHorse> horseListByRank = race.HorseListSelected.OrderBy(h => h.RankMean);
                    //lowestRankSumHorseList.Add(horseListByRank.First());
                    //highestRankSumHorseList.Add(horseListByRank.Last());

                    IOrderedEnumerable<HPTHorse> horseListByRank = race.HorseListSelected.OrderBy(h => h.RankWeighted);
                    lowestRankSumHorseList.Add(horseListByRank.First());
                    highestRankSumHorseList.Add(horseListByRank.Last());

                    //IOrderedEnumerable<HPTHorse> horseListByMarksPercent = race.HorseListSelected.OrderBy(h => h.MarksPercentExact);
                    //lowestMarksPercentHorseList.Add(horseListByMarksPercent.First());
                    //highestMarksPercentHorseList.Add(horseListByMarksPercent.Last());

                    IOrderedEnumerable<HPTHorse> horseListByStartNumber = race.HorseListSelected.OrderBy(h => h.StartNr);
                    lowestStartNumberSumHorseList.Add(horseListByStartNumber.First());
                    highestStartNumberSumHorseList.Add(horseListByStartNumber.Last());

                    IOrderedEnumerable<HPTHorse> horseListByOdds = race.HorseListSelected.OrderBy(h => h.VinnarOdds);
                    lowestOddsSumHorseList.Add(horseListByOdds.First());
                    highestOddsSumHorseList.Add(horseListByOdds.Last());

                    IOrderedEnumerable<HPTHorse> horseListByATGRank = race.HorseListSelected.OrderBy(h => h.RankATG);
                    lowestATGRankSumHorseList.Add(horseListByATGRank.First());
                    highestATGRankSumHorseList.Add(horseListByATGRank.Last());

                    IOrderedEnumerable<HPTHorse> horseListByOwnRank = race.HorseListSelected.OrderBy(h => h.RankOwn);
                    lowestOwnRankSumHorseList.Add(horseListByOwnRank.First());
                    highestOwnRankSumHorseList.Add(horseListByOwnRank.Last());
                }
            }

            // Lägsta radvärde och högsta insatsfördelningsprocent
            HPTMarkBetSingleRow singleRow = new HPTMarkBetSingleRow(lowestRowValueHorseList.ToArray());
            singleRow.CalculateValues();
            singleRow.EstimateRowValue(markBet);
            lowestEstimatedRowValue = singleRow.RowValue;
            highestStakePercentSum = singleRow.StakePercentSum;

            // Högsta radvärde och lägsta insatsfördelningsprocent
            singleRow = new HPTMarkBetSingleRow(highestRowValueHorseList.ToArray());
            singleRow.CalculateValues();
            singleRow.EstimateRowValue(markBet);
            highestEstimatedRowValue = singleRow.RowValue;
            lowestStakePercentSum = singleRow.StakePercentSum;

            // Lägsta ranksumma
            singleRow = new HPTMarkBetSingleRow(lowestRankSumHorseList.ToArray());
            singleRow.CalculateValues();
            //this.MinRankSum = singleRow.RankSum;
            MinRankSum = Math.Round(singleRow.RankSum - 0.05M, 1);

            // Högsta ranksumma
            singleRow = new HPTMarkBetSingleRow(highestRankSumHorseList.ToArray());
            singleRow.CalculateValues();
            MaxRankSum = Math.Round(singleRow.RankSum + 0.05M, 1);

            //// Lägsta streckprocent
            //singleRow = new HPTMarkBetSingleRow(lowestMarksPercentHorseList.ToArray());
            //singleRow.CalculateValues();
            //this.lowestPercentSum = Convert.ToInt32(Math.Floor(singleRow.PercentSumExact));

            //// Högsta streckprocent
            //singleRow = new HPTMarkBetSingleRow(highestMarksPercentHorseList.ToArray());
            //singleRow.CalculateValues();
            //this.highestPercentSum = Convert.ToInt32(Math.Ceiling(singleRow.PercentSumExact));

            // Lägsta startnummersumma
            singleRow = new HPTMarkBetSingleRow(lowestStartNumberSumHorseList.ToArray());
            singleRow.CalculateValues();
            lowestStartNumberSum = singleRow.StartNrSum;

            // Högsta startnummersumma
            singleRow = new HPTMarkBetSingleRow(highestStartNumberSumHorseList.ToArray());
            singleRow.CalculateValues();
            highestStartNumberSum = singleRow.StartNrSum;

            // Lägsta oddssumma
            singleRow = new HPTMarkBetSingleRow(lowestOddsSumHorseList.ToArray());
            singleRow.CalculateValues();
            lowestOddsSum = singleRow.OddsSum;

            // Högsta oddssumma
            singleRow = new HPTMarkBetSingleRow(highestOddsSumHorseList.ToArray());
            singleRow.CalculateValues();
            highestOddsSum = singleRow.OddsSum;

            // Lägsta ATG-ranksumma
            singleRow = new HPTMarkBetSingleRow(lowestATGRankSumHorseList.ToArray());
            singleRow.CalculateValues();
            lowestATGRankSum = singleRow.ATGRankSum;

            // Högsta ATG-ranksumma
            singleRow = new HPTMarkBetSingleRow(highestATGRankSumHorseList.ToArray());
            singleRow.CalculateValues();
            highestATGRankSum = singleRow.ATGRankSum;

            // Lägsta egen ranksumma
            singleRow = new HPTMarkBetSingleRow(lowestOwnRankSumHorseList.ToArray());
            singleRow.CalculateValues();
            lowestOwnRankSum = singleRow.OwnRankSum;

            // Högsta egen ranksumma
            singleRow = new HPTMarkBetSingleRow(highestOwnRankSumHorseList.ToArray());
            singleRow.CalculateValues();
            highestOwnRankSum = singleRow.OwnRankSum;

            // Sätt värdena på rätt ställe
            markBet.RowValueReductionRule.LowestSum = lowestEstimatedRowValue;
            markBet.RowValueReductionRule.HighestSum = highestEstimatedRowValue;
            //this.markBet.PercentSumReductionRule.LowestSum = this.lowestPercentSum;
            //this.markBet.PercentSumReductionRule.HighestSum = this.highestPercentSum;
            markBet.StakePercentSumReductionRule.LowestSum = lowestStakePercentSum;
            markBet.StakePercentSumReductionRule.HighestSum = highestStakePercentSum;
            markBet.StartNrSumReductionRule.LowestSum = lowestStartNumberSum;
            markBet.StartNrSumReductionRule.HighestSum = highestStartNumberSum;
            markBet.OddsSumReductionRule.LowestSum = lowestOddsSum;
            markBet.OddsSumReductionRule.HighestSum = highestOddsSum;
            markBet.ATGRankSumReductionRule.LowestSum = lowestATGRankSum;
            markBet.ATGRankSumReductionRule.HighestSum = highestATGRankSum;
            markBet.OwnRankSumReductionRule.LowestSum = lowestOwnRankSum;
            markBet.OwnRankSumReductionRule.HighestSum = highestOwnRankSum;

            markBet.pauseRecalculation = recalculationPaused;
        }

        internal void HandleHighestAndLowestIncludedSums()
        {
            if (SingleRows == null || SingleRows.Count == 0)
            {
                if (markBet.RowValueReductionRule != null)
                {
                    markBet.RowValueReductionRule.LowestIncludedSum = lowestEstimatedRowValue;
                    markBet.RowValueReductionRule.HighestIncludedSum = highestEstimatedRowValue;
                }
                return;
            }

            markBet.RowValueReductionRule.LowestIncludedSum = SingleRows.Min(sr => sr.RowValue);
            markBet.RowValueReductionRule.HighestIncludedSum = SingleRows.Max(sr => sr.RowValue);
            //this.markBet.PercentSumReductionRule.LowestIncludedSum = this.SingleRows.Min(sr => sr.PercentSum);
            //this.markBet.PercentSumReductionRule.HighestIncludedSum = this.SingleRows.Max(sr => sr.PercentSum);
            markBet.StakePercentSumReductionRule.LowestIncludedSum = SingleRows.Min(sr => sr.StakePercentSum);
            markBet.StakePercentSumReductionRule.HighestIncludedSum = SingleRows.Max(sr => sr.StakePercentSum);
            markBet.StartNrSumReductionRule.LowestIncludedSum = SingleRows.Min(sr => sr.StartNrSum);
            markBet.StartNrSumReductionRule.HighestIncludedSum = SingleRows.Max(sr => sr.StartNrSum);
            markBet.OddsSumReductionRule.LowestIncludedSum = SingleRows.Min(sr => sr.OddsSum);
            markBet.OddsSumReductionRule.HighestIncludedSum = SingleRows.Max(sr => sr.OddsSum);
            markBet.OwnRankSumReductionRule.LowestIncludedSum = SingleRows.Min(sr => sr.OwnRankSum);
            markBet.OwnRankSumReductionRule.HighestIncludedSum = SingleRows.Max(sr => sr.OwnRankSum);
            markBet.ATGRankSumReductionRule.LowestIncludedSum = SingleRows.Min(sr => sr.ATGRankSum);
            markBet.ATGRankSumReductionRule.HighestIncludedSum = SingleRows.Max(sr => sr.ATGRankSum);
        }

        #endregion

        public void RecalculateRowValues()
        {
            if (SingleRows == null || SingleRows.Count() == 0)
            {
                return;
            }
            SingleRows
                .ForEach(sr =>
                {
                    sr.CalculateValues();
                    sr.EstimateRowValue(markBet);
                });

            //this.SingleRows
            //    .AsParallel()
            //    .ForAll(sr =>
            //    {
            //        sr.CalculateValues();
            //        sr.EstimateRowValue(this.markBet);
            //    });
        }

        public int RecalculateTotalCouponSize()
        {
            totalCouponSize = 0;
            if (SingleRows != null)
            {
                totalCouponSize = SingleRows.Sum(sr => sr.BetMultiplierList.Sum());
            }
            return totalCouponSize;
        }

        #region Properties

        private int currentCouponNumber;
        public int CurrentCouponNumber
        {
            get
            {
                return currentCouponNumber;
            }
            set
            {
                currentCouponNumber = value;
                OnPropertyChanged("CurrentCouponNumber");
            }
        }

        private int numberOfCoveredRows;
        public int NumberOfCoveredRows
        {
            get
            {
                return numberOfCoveredRows;
            }
            set
            {
                numberOfCoveredRows = value;
                OnPropertyChanged("NumberOfCoveredRows");
            }
        }

        private decimal coveredRowsShare;
        public decimal CoveredRowsShare
        {
            get
            {
                return coveredRowsShare;
            }
            set
            {
                coveredRowsShare = value;
                OnPropertyChanged("CoveredRowsShare");
            }
        }

        private int numberOfAnalyzedRows;
        public int NumberOfAnalyzedRows
        {
            get
            {
                return numberOfAnalyzedRows;
            }
            set
            {
                numberOfAnalyzedRows = value;
                OnPropertyChanged("NumberOfAnalyzedRows");
            }
        }

        private decimal analyzedRowsShare;
        public decimal AnalyzedRowsShare
        {
            get
            {
                return analyzedRowsShare;
            }
            set
            {
                analyzedRowsShare = value;
                OnPropertyChanged("AnalyzedRowsShare");
            }
        }

        #endregion

        #region Komprimera till kuponger

        public void ResetCouponNumber()
        {
            if (SingleRows != null)
            {
                SingleRows
                    .ToList()
                    .ForEach(sr => sr.CouponNumber = 0);
            }
        }

        public void ClearRowCombinations()
        {
            if (CompressedCoupons != null && CompressedCoupons.Count > 1)
            {
                CompressedCoupons.Clear();
            }
            CurrentCouponNumber = 0;
            NumberOfCoveredRows = 0;
            CoveredRowsShare = 0M;
        }

        public void CompressToCouponsV6SingleRows()
        {
            var rowsWithV6 = SingleRows
                .Where(r => r.V6)
                .SelectMany(sr => sr.GetUniqueList())
                .ToList();

            var rowsWithoutV6 = SingleRows
                .Where(r => !r.V6)
                .SelectMany(sr => sr.GetUniqueList())
                .ToList();

            //var rowsToNotCompress = this.SingleRows
            //    .SelectMany(sr => sr.GetDuplicateList())
            //    .ToList();

            CompressToCouponsBetmultiplierSingleRows(rowsWithV6);
            CompressToCouponsBetmultiplierSingleRows(rowsWithoutV6);

            int depth = 1;
            while (rowsWithoutV6.Count > 0 || rowsWithV6.Count > 0)
            {
                rowsWithV6 = SingleRows
                .Where(r => r.V6)
                .SelectMany(sr => sr.GetDuplicateList(depth))
                .ToList();

                rowsWithoutV6 = SingleRows
                .Where(r => !r.V6)
                .SelectMany(sr => sr.GetDuplicateList(depth))
                .ToList();

                CompressToCouponsBetmultiplierSingleRows(rowsWithV6);
                CompressToCouponsBetmultiplierSingleRows(rowsWithoutV6);

                depth++;
            }

            int couponId = 1;
            foreach (var coupon in CompressedCoupons)
            {
                coupon.CouponNumber = couponId++;
            }
        }

        //public void CompressToCouponsV6SingleRows()
        //{
        //    var rowsWithV6 = this.SingleRows
        //        .Where(r => r.V6)
        //        .SelectMany(sr => sr.GetUniqueList())
        //        .ToList();

        //    var rowsWithoutV6 = this.SingleRows
        //        .Where(r => !r.V6)
        //        .SelectMany(sr => sr.GetUniqueList())
        //        .ToList();

        //    var rowsToNotCompress = this.SingleRows
        //        .SelectMany(sr => sr.GetDuplicateList())
        //        .ToList();

        //    // Skapa enkelradskuponger av specialfallen med flera exakt likadana rader
        //    foreach (var rowToNotCompress in rowsToNotCompress)
        //    {
        //        var duplicateMultiplier = rowToNotCompress.BetMultiplierList
        //            .Where(bm => bm == rowToNotCompress.BetMultiplier)
        //            .ToArray();

        //        for (int j = 1; j < duplicateMultiplier.Length; j++)
        //        {
        //            var rowCombination = new HPTMarkBetSingleRowCombination(rowToNotCompress);
        //            rowCombination.BetMultiplier = duplicateMultiplier[j];
        //            AddRowCombination(rowCombination, rowToNotCompress.BetMultiplier, rowToNotCompress.V6);
        //        }
        //    }

        //    CompressToCouponsBetmultiplierSingleRows(rowsWithV6);
        //    CompressToCouponsBetmultiplierSingleRows(rowsWithoutV6);

        //    int couponId = 1;
        //    foreach (var coupon in this.CompressedCoupons)
        //    {
        //        coupon.CouponNumber = couponId++;
        //    }
        //}

        public void CompressToCouponsBetmultiplierSingleRows(List<HPTMarkBetSingleRow> rowsToCompress)
        {
            if (rowsToCompress == null || rowsToCompress.Count() == 0)
            {
                return;
            }

            bool v6 = rowsToCompress.First().V6;

            foreach (int betMultiplier in markBet.BetType.BetMultiplierList)
            {
                var betMultiplierRowsToCompress = rowsToCompress
                    .Where(r => r.BetMultiplier == betMultiplier)
                    .ToDictionary(r => r.UniqueCode);
                CompressCouponsBruteForce(betMultiplierRowsToCompress, betMultiplier, v6);
            }
        }

        private void AddRowCombination(HPTMarkBetSingleRowCombination rowCombination, int betMultiplier, bool v6)
        {
            rowCombination.CouponNumber = CurrentCouponNumber;
            rowCombination.BetMultiplier = betMultiplier;
            rowCombination.V6 = v6;
            CompressedCoupons.Add(rowCombination);
            NumberOfCoveredRows += rowCombination.Size;
            CoveredRowsShare = Convert.ToDecimal(NumberOfCoveredRows) / Convert.ToDecimal(SingleRows.Count);
        }

        public void CompressToCouponsThreaded()
        {
            try
            {
                if (SingleRows == null
                || SingleRows.Count == 0
                || markBet.ReducedSize == 0)
                {
                    return;
                }
                // Skicka iväg komprimeringen i en ny tråd
                ThreadPool.QueueUserWorkItem(CompressToCouponsDelegate);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        internal void CompressToCouponsDelegate(object stateInfo)
        {
            try
            {
                // Håll koll på när beräkningen startade
                var dt = DateTime.Now;
                StopCalculation = true;
                //if (this.CouponCompressorList != null && this.CouponCompressorList.Count > 0)
                //{
                //    foreach (HPTCouponCompressor compressor in this.CouponCompressorList)
                //    {
                //        compressor.StopCalculation = true;
                //    }
                //}

                int i = 0;
                while ((CalculationInProgress || CompressionInProgress)
                    && i < 50)
                {
                    if (dt < lastRequestedRecalculation)
                    {
                        return;
                    }
                    Thread.Sleep(100);
                    i++;
                }
                if (SingleRows.First().CouponNumber > 0)
                {
                    ResetCouponNumber();
                }
                StopCalculation = false;
                CompressToCoupons();
            }
            catch (Exception)
            {
                StopCalculation = false;
            }
            CompressionInProgress = false;
        }

        public void CompressToCoupons()
        {
            NumberOfCoveredRows = 0;

            if (SingleRows == null
                || SingleRows.Count == 0
                || SingleRows.First().CouponNumber > 0
                || markBet.ReducedSize == 0)
            {
                CompressionInProgress = false;
                return;
            }

            CompressionInProgress = true;
            if (CompressedCoupons == null)
            {
                CompressedCoupons = new List<HPTMarkBetSingleRowCombination>();
            }
            else
            {
                CompressedCoupons.Clear(); ;
            }
            CurrentCouponNumber = 0;

            lock (this)
            {
                // Sortera fallande efter raderna med högst täckning på systemet. Större chans att de går att slå ihop
                //this.SingleRows = this.SingleRowso.OrderByDescending(sr => sr.HorseList.Sum(h => h.SystemCoverage)).ToList();
                //var singleRows = this.SingleRowsObservable
                //    .OrderByDescending(sr => sr.HorseList.Sum(h => h.SystemCoverage))
                //    .ToList();

                // Om ingen reducering föreligger
                if (reductionRulesToApply.Count == 0 && !markBet.V6SingleRows && !markBet.SingleRowBetMultiplier && !markBet.ReductionV6BetMultiplierRule)
                {
                    NumberOfCoveredRows = markBet.SystemSize;
                    CurrentCouponNumber = 1;
                    CoveredRowsShare = 1M;
                }

                // Om man har valt V6 och/eller flerbong på enskilda enkelrader samt överskrider ATGs nya gränser
                else if (markBet.V6SingleRows
                    || markBet.SingleRowBetMultiplier
                    || markBet.ReductionV6BetMultiplierRule
                    || SingleRows.Any(sr => sr.Edited))
                {
                    CompressToCouponsV6SingleRows();
                }

                // Utfyllning med flerbong upp till ett viss belopp
                else if (markBet.BetMultiplierRowAddition && markBet.BetMultiplierRowAdditionTarget > 0)
                {
                    CompressToCouponsV6SingleRows();
                }

                // Ordinarie brute force-lösning...
                else
                {
                    CompressCouponsBruteForce();
                }
                CompressionInProgress = false;
                markBet.UpdateCoupons();
                StopCalculation = false;
                markBet.TotalCouponSize = RecalculateTotalCouponSize();
            }
        }

        private void CompressCouponsBruteForce()
        {
            var rowsToCompress = SingleRows
                .Where(sr => sr.CouponNumber == 0)
                .OrderByDescending(sr => sr.HorseList.Sum(h => h.SystemCoverage))
                .ToDictionary(sr => sr.UniqueCode);

            CompressCouponsBruteForce(rowsToCompress, markBet.BetMultiplier, markBet.V6);
        }

        private void CompressCouponsBruteForce(Dictionary<string, HPTMarkBetSingleRow> rowsToCompress, int betMultiplier, bool v6)
        {
            var dt = DateTime.Now;  // DEBUG

            while (rowsToCompress.Count > 0)
            {
                if (StopCalculation || dt < lastRequestedRecalculation)
                {
                    throw new Exception();
                }

                var startRow = rowsToCompress.Values.First();

                startRow.CouponNumber = CurrentCouponNumber + 1;
                var rowCombination = new HPTMarkBetSingleRowCombination(startRow);
                rowsToCompress.Remove(startRow.UniqueCode);

                var remainingHorsesList = markBet.RaceDayInfo.HorseListSelected
                    .Except(startRow.HorseList)
                    .OrderByDescending(h => h.SystemCoverage)
                    .ToList();

                foreach (HPTHorse horse in remainingHorsesList)
                {
                    rowCombination.AddHorseNew(horse);
                    bool includeHorse = rowCombination.CheckAddedRow(rowsToCompress, CurrentCouponNumber + 1);
                }
                CurrentCouponNumber++;

                AddRowCombination(rowCombination, betMultiplier, v6);
            }
            TimeSpan ts = DateTime.Now - dt;
            string s = ts.TotalSeconds.ToString();
        }

        #endregion

        public void SetRankSums(HPTMarkBetTemplateRank templateRank)
        {
            var singleRows = SingleRows
                .OrderBy(sr => sr.RankSum)
                .ToList();

            decimal factor = Convert.ToDecimal(templateRank.DesiredSystemSize) / (1.0M - templateRank.ReductionPercentage / 100.0M) / Convert.ToDecimal(singleRows.Count);
            int minPos = Convert.ToInt32(singleRows.Count * templateRank.LowerPercentageLimit / 100 * factor) - 1;
            int maxPos = Convert.ToInt32(singleRows.Count * templateRank.UpperPercentageLimit / 100 * factor) - 1;
            templateRank.MinRankValue = Math.Round(singleRows[minPos].RankSum, 1);
            templateRank.MaxRankValue = Math.Round(singleRows[maxPos].RankSum, 1);
        }

        private int CompareRankSum(HPTMarkBetSingleRow sr1, HPTMarkBetSingleRow sr2)
        {
            decimal result = sr1.RankSum - sr2.RankSum;
            if (result < 0)
            {
                return -1;
            }
            else if (result > 0)
            {
                return 1;
            }
            return 0;
        }

        #region Beräkna siffror för enskild reducering

        private HPTReductionRule reductionRuleToTest;
        private Dictionary<int, List<HPTHorse>> raceSelectedHorseList;
        private Dictionary<int, int> numberOfCorrectDictionary;
        public void CalculateRuleStatistics(HPTReductionRule reductionRuleToTest)
        {
            try
            {
                // Sätt rätt regel
                this.reductionRuleToTest = reductionRuleToTest;
                this.reductionRuleToTest.Reset();

                // Nollställ räknaren
                numberOfRowsForTestedRule = 0;
                probabilityForTestedRule = 0M;

                // Skapa lista att loopa över
                raceSelectedHorseList = markBet.RaceDayInfo.RaceList
                    .Select(r => r.HorseListSelected)
                    .ToDictionary(h => h.First().ParentRace.LegNr);

                // Skapa dictionary för att kontrollera hur många vinstrader villkoret skulle gett
                if (markBet.RaceDayInfo.ResultComplete)
                {
                    this.reductionRuleToTest.GetRuleResultForCorrectRow(markBet);

                    numberOfCorrectDictionary = new Dictionary<int, int>();
                    markBet.RaceDayInfo.PayOutList
                        .OrderByDescending(po => po.NumberOfCorrect)
                        .ToList()
                        .ForEach(po =>
                        {
                            numberOfCorrectDictionary.Add(po.NumberOfCorrect, 0);
                        });
                }

                // Skapa alla enkelrader
                CalculateRuleStatistics(1);

                // Sätt värdena på regeln
                reductionRuleToTest.RemainingRowsPercentage = Convert.ToDecimal(numberOfRowsForTestedRule) / Convert.ToDecimal(markBet.SystemSize);
                reductionRuleToTest.Probability = probabilityForTestedRule;
                reductionRuleToTest.ProbabilityRelative = probabilityForTestedRule / markBet.SystemProbability / reductionRuleToTest.RemainingRowsPercentage;
                reductionRuleToTest.RemainingRows = numberOfRowsForTestedRule;
                reductionRuleToTest.SetReductionSpecificationString();

                // Sätt antal rader med 0, 1 och 2 fel
                if (numberOfCorrectDictionary != null)
                {
                    this.reductionRuleToTest.NumberOfAllCorrect = numberOfCorrectDictionary.Values.ElementAt(0);
                    if (numberOfCorrectDictionary.Count > 1)
                    {
                        this.reductionRuleToTest.NumberOfOneError = numberOfCorrectDictionary.Values.ElementAt(1);
                        if (numberOfCorrectDictionary.Count > 2)
                        {
                            // TODO: V85
                            this.reductionRuleToTest.NumberOfTwoErrors = numberOfCorrectDictionary.Values.ElementAt(2);
                            if (numberOfCorrectDictionary.Count > 3)
                            {
                                // TODO: V85
                                this.reductionRuleToTest.NumberOfThreeErrors = numberOfCorrectDictionary.Values.ElementAt(3);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
                //ResetValues();
                //throw exc;
            }
        }

        internal int numberOfRowsForTestedRule;
        internal decimal probabilityForTestedRule;
        private void CalculateRuleStatistics(int raceNumber)
        {
            try
            {
                if (raceNumber > markBet.NumberOfRaces)   // Sista loppet
                {
                    var singleRow = new HPTMarkBetSingleRow(horseList);
                    if (reductionRuleToTest.IncludeRow(markBet, singleRow))
                    {
                        if (!singleRow.ValuesCalculated)
                        {
                            singleRow.CalculateValues();
                        }
                        probabilityForTestedRule += singleRow.RowShareStake;
                        numberOfRowsForTestedRule++;
                        if (numberOfCorrectDictionary != null)
                        {
                            int numberOfCorrect = markBet.CouponCorrector.HorseList.Intersect(horseList).Count();
                            if (numberOfCorrectDictionary.ContainsKey(numberOfCorrect))
                            {
                                numberOfCorrectDictionary[numberOfCorrect] += 1;
                            }
                        }
                    }
                    return;
                }
                else if (raceNumber > 2)    // Andra loppet eller senare
                {
                    if (!reductionRuleToTest.IncludeRow(markBet, horseList, raceNumber - 1))
                    {
                        return;
                    }
                }
                foreach (var horse in raceSelectedHorseList[raceNumber])
                {
                    horseList[raceNumber - 1] = horse;
                    CalculateRuleStatistics(raceNumber + 1);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return;
        }


        #endregion

        #region Intervallreduceringar

        private decimal minRankSum;
        public decimal MinRankSum
        {
            get
            {
                return minRankSum;
            }
            set
            {
                minRankSum = value;
                OnPropertyChanged("MinRankSum");
            }
        }

        private decimal maxRankSum;
        public decimal MaxRankSum
        {
            get
            {
                return maxRankSum;
            }
            set
            {
                maxRankSum = value;
                OnPropertyChanged("MaxRankSum");
            }
        }

        private int lowestEstimatedRowValue;

        private int highestEstimatedRowValue;

        private int lowestPercentSum;

        private int highestPercentSum;

        private int lowestStartNumberSum;

        private int highestStartNumberSum;

        private int lowestOddsSum;

        private int highestOddsSum;

        private int lowestStakePercentSum;

        private int highestStakePercentSum;

        private int lowestATGRankSum;

        private int highestATGRankSum;

        private int lowestOwnRankSum;

        private int highestOwnRankSum;

        private void ApplyRankSumPercentageRule()
        {
            if (markBet.ReductionRank && (markBet.MinRankSumPercent > 0 || markBet.MaxRankSumPercent < 100))
            {
                IOrderedEnumerable<HPTMarkBetSingleRow> orderedRows = AllRows.OrderBy(singleRow => singleRow.RankSum);
                int lowerPos = Convert.ToInt32(Convert.ToDecimal(markBet.MinRankSumPercent) / 100M * AllRows.Length);
                int upperPos = Convert.ToInt32(Convert.ToDecimal(markBet.MaxRankSumPercent) / 100M * AllRows.Length);
                RemoveSingleRows(lowerPos, upperPos, orderedRows);
            }
        }

        private void ApplyRowValuePercentageRule()
        {
            if (markBet.RowValueReductionRule.Use && (markBet.RowValueReductionRule.MinPercentSum > 0 || markBet.RowValueReductionRule.MaxPercentSum < 100))
            {
                IOrderedEnumerable<HPTMarkBetSingleRow> orderedRows = AllRows.OrderBy(singleRow => singleRow.RowValue);
                int lowerPos = Convert.ToInt32(Convert.ToDecimal(markBet.RowValueReductionRule.MinPercentSum) / 100M * AllRows.Length);
                int upperPos = Convert.ToInt32(Convert.ToDecimal(markBet.RowValueReductionRule.MaxPercentSum) / 100M * AllRows.Length);
                RemoveSingleRows(lowerPos, upperPos, orderedRows);
            }
        }

        private void ApplyStakePercentSumPercentageRule()
        {
            if (markBet.StakePercentSumReductionRule.Use && (markBet.StakePercentSumReductionRule.MinPercentSum > 0 || markBet.StakePercentSumReductionRule.MaxPercentSum < 100))
            {
                IOrderedEnumerable<HPTMarkBetSingleRow> orderedRows = AllRows.OrderBy(singleRow => singleRow.StakePercentSum);
                int lowerPos = Convert.ToInt32(Convert.ToDecimal(markBet.StakePercentSumReductionRule.MinPercentSum) / 100M * AllRows.Length);
                int upperPos = Convert.ToInt32(Convert.ToDecimal(markBet.StakePercentSumReductionRule.MaxPercentSum) / 100M * AllRows.Length);
                RemoveSingleRows(lowerPos, upperPos, orderedRows);
            }
        }

        private void ApplyStartNumberSumPercentageRule()
        {
            if (markBet.StartNrSumReductionRule.Use && (markBet.StartNrSumReductionRule.MinPercentSum > 0 || markBet.StartNrSumReductionRule.MaxPercentSum < 100))
            {
                IOrderedEnumerable<HPTMarkBetSingleRow> orderedRows = AllRows.OrderBy(singleRow => singleRow.StartNrSum);
                int lowerPos = Convert.ToInt32(Convert.ToDecimal(markBet.StartNrSumReductionRule.MinPercentSum) / 100M * AllRows.Length);
                int upperPos = Convert.ToInt32(Convert.ToDecimal(markBet.StartNrSumReductionRule.MaxPercentSum) / 100M * AllRows.Length);
                RemoveSingleRows(lowerPos, upperPos, orderedRows);
            }
        }

        private void RemoveSingleRows(int lowerPos, int upperPos, IOrderedEnumerable<HPTMarkBetSingleRow> orderedRows)
        {
            HPTMarkBetSingleRow[] orderedArray = orderedRows.ToArray();
            for (int i = 0; i < lowerPos; i++)
            {
                HPTMarkBetSingleRow row = SingleRows.FirstOrDefault(r => r.UniqueCode == orderedArray[i].UniqueCode);
                if (row != null)
                {
                    SingleRows.Remove(row);
                }
            }
            for (int i = upperPos; i < AllRows.Length; i++)
            {
                var row = SingleRows
                    .FirstOrDefault(r => r.UniqueCode == orderedArray[i].UniqueCode);
                if (row != null)
                {
                    SingleRows.Remove(row);
                }
            }
        }

        public string ToSingleRowsString()
        {
            if (SingleRows == null)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Enkelrader:");
            foreach (HPTMarkBetSingleRow singleRow in SingleRows.OrderBy(sr => sr.RowNumber))
            {
                sb.AppendLine(singleRow.ToString());
            }
            return sb.ToString();
        }

        public string ToCouponsString()
        {
            StringBuilder sb = new StringBuilder();
            // TODO: Använd HPTCoupon istället och ta med reserverna
            foreach (HPTMarkBetSingleRowCombination rowCombination in CompressedCoupons)
            {
                sb.AppendLine(rowCombination.ToCouponString());
            }

            return sb.ToString();
        }

        #endregion

        #region Plocka bort utifrån radskillnad

        internal void RemoveWithRowDifference()
        {
            if (markBet.GuaranteeReduction && markBet.NumberOfToleratedErrors > 0)
            {
                // Håll koll på när beräkningen börjar
                var dt = DateTime.Now;

                //this.smallestRowDifferenceToSelect = this.markBet.NumberOfToleratedErrors + 1;

                // Hela uppsättninge rader
                var selectedRows = SingleRows
                    .Where(sr => sr.Selected)
                    .OrderBy(sr => sr.UniqueCode)
                    .ToDictionary(sr => sr.UniqueCode);

                rowsToSelectFrom = new Dictionary<string, HPTMarkBetSingleRow>(selectedRows);

                // Temporär lista för de rader som täcker upp övriga
                var rowsToKeep = new List<HPTMarkBetSingleRow>();

                // Nollställ reducerad storlek så det syns i gränssnittet att bearbetningen fortgår
                markBet.ReducedSize = 0;
                singleRows.Clear();

                // Välj första raden för att dra igång det hela
                var singleRow = rowsToSelectFrom.Values.First();

                while (selectedRows.Count > 0)  // Så länge inte alla rader är täckta
                {
                    if (StopCalculation || dt < lastRequestedRecalculation)
                    {
                        return;
                    }

                    selectedRows.Remove(singleRow.UniqueCode);
                    if (rowsToSelectFrom.ContainsKey(singleRow.UniqueCode))
                    {
                        rowsToSelectFrom.Remove(singleRow.UniqueCode);
                    }


                    // Ta bort de täckta raderna
                    RemoveCodesWithDifference(singleRow, selectedRows, markBet.NumberOfToleratedErrors);

                    singleRow.Selected = true;
                    rowsToKeep.Add(singleRow);
                    singleRows.Add(singleRow);
                    markBet.ReducedSize++;

                    if (rowsToSelectFrom.Count > 0)
                    {
                        singleRow = rowsToSelectFrom.Values.First();
                    }
                    else if (selectedRows.Count > 0)
                    {
                        singleRow = selectedRows.Values.First();
                    }
                }
                //this.SingleRows.Clear();
                //this.SingleRows.AddRange(rowsToKeep);
                //this.SingleRows = new List<HPTMarkBetSingleRow>(rowsToKeep);
                ResetNumberOfCoveredRows();
                RecalculateTotalCouponSize();
            }
        }

        Dictionary<string, HPTMarkBetSingleRow> rowsToSelectFrom;
        internal void RemoveCodesWithDifference(HPTMarkBetSingleRow singleRow, Dictionary<string, HPTMarkBetSingleRow> selectedRows, int difference)
        {
            var horseList = singleRow.HorseList
                .ToDictionary(h => h.ParentRace.LegNr);

            markBet.RaceDayInfo.HorseListSelected
                .Except(horseList.Values)
                .ToList()
                .ForEach(horse =>
                {
                    var originalHorse = horseList[horse.ParentRace.LegNr];
                    horseList[horse.ParentRace.LegNr] = horse;

                    string uniqueCode = horseList.Values
                        .Select(h => h.HexCode)
                        .Aggregate((h, next) => h + next);

                    if (difference > 0 && selectedRows.ContainsKey(uniqueCode))
                    {
                        selectedRows.Remove(uniqueCode);
                        if (rowsToSelectFrom.ContainsKey(uniqueCode))
                        {
                            rowsToSelectFrom.Remove(uniqueCode);
                        }
                    }
                    //if (this.rowsToSelectFrom.ContainsKey(uniqueCode))
                    //{
                    //    this.rowsToSelectFrom.Remove(uniqueCode);
                    //}
                    if (difference > 0)
                    {
                        var newSingleRow = new HPTMarkBetSingleRow(horseList.Values.ToArray());
                        RemoveCodesWithDifference(newSingleRow, selectedRows, difference - 1);
                    }
                    horseList[horse.ParentRace.LegNr] = originalHorse;
                });
        }

        internal void RemoveWithRowDifferenceSlow()
        {
            if (markBet.GuaranteeReduction && markBet.NumberOfToleratedErrors > 0)
            {
                // Håll koll på när beräkningen börjar
                var dt = DateTime.Now;

                // Hela uppsättninge rader
                var selectedRowsDictionary = SingleRows
                    .Where(sr => sr.Selected)
                    .OrderBy(sr => sr.UniqueCode)
                    .ToDictionary(sr => sr.UniqueCode);

                rowsToSelectFrom = new Dictionary<string, HPTMarkBetSingleRow>(selectedRowsDictionary);

                // Temporär lista för de rader som täcker upp övriga
                var rowsToKeep = new List<HPTMarkBetSingleRow>();

                // Nollställ reducerad storlek så det syns i gränssnittet att bearbetningen fortgår
                markBet.ReducedSize = 0;
                singleRows.Clear();

                var rowsToSelectFromTemp = selectedRowsDictionary.Values.ToList();

                //// Välj första raden som dummy
                //var singleRow = rowsToSelectFrom.Values
                //    .OrderByDescending(sr => sr.HorseList.Sum(h => h.NumberOfCoveredRows))
                //    .First();

                int maxCoveredRows = markBet.SystemSize - 1;
                int maxCoveredRows2 = markBet.SystemSize - 1;
                HPTMarkBetSingleRow singleRow = null;

                while (selectedRowsDictionary.Count > 0)  // Så länge inte alla rader är täckta
                {
                    if (StopCalculation || dt < lastRequestedRecalculation)
                    {
                        return;
                    }

                    // Bästa resultatet i loopen nedan
                    int mostCoveredRows = 0;
                    int leastCoveredRows2 = 0;

                    //foreach (var row in rowsToSelectFrom.Values)
                    foreach (var row in rowsToSelectFromTemp)
                    {
                        //if (this.markBet.NumberOfToleratedErrors == 1)
                        if (markBet.NumberOfToleratedErrors < 3)
                        {
                            int numberOfDiff1 = 0;
                            int numberOfDiff2 = 0;
                            //foreach (var innerRow in rowsToSelectFrom.Values)
                            foreach (var innerRow in selectedRowsDictionary.Values)
                            {
                                int numberOfDiff = innerRow.RowDifference(row, 2);
                                if (numberOfDiff == 1)
                                {
                                    numberOfDiff1++;
                                }
                                else if (numberOfDiff == 2)
                                {
                                    numberOfDiff2++;
                                }
                            }
                            if (numberOfDiff1 > mostCoveredRows || (numberOfDiff1 == mostCoveredRows && numberOfDiff2 < leastCoveredRows2))
                            {
                                mostCoveredRows = numberOfDiff1;
                                leastCoveredRows2 = numberOfDiff2;
                                singleRow = row;
                            }
                        }
                        //else if (this.markBet.NumberOfToleratedErrors == 2)
                        //{
                        //    int numberOfRowsCovered = rowsToSelectFrom.Values.Count(sr => sr.RowDifferenceInInterval(row, 1, 2));
                        //    if (numberOfRowsCovered >= mostCoveredRows)
                        //    {
                        //        singleRow = row;
                        //        mostCoveredRows = numberOfRowsCovered;
                        //        if (mostCoveredRows == maxCoveredRows)
                        //        {
                        //            break;
                        //        }
                        //    }
                        //}
                    }

                    if (mostCoveredRows == 0)
                    {
                        singleRow = rowsToSelectFrom.Values.First();
                    }

                    //foreach (var sr in rowsToSelectFrom.Values)
                    //{
                    //    coveredRowsGuaranteeReductionTemp = 0;
                    //    CountCoveredRowsWithDifference(sr, rowsToSelectFrom, 2);// this.markBet.NumberOfToleratedErrors);
                    //    if (coveredRowsGuaranteeReductionTemp > mostCoveredRows)
                    //    {
                    //        singleRow = sr;
                    //        mostCoveredRows = coveredRowsGuaranteeReductionTemp;
                    //        if (mostCoveredRows == maxCoveredRows)
                    //        {
                    //            break;
                    //        }
                    //    }
                    //}

                    // Ändra max antal täckta rader
                    if (mostCoveredRows < maxCoveredRows)
                    {
                        maxCoveredRows = mostCoveredRows;
                    }

                    selectedRowsDictionary.Remove(singleRow.UniqueCode);
                    if (rowsToSelectFrom.ContainsKey(singleRow.UniqueCode))
                    {
                        rowsToSelectFrom.Remove(singleRow.UniqueCode);
                    }

                    // Ta bort de täckta raderna
                    RemoveCodesWithDifference(singleRow, selectedRowsDictionary, markBet.NumberOfToleratedErrors);

                    rowsToSelectFromTemp = rowsToSelectFrom.Values
                        .Where(sr => sr.RowDifferenceInInterval(singleRow, 3, 3))
                        .OrderBy(sr => sr.UniqueCode)
                        .ToList();
                    //rowsToSelectFromTemp = rowsToSelectFrom.Values.Where(sr => sr.RowDifferenceInInterval(singleRow, this.markBet.NumberOfToleratedErrors + 1, this.markBet.NumberOfToleratedErrors + 2)).ToList();

                    if (rowsToSelectFromTemp.Count == 0)
                    {
                        rowsToSelectFromTemp = rowsToSelectFrom.Values
                            .OrderBy(sr => sr.UniqueCode)
                            .ToList();
                    }

                    singleRow.Selected = true;
                    rowsToKeep.Add(singleRow);
                    singleRows.Add(singleRow);
                    markBet.ReducedSize++;
                }
                ResetNumberOfCoveredRows();
                RecalculateTotalCouponSize();
            }
        }

        int coveredRowsGuaranteeReductionTemp;
        internal void CountCoveredRowsWithDifference(HPTMarkBetSingleRow singleRow, Dictionary<string, HPTMarkBetSingleRow> selectedRows, int difference)
        {
            var horseList = singleRow.HorseList
                .ToDictionary(h => h.ParentRace.LegNr);

            markBet.RaceDayInfo.HorseListSelected
                .Except(horseList.Values)
                .ToList()
                .ForEach(horse =>
                {
                    var originalHorse = horseList[horse.ParentRace.LegNr];
                    horseList[horse.ParentRace.LegNr] = horse;

                    string uniqueCode = horseList.Values
                        .Select(h => h.HexCode)
                        .Aggregate((h, next) => h + next);

                    // Raden finns bland kvaravarande rader vårt reducerade system
                    if (selectedRows.ContainsKey(uniqueCode))
                    {
                        coveredRowsGuaranteeReductionTemp++;
                    }

                    // För tvåfelsgarantireducering, avvaktar med den
                    if (difference > 1)
                    {
                        var newSingleRow = new HPTMarkBetSingleRow(horseList.Values.ToArray());
                        CountCoveredRowsWithDifference(newSingleRow, selectedRows, difference - 1);
                    }

                    horseList[horse.ParentRace.LegNr] = originalHorse;
                });
        }

        //internal int CompareRows(HPTMarkBetSingleRow sr1, HPTMarkBetSingleRow sr2, int max)
        //{
        //    int numberOfDifferent = 0;
        //    for (int i = 0; i < sr1.RowDifference; i++)
        //    {

        //    }
        //}

        internal void SetBetMultiplierToReachTarget()
        {
            RecalculateTotalCouponSize();
            decimal systemCost = totalCouponSize * markBet.BetType.RowCost;
            if (markBet.BetMultiplierRowAddition && markBet.BetMultiplierRowAdditionTarget > systemCost)
            {
                int numberOfRowsToDouble = Convert.ToInt32((markBet.BetMultiplierRowAdditionTarget - systemCost) /
                                          markBet.BetType.RowCost);

                var rowsToDouble = singleRows.Where(sr => sr.BetMultiplier == 1).OrderBy(sr => sr.RowValue).Take(numberOfRowsToDouble);
                foreach (var singleRow in rowsToDouble)
                {
                    if (StopCalculation)
                    {
                        return;
                    }
                    singleRow.BetMultiplier = 2;
                    singleRow.BetMultiplierList = new List<int>() { 2 };
                    totalCouponSize++;
                }
            }
        }

        internal void RemoveOwnProbabilityRowsToReachTarget()
        {
            decimal systemCost = totalCouponSize * markBet.BetType.RowCost;

            if (markBet.OwnProbabilityCost && markBet.OwnProbabilityCostTarget < systemCost)
            {
                int numberOfRowsToRemove = Convert.ToInt32((systemCost - markBet.OwnProbabilityCostTarget) /
                                          markBet.BetType.RowCost);

                var singleRowsArray = singleRows
                    .OrderBy(sr => sr.OwnProbabilityQuota)
                    .Take(numberOfRowsToRemove)
                    .ToArray();

                foreach (var singleRowToRemove in singleRowsArray)
                {
                    singleRows.Remove(singleRowToRemove);
                }
                ResetNumberOfCoveredRows();
            }
        }

        internal void RemoveRandomRowsToReachTarget()
        {
            decimal systemCost = totalCouponSize * markBet.BetType.RowCost;

            if (markBet.RandomRowReduction && markBet.RandomRowReductionTarget < systemCost)
            {
                int numberOfRowsToRemove = Convert.ToInt32((systemCost - markBet.RandomRowReductionTarget) /
                                          markBet.BetType.RowCost);
                int couponSizeToReach = totalCouponSize - numberOfRowsToRemove;
                int upperLimit = singleRows.Count - 1;
                var singleRowsArray = singleRows.ToArray();
                var rnd = new Random();
                for (int i = 0; i < numberOfRowsToRemove; i++)
                {
                    if (StopCalculation)
                    {
                        return;
                    }
                    int position = rnd.Next(upperLimit);
                    var singleRowToRemove = singleRowsArray[position];
                    SingleRows.Remove(singleRowToRemove);
                    singleRowsArray[position] = singleRowsArray[upperLimit];
                    totalCouponSize -= singleRowToRemove.BetMultiplier;
                    if (totalCouponSize <= couponSizeToReach)
                    {
                        break;
                    }
                    upperLimit--;
                }
                ResetNumberOfCoveredRows();
            }
        }

        internal void ResetNumberOfCoveredRows()
        {
            foreach (var horse in markBet.RaceDayInfo.HorseListSelected)
            {
                horse.NumberOfCoveredRows = 0;
            }
            foreach (var singleRow in SingleRows)
            {
                foreach (var horse in singleRow.HorseList)
                {
                    horse.NumberOfCoveredRows++;
                }
            }
        }

        #endregion

        #region OBSOLET KOD

        //internal void RemoveWithRowDifferenceBruteForce()
        //{
        //    if (this.markBet.GuaranteeReduction && this.markBet.NumberOfToleratedErrors > 0)
        //    {
        //        // Håll koll på när beräkningen börjar
        //        var dt = DateTime.Now;

        //        int smallestRowDifferenceToSelect = this.markBet.NumberOfToleratedErrors + 1;
        //        var selectedRows = this.SingleRows
        //            .Where(sr => sr.Selected)
        //            .OrderByDescending(sr => sr.HorseList.Sum(h => h.NumberOfCoveredRows))
        //            .ToList();

        //        // Skapa int-arrayer för snabbare bearbetning längre ner
        //        selectedRows.ForEach(sr => sr.StartNrList = sr.HorseList.Select(h => h.StartNr).ToArray());

        //        // Temporär lista för de rader som täcker upp övriga
        //        var rowsToKeep = new List<HPTMarkBetSingleRow>();

        //        // Nollställ reducerad storlek så det syns i gränssnittet att bearbetningen fortgår
        //        this.markBet.ReducedSize = 0;

        //        // Välj första raden för att inleda beräkningen
        //        var singleRow = selectedRows.First();

        //        // Välj de som är tillräckligt lika
        //        var rowsToDeselect = selectedRows
        //                    .Where(sr => !sr.HasRowDifference(singleRow, this.markBet.NumberOfToleratedErrors))
        //                    .ToList();

        //        // Sätt ribban för hur bra täckning vi vill ha
        //        int numberOfRowsToCover = rowsToDeselect.Count;

        //        // Ta bort raderna från originalkollektionen               
        //        foreach (var rowToDeselect in rowsToDeselect)
        //        {
        //            rowToDeselect.Selected = false;
        //            selectedRows.Remove(rowToDeselect);
        //        }
        //        singleRow.Selected = true;
        //        rowsToKeep.Add(singleRow);
        //        selectedRows.Remove(singleRow);
        //        this.markBet.ReducedSize++;

        //        while (selectedRows.Count > 0)  // Så länge inte alla rader är täckta
        //        {
        //            if (StopCalculation || dt < this.lastRequestedRecalculation)
        //            {
        //                return;
        //            }

        //            // Välj alla rader som är tillräckligt olik de redan utvalda
        //            var singleRowsWithPotential = selectedRows
        //                .Where(sr => sr.HasRowDifference(rowsToKeep, smallestRowDifferenceToSelect))
        //                .ToList();

        //            if (singleRowsWithPotential == null || singleRowsWithPotential.Count == 0)  // Det finns inga rader som är tillräckligt olika jämfört med redan utvalda
        //            {
        //                smallestRowDifferenceToSelect--;
        //            }
        //            else  // Det hittades minst en rad som var väldigt olik redan valda rader
        //            {
        //                HPTMarkBetSingleRow bestRow = null; // Bästa rad hittills
        //                int mostNumberOfRowsCovered = 0;    // Anntal rader ovanstående täcker

        //                foreach (var rowToTest in singleRowsWithPotential)
        //                {
        //                    // Välj rader som är tillräckligt lika
        //                    rowsToDeselect = selectedRows
        //                        .Where(sr => !sr.HasRowDifference(rowToTest, this.markBet.NumberOfToleratedErrors))
        //                        .ToList();

        //                    // Vi uppnår målet och lämnar loopen
        //                    if (rowsToDeselect.Count >= numberOfRowsToCover)
        //                    {
        //                        numberOfRowsToCover = rowsToDeselect.Count;
        //                        bestRow = rowToTest;
        //                        break;
        //                    }
        //                    // Vi uppnår inte målet, men ska kolla om raden åtminstone var bättre än tidigare bästa
        //                    else if (bestRow == null || rowsToDeselect.Count > mostNumberOfRowsCovered)
        //                    {
        //                        mostNumberOfRowsCovered = rowsToDeselect.Count;
        //                        bestRow = singleRow;
        //                    }
        //                }

        //                // Ställ om den till bästa resultatet vi nådde i den här rundan
        //                numberOfRowsToCover = mostNumberOfRowsCovered;

        //                // Välj de rader som slutligen ska tas bort
        //                rowsToDeselect = selectedRows
        //                    .Where(sr => !sr.HasRowDifference(bestRow, this.markBet.NumberOfToleratedErrors))
        //                    .ToList();

        //                // Ta bort raderna
        //                foreach (var rowToDeselect in rowsToDeselect)
        //                {
        //                    rowToDeselect.Selected = false;
        //                    selectedRows.Remove(rowToDeselect);
        //                }
        //                bestRow.Selected = true;
        //                rowsToKeep.Add(bestRow);
        //                selectedRows.Remove(bestRow);
        //                this.markBet.ReducedSize++;
        //            }
        //        }
        //        this.SingleRows = new List<HPTMarkBetSingleRow>(rowsToKeep);
        //        ResetNumberOfCoveredRows();
        //        RecalculateTotalCouponSize();
        //    }
        //}

        //// Komprimering för system som bara innehåller ABCD-reducering
        //public void CompressToCouponsABC()
        //{
        //    List<string> uniqueABCCombinations = this.SingleRows
        //        .Select(r => r.ABCDRankCode)
        //        .Distinct()
        //        .ToList();

        //    foreach (string abcdRankCode in uniqueABCCombinations)
        //    {
        //        if (this.StopCalculation)
        //        {
        //            throw new Exception();
        //        }
        //        List<HPTMarkBetSingleRow> abcdRankRowsToCompress = this.SingleRows
        //            .Where(r => r.ABCDRankCode == abcdRankCode)
        //            .ToList();

        //        HPTMarkBetSingleRow startRow = abcdRankRowsToCompress.First();

        //        List<HPTHorse> horsesToTest = new List<HPTHorse>();
        //        foreach (HPTHorse horse in startRow.HorseList)
        //        {
        //            horsesToTest.AddRange(this.markBet.RaceDayInfo.HorseListSelected.Where(h => h.Prio == horse.Prio && h.ParentRace.LegNr == horse.ParentRace.LegNr && h != horse));
        //        }

        //        HPTMarkBetSingleRowCombination rowCombination = new HPTMarkBetSingleRowCombination(startRow);

        //        foreach (HPTHorse horse in horsesToTest)
        //        {
        //            rowCombination.AddHorse(horse);
        //        }
        //        if (rowCombination.UniqueCodesRegex == null)
        //        {
        //            rowCombination.SetUniqueCodesRegexAndSystemSize();
        //        }
        //        rowCombination.CheckAddedRow(abcdRankRowsToCompress, this.currentCouponNumber + 1);
        //        this.CurrentCouponNumber++;
        //        AddRowCombination(rowCombination, this.markBet.BetMultiplier, this.markBet.V6);
        //    }
        //}    

        //internal void RemoveWithRowDifference()
        //{
        //    if (this.markBet.GuaranteeReduction && this.markBet.NumberOfToleratedErrors > 0)
        //    {
        //        // Håll koll på när beräkningen börjar
        //        var dt = DateTime.Now;

        //        int numberOfHorsesToMatch = this.markBet.NumberOfRaces - this.markBet.NumberOfToleratedErrors;
        //        var selectedRows = this.SingleRows.Where(sr => sr.Selected).ToList();
        //        var rowsToKeep = new List<HPTMarkBetSingleRow>();
        //        this.markBet.ReducedSize = 0;

        //        while (selectedRows.Count > 0)
        //        {
        //            if (StopCalculation || dt < this.lastRequestedRecalculation)
        //            {
        //                return;
        //            }
        //            var singleRow = selectedRows.First();

        //            var rowsToDeselect = selectedRows.Where(sr => sr.HorseList.Intersect(singleRow.HorseList).Count() >= numberOfHorsesToMatch).ToList();
        //            foreach (var rowToDeselect in rowsToDeselect)
        //            {
        //                rowToDeselect.Selected = false;
        //                selectedRows.Remove(rowToDeselect);
        //            }
        //            singleRow.Selected = true;
        //            rowsToKeep.Add(singleRow);
        //            selectedRows.Remove(singleRow);
        //            this.markBet.ReducedSize++;
        //        }
        //        this.SingleRows = new List<HPTMarkBetSingleRow>(rowsToKeep);
        //        ResetNumberOfCoveredRows();
        //        RecalculateTotalCouponSize();
        //    }
        //}

        //internal void RemoveWithRowDifferenceAlt()
        //{
        //    if (this.markBet.GuaranteeReduction && this.markBet.NumberOfToleratedErrors > 0)
        //    {
        //        int numberOfHorsesToMatch = this.markBet.NumberOfRaces - this.markBet.NumberOfToleratedErrors;
        //        var selectedRows = this.SingleRows
        //            .Where(sr => sr.Selected)
        //            .OrderBy(sr => sr.StartNrSum)
        //            .ToList();

        //        // Skapa int-arrayer för snabbare bearbetning längre ner
        //        selectedRows.ForEach(sr => sr.StartNrList = sr.HorseList.Select(h => h.StartNr).ToArray());

        //        // Temporär lista för de rader som täcker upp övriga
        //        var rowsToKeep = new List<HPTMarkBetSingleRow>();

        //        // Nollställ reducerad storlek så det syns i gränssnittet att bearbetningen fortgår
        //        this.markBet.ReducedSize = 0;

        //        while (selectedRows.Count > 0)  // Så länge inte alla rader är täckta
        //        {
        //            if (StopCalculation)
        //            {
        //                return;
        //            }
        //            var singleRow = selectedRows.First();

        //            var rowsToDeselect = selectedRows
        //                .Where(sr => !sr.HasRowDifference(singleRow, this.markBet.NumberOfToleratedErrors))
        //                .ToList();

        //            foreach (var rowToDeselect in rowsToDeselect)
        //            {
        //                rowToDeselect.Selected = false;
        //                selectedRows.Remove(rowToDeselect);
        //            }
        //            singleRow.Selected = true;
        //            rowsToKeep.Add(singleRow);
        //            selectedRows.Remove(singleRow);
        //            this.markBet.ReducedSize++;
        //        }
        //        this.SingleRows = new List<HPTMarkBetSingleRow>(rowsToKeep);
        //        ResetNumberOfCoveredRows();
        //        RecalculateTotalCouponSize();
        //    }
        //}

        //internal void RemoveWithRowDifferenceOptimized()
        //{
        //    if (this.markBet.GuaranteeReduction && this.markBet.NumberOfToleratedErrors > 0)
        //    {
        //        // Håll koll på när beräkningen börjar
        //        var dt = DateTime.Now;

        //        int smallestRowDifferenceToSelect = this.markBet.NumberOfToleratedErrors + 1;
        //        var selectedRows = this.SingleRows
        //            .Where(sr => sr.Selected)
        //            //.OrderBy(sr => sr.StartNrSum)     // Orsakar olika resultat
        //            .OrderBy(sr => sr.UniqueCode)
        //            .ToList();

        //        // Skapa int-arrayer för snabbare bearbetning längre ner
        //        selectedRows.ForEach(sr => sr.StartNrList = sr.HorseList.Select(h => h.StartNr).ToArray());

        //        // Temporär lista för de rader som täcker upp övriga
        //        var rowsToKeep = new List<HPTMarkBetSingleRow>();

        //        // Nollställ reducerad storlek så det syns i gränssnittet att bearbetningen fortgår
        //        this.markBet.ReducedSize = 0;

        //        while (selectedRows.Count > 0)  // Så länge inte alla rader är täckta
        //        {
        //            if (StopCalculation || dt < this.lastRequestedRecalculation)
        //            {
        //                return;
        //            }

        //            // Välj första rad som är väldigt olik de redan utvalda
        //            var singleRow = selectedRows.FirstOrDefault(sr => sr.HasRowDifference(rowsToKeep, smallestRowDifferenceToSelect));

        //            if (singleRow == null)  // Det finns inga rader som är tillräckligt olika jämfört med redan utvalda
        //            {
        //                smallestRowDifferenceToSelect--;
        //            }
        //            else  // Det hittades en rad som var väldigt olik redan valda rader
        //            {
        //                var rowsToDeselect = selectedRows
        //                    .Where(sr => !sr.HasRowDifference(singleRow, this.markBet.NumberOfToleratedErrors))
        //                    .ToList();

        //                rowsToDeselect.ForEach(sr =>
        //                    {
        //                        sr.Selected = false;
        //                        selectedRows.Remove(sr);
        //                    });
        //                //foreach (var rowToDeselect in rowsToDeselect)
        //                //{
        //                //    rowToDeselect.Selected = false;
        //                //    selectedRows.Remove(rowToDeselect);
        //                //}
        //                singleRow.Selected = true;
        //                rowsToKeep.Add(singleRow);
        //                selectedRows.Remove(singleRow);
        //                this.markBet.ReducedSize++;
        //            }
        //        }
        //        this.SingleRows = new List<HPTMarkBetSingleRow>(rowsToKeep);
        //        ResetNumberOfCoveredRows();
        //        RecalculateTotalCouponSize();

        //        TimeSpan ts = DateTime.Now - dt;
        //        string s = ts.TotalMilliseconds.ToString();
        //    }
        //}

        //private void CompressCouponsBruteForce(List<HPTMarkBetSingleRow> rowsToCompress, int betMultiplier, bool v6)
        //{
        //    var dt = DateTime.Now;
        //    while (rowsToCompress.Count > 0)
        //    {
        //        if (this.StopCalculation || dt < this.lastRequestedRecalculation)
        //        {
        //            throw new Exception();
        //        }

        //        HPTMarkBetSingleRow startRow = rowsToCompress.First();
        //        startRow.CouponNumber = this.CurrentCouponNumber + 1;
        //        HPTMarkBetSingleRowCombination rowCombination = new HPTMarkBetSingleRowCombination(startRow);

        //        List<HPTHorse> remainingHorsesList = this.markBet.RaceDayInfo.HorseListSelected
        //            .Except(startRow.HorseList)
        //            .OrderByDescending(h => h.SystemCoverage)
        //            .ToList();

        //        foreach (HPTHorse horse in remainingHorsesList)
        //        {
        //            rowCombination.AddHorse(horse);
        //            bool includeHorse = rowCombination.CheckAddedRow(rowsToCompress, this.CurrentCouponNumber + 1);
        //            if (!includeHorse)
        //            {
        //                rowCombination.RemoveHorse(horse);
        //            }
        //        }
        //        this.CurrentCouponNumber++;

        //        AddRowCombination(rowCombination, betMultiplier, v6);

        //        rowsToCompress = rowsToCompress
        //            .Where(sr => sr.CouponNumber == 0)
        //            .ToList();
        //    }

        //    TimeSpan ts = DateTime.Now - dt;
        //    string s = ts.TotalSeconds.ToString();
        //}

        //public void RemoveHorse(HPTHorse horse)
        //{
        //    int raceIndex = horse.ParentRace.RaceNr - 1;
        //    IEnumerable<HPTMarkBetSingleRow> allRows = this.AllRows.Where(sr => sr.HorseList[raceIndex] != horse);
        //    this.AllRows = allRows.ToArray();
        //    GC.Collect();
        //}

        //private List<HPTMarkBetSingleRow> addedRows;
        //public void AddHorse(HPTHorse horse)
        //{
        //    this.addedRows = new List<HPTMarkBetSingleRow>();
        //    MakeSingleRowCollection(0, horse);
        //    this.AllRows = this.AllRows.Concat(this.addedRows).ToArray();
        //    GC.Collect();
        //}

        //private void MakeSingleRowCollection(int legNumber, HPTHorse addedHorse)
        //{
        //    if (this.StopCalculation)
        //    {
        //        throw new Exception();
        //    }

        //    if (legNumber == this.markBet.NumberOfRaces)
        //    {
        //        HPTMarkBetSingleRow singleRow = new HPTMarkBetSingleRow(this.horseList);
        //        this.addedRows.Add(singleRow);
        //        return;
        //    }
        //    if (legNumber + 1 == addedHorse.ParentRace.LegNr)
        //    {
        //        this.horseList[legNumber] = addedHorse;
        //        MakeSingleRowCollection(legNumber + 1, addedHorse);
        //    }
        //    foreach (HPTHorse horse in this.markBet.RaceDayInfo.RaceList[legNumber].HorseListSelected)
        //    {
        //        this.horseList[legNumber] = horse;
        //        MakeSingleRowCollection(legNumber + 1, addedHorse);
        //    }
        //    return;
        //}

        #endregion
    }
}
