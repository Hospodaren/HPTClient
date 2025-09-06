using System;
using System.Collections.Generic;
using System.Linq;
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
            this.horseList = new HPTHorse[markBet.NumberOfRaces];
            this.lastRequestedRecalculation = DateTime.Now;
            this.singleRows = new List<HPTMarkBetSingleRow>();
        }

        private List<HPTMarkBetSingleRow> singleRows;
        public List<HPTMarkBetSingleRow> SingleRows
        {
            get
            {
                return this.singleRows;
            }
            set
            {
                this.singleRows = value;
                OnPropertyChanged("SingleRowsObservable");
            }
        }

        public void ClearAll()
        {
            if (this.SingleRows != null)
            {
                this.SingleRows.Clear();
            }
            if (this.CompressedCoupons != null)
            {
                this.CompressedCoupons.Clear();
            }
            this.CoveredRowsShare = 0M;
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
                this.StopCalculation = false;
                this.CalculationInProgress = true;
                try
                {
                    InitForRecalculation();

                    // Skapa alla enkelrader
                    MakeSingleRowCollection(0);

                    // Kör resterande regler och avsluta bearbetningen
                    if (this.markBet.IsCalculatingTemplates)
                    {
                        this.markBet.ReducedSize = this.singleRows.Count;
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
            this.totalCouponSize = 0;
            this.markBet.TotalCouponSize = 0;
            this.AnalyzedRowsShare = 0M;
            this.NumberOfAnalyzedRows = 0;
            this.NumberOfCoveredRows = 0;
            this.CoveredRowsShare = 0M;
            this.CalculationInProgress = false;
        }

        internal bool CheckCalculationStatus()
        {
            if (this.markBet.IsCalculatingTemplates)
            {
                return true;
            }
            DateTime dt = DateTime.Now;
            if (dt > this.lastRequestedRecalculation)
            {
                this.lastRequestedRecalculation = dt;
            }

            // Fall där det är onödigt att beräkna reducering
            if (this.markBet.IsDeserializing || this.markBet.SystemSize == 0)
            {
                return false;
            }

            // Kolla om beräkning av antal rader pågår
            if (this.CalculationInProgress)
            {
                this.StopCalculation = true;
            }

            // Kolla om kupongkomprimering pågår
            if (this.CompressionInProgress)
            {
                this.StopCalculation = true;
            }

            int i = 0;
            while ((this.CalculationInProgress || this.CompressionInProgress)
                && i < 50)
            {
                if (dt < lastRequestedRecalculation)
                {
                    return false;
                }
                System.Threading.Thread.Sleep(100);
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
            this.singleRows.Clear();

            this.orderedRaceHorseList = new List<HPTHorse>[this.markBet.RaceDayInfo.RaceList.Count];
            for (int i = 0; i < this.markBet.RaceDayInfo.RaceList.Count; i++)
            {
                if (this.markBet.ABCDEFReductionRule.Use || this.markBet.MultiABCDEFReductionRule.Use)
                {
                    this.orderedRaceHorseList[i] = this.markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.Prio)
                        .ToList();
                }
                else if (this.markBet.ReductionRank)
                {
                    this.orderedRaceHorseList[i] = this.markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.RankWeighted)
                        .ToList();
                }
                else if (this.markBet.StakePercentSumReductionRule.Use)
                {
                    this.orderedRaceHorseList[i] = this.markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.StakeDistributionPercent)
                        .ToList();
                }
                else if (this.markBet.StartNrSumReductionRule.Use)
                {
                    this.orderedRaceHorseList[i] = this.markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.StartNr)
                        .ToList();
                }
                else if (this.markBet.ATGRankSumReductionRule.Use)
                {
                    this.orderedRaceHorseList[i] = this.markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.RankATG)
                        .ToList();
                }
                else if (this.markBet.OwnRankSumReductionRule.Use)
                {
                    this.orderedRaceHorseList[i] = this.markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.RankOwn)
                        .ToList();
                }
                else if (this.markBet.OddsSumReductionRule.Use)
                {
                    this.orderedRaceHorseList[i] = this.markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderByDescending(h => h.VinnarOdds)
                        .ToList();
                }
                else
                {
                    this.orderedRaceHorseList[i] = this.markBet.RaceDayInfo.RaceList[i].HorseList
                        .Where(h => h.Selected)
                        .OrderBy(h => h.StakeDistributionPercent)
                        .ToList();
                }
            }

            this.NumberOfAnalyzedRows = 0;
            this.AnalyzedRowsShare = 0M;

            this.rowNumber = 1;
            this.reductionRulesToApply = this.markBet.ReductionRulesToApply;

            // Initiera variabel för totalt antal inlämnade rader med flerbong inkluderat
            this.totalCouponSize = 0;

            // Specialfall där vi behöver spara alla rader i ramen
            if (this.markBet.HasPercentageSumReduction || this.markBet.IsCalculatingTemplates)
            {
                this.SaveAllRows = true;
            }
            if (this.SaveAllRows)
            {
                this.AllRows = new HPTMarkBetSingleRow[this.markBet.SystemSize];
            }
        }

        internal void HandleAnalyzedRowCollection()
        {
            // Kör procentsummereglerna
            if (this.markBet.HasPercentageSumReduction)
            {
                ApplyRowValuePercentageRule();
                //ApplyPercentSumPercentageRule();
                ApplyRankSumPercentageRule();
                ApplyStakePercentSumPercentageRule();
                ApplyStartNumberSumPercentageRule();

                ResetNumberOfCoveredRows();
            }


            #region Test av algoritmer

            if (this.markBet.GuaranteeReduction && this.markBet.NumberOfToleratedErrors > 0)
            {
                if (this.markBet.FastGuaranteeReduction)
                {
                    RemoveWithRowDifference(); // Verkar funka som den ska
                }
                else
                {
                    DateTime dtSTart = DateTime.Now;
                    RemoveWithRowDifferenceSlow(); // Ny version att testa
                    TimeSpan ts = DateTime.Now - dtSTart;
                    string s = ts.TotalSeconds.ToString();
                    string n = this.markBet.ReducedSize.ToString();
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


            this.markBet.ReducedSize = this.SingleRows.Count;
            this.markBet.TotalCouponSize = this.totalCouponSize;

            //CompressToCoupons();
            this.CalculationInProgress = false;
            this.StopCalculation = false;

            if (this.AnalyzingFinished != null)
            {
                this.AnalyzingFinished();
            }
        }

        public void RecalculateCurrentRows()
        {
            if (this.AllRows == null || this.AllRows.Length == 0)
            {
                this.SaveAllRows = true;
                UpdateRowCollection();
                return;
            }
            if (this.reductionRulesToApply == null || this.reductionRulesToApply.Count == 0)
            {
                this.reductionRulesToApply = this.markBet.ReductionRulesToApply;
            }
            this.SingleRows.Clear();
            //this.SingleRows = new List<HPTMarkBetSingleRow>();

            this.NumberOfAnalyzedRows = 0;
            this.AnalyzedRowsShare = 0M;

            this.rowNumber = 1;
            foreach (HPTMarkBetSingleRow singleRow in this.AllRows)
            {
                AnalyzeRow(singleRow);
            }
            //this.SingleRowsObservable = new ObservableCollection<HPTMarkBetSingleRow>(this.SingleRows);
            this.markBet.ReducedSize = this.SingleRows.Count;
            this.markBet.TotalCouponSize = this.totalCouponSize;
        }

        private List<HPTReductionRule> reductionRulesToApply;
        private void MakeSingleRowCollection(int raceNumber)
        {
            if (raceNumber == this.markBet.NumberOfRaces)   // Sista loppet
            {
                if (this.StopCalculation)
                {
                    throw new Exception();
                }
                HPTMarkBetSingleRow singleRow = new HPTMarkBetSingleRow(this.horseList);
                if (this.SaveAllRows)
                {
                    this.AllRows[this.numberOfAnalyzedRows] = singleRow;
                }
                AnalyzeRow(singleRow);
                return;
            }
            else if (raceNumber > 1 && !this.SaveAllRows)    // Andra loppet eller senare
            {
                if (this.StopCalculation)
                {
                    throw new Exception();
                }
                if (!AnalyzeRowInAdvance(raceNumber))    // raceNumber är index i array... :-(
                {
                    return;
                }
            }
            foreach (HPTHorse horse in this.orderedRaceHorseList[raceNumber])
            {
                this.horseList[raceNumber] = horse;
                MakeSingleRowCollection(raceNumber + 1);
            }
            return;
        }

        internal bool AnalyzeRowInAdvance(int numberOfRacesToTest)
        {
            foreach (var reductionRule in this.reductionRulesToApply)
            {
                bool include = reductionRule.IncludeRow(this.markBet, this.horseList, numberOfRacesToTest);
                if (!include)
                {
                    // Hur många rader slipper vi kontrollera tack vare tidigt avbrott
                    int numberOfRowsCalculated = this.markBet.RaceDayInfo.RaceList
                        .Where(r => r.LegNr > numberOfRacesToTest)
                        .Select(r => r.NumberOfSelectedHorses)
                        .Aggregate((numberOfChosen, next) => numberOfChosen * next);

                    // Uppdatera så att antalet analyserade rader stämmer
                    this.NumberOfAnalyzedRows += numberOfRowsCalculated;
                    this.AnalyzedRowsShare = Convert.ToDecimal(this.numberOfAnalyzedRows) / Convert.ToDecimal(this.markBet.SystemSize);

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
                this.numberOfAnalyzedRows++;
                if (this.numberOfAnalyzedRows % 739 == 0 || this.numberOfAnalyzedRows == this.markBet.SystemSize)
                {
                    this.NumberOfAnalyzedRows = this.numberOfAnalyzedRows;
                    this.AnalyzedRowsShare = Convert.ToDecimal(this.numberOfAnalyzedRows) /
                                             Convert.ToDecimal(this.markBet.SystemSize);
                }
            }

            // Kör igenom alla regler
            if (!TestRules(singleRow))
            {
                return;
            }

            // Add single row to collection
            lock (this.SingleRows)
            {
                // Gör beräkningar på enkelraden
                if (!singleRow.ValuesCalculated)
                {
                    singleRow.CalculateValues();
                }
                if (singleRow.RowValue == 0)
                {
                    singleRow.EstimateRowValue(this.markBet);
                }
                this.SingleRows.Add(singleRow);
            }
            singleRow.SetV6BetMultiplier(this.markBet);
            this.totalCouponSize += singleRow.BetMultiplier;

            // Lägg till radtäckning för varje häst
            foreach (HPTHorse horse in singleRow.HorseList)
            {
                lock (horse)
                {
                    horse.NumberOfCoveredRows++;
                }
            }

            // Räkna upp reducerad storlek
            this.markBet.ReducedSize = this.SingleRows.Count;
            this.markBet.ReductionQuota = 1M - Convert.ToDecimal(this.markBet.ReducedSize) / Convert.ToDecimal(this.SingleRows.Count);

            singleRow.Selected = true;
            singleRow.RowNumber = rowNumber;
            singleRow.CouponNumber = 0;
            this.rowNumber++;
        }

        private bool TestRules(HPTMarkBetSingleRow singleRow)
        {
            // Kör igenom alla regler
            foreach (HPTReductionRule rule in this.reductionRulesToApply)
            {
                if (!rule.IncludeRow(this.markBet, singleRow))
                {
                    return false;
                }
            }
            return true;
        }

        internal void HandleHighestAndLowestSums()
        {
            if (this.markBet.SystemSize == 0)
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

            foreach (var race in this.markBet.RaceDayInfo.RaceList)
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
            singleRow.EstimateRowValue(this.markBet);
            this.lowestEstimatedRowValue = singleRow.RowValue;
            this.highestStakePercentSum = singleRow.StakePercentSum;

            // Högsta radvärde och lägsta insatsfördelningsprocent
            singleRow = new HPTMarkBetSingleRow(highestRowValueHorseList.ToArray());
            singleRow.CalculateValues();
            singleRow.EstimateRowValue(this.markBet);
            this.highestEstimatedRowValue = singleRow.RowValue;
            this.lowestStakePercentSum = singleRow.StakePercentSum;

            // Lägsta ranksumma
            singleRow = new HPTMarkBetSingleRow(lowestRankSumHorseList.ToArray());
            singleRow.CalculateValues();
            //this.MinRankSum = singleRow.RankSum;
            this.MinRankSum = Math.Round(singleRow.RankSum - 0.05M, 1);

            // Högsta ranksumma
            singleRow = new HPTMarkBetSingleRow(highestRankSumHorseList.ToArray());
            singleRow.CalculateValues();
            this.MaxRankSum = Math.Round(singleRow.RankSum + 0.05M, 1);

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
            this.lowestStartNumberSum = singleRow.StartNrSum;

            // Högsta startnummersumma
            singleRow = new HPTMarkBetSingleRow(highestStartNumberSumHorseList.ToArray());
            singleRow.CalculateValues();
            this.highestStartNumberSum = singleRow.StartNrSum;

            // Lägsta oddssumma
            singleRow = new HPTMarkBetSingleRow(lowestOddsSumHorseList.ToArray());
            singleRow.CalculateValues();
            this.lowestOddsSum = singleRow.OddsSum;

            // Högsta oddssumma
            singleRow = new HPTMarkBetSingleRow(highestOddsSumHorseList.ToArray());
            singleRow.CalculateValues();
            this.highestOddsSum = singleRow.OddsSum;

            // Lägsta ATG-ranksumma
            singleRow = new HPTMarkBetSingleRow(lowestATGRankSumHorseList.ToArray());
            singleRow.CalculateValues();
            this.lowestATGRankSum = singleRow.ATGRankSum;

            // Högsta ATG-ranksumma
            singleRow = new HPTMarkBetSingleRow(highestATGRankSumHorseList.ToArray());
            singleRow.CalculateValues();
            this.highestATGRankSum = singleRow.ATGRankSum;

            // Lägsta egen ranksumma
            singleRow = new HPTMarkBetSingleRow(lowestOwnRankSumHorseList.ToArray());
            singleRow.CalculateValues();
            this.lowestOwnRankSum = singleRow.OwnRankSum;

            // Högsta egen ranksumma
            singleRow = new HPTMarkBetSingleRow(highestOwnRankSumHorseList.ToArray());
            singleRow.CalculateValues();
            this.highestOwnRankSum = singleRow.OwnRankSum;

            // Sätt värdena på rätt ställe
            this.markBet.RowValueReductionRule.LowestSum = this.lowestEstimatedRowValue;
            this.markBet.RowValueReductionRule.HighestSum = this.highestEstimatedRowValue;
            //this.markBet.PercentSumReductionRule.LowestSum = this.lowestPercentSum;
            //this.markBet.PercentSumReductionRule.HighestSum = this.highestPercentSum;
            this.markBet.StakePercentSumReductionRule.LowestSum = this.lowestStakePercentSum;
            this.markBet.StakePercentSumReductionRule.HighestSum = this.highestStakePercentSum;
            this.markBet.StartNrSumReductionRule.LowestSum = this.lowestStartNumberSum;
            this.markBet.StartNrSumReductionRule.HighestSum = this.highestStartNumberSum;
            this.markBet.OddsSumReductionRule.LowestSum = this.lowestOddsSum;
            this.markBet.OddsSumReductionRule.HighestSum = this.highestOddsSum;
            this.markBet.ATGRankSumReductionRule.LowestSum = this.lowestATGRankSum;
            this.markBet.ATGRankSumReductionRule.HighestSum = this.highestATGRankSum;
            this.markBet.OwnRankSumReductionRule.LowestSum = this.lowestOwnRankSum;
            this.markBet.OwnRankSumReductionRule.HighestSum = this.highestOwnRankSum;

            markBet.pauseRecalculation = recalculationPaused;
        }

        internal void HandleHighestAndLowestIncludedSums()
        {
            if (this.SingleRows == null || this.SingleRows.Count == 0)
            {
                if (this.markBet.RowValueReductionRule != null)
                {
                    this.markBet.RowValueReductionRule.LowestIncludedSum = this.lowestEstimatedRowValue;
                    this.markBet.RowValueReductionRule.HighestIncludedSum = this.highestEstimatedRowValue;
                }
                return;
            }

            this.markBet.RowValueReductionRule.LowestIncludedSum = this.SingleRows.Min(sr => sr.RowValue);
            this.markBet.RowValueReductionRule.HighestIncludedSum = this.SingleRows.Max(sr => sr.RowValue);
            //this.markBet.PercentSumReductionRule.LowestIncludedSum = this.SingleRows.Min(sr => sr.PercentSum);
            //this.markBet.PercentSumReductionRule.HighestIncludedSum = this.SingleRows.Max(sr => sr.PercentSum);
            this.markBet.StakePercentSumReductionRule.LowestIncludedSum = this.SingleRows.Min(sr => sr.StakePercentSum);
            this.markBet.StakePercentSumReductionRule.HighestIncludedSum = this.SingleRows.Max(sr => sr.StakePercentSum);
            this.markBet.StartNrSumReductionRule.LowestIncludedSum = this.SingleRows.Min(sr => sr.StartNrSum);
            this.markBet.StartNrSumReductionRule.HighestIncludedSum = this.SingleRows.Max(sr => sr.StartNrSum);
            this.markBet.OddsSumReductionRule.LowestIncludedSum = this.SingleRows.Min(sr => sr.OddsSum);
            this.markBet.OddsSumReductionRule.HighestIncludedSum = this.SingleRows.Max(sr => sr.OddsSum);
            this.markBet.OwnRankSumReductionRule.LowestIncludedSum = this.SingleRows.Min(sr => sr.OwnRankSum);
            this.markBet.OwnRankSumReductionRule.HighestIncludedSum = this.SingleRows.Max(sr => sr.OwnRankSum);
            this.markBet.ATGRankSumReductionRule.LowestIncludedSum = this.SingleRows.Min(sr => sr.ATGRankSum);
            this.markBet.ATGRankSumReductionRule.HighestIncludedSum = this.SingleRows.Max(sr => sr.ATGRankSum);
        }

        #endregion

        public void RecalculateRowValues()
        {
            if (this.SingleRows == null || this.SingleRows.Count() == 0)
            {
                return;
            }
            this.SingleRows
                .ForEach(sr =>
                {
                    sr.CalculateValues();
                    sr.EstimateRowValue(this.markBet);
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
            this.totalCouponSize = 0;
            if (this.SingleRows != null)
            {
                this.totalCouponSize = this.SingleRows.Sum(sr => sr.BetMultiplierList.Sum());
            }
            return this.totalCouponSize;
        }

        #region Properties

        private int currentCouponNumber;
        public int CurrentCouponNumber
        {
            get
            {
                return this.currentCouponNumber;
            }
            set
            {
                this.currentCouponNumber = value;
                OnPropertyChanged("CurrentCouponNumber");
            }
        }

        private int numberOfCoveredRows;
        public int NumberOfCoveredRows
        {
            get
            {
                return this.numberOfCoveredRows;
            }
            set
            {
                this.numberOfCoveredRows = value;
                OnPropertyChanged("NumberOfCoveredRows");
            }
        }

        private decimal coveredRowsShare;
        public decimal CoveredRowsShare
        {
            get
            {
                return this.coveredRowsShare;
            }
            set
            {
                this.coveredRowsShare = value;
                OnPropertyChanged("CoveredRowsShare");
            }
        }

        private int numberOfAnalyzedRows;
        public int NumberOfAnalyzedRows
        {
            get
            {
                return this.numberOfAnalyzedRows;
            }
            set
            {
                this.numberOfAnalyzedRows = value;
                OnPropertyChanged("NumberOfAnalyzedRows");
            }
        }

        private decimal analyzedRowsShare;
        public decimal AnalyzedRowsShare
        {
            get
            {
                return this.analyzedRowsShare;
            }
            set
            {
                this.analyzedRowsShare = value;
                OnPropertyChanged("AnalyzedRowsShare");
            }
        }

        #endregion

        #region Komprimera till kuponger

        public void ResetCouponNumber()
        {
            if (this.SingleRows != null)
            {
                this.SingleRows
                    .ToList()
                    .ForEach(sr => sr.CouponNumber = 0);
            }
        }

        public void ClearRowCombinations()
        {
            if (this.CompressedCoupons != null && this.CompressedCoupons.Count > 1)
            {
                this.CompressedCoupons.Clear();
            }
            this.CurrentCouponNumber = 0;
            this.NumberOfCoveredRows = 0;
            this.CoveredRowsShare = 0M;
        }

        public void CompressToCouponsV6SingleRows()
        {
            var rowsWithV6 = this.SingleRows
                .Where(r => r.V6)
                .SelectMany(sr => sr.GetUniqueList())
                .ToList();

            var rowsWithoutV6 = this.SingleRows
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
                rowsWithV6 = this.SingleRows
                .Where(r => r.V6)
                .SelectMany(sr => sr.GetDuplicateList(depth))
                .ToList();

                rowsWithoutV6 = this.SingleRows
                .Where(r => !r.V6)
                .SelectMany(sr => sr.GetDuplicateList(depth))
                .ToList();

                CompressToCouponsBetmultiplierSingleRows(rowsWithV6);
                CompressToCouponsBetmultiplierSingleRows(rowsWithoutV6);

                depth++;
            }

            int couponId = 1;
            foreach (var coupon in this.CompressedCoupons)
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

            foreach (int betMultiplier in this.markBet.BetType.BetMultiplierList)
            {
                var betMultiplierRowsToCompress = rowsToCompress
                    .Where(r => r.BetMultiplier == betMultiplier)
                    .ToDictionary(r => r.UniqueCode);
                CompressCouponsBruteForce(betMultiplierRowsToCompress, betMultiplier, v6);
            }
        }

        private void AddRowCombination(HPTMarkBetSingleRowCombination rowCombination, int betMultiplier, bool v6)
        {
            rowCombination.CouponNumber = this.CurrentCouponNumber;
            rowCombination.BetMultiplier = betMultiplier;
            rowCombination.V6 = v6;
            this.CompressedCoupons.Add(rowCombination);
            this.NumberOfCoveredRows += rowCombination.Size;
            this.CoveredRowsShare = Convert.ToDecimal(this.NumberOfCoveredRows) / Convert.ToDecimal(this.SingleRows.Count);
        }

        public void CompressToCouponsThreaded()
        {
            try
            {
                if (this.SingleRows == null
                || this.SingleRows.Count == 0
                || this.markBet.ReducedSize == 0)
                {
                    return;
                }
                // Skicka iväg komprimeringen i en ny tråd
                System.Threading.ThreadPool.QueueUserWorkItem(CompressToCouponsDelegate);
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
                this.StopCalculation = true;
                //if (this.CouponCompressorList != null && this.CouponCompressorList.Count > 0)
                //{
                //    foreach (HPTCouponCompressor compressor in this.CouponCompressorList)
                //    {
                //        compressor.StopCalculation = true;
                //    }
                //}

                int i = 0;
                while ((this.CalculationInProgress || this.CompressionInProgress)
                    && i < 50)
                {
                    if (dt < this.lastRequestedRecalculation)
                    {
                        return;
                    }
                    System.Threading.Thread.Sleep(100);
                    i++;
                }
                if (this.SingleRows.First().CouponNumber > 0)
                {
                    ResetCouponNumber();
                }
                this.StopCalculation = false;
                CompressToCoupons();
            }
            catch (Exception)
            {
                this.StopCalculation = false;
            }
            this.CompressionInProgress = false;
        }

        public void CompressToCoupons()
        {
            this.NumberOfCoveredRows = 0;

            if (this.SingleRows == null
                || this.SingleRows.Count == 0
                || this.SingleRows.First().CouponNumber > 0
                || this.markBet.ReducedSize == 0)
            {
                this.CompressionInProgress = false;
                return;
            }

            this.CompressionInProgress = true;
            if (this.CompressedCoupons == null)
            {
                this.CompressedCoupons = new List<HPTMarkBetSingleRowCombination>();
            }
            else
            {
                this.CompressedCoupons.Clear(); ;
            }
            this.CurrentCouponNumber = 0;

            lock (this)
            {
                // Sortera fallande efter raderna med högst täckning på systemet. Större chans att de går att slå ihop
                //this.SingleRows = this.SingleRowso.OrderByDescending(sr => sr.HorseList.Sum(h => h.SystemCoverage)).ToList();
                //var singleRows = this.SingleRowsObservable
                //    .OrderByDescending(sr => sr.HorseList.Sum(h => h.SystemCoverage))
                //    .ToList();

                // Om ingen reducering föreligger
                if (this.reductionRulesToApply.Count == 0 && !this.markBet.V6SingleRows && !this.markBet.SingleRowBetMultiplier && !this.markBet.ReductionV6BetMultiplierRule)
                {
                    this.NumberOfCoveredRows = this.markBet.SystemSize;
                    this.CurrentCouponNumber = 1;
                    this.CoveredRowsShare = 1M;
                }

                // Om man har valt V6 och/eller flerbong på enskilda enkelrader samt överskrider ATGs nya gränser
                else if (this.markBet.V6SingleRows
                    || this.markBet.SingleRowBetMultiplier
                    || this.markBet.ReductionV6BetMultiplierRule
                    || this.SingleRows.Any(sr => sr.Edited))
                {
                    CompressToCouponsV6SingleRows();
                }

                // Utfyllning med flerbong upp till ett viss belopp
                else if (this.markBet.BetMultiplierRowAddition && this.markBet.BetMultiplierRowAdditionTarget > 0)
                {
                    CompressToCouponsV6SingleRows();
                }

                // Ordinarie brute force-lösning...
                else
                {
                    CompressCouponsBruteForce();
                }
                this.CompressionInProgress = false;
                this.markBet.UpdateCoupons();
                this.StopCalculation = false;
                this.markBet.TotalCouponSize = RecalculateTotalCouponSize();
            }
        }

        private void CompressCouponsBruteForce()
        {
            var rowsToCompress = this.SingleRows
                .Where(sr => sr.CouponNumber == 0)
                .OrderByDescending(sr => sr.HorseList.Sum(h => h.SystemCoverage))
                .ToDictionary(sr => sr.UniqueCode);

            CompressCouponsBruteForce(rowsToCompress, this.markBet.BetMultiplier, this.markBet.V6);
        }

        private void CompressCouponsBruteForce(Dictionary<string, HPTMarkBetSingleRow> rowsToCompress, int betMultiplier, bool v6)
        {
            var dt = DateTime.Now;  // DEBUG

            while (rowsToCompress.Count > 0)
            {
                if (this.StopCalculation || dt < this.lastRequestedRecalculation)
                {
                    throw new Exception();
                }

                var startRow = rowsToCompress.Values.First();

                startRow.CouponNumber = this.CurrentCouponNumber + 1;
                var rowCombination = new HPTMarkBetSingleRowCombination(startRow);
                rowsToCompress.Remove(startRow.UniqueCode);

                var remainingHorsesList = this.markBet.RaceDayInfo.HorseListSelected
                    .Except(startRow.HorseList)
                    .OrderByDescending(h => h.SystemCoverage)
                    .ToList();

                foreach (HPTHorse horse in remainingHorsesList)
                {
                    rowCombination.AddHorseNew(horse);
                    bool includeHorse = rowCombination.CheckAddedRow(rowsToCompress, this.CurrentCouponNumber + 1);
                }
                this.CurrentCouponNumber++;

                AddRowCombination(rowCombination, betMultiplier, v6);
            }
            TimeSpan ts = DateTime.Now - dt;
            string s = ts.TotalSeconds.ToString();
        }

        #endregion

        public void SetRankSums(HPTMarkBetTemplateRank templateRank)
        {
            var singleRows = this.SingleRows
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
                this.numberOfRowsForTestedRule = 0;
                this.probabilityForTestedRule = 0M;

                // Skapa lista att loopa över
                this.raceSelectedHorseList = this.markBet.RaceDayInfo.RaceList
                    .Select(r => r.HorseListSelected)
                    .ToDictionary(h => h.First().ParentRace.LegNr);

                // Skapa dictionary för att kontrollera hur många vinstrader villkoret skulle gett
                if (this.markBet.RaceDayInfo.ResultComplete)
                {
                    this.reductionRuleToTest.GetRuleResultForCorrectRow(this.markBet);

                    this.numberOfCorrectDictionary = new Dictionary<int, int>();
                    this.markBet.RaceDayInfo.PayOutList
                        .OrderByDescending(po => po.NumberOfCorrect)
                        .ToList()
                        .ForEach(po =>
                        {
                            this.numberOfCorrectDictionary.Add(po.NumberOfCorrect, 0);
                        });
                }

                // Skapa alla enkelrader
                CalculateRuleStatistics(1);

                // Sätt värdena på regeln
                reductionRuleToTest.RemainingRowsPercentage = Convert.ToDecimal(this.numberOfRowsForTestedRule) / Convert.ToDecimal(this.markBet.SystemSize);
                reductionRuleToTest.Probability = this.probabilityForTestedRule;
                reductionRuleToTest.ProbabilityRelative = this.probabilityForTestedRule / this.markBet.SystemProbability / reductionRuleToTest.RemainingRowsPercentage;
                reductionRuleToTest.RemainingRows = this.numberOfRowsForTestedRule;
                reductionRuleToTest.SetReductionSpecificationString();

                // Sätt antal rader med 0, 1 och 2 fel
                if (this.numberOfCorrectDictionary != null)
                {
                    this.reductionRuleToTest.NumberOfAllCorrect = this.numberOfCorrectDictionary.Values.ElementAt(0);
                    if (this.numberOfCorrectDictionary.Count > 1)
                    {
                        this.reductionRuleToTest.NumberOfOneError = this.numberOfCorrectDictionary.Values.ElementAt(1);
                        if (this.numberOfCorrectDictionary.Count > 2)
                        {
                            this.reductionRuleToTest.NumberOfTwoErrors = this.numberOfCorrectDictionary.Values.ElementAt(2);
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
                if (raceNumber > this.markBet.NumberOfRaces)   // Sista loppet
                {
                    var singleRow = new HPTMarkBetSingleRow(this.horseList);
                    if (this.reductionRuleToTest.IncludeRow(this.markBet, singleRow))
                    {
                        if (!singleRow.ValuesCalculated)
                        {
                            singleRow.CalculateValues();
                        }
                        this.probabilityForTestedRule += singleRow.RowShareStake;
                        this.numberOfRowsForTestedRule++;
                        if (this.numberOfCorrectDictionary != null)
                        {
                            int numberOfCorrect = this.markBet.CouponCorrector.HorseList.Intersect(this.horseList).Count();
                            if (this.numberOfCorrectDictionary.ContainsKey(numberOfCorrect))
                            {
                                this.numberOfCorrectDictionary[numberOfCorrect] += 1;
                            }
                        }
                    }
                    return;
                }
                else if (raceNumber > 2)    // Andra loppet eller senare
                {
                    if (!this.reductionRuleToTest.IncludeRow(this.markBet, this.horseList, raceNumber - 1))
                    {
                        return;
                    }
                }
                foreach (var horse in this.raceSelectedHorseList[raceNumber])
                {
                    this.horseList[raceNumber - 1] = horse;
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
                return this.minRankSum;
            }
            set
            {
                this.minRankSum = value;
                OnPropertyChanged("MinRankSum");
            }
        }

        private decimal maxRankSum;
        public decimal MaxRankSum
        {
            get
            {
                return this.maxRankSum;
            }
            set
            {
                this.maxRankSum = value;
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
            if (this.markBet.ReductionRank && (this.markBet.MinRankSumPercent > 0 || this.markBet.MaxRankSumPercent < 100))
            {
                IOrderedEnumerable<HPTMarkBetSingleRow> orderedRows = this.AllRows.OrderBy(singleRow => singleRow.RankSum);
                int lowerPos = Convert.ToInt32(Convert.ToDecimal(this.markBet.MinRankSumPercent) / 100M * this.AllRows.Length);
                int upperPos = Convert.ToInt32(Convert.ToDecimal(this.markBet.MaxRankSumPercent) / 100M * this.AllRows.Length);
                RemoveSingleRows(lowerPos, upperPos, orderedRows);
            }
        }

        private void ApplyRowValuePercentageRule()
        {
            if (this.markBet.RowValueReductionRule.Use && (this.markBet.RowValueReductionRule.MinPercentSum > 0 || this.markBet.RowValueReductionRule.MaxPercentSum < 100))
            {
                IOrderedEnumerable<HPTMarkBetSingleRow> orderedRows = this.AllRows.OrderBy(singleRow => singleRow.RowValue);
                int lowerPos = Convert.ToInt32(Convert.ToDecimal(this.markBet.RowValueReductionRule.MinPercentSum) / 100M * this.AllRows.Length);
                int upperPos = Convert.ToInt32(Convert.ToDecimal(this.markBet.RowValueReductionRule.MaxPercentSum) / 100M * this.AllRows.Length);
                RemoveSingleRows(lowerPos, upperPos, orderedRows);
            }
        }

        private void ApplyStakePercentSumPercentageRule()
        {
            if (this.markBet.StakePercentSumReductionRule.Use && (this.markBet.StakePercentSumReductionRule.MinPercentSum > 0 || this.markBet.StakePercentSumReductionRule.MaxPercentSum < 100))
            {
                IOrderedEnumerable<HPTMarkBetSingleRow> orderedRows = this.AllRows.OrderBy(singleRow => singleRow.StakePercentSum);
                int lowerPos = Convert.ToInt32(Convert.ToDecimal(this.markBet.StakePercentSumReductionRule.MinPercentSum) / 100M * this.AllRows.Length);
                int upperPos = Convert.ToInt32(Convert.ToDecimal(this.markBet.StakePercentSumReductionRule.MaxPercentSum) / 100M * this.AllRows.Length);
                RemoveSingleRows(lowerPos, upperPos, orderedRows);
            }
        }

        private void ApplyStartNumberSumPercentageRule()
        {
            if (this.markBet.StartNrSumReductionRule.Use && (this.markBet.StartNrSumReductionRule.MinPercentSum > 0 || this.markBet.StartNrSumReductionRule.MaxPercentSum < 100))
            {
                IOrderedEnumerable<HPTMarkBetSingleRow> orderedRows = this.AllRows.OrderBy(singleRow => singleRow.StartNrSum);
                int lowerPos = Convert.ToInt32(Convert.ToDecimal(this.markBet.StartNrSumReductionRule.MinPercentSum) / 100M * this.AllRows.Length);
                int upperPos = Convert.ToInt32(Convert.ToDecimal(this.markBet.StartNrSumReductionRule.MaxPercentSum) / 100M * this.AllRows.Length);
                RemoveSingleRows(lowerPos, upperPos, orderedRows);
            }
        }

        private void RemoveSingleRows(int lowerPos, int upperPos, IOrderedEnumerable<HPTMarkBetSingleRow> orderedRows)
        {
            HPTMarkBetSingleRow[] orderedArray = orderedRows.ToArray();
            for (int i = 0; i < lowerPos; i++)
            {
                HPTMarkBetSingleRow row = this.SingleRows.FirstOrDefault(r => r.UniqueCode == orderedArray[i].UniqueCode);
                if (row != null)
                {
                    this.SingleRows.Remove(row);
                }
            }
            for (int i = upperPos; i < this.AllRows.Length; i++)
            {
                var row = this.SingleRows
                    .FirstOrDefault(r => r.UniqueCode == orderedArray[i].UniqueCode);
                if (row != null)
                {
                    this.SingleRows.Remove(row);
                }
            }
        }

        public string ToSingleRowsString()
        {
            if (this.SingleRows == null)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Enkelrader:");
            foreach (HPTMarkBetSingleRow singleRow in this.SingleRows.OrderBy(sr => sr.RowNumber))
            {
                sb.AppendLine(singleRow.ToString());
            }
            return sb.ToString();
        }

        public string ToCouponsString()
        {
            StringBuilder sb = new StringBuilder();
            // TODO: Använd HPTCoupon istället och ta med reserverna
            foreach (HPTMarkBetSingleRowCombination rowCombination in this.CompressedCoupons)
            {
                sb.AppendLine(rowCombination.ToCouponString());
            }

            return sb.ToString();
        }

        #endregion

        #region Plocka bort utifrån radskillnad

        internal void RemoveWithRowDifference()
        {
            if (this.markBet.GuaranteeReduction && this.markBet.NumberOfToleratedErrors > 0)
            {
                // Håll koll på när beräkningen börjar
                var dt = DateTime.Now;

                //this.smallestRowDifferenceToSelect = this.markBet.NumberOfToleratedErrors + 1;

                // Hela uppsättninge rader
                var selectedRows = this.SingleRows
                    .Where(sr => sr.Selected)
                    .OrderBy(sr => sr.UniqueCode)
                    .ToDictionary(sr => sr.UniqueCode);

                this.rowsToSelectFrom = new Dictionary<string, HPTMarkBetSingleRow>(selectedRows);

                // Temporär lista för de rader som täcker upp övriga
                var rowsToKeep = new List<HPTMarkBetSingleRow>();

                // Nollställ reducerad storlek så det syns i gränssnittet att bearbetningen fortgår
                this.markBet.ReducedSize = 0;
                this.singleRows.Clear();

                // Välj första raden för att dra igång det hela
                var singleRow = rowsToSelectFrom.Values.First();

                while (selectedRows.Count > 0)  // Så länge inte alla rader är täckta
                {
                    if (StopCalculation || dt < this.lastRequestedRecalculation)
                    {
                        return;
                    }

                    selectedRows.Remove(singleRow.UniqueCode);
                    if (this.rowsToSelectFrom.ContainsKey(singleRow.UniqueCode))
                    {
                        this.rowsToSelectFrom.Remove(singleRow.UniqueCode);
                    }


                    // Ta bort de täckta raderna
                    RemoveCodesWithDifference(singleRow, selectedRows, this.markBet.NumberOfToleratedErrors);

                    singleRow.Selected = true;
                    rowsToKeep.Add(singleRow);
                    this.singleRows.Add(singleRow);
                    this.markBet.ReducedSize++;

                    if (this.rowsToSelectFrom.Count > 0)
                    {
                        singleRow = this.rowsToSelectFrom.Values.First();
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

            this.markBet.RaceDayInfo.HorseListSelected
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
                        if (this.rowsToSelectFrom.ContainsKey(uniqueCode))
                        {
                            this.rowsToSelectFrom.Remove(uniqueCode);
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
            if (this.markBet.GuaranteeReduction && this.markBet.NumberOfToleratedErrors > 0)
            {
                // Håll koll på när beräkningen börjar
                var dt = DateTime.Now;

                // Hela uppsättninge rader
                var selectedRowsDictionary = this.SingleRows
                    .Where(sr => sr.Selected)
                    .OrderBy(sr => sr.UniqueCode)
                    .ToDictionary(sr => sr.UniqueCode);

                this.rowsToSelectFrom = new Dictionary<string, HPTMarkBetSingleRow>(selectedRowsDictionary);

                // Temporär lista för de rader som täcker upp övriga
                var rowsToKeep = new List<HPTMarkBetSingleRow>();

                // Nollställ reducerad storlek så det syns i gränssnittet att bearbetningen fortgår
                this.markBet.ReducedSize = 0;
                this.singleRows.Clear();

                var rowsToSelectFromTemp = selectedRowsDictionary.Values.ToList();

                //// Välj första raden som dummy
                //var singleRow = rowsToSelectFrom.Values
                //    .OrderByDescending(sr => sr.HorseList.Sum(h => h.NumberOfCoveredRows))
                //    .First();

                int maxCoveredRows = this.markBet.SystemSize - 1;
                int maxCoveredRows2 = this.markBet.SystemSize - 1;
                HPTMarkBetSingleRow singleRow = null;

                while (selectedRowsDictionary.Count > 0)  // Så länge inte alla rader är täckta
                {
                    if (StopCalculation || dt < this.lastRequestedRecalculation)
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
                        if (this.markBet.NumberOfToleratedErrors < 3)
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
                    if (this.rowsToSelectFrom.ContainsKey(singleRow.UniqueCode))
                    {
                        this.rowsToSelectFrom.Remove(singleRow.UniqueCode);
                    }

                    // Ta bort de täckta raderna
                    RemoveCodesWithDifference(singleRow, selectedRowsDictionary, this.markBet.NumberOfToleratedErrors);

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
                    this.singleRows.Add(singleRow);
                    this.markBet.ReducedSize++;
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

            this.markBet.RaceDayInfo.HorseListSelected
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
            decimal systemCost = this.totalCouponSize * this.markBet.BetType.RowCost;
            if (this.markBet.BetMultiplierRowAddition && this.markBet.BetMultiplierRowAdditionTarget > systemCost)
            {
                int numberOfRowsToDouble = Convert.ToInt32((this.markBet.BetMultiplierRowAdditionTarget - systemCost) /
                                          this.markBet.BetType.RowCost);

                var rowsToDouble = this.singleRows.Where(sr => sr.BetMultiplier == 1).OrderBy(sr => sr.RowValue).Take(numberOfRowsToDouble);
                foreach (var singleRow in rowsToDouble)
                {
                    if (StopCalculation)
                    {
                        return;
                    }
                    singleRow.BetMultiplier = 2;
                    singleRow.BetMultiplierList = new List<int>() { 2 };
                    this.totalCouponSize++;
                }
            }
        }

        internal void RemoveOwnProbabilityRowsToReachTarget()
        {
            decimal systemCost = this.totalCouponSize * this.markBet.BetType.RowCost;

            if (this.markBet.OwnProbabilityCost && this.markBet.OwnProbabilityCostTarget < systemCost)
            {
                int numberOfRowsToRemove = Convert.ToInt32((systemCost - this.markBet.OwnProbabilityCostTarget) /
                                          this.markBet.BetType.RowCost);

                var singleRowsArray = this.singleRows
                    .OrderBy(sr => sr.OwnProbabilityQuota)
                    .Take(numberOfRowsToRemove)
                    .ToArray();

                foreach (var singleRowToRemove in singleRowsArray)
                {
                    this.singleRows.Remove(singleRowToRemove);
                }
                ResetNumberOfCoveredRows();
            }
        }

        internal void RemoveRandomRowsToReachTarget()
        {
            decimal systemCost = this.totalCouponSize * this.markBet.BetType.RowCost;

            if (this.markBet.RandomRowReduction && this.markBet.RandomRowReductionTarget < systemCost)
            {
                int numberOfRowsToRemove = Convert.ToInt32((systemCost - this.markBet.RandomRowReductionTarget) /
                                          this.markBet.BetType.RowCost);
                int couponSizeToReach = this.totalCouponSize - numberOfRowsToRemove;
                int upperLimit = this.singleRows.Count - 1;
                var singleRowsArray = this.singleRows.ToArray();
                var rnd = new Random();
                for (int i = 0; i < numberOfRowsToRemove; i++)
                {
                    if (StopCalculation)
                    {
                        return;
                    }
                    int position = rnd.Next(upperLimit);
                    var singleRowToRemove = singleRowsArray[position];
                    this.SingleRows.Remove(singleRowToRemove);
                    singleRowsArray[position] = singleRowsArray[upperLimit];
                    this.totalCouponSize -= singleRowToRemove.BetMultiplier;
                    if (this.totalCouponSize <= couponSizeToReach)
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
            foreach (var horse in this.markBet.RaceDayInfo.HorseListSelected)
            {
                horse.NumberOfCoveredRows = 0;
            }
            foreach (var singleRow in this.SingleRows)
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
