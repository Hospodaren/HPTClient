using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTMarkBet : HPTBet
    {
        public HPTMarkBet()
            : base()
        {
            this.IsDeserializing = true;

            // Klass för rättning av kuponger
            this.CouponCorrector = new HPTCouponCorrector();
            this.CouponCorrector.CouponHelper = new ATGCouponHelper(this);

            // Lista med resultat av mallberäkning
            this.TemplateResultList = new ObservableCollection<HPTMarkBetTemplateResult>();

            // Klass för mailskickning
            this.MailSender = new HPTMailSender();

            this.ReductionRuleInfoList = new ObservableCollection<ReductionRuleInfo>();
            this.ReductionRuleStatisticsList = new ObservableCollection<HPTReductionRule>();
            this.SingleRowsObservable = new ObservableCollection<HPTMarkBetSingleRow>();

            CreateParentRaceDayInfo();
        }

        public HPTMarkBet(HPTRaceDayInfo rdi, HPTBetType bt)
            : base(rdi, bt)
        {
            this.BetType = bt;
            this.NumberOfRaces = this.RaceDayInfo.RaceList.Count;

            SetGeneralValues();
            InitializeReductionRules();

            // Skapa listor med kuskar och tränare för reduceringsregler och hästar
            HPTServiceToHPTHelper.SetTrainerAndDriver(this);

            //SetEventHandlers();

            switch (this.BetType.Code)
            {
                case "V64":
                case "V65":
                case "GS75":
                    //case "V75":
                    //case "V86":
                    this.V6Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    this.V6Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }

            // Lista med beräknade templates
            this.TemplateResultList = new ObservableCollection<HPTMarkBetTemplateResult>();

            // Hantering av rankvariabler
            this.HorseVariableList = new ObservableCollection<HPTHorseVariable>(HPTHorseVariable.CreateVariableList());

            // Klassen som hanterar mailskickning av bolagssystem
            this.MailSender = new HPTMailSender();

            // Kör bearbetning när något i konfiguration ändras
            HPTConfig.Config.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Config_PropertyChanged);

            // Ytterligare initiering
            this.ReductionRuleInfoList = new ObservableCollection<ReductionRuleInfo>();
            CreateRankVariableList();
            ApplyConfigRankVariables();
            RecalculateAllRanks();
            this.BetMultiplier = 1;

            // IHorseListContainer
            //this.HorseList = new ObservableCollection<HPTHorse>(this.RaceDayInfo.RaceList.SelectMany(r => r.HorseList));
            this.HorseList = new List<HPTHorse>(this.RaceDayInfo.RaceList.SelectMany(r => r.HorseList));
            //this.ParentRaceDayInfo = this.RaceDayInfo;
            CreateParentRaceDayInfo();

            // Om hästar valts automatiskt i laddningen
            var selectedHorses = this.RaceDayInfo.RaceList.SelectMany(r => r.HorseList.Where(h => h.Selected));
            if (this.RaceDayInfo.HorseListSelected == null)
            {
                this.RaceDayInfo.HorseListSelected = new ObservableCollection<HPTHorse>(selectedHorses);
            }
            else
            {
                this.RaceDayInfo.HorseListSelected.Clear();
                foreach (var horse in selectedHorses)
                {
                    this.RaceDayInfo.HorseListSelected.Add(horse);
                }
            }
            this.ReductionRuleStatisticsList = new ObservableCollection<HPTReductionRule>();
            this.SingleRowsObservable = new ObservableCollection<HPTMarkBetSingleRow>();
        }

        private void CreateParentRaceDayInfo()
        {
            // Fastställd konfiguration för Poäng-fliken
            this.ParentRaceDayInfo = new HPTRaceDayInfo()
            {
                DataToShow = new HPTHorseDataToShow()
                {
                    ShowRankAlternate = true,
                    ShowRankOwn = true,
                    ShowPrio = true,
                    ShowStakeDistributionPercent = true,
                    ShowVinnarOdds = true,
                    ShowName = true,
                    ShowStartNr = true,
                    ShowOwnProbability = true,
                    ColumnsInOrder = new List<string>()
                        {
                            "ShowStartNr",
                            "ShowName",
                            "ShowStakeDistributionPercent",
                            "ShowOwnProbability",
                            "ShowVinnarOdds",
                            "ShowPrio",
                            "ShowRankOwn",
                            "ShowRankAlternate"
                        }
                }
            };
        }

        [OnDeserializing]
        public void InitializeOnDeserialization(StreamingContext sc)
        {
            this.IsDeserializing = true;

            // Klass för rättning av kuponger
            this.CouponCorrector = new HPTCouponCorrector();
            this.CouponCorrector.CouponHelper = new ATGCouponHelper(this);

            // Lista med resulta av mallberäkning
            this.TemplateResultList = new ObservableCollection<HPTMarkBetTemplateResult>();

            // Klass för mailskickning
            this.MailSender = new HPTMailSender();

            this.ReductionRuleInfoList = new ObservableCollection<ReductionRuleInfo>();
        }

        [OnDeserialized]
        public void InitializeOnDeserialized(StreamingContext sc)
        {
            // Regel för egen chansvärdering
            if (this.OwnProbabilityReductionRule == null)
            {
                this.OwnProbabilityReductionRule = new HPTOwnProbabilityReductionRule();
            }

            if (this.AlternateRankSumReductionRule == null)
            {
                this.AlternateRankSumReductionRule = new HPTAlternateRankSumReductionRule()
                {
                    MaxSum = this.NumberOfRaces * 15,
                    MaxPercentSum = 100,
                    LowestSum = this.NumberOfRaces,
                    HighestSum = this.NumberOfRaces * 100,
                    IncrementLower = 1,
                    IncrementUpper = 1
                };
            }

            InitializeReductionRulesEventHandlers();

            // Lista med de rankregler som bygger på egen rank
            this.HorseOwnRankSumReductionRuleList = new ObservableCollection<HPTHorseRankSumReductionRule>();
            var horseRankSumReductionRuleABC = GetHorseRankSumReductionRule("RankABC");
            if (horseRankSumReductionRuleABC != null)
            {
                this.HorseOwnRankSumReductionRuleList.Add(horseRankSumReductionRuleABC);
            }
            var horseRankSumReductionRuleOwn = GetHorseRankSumReductionRule("RankOwn");
            if (horseRankSumReductionRuleOwn != null)
            {
                this.HorseOwnRankSumReductionRuleList.Add(horseRankSumReductionRuleOwn);
            }
            var horseRankSumReductionRuleAlternate = GetHorseRankSumReductionRule("RankAlternate");
            if (horseRankSumReductionRuleAlternate != null)
            {
                this.HorseOwnRankSumReductionRuleList.Add(horseRankSumReductionRuleAlternate);
            }

            this.ReductionRuleStatisticsList = new ObservableCollection<HPTReductionRule>();

            this.IsDeserializing = true;
        }

        internal void InitializeReductionRules()
        {
            // Regler som alltid ska vara definierade
            this.V6BetMultiplierRuleList = new ObservableCollection<HPTV6BetMultiplierRule>();
            this.GroupIntervalRulesCollection = new HPTGroupIntervalRulesCollection(0, false);
            this.ComplementaryRulesCollection = new HPTComplementaryRulesCollection(0, false);
            this.ABCDEFReductionRule = new HPTABCDEFReductionRule(this);
            this.MultiABCDEFReductionRule = new HPTMultiABCDEFReductionRule()
            {
                ABCDEFReductionRuleList = new ObservableCollection<HPTABCDEFReductionRule>() { this.ABCDEFReductionRule }
            };
            this.TrainerRulesCollection = new HPTPersonRulesCollection(this.NumberOfRaces, false, PersonReductionType.Trainer);
            this.DriverRulesCollection = new HPTPersonRulesCollection(this.NumberOfRaces, false, PersonReductionType.Driver);

            // Regel för snittrank
            this.RankReductionRule = new HPTRankReductionRule();

            // Regel för egen chansvärdering
            this.OwnProbabilityReductionRule = new HPTOwnProbabilityReductionRule();

            // Lista med individuella rankregler
            var horseRankSumReductionRuleList = this.HorseRankVariablesToShowList
                .Select(hrv => new HPTHorseRankSumReductionRule(hrv, this.NumberOfRaces));
            this.HorseRankSumReductionRuleList = new ObservableCollection<HPTHorseRankSumReductionRule>(horseRankSumReductionRuleList);

            // Lista med de rankregler som bygger på egen rank
            this.HorseOwnRankSumReductionRuleList = new ObservableCollection<HPTHorseRankSumReductionRule>();
            var horseRankSumReductionRuleABC = GetHorseRankSumReductionRule("RankABC");
            if (horseRankSumReductionRuleABC != null)
            {
                this.HorseOwnRankSumReductionRuleList.Add(horseRankSumReductionRuleABC);
            }
            var horseRankSumReductionRuleOwn = GetHorseRankSumReductionRule("RankOwn");
            if (horseRankSumReductionRuleOwn != null)
            {
                this.HorseOwnRankSumReductionRuleList.Add(horseRankSumReductionRuleOwn);
            }
            var horseRankSumReductionRuleAlternate = GetHorseRankSumReductionRule("RankAlternate");
            if (horseRankSumReductionRuleAlternate != null)
            {
                this.HorseOwnRankSumReductionRuleList.Add(horseRankSumReductionRuleAlternate);
            }

            #region Intervalreduction rules

            //this.PercentSumReductionRule = new HPTPercentSumReductionRule()
            //{
            //    MaxSum = this.NumberOfRaces * 100,
            //    MaxPercentSum = 100,
            //    HighestSum = this.NumberOfRaces * 100,
            //    IncrementLower = 10,
            //    IncrementUpper = 10
            //};

            this.RowValueReductionRule = new HPTRowValueReductionRule()
            {
                MaxSum = 100000000,
                //MaxSum = this.RaceDayInfo.MaxPayOut,
                MaxPercentSum = 100,
                HighestSum = 100000000, // 100 miljoner
                //HighestSum = this.RaceDayInfo.MaxPayOut,
                IncrementLower = 100,
                IncrementUpper = 1000
            };

            this.StakePercentSumReductionRule = new HPTStakePercentSumReductionRule()
            {
                MaxSum = this.NumberOfRaces * 100,
                MaxPercentSum = 100,
                HighestSum = this.NumberOfRaces * 100,
                IncrementLower = 10,
                IncrementUpper = 10
            };

            this.StartNrSumReductionRule = new HPTStartNrSumReductionRule()
            {
                MaxSum = this.NumberOfRaces * 15,
                MaxPercentSum = 100,
                LowestSum = this.NumberOfRaces,
                HighestSum = this.NumberOfRaces * 15,
                IncrementLower = 1,
                IncrementUpper = 1
            };

            this.ATGRankSumReductionRule = new HPTATGRankSumReductionRule()
            {
                MaxSum = this.NumberOfRaces * 15,
                MaxPercentSum = 100,
                LowestSum = this.NumberOfRaces,
                HighestSum = this.NumberOfRaces * 15,
                IncrementLower = 1,
                IncrementUpper = 1
            };

            this.OwnRankSumReductionRule = new HPTOwnRankSumReductionRule()
            {
                MaxSum = this.NumberOfRaces * 100,
                MaxPercentSum = 100,
                LowestSum = this.NumberOfRaces,
                HighestSum = this.NumberOfRaces * 100,
                IncrementLower = 1,
                IncrementUpper = 1
            };

            this.AlternateRankSumReductionRule = new HPTAlternateRankSumReductionRule()
            {
                MaxSum = this.NumberOfRaces * 100,
                MaxPercentSum = 100,
                LowestSum = this.NumberOfRaces,
                HighestSum = this.NumberOfRaces * 100,
                IncrementLower = 1,
                IncrementUpper = 1
            };

            this.OddsSumReductionRule = new HPTOddsSumReductionRule()
            {
                MaxSum = this.NumberOfRaces * 1000,
                MaxPercentSum = 100,
                LowestSum = this.NumberOfRaces * 10,
                HighestSum = this.NumberOfRaces * 1000,
                IncrementLower = 1,
                IncrementUpper = 10
            };

            this.IntervalReductionRuleList = new ObservableCollection<HPTIntervalReductionRule>()
                {
                    this.StakePercentSumReductionRule,
                    //this.PercentSumReductionRule,
                    this.StartNrSumReductionRule,
                    this.ATGRankSumReductionRule,
                    this.OwnRankSumReductionRule,
                    this.AlternateRankSumReductionRule,
                    this.OddsSumReductionRule,
                    this.RowValueReductionRule
                };

            #endregion

            InitializeReductionRulesEventHandlers();
        }

        private HPTHorseRankSumReductionRule GetHorseRankSumReductionRule(string propertyName)
        {
            var horseRankSumReductionRule = this.HorseRankSumReductionRuleList
                .FirstOrDefault(hrsr => hrsr.PropertyName == propertyName);

            if (horseRankSumReductionRule == null)
            {
                var horseRankVariable = this.HorseRankVariableList
                    .FirstOrDefault(hrv => hrv.PropertyName == propertyName);

                if (horseRankVariable != null)
                {
                    horseRankSumReductionRule = new HPTHorseRankSumReductionRule(horseRankVariable, this.NumberOfRaces);
                }
            }
            return horseRankSumReductionRule;
        }

        private void InitializeReductionRulesEventHandlers()
        {
            this.MultiABCDEFReductionRule.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReductionRule_PropertyChanged);
            this.ABCDEFReductionRule.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReductionRule_PropertyChanged);
            this.DriverRulesCollection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReductionRule_PropertyChanged);
            this.TrainerRulesCollection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReductionRule_PropertyChanged);
            this.ComplementaryRulesCollection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReductionRule_PropertyChanged);
            this.OwnProbabilityReductionRule.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReductionRule_PropertyChanged);

            if (this.IntervalReductionRuleList == null)
            {
                // Create IntervalReductionRuleList
                this.IntervalReductionRuleList = new ObservableCollection<HPTIntervalReductionRule>()
                {
                    //this.PercentSumReductionRule,
                    this.RowValueReductionRule,
                    this.StakePercentSumReductionRule,
                    this.StartNrSumReductionRule,
                    this.ATGRankSumReductionRule,
                    this.OwnRankSumReductionRule,
                    this.AlternateRankSumReductionRule,
                    this.OddsSumReductionRule
                };
            }

            foreach (var intervalReductionRule in this.IntervalReductionRuleList)
            {
                intervalReductionRule.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReductionRule_PropertyChanged);
            }
            this.GroupIntervalRulesCollection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReductionRule_PropertyChanged);

            this.RaceDayInfo.RankTemplateChanged -= ApplyConfigRankVariables;
            this.RaceDayInfo.RankTemplateChanged += ApplyConfigRankVariables;
        }

        void ReductionRule_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Use" || e.PropertyName == "ReductionHorseRank" || e.PropertyName == "ReductionRank")
            {
                RecalculateReduction(RecalculateReason.All);
            }
        }

        internal bool HasDynamicReductionRule()
        {
            // Fler varianter som ska inkluderas
            if (this.RowValueReductionRule.Use
                || this.StakePercentSumReductionRule.Use
                //|| this.PercentSumReductionRule.Use 
                || this.OddsSumReductionRule.Use
                || this.GroupIntervalRulesCollection.Use
                || this.ReductionRank)
            {
                return true;
            }
            return false;
        }

        internal bool HasSingleRowRule()
        {
            // Målvinst
            if (this.SingleRowBetMultiplier && this.SingleRowTargetProfit > 0)
                return true;

            // Garantireducering
            if (this.GuaranteeReduction && this.NumberOfToleratedErrors > 0)
                return true;

            // Slumpmässig borttagning
            if (this.RandomRowReduction && this.RandomRowReductionTarget > 0)
                return true;

            // Lägg till rader för att uppnå insats
            if (this.BetMultiplierRowAddition && this.BetMultiplierRowAdditionTarget > 0)
                return true;

            // Lägg till rader för att uppnå insats
            if (this.OwnProbabilityCost && this.OwnProbabilityCostTarget > 0)
                return true;

            // V6 radvärde
            if (this.V6SingleRows && this.V6UpperBoundary > 0)
                return true;

            // V6 egen rank
            if (this.V6OwnRank && this.V6OwnRankMax > 0)
                return true;

            return false;
        }

        internal bool HasOwnRankReduction()
        {
            // Intervallregel för egen rank används
            if (this.OwnRankSumReductionRule.Use)
            {
                return true;
            }

            // Egen rank används i snittrank och rankintervall används
            if (this.HorseRankVariableList != null && this.HorseRankVariableList.Count > 0)
            {
                var ownRankVariable = this.HorseRankVariableList.FirstOrDefault(hr => hr.PropertyName == "RankOwn");
                if (ownRankVariable != null && ownRankVariable.Use && this.ReductionRank)
                {
                    return true;
                }
            }

            // Intervall- och/eller gruppintervallregel med egen rank används
            if (this.HorseRankSumReductionRuleList != null && this.HorseRankSumReductionRuleList.Count > 0)
            {
                var horseRankSumReductionRule = this.HorseRankSumReductionRuleList.FirstOrDefault(hrs => hrs.PropertyName == "RankOwn");
                if (horseRankSumReductionRule != null && horseRankSumReductionRule.Use)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool HasAlternateRankReduction()
        {
            // Intervallregel för alternativ rank används
            if (this.AlternateRankSumReductionRule.Use)
            {
                return true;
            }

            // Alternativ rank används i snittrank och rankintervall används
            if (this.HorseRankVariableList != null && this.HorseRankVariableList.Count > 0)
            {
                var alternateRankVariable = this.HorseRankVariableList.FirstOrDefault(hr => hr.PropertyName == "RankAlternate");
                if (alternateRankVariable != null && alternateRankVariable.Use && this.ReductionRank)
                {
                    return true;
                }
            }

            // Intervall- och/eller gruppintervallregel med egen rank används
            if (this.HorseRankSumReductionRuleList != null && this.HorseRankSumReductionRuleList.Count > 0)
            {
                var horseRankSumReductionRule = this.HorseRankSumReductionRuleList.FirstOrDefault(hrs => hrs.PropertyName == "RankAlternate");
                if (horseRankSumReductionRule != null && horseRankSumReductionRule.Use)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool HasRankReduction()
        {
            // Intervallregel för egen rank används
            if (this.OwnRankSumReductionRule.Use || this.ReductionRank || this.ReductionHorseRank)
            {
                return true;
            }

            // Intervall- och/eller gruppintervallregel med egen rank används
            if (this.ReductionHorseRank && this.HorseRankSumReductionRuleList != null && this.HorseRankSumReductionRuleList.Count > 0)
            {
                int numberOfReductionRulesUsed = this.HorseRankSumReductionRuleList.Count(hrs => hrs.Use);
                if (numberOfReductionRulesUsed > 0)
                {
                    return true;
                }
            }
            return false;
        }

        internal void ApplyConfigRankVariables()
        {
            if (HPTConfig.Config.UseDefaultRankTemplate)
            {
                ApplyConfigRankVariables(HPTConfig.Config.DefaultRankTemplate);
            }
        }

        internal override void ApplyConfigRankVariables(HPTRankTemplate rankTemplate)
        {
            bool recalculationPaused = this.pauseRecalculation;
            this.pauseRecalculation = true;

            base.ApplyConfigRankVariables(rankTemplate);

            this.pauseRecalculation = recalculationPaused;

            if (HasRankReduction())
            {
                RecalculateReduction(RecalculateReason.Rank);
            }
            else
            {
                RecalculateAllRanks();
                RecalculateRank();
            }
        }

        public ObservableCollection<HPTMarkBetSingleRow> SingleRowsObservable { get; set; }
        internal void SingleRowCollection_AnalyzingFinished()
        {
            if (this.IsCalculatingTemplates)
            {
                return;
            }
            // Beräkna varje hästs täckning
            if (this.ReducedSize > 0
                && !this.SingleRowCollection.CalculationInProgress)
            {
                var AllHorses = this.RaceDayInfo.RaceList
                    .SelectMany(r => r.HorseList)
                    .ToList();

                foreach (HPTHorse horse in AllHorses)
                {
                    horse.SystemCoverage = Convert.ToDecimal(horse.NumberOfCoveredRows) / Convert.ToDecimal(this.ReducedSize);
                }
                OnPropertyChanged("SingleRowCollection");
            }

            // Skapa inte kuponger om man valt att låsa kupongerna
            if (this.LockCoupons)
            {
                return;
            }

            // Ingen komprimering vald eller komprimering som pågår
            if (!this.CompressCoupons || this.SingleRowCollection.CalculationInProgress)
            {
                return;
            }

            //// Fyll på med enkelraderna
            //this.SingleRowsObservable.Clear();
            //this.SingleRowCollection.SingleRows.ForEach(sr => this.SingleRowsObservable.Add(sr));

            // Komprimera till kuponger
            this.SingleRowCollection.CompressToCouponsThreaded();

            try
            {
                this.RaceDayInfo.NumberOfFinishedRaces = 0;
                this.RaceDayInfo.ResultComplete = false;
                this.RaceDayInfo.PayOutList?.Clear();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        public void CheckForWarnings()
        {
            // Varna vid hästar som inte kommer med på systemet?
            HandleUncoveredHorses();

            // Varna om det finns överflödiga ABCD-villkor på systemet
            HandleSuperfluousXReduction();

            // Varna när hästar är med på flera utgångar
            HandleOverlappingComplementaryRules();

            // Varna om man inte aktivt valt reserver?
            if (HPTConfig.Config.WarnIfNoReserv)
            {
                if (this.ReservHandling == HPTClient.ReservHandling.None)
                {
                    this.NoReservChoiceMade = true;
                }
                else if (this.ReservHandling == HPTClient.ReservHandling.Own)
                {
                    int numberOfReserves = this.RaceDayInfo.RaceList.SelectMany(r => r.HorseList).Where(h => h.Reserv1 == true || h.Reserv2 == true).Count();
                    this.NoReservChoiceMade = numberOfReserves == 0;
                }
            }
            else
            {
                this.NoReservChoiceMade = false;
            }
        }

        #region Create dummy markbet

        //public static HPTMarkBet CreateDummyMarkBet(HPTBetType bt)
        //{
        //    Random rnd = new Random();
        //    HPTRaceDayInfo rdi = new HPTRaceDayInfo()
        //    {
        //        TypeCategory = bt,
        //        Trackcode = "NN",
        //        TrackId = 0,
        //        Trackname = "Okänd",
        //        RaceDayDate = DateTime.Now,
        //        DriverList = new HPTPersonCollection(),
        //        TrainerList = new HPTPersonCollection(),
        //        RaceList = Enumerable.Range(1, bt.NumberOfRaces).Select(raceNumber => new HPTRace()
        //        {
        //            RaceNr = raceNumber,
        //            LegNr = raceNumber,

        //            HorseList = new ObservableCollection<HPTHorse>(Enumerable.Range(1, 12).Select(startNr => new HPTHorse()
        //            {
        //                StartNr = startNr,
        //                HorseName = "Avd" + raceNumber.ToString() + " Häst" + startNr.ToString(),
        //                VinnarOdds = rnd.Next(10, 200),
        //                MarksPercent = rnd.Next(1, 70),
        //                StakeDistributionPercent = rnd.Next(1, 60),
        //                Scratched = false,
        //                NotScratched = true,
        //                DriverName = "Kusk " + startNr.ToString(),
        //                DriverNameShort = "K " + startNr.ToString(),
        //                TrainerName = "Tränare " + startNr.ToString(),
        //                TrainerNameShort = "T " + startNr.ToString(),
        //                CurrentYearStatistics = HPTHorseYearStatistics.CreateDummyStatistics(DateTime.Now.Year.ToString()),
        //                PreviousYearStatistics = HPTHorseYearStatistics.CreateDummyStatistics(DateTime.Now.AddYears(-1).Year.ToString()),
        //                TotalStatistics = HPTHorseYearStatistics.CreateDummyStatistics("Totalt")
        //            }))
        //        }).ToList()
        //    };

        //    IEnumerable<HPTHorse> completeHorseList = rdi.RaceList.SelectMany(r => r.HorseList);

        //    // Kuskar
        //    rdi.DriverList = new HPTPersonCollection(rdi);
        //    IEnumerable<HPTPerson> driverList = Enumerable.Range(1, 12).Select(nr => new HPTPerson()
        //    {
        //        Name = "Kusk " + nr.ToString(),
        //        ShortName = "K " + nr.ToString(),
        //        ParentRaceDayInfo = rdi                
        //    });
        //    rdi.DriverList.PersonList = new ObservableCollection<HPTPerson>(driverList);

        //    foreach (HPTPerson driver in rdi.DriverList.PersonList)
        //    {
        //        driver.HorseList = new ObservableCollection<HPTHorse>(
        //            completeHorseList.Where(h => h.DriverNameShort == driver.ShortName));
        //    }

        //    // Tränare
        //    rdi.TrainerList = new HPTPersonCollection(rdi);
        //    IEnumerable<HPTPerson> trainerList = Enumerable.Range(1, 12).Select(nr => new HPTPerson()
        //    {
        //        Name = "Tränare " + nr.ToString(),
        //        ShortName = "T " + nr.ToString(),
        //        ParentRaceDayInfo = rdi
        //    });
        //    rdi.TrainerList.PersonList = new ObservableCollection<HPTPerson>(trainerList);

        //    foreach (HPTPerson trainer in rdi.TrainerList.PersonList)
        //    {
        //        trainer.HorseList = new ObservableCollection<HPTHorse>(
        //            completeHorseList.Where(h => h.TrainerNameShort == trainer.ShortName));
        //    }

        //    // Komplettera hästinformation
        //    foreach (HPTRace race in rdi.RaceList)
        //    {
        //        race.ParentRaceDayInfo = rdi;
        //        foreach (HPTHorse horse in race.HorseList)
        //        {
        //            horse.ParentRace = race;
        //            horse.MarksPercentString = horse.MarksPercent.ToString();
        //            horse.MarksQuantity = horse.MarksPercent * 11111;
        //            horse.Driver = rdi.DriverList.PersonList.First(p => p.ShortName == horse.DriverNameShort);
        //            horse.Trainer = rdi.TrainerList.PersonList.First(p => p.ShortName == horse.TrainerNameShort);
        //            horse.YearStatisticsList = new List<HPTHorseYearStatistics>() { horse.CurrentYearStatistics, horse.PreviousYearStatistics, horse.TotalStatistics };
        //            horse.CreateRankList();
        //            horse.CreateXReductionRuleList();
        //        }
        //    }

        //    HPTMarkBet markBet = new HPTMarkBet(rdi, bt);
        //    return markBet;
        //}

        #endregion

        void Config_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "UseA":
                    this.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.A).Use = HPTConfig.Config.UseA;
                    RemovePrioFromHorses(HPTPrio.A);
                    break;
                case "UseB":
                    this.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.B).Use = HPTConfig.Config.UseB;
                    RemovePrioFromHorses(HPTPrio.B);
                    break;
                case "UseC":
                    this.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.C).Use = HPTConfig.Config.UseC;
                    RemovePrioFromHorses(HPTPrio.C);
                    break;
                case "UseD":
                    this.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.D).Use = HPTConfig.Config.UseD;
                    RemovePrioFromHorses(HPTPrio.D);
                    break;
                case "UseE":
                    this.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.E).Use = HPTConfig.Config.UseE;
                    RemovePrioFromHorses(HPTPrio.E);
                    break;
                case "UseF":
                    this.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.F).Use = HPTConfig.Config.UseF;
                    RemovePrioFromHorses(HPTPrio.F);
                    break;
            }
        }

        void RemovePrioFromHorses(HPTPrio prio)
        {
            IEnumerable<HPTHorseXReduction> horseXReductionListPrio = this.RaceDayInfo.RaceList.SelectMany(r => r.HorseListSelected).Where(h => h.Prio == prio).SelectMany(h => h.HorseXReductionList);
            bool use = this.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == prio).Use;
            foreach (HPTHorseXReduction horseXReduction in horseXReductionListPrio)
            {
                horseXReduction.Selectable = use;
            }
        }

        private HPTCouponCorrector couponCorrector;
        [XmlIgnore]
        public HPTCouponCorrector CouponCorrector
        {
            get
            {
                return this.couponCorrector;
            }
            set
            {
                this.couponCorrector = value;
                //OnPropertyChanged("CouponCorrector");
            }
        }

        private bool lockCoupons;
        [DataMember]
        public bool LockCoupons
        {
            get
            {
                return this.lockCoupons;
            }
            set
            {
                this.lockCoupons = value;
                OnPropertyChanged("LockCoupons");
            }
        }

        private ObservableCollection<HPTCoupon> couponList;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTCoupon> CouponList
        {
            get
            {
                return this.couponList;
            }
            set
            {
                this.couponList = value;
                OnPropertyChanged("CouponList");
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTMarkBetSingleRowEdited> SingleRowEditedList { get; set; }

        internal void CreateSingleRowEditedList()
        {
            if (this.SingleRowCollection == null || this.SingleRowCollection.SingleRows == null)
            {
                return;
            }
            var editedList = this.SingleRowCollection.SingleRows
                .Where(sr => sr.Edited)
                .Select(sr => new HPTMarkBetSingleRowEdited()
                {
                    BetMultiplier = sr.BetMultiplier,
                    UniqueCode = sr.UniqueCode,
                    V6 = sr.V6
                })
                .ToList();

            if (editedList.Count > 0)
            {
                this.SingleRowEditedList = editedList;
            }
            else
            {
                this.SingleRowEditedList = null;
            }
        }

        internal void SetEditedSingleRows()
        {
            if (this.SingleRowEditedList != null && this.SingleRowEditedList.Count > 0)
            {
                foreach (var srEdited in this.SingleRowEditedList)
                {
                    var singleRow = this.SingleRowCollection.SingleRows
                        .FirstOrDefault(sr => sr.UniqueCode == srEdited.UniqueCode);
                    if (singleRow != null)
                    {
                        singleRow.Edited = true;
                        singleRow.V6 = srEdited.V6;
                        singleRow.BetMultiplier = srEdited.BetMultiplier;
                        singleRow.CreateBetMultiplierList(this);
                    }
                }
            }
            this.SingleRowEditedList = null;
        }

        #region Methods

        public HPTMarkBet Clone()
        {
            try
            {
                // KOMMANDE
                var hmbClone = (HPTMarkBet)HPTSerializer.CreateDeepCopy(this);
                hmbClone.Config = HPTConfig.Config;
                HPTServiceToHPTHelper.SetNonSerializedValues(hmbClone);
                return hmbClone;

                //string fileName = this.SaveDirectory + this.ToFileNameString() + ".hpt5";
                //HPTSerializer.SerializeHPTSystem(fileName, this);
                //HPTMarkBet hmbClone = HPTSerializer.DeserializeHPTSystem(fileName);
                //return hmbClone;
            }
            catch (Exception exc)
            {
                string error = exc.Message;
            }
            return null;
        }

        public void RecalculateReductionRank()
        {
            RecalculateReduction(RecalculateReason.Rank);
        }

        //private int numberOfRecalculations = 0;
        //StringBuilder sbStackTrace;
        public void RecalculateReduction(RecalculateReason reason)
        {
            if (this.pauseRecalculation || this.IsDeserializing || this.IsCalculatingTemplates)
            {
                return;
            }

            SetReductionRulesToApply();
            if ((this.SystemSize == 0 || this.ReductionRulesToApply.Count == 0)
                && (!this.V6SingleRows && !this.SingleRowBetMultiplier && !this.ReductionV6BetMultiplierRule)
                && !HPTConfig.Config.AlwaysCreateSingleRows)
            {
                if (this.SingleRowCollection.CalculationInProgress || this.SingleRowCollection.CompressionInProgress)
                {
                    this.SingleRowCollection.StopCalculation = true;
                }
                this.ReducedSize = this.SystemSize;
                RecalculateRank();
                RecalculateNumberOfX();
                if (this.SingleRowCollection.SingleRows != null && this.SingleRowCollection.SingleRows.Count > 0)
                {
                    this.SingleRowCollection.SingleRows.Clear();
                }
                this.SingleRowCollection.NumberOfCoveredRows = this.ReducedSize;
                this.SingleRowCollection.NumberOfAnalyzedRows = 0;
                this.SingleRowCollection.AnalyzedRowsShare = 0M;

                if (this.SystemSize > 0)
                {
                    foreach (var race in this.RaceDayInfo.RaceList)
                    {
                        foreach (var horse in race.HorseList)
                        {
                            if (horse.Selected)
                            {
                                horse.NumberOfCoveredRows = this.SystemSize / race.NumberOfSelectedHorses;
                                horse.SystemCoverage = 1M / race.NumberOfSelectedHorses;
                            }
                            else
                            {
                                horse.NumberOfCoveredRows = 0;
                                horse.SystemCoverage = 0M;
                            }
                        }
                    }
                    this.SingleRowCollection.HandleHighestAndLowestSums();
                    this.SingleRowCollection.HandleHighestAndLowestIncludedSums();
                    this.CouponCorrector.CouponHelper.CreateCoupons();

                    this.TotalCouponSize = this.SystemSize * this.BetMultiplier;
                    this.NumberOfCoupons = 1;
                    this.SingleRowCollection.CurrentCouponNumber = 1;
                    if (this.CouponCorrector.RaceDayInfo == null)
                    {
                        this.CouponCorrector.RaceDayInfo = this.RaceDayInfo;
                    }
                }
                else
                {
                    foreach (var horse in this.RaceDayInfo.RaceList.SelectMany(r => r.HorseList))
                    {
                        horse.NumberOfCoveredRows = 0;
                        horse.SystemCoverage = 0M;
                    }
                }
                return;
            }

            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(RecalculateReductionThreaded), ThreadPriority.Lowest);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        public void RecalculateReductionThreaded(object stateInfo)
        {
            try
            {
                if (this.IsDeserializing)
                {
                    return;
                }
                this.SingleRowCollection.StopCalculation = true;
                RecalculateNumberOfX();
                if (this.SystemSize == 0)
                {
                    return;
                }

                if (stateInfo != null)
                {
                    try
                    {
                        var prio = (ThreadPriority)stateInfo;
                        Thread.CurrentThread.Priority = prio;
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                }

                // Nollställ räknaren för hur många rader varje häst är med på
                foreach (HPTRace race in this.RaceDayInfo.RaceList)
                {
                    foreach (HPTHorse horse in race.HorseList)
                    {
                        horse.NumberOfCoveredRows = 0;
                    }
                }

                this.SingleRowCollection.UpdateRowCollection();
            }
            catch (Exception exc)
            {
                this.SingleRowCollection.StopCalculation = false;
                this.SingleRowCollection.CalculationInProgress = false;
                string s = exc.Message;
            }
        }

        public void CalculateSingleRowsPotential()
        {
            if (this.BetType.Code != "V64" && this.BetType.Code != "V75" && this.BetType.Code != "V86" && this.BetType.Code != "V65" && this.BetType.Code != "GS75" && this.BetType.Code != "V85")
            {
                return;
            }
            if (this.SingleRowCollection == null || this.SingleRowCollection.SingleRows == null)
            {
                return;
            }

            foreach (var singleRow in this.SingleRowCollection.SingleRows)
            {
                var payOutListOneError = new List<int>();
                var payOutListTwoErrors = new List<int>();
                var horseListToCalculate = singleRow.HorseList.ToArray();
                for (int i = 0; i < singleRow.HorseList.Count(); i++)
                {
                    var horseToExchange = horseListToCalculate[i];
                    var horseToCalculateOn = horseToExchange.ParentRace.HorseList
                        .Except(horseToExchange.ParentRace.HorseListSelected)
                        .OrderByDescending(h => h.StakeDistribution)
                        .FirstOrDefault();
                    if (horseToCalculateOn != null)
                    {
                        horseListToCalculate[i] = horseToCalculateOn;
                        int payOut = this.CouponCorrector.CalculatePayOutOneError(horseListToCalculate, this.RaceDayInfo.BetType.PoolShareOneError * this.RaceDayInfo.BetType.RowCost);
                        payOutListOneError.Add(payOut);

                        for (int j = i + 1; j < singleRow.HorseList.Count(); j++)
                        {
                            var horseToExchange2 = horseListToCalculate[j];
                            var horseToCalculateOn2 = horseToExchange2.ParentRace.HorseList
                                .Except(horseToExchange2.ParentRace.HorseListSelected)
                                .OrderByDescending(h => h.StakeDistribution)
                                .FirstOrDefault();
                            if (horseToCalculateOn2 != null)
                            {
                                horseListToCalculate[j] = horseToCalculateOn2;
                                int payOut2 = this.CouponCorrector.CalculatePayOutTwoErrors(horseListToCalculate, this.RaceDayInfo.BetType.PoolShareTwoErrors * this.RaceDayInfo.BetType.RowCost);
                                payOutListTwoErrors.Add(payOut2);
                                horseListToCalculate[j] = horseToExchange2;
                            }
                        }
                        horseListToCalculate[i] = horseToExchange;
                    }
                }
                singleRow.RowValueOneErrorLower = payOutListOneError.Min();
                singleRow.RowValueOneErrorUpper = payOutListOneError.Max();
                singleRow.RowValueTwoErrorsLower = payOutListTwoErrors.Min();
                singleRow.RowValueTwoErrorsUpper = payOutListTwoErrors.Max();
                //singleRow.RowValueThreeErrorsLower = payOutListTwoErrors.Min();   // TODO
                //singleRow.RowValueThreeErrorsUpper= payOutListTwoErrors.Max();
            }
        }

        public void CalculateSingleRowsPotentialRecursive() // TODO: Vafan används den här till egentligen
        {
            if (!this.BetType.HasMultiplePools || this.SingleRowCollection == null || this.SingleRowCollection.SingleRows == null)
            {
                return;
            }

            foreach (var singleRow in this.SingleRowCollection.SingleRows)
            {
                var payOutListOneError = new List<int>();
                var payOutListTwoErrors = new List<int>();
                var horseListToCalculate = singleRow.HorseList.ToArray();
                for (int i = 0; i < singleRow.HorseList.Count(); i++)
                {
                    var horseToExchange = horseListToCalculate[i];
                    var horseToCalculateOn = horseToExchange.ParentRace.HorseList
                        .Except(horseToExchange.ParentRace.HorseListSelected)
                        .OrderByDescending(h => h.StakeDistribution)
                        .FirstOrDefault();
                    if (horseToCalculateOn != null)
                    {
                        horseListToCalculate[i] = horseToCalculateOn;
                        int payOut = this.CouponCorrector.CalculatePayOutOneError(horseListToCalculate, this.RaceDayInfo.BetType.PoolShareOneError * this.RaceDayInfo.BetType.RowCost);
                        payOutListOneError.Add(payOut);

                        for (int j = i + 1; j < singleRow.HorseList.Count(); j++)
                        {
                            var horseToExchange2 = horseListToCalculate[j];
                            var horseToCalculateOn2 = horseToExchange2.ParentRace.HorseList
                                .Except(horseToExchange2.ParentRace.HorseListSelected)
                                .OrderByDescending(h => h.StakeDistribution)
                                .FirstOrDefault();
                            if (horseToCalculateOn2 != null)
                            {
                                horseListToCalculate[j] = horseToCalculateOn2;
                                int payOut2 = this.CouponCorrector.CalculatePayOutTwoErrors(horseListToCalculate, this.RaceDayInfo.BetType.PoolShareTwoErrors * this.RaceDayInfo.BetType.RowCost);
                                payOutListTwoErrors.Add(payOut2);
                                horseListToCalculate[j] = horseToExchange2;
                            }
                        }
                        horseListToCalculate[i] = horseToExchange;
                    }
                }
                singleRow.RowValueOneErrorLower = payOutListOneError.Min();
                singleRow.RowValueOneErrorUpper = payOutListOneError.Max();
                singleRow.RowValueTwoErrorsLower = payOutListTwoErrors.Min();
                singleRow.RowValueTwoErrorsUpper = payOutListTwoErrors.Max();
            }
        }

        public void UpdateCoupons()
        {
            if (this.LockCoupons)
            {
                return;
            }
            try
            {
                if (this.CouponCorrector == null)
                {
                    this.CouponCorrector = new HPTCouponCorrector();
                }
                if (this.CouponCorrector.RaceDayInfo == null)
                {
                    this.CouponCorrector.RaceDayInfo = this.RaceDayInfo;
                }
                if (this.CouponCorrector.CouponHelper == null)
                {
                    this.CouponCorrector.CouponHelper = new ATGCouponHelper(this);
                }
                this.CouponCorrector.CouponHelper.CreateCoupons();

                // Kontrollera om det skapats för många kuponger enligt ATGs nya regler
                HandleTooManyCoupons();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        internal void SetGeneralValues()
        {
            this.MaxRankSum = this.numberOfRaces * 16;
            this.MaxRankSumPercent = 100;

            // Klass för rättning av kuponger
            this.CouponCorrector = new HPTCouponCorrector();
            this.CouponCorrector.RaceDayInfo = this.RaceDayInfo;
            this.CouponCorrector.CouponHelper = new ATGCouponHelper(this);

            // Lista med resulta av mallberäkning
            this.TemplateResultList = new ObservableCollection<HPTMarkBetTemplateResult>();

            // Skapa klass för enkelrader och reduceringsberäkningar
            this.SingleRowCollection = new HPTMarkBetSingleRowCollection(this);
            this.SingleRowCollection.AnalyzingFinished += SingleRowCollection_AnalyzingFinished;

            // Klass för mailskickning
            this.MailSender = new HPTMailSender();

            this.ReductionRuleInfoList = new ObservableCollection<ReductionRuleInfo>();

            // Set coupon compression as default
            this.CompressCoupons = true;
        }

        internal void SetEventHandlers()
        {
            foreach (HPTRace race in this.RaceDayInfo.RaceList)
            {
                race.NumberOfSelectedChanged -= race_NumberOfSelectedChanged;
                race.NumberOfSelectedChanged += race_NumberOfSelectedChanged;
                race.SetAllSelected -= race_SetAllSelected;
                race.SetAllSelected += race_SetAllSelected;
                race.ClearABCD -= race_ClearABCD;
                race.ClearABCD += race_ClearABCD;
                race.PropertyChanged -= race_PropertyChanged;
                race.PropertyChanged += race_PropertyChanged;
            }

            this.RaceDayInfo.ABCDChanged -= RaceDayInfo_ABCDChanged;
            this.RaceDayInfo.ABCDChanged += RaceDayInfo_ABCDChanged;

            this.RaceDayInfo.ClearABCD -= RaceDayInfoOnClearAbcd;
            this.RaceDayInfo.ClearABCD += RaceDayInfoOnClearAbcd;

            this.RaceDayInfo.RankTemplateChanged -= ApplyConfigRankVariables;
            this.RaceDayInfo.RankTemplateChanged += ApplyConfigRankVariables;
        }

        void race_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Reserv1Nr" || e.PropertyName == "Reserv2Nr")
            {
                // Uppdatera kupongerna
                if (this.CouponCorrector != null && this.CouponCorrector.CouponHelper != null)
                {
                    this.CouponCorrector.CouponHelper.HandleReserverForCoupons(this.ReservHandling);
                }
            }
        }

        void race_ClearABCD(HPTRace race)
        {
            bool recalculationPaused = this.pauseRecalculation;
            this.pauseRecalculation = true;
            this.CompressCoupons = false;
            try
            {
                foreach (HPTHorse horse in race.HorseList)
                {
                    var horseXReduction = horse.HorseXReductionList.FirstOrDefault(hx => hx.Selected);
                    if (horseXReduction != null)
                    {
                        horseXReduction.Selected = false;
                        horse.Prio = HPTPrio.M;
                        horse.PrioString = string.Empty;
                    }
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            this.CompressCoupons = true;// couponsCompressed;
            this.pauseRecalculation = recalculationPaused;

            this.RecalculateReduction(RecalculateReason.All);
        }

        private void RaceDayInfoOnClearAbcd()
        {
            bool recalculationPaused = this.pauseRecalculation;
            this.pauseRecalculation = true;
            this.CompressCoupons = false;
            try
            {
                foreach (var race in this.RaceDayInfo.RaceList)
                {
                    foreach (HPTHorse horse in race.HorseList)
                    {
                        var horseXReduction = horse.HorseXReductionList.FirstOrDefault(hx => hx.Selected);
                        if (horseXReduction != null)
                        {
                            horseXReduction.Selected = false;
                            horse.Prio = HPTPrio.M;
                            horse.PrioString = string.Empty;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            this.CompressCoupons = true;
            this.pauseRecalculation = recalculationPaused;

            this.RecalculateReduction(RecalculateReason.All);
        }

        void race_SetAllSelected(HPTRace race, bool selected)
        {
            bool recalculationPaused = this.pauseRecalculation;
            this.pauseRecalculation = true;
            //bool couponsCompressed = this.CompressCoupons;
            this.CompressCoupons = false;

            try
            {
                foreach (HPTHorse horse in race.HorseList)
                {
                    if (horse.Scratched == false || horse.Scratched == null)
                    {
                        horse.Selected = selected;
                    }
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }

            this.CompressCoupons = true;//couponsCompressed;
            this.pauseRecalculation = recalculationPaused;

            this.RecalculateReduction(RecalculateReason.All);

            if (!selected)
            {
                this.ReducedSize = 0;
                this.ReductionQuota = 0M;
                race.Locked = false;
            }
        }

        void RaceDayInfo_ABCDChanged(object sender, EventArgs e)
        {
            if (!this.pauseRecalculation)
            {
                if (this.ABCDEFReductionRule.Use)
                {
                    RecalculateReduction(RecalculateReason.XReduction);
                }
                // SKA DET HÄR ALLTID GÖRAS?
                else if (this.ReductionHorseRank && this.HorseRankSumReductionRuleList.First(r => r.PropertyName == "RankABC").Use)
                {
                    RecalculateAllRanks();
                    RecalculateReduction(RecalculateReason.All);
                }
            }
            else if (!this.IsCalculatingTemplates)
            {
                RecalculateNumberOfX();
            }
        }

        internal void race_NumberOfSelectedChanged(int legNr, int startNr, bool selected)
        {
            bool recalculatingPaused = this.pauseRecalculation;
            this.pauseRecalculation = true;

            try
            {
                lock (this.RaceDayInfo.HorseListSelected)
                {
                    var race = this.RaceDayInfo.RaceList.FirstOrDefault(r => r.LegNr == legNr);
                    var selectedHorses = race.HorseList.Where(h => h.Selected).ToList();
                    if (race.HorseListSelected == null)
                    {
                        //race.HorseListSelected = new List<HPTHorse>(selectedHorses);
                        race.HorseListSelected = new List<HPTHorse>();
                    }

                    lock (race.HorseListSelected)
                    {
                        race.HorseListSelected.Clear();
                        race.HorseListSelected.AddRange(selectedHorses);
                        //else
                        //{
                        //    race.HorseListSelected.Clear();
                        //    race.HorseListSelected.AddRange(selectedHorses);
                        //}
                        race.NumberOfSelectedHorses = race.HorseListSelected.Count;
                    }

                    var horse = race.HorseList.FirstOrDefault(h => h.StartNr == startNr);
                    if (!horse.Selected)
                    {
                        foreach (var v6BetMultiplierRule in this.V6BetMultiplierRuleList)
                        {
                            v6BetMultiplierRule.RemoveHorse(horse);
                        }
                    }

                    if (horse.Selected && !this.RaceDayInfo.HorseListSelected.Contains(horse))
                    {
                        this.RaceDayInfo.HorseListSelected.Add(horse);
                    }
                    else if (!horse.Selected && this.RaceDayInfo.HorseListSelected.Contains(horse))
                    {
                        this.RaceDayInfo.HorseListSelected.Remove(horse);
                    }
                }

                // Beräkna systemstorlek
                this.SystemSize = this.RaceDayInfo.RaceList
                    .Select(r => r.NumberOfSelectedHorses)
                    .Aggregate((numberOfChosen, next) => numberOfChosen * next);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            this.pauseRecalculation = recalculatingPaused;

            if (this.IsCalculatingTemplates || this.IsDeserializing || this.pauseRecalculation)
            {
                return;
            }
            RecalculateReduction(RecalculateReason.All);
        }

        //internal void race_NumberOfSelectedChanged(int raceNr, int startNr, bool selected)
        //{
        //    bool recalculatingPaused = this.pauseRecalculation;
        //    this.pauseRecalculation = true;

        //    try
        //    {
        //        lock (this.RaceDayInfo.HorseListSelected)
        //        {
        //            //var race = this.RaceDayInfo.RaceList.FirstOrDefault(r => r.RaceNr == raceNr);
        //            horse.ParentRace.HorseListSelected = horse.ParentRace.HorseList.Where(h => h.Selected).ToList();
        //            if (horse.Selected)
        //            {
        //                this.RaceDayInfo.HorseListSelected.Add(horse);
        //            }
        //            else
        //            {
        //                this.RaceDayInfo.HorseListSelected.Remove(horse);
        //            }
        //        }

        //        if (!horse.Selected)
        //        {
        //            foreach (var v6BetMultiplierRule in this.V6BetMultiplierRuleList)
        //            {
        //                v6BetMultiplierRule.RemoveHorse(horse);
        //            }
        //        }

        //        // Beräkna systemstorlek
        //        this.SystemSize = this.RaceDayInfo.RaceList
        //            .Select(r => r.NumberOfSelectedHorses)
        //            .Aggregate((numberOfChosen, next) => numberOfChosen * next);
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.AddToErrorLogStatic(exc);
        //    }
        //    this.pauseRecalculation = recalculatingPaused;

        //    if (this.IsCalculatingTemplates || this.IsDeserializing || this.pauseRecalculation)
        //    {
        //        return;
        //    }
        //    RecalculateReduction(RecalculateReason.All);
        //}

        public void RecalculateNumberOfX()
        {
            foreach (HPTXReductionRule rule in this.ABCDEFReductionRule.XReductionRuleList)
            {
                // Antal hästar med viss Prio
                rule.NumberOfX = this.RaceDayInfo.HorseListSelected.Count(h => h.Prio == rule.Prio);

                // Antal lopp med hästar som har viss Prio
                rule.NumberOfRacesWithX = this.RaceDayInfo.HorseListSelected
                    .Where(h => h.Prio == rule.Prio)
                    .GroupBy(h => h.ParentRace.LegNr)
                    .Count();

                List<HPTXReductionRule> multiABCDXReductionList = this.MultiABCDEFReductionRule.ABCDEFReductionRuleList
                    .SelectMany(ar => ar.XReductionRuleList).Where(xr => xr.Prio == rule.Prio).ToList();

                foreach (var hptxReductionRule in multiABCDXReductionList)
                {
                    hptxReductionRule.NumberOfX = rule.NumberOfX;
                    hptxReductionRule.NumberOfRacesWithX = rule.NumberOfRacesWithX;

                    foreach (HPTNumberOfWinners multiNow in hptxReductionRule.NumberOfWinnersList)
                    {
                        multiNow.Selectable = multiNow.NumberOfWinners <= rule.NumberOfRacesWithX;
                        multiNow.IsSuperfluous = false;
                    }
                }

                foreach (HPTNumberOfWinners now in rule.NumberOfWinnersList)
                {
                    now.Selectable = now.NumberOfWinners <= rule.NumberOfRacesWithX;
                    now.IsSuperfluous = false;
                }
            }

            // Antal lopp som har någon ABCD-markering
            int numberOfRacesWithXReduction =
                this.RaceDayInfo.RaceList
                    .SelectMany(r => r.HorseListSelected)
                    .Where(h => (int)h.Prio > 0).
                    Select(h => h.ParentRace.LegNr)
                    .Distinct()
                    .Count();

            // Sätt flagga om villkoret är överflödigt
            SetSuperfluousFlag(this.ABCDEFReductionRule, numberOfRacesWithXReduction);
            foreach (var abcdefReductionRule in this.MultiABCDEFReductionRule.ABCDEFReductionRuleList)
            {
                SetSuperfluousFlag(abcdefReductionRule, numberOfRacesWithXReduction);
            }
        }

        internal void SetSuperfluousFlag(HPTABCDEFReductionRule rule, int numberOfRacesWithXReduction)
        {
            List<HPTXReductionRule> rulesToCheck = rule.XReductionRuleList.Where(r => r.NumberOfRacesWithX > 0).ToList();

            foreach (var reductionRule in rulesToCheck)
            {
                // För högt antal ABCD
                int sumOfRestMin = rulesToCheck.Where(xr => xr != reductionRule).Sum(xr => xr.MinNumberOfX);
                var superfluousNowList = reductionRule.NumberOfWinnersList.Where(now => now.NumberOfWinners > numberOfRacesWithXReduction - sumOfRestMin);
                foreach (var now in superfluousNowList)
                {
                    now.IsSuperfluous = true;
                }

                // För lågt antal ABCD
                int sumOfRestMax = rulesToCheck.Where(xr => xr != reductionRule).Sum(xr => xr.MaxNumberOfX);
                superfluousNowList = reductionRule.NumberOfWinnersList.Where(now => now.NumberOfWinners < numberOfRacesWithXReduction - sumOfRestMax);
                foreach (var now in superfluousNowList)
                {
                    now.IsSuperfluous = true;
                }
            }
        }

        public void ResetFilenameAndSave()
        {
            try
            {
                if (!Directory.Exists(this.SaveDirectory))  // Not saved locally
                {
                    this.SaveDirectory = HPTConfig.MyDocumentsPath + this.RaceDayInfo.ToDateAndTrackString() + "\\";
                    Directory.CreateDirectory(this.SaveDirectory);
                    HPTSerializer.SerializeHPTSystem(this.SaveDirectory + this.ToFileNameString() + ".hpt5", this);
                }
            }
            catch (Exception exc)
            {
                string fel = exc.Message;
            }
        }

        public void SetSerializerValues()
        {
            // Manuellt editerade enkelrader som måste sparas
            CreateSingleRowEditedList();

            // Utgångar
            if (this.ComplementaryRulesCollection.ReductionRuleList != null && this.ComplementaryRulesCollection.ReductionRuleList.Count > 0)
            {
                foreach (HPTComplementaryReductionRule complementaryReductionRule in this.ComplementaryRulesCollection.ReductionRuleList)
                {
                    complementaryReductionRule.HorseLightList = complementaryReductionRule.HorseList
                        .Select(h => new HPTHorseLight()
                        {
                            LegNr = h.ParentRace.LegNr,
                            StartNr = h.StartNr
                        }).ToList();
                }
            }

            // Kupongerna ska sparas oavsett villkor i övrigt
            if (this.LockCoupons)
            {
                this.CouponList = this.CouponCorrector.CouponHelper.CouponList;
            }
            else
            {
                this.CouponList = null;
            }

            // Spara undan hästinformation
            foreach (var race in this.RaceDayInfo.RaceList)
            {
                foreach (var horse in race.HorseList)
                {
                    // Lägg till informationen i den gemensamma kommentarsfilen
                    HPTConfig.Config.HorseOwnInformationCollection.MergeHorseOwnInformation(horse);
                }
            }
        }

        #endregion

        private ObservableCollection<ReductionRuleInfo> reductionRuleInfoList;
        [XmlIgnore]
        public ObservableCollection<ReductionRuleInfo> ReductionRuleInfoList
        {
            get
            {
                return this.reductionRuleInfoList;
            }
            set
            {
                this.reductionRuleInfoList = value;
                OnPropertyChanged("ReductionRuleInfoList");
            }
        }

        public string ClipboardString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(this.BetType.Code);
                sb.Append(" - ");
                sb.Append(this.RaceDayInfo.RaceDayDateString);
                sb.Append(" - ");
                sb.AppendLine(this.RaceDayInfo.Trackname);
                sb.AppendLine("Systemet skapat med 'Hjälp på traven!' (http://www.hpt.nu)");
                if (!string.IsNullOrEmpty(this.SystemURL))
                {
                    sb.Append("Rättningslänk: ");
                    sb.AppendLine(this.SystemURL);
                }
                //if (!string.IsNullOrEmpty(this.UploadedSystemGUID))
                //{
                //    sb.Append("RättningsURL: http://correction.hpt.nu/Default.aspx?SystemGUID=");
                //    sb.AppendLine(this.UploadedSystemGUID);
                //}
                return sb.ToString();
            }
        }

        internal string SetRankMeanString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.BetType.Code);
            sb.Append(" - ");
            sb.Append(this.RaceDayInfo.RaceDayDateString);
            sb.Append(" - ");
            sb.Append(this.RaceDayInfo.Trackname);
            try
            {
                foreach (var race in this.RaceDayInfo.RaceList)
                {
                    sb.AppendLine();
                    sb.Append(race.LegNr);
                    sb.Append(": ");
                    foreach (var horse in race.HorseList
                        .Where(h => h.Scratched == false || h.Scratched == null)
                        .OrderBy(h => h.RankWeighted))
                    {
                        sb.Append(horse.StartNr);
                        sb.Append(", ");
                    }
                    sb.Remove(sb.Length - 2, 2);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return sb.ToString();
        }

        internal string SetRankOwnString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.BetType.Code);
            sb.Append(" - ");
            sb.Append(this.RaceDayInfo.RaceDayDateString);
            sb.Append(" - ");
            sb.Append(this.RaceDayInfo.Trackname);
            try
            {
                foreach (var race in this.RaceDayInfo.RaceList)
                {
                    sb.AppendLine();
                    sb.Append(race.LegNr);
                    sb.Append(": ");
                    foreach (var horse in race.HorseList
                        .Where(h => h.Scratched == false || h.Scratched == null)
                        .OrderBy(h => h.RankOwn))
                    {
                        sb.Append(horse.StartNr);
                        sb.Append(", ");
                    }
                    sb.Remove(sb.Length - 2, 2);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return sb.ToString();
        }

        public void SetReductionRuleString()
        {
            this.ReductionRuleInfoList.Clear();
            StringBuilder sb = new StringBuilder();

            // Texter för olika varianter av V6/V7/V8/Flerbong
            if (this.V6)
            {
                string v6String = "OBS! Endast " + this.BetType.V6String;
                sb.AppendLine(v6String);
                this.ReductionRuleInfoList.Add(new ReductionRuleInfo() { ReductionTypeString = v6String });
            }
            if (this.BetMultiplier > 1)
            {
                string betMultiplierString = "OBS! Flerbong x " + this.BetMultiplier.ToString();
                sb.AppendLine(betMultiplierString);
                this.ReductionRuleInfoList.Add(new ReductionRuleInfo() { ReductionTypeString = betMultiplierString });

            }
            if (this.V6SingleRows)
            {
                string v6SingleRowsString = "OBS! " + this.BetType.V6String + " för rader med beräknat radvärde under " + this.V6UpperBoundary.ToString() + " kr";
                sb.AppendLine(v6SingleRowsString);
                this.ReductionRuleInfoList.Add(new ReductionRuleInfo() { ReductionTypeString = v6SingleRowsString });
            }
            if (this.SingleRowBetMultiplier)
            {
                string betMultiplierSingleRowsString = "OBS! Flerbong på enkelrader för att uppnå målvinsten " + this.SingleRowTargetProfit.ToString() + " kr";
                sb.AppendLine(betMultiplierSingleRowsString);
                this.ReductionRuleInfoList.Add(new ReductionRuleInfo() { ReductionTypeString = betMultiplierSingleRowsString });
            }

            // Texter för alla specialregler för V6/V7/V8/Flerbong
            foreach (var v6BetMultiplierRule in this.V6BetMultiplierRuleList)
            {
                string ruleString = v6BetMultiplierRule.ToString(this);
                sb.AppendLine(v6BetMultiplierRule.ClipboardString);

                ReductionRuleInfo rri = new ReductionRuleInfo();
                rri.ReductionRuleString = ruleString;
                this.ReductionRuleInfoList.Add(rri);
            }

            // Text för garantireducering
            if (this.GuaranteeReduction && this.NumberOfToleratedErrors > 0)
            {
                int numberOfCorrectGuaranteed = this.NumberOfRaces - this.NumberOfToleratedErrors;
                string ruleString = "Garantireducering: " + numberOfCorrectGuaranteed.ToString() +
                                    " rätt om alla villkor sitter.";
                sb.AppendLine(ruleString);

                ReductionRuleInfo rri = new ReductionRuleInfo();
                rri.ReductionRuleString = ruleString;
                this.ReductionRuleInfoList.Add(rri);
            }

            // Text för flerbong för utökad systemstorlek
            if (this.RandomRowReduction && this.RandomRowReductionTarget > 0)
            {
                sb.Append("OBS! Rader slumpmässigt borttagna för att nå en kostnad på ");
                sb.Append(this.RandomRowReductionTarget);
                sb.AppendLine(" kr.");
                sb.AppendLine();

                string ruleString = "Rader borttagna för att nå " + this.RandomRowReductionTarget.ToString() + " kr";
                ReductionRuleInfo rri = new ReductionRuleInfo();
                rri.ReductionRuleString = ruleString;
                this.ReductionRuleInfoList.Add(rri);
            }

            // Text för slumpmässigt borttag av rader för att nå målstorlek
            if (this.BetMultiplierRowAddition && this.BetMultiplierRowAdditionTarget > 0)
            {
                sb.Append("OBS! Flerbong spelat på vissa rader för att uppnå en kostnad på ");
                sb.Append(this.betMultiplierRowAdditionTarget);
                sb.AppendLine(" kr.");
                sb.AppendLine();

                string ruleString = "Flerbong för att uppnå " + this.BetMultiplierRowAdditionTarget.ToString() + " kr";
                ReductionRuleInfo rri = new ReductionRuleInfo();
                rri.ReductionRuleString = ruleString;
                this.ReductionRuleInfoList.Add(rri);
            }

            // Texter för alla reduceringsregler
            foreach (HPTReductionRule rule in this.ReductionRulesToApply)
            {
                foreach (var ruleInfo in rule.GetReductionRuleInfoList(this))
                {
                    sb.Append(ruleInfo.ToString());
                    this.ReductionRuleInfoList.Add(ruleInfo);
                }
            }

            // Skapa komplett text för klippbordet
            this.ReductionRulesString = sb.ToString();
        }

        private string reductionRulesString;
        [XmlIgnore]
        public string ReductionRulesString
        {
            get
            {
                return this.reductionRulesString;
            }
            set
            {
                this.reductionRulesString = value;
                OnPropertyChanged("ReductionRulesString");
            }
        }

        public string RaceInformationString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(this.ClipboardString);

                foreach (HPTRace race in this.RaceDayInfo.RaceList)
                {
                    sb.AppendLine();
                    sb.Append(race.ToClipboardString());
                }
                return sb.ToString();
            }
        }

        public string RaceInformationStringCompact
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                //sb.Append(this.ClipboardString);

                foreach (HPTRace race in this.RaceDayInfo.RaceList)
                {
                    sb.Append(race.ToCompactClipboardString());
                    sb.AppendLine();
                }
                return sb.ToString();
            }
        }

        public string ToClipboardString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.RaceInformationString);

            sb.AppendLine();

            sb.Append("Systemstorlek: ");
            sb.AppendLine(string.Format("{0:## ### ##0}", this.SystemSize));

            sb.Append("Reducerad storlek: ");
            sb.AppendLine(string.Format("{0:## ### ##0}", this.ReducedSize));

            sb.Append("Kostnad: ");
            sb.AppendLine(string.Format("{0:# ### ##0.00 kr}", this.SystemCost));

            sb.Append("Reduceringsgrad: ");
            sb.AppendLine(string.Format("{0:P1}", this.ReductionQuota));

            sb.AppendLine();

            SetReductionRuleString();
            sb.Append(this.ReductionRulesString);

            return sb.ToString();
        }

        public string ToReductionNamesString()
        {
            var reductionTypes = this.ReductionRulesToApply.Select(rr => rr.ReductionTypeString).Distinct();
            //var result = string.Join("\r\n", reductionTypes);
            var result = string.Join(",", reductionTypes);
            return result;
        }

        public string ExportHorseRanksToExcelFormat()
        {
            // HPTConfig.Config.DefaultRankTemplate.HorseRankVariableList

            var sb = new StringBuilder();
            sb.Append("\t\t\t");

            var ranksInOrder = this.RaceDayInfo.RaceList
                .First()
                .HorseList
                .First()
                .RankList.Select(hr => HPTConfig.Config.DefaultRankTemplate.HorseRankVariableList.First(rv => hr.Name == rv.PropertyName));

            string header = ranksInOrder
                .Select(r => r.Text + " (" + r.CategoryText + ")")
                .Aggregate((r, next) => r + "\t" + next);

            sb.AppendLine(header);
            this.RaceDayInfo.RaceList
                .SelectMany(r => r.HorseList)
                .ToList()
                .ForEach(h =>
                {
                    sb.Append(h.ParentRace.LegNrString);
                    sb.Append("\t");
                    sb.Append(h.StartNr);
                    sb.Append("\t");
                    sb.Append(h.HorseName);
                    sb.Append("\t");
                    string horseRanks = h.RankList.Select(hr => hr.Rank.ToString()).Aggregate((r, next) => r + "\t" + next);
                    sb.AppendLine(horseRanks);
                });

            return sb.ToString();
        }

        #region Reduction settings

        public static List<HPTReductionAttribute> GetReductionAttributeList()
        {
            List<HPTReductionAttribute> reductionAttributeList = new List<HPTReductionAttribute>();
            foreach (PropertyInfo pi in (typeof(HPTMarkBet)).GetProperties())
            {
                foreach (object o in pi.GetCustomAttributes(true))
                {
                    if (o.GetType() == typeof(HPTReductionAttribute))
                    {
                        HPTReductionAttribute hra = (HPTReductionAttribute)o;
                        reductionAttributeList.Add(hra);
                    }
                }
            }
            return reductionAttributeList;
        }

        private decimal minRankSum;
        [DataMember]
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
        [DataMember]
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

        private int minRankSumPercent;
        [DataMember]
        public int MinRankSumPercent
        {
            get
            {
                return this.minRankSumPercent;
            }
            set
            {
                this.minRankSumPercent = value;
                OnPropertyChanged("MinRankSumPercent");
            }
        }

        private int maxRankSumPercent;
        [DataMember]
        public int MaxRankSumPercent
        {
            get
            {
                return this.maxRankSumPercent;
            }
            set
            {
                this.maxRankSumPercent = value;
                OnPropertyChanged("MaxRankSumPercent");
            }
        }

        public List<HPTReductionAttribute> GetReductionAttributes()
        {
            List<HPTReductionAttribute> attributeList = new List<HPTReductionAttribute>();
            foreach (PropertyInfo pi in (typeof(HPTMarkBet)).GetProperties())
            {
                foreach (object o in pi.GetCustomAttributes(true))
                {
                    if (o.GetType() == typeof(HPTReductionAttribute))
                    {
                        HPTReductionAttribute ra = (HPTReductionAttribute)o;
                        attributeList.Add(ra);
                    }
                }
            }
            return attributeList;
        }

        private bool reductionV6BetMultiplierRule;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ReductionV6BetMultiplierRule
        {
            get
            {
                return reductionV6BetMultiplierRule;
            }
            set
            {
                if (this.reductionV6BetMultiplierRule == value)
                {
                    return;
                }
                reductionV6BetMultiplierRule = value;
                OnPropertyChanged("ReductionV6BetMultiplierRule");
                UpdateV6BetMultiplierSingleRows();
            }
        }

        private bool reductionRank;
        [HPTReduction("Rankpoäng", "ReductionRank", true, 2)]
        [DataMember]
        public bool ReductionRank
        {
            get
            {
                return reductionRank;
            }
            set
            {
                if (this.reductionRank == value)
                {
                    return;
                }
                reductionRank = value;
                OnPropertyChanged("ReductionRank");
                if (!this.IsDeserializing)
                {
                    RecalculateRank();
                    RecalculateReduction(RecalculateReason.Rank);
                }
            }
        }

        private bool reductionHorseRank;
        [HPTReduction("Rankvariabler", "ReductionHorseRank", true, 14)]
        [DataMember]
        public bool ReductionHorseRank
        {
            get
            {
                return reductionHorseRank;
            }
            set
            {
                if (this.reductionHorseRank == value)
                {
                    return;
                }
                reductionHorseRank = value;
                OnPropertyChanged("ReductionHorseRank");
                if (!this.IsDeserializing)
                {
                    RecalculateAllRanks();
                    RecalculateRank();
                    RecalculateReduction(RecalculateReason.Rank);
                }
            }
        }

        internal bool HasPercentageSumReduction
        {
            get
            {
                foreach (var intervalReductionRule in this.IntervalReductionRuleList)
                {
                    if (intervalReductionRule.MinPercentSum != 0 || intervalReductionRule.MaxPercentSum != 100)
                    {
                        return true;
                    }
                }
                if (this.ReductionRank && (this.MinRankSumPercent > 0 || this.MaxRankSumPercent < 100))
                {
                    return true;
                }
                return false;
            }
        }

        private ObservableCollection<HPTGroupIntervalRulesCollection> groupIntervalRulesCollectionList;
        [XmlIgnore]
        public ObservableCollection<HPTGroupIntervalRulesCollection> GroupIntervalRulesCollectionList
        {
            get
            {
                if (this.groupIntervalRulesCollectionList == null)
                {
                    this.groupIntervalRulesCollectionList = new ObservableCollection<HPTGroupIntervalRulesCollection>();
                }
                if (this.groupIntervalRulesCollectionList.Count == 0)
                {
                    var groupIntervalRulesCollectionsToShow =
                        this.Config.GroupIntervalRulesCollectionList.Where(
                            g => g.TypeCategory == this.BetType.TypeCategory);

                    foreach (var hptGroupIntervalRulesCollection in groupIntervalRulesCollectionsToShow)
                    {
                        this.groupIntervalRulesCollectionList.Add(hptGroupIntervalRulesCollection);
                    }

                    this.Config.GroupIntervalRulesCollectionList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(GroupIntervalRulesCollectionList_CollectionChanged);
                }
                return this.groupIntervalRulesCollectionList;
            }
        }

        void GroupIntervalRulesCollectionList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.groupIntervalRulesCollectionList.Clear();
            var groupIntervalRulesCollectionsToShow =
                        this.Config.GroupIntervalRulesCollectionList.Where(
                            g => g.TypeCategory == this.BetType.TypeCategory);

            foreach (var hptGroupIntervalRulesCollection in groupIntervalRulesCollectionsToShow)
            {
                this.groupIntervalRulesCollectionList.Add(hptGroupIntervalRulesCollection);
            }
        }

        private ObservableCollection<HPTHorseRankSumReductionRuleCollection> horseRankSumReductionRuleCollectionList;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankSumReductionRuleCollection> HorseRankSumReductionRuleCollectionList
        {
            get
            {
                if (this.horseRankSumReductionRuleCollectionList == null)
                {
                    this.horseRankSumReductionRuleCollectionList = new ObservableCollection<HPTHorseRankSumReductionRuleCollection>();
                }
                if (this.horseRankSumReductionRuleCollectionList.Count == 0)
                {
                    var horseRankSumReductionRuleCollectionsToShow =
                        this.Config.RankSumReductionRuleCollection.Where(
                            g => g.TypeCategory == this.BetType.TypeCategory);

                    foreach (var horseRankSumReductionRuleCollection in horseRankSumReductionRuleCollectionsToShow)
                    {
                        this.horseRankSumReductionRuleCollectionList.Add(horseRankSumReductionRuleCollection);
                    }

                    this.Config.RankSumReductionRuleCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(HorseRankSumReductionRuleCollectionList_CollectionChanged);
                }
                return this.horseRankSumReductionRuleCollectionList;
            }
        }

        void HorseRankSumReductionRuleCollectionList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.horseRankSumReductionRuleCollectionList.Clear();
            var horseRankSumReductionRuleCollectionsToShow =
                        this.Config.RankSumReductionRuleCollection.Where(
                            g => g.TypeCategory == this.BetType.TypeCategory);

            foreach (var horseRankSumReductionRuleCollection in horseRankSumReductionRuleCollectionsToShow)
            {
                this.horseRankSumReductionRuleCollectionList.Add(horseRankSumReductionRuleCollection);
            }
        }

        private ObservableCollection<HPTMarkBetTemplate> markBetTemplateList;
        [XmlIgnore]
        public ObservableCollection<HPTMarkBetTemplate> MarkBetTemplateList
        {
            get
            {
                if (this.markBetTemplateList == null)
                {
                    this.markBetTemplateList = new ObservableCollection<HPTMarkBetTemplate>();
                }
                if (this.markBetTemplateList.Count == 0)
                {
                    SetAvailableMarkBetTemplates();
                    this.Config.MarkBetTemplateABCDList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(MarkBetTemplateABCDList_CollectionChanged);
                    this.Config.MarkBetTemplateRankList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(MarkBetTemplateABCDList_CollectionChanged);
                }
                return this.markBetTemplateList;
            }
        }

        void MarkBetTemplateABCDList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    this.MarkBetTemplateList.Add(e.NewItems[0] as HPTMarkBetTemplate);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    this.MarkBetTemplateList.Remove(e.OldItems[0] as HPTMarkBetTemplate);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        internal void SetAvailableMarkBetTemplates()
        {
            // Rensa listan
            this.markBetTemplateList.Clear();

            // Lista med de ABCD-grupper som ska finnas tillgängliga i denna instans av HPTMarkBet
            IEnumerable<HPTPrio> priosToUse = this.ABCDEFReductionRule.XReductionRuleList
                .Where(xr => xr.Use)
                .Select(xr => xr.Prio);

            // Vilka ABCD-mallar som går att tillämpa
            IEnumerable<HPTMarkBetTemplate> tempListABCD =
                this.Config.MarkBetTemplateABCDList.Where(
                    mbt => mbt.TypeCategory == this.BetType.TypeCategory
                        && mbt.PriosToUse
                        .Intersect(priosToUse)
                        .Count() == mbt.PriosToUse.Count()
                        );

            IEnumerable<HPTMarkBetTemplate> tempListRank =
                this.Config.MarkBetTemplateRankList.Where(
                    mbt => mbt.TypeCategory == this.BetType.TypeCategory);

            foreach (var template in tempListABCD)
            {
                this.markBetTemplateList.Add(template);
            }
            foreach (var template in tempListRank)
            {
                this.markBetTemplateList.Add(template);
            }
        }

        private HPTMarkBetTemplateABCD markBetTemplateABCD;
        [XmlIgnore]
        public HPTMarkBetTemplateABCD MarkBetTemplateABCD
        {
            get
            {
                return markBetTemplateABCD;
            }
            set
            {
                markBetTemplateABCD = value;
                OnPropertyChanged("MarkBetTemplate");
            }
        }

        private HPTMarkBetTemplateRank markBetTemplateRank;
        [XmlIgnore]
        public HPTMarkBetTemplateRank MarkBetTemplateRank
        {
            get
            {
                return markBetTemplateRank;
            }
            set
            {
                markBetTemplateRank = value;
                OnPropertyChanged("MarkBetTemplate");
            }
        }

        #endregion

        #region Reduction rules

        public void SetReductionRulesToApply()
        {
            if (this.reductionRulesToApply == null)
            {
                this.reductionRulesToApply = new List<HPTReductionRule>();
            }
            else
            {
                this.reductionRulesToApply.Clear();
            }

            // ABCD och Multi-ABCD. Multi-ABCD övertrumfar
            if (this.MultiABCDEFReductionRule.Use)
            {
                this.reductionRulesToApply.Add(this.MultiABCDEFReductionRule);
                foreach (var abcdReductionRule in this.MultiABCDEFReductionRule.ABCDEFReductionRuleList)
                {
                    foreach (var xReductionRule in abcdReductionRule.XReductionRuleList)
                    {
                        xReductionRule.SetSkipRule();
                        //CalculateBestABCDCombination(xReductionRule);
                    }
                }
            }
            else if (this.ABCDEFReductionRule.Use)
            {
                this.reductionRulesToApply.Add(this.ABCDEFReductionRule);
                foreach (var xReductionRule in this.ABCDEFReductionRule.XReductionRuleList)
                {
                    xReductionRule.SetSkipRule();
                    //CalculateBestABCDCombination(xReductionRule);
                }

                string header = Enumerable.Range(0, this.NumberOfRaces + 1)
                .Select(nr => nr.ToString())
                .Aggregate((nr, next) => nr + "\t" + next);
            }

            // Kusk och tränare
            if (this.DriverRulesCollection.Use)
            {
                this.reductionRulesToApply.Add(this.DriverRulesCollection);
            }
            if (this.TrainerRulesCollection.Use)
            {
                this.reductionRulesToApply.Add(this.TrainerRulesCollection);
            }

            // Complementary reduction rules
            if (this.ComplementaryRulesCollection.Use)
            {
                this.reductionRulesToApply.Add(this.ComplementaryRulesCollection);
            }

            // Interval reduction rules
            this.reductionRulesToApply.AddRange(this.IntervalReductionRuleList.Where(r => r.Use));

            // Ranksummereducering
            if (this.ReductionRank && RankReductionRule != null)
            {
                this.reductionRulesToApply.Add(RankReductionRule);
            }

            // Rankvariabelreducering
            if (this.ReductionHorseRank && this.HorseRankSumReductionRuleList != null)
            {
                this.reductionRulesToApply.AddRange(this.HorseRankSumReductionRuleList.Where(r => r.Use));
                this.reductionRulesToApply.AddRange(this.HorseOwnRankSumReductionRuleList.Where(r => r.Use && !this.reductionRulesToApply.Contains(r)));
                foreach (var rule in this.HorseRankSumReductionRuleList.SelectMany(hr => hr.ReductionRuleList))
                {
                    rule.SetSkipRule();
                }
            }

            // Groupinterval reduction rules
            if (this.GroupIntervalRulesCollection.Use)
            {
                this.reductionRulesToApply.Add(this.GroupIntervalRulesCollection);
                foreach (var groupIntervalReductionRule in this.GroupIntervalRulesCollection.ReductionRuleList)
                {
                    groupIntervalReductionRule.SetSkipRule();
                }
            }

            // Chansvärderingsregel
            if (this.OwnProbabilityReductionRule.Use)
            {
                this.reductionRulesToApply.Add(this.OwnProbabilityReductionRule);
            }

            // Om ingen reducering föreligger, men att det finns andra villkor som ska skapa upp rader och kuponger
            if (this.reductionRulesToApply.Count == 0)
            {
                if (HasSingleRowRule())
                {
                    this.reductionRulesToApply.Add(new HPTReductionRule());
                }
                //else if (this.ReducedSize != this.SystemSize && this.ReducedSize > 0)
                //{
                //    this.reductionRulesToApply.Add(new HPTReductionRule());
                //}
            }
            //if (this.reductionRulesToApply.Count == 0 && (HasSingleRowRule() || this.ReducedSize != this.SystemSize))
            //{
            //    this.reductionRulesToApply.Add(new HPTReductionRule());
            //}

            // Gör nödvändiga förberedelse inför ny beräkning
            this.reductionRulesToApply.ForEach(r => r.Reset());
            this.ReductionRulesToApply = this.reductionRulesToApply;

            // Specialhantering för när hästar i utgångar blivit borttagna
            if (this.ComplementaryRulesCollection.Use && this.ComplementaryRulesCollection.ReductionRuleList.Count(r => r.Use) > 0)
            {
                foreach (HPTComplementaryReductionRule rule in this.ComplementaryRulesCollection.ReductionRuleList.Where(r => r.Use))
                {
                    List<HPTHorse> horseList = rule.HorseList.Where(h => !h.Selected).ToList();
                    if (horseList.Count > 0)
                    {
                        foreach (var hptHorse in horseList)
                        {
                            rule.HorseList.Remove(hptHorse);
                        }
                    }
                }
            }
        }

        private List<HPTReductionRule> reductionRulesToApply;
        [XmlIgnore]
        public List<HPTReductionRule> ReductionRulesToApply
        {
            get
            {
                if (this.reductionRulesToApply == null)
                {
                    SetReductionRulesToApply();
                }
                return this.reductionRulesToApply;
            }
            set
            {
                this.reductionRulesToApply = value;
                OnPropertyChanged("ReductionRulesToApply");
            }
        }

        [XmlIgnore]
        public bool OnlyABCDReduction
        {
            get
            {
                foreach (HPTReductionRule rule in this.ReductionRulesToApply)
                {
                    if (rule.GetType() != typeof(HPTXReductionRule))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTRankReductionRule RankReductionRule { get; set; }

        [HPTReduction("Radvärde", "RowValueReductionRule.Use", true, 7)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTRowValueReductionRule RowValueReductionRule { get; set; }

        [HPTReduction("Spårsumma", "StartNrSumReductionRule.Use", true, 8)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTStartNrSumReductionRule StartNrSumReductionRule { get; set; }

        //[HPTReduction("Procentsumma", "PercentSumReductionRule.Use", true, 9)]
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTPercentSumReductionRule PercentSumReductionRule { get; set; }

        [HPTReduction("Insatsfördelning", "StakePercentSumReductionRule.Use", true, 10)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTStakePercentSumReductionRule StakePercentSumReductionRule { get; set; }

        [HPTReduction("ATG-rank", "ATGRankSumReductionRule.Use", true, 11)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTATGRankSumReductionRule ATGRankSumReductionRule { get; set; }

        [HPTReduction("Egen rank", "OwnRankSumReductionRule.Use", true, 12)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTOwnRankSumReductionRule OwnRankSumReductionRule { get; set; }

        [HPTReduction("Poäng", "AlternateRankSumReductionRule.Use", true, 12)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTAlternateRankSumReductionRule AlternateRankSumReductionRule { get; set; }

        [HPTReduction("Oddssumma", "OddsSumReductionRule.Use", true, 13)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTOddsSumReductionRule OddsSumReductionRule { get; set; }

        // KOMMANDE
        [HPTReduction("Chansvärdering", "OwnProbabilityReductionRule.Use", true, 14)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTOwnProbabilityReductionRule OwnProbabilityReductionRule { get; set; }

        [HPTReduction("Utgång", "ComplementaryRulesCollection.Use", true, 5)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTComplementaryRulesCollection ComplementaryRulesCollection { get; set; }

        [HPTReduction("Kusk", "DriverRulesCollection.Use", true, 3)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTPersonRulesCollection DriverRulesCollection { get; set; }

        [HPTReduction("Tränare", "TrainerRulesCollection.Use", true, 4)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTPersonRulesCollection TrainerRulesCollection { get; set; }

        [HPTReduction("Gruppintervall", "GroupIntervalRulesCollection.Use", true, 6)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTGroupIntervalRulesCollection GroupIntervalRulesCollection { get; set; }

        [XmlIgnore]
        public ObservableCollection<HPTIntervalReductionRule> IntervalReductionRuleList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTV6BetMultiplierRule> V6BetMultiplierRuleList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTHorseRankSumReductionRule> HorseRankSumReductionRuleList { get; set; }

        [XmlIgnore]
        public ObservableCollection<HPTHorseRankSumReductionRule> HorseOwnRankSumReductionRuleList { get; set; }

        [XmlIgnore]
        public ObservableCollection<HPTHorseVariable> HorseVariableList { get; set; }

        [XmlIgnore]
        public ObservableCollection<HPTHorse> HorseListUncovered { get; set; }

        [XmlIgnore]
        public ObservableCollection<HPTHorse> HorseListOverlappingComplementaryRule { get; set; }

        [HPTReduction("ABCD", "ABCDEFReductionRule.Use", false, 1)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTABCDEFReductionRule ABCDEFReductionRule { get; set; }

        [HPTReduction("Multi-ABCD", "MultiABCDEFReductionRule.Use", true, 1)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTMultiABCDEFReductionRule MultiABCDEFReductionRule { get; set; }

        #endregion

        #region General properties

        private string systemURL;
        [DataMember]
        public string SystemURL
        {
            get
            {
                return this.systemURL;
            }
            set
            {
                this.systemURL = value;
                OnPropertyChanged("SystemURL");
            }
        }

        private string userCommentsDescription;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string UserCommentsDescription
        {
            get
            {
                return this.userCommentsDescription;
            }
            set
            {
                this.userCommentsDescription = value;
                OnPropertyChanged("UserCommentsDescription");
            }
        }

        private string systemComment;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string SystemComment
        {
            get
            {
                return this.systemComment;
            }
            set
            {
                this.systemComment = value;
                OnPropertyChanged("SystemComment");
            }
        }

        private HPTMailSender mailSender;
        [XmlIgnore]
        public HPTMailSender MailSender
        {
            get
            {
                if (this.mailSender == null)
                {
                    this.mailSender = new HPTMailSender();
                }
                return this.mailSender;
            }
            set
            {
                this.mailSender = value;
                OnPropertyChanged("MailSender");
            }
        }

        //private CouponCompression couponCompression;
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public CouponCompression CouponCompression
        //{
        //    get
        //    {
        //        return this.couponCompression;
        //    }
        //    set
        //    {
        //        this.couponCompression = value;
        //        OnPropertyChanged("CouponCompression");

        //        // Uppdatera kupongerna
        //        if (this.CouponCorrector != null && this.CouponCorrector.CouponHelper != null)
        //        {
        //            this.CouponCorrector.CouponHelper.HandleReserverForCoupons(this.ReservHandling);
        //        }
        //    }
        //}

        private bool compressCoupons;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool CompressCoupons
        {
            get
            {
                return this.compressCoupons;
            }
            set
            {
                this.compressCoupons = value;
                OnPropertyChanged("CompressCoupons");
                if (value && this.SingleRowCollection != null
                    && !this.IsDeserializing)
                {
                    try
                    {
                        this.SingleRowCollection.CompressToCouponsThreaded();
                    }
                    catch (Exception exc)
                    {
                        this.Config.AddToErrorLog(exc);
                        return;
                    }
                }
                else if (!value && this.SingleRowCollection != null
                    && this.SingleRowCollection.CompressedCoupons != null
                    && !this.IsDeserializing)
                {
                    this.SingleRowCollection.ClearRowCombinations();
                    this.CouponCorrector.CouponHelper.CouponList.Clear();
                }
            }
        }

        private bool useV6BetMultiplierRules;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool UseV6BetMultiplierRules
        {
            get
            {
                return this.useV6BetMultiplierRules;
            }
            set
            {
                this.useV6BetMultiplierRules = value;
                OnPropertyChanged("UseV6BetMultiplierRules");
                //SetV6BetMultiplierSingleRows();
            }
        }

        private bool v6;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool V6
        {
            get
            {
                return this.v6;
            }
            set
            {
                this.v6 = value;
                OnPropertyChanged("V6");
                UpdateV6BetMultiplierSingleRows();
            }
        }

        private bool v6SingleRows;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool V6SingleRows
        {
            get
            {
                return this.v6SingleRows;
            }
            set
            {
                if (this.v6SingleRows == value)
                {
                    return;
                }
                this.v6SingleRows = value;
                OnPropertyChanged("V6SingleRows");
                UpdateV6BetMultiplierSingleRows();
            }
        }

        private decimal v6UpperBoundary;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public decimal V6UpperBoundary
        {
            get
            {
                return this.v6UpperBoundary;
            }
            set
            {
                this.v6UpperBoundary = value;
                OnPropertyChanged("V6UpperBoundary");
                if (V6SingleRows)
                {
                    UpdateV6BetMultiplierSingleRows();
                    //SetV6BetMultiplierSingleRows();
                }
            }
        }

        private bool v6OwnRank;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool V6OwnRank
        {
            get
            {
                return this.v6OwnRank;
            }
            set
            {
                if (this.v6OwnRank == value)
                {
                    return;
                }
                this.v6OwnRank = value;
                OnPropertyChanged("V6OwnRank");
                UpdateV6BetMultiplierSingleRows();
            }
        }

        private int v6OwnRankMax;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int V6OwnRankMax
        {
            get
            {
                return this.v6OwnRankMax;
            }
            set
            {
                this.v6OwnRankMax = value;
                OnPropertyChanged("V6OwnRankMax");
                if (this.V6OwnRank)
                {
                    UpdateV6BetMultiplierSingleRows();
                }
            }
        }

        private bool singleRowBetMultiplier;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool SingleRowBetMultiplier
        {
            get
            {
                return this.singleRowBetMultiplier;
            }
            set
            {
                if (this.singleRowBetMultiplier == value)
                {
                    return;
                }
                this.singleRowBetMultiplier = value;
                OnPropertyChanged("SingleRowBetMultiplier");
                UpdateV6BetMultiplierSingleRows();
                //SetV6BetMultiplierSingleRows();
            }
        }

        private int ownProbabilityCostTarget;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int OwnProbabilityCostTarget
        {
            get
            {
                return this.ownProbabilityCostTarget;
            }
            set
            {
                this.ownProbabilityCostTarget = value;
                OnPropertyChanged("OwnProbabilityCostTarget");
                if (!this.IsDeserializing && !this.pauseRecalculation && this.OwnProbabilityCost)
                {
                    RecalculateReduction(RecalculateReason.All);
                }
            }
        }

        private bool ownProbabilityCost;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool OwnProbabilityCost
        {
            get
            {
                return this.ownProbabilityCost;
            }
            set
            {
                if (this.ownProbabilityCost == value)
                {
                    return;
                }
                this.ownProbabilityCost = value;
                OnPropertyChanged("OwnProbabilityCost");
                if (!this.IsDeserializing && !this.pauseRecalculation)
                {
                    RecalculateReduction(RecalculateReason.All);
                }
            }
        }

        private int singleRowTargetProfit;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int SingleRowTargetProfit
        {
            get
            {
                return this.singleRowTargetProfit;
            }
            set
            {
                this.singleRowTargetProfit = value;
                OnPropertyChanged("SingleRowTargetProfit");
                if (this.SingleRowBetMultiplier)
                {
                    UpdateV6BetMultiplierSingleRows();
                    //SetV6BetMultiplierSingleRows();
                }
            }
        }

        private bool guaranteeReduction;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool GuaranteeReduction
        {
            get
            {
                return this.guaranteeReduction;
            }
            set
            {
                if (this.guaranteeReduction == value)
                {
                    return;
                }
                this.guaranteeReduction = value;
                OnPropertyChanged("GuaranteeReduction");
                if (!this.IsDeserializing && !this.pauseRecalculation)
                {
                    RecalculateReduction(RecalculateReason.All);
                }
            }
        }


        private bool _FastGuaranteeReduction;
        [DataMember]
        public bool FastGuaranteeReduction
        {
            get
            {
                return _FastGuaranteeReduction;
            }
            set
            {
                this._FastGuaranteeReduction = value;
                OnPropertyChanged("FastGuaranteeReduction");
                if (this.guaranteeReduction && !this.IsDeserializing && !this.pauseRecalculation)
                {
                    RecalculateReduction(RecalculateReason.All);
                }
            }
        }

        private int numberOfToleratedErrors;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int NumberOfToleratedErrors
        {
            get
            {
                return this.numberOfToleratedErrors;
            }
            set
            {
                this.numberOfToleratedErrors = value;
                OnPropertyChanged("NumberOfToleratedErrors");
                if (this.GuaranteeReduction && !this.IsDeserializing && !this.pauseRecalculation)
                {
                    RecalculateReduction(RecalculateReason.All);
                }
            }
        }

        private bool randomRowReduction;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool RandomRowReduction
        {
            get
            {
                return this.randomRowReduction;
            }
            set
            {
                if (this.randomRowReduction == value)
                {
                    return;
                }
                this.randomRowReduction = value;
                OnPropertyChanged("RandomRowReduction");
                if (!this.IsDeserializing && !this.pauseRecalculation)
                {
                    RecalculateReduction(RecalculateReason.All);
                }
            }
        }

        private int randomRowReductionTarget;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int RandomRowReductionTarget
        {
            get
            {
                return this.randomRowReductionTarget;
            }
            set
            {
                this.randomRowReductionTarget = value;
                OnPropertyChanged("RandomRowReductionTarget");
                if (!this.IsDeserializing && !this.pauseRecalculation && this.RandomRowReduction)
                {
                    RecalculateReduction(RecalculateReason.All);
                }
            }
        }

        private bool betMultiplierRowAddition;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool BetMultiplierRowAddition
        {
            get
            {
                return this.betMultiplierRowAddition;
            }
            set
            {
                if (this.betMultiplierRowAddition == value)
                {
                    return;
                }
                this.betMultiplierRowAddition = value;
                OnPropertyChanged("BetMultiplierRowAddition");
                if (!this.IsDeserializing && !this.pauseRecalculation)
                {
                    RecalculateReduction(RecalculateReason.All);
                }
            }
        }

        private int betMultiplierRowAdditionTarget;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int BetMultiplierRowAdditionTarget
        {
            get
            {
                return this.betMultiplierRowAdditionTarget;
            }
            set
            {
                this.betMultiplierRowAdditionTarget = value;
                OnPropertyChanged("BetMultiplierRowAdditionTarget");
                if (!this.IsDeserializing && !this.pauseRecalculation)
                {
                    RecalculateReduction(RecalculateReason.All);
                }
            }
        }

        private bool v6Enabled = true;
        [XmlIgnore]
        public bool V6Enabled
        {
            get
            {
                return this.v6Enabled;
            }
            set
            {
                this.v6Enabled = value;
                OnPropertyChanged("V6Enabled");
            }
        }

        private bool compressionEnabled = true;
        [XmlIgnore]
        public bool CompressionEnabled
        {
            get
            {
                return this.compressionEnabled;
            }
            set
            {
                this.compressionEnabled = value;
                OnPropertyChanged("CompressionEnabled");
            }
        }

        private void InterruptCalculationAndCompression()
        {
            SingleRowCollection_AnalyzingFinished();
        }

        //public void SetV6BetMultiplierSingleRows()
        //{
        //    // Vid deserialisering 
        //    if (this.SingleRowCollection == null || this.SingleRowCollection.SingleRows == null)
        //    {
        //        return;
        //    }
        //    if (this.SingleRowCollection.SingleRows.Count == 0 || this.SingleRowTargetProfit <= 0)
        //    {
        //        if (this.SingleRowBetMultiplier || this.V6SingleRows || this.ReductionV6BetMultiplierRule)
        //        {
        //            RecalculateReductionThreaded(new object());
        //            return;
        //        }
        //    }

        //    DateTime dtStart1 = DateTime.Now;
        //    int numberOfRows = 0;
        //    this.SingleRowCollection.SingleRows
        //        .AsParallel()
        //        .ForAll(sr =>
        //            {
        //                sr.SetV6BetMultiplier(this);
        //                numberOfRows += sr.BetMultiplier;
        //            });
        //    TimeSpan ts1 = DateTime.Now - dtStart1;

        //    DateTime dtStart2 = DateTime.Now;
        //    numberOfRows = 0;
        //    this.SingleRowCollection.SingleRows.ForEach(sr =>
        //    {
        //        sr.SetV6BetMultiplier(this);
        //        numberOfRows += sr.BetMultiplier;
        //    });
        //    TimeSpan ts2 = DateTime.Now - dtStart1;

        //    DateTime dtStart3 = DateTime.Now;
        //    numberOfRows = 0;
        //    foreach (HPTMarkBetSingleRow singlerow in this.SingleRowCollection.SingleRows)
        //    {
        //        singlerow.SetV6BetMultiplier(this);
        //        numberOfRows += singlerow.BetMultiplier;
        //    }
        //    TimeSpan ts3 = DateTime.Now - dtStart1;

        //    var sb = new StringBuilder();
        //    sb.AppendLine("PLINQ: " + ts1.TotalMilliseconds.ToString());
        //    sb.AppendLine("LINQ: " + ts2.TotalMilliseconds.ToString());
        //    sb.AppendLine("FOREACH: " + ts3.TotalMilliseconds.ToString());
        //    string s = sb.ToString();

        //    this.TotalCouponSize = numberOfRows;
        //    SingleRowCollection_AnalyzingFinished();
        //}

        public void UpdateV6BetMultiplierSingleRows()
        {
            // Vid deserialisering 
            if (this.IsDeserializing)
            {
                return;
            }
            else if (this.SingleRowCollection == null || this.SingleRowCollection.SingleRows == null || this.SingleRowCollection.SingleRows.Count == 0)
            {
                if (this.SystemSize > 0)
                {
                    RecalculateReduction(RecalculateReason.All);
                }
                return;
            }

            int numberOfRows = 0;
            this.SingleRowCollection.SingleRows
                .ForEach(sr =>
                {
                    sr.SetV6BetMultiplier(this);
                    numberOfRows += sr.BetMultiplier;
                });

            //DateTime dtStart1 = DateTime.Now;
            //int numberOfRows = 0;
            //this.SingleRowCollection.SingleRows
            //    .AsParallel()
            //    .ForAll(sr =>
            //    {
            //        sr.SetV6BetMultiplier(this);
            //        numberOfRows += sr.BetMultiplier;
            //    });
            //TimeSpan ts1 = DateTime.Now - dtStart1;

            //DateTime dtStart2 = DateTime.Now;
            //numberOfRows = 0;
            //this.SingleRowCollection.SingleRows.ForEach(sr =>
            //{
            //    sr.SetV6BetMultiplier(this);
            //    numberOfRows += sr.BetMultiplier;
            //});
            //TimeSpan ts2 = DateTime.Now - dtStart1;

            //DateTime dtStart3 = DateTime.Now;
            //numberOfRows = 0;
            //foreach (HPTMarkBetSingleRow singlerow in this.SingleRowCollection.SingleRows)
            //{
            //    singlerow.SetV6BetMultiplier(this);
            //    numberOfRows += singlerow.BetMultiplier;
            //}
            //TimeSpan ts3 = DateTime.Now - dtStart1;

            //var sb = new StringBuilder();
            //sb.AppendLine("PLINQ: " + ts1.TotalMilliseconds.ToString());
            //sb.AppendLine("LINQ: " + ts2.TotalMilliseconds.ToString());
            //sb.AppendLine("FOREACH: " + ts3.TotalMilliseconds.ToString());
            //string s = sb.ToString();

            this.TotalCouponSize = numberOfRows;
            this.SingleRowEditedList = null;
            SingleRowCollection_AnalyzingFinished();
        }

        internal void UpdateTotalCouponSize()
        {
            // Vid deserialisering 
            if (this.SingleRowCollection == null || this.SingleRowCollection.SingleRows == null || this.SingleRowCollection.SingleRows.Count == 0)
            {
                return;
            }

            this.TotalCouponSize = this.SingleRowCollection.SingleRows.Sum(sr => sr.BetMultiplier);
        }

        private System.Windows.Visibility v6Visibility;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public System.Windows.Visibility V6Visibility
        {
            get
            {
                return this.v6Visibility;
            }
            set
            {
                this.v6Visibility = value;
                OnPropertyChanged("V6Visibility");
            }
        }

        public System.Windows.Visibility MultiplePoolsVisibility
        {
            get
            {
                if (this.BetType.PayOutDummyList.Length > 1)
                {
                    return System.Windows.Visibility.Visible;
                }
                return System.Windows.Visibility.Collapsed;
            }
        }

        internal void UpdateSystemSize()
        {
            foreach (var race in this.RaceDayInfo.RaceList)
            {
                race.HorseListSelected = race.HorseList.Where(h => h.Selected == true).ToList();
                race.NumberOfSelectedHorses = race.HorseListSelected.Count;
            }

            // Beräkna systemstorlek
            this.SystemSize = this.RaceDayInfo.RaceList
                .Select(r => r.NumberOfSelectedHorses)
                .Aggregate((numberOfChosen, next) => numberOfChosen * next);
        }

        private int size;
        [DataMember]
        public int SystemSize
        {
            get
            {
                return size;
            }
            set
            {
                this.size = value;
                OnPropertyChanged("SystemSize");
            }
        }

        private int? maxPotentialWinnings;
        public int? MaxPotentialWinnings
        {
            get
            {
                return maxPotentialWinnings;
            }
            set
            {
                this.maxPotentialWinnings = value;
                OnPropertyChanged("MaxPotentialWinnings");
            }
        }

        private decimal systemProbability;
        public decimal SystemProbability
        {
            get
            {
                return systemProbability;
            }
            set
            {
                this.systemProbability = value;
                OnPropertyChanged("SystemProbability");
            }
        }

        private decimal jackpotProbability;
        public decimal JackpotProbability
        {
            get
            {
                return jackpotProbability;
            }
            set
            {
                this.jackpotProbability = value;
                OnPropertyChanged("JackpotProbability");
            }
        }

        private int jackpotRowsOneError;
        public int JackpotRowsOneError
        {
            get
            {
                return jackpotRowsOneError;
            }
            set
            {
                this.jackpotRowsOneError = value;
                OnPropertyChanged("JackpotRowsOneError");
            }
        }

        private int jackpotRowsTwoErrors;
        public int JackpotRowsTwoErrors
        {
            get
            {
                return jackpotRowsTwoErrors;
            }
            set
            {
                this.jackpotRowsTwoErrors = value;
                OnPropertyChanged("JackpotRowsTwoErrors");
            }
        }

        private decimal reducedSystemProbability;
        public decimal ReducedSystemProbability
        {
            get
            {
                return reducedSystemProbability;
            }
            set
            {
                this.reducedSystemProbability = value;
                OnPropertyChanged("ReducedSystemProbability");
            }
        }

        private decimal systemProbabilityRatio;
        public decimal SystemProbabilityRatio
        {
            get
            {
                return systemProbabilityRatio;
            }
            set
            {
                this.systemProbabilityRatio = value;
                OnPropertyChanged("SystemProbabilityRatio");
            }
        }

        private int totalCouponSize;
        [DataMember]
        public int TotalCouponSize
        {
            get
            {
                return totalCouponSize;
            }
            set
            {
                this.totalCouponSize = value;
                OnPropertyChanged("TotalCouponSize");
                this.SystemCost = value * this.BetType.RowCost;
            }
        }

        private int reducedSize;
        [DataMember]
        public int ReducedSize
        {
            get
            {
                return this.reducedSize;
            }
            set
            {
                this.reducedSize = value;
                OnPropertyChanged("ReducedSize");
                if (this.SystemSize > 0)
                {
                    this.ReductionQuota = Convert.ToDecimal(this.ReducedSize) / Convert.ToDecimal(this.SystemSize);
                    this.ReductionQuota = 1 - this.reductionQuota;
                }
            }
        }

        private decimal systemCost;
        [DataMember]
        public decimal SystemCost
        {
            get
            {
                return this.systemCost;
            }
            set
            {
                // Beräkna skillnad i kostnad
                if (this.systemCost != value && this.systemCost > 0M && value > 0M)
                {
                    this.SystemCostChange = value - this.systemCost;
                    this.SystemCostChangeRelative = (value / this.systemCost) - 1M;
                }

                // Sätt nytt värde
                this.systemCost = value;
                OnPropertyChanged("SystemCost");

                //// Specialhantering om gratisanvändare gör för stort system
                //if (!HPTConfig.Config.IsPayingCustomer && value > this.BetType.MaxBetForNotPayingCustomer)
                //{
                //    this.TooExpensive = true;
                //}
                //else if (this.TooExpensive)
                //{
                //    this.TooExpensive = false;
                //}
            }
        }

        private int numberOfSystems;
        [XmlIgnore]
        public int NumberOfSystems
        {
            get
            {
                return this.numberOfSystems;
            }
            set
            {
                this.numberOfSystems = value;
                OnPropertyChanged("NumberOfSystems");
            }
        }

        private decimal systemCostChange;
        [XmlIgnore]
        public decimal SystemCostChange
        {
            get
            {
                return this.systemCostChange;
            }
            set
            {
                this.systemCostChange = value;
                OnPropertyChanged("SystemCostChange");
            }
        }

        private decimal systemCostChangeRelative;
        [XmlIgnore]
        public decimal SystemCostChangeRelative
        {
            get
            {
                return this.systemCostChangeRelative;
            }
            set
            {
                this.systemCostChangeRelative = value;
                OnPropertyChanged("SystemCostChangeRelative");
            }
        }

        private bool tooExpensive;
        [XmlIgnore]
        public bool TooExpensive
        {
            get
            {
                return this.tooExpensive;
            }
            set
            {
                this.tooExpensive = value;
                OnPropertyChanged("TooExpensive");
            }
        }

        private decimal reductionQuota;
        [DataMember]
        public decimal ReductionQuota
        {
            get
            {
                return this.reductionQuota;
            }
            set
            {
                this.reductionQuota = value;
                OnPropertyChanged("ReductionQuota");
            }
        }

        private int numberOfRaces;
        [DataMember]
        public int NumberOfRaces
        {
            get
            {
                return this.numberOfRaces;
            }
            set
            {
                this.numberOfRaces = value;
                OnPropertyChanged("NumberOfRaces");
            }
        }

        private int numberOfCoupons;
        [DataMember]
        public int NumberOfCoupons
        {
            get
            {
                return this.numberOfCoupons;
            }
            set
            {
                this.numberOfCoupons = value;
                OnPropertyChanged("NumberOfCoupons");
            }
        }

        [DataMember]
        public int TotalMarksQuantity { get; set; }

        private HPTMarkBetSingleRowCollection singleRowCollection;
        [XmlIgnore]
        public HPTMarkBetSingleRowCollection SingleRowCollection
        {
            get
            {
                return this.singleRowCollection;
            }
            set
            {
                this.singleRowCollection = value;
                OnPropertyChanged("SingleRowCollection");
            }
        }

        private int betMultiplier;
        [DataMember]
        public int BetMultiplier
        {
            get
            {
                if (this.betMultiplier == 0)
                {
                    this.betMultiplier = 1;
                    this.BetMultiplierList = new List<int>() { 1 };
                }
                return this.betMultiplier;
            }
            set
            {
                this.betMultiplier = value;
                OnPropertyChanged("BetMultiplier");
                if (this.SystemSize > 0)
                {
                    CreateBetMultiplierList();
                    UpdateV6BetMultiplierSingleRows();
                    //this.TotalCouponSize = this.ReducedSize * this.BetMultiplier;
                    if (!this.IsDeserializing)
                    {
                        this.CouponCorrector.CouponHelper.CreateCoupons();
                    }
                }
            }
        }

        [XmlIgnore]
        public List<int> BetMultiplierList { get; set; }

        public void CreateBetMultiplierList()
        {
            this.BetMultiplierList = new List<int>();
            int tempMultiplier = this.BetMultiplier;

            while (tempMultiplier > 0)
            {
                int partialMultiplier = this.BetType.BetMultiplierList.Where(bm => bm <= tempMultiplier).Max();
                this.BetMultiplierList.Add(partialMultiplier);
                tempMultiplier -= partialMultiplier;
            }
        }

        #endregion

        #region ToString-versions

        private string uploadedSystemGUID;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string UploadedSystemGUID
        {
            get
            {
                return this.uploadedSystemGUID;
            }
            set
            {
                this.uploadedSystemGUID = value;
                OnPropertyChanged("UploadedSystemGUID");
            }
        }

        public string ToFileNameString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.BetType.Code);
            sb.Append("_");
            sb.Append(this.RaceDayInfo.TracknameFile);
            sb.Append("_");
            sb.Append(this.RaceDayInfo.TrackId);
            sb.Append("_");
            sb.Append(this.RaceDayInfo.RaceDayDateString);
            sb.Append("_");
            sb.Append(this.ReducedSize);
            sb.Append("_");
            sb.Append(this.SystemSize);
            try
            {
                if (!string.IsNullOrEmpty(this.SystemName))
                {
                    string systemName = this.SystemName.Replace("\\", string.Empty);
                    systemName = systemName.Replace("/", string.Empty);
                    foreach (char c in Path.GetInvalidPathChars())
                    {
                        systemName = systemName.Replace(c, '_');
                    }

                    sb.Append("_");
                    sb.Append(systemName);
                    sb.Append("_");
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return sb.ToString();
        }

        #endregion

        #region Singlerow calculations testing

        #endregion

        public void ClearAll()
        {
            Clear(true, true, true);
        }

        public void ClearReductions()
        {
            Clear(false, false, true);
        }

        public void ClearABCD()
        {
            Clear(false, true, false);
        }

        public void Clear(bool clearLockedRaces, bool clearABCD, bool clearReductions)
        {
            bool recalculationPaused = this.pauseRecalculation;
            pauseRecalculation = true;

            try
            {
                // Gör en lista med ALLA hästar
                IEnumerable<HPTHorse> horseList = this.RaceDayInfo.RaceList.SelectMany(r => r.HorseList);
                if (clearLockedRaces)  // Nollställ Locked-flaggan på de lopp som har det
                {
                    IEnumerable<HPTRace> lockedRaces = this.RaceDayInfo.RaceList.Where(r => r.Locked);
                    foreach (var race in lockedRaces)
                    {
                        race.Locked = false;
                    }
                }
                else  // Ta bort hästar som är i låsta lopp
                {
                    horseList = horseList.Where(h => h.ParentRace.Locked != true && h.Locked != true);
                }

                // Nollställ alla hästar
                foreach (HPTHorse horse in horseList)
                {
                    if (horse.Selected)
                    {
                        if ((clearABCD || clearReductions) && !clearLockedRaces)
                        {
                            var xReduction = horse.HorseXReductionList.FirstOrDefault(xr => xr.Prio == horse.Prio);
                            if (xReduction != null && xReduction.Prio != HPTPrio.M)
                            {
                                xReduction.Selected = false;
                            }
                            horse.Prio = HPTPrio.M;
                            horse.PrioString = string.Empty;
                        }
                        else
                        {
                            horse.Selected = false;
                        }
                        horse.NumberOfCoveredRows = 0;
                        horse.SystemCoverage = 0M;
                        horse.SelectedForComplimentaryRule = false;
                    }
                    try
                    {
                        horse.Reserv1 = null;
                        horse.Reserv2 = null;
                    }
                    catch (Exception) { }
                }


                if (this.reductionRulesToApply != null && clearReductions)
                {
                    this.reductionRulesToApply.Clear();

                    // Ställ tillbaka alla reduceringsvarianter till false
                    this.ABCDEFReductionRule.Use = false;
                    this.ABCDEFReductionRule.Clear();
                    this.ReductionRank = false;
                    this.ReductionHorseRank = false;
                    this.ComplementaryRulesCollection.Use = false;
                    this.ComplementaryRulesCollection.Clear();
                    this.DriverRulesCollection.Use = false;
                    this.DriverRulesCollection.Clear();
                    this.TrainerRulesCollection.Use = false;
                    this.TrainerRulesCollection.Clear();
                    this.GroupIntervalRulesCollection.Use = false;
                    this.GroupIntervalRulesCollection.Clear();
                    if (this.OwnProbabilityReductionRule != null)
                    {
                        this.OwnProbabilityReductionRule.MinProbability = 0M;
                        this.OwnProbabilityReductionRule.Use = false;
                    }

                    foreach (var intervalReductionRule in this.IntervalReductionRuleList)
                    {
                        intervalReductionRule.Use = false;
                    }

                    this.MultiABCDEFReductionRule.Use = false;
                    foreach (var abcdefReductionRule in this.MultiABCDEFReductionRule.ABCDEFReductionRuleList)
                    {
                        foreach (HPTXReductionRule rule in abcdefReductionRule.XReductionRuleList)
                        {
                            abcdefReductionRule.Use = false;
                            abcdefReductionRule.Clear();
                        }
                    }

                    foreach (var rankReductionRule in this.HorseRankSumReductionRuleList)
                    {
                        //rankReductionRule.Use = false;
                        //rankReductionRule.Reset();
                    }
                    this.ReductionHorseRank = false;
                }
                else if (clearABCD)
                {
                    // Ställ tillbaka alla reduceringsvarianter till false
                    this.ABCDEFReductionRule.Use = false;
                    this.ABCDEFReductionRule.Clear();

                    foreach (var abcdefReductionRule in this.MultiABCDEFReductionRule.ABCDEFReductionRuleList)
                    {
                        foreach (HPTXReductionRule rule in abcdefReductionRule.XReductionRuleList)
                        {
                            abcdefReductionRule.Use = false;
                            abcdefReductionRule.Clear();
                        }
                    }
                }


                // Mallar
                if (this.TemplateResultList != null)
                {
                    this.TemplateResultList.Clear();
                }

                // Uppdatera enkelrader
                this.V6SingleRows = false;
                this.V6UpperBoundary = 0;
                this.SingleRowBetMultiplier = false;
                this.SingleRowTargetProfit = 0;
                this.SingleRowCollection.ClearAll();
                this.RandomRowReduction = false;
                this.RandomRowReductionTarget = 0;
                this.BetMultiplierRowAddition = false;
                this.BetMultiplierRowAdditionTarget = 0;
                this.GuaranteeReduction = false;
                this.NumberOfToleratedErrors = 0;
                this.OwnProbabilityCost = false;
                this.ownProbabilityCostTarget = 0;


                // V6/Flerbong
                this.ReductionV6BetMultiplierRule = false;
                if (this.V6BetMultiplierRuleList != null && this.V6BetMultiplierRuleList.Count > 0)
                {
                    try
                    {
                        this.V6BetMultiplierRuleList.Clear();
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                }

                // Uppdatera kuponger
                UpdateCoupons();
                this.CouponCorrector.CouponHelper.TotalSystemSize = 0;
                this.CouponCorrector.CouponHelper.CouponList.Clear();

                pauseRecalculation = false;

                // Nollställ övriga informationsfält
                this.NumberOfCoupons = 0;
                this.ReductionQuota = 0;
                this.SingleRowCollection.CurrentCouponNumber = 0;
                this.TotalCouponSize = 0;
                this.SingleRowCollection.CoveredRowsShare = 0M;
                this.SingleRowCollection.HandleHighestAndLowestSums();
                this.SingleRowCollection.HandleHighestAndLowestIncludedSums();

                // Nollställ radvärden för GUI-visning
                this.RowValueReductionRule.LowestIncludedSum = 0;
                this.RowValueReductionRule.LowestSum = 0;
                this.RowValueReductionRule.HighestIncludedSum = 0;
                this.RowValueReductionRule.HighestSum = 0;

                // Uppdatera systemstorleken om man bara raderat ABCD-tecken
                if (this.SystemSize == 0 && clearABCD)
                {
                    UpdateSystemSize();
                }

                // Gör en ny beräkning med tomt system
                RecalculateReduction(RecalculateReason.All);

                // Uppdatera resultat
                if (this.CouponCorrector.RaceDayInfo != null && this.CouponCorrector.RaceDayInfo.HasResult)
                {
                    this.CouponCorrector.UpdateResult(0, false);
                }

                // Nollställ varningar
                this.HasOverlappingComplementaryRuleHorses = false;
                this.HasSuperfluousXReduction = false;
                this.HasUncoveredHorses = false;
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }

            this.pauseRecalculation = recalculationPaused;
        }

        private bool noReservChoiceMade;
        [XmlIgnore]
        public bool NoReservChoiceMade
        {
            get
            {
                return this.noReservChoiceMade;
            }
            set
            {
                this.noReservChoiceMade = value;
                OnPropertyChanged("NoReservChoiceMade");
            }
        }

        private bool hasOverlappingComplementaryRuleHorses;
        [XmlIgnore]
        public bool HasOverlappingComplementaryRuleHorses
        {
            get
            {
                return this.hasOverlappingComplementaryRuleHorses;
            }
            set
            {
                this.hasOverlappingComplementaryRuleHorses = value;
                OnPropertyChanged("HasOverlappingComplementaryRuleHorses");
            }
        }

        private bool hasUncoveredHorses;
        [XmlIgnore]
        public bool HasUncoveredHorses
        {
            get
            {
                return this.hasUncoveredHorses;
            }
            set
            {
                this.hasUncoveredHorses = value;
                OnPropertyChanged("HasUncoveredHorses");
            }
        }

        private bool hasTooManySystems;
        [XmlIgnore]
        public bool HasTooManySystems
        {
            get
            {
                return this.hasTooManySystems;
            }
            set
            {
                this.hasTooManySystems = value;
                OnPropertyChanged("HasTooManySystems");
            }
        }

        private bool hasSuperfluousXReduction;
        [XmlIgnore]
        public bool HasSuperfluousXReduction
        {
            get
            {
                return this.hasSuperfluousXReduction;
            }
            set
            {
                this.hasSuperfluousXReduction = value;
                OnPropertyChanged("HasSuperfluousXReduction");
            }
        }

        internal void HandleTooManyCoupons()
        {
            //this.NumberOfSystems = Convert.ToInt32(this.CouponCorrector.CouponHelper.CouponList.Sum(c => c.BetMultiplier));
            this.NumberOfSystems = Convert.ToInt32(this.CouponCorrector.CouponHelper.CouponList.Sum(c => c.SystemSizeATG));
            if (this.NumberOfSystems > this.BetType.MaxNumberOfSystemsInFile)
            {
                this.HasTooManySystems = true;
            }
            else
            {
                this.HasTooManySystems = false;
            }
        }

        internal void HandleUncoveredHorses()
        {
            if (HPTConfig.Config.WarnIfUncoveredHorses && this.ReductionRulesToApply.Count > 0)
            {
                // Hästar som inte täcks på det reducerade systemet
                IEnumerable<HPTHorse> uncoveredHorses = this.RaceDayInfo.HorseListSelected.Where(h => h.SystemCoverage == 0M);
                if (uncoveredHorses.Any())
                {
                    this.HorseListUncovered = new ObservableCollection<HPTHorse>(uncoveredHorses);
                    this.HasUncoveredHorses = true;
                }
                else
                {
                    this.HasUncoveredHorses = false;
                }
            }
        }

        internal void HandleSuperfluousXReduction()
        {
            if (HPTConfig.Config.WarnIfSuperfluousXReduction && this.ABCDEFReductionRule.Use)
            {
                // Överflödig ABCD-reduceringsvillkor
                if (IsSuperfluousXreduction(true))
                {
                    this.HasSuperfluousXReduction = true;
                }
                else
                {
                    this.HasSuperfluousXReduction = false;
                }
            }
        }

        internal void HandleOverlappingComplementaryRules()
        {
            if (HPTConfig.Config.WarnIfOverlappingComplementaryRules && this.ComplementaryRulesCollection.Use)
            {
                IEnumerable<HPTHorse> allComplementaryRulesHorses = this.ComplementaryRulesCollection.ReductionRuleList
                    .Where(rr => rr.Use)
                    .Cast<HPTComplementaryReductionRule>()
                    .SelectMany(rr => rr.HorseList);

                IEnumerable<List<HPTHorse>> overlappingHorses = allComplementaryRulesHorses.GroupBy(h => new { h.StartNr, h.ParentRace.RaceNr })
                .Where(g => g.Count() > 1)
                .Select(g => g.ToList());

                if (overlappingHorses.Any())
                {
                    this.HorseListOverlappingComplementaryRule = new ObservableCollection<HPTHorse>(overlappingHorses.Select(oh => oh.First()));
                    this.HasOverlappingComplementaryRuleHorses = true;
                }
                else
                {
                    this.HasUncoveredHorses = false;
                }
            }
        }

        private ReservHandling reservHandling;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ReservHandling ReservHandling
        {
            get
            {
                return this.reservHandling;
            }
            set
            {
                this.reservHandling = value;
                OnPropertyChanged("ReservHandling");
            }
        }

        #region Template handling

        private ObservableCollection<HPTMarkBetTemplateResult> templateResultList;
        [XmlIgnore]
        public ObservableCollection<HPTMarkBetTemplateResult> TemplateResultList
        {
            get
            {
                return this.templateResultList;
            }
            set
            {
                this.templateResultList = value;
                OnPropertyChanged("TemplateResultList");
            }
        }

        private int numberOfTestedTemplates;
        [XmlIgnore]
        public int NumberOfTestedTemplates
        {
            get
            {
                return this.numberOfTestedTemplates;
            }
            set
            {
                this.numberOfTestedTemplates = value;
                OnPropertyChanged("NumberOfTestedTemplates");
            }
        }

        private int numberOfAddedTemplates;
        [XmlIgnore]
        public int NumberOfAddedTemplates
        {
            get
            {
                return this.numberOfAddedTemplates;
            }
            set
            {
                this.numberOfAddedTemplates = value;
                OnPropertyChanged("NumberOfAddedTemplates");
            }
        }

        private int numberOfSpikes;
        internal List<HPTHorse> SelectHorsesFromTemplate(HPTMarkBetTemplate markBetTemplate)
        {
            pauseRecalculation = true;

            ApplyConfigRankVariables(markBetTemplate.RankTemplate);

            this.TemplateResultList.Clear();

            this.NumberOfAddedTemplates = 0;
            this.NumberOfTestedTemplates = 0;

            foreach (HPTHorseRankVariable variable in this.HorseRankVariableList)
            {
                HPTHorseRankVariable hptHrv = markBetTemplate.RankTemplate.HorseRankVariableList
                    .FirstOrDefault(hrv => hrv.PropertyName == variable.PropertyName);

                if (hptHrv != null)
                {
                    variable.Use = hptHrv.Use;
                }
            }

            RecalculateRank();

            // Låsta lopp
            IEnumerable<HPTRace> lockedRaceList = this.RaceDayInfo.RaceList
                .Where(r => r.Locked);

            // Hitta spikarna
            this.numberOfSpikes = markBetTemplate.NumberOfSpikes > this.NumberOfRaces ? this.NumberOfRaces : markBetTemplate.NumberOfSpikes;
            int chosenSpikes = lockedRaceList.Count(r => r.NumberOfSelectedHorses == 1);
            int spikeNr = chosenSpikes > this.numberOfSpikes ? this.numberOfSpikes : chosenSpikes;

            // Plocka ut de egna spikarna
            HPTHorse[] lockedHorseList = lockedRaceList
                .Where(r => r.NumberOfSelectedHorses == 1)
                .Select(r => r.HorseListSelected.First(h => h.Selected))
                .OrderBy(h => h.RankWeighted)
                .ToArray();

            // Ta ut alla hästar som inte är i lopp med spik(ar)
            List<HPTHorse> horseList = this.RaceDayInfo.RaceList
                .Where(r => !r.Locked)
                .SelectMany(r => r.HorseList)
                .OrderBy(h => h.RankWeighted)
                .ToList();

            // Rensa alla valda hästar
            horseList.ForEach(h => h.Selected = (h.Locked == true && h.Selected));  // Låsta hästar ska inte nollställas

            int listPosition = 0;
            while (spikeNr < this.numberOfSpikes && listPosition < horseList.Count)
            {
                HPTHorse spikeCandidate = horseList[listPosition];
                bool selectAsSpike = (spikeCandidate.Selected && spikeCandidate.ParentRace.NumberOfSelectedHorses == 1)
                    || spikeCandidate.ParentRace.NumberOfSelectedHorses == 0;

                if (!selectAsSpike && spikeCandidate.ParentRace.NumberOfSelectedHorses == 0)
                {
                    selectAsSpike = true;
                }
                if (selectAsSpike)
                {
                    spikeCandidate.Selected = true;
                    spikeCandidate.ParentRace.Locked = true;
                    if (spikeCandidate.Prio != HPTPrio.M)
                    {
                        spikeCandidate.HorseXReductionList.First(hx => hx.Prio == spikeCandidate.Prio).Selected = false;
                    }
                    spikeNr++;
                }
                listPosition++;
            }

            int fullSize = markBetTemplate.DesiredSystemSize * 100 / (100 - markBetTemplate.ReductionPercentage);
            int rankPosition = 0;
            horseList = horseList
                .Where(h => !h.ParentRace.Locked)
                .OrderBy(h => h.RankWeighted)
                .ToList();

            while (this.SystemSize < fullSize && rankPosition < horseList.Count)
            {
                HPTHorse horse = horseList[rankPosition];
                horse.Selected = true;
                rankPosition++;
            }

            return this.RaceDayInfo.RaceList
                .SelectMany(r => r.HorseList)
                .Where(h => h.Selected)
                .OrderBy(h => h.RankWeighted)
                //.OrderBy(h => h.RankMean)
                .ToList();
        }

        [XmlIgnore]
        internal bool pauseRecalculation;

        public void SelectFromTemplateRank()
        {
            if (this.MarkBetTemplateRank == null)
            {
                return;
            }
            bool recalculationPaused = this.pauseRecalculation;
            pauseRecalculation = true;

            //this.vts = this.MarkBetTemplateRank.GetVxxTemplateSettings(this.RaceDayInfo.TypeCategory.Code);
            List<HPTHorse> horseList = SelectHorsesFromTemplate(this.MarkBetTemplateRank);

            //var templateSettings = (VxxTemplateRankSettings)this.vts;
            this.IsCalculatingTemplates = true;
            RecalculateReductionThreaded(this);
            this.templateResultList = new ObservableCollection<HPTMarkBetTemplateResult>();

            // Beräkna ranksummegränserna
            var rowsInOrder = this.SingleRowCollection.SingleRows
                .OrderBy(sr => sr.RankSum)
                .ToArray();
            decimal factor = rowsInOrder.Length / 100M;
            int lowerPosition = Convert.ToInt32(Convert.ToDecimal(this.MarkBetTemplateRank.LowerPercentageLimit) * factor);
            int upperPosition = Convert.ToInt32(Convert.ToDecimal(this.MarkBetTemplateRank.UpperPercentageLimit) * factor);
            this.MarkBetTemplateRank.MinRankValue = rowsInOrder[lowerPosition].RankSum;
            this.MarkBetTemplateRank.MaxRankValue = rowsInOrder[upperPosition].RankSum;

            this.ReductionRank = true;

            ChangeRankSize(this.MarkBetTemplateRank);

            if (this.TemplateResultList.Count > 0)
            {
                ApplyTemplateResult(this.TemplateResultList[0]);
            }
            else
            {
                this.ReductionRank = false;
            }

            this.IsCalculatingTemplates = false;
            pauseRecalculation = recalculationPaused;
        }

        [XmlIgnore]
        public HPTTemplateForBeginners TemplateForBeginners { get; set; }
        public void SelectFromBeginnerTemplate()
        {
            decimal reductionPercentage = 0.75M;
            switch (this.TemplateForBeginners.ReductionRisk)
            {
                case HPTReductionRisk.Medium:
                    reductionPercentage = 0.75M;
                    break;
                case HPTReductionRisk.Low:
                    reductionPercentage = 0.60M;
                    break;
                case HPTReductionRisk.High:
                    reductionPercentage = 0.90M;
                    break;
                default:
                    break;
            }

            int desiredSystemSize = Convert.ToInt32(this.TemplateForBeginners.Stake / this.BetType.RowCost);

            this.MarkBetTemplateABCD = new HPTMarkBetTemplateABCD()
            {
                DesiredSystemSize = desiredSystemSize,
                NumberOfSpikes = this.TemplateForBeginners.NumberOfSpikes,
                Use = true,
                ReductionPercentage = Convert.ToInt32(reductionPercentage * 100),
                RankTemplate = new HPTRankTemplate()
                {
                    HorseRankVariableList = new List<HPTHorseRankVariable>(this.TemplateForBeginners.HorseRankVariableList),
                    //Name = "Nybörjarmall"
                    Name = "Expressmall"
                }
            };
            this.MarkBetTemplateABCD.InitializeTemplate(new HPTPrio[] { HPTPrio.A, HPTPrio.B, HPTPrio.C });

            // Välj hästar
            SelectFromTemplateABCD();

            // Beräkna fram förslag
            //bool couponsCompressed = this.CompressCoupons;
            this.CompressCoupons = false;
            try
            {
                CreateSystemsFromTemplateABCD(new object());

                // Välj förslag utifrån hur mycket man vill vinna
                int lowerRowLimit = Convert.ToInt32(this.TemplateForBeginners.Stake / this.BetType.RowCost * 0.9M);
                int upperRowLimit = Convert.ToInt32(this.TemplateForBeginners.Stake / this.BetType.RowCost * 1.1M);
                var templatesToSchooseFrom = this.TemplateResultList
                    .Where(tr => tr.ReducedSize >= lowerRowLimit && tr.ReducedSize <= upperRowLimit)
                    .OrderBy(tr => tr.MinRowValue)
                    .ToArray();

                if (templatesToSchooseFrom.Length == 1)
                {
                    ApplyTemplateResult(templatesToSchooseFrom[0]);
                }
                else if (templatesToSchooseFrom.Length > 1)
                {
                    switch (this.TemplateForBeginners.DesiredProfit)
                    {
                        case HPTDesiredProfit.Medium:
                            int position = templatesToSchooseFrom.Length / 2;
                            ApplyTemplateResult(templatesToSchooseFrom[position]);
                            break;
                        case HPTDesiredProfit.Low:
                            ApplyTemplateResult(templatesToSchooseFrom.First());
                            break;
                        case HPTDesiredProfit.High:
                            ApplyTemplateResult(templatesToSchooseFrom.Last());
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }

            this.CompressCoupons = true;//couponsCompressed;
        }

        public HPTTemplateForBeginners CreateTemplateForBeginners()
        {
            // Välj ut de vanligaste rankvariablerna
            var rankVariablesToUse = this.HorseRankVariableList.Where(rv => rv.HorseRankInfo.UseForBeginner);

            // Skapa nybörjarmall med defaultvärden
            var templateForBeginners = new HPTTemplateForBeginners()
            {
                DesiredProfit = HPTDesiredProfit.Medium,
                HorseRankVariableList = new List<HPTHorseRankVariable>(rankVariablesToUse),
                NumberOfSpikes = 1,
                ReductionRisk = HPTReductionRisk.Medium,
                Stake = 300
            };
            return templateForBeginners;
        }

        public void SelectFromTemplateABCD()
        {
            if (this.MarkBetTemplateABCD == null)
            {
                return;
            }
            bool recalculationPaused = this.pauseRecalculation;
            pauseRecalculation = true;

            var allHorses = SelectHorsesFromTemplate(this.MarkBetTemplateABCD);
            var horseList = allHorses.Where(h => !h.ParentRace.Locked).ToList();

            int numberOfPrioLeft = this.MarkBetTemplateABCD.ABCDTemplateSettingsList.Count(abcd => abcd.Selected);
            foreach (ABCDTemplateSettings templateSettings in this.MarkBetTemplateABCD.ABCDTemplateSettingsList.Where(abcd => abcd.Selected).OrderBy(ts => ts.Prio))
            {
                if (templateSettings.Prio == HPTPrio.A)
                {
                    foreach (var race in this.RaceDayInfo.RaceList.Where(r => !r.Locked))
                    {
                        var aHorse = horseList.First(h => h.ParentRace == race);
                        var lockedAHorse = horseList.FirstOrDefault(h => h.ParentRace == race && h.Prio == HPTPrio.A);
                        if (aHorse.Prio == HPTPrio.M && lockedAHorse == null)
                        {
                            aHorse.HorseXReductionList.First(x => x.Prio == templateSettings.Prio).Selected = true;
                        }
                    }
                }
                else
                {
                    int numberOfHorseWithPrio = horseList.Count(h => h.Prio == templateSettings.Prio);
                    int numberOfHorsesToSelect = (horseList.Count / numberOfPrioLeft) - numberOfHorseWithPrio;
                    var horsesToChooseFrom = horseList
                        .Where(h => h.Prio != templateSettings.Prio)
                        .Take(numberOfHorsesToSelect)
                        .ToList();

                    foreach (var horse in horsesToChooseFrom)
                    {
                        if (horse.Prio == HPTPrio.M)
                        {
                            horse.HorseXReductionList.First(x => x.Prio == templateSettings.Prio).Selected = true;
                        }
                    }
                }
                numberOfPrioLeft--;
                horseList = horseList.Where(h => h.Prio != templateSettings.Prio).ToList();
            }

            foreach (HPTHorse horse in this.RaceDayInfo.HorseListSelected)
            {
                foreach (HPTHorseXReduction horseXReduction in horse.HorseXReductionList)
                {
                    horseXReduction.Selected = horse.Prio == horseXReduction.Prio;
                }
            }

            RecalculateNumberOfX();
            pauseRecalculation = recalculationPaused;
        }

        private bool isCalculatingTemplates;
        [XmlIgnore]
        public bool IsCalculatingTemplates
        {
            get
            {
                return this.isCalculatingTemplates;
            }
            set
            {
                this.isCalculatingTemplates = value;
                OnPropertyChanged("IsCalculatingTemplates");
            }
        }


        [XmlIgnore]
        internal bool InterruptSystemsCreation = false;

        public void CreateSystemsFromTemplateABCD()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateSystemsFromTemplateABCD));
        }

        public void CreateSystemsFromTemplateABCD(object stateInfo)
        {
            if (this.SystemSize == 0)
            {
                return;
            }
            bool recalculationPaused = this.pauseRecalculation;
            try
            {
                pauseRecalculation = true;
                this.InterruptSystemsCreation = false;

                this.templateResultList = new ObservableCollection<HPTMarkBetTemplateResult>();

                if (ABCDTemplateCombinations == null)
                {
                    InitiateABCDTemplateCombinations();
                }
                RecalculateNumberOfX();
                this.NumberOfRacesWithXReduction = this.RaceDayInfo.RaceList.Count - numberOfSpikes;
                this.XreductionRulesToUse = new List<HPTXReductionRule>();
                foreach (HPTXReductionRule rule in this.ABCDEFReductionRule.XReductionRuleList)
                {
                    var abcdTemplateSetting = this.MarkBetTemplateABCD.ABCDTemplateSettingsList.First(ats => ats.Prio == rule.Prio);
                    rule.Use = abcdTemplateSetting.Selected;
                    if (rule.Use && rule.NumberOfRacesWithX > 0)
                    {
                        this.XreductionRulesToUse.Add(rule);

                        rule.CombinationsToTest = GetABCDTemplateCombinations(rule.Prio, rule.NumberOfRacesWithX);

                        //int combinationsToTestIndex = 0;

                        //switch (rule.Prio)
                        //{
                        //    case HPTPrio.A:
                        //        combinationsToTestIndex = rule.NumberOfRacesWithX > 5 ? 5 : rule.NumberOfRacesWithX;
                        //        break;
                        //    case HPTPrio.B:
                        //        combinationsToTestIndex = rule.NumberOfRacesWithX > 4 ? 4 : rule.NumberOfRacesWithX;
                        //        break;
                        //    case HPTPrio.C:
                        //        combinationsToTestIndex = rule.NumberOfRacesWithX > 3 ? 3 : rule.NumberOfRacesWithX;
                        //        break;
                        //    case HPTPrio.D:
                        //        combinationsToTestIndex = rule.NumberOfRacesWithX > 2 ? 2 : rule.NumberOfRacesWithX;
                        //        break;
                        //    case HPTPrio.E:
                        //        combinationsToTestIndex = rule.NumberOfRacesWithX > 1 ? 1 : rule.NumberOfRacesWithX;
                        //        break;
                        //    case HPTPrio.F:
                        //        combinationsToTestIndex = rule.NumberOfRacesWithX > 1 ? 1 : rule.NumberOfRacesWithX;
                        //        break;
                        //    default:
                        //        break;
                        //}
                        //rule.CombinationsToTest = ABCDTemplateCombinations[combinationsToTestIndex];
                    }
                }

                this.IsCalculatingTemplates = true;
                this.ABCDEFReductionRule.Reset();
                this.ABCDEFReductionRule.Use = true;
                SetReductionRulesToApply();

                //bool currentCouponCompression = this.CompressCoupons;
                try
                {
                    this.CompressCoupons = false;
                }
                catch (NotSupportedException)
                {
                }
                //this.SingleRowCollection.SingleRowsObservable = new ObservableCollection<HPTMarkBetSingleRow>();
                this.SingleRowCollection.SingleRows.Clear();
                ChangeABCDSize(this.MarkBetTemplateABCD.DesiredSystemSize, 0);  // Testa alla alternativ


                this.TemplateResultList = new ObservableCollection<HPTMarkBetTemplateResult>(this.templateResultList.OrderBy(tr => tr.AbsDiff));
                this.InterruptSystemsCreation = false;
                this.IsCalculatingTemplates = false;
                pauseRecalculation = false;

                if (this.TemplateResultList.Count > 0)
                {
                    ApplyTemplateResult(this.TemplateResultList[0]);

                    // Ställ tillbaka kupongkomprimering, fånga trådfel... :-(
                    //if (this.CompressCoupons != currentCouponCompression)
                    if (!this.CompressCoupons)
                    {
                        try
                        {
                            this.CompressCoupons = true;//currentCouponCompression;
                        }
                        catch (NotSupportedException)
                        {
                        }
                    }
                }
                else
                {
                    pauseRecalculation = true;
                    this.ABCDEFReductionRule.Clear();
                    this.ABCDEFReductionRule.Use = false;
                    pauseRecalculation = recalculationPaused;
                    RecalculateReduction(RecalculateReason.XReduction);
                    this.CompressCoupons = true;// currentCouponCompression;
                }
            }
            catch (Exception exc)
            {
                pauseRecalculation = true;
                this.ABCDEFReductionRule.Clear();
                this.ABCDEFReductionRule.Use = false;
                pauseRecalculation = recalculationPaused;
                RecalculateReduction(RecalculateReason.XReduction);
                Config.AddToErrorLog(exc);
            }
            this.IsCalculatingTemplates = false;
        }

        public void ApplyTemplateResult(HPTMarkBetTemplateResult tr)
        {
            pauseRecalculation = true;
            if (this.ABCDEFReductionRule.Use)
            {
                foreach (HPTXReductionRule newRule in tr.XReductionRuleList)
                {
                    HPTXReductionRule oldRule = this.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == newRule.Prio);
                    foreach (HPTNumberOfWinners newNow in newRule.NumberOfWinnersList)
                    {
                        HPTNumberOfWinners oldNow = oldRule.NumberOfWinnersList.First(now => now.NumberOfWinners == newNow.NumberOfWinners);
                        oldNow.Selected = newNow.Selected;
                    }
                }
                this.IsCalculatingTemplates = false;
                pauseRecalculation = false;
                RecalculateReduction(RecalculateReason.XReduction);
            }
            else if (this.ReductionRank)
            {
                this.MinRankSum = tr.MinRankSum;
                this.MaxRankSum = tr.MaxRankSum;
                pauseRecalculation = false;
                this.IsCalculatingTemplates = false;
                RecalculateReduction(RecalculateReason.Rank);
            }
        }

        private int NumberOfRacesWithXReduction;
        private List<HPTXReductionRule> XreductionRulesToUse;

        private void ChangeABCDSize(int desiredSystemSize, int ruleIndex)
        {
            HPTXReductionRule rule = this.ABCDEFReductionRule.XReductionRuleList[ruleIndex];
            foreach (System.Collections.BitArray ba in rule.CombinationsToTest)
            {
                for (int numberOfSelected = 0; numberOfSelected < ba.Length; numberOfSelected++)
                {
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == numberOfSelected).Selected = ba[numberOfSelected];
                }
                rule.Reset();
                rule.SkipRule = false;

                // Kolla om reduceringsvarianten är överflödig
                if (ruleIndex > 0 && IsSuperfluousXreduction(ruleIndex == this.XreductionRulesToUse.Count - 1))
                {
                    for (int numberOfSelected = 0; numberOfSelected < ba.Length; numberOfSelected++)
                    {
                        rule.NumberOfWinnersList.First(now => now.NumberOfWinners == numberOfSelected).Selected = false;
                    }
                }
                else if (ruleIndex == this.XreductionRulesToUse.Count - 1)
                {
                    if (this.InterruptSystemsCreation)
                    {
                        return;
                    }
                    RecalculateReductionThreaded(this);
                    this.NumberOfTestedTemplates++;

                    decimal quota = Convert.ToDecimal(this.ReducedSize) / Convert.ToDecimal(desiredSystemSize);
                    this.NumberOfTestedTemplates++;
                    if (quota > 0.7M && quota < 1.4M)
                    {
                        var tr = new HPTMarkBetTemplateResult(this);
                        tr.AbsDiff = Math.Abs(this.ReducedSize - desiredSystemSize);
                        this.templateResultList.Add(tr);
                        this.NumberOfAddedTemplates++;
                    }
                }
                else
                {
                    ChangeABCDSize(desiredSystemSize, ruleIndex + 1);
                }
            }
        }

        private bool IsSuperfluousXreduction(bool allRulesSet)
        {
            List<HPTXReductionRule> rulesToCheck =
                    this.ABCDEFReductionRule.XReductionRuleList.Where(rule => rule.NumberOfRacesWithX > 0).ToList();

            int numberOfRacesWithXReduction =
                this.RaceDayInfo.RaceList
                    .SelectMany(r => r.HorseListSelected)
                    .Where(h => (int)h.Prio > 0).
                    Select(h => h.ParentRace.LegNr)
                    .Distinct()
                    .Count();

            foreach (var reductionRule in rulesToCheck)
            {
                // För högt antal ABCD
                int sumOfRestMin = rulesToCheck.Where(xr => xr != reductionRule).Sum(xr => xr.MinNumberOfX);
                if (sumOfRestMin + reductionRule.MaxNumberOfX > numberOfRacesWithXReduction)
                {
                    return true;
                }

                // För lågt antal ABCD
                if (allRulesSet)
                {
                    int sumOfRestMax = rulesToCheck.Where(xr => xr != reductionRule).Sum(xr => xr.MaxNumberOfX);
                    if (sumOfRestMax + reductionRule.MinNumberOfX < numberOfRacesWithXReduction)
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        internal void ChangeRankSize(HPTMarkBetTemplateRank markBetTemplateRank)
        {
            DateTime dtStart = DateTime.Now;
            //this.SingleRowCollection.SetRankSums(markBetTemplateRank);
            SetReductionRulesToApply();
            TimeSpan tsDuration = DateTime.Now - dtStart;

            this.MinRankSum = markBetTemplateRank.MinRankValue;
            this.MaxRankSum = markBetTemplateRank.MaxRankValue;
            RecalculateReductionThreaded(this);
            this.TemplateResultList.Add(new HPTMarkBetTemplateResult(this));

            this.MinRankSum = markBetTemplateRank.MinRankValue + 0.1M;
            this.MaxRankSum = markBetTemplateRank.MaxRankValue + 0.1M;
            RecalculateReductionThreaded(this);
            this.TemplateResultList.Add(new HPTMarkBetTemplateResult(this));

            this.MinRankSum = markBetTemplateRank.MinRankValue - 0.1M;
            this.MaxRankSum = markBetTemplateRank.MaxRankValue - 0.1M;
            RecalculateReductionThreaded(this);
            this.TemplateResultList.Add(new HPTMarkBetTemplateResult(this));

            this.MinRankSum = markBetTemplateRank.MinRankValue - 0.1M;
            this.MaxRankSum = markBetTemplateRank.MaxRankValue;
            RecalculateReductionThreaded(this);
            this.TemplateResultList.Add(new HPTMarkBetTemplateResult(this));

            this.MinRankSum = markBetTemplateRank.MinRankValue;
            this.MaxRankSum = markBetTemplateRank.MaxRankValue + 0.1M;
            RecalculateReductionThreaded(this);
            this.TemplateResultList.Add(new HPTMarkBetTemplateResult(this));

            //this.MinRankSumPercent = markBetTemplateRank.LowerPercentageLimit;
            //this.MaxRankSumPercent = markBetTemplateRank.UpperPercentageLimit;
        }

        private List<System.Collections.BitArray> GetABCDTemplateCombinations(HPTPrio prio, int numberOfRacesWithX)
        {
            string[] abcdCombinations = null;

            switch (numberOfRacesWithX)
            {
                case 0:
                    break;
                case 1:
                    abcdCombinations = new string[] { "01", "11" };
                    break;
                case 2:
                    abcdCombinations = new string[] { "110", "011", "111", "010" };
                    break;
                case 3:
                    switch (prio)
                    {
                        case HPTPrio.A:
                            abcdCombinations = new string[] { "0110", "0100", "0010", "0111", "0011" };
                            break;
                        case HPTPrio.B:
                            abcdCombinations = new string[] { "0110", "1100", "0100", "0010", "1110", "0111", "0011" };
                            break;
                        case HPTPrio.C:
                        case HPTPrio.D:
                        case HPTPrio.E:
                        case HPTPrio.F:
                            abcdCombinations = new string[] { "1100", "1110", "0110", "0100" };
                            break;
                    }
                    abcdCombinations = new string[] { "110", "011", "111", "010" };
                    break;
                case 4:
                    switch (prio)
                    {
                        case HPTPrio.A:
                            //abcdCombinations = new string[] { "01100", "00110", "01110", "01111", "00111", "00011", "01000", "00100", "00010" };
                            abcdCombinations = new string[] { "01100", "00110", "01110", "01111", "00111", "00011" };
                            break;
                        case HPTPrio.B:
                            //abcdCombinations = new string[] { "01100", "00110", "11000", "01110", "01111", "00111", "00011", "01000", "00100", "00010", "11100" };
                            abcdCombinations = new string[] { "01100", "00110", "11000", "01110", "01111", "00111", "00011", "11100" };
                            break;
                        case HPTPrio.C:
                        case HPTPrio.D:
                        case HPTPrio.E:
                        case HPTPrio.F:
                            //abcdCombinations = new string[] { "11000", "01100", "11100", "01110", "00110", "01000", "00100" };
                            abcdCombinations = new string[] { "11000", "01100", "11100", "01110", "00110" };
                            break;
                    }
                    break;
                case 5:
                    switch (prio)
                    {
                        case HPTPrio.A:
                            //abcdCombinations = new string[] { "001100", "001110", "000110", "001111", "000111", "000011", "011100", "011110", "011000", "001000", "000100", "000010" };
                            abcdCombinations = new string[] { "001100", "001110", "000110", "001111", "000111", "000011", "011100", "011110", "011000" };
                            break;
                        case HPTPrio.B:
                            abcdCombinations = new string[] { "011100", "011110", "011000", "001100", "001110", "000110" };
                            //abcdCombinations = new string[] { "011100", "011110", "011000", "001100", "001110", "000110", "001111", "000111", "000011", "001000", "000100", "000010" };
                            break;
                        case HPTPrio.C:
                        case HPTPrio.D:
                        case HPTPrio.E:
                        case HPTPrio.F:
                            //abcdCombinations = new string[] { "110000", "111000", "011000", "011100", "111100", "011110", "010000", "001000", "000100" };
                            abcdCombinations = new string[] { "110000", "111000", "011000", "011100" };
                            break;
                    }
                    break;
                case 6:
                case 7:
                case 8:
                    switch (prio)
                    {
                        case HPTPrio.A:
                            //abcdCombinations = new string[] { "001100", "001110", "000110", "001111", "000111", "000011", "011100", "011110", "011000", "001000", "000100", "000010" };
                            abcdCombinations = new string[] { "001100", "001110", "000110", "000111", "000011" };
                            break;
                        case HPTPrio.B:
                            abcdCombinations = new string[] { "011100", "011000", "001100", "001110", "000110", "000111", "000011" };
                            //abcdCombinations = new string[] { "011100", "011110", "011000", "001100", "001110", "000110", "001111", "000111", "000011", "001000", "000100", "000010" };
                            break;
                        case HPTPrio.C:
                        case HPTPrio.D:
                        case HPTPrio.E:
                        case HPTPrio.F:
                            //abcdCombinations = new string[] { "110000", "111000", "011000", "011100", "111100", "011110", "010000", "001000", "000100" };
                            abcdCombinations = new string[] { "110000", "111000", "011000", "011100", "001100", "001110" };
                            break;
                    }
                    break;
                default:
                    break;
            }

            return CreateBitArrayListFromStringArray(abcdCombinations);
        }

        private static SortedList<int, List<System.Collections.BitArray>> ABCDTemplateCombinations;
        private void InitiateABCDTemplateCombinations()
        {
            ABCDTemplateCombinations = new SortedList<int, List<System.Collections.BitArray>>(7);

            string[] abcdCombinations1 = new string[] { "01", "11" };
            string[] abcdCombinations2 = new string[] { "110", "011", "111", "010" };
            string[] abcdCombinations3 = new string[] { "0110", "1100", "0100", "0010", "1110", "0111", "0011" };
            string[] abcdCombinations4 = new string[] { "01100", "00110", "11000", "01110", "01111", "00111", "00011", "01000", "00100", "00010", "11100", "00001" };
            string[] abcdCombinations5 = new string[] { "001100", "001110", "000110", "001111", "000111", "000011", "011000", "011100", "011110", "001000", "000100", "000010", "000001" };
            //string[] abcdCombinations6 = new string[] { "1100000", "0100000", "1110000", "0110000", "0010000", "1111000", "0111000", "0011000", "0001000", "1111100", "0111100", "0011100", "0001100", "0000100", "1111110", "0111110", "0011110", "0001110", "0000110", "0000010", "0111111", "0011111", "0001111", "0000111", "0000011", "0000001" };

            ABCDTemplateCombinations.Add(1, CreateBitArrayListFromStringArray(abcdCombinations1));
            ABCDTemplateCombinations.Add(2, CreateBitArrayListFromStringArray(abcdCombinations2));
            ABCDTemplateCombinations.Add(3, CreateBitArrayListFromStringArray(abcdCombinations3));
            ABCDTemplateCombinations.Add(4, CreateBitArrayListFromStringArray(abcdCombinations4));
            ABCDTemplateCombinations.Add(5, CreateBitArrayListFromStringArray(abcdCombinations5));
            //ABCDTemplateCombinations.Add(6, CreateBitArrayListFromStringArray(abcdCombinations6));
        }

        private static List<System.Collections.BitArray> CreateBitArrayListFromStringArray(string[] stringArray)
        {
            List<System.Collections.BitArray> baList = new List<System.Collections.BitArray>();
            foreach (string s in stringArray)
            {
                baList.Add(ConvertStringToBitArray(s));
            }
            return baList;
        }

        private static System.Collections.BitArray ConvertStringToBitArray(string byteArray)
        {
            System.Collections.BitArray ba = new System.Collections.BitArray(byteArray.Length, false);
            char[] ca = byteArray.ToCharArray();
            for (int i = 0; i < ca.Length; i++)
            {
                if (ca[i] == '1')
                {
                    ba[i] = true;
                }
            }
            return ba;
        }

        #endregion

        internal void SaveFiles()
        {
            PrepareForSave();
            this.CouponCorrector.CouponHelper.CreateATGFile();
            HPTSerializer.SerializeHPTSystem(this.MailSender.HPT3FileName, this);
        }

        internal void PrepareForSave()
        {
            CheckForWarnings();

            // Varna för att det är fler kuponger än tillåtet
            HandleTooManyCoupons();

            string fileName = this.SaveDirectory + ToFileNameString();
            SystemFilename = fileName + ".xml";
            //Clipboard.SetText(this.MarkBet.SystemFilename);
            string hpt3Filename = fileName + ".hpt5";
            MailSender.HPT3FileName = hpt3Filename;
            SetSerializerValues();
        }

        private DateTime lastSaveTime;
        [DataMember]
        public DateTime LastSaveTime
        {
            get
            {
                return lastSaveTime;
            }
            set
            {
                lastSaveTime = value;
                OnPropertyChanged("LastSaveTime");
                if (value == DateTime.MinValue)
                {
                    this.LastSaveString = "Ej sparad";
                }
                else
                {
                    this.LastSaveString = "Sparad " + value.ToString("H:mm"); ;
                }
            }
        }

        private string lastSaveString;
        [XmlIgnore]
        public string LastSaveString
        {
            get
            {
                if (string.IsNullOrEmpty(this.lastSaveString))
                {
                    return "Ej sparad";
                }
                return lastSaveString;
            }
            set
            {
                lastSaveString = value;
                OnPropertyChanged("LastSaveString");
            }
        }

        #region Hantera inklistrade tips

        internal bool ParseTips(string tips)
        {
            var parserHptABC = new HPTTipsParserHptABC();
            if (parserHptABC.IsTips(tips))
            {
                parserHptABC.ParseTips(tips, this);
                return false;
                //return parserHptABC.ParseTips(tips, this);
            }

            var parserAftonbladet = new HPTTipsParserAftonbladet();
            if (parserAftonbladet.IsTips(tips))
            {
                return parserAftonbladet.ParseTips(tips, this);
            }

            var parserStandard1 = new HPTTipsParserStandard1();
            if (parserStandard1.IsTips(tips))
            {
                return parserStandard1.ParseTips(tips, this);
            }

            var parserStandard2 = new HPTTipsParserStandard2();
            if (parserStandard2.IsTips(tips))
            {
                return parserStandard2.ParseTips(tips, this);
            }
            return false;
        }

        #endregion

        #region Obsolete/Not in use


        //#region Undo/Redo

        //private bool UndoInProgress;
        //private bool RedoInProgress;
        //private LinkedList<PropertySetterInfo> ChangedValuesQueue;
        //private LinkedListNode<PropertySetterInfo> CurrentPropertySetter;
        //void HPTMarkBet_ValueChanged(PropertySetterInfo psi)
        //{
        //    if (!this.UndoInProgress && !this.RedoInProgress)
        //    {
        //        LinkedListNode<PropertySetterInfo> node = this.ChangedValuesQueue.AddLast(psi);
        //        this.CurrentPropertySetter = node;
        //        this.UndoPossible = true;
        //    }
        //}

        ////public PropertySetterInfo Undo()
        //public void Undo()
        //{
        //    if (this.CurrentPropertySetter == null)
        //        return;
        //    this.UndoInProgress = true;
        //    PropertySetterInfo psi = this.CurrentPropertySetter.Value;
        //    psi.ClassType.GetProperty(psi.PropertyName).SetValue(psi.ClassInstance, psi.OldValue, null);
        //    this.RedoPossible = this.CurrentPropertySetter != null;
        //    this.CurrentPropertySetter = this.CurrentPropertySetter.Previous;
        //    this.UndoInProgress = false;
        //    this.UndoPossible = this.CurrentPropertySetter != null;
        //    //return this.CurrentPropertySetter.Value;
        //}

        ////public PropertySetterInfo Redo()
        //public void Redo()
        //{
        //    if (this.CurrentPropertySetter == null)
        //        return;
        //    if (this.CurrentPropertySetter.Next == null)
        //        return;
        //    this.RedoInProgress = true;
        //    this.CurrentPropertySetter = this.CurrentPropertySetter.Next;
        //    PropertySetterInfo psi = this.CurrentPropertySetter.Value;
        //    psi.ClassType.GetProperty(psi.PropertyName).SetValue(psi.ClassInstance, psi.NewValue, null);
        //    this.UndoPossible = this.CurrentPropertySetter != null;
        //    //this.CurrentPropertySetter = this.CurrentPropertySetter.Next;
        //    this.RedoInProgress = false;            
        //    this.RedoPossible = this.CurrentPropertySetter.Next != null;            
        //    //return this.CurrentPropertySetter.Value;
        //}

        //private void ActivateValueChanged(string propertyName, object oldValue, object newValue)
        //{
        //    PropertySetterInfo psi = new PropertySetterInfo();
        //    psi.PropertyName = propertyName;
        //    psi.OldValue = oldValue;
        //    psi.NewValue = newValue;
        //    psi.ClassInstance = this;
        //    psi.ClassType = typeof(HPTMarkBet);
        //    this.OnValueChanged(psi);
        //}

        //private bool undoPossible;
        //public bool UndoPossible
        //{
        //    get
        //    {
        //        return this.undoPossible;
        //    }
        //    set
        //    {
        //        this.undoPossible = value;
        //        OnPropertyChanged("UndoPossible");
        //    }
        //}

        //private bool redoPossible;
        //public bool RedoPossible
        //{
        //    get
        //    {
        //        return this.redoPossible;
        //    }
        //    set
        //    {
        //        this.redoPossible = value;
        //        OnPropertyChanged("RedoPossible");
        //    }
        //}

        //#endregion


        #endregion

        #region Hemligt system

        internal void CreateBaseSystem()
        {
            this.pauseRecalculation = true;

            ClearAll();
            foreach (var horse in this.RaceDayInfo.RaceList.SelectMany(r => r.HorseList.Where(h => h.Scratched == false || h.Scratched == null)).ToList())
            {
                horse.Selected = true;
            }
            foreach (var race in this.RaceDayInfo.RaceList)
            {
                //race.SelectAll(true);
                var horseListOrdered = race.HorseList.Where(h => h.Scratched == false || h.Scratched == null).OrderByDescending(h => h.StakeDistributionShare).ToArray();

                // Favoriten som A-Häst
                horseListOrdered[0].HorseXReductionList.First(h => h.Prio == HPTPrio.A).Selected = true;
            }

            // Sätt ABC-villkor
            var rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.A);
            rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
            rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
            rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;
            this.ABCDEFReductionRule.Use = true;

            // Sätt gruppintervall för storskrällar
            var groupRule = new HPTGroupIntervalReductionRule(this.RaceDayInfo.RaceList.Count, false);
            groupRule.HorseVariable = HPTConfig.Config.HorseVariableList.FirstOrDefault(hv => hv.PropertyName == "StakeDistributionPercent");
            groupRule.LowerBoundary = 0M;
            groupRule.UpperBoundary = 4M;
            groupRule.NumberOfWinnersList.First(now => now.NumberOfWinners == 0).Selected = true;
            groupRule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
            groupRule.Use = true;
            this.GroupIntervalRulesCollection.ReductionRuleList.Add(groupRule);
            this.GroupIntervalRulesCollection.Use = true;

            // Sätt flerbong
            this.SingleRowTargetProfit = 4000;
            this.SingleRowBetMultiplier = true;
        }

        internal void CreateBaseSystemAlt()
        {
            this.pauseRecalculation = true;

            ClearAll();
            foreach (var race in this.RaceDayInfo.RaceList)
            {
                var horseListOrdered = race.HorseList.Where(h => h.Scratched == false || h.Scratched == null).OrderByDescending(h => h.StakeDistributionShare).ToArray();

                decimal lowestRankMeanToSelect = horseListOrdered.Length * 0.7M;

                // Favoriten som A-Häst
                horseListOrdered[0].HorseXReductionList.First(h => h.Prio == HPTPrio.A).Selected = true;

                // Övriga som B- eller C-hästar
                for (int i = 1; i < horseListOrdered.Length; i++)
                {
                    var horse = horseListOrdered[i];
                    if (horse.RankMean <= lowestRankMeanToSelect)
                    {
                        if (horse.StakeDistributionShare >= 0.06M)
                        {
                            horse.HorseXReductionList.First(h => h.Prio == HPTPrio.B).Selected = true;
                        }
                        else
                        {
                            horse.HorseXReductionList.First(h => h.Prio == HPTPrio.C).Selected = true;
                        }
                    }
                }

                if (horseListOrdered[0].StakeDistributionShare - horseListOrdered[1].StakeDistributionShare < 0.02M)
                {
                    // Andrahandsfavoriten som A-Häst om diffen är extremt liten
                    horseListOrdered[1].HorseXReductionList.First(h => h.Prio == HPTPrio.A).Selected = true;
                    horseListOrdered[1].HorseXReductionList.First(h => h.Prio == HPTPrio.B).Selected = false;
                    horseListOrdered[1].HorseXReductionList.First(h => h.Prio == HPTPrio.C).Selected = false;
                }
            }

            // Uppdatera antal av varje Prio
            RecalculateNumberOfX();

            // Sätt ABC-villkor
            var ruleA = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.A);
            var ruleB = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.B);
            var ruleC = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.C);

            ruleA.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
            ruleA.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
            ruleA.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;

            ruleB = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.B);
            ruleB.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
            ruleB.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
            if (ruleB.NumberOfRacesWithX == 4)
            {
                ruleB.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;
            }
            else
            {
                ruleC.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
            }

            ruleC = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.C);
            ruleC.NumberOfWinnersList.First(now => now.NumberOfWinners == 0).Selected = true;
            ruleC.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;

            this.ABCDEFReductionRule.Use = true;

            // Sätt regel för ranksumma
            this.SingleRowCollection.HandleHighestAndLowestSums();
            this.MinRankSum = this.SingleRowCollection.MinRankSum * 1.2M;
            this.MaxRankSum = this.SingleRowCollection.MinRankSum < 10M ? 20M : this.SingleRowCollection.MinRankSum * 2M;
            this.ReductionRank = true;

            // Skapa regel för radvärde
            this.RowValueReductionRule.MaxSum = 30000;
            this.RowValueReductionRule.Use = true;

            //// Sätt gruppintervall för storskrällar
            //var groupRule = new HPTGroupIntervalReductionRule(this.RaceDayInfo.RaceList.Count, false);
            //groupRule.HorseVariable = HPTConfig.Config.HorseVariableList.FirstOrDefault(hv => hv.PropertyName == "StakeDistributionPercent");
            //groupRule.LowerBoundary = 0M;
            //groupRule.UpperBoundary = 4M;
            //groupRule.NumberOfWinnersList.First(now => now.NumberOfWinners == 0).Selected = true;
            //groupRule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
            //groupRule.Use = true;
            //this.GroupIntervalRulesCollection.ReductionRuleList.Add(groupRule);
            //this.GroupIntervalRulesCollection.Use = true;

            // Sätt flerbong
            this.SingleRowTargetProfit = 4000;
            this.SingleRowBetMultiplier = true;
        }

        // Alternativ 1
        internal void CreateInternalSystemV4PlaceTotal()
        {
            // Skapa grundsystemet
            CreateBaseSystem();

            // Sätt rankreduceringsintervall för platsandel totalt
            var rule = this.HorseRankSumReductionRuleList.FirstOrDefault(hrr => hrr.PropertyName == "PercentFirstPlaceTotal");
            rule.MinSum = 7;
            rule.MaxSum = 18;
            rule.Use = true;
            this.ReductionHorseRank = true;

            this.pauseRecalculation = false;
            RecalculateReduction(RecalculateReason.All);
        }

        // Alternativ 3
        internal void CreateInternalSystemV4EarningsThisYear()
        {
            // Skapa grundsystemet
            CreateBaseSystem();

            // Sätt rankreduceringsintervall för platsandel totalt
            var rule = this.HorseRankSumReductionRuleList.FirstOrDefault(hrr => hrr.PropertyName == "EarningsMeanThisYear");
            rule.MinSum = 4;
            rule.MaxSum = 14;
            rule.Use = true;
            this.ReductionHorseRank = true;

            this.pauseRecalculation = false;
            RecalculateReduction(RecalculateReason.All);
        }

        // Alternativ 2
        internal void CreateInternalSystemV4PlaceThisYear()
        {
            // Skapa grundsystemet
            CreateBaseSystem();

            // Sätt rankreduceringsintervall för platsandel totalt
            var rule = this.HorseRankSumReductionRuleList.FirstOrDefault(hrr => hrr.PropertyName == "PercentFirstPlaceThisYear");
            rule.MinSum = 4;
            rule.MaxSum = 14;
            rule.Use = true;
            this.ReductionHorseRank = true;

            this.pauseRecalculation = false;
            RecalculateReduction(RecalculateReason.All);
        }

        // Alternativ 4
        internal void CreateInternalSystemV4CombinedRankingThisYear()
        {
            // Skapa grundsystemet
            CreateBaseSystem();

            // Sätt rankreduceringsintervall för vinstandel totalt, intjänat i år och vinstandel totalt
            SetRankSum("PercentTop3ThisYear", 4, 18);
            //SetRankSum("PercentFirstPlaceThisYear", 4, 22);
            SetRankSum("EarningsMeanThisYear", 4, 16);
            this.ReductionHorseRank = true;

            this.pauseRecalculation = false;
            RecalculateReduction(RecalculateReason.All);
        }

        // Alternativ 5
        internal void CreateInternalSystemV4CombinedRankingLastYear()
        {
            // Skapa grundsystemet
            CreateBaseSystem();

            // Sätt rankreduceringsintervall för vinstandel totalt, intjänat i år och vinstandel totalt
            SetRankSum("PercentTop3LastYear", 4, 20);
            SetRankSum("PercentFirstPlaceLastYear", 4, 20);
            SetRankSum("EarningsMeanLastYear", 4, 20);
            this.ReductionHorseRank = true;

            this.pauseRecalculation = false;
            RecalculateReduction(RecalculateReason.All);
        }

        // Alternativ 6
        internal void CreateInternalSystemV4CombinedRankingTotal()
        {
            // Skapa grundsystemet
            CreateBaseSystem();

            // Sätt rankreduceringsintervall för vinstandel totalt, intjänat i år och vinstandel totalt
            SetRankSum("PercentTop3Total", 4, 20);
            SetRankSum("PercentFirstPlaceTotal", 4, 20);
            SetRankSum("EarningsMeanTotal", 4, 20);
            this.ReductionHorseRank = true;

            this.pauseRecalculation = false;
            RecalculateReduction(RecalculateReason.All);
        }

        // Alternativ 7
        internal void CreateInternalSystemV4StakeDistributionRanking()
        {
            // Skapa grundsystemet
            CreateBaseSystem();

            // Sätt rankreduceringsintervall för vinstandel totalt, intjänat i år och vinstandel totalt
            SetRankSum("StakeDistributionShare", 6, 17);
            SetRankSum("MaxPlatsOdds", 6, 17);
            //SetRankSum("PercentTop3ThisYear", 6, 18);
            SetRankSum("EarningsMeanThisYear", 6, 17);
            this.ReductionHorseRank = true;

            this.pauseRecalculation = false;
            RecalculateReduction(RecalculateReason.All);
        }

        // Alternativ 7
        internal void CreateInternalSystemV4ABC()
        {
            // Skapa grundsystemet
            CreateBaseSystemAlt();

            //// Sätt rankreduceringsintervall för vinstandel totalt, intjänat i år och vinstandel totalt
            //SetRankSum("StakeDistributionShare", 6, 20);
            //SetRankSum("MaxPlatsOdds", 6, 20);
            //this.ReductionHorseRank = true;

            this.pauseRecalculation = false;
            RecalculateReduction(RecalculateReason.All);
        }

        internal void SetRankSum(string propertyName, int minSum, int maxSum)
        {
            try
            {
                var rule = this.HorseRankSumReductionRuleList.FirstOrDefault(hrr => hrr.PropertyName == propertyName);
                rule.MinSum = minSum;
                rule.MaxSum = maxSum;
                rule.Use = true;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        internal void CreateInternalSystemV4Old()
        {
            this.pauseRecalculation = true;

            ClearAll();
            foreach (var race in this.RaceDayInfo.RaceList)
            {
                var horseListOrdered = race.HorseList.Where(h => h.Scratched == false || h.Scratched == null).OrderByDescending(h => h.StakeDistributionShare).ToArray();

                // Favoriten som A-Häst
                horseListOrdered[0].HorseXReductionList.First(h => h.Prio == HPTPrio.A).Selected = true;

                // Variabler för att hålla ordning på looparna
                bool selectionFinished = false;
                int stakeRank = 1;

                // Välj B-hästar
                while (!selectionFinished)
                {
                    var horseToSelect = horseListOrdered[stakeRank];
                    if (horseToSelect.StakeDistributionShareAccumulated > 0.7M)
                    {
                        selectionFinished = true;
                    }
                    horseToSelect.HorseXReductionList.First(h => h.Prio == HPTPrio.B).Selected = true;
                    stakeRank++;
                }

                // Välj C-hästar
                selectionFinished = false;
                while (!selectionFinished)
                {
                    var horseToSelect = horseListOrdered[stakeRank];
                    if (horseToSelect.StakeDistributionShareAccumulated > 0.9M)
                    {
                        selectionFinished = true;
                    }
                    horseToSelect.HorseXReductionList.First(h => h.Prio == HPTPrio.C).Selected = true;
                    stakeRank++;
                }
            }

            // Sätt ABC-villkor
            var rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.A);
            rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
            rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
            rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;

            rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.B);
            rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
            rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
            rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;

            rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.C);
            rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 0).Selected = true;
            rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;

            this.ABCDEFReductionRule.Use = true;

            // Sätt flerbong
            this.SingleRowTargetProfit = 3000;
            this.SingleRowBetMultiplier = true;

            this.pauseRecalculation = false;
            RecalculateReduction(RecalculateReason.All);
        }

        internal void CreateInternalSystem()
        {
            switch (this.BetType.Code)
            {
                case "V4":
                    CreateInternalSystemV4();
                    break;
                case "V64":
                    CreateInternalSystemV64();
                    break;
                default:
                    break;
            }
        }

        internal void CreateInternalSystemV4()
        {
            this.pauseRecalculation = true;

            try
            {
                ClearAll();
                foreach (var race in this.RaceDayInfo.RaceList)
                {
                    var horseListOrdered = race.HorseList
                        .Where(h => h.Scratched == false || h.Scratched == null)
                        .Where(h => h.StakeDistributionShare > 0.1M)
                        .OrderByDescending(h => h.StakeDistributionShare)
                        .ToArray();

                    // 1 A-, 3 B- och 2 C-hästar
                    horseListOrdered[0].HorseXReductionList.First(h => h.Prio == HPTPrio.A).Selected = true;
                    for (int i = 1; i < horseListOrdered.Length; i++)
                    {
                        horseListOrdered[i].HorseXReductionList.First(h => h.Prio == HPTPrio.B).Selected = true;
                    }

                    var cHorseList = race.HorseList
                        .Where(h => h.Scratched == false || h.Scratched == null)
                        .Where(h => h.RankMean < 6.0M && !h.Selected);

                    foreach (var cHorse in cHorseList)
                    {
                        cHorse.HorseXReductionList.First(h => h.Prio == HPTPrio.C).Selected = true;
                    }

                    //horseListOrdered[1].HorseXReductionList.First(h => h.Prio == HPTPrio.B).Selected = true;
                    //horseListOrdered[2].HorseXReductionList.First(h => h.Prio == HPTPrio.B).Selected = true;
                    //horseListOrdered[3].HorseXReductionList.First(h => h.Prio == HPTPrio.B).Selected = true;
                    //horseListOrdered[4].HorseXReductionList.First(h => h.Prio == HPTPrio.C).Selected = true;
                    //horseListOrdered[5].HorseXReductionList.First(h => h.Prio == HPTPrio.C).Selected = true;
                }

                // Sätt ABC-villkor
                var rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.A);
                rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
                rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
                rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;

                rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.B);
                rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
                rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
                rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;

                rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.C);
                rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 0).Selected = true;
                rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;

                this.ABCDEFReductionRule.Use = true;

                // Sätt flerbong
                this.SingleRowTargetProfit = 1000;
                this.SingleRowBetMultiplier = true;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }

            this.pauseRecalculation = false;
            RecalculateReduction(RecalculateReason.All);
        }

        internal void CreateInternalSystemV64()
        {
            this.pauseRecalculation = true;

            try
            {
                ClearAll();

                var allHorses = this.RaceDayInfo.RaceList
                        .SelectMany(r => r.HorseList)
                        .Where(h => h.Scratched == false || h.Scratched == null)
                        .Where(h => h.StakeDistributionShare > 0.1M || h.RankMean < 5.0M)
                        .OrderByDescending(h => h.StakeDistributionShare);

                // var rank = this.RankList.FirstOrDefault(hr => hr.Name == "StakeDistributionShare");
                var abHorses = allHorses
                    .Where(h => h.StakeDistributionShare > 0.2M || h.RankList.First(hr => hr.Name == "StakeDistributionShare").Rank == 1);
                //.Where(h => h.RankList.First(hr => hr.Name == "StakeDistributionShare").Rank == 1);

                var aHorses = abHorses.Where(h => h.StakeDistributionShare > 0.4M);
                if (aHorses.Count() >= 2)   // Minst två storfavoriter
                {
                    aHorses
                        .OrderByDescending(h => h.StakeDistributionShare)
                        .Take(2)
                        .ToList()
                        .ForEach(h =>
                        {
                            h.HorseXReductionList.First(hr => hr.Prio == HPTPrio.A).Selected = true;
                        });

                    abHorses
                        .ToList()
                        .ForEach(h =>
                        {
                            if (!h.Selected)
                            {
                                h.HorseXReductionList.First(hr => hr.Prio == HPTPrio.B).Selected = true;
                            }
                        });

                    allHorses
                        .Except(abHorses)
                        .ToList()
                        .ForEach(h =>
                    {
                        if (h.StakeDistributionShare > 0.1M)
                        {
                            h.HorseXReductionList.First(hr => hr.Prio == HPTPrio.C).Selected = true;
                        }
                        else
                        {
                            h.HorseXReductionList.First(hr => hr.Prio == HPTPrio.D).Selected = true;
                        }
                    });

                    // Sätt ABCD-villkor
                    var rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.A);
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;

                    int numberOfB = allHorses.Count(h => h.Prio == HPTPrio.B);
                    rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.B);
                    if (numberOfB < 6)
                    {
                        rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
                    }
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;
                    if (numberOfB > 9)
                    {
                        rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 4).Selected = true;
                    }

                    int numberOfC = allHorses.Count(h => h.Prio == HPTPrio.C);
                    rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.C);
                    if (allHorses.Count(h => h.Prio == HPTPrio.C) < 9)
                    {
                        rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
                    }
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;

                    rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.D);
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 0).Selected = true;
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
                }
                else    // Inte två storfavoriter
                {
                    abHorses
                        .Where(h => h.RankList.First(hr => hr.Name == "StakeDistributionShare").Rank == 1)
                        .ToList()
                        .ForEach(h =>
                        {
                            h.HorseXReductionList.First(hr => hr.Prio == HPTPrio.B).Selected = true;
                        });

                    allHorses.Except(abHorses).ToList().ForEach(h =>
                    {
                        if (h.StakeDistributionShare > 0.1M)
                        {
                            h.HorseXReductionList.First(hr => hr.Prio == HPTPrio.B).Selected = true;
                        }
                        else
                        {
                            h.HorseXReductionList.First(hr => hr.Prio == HPTPrio.C).Selected = true;
                        }
                    });

                    // Sätt ABC-villkor
                    var rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.A);
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;

                    int numberOfB = allHorses.Count(h => h.Prio == HPTPrio.B);
                    rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.B);
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;

                    int numberOfC = allHorses.Count(h => h.Prio == HPTPrio.C);
                    rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.C);
                    if (numberOfC < 7)
                    {
                        rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 0).Selected = true;
                    }
                    rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
                    if (numberOfC > 6)
                    {
                        rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
                    }
                }

                // Använd villkoret
                this.ABCDEFReductionRule.Use = true;

                // Sätt V6
                this.V6UpperBoundary = 1000;
                this.V6SingleRows = true;

                // Sätt målvinst
                this.SingleRowTargetProfit = 2000;
                this.SingleRowBetMultiplier = true;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }

            this.pauseRecalculation = false;
            RecalculateReduction(RecalculateReason.All);
        }

        //internal void CreateInternalSystemV64()
        //{
        //    this.pauseRecalculation = true;

        //    try
        //    {
        //        ClearAll();
        //        foreach (var race in this.RaceDayInfo.RaceList)
        //        {
        //            var horseListOrdered = race.HorseList
        //                .Where(h => h.Scratched == false || h.Scratched == null)
        //                .Where(h => h.StakeDistributionShare > 0.1M)
        //                .OrderByDescending(h => h.StakeDistributionShare)
        //                .ToArray();

        //            // 1 A-, 3 B- och 2 C-hästar
        //            horseListOrdered[0].HorseXReductionList.First(h => h.Prio == HPTPrio.A).Selected = true;
        //            for (int i = 1; i < horseListOrdered.Length; i++)
        //            {
        //                horseListOrdered[i].HorseXReductionList.First(h => h.Prio == HPTPrio.B).Selected = true;
        //            }

        //            var cHorseList = race.HorseList
        //                .Where(h => h.Scratched == false || h.Scratched == null)
        //                .Where(h => h.RankMean < 6.0M && !h.Selected);

        //            foreach (var cHorse in cHorseList)
        //            {
        //                cHorse.HorseXReductionList.First(h => h.Prio == HPTPrio.C).Selected = true;
        //            }

        //            //horseListOrdered[1].HorseXReductionList.First(h => h.Prio == HPTPrio.B).Selected = true;
        //            //horseListOrdered[2].HorseXReductionList.First(h => h.Prio == HPTPrio.B).Selected = true;
        //            //horseListOrdered[3].HorseXReductionList.First(h => h.Prio == HPTPrio.B).Selected = true;
        //            //horseListOrdered[4].HorseXReductionList.First(h => h.Prio == HPTPrio.C).Selected = true;
        //            //horseListOrdered[5].HorseXReductionList.First(h => h.Prio == HPTPrio.C).Selected = true;
        //        }

        //        // Sätt ABC-villkor
        //        var rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.A);
        //        //rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
        //        rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
        //        rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;

        //        rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.B);
        //        //rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
        //        rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
        //        rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 3).Selected = true;

        //        rule = this.ABCDEFReductionRule.XReductionRuleList.First(rr => rr.Prio == HPTPrio.C);
        //        rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 0).Selected = true;
        //        rule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;

        //        this.ABCDEFReductionRule.Use = true;

        //        // Sätt V6
        //        this.V6UpperBoundary = 1000;
        //        this.V6SingleRows = true;

        //        // Sätt flerbong
        //        this.SingleRowTargetProfit = 3000;
        //        this.SingleRowBetMultiplier = true;
        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //    }

        //    this.pauseRecalculation = false;
        //    RecalculateReduction(RecalculateReason.All);
        //}

        #endregion

        #region Beräkningar till Villkorsstatistik-fliken

        internal string CalculateDifficulty()
        {
            try
            {
                ClearAll();

                var currentHorseList = this.RaceDayInfo.RaceList
                    .Select(r => r.HorseList
                        .Where(h => h.Scratched == false || h.Scratched == null)
                        .OrderByDescending(h => h.StakeDistributionShare)
                        .First())
                    .ToDictionary(h => h.ParentRace.LegNr);

                var allHorses = this.RaceDayInfo.RaceList
                            .SelectMany(r => r.HorseList)
                            .Where(h => h.Scratched == false || h.Scratched == null)
                            .OrderByDescending(h => h.StakeDistributionShare);

                var remainingHorses = allHorses
                    .Except(currentHorseList.Values)
                    .ToList();

                var payOutSizeList = new List<Tuple<int, int, int>>();

                int maxPayOut = 0;
                int numberOfHorses = currentHorseList.Count;
                while (maxPayOut < 10000)
                {
                    var singleRow = new HPTMarkBetSingleRow(currentHorseList.Values.ToArray());
                    singleRow.CalculateValues();
                    singleRow.EstimateRowValue(this);
                    maxPayOut = singleRow.RowValue;

                    // Systemstorlek
                    int systemSize = currentHorseList.Values
                        .Select(h => h.RankList.First(hr => hr.Name == "StakeDistributionShare").Rank)
                        .Aggregate((r, next) => r * next);

                    // Plocka ut nästa häst
                    var nextHorse = remainingHorses.First();
                    currentHorseList[nextHorse.ParentRace.LegNr] = nextHorse;
                    remainingHorses.Remove(nextHorse);

                    // Lägg till objektet i listan
                    payOutSizeList.Add(new Tuple<int, int, int>(numberOfHorses, systemSize, maxPayOut));

                    // Räkna upp antalet hästar
                    numberOfHorses++;
                }
                //payOutSizeList.ToString();
                var sb = new StringBuilder();
                payOutSizeList.ForEach(po =>
                {
                    sb.Append(po.Item1);
                    sb.Append("\t");
                    sb.Append(po.Item2);
                    sb.Append("\t");
                    sb.Append(po.Item3);
                    sb.AppendLine();
                });
                return sb.ToString();
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }

        internal Dictionary<int, HPTHorse> calculationHorseDictionary;
        internal Dictionary<int, int> numberOfRowsUnderRowValue;
        internal string CalculateDifficultyAlt()
        {
            try
            {
                this.numberOfRowsUnderRowValue = new Dictionary<int, int>();
                Enumerable.Range(1, 50).ToList().ForEach(i =>
                {
                    this.numberOfRowsUnderRowValue.Add(i * 1000, 0);
                });

                this.calculationHorseDictionary = this.RaceDayInfo.RaceList
                    .Select(r => r.HorseList
                        .Where(h => h.Scratched == false || h.Scratched == null)
                        .OrderByDescending(h => h.StakeDistributionShare)
                        .First())
                    .ToDictionary(h => h.ParentRace.LegNr);

                CalculateDifficultyAlt(1);

                // Skapa sträng som går att se i Excel
                var sb = new StringBuilder();
                sb.AppendLine(this.RaceDayInfo.RaceDayDateShortString);

                this.numberOfRowsUnderRowValue
                    .Keys
                    .ToList()
                    .ForEach(k =>
                    {
                        //sb.Append("Antal under\t");
                        //sb.Append(k);
                        //sb.Append("\t");
                        sb.Append(this.numberOfRowsUnderRowValue[k]);
                        sb.AppendLine();
                    });

                return sb.ToString();
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }

        internal void CalculateDifficultyAlt(int legNr)
        {
            this.RaceDayInfo.RaceList
                .First(r => r.LegNr == legNr)
                .HorseList
                .OrderBy(h => h.RankList.First(hr => hr.Name == "StakeDistributionShare").Rank)
                .ToList()
                .ForEach(h =>
                {
                    this.calculationHorseDictionary[legNr] = h;
                    if (legNr < this.NumberOfRaces)
                    {
                        CalculateDifficultyAlt(legNr + 1);
                    }
                    else
                    {
                        var singleRow = new HPTMarkBetSingleRow(this.calculationHorseDictionary.Values.ToArray());
                        singleRow.CalculateValues();
                        singleRow.EstimateRowValue(this);
                        int adjustedRowValue = Convert.ToInt32(singleRow.RowValue / this.RaceDayInfo.JackpotFactor);
                        if (adjustedRowValue < 50000)
                        {
                            this.numberOfRowsUnderRowValue
                                .Keys
                                .ToList()
                                .ForEach(k =>
                                {
                                    if (adjustedRowValue < k)
                                    {
                                        this.numberOfRowsUnderRowValue[k] += 1;
                                    }
                                });
                        }
                        else
                        {
                            return;
                        }
                    }
                });
        }

        internal string CalculateJackpotRows()
        {
            try
            {
                switch (this.BetType.Code)
                {
                    case "V65":
                        this.numberOfRowsUnderRowValue = new Dictionary<int, int>()
                            {
                                {1, 0}
                            };
                        break;
                    case "V64":
                    case "V75":
                    case "GS75":
                    case "V86":
                        this.numberOfRowsUnderRowValue = new Dictionary<int, int>()
                            {
                                {1, 0},
                                {2, 0}
                            };
                        break;
                    case "V85":
                        this.numberOfRowsUnderRowValue = new Dictionary<int, int>()
                            {
                                {1, 0},
                                {2, 0},
                                {3, 0}
                            };
                        break;
                    default:
                        return string.Empty;
                }

                this.calculationHorseDictionary = this.RaceDayInfo.RaceList
                    .Select(r => r.HorseList
                        .Where(h => h.Scratched == false || h.Scratched == null)
                        .OrderByDescending(h => h.StakeDistributionShare)
                        .First())
                        .ToDictionary(h => h.ParentRace.LegNr);

                this.jackpotProbability = 0M;

                CalculateJackpotRows(1);

                // Skapa sträng som går att se i Excel
                var sb = new StringBuilder();
                sb.Append("Risk för jackpott:\t\t");
                sb.AppendLine(this.jackpotProbability.ToString("P2"));

                this.numberOfRowsUnderRowValue
                    .Keys
                    .ToList()
                    .ForEach(k =>
                    {
                        sb.Append("Jackpottrader vid ");
                        sb.Append(k);
                        sb.Append(" fel:\t");
                        //sb.Append(this.numberOfRowsUnderRowValue[k]);
                        sb.Append(string.Format("{0:### ##0}", this.numberOfRowsUnderRowValue[k]));
                        sb.AppendLine();
                    });

                this.JackpotProbability = this.jackpotProbability;
                this.JackpotRowsOneError = this.numberOfRowsUnderRowValue[1];
                if (this.numberOfRowsUnderRowValue.Count > 1)
                {
                    this.JackpotRowsTwoErrors = this.numberOfRowsUnderRowValue[2];
                }

                return sb.ToString();
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }

        internal bool CalculateJackpotRows(int legNr)
        {
            var horsesInOrder = this.RaceDayInfo.RaceList
                    .First(r => r.LegNr == legNr)
                    .HorseList
                    .Where(h => h.Scratched == false || h.Scratched == null)
                    .OrderByDescending(h => h.StakeDistributionShare)
                    .ToList();
            try
            {
                foreach (var h in horsesInOrder)
                {
                    this.calculationHorseDictionary[legNr] = h;
                    if (legNr < this.NumberOfRaces)
                    {
                        // Kolla om det är någon mening att ens anropa rekursivt
                        var singleRow = new HPTMarkBetSingleRow(this.calculationHorseDictionary.Values.ToArray());
                        singleRow.CalculateValues();

                        int calculatedPayout = 0;
                        if (this.numberOfRowsUnderRowValue.Count == 1)  // V65
                        {
                            calculatedPayout = this.CouponCorrector.CalculatePayOutOneError(singleRow.HorseList, this.RaceDayInfo.BetType.PoolShareOneError * this.RaceDayInfo.BetType.RowCost);
                        }
                        else
                        {
                            calculatedPayout = this.CouponCorrector.CalculatePayOutTwoErrors(singleRow.HorseList, this.RaceDayInfo.BetType.PoolShareTwoErrors * this.RaceDayInfo.BetType.RowCost);
                        }

                        if (calculatedPayout > this.RaceDayInfo.BetType.JackpotLimit)
                        {
                            this.calculationHorseDictionary[legNr] = horsesInOrder.First();
                            return false;
                        }

                        CalculateJackpotRows(legNr + 1);
                    }
                    else
                    {
                        var singleRow = new HPTMarkBetSingleRow(this.calculationHorseDictionary.Values.ToArray());
                        singleRow.CalculateValues();

                        int oneErrorPayout = this.CouponCorrector.CalculatePayOutOneError(singleRow.HorseList, this.RaceDayInfo.BetType.PoolShareOneError * this.RaceDayInfo.BetType.RowCost);
                        if (oneErrorPayout < this.RaceDayInfo.BetType.JackpotLimit)
                        {
                            this.numberOfRowsUnderRowValue[1] += 1;
                            if (this.numberOfRowsUnderRowValue.Count > 1)
                            {
                                this.numberOfRowsUnderRowValue[2] += 1;
                            }
                            this.jackpotProbability += singleRow.RowShareStake;
                        }
                        else if (this.numberOfRowsUnderRowValue.Count > 1)
                        {
                            int twoErrorsPayout = this.CouponCorrector.CalculatePayOutTwoErrors(singleRow.HorseList, this.RaceDayInfo.BetType.PoolShareTwoErrors * this.RaceDayInfo.BetType.RowCost);
                            if (twoErrorsPayout < this.RaceDayInfo.BetType.JackpotLimit)
                            {
                                this.numberOfRowsUnderRowValue[2] += 1;
                                this.jackpotProbability += singleRow.RowShareStake;
                            }
                            else
                            {
                                this.calculationHorseDictionary[legNr] = horsesInOrder.First();
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            this.calculationHorseDictionary[legNr] = horsesInOrder.First();
            return true;
        }

        int numberOfSingleRows = 0;
        int totalNumberOfGambledRows = 10000000;
        internal int CalculateNumberOfSingleRows()
        {
            try
            {
                this.numberOfSingleRows = 0;

                this.calculationHorseDictionary = this.RaceDayInfo.RaceList
                    .Select(r => r.HorseList
                        .Where(h => h.Scratched == false || h.Scratched == null)
                        .OrderBy(h => h.StakeDistributionShare)
                        .First())
                    .ToDictionary(h => h.ParentRace.LegNr);

                this.jackpotProbability = 0M;

                CalculateNumberOfSingleRows(1);

                return this.numberOfSingleRows;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        internal bool CalculateNumberOfSingleRows(int legNr)
        {
            var horsesInOrder = this.RaceDayInfo.RaceList
                    .First(r => r.LegNr == legNr)
                    .HorseList
                    .Where(h => h.Scratched == false || h.Scratched == null)
                    .OrderBy(h => h.StakeDistributionShare)
                    .ToList();
            try
            {
                foreach (var h in horsesInOrder)
                {
                    this.calculationHorseDictionary[legNr] = h;
                    var singleRow = new HPTMarkBetSingleRow(this.calculationHorseDictionary.Values.ToArray());
                    singleRow.CalculateValues();

                    decimal expectedNumberOfRows = singleRow.RowShareStake * this.totalNumberOfGambledRows;

                    if (legNr < this.NumberOfRaces)
                    {
                        if (expectedNumberOfRows > 1.5M)
                        {
                            return true;
                        }
                        CalculateNumberOfSingleRows(legNr + 1);
                    }
                    else
                    {
                        if (expectedNumberOfRows > 0.5M && expectedNumberOfRows < 1.5M)
                        {
                            this.numberOfSingleRows++;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            this.calculationHorseDictionary[legNr] = horsesInOrder.First();
            return true;
        }

        public ObservableCollection<HPTReductionRule> ReductionRuleStatisticsList { get; set; }
        internal void CalculateRuleStatistics()
        {
            if (this.ReducedSize == 0)
            {
                return;
            }
            // Återställ variabler
            this.ReductionRuleStatisticsList.Clear();
            var singleRowCollection = new HPTMarkBetSingleRowCollection(this);

            // Beräkna ramsystemssannolikhet
            this.SystemProbability = this.RaceDayInfo.RaceList
                .Select(r => r.HorseListSelected.Sum(h => h.StakeDistributionShare))
                .Aggregate((ss, next) => ss * next);

            // Beräkna reducerat systems sannolikhet
            this.ReducedSystemProbability = this.SingleRowCollection.SingleRows
                .Sum(sr => sr.RowShareStake);

            // Beräkna kvoten för sannolikheten
            this.SystemProbabilityRatio = this.ReducedSystemProbability / this.SystemProbability / (1M - this.ReductionQuota);

            if (this.ABCDEFReductionRule.Use)
            {
                SingleRowCollection.CalculateRuleStatistics(this.ABCDEFReductionRule);
                this.ReductionRuleStatisticsList.Add(this.ABCDEFReductionRule);

                this.ABCDEFReductionRule.XReductionRuleList
                    .Where(rr => rr.Use && rr.NumberOfRacesWithX > 0)
                    .ToList()
                    .ForEach(xr =>
                    {
                        SingleRowCollection.CalculateRuleStatistics(xr);
                        this.ReductionRuleStatisticsList.Add(xr);
                    });
            }

            if (this.ComplementaryRulesCollection.Use)
            {
                if (this.ComplementaryRulesCollection.ReductionRuleList.Count(rr => rr.Use) > 1)
                {
                    SingleRowCollection.CalculateRuleStatistics(this.ComplementaryRulesCollection);
                    this.ReductionRuleStatisticsList.Add(this.ComplementaryRulesCollection);
                }

                this.ComplementaryRulesCollection
                    .ReductionRuleList
                    .Where(rr => rr.Use)
                    .ToList()
                    .ForEach(xr =>
                    {
                        SingleRowCollection.CalculateRuleStatistics(xr);
                        this.ReductionRuleStatisticsList.Add(xr);
                    });
            }

            this.IntervalReductionRuleList
                .Where(rr => rr.Use)
                .ToList()
                .ForEach(rr =>
                {
                    SingleRowCollection.CalculateRuleStatistics(rr);
                    this.ReductionRuleStatisticsList.Add(rr);
                }
                );

            if (this.ReductionRank)
            {
                SingleRowCollection.CalculateRuleStatistics(this.RankReductionRule);
                this.ReductionRuleStatisticsList.Add(this.RankReductionRule);
            }

            if (this.ReductionHorseRank)
            {
                this.HorseRankSumReductionRuleList
                    .Where(hrs => hrs.Use)
                    .ToList()
                    .ForEach(hrs =>
                    {
                        SingleRowCollection.CalculateRuleStatistics(hrs);
                        this.ReductionRuleStatisticsList.Add(hrs);

                        hrs.ReductionRuleList
                            .ToList()
                            .ForEach(rr =>
                            {
                                SingleRowCollection.CalculateRuleStatistics(rr);
                                this.ReductionRuleStatisticsList.Add(rr);
                            }
                            );
                    }
                    );
            }

            if (this.GroupIntervalRulesCollection.Use)
            {
                if (this.GroupIntervalRulesCollection.ReductionRuleList.Count(rr => rr.Use) > 1)
                {
                    SingleRowCollection.CalculateRuleStatistics(this.GroupIntervalRulesCollection);
                    this.ReductionRuleStatisticsList.Add(this.GroupIntervalRulesCollection);
                }

                this.GroupIntervalRulesCollection
                    .ReductionRuleList
                    .Where(rr => rr.Use)
                    .ToList()
                    .ForEach(xr =>
                    {
                        SingleRowCollection.CalculateRuleStatistics(xr);
                        this.ReductionRuleStatisticsList.Add(xr);
                    });
            }

            if (this.TrainerRulesCollection.Use)
            {
                if (this.TrainerRulesCollection.ReductionRuleList.Count(rr => rr.Use) > 1)
                {
                    SingleRowCollection.CalculateRuleStatistics(this.TrainerRulesCollection);
                    this.ReductionRuleStatisticsList.Add(this.TrainerRulesCollection);
                }

                this.TrainerRulesCollection
                    .ReductionRuleList
                    .Where(rr => rr.Use)
                    .ToList()
                    .ForEach(xr =>
                    {
                        SingleRowCollection.CalculateRuleStatistics(xr);
                        this.ReductionRuleStatisticsList.Add(xr);
                    });
            }

            if (this.DriverRulesCollection.Use)
            {
                if (this.DriverRulesCollection.ReductionRuleList.Count(rr => rr.Use) > 1)
                {
                    SingleRowCollection.CalculateRuleStatistics(this.DriverRulesCollection);
                    this.ReductionRuleStatisticsList.Add(this.DriverRulesCollection);
                }

                this.DriverRulesCollection
                    .ReductionRuleList
                    .Where(rr => rr.Use)
                    .ToList()
                    .ForEach(xr =>
                    {
                        SingleRowCollection.CalculateRuleStatistics(xr);
                        this.ReductionRuleStatisticsList.Add(xr);
                    });
            }
        }

        internal void CalculateRowValueStatistics()
        {
            if (this.SingleRowCollection.SingleRows == null || this.SingleRowCollection.SingleRows.Count == 0)
            {
                return;
            }

            var singleRowsInOrder = this.SingleRowCollection.SingleRows.OrderBy(sr => sr.RowValueV6);

            this.BetType.RowValueIntervalList
                .ToList()
                .ForEach(rwi =>
                {
                    int lowerLimit = rwi.LowerLimit == null ? 0 : Convert.ToInt32(rwi.LowerLimit);
                    int upperLimit = rwi.UpperLimit == null ? int.MaxValue : Convert.ToInt32(rwi.UpperLimit);
                    rwi.NumberOfRows = singleRowsInOrder.Count(sr => sr.RowValueV6 >= Convert.ToInt32(lowerLimit) && sr.RowValueV6 < Convert.ToInt32(upperLimit));
                    rwi.PercentageOfRows = Convert.ToDecimal(rwi.NumberOfRows) / Convert.ToDecimal(this.ReducedSize);
                });

            if (this.BetType.RowValueIntervalList.Last().NumberOfRows == 0)
            {
                if (this.BetType.RowValueIntervalSingleWinner != null && this.BetType.RowValueIntervalList.Contains(this.BetType.RowValueIntervalSingleWinner))
                {
                    this.BetType.RowValueIntervalList.Remove(this.BetType.RowValueIntervalSingleWinner);
                }
                this.BetType.RowValueIntervalSingleWinner = null;
            }
            else
            {
                this.BetType.RowValueIntervalSingleWinner = new HPTRowValueInterval()
                {
                    LowerLimit = this.RaceDayInfo.MaxPayOut,
                    UpperLimit = null,
                    NumberOfRows = singleRowsInOrder.Count(sr => sr.RowValueV6 >= this.RaceDayInfo.MaxPayOut)
                };
                this.BetType.RowValueIntervalSingleWinner.PercentageOfRows = Convert.ToDecimal(this.BetType.RowValueIntervalSingleWinner.NumberOfRows) / Convert.ToDecimal(this.ReducedSize);
                this.BetType.RowValueIntervalList.Add(this.BetType.RowValueIntervalSingleWinner);
            }

            this.BetType.RowValuePercentileList
                .ToList()
                .ForEach(rwp =>
                {
                    int position = Convert.ToInt32(this.ReducedSize * rwp.Percentile);
                    if (position > 0)
                    {
                        position--;
                    }
                    rwp.RowValue = singleRowsInOrder.ElementAt(position).RowValueV6;
                });
        }

        #endregion

        #region Beräkna bästa ABCD-reducering utifrån sannolikheter

        internal decimal[] probabilitySumArray;
        internal bool[] useStakeShareArray;
        internal decimal[] selectedStakeShareArray;

        internal string CalculateBestABCDCombination()
        {
            try
            {
                var allHorses = this.RaceDayInfo.RaceList.SelectMany(r => r.HorseList).OrderByDescending(h => h.StakeDistributionShare).ToArray();
                var selectedHorses = allHorses.Take(1).ToList();

                string header = Enumerable.Range(0, this.RaceDayInfo.RaceList.Count + 1).Select(i => i.ToString()).Aggregate((i, next) => i + "\t" + next);
                var sb = new StringBuilder(header + "\r\n");

                for (int i = 1; i < 10; i++)
                {
                    int arraySize = i >= this.RaceDayInfo.RaceList.Count ? this.RaceDayInfo.RaceList.Count : i + 1;
                    this.useStakeShareArray = new bool[arraySize];
                    this.probabilitySumArray = new decimal[arraySize + 1];

                    selectedHorses.Add(allHorses[i]);

                    this.selectedStakeShareArray = selectedHorses
                        .GroupBy(h => h.ParentRace.RaceNr)
                        .Select(g => g.Sum(h => h.StakeDistributionShare))
                        .ToArray();

                    CalculatePropabilityForSelectedHorses(true, 0);
                    CalculatePropabilityForSelectedHorses(false, 0);
                    string probabilities = this.probabilitySumArray
                        .Select(p => p.ToString())
                        .Aggregate((p, next) => p.ToString() + "\t" + next.ToString());

                    sb.AppendLine(probabilities);
                }
                return sb.ToString();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return string.Empty;
        }

        internal string CalculateBestAPlusBCombinations()
        {
            var sb = new StringBuilder();
            try
            {
                for (int positionA = 0, numberOfHorsesA = 2; numberOfHorsesA <= 12; numberOfHorsesA++, positionA++)
                {
                    sb.Append(numberOfHorsesA);
                    sb.Append(" A-hästar\t");
                    sb.AppendLine(CalculateBestABCDCombination(0, numberOfHorsesA));
                    sb.AppendLine();
                    for (int positionB = numberOfHorsesA, numberOfHorsesB = 2; numberOfHorsesB <= 12; numberOfHorsesB++, positionB++)
                    {
                        sb.Append(numberOfHorsesB);
                        sb.Append(" B-hästar\t");
                        sb.AppendLine(CalculateBestABCDCombination(numberOfHorsesA, numberOfHorsesB));
                    }
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return sb.ToString();
        }

        internal string CalculateBestAPlusBPlusCCombinations()
        {
            var sb = new StringBuilder();
            try
            {
                int numberOfHorses = this.RaceDayInfo.RaceList
                    .SelectMany(r => r.HorseList)
                    .Where(h => h.StakeDistributionShare > 0.1M)
                    .OrderByDescending(h => h.StakeDistributionShare)
                    .Count();

                Enumerable.Range(2, 10).ToList().ForEach(a =>
                    {
                        string aTabs = new string('\t', 2);
                        string aString = a.ToString()
                            + " A-hästar\t"
                            + CalculateBestABCDCombination(0, a)
                            + aTabs;

                        Enumerable.Range(3, 10).ToList().ForEach(b =>
                        {
                            string bTabs = new string('\t', 2);
                            string bString = b.ToString()
                                + " B-hästar\t"
                                + CalculateBestABCDCombination(a, b)
                                + bTabs;

                            Enumerable.Range(4, 10).ToList().ForEach(c =>
                            {
                                string cString = c.ToString()
                                    + " C-hästar\t"
                                    + CalculateBestABCDCombination(b, c);

                                if (a + b + c == numberOfHorses)
                                {
                                    sb.Append(aString);
                                    sb.Append(bString);
                                    sb.AppendLine(cString);
                                }
                            });
                        });
                    });
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return sb.ToString();
        }

        //internal string CalculateBestAPlusBCombinations()
        //{
        //    var sb = new StringBuilder();
        //    try
        //    {
        //        for (int positionA = 0, numberOfHorsesA = 2; numberOfHorsesA <= 12; numberOfHorsesA++, positionA++)
        //        {
        //            sb.Append(numberOfHorsesA);
        //            sb.Append(" A-hästar\t");
        //            sb.AppendLine(CalculateBestABCCombination(positionA, numberOfHorsesA));
        //            sb.AppendLine();
        //            for (int positionB = numberOfHorsesA, numberOfHorsesB = 2; numberOfHorsesB <= 12; numberOfHorsesB++, positionB++)
        //            {
        //                sb.Append(numberOfHorsesB);
        //                sb.Append(" B-hästar\t");
        //                sb.AppendLine(CalculateBestABCCombination(positionB, numberOfHorsesB));
        //            }
        //            sb.AppendLine();
        //            sb.AppendLine();
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //    }
        //    return sb.ToString();
        //}

        //internal string CalculateBestABCCombination(int numberOfAHorses, int numberOfBHorses)
        //{
        //    try
        //    {
        //        var allHorses = this.RaceDayInfo.RaceList
        //            .SelectMany(r => r.HorseList)
        //            .OrderByDescending(h => h.StakeDistributionShare)
        //            .ToList();

        //        var selectedAHorses = allHorses
        //            .Take(numberOfAHorses)
        //            .ToList();

        //        var selectedBHorses = allHorses
        //            .Except(selectedAHorses)
        //            .Take(numberOfBHorses)
        //            .ToList();

        //        int arraySize = numberOfHorses >= this.RaceDayInfo.RaceList.Count ? this.RaceDayInfo.RaceList.Count : numberOfHorses + 1;
        //        this.useStakeShareArray = new bool[arraySize];
        //        this.probabilitySumArray = new decimal[arraySize + 1];

        //        this.selectedStakeShareArray = selectedHorses
        //            .GroupBy(h => h.ParentRace.RaceNr)
        //            .Select(g => g.Sum(h => h.StakeDistributionShare))
        //            .ToArray();

        //        CalculatePropabilityForSelectedHorses(true, 0);
        //        CalculatePropabilityForSelectedHorses(false, 0);
        //        string probabilities = this.probabilitySumArray
        //            .Select(p => p.ToString())
        //            .Aggregate((p, next) => p.ToString() + "\t" + next.ToString());

        //        return probabilities;
        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //    }
        //    return string.Empty;
        //}

        internal string CalculateBestABCDCombination(int startPosition, int numberOfHorses)
        {
            try
            {
                var allHorses = this.RaceDayInfo.RaceList.SelectMany(r => r.HorseList).OrderByDescending(h => h.StakeDistributionShare).ToArray();
                var selectedHorses = new List<HPTHorse>();
                for (int i = startPosition; i < startPosition + numberOfHorses; i++)
                {
                    selectedHorses.Add(allHorses[i]);
                }

                //int arraySize = numberOfHorses >= this.RaceDayInfo.RaceList.Count ? this.RaceDayInfo.RaceList.Count : numberOfHorses + 1;
                int arraySize = this.RaceDayInfo.RaceList.Count;
                this.useStakeShareArray = new bool[arraySize];
                this.probabilitySumArray = new decimal[arraySize + 1];

                this.selectedStakeShareArray = selectedHorses
                    .GroupBy(h => h.ParentRace.LegNr)
                    .Select(g => g.Sum(h => h.StakeDistributionShare))
                    .ToArray();

                CalculatePropabilityForSelectedHorses(true, 0);
                CalculatePropabilityForSelectedHorses(false, 0);
                string probabilities = this.probabilitySumArray
                    .Select(p => p.ToString())
                    .Aggregate((p, next) => p.ToString() + "\t" + next.ToString());

                return probabilities;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return string.Empty;
        }

        internal void CalculateBestABCDCombination(HPTXReductionRule rule)
        {
            try
            {
                var selectedHorses = this.RaceDayInfo.RaceList
                    .SelectMany(r => r.HorseList)
                    .Where(h => h.Prio == rule.Prio);

                if (selectedHorses.Count() > 0)
                {
                    var probabilitySumArray = CalculateProbabilities(selectedHorses);

                    Enumerable.Range(0, probabilitySumArray.Length)
                        .ToList()
                        .ForEach(i => rule.NumberOfWinnersList.First(now => now.NumberOfWinners == i).Probability = probabilitySumArray[i]);

                    rule.Probability = rule.NumberOfWinnersList
                        .Where(now => now.Selected)
                        .Sum(now => now.Probability);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        internal decimal[] CalculateProbabilities(IEnumerable<HPTHorse> selectedHorses)
        {
            try
            {
                int arraySize = selectedHorses
                    .Select(h => h.ParentRace.LegNr)
                    .Distinct()
                    .Count();

                this.useStakeShareArray = new bool[arraySize];
                this.probabilitySumArray = new decimal[arraySize + 1];

                this.selectedStakeShareArray = selectedHorses
                    .GroupBy(h => h.ParentRace.LegNr)
                    .Select(g => g.Sum(h => h.StakeDistributionShare))
                    .ToArray();

                CalculatePropabilityForSelectedHorses(true, 0);
                CalculatePropabilityForSelectedHorses(false, 0);

                return this.probabilitySumArray;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return null;
        }

        internal string CalculateBestABCDCombination(IEnumerable<HPTHorse> selectedHorses)
        {
            try
            {
                int arraySize = selectedHorses
                    .Select(h => h.ParentRace.LegNr)
                    .Distinct()
                    .Count();

                this.useStakeShareArray = new bool[arraySize];
                this.probabilitySumArray = new decimal[arraySize + 1];

                this.selectedStakeShareArray = selectedHorses
                    .GroupBy(h => h.ParentRace.LegNr)
                    .Select(g => g.Sum(h => h.StakeDistributionShare))
                    .ToArray();

                CalculatePropabilityForSelectedHorses(true, 0);
                CalculatePropabilityForSelectedHorses(false, 0);
                string probabilities = this.probabilitySumArray
                    .Select(p => p.ToString())
                    .Aggregate((p, next) => p.ToString() + "\t" + next.ToString());

                return probabilities;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return string.Empty;
        }

        internal void CalculatePropabilityForSelectedHorses(bool useStakeShare, int indexToUse)
        {
            try
            {
                this.useStakeShareArray[indexToUse] = useStakeShare;
                if (indexToUse == this.selectedStakeShareArray.Length - 1)
                {
                    decimal probability = 1M;
                    for (int i = 0; i < this.selectedStakeShareArray.Length; i++)
                    {
                        if (this.useStakeShareArray[i])
                        {
                            probability *= this.selectedStakeShareArray[i];
                        }
                        else
                        {
                            probability *= (1M - this.selectedStakeShareArray[i]);
                        }
                    }
                    int probabilitySumIndex = this.useStakeShareArray.Count(us => us);
                    this.probabilitySumArray[probabilitySumIndex] += probability;
                }
                else
                {
                    CalculatePropabilityForSelectedHorses(true, indexToUse + 1);
                    CalculatePropabilityForSelectedHorses(false, indexToUse + 1);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        //internal void CalculateABCDProbability()
        //{
        //    try
        //    {
        //        foreach (var abcdReductionRule in this.ABCDEFReductionRule.XReductionRuleList)
        //        {
        //            var useStakeShareArrayLocal = abcdReductionRule.NumberOfWinnersList
        //                .Where(now => now.Selectable)
        //                .OrderBy(now => now.NumberOfWinners)
        //                .Select(now => now.Selected)
        //                .ToArray();

        //            var selectedHorses = this.RaceDayInfo.RaceList
        //                .SelectMany(r => r.HorseList)
        //                .Where(h => h.Selected && h.Prio == abcdReductionRule.Prio)
        //                .ToList();

        //            this.selectedStakeShareArray = selectedHorses
        //                .GroupBy(h => h.ParentRace.RaceNr)
        //                .Select(g => g.Sum(h => h.StakeDistributionShare))
        //                .ToArray();

        //            this.useStakeShareArray = new bool[useStakeShareArrayLocal.Length];
        //            this.probabilitySumArray = new decimal[useStakeShareArrayLocal.Length];

        //            CalculatePropabilityForSelectedHorses(true, 0);
        //            CalculatePropabilityForSelectedHorses(false, 0);

        //            decimal probabilitySum = 0M;
        //            for (int i = 0; i < useStakeShareArrayLocal.Length; i++)
        //            {
        //                if (useStakeShareArrayLocal[i])
        //                {
        //                    probabilitySum += this.probabilitySumArray[i];
        //                }
        //            }
        //            abcdReductionRule.Probability = probabilitySum;
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //    }
        //}

        #endregion

        #region Ta bort/lägg till rader från kupongfil

        internal void RemoveRowsInFileFromSystem(string couponFileName)
        {
            var fs = new FileStream(couponFileName, FileMode.Open);
            var serializer = new XmlSerializer(typeof(issuer));
            var atgXml = (issuer)serializer.Deserialize(fs);

            var couponHelper = new ATGCouponHelper(this);
            couponHelper.CouponList = couponHelper.CreateHPTCouponsFromATGFile(atgXml);
            couponHelper.CreateHorseListsForCoupons();
            var singleRowsToRemove = couponHelper.CreateSingleRowsFromCoupons();
            foreach (var singleRow in singleRowsToRemove)
            {
                singleRow.CalculateValues();
                var singleRowToRemove = this.SingleRowCollection.SingleRows
                    .FirstOrDefault(sr => sr.UniqueCode == singleRow.UniqueCode);
                if (singleRowToRemove != null)
                {
                    //this.SingleRowCollection.SingleRowso.Remove(singleRowToRemove);
                    this.SingleRowCollection.SingleRows.Remove(singleRowToRemove);
                }
            }
            this.ReducedSize = this.SingleRowCollection.SingleRows.Count;
            this.TotalCouponSize = this.SingleRowCollection.SingleRows.Sum(sr => sr.BetMultiplier);
            SingleRowCollection_AnalyzingFinished();
            //this.LockCoupons = true;
            fs.Flush();
            fs.Close();
            fs = null;
        }

        internal void AddRowsInFileToSystem(string couponFileName)
        {
            var fs = new FileStream(couponFileName, FileMode.Open);
            var serializer = new XmlSerializer(typeof(issuer));
            var atgXml = (issuer)serializer.Deserialize(fs);

            var couponHelper = new ATGCouponHelper(this);
            couponHelper.CouponList = couponHelper.CreateHPTCouponsFromATGFile(atgXml);
            couponHelper.CreateHorseListsForCoupons();
            var singleRowsToAdd = couponHelper.CreateSingleRowsFromCoupons();
            int rowNumber = this.SingleRowCollection.SingleRows
                .Max(sr => sr.RowNumber);
            foreach (var singleRow in singleRowsToAdd)
            {
                singleRow.CalculateValues();
                var singleRowToAdd = this.SingleRowCollection.SingleRows
                    .FirstOrDefault(sr => sr.UniqueCode == singleRow.UniqueCode);
                if (singleRowToAdd == null)
                {
                    singleRow.RowNumber = ++rowNumber;
                    singleRow.BetMultiplier = 1;
                    //this.SingleRowCollection.SingleRows.Add(singleRow);
                    this.SingleRowCollection.SingleRows.Add(singleRow);
                }
            }
            this.ReducedSize = this.SingleRowCollection.SingleRows.Count;
            this.TotalCouponSize = this.SingleRowCollection.SingleRows.Sum(sr => sr.BetMultiplier);
            SingleRowCollection_AnalyzingFinished();
            //this.LockCoupons = true;
            fs.Flush();
            fs.Close();
            fs = null;
        }

        #endregion

        public override string SystemNameForIdentification
        {
            get
            {
                if (this.HasSystemName)
                {
                    return this.SystemName;
                }
                return this.BetType.Name + ": " + this.ReducedSize.ToString() + " - " + this.SystemSize.ToString();
            }
        }

        internal void SuggestNextTimers()
        {
            this.RaceDayInfo.RaceList
                    .ForEach(r =>
                    {
                        var winner = r.HorseList.FirstOrDefault(h => h.HorseResultInfo.FinishingPosition == 1);
                        if (winner != null)
                        {
                            r.HorseList.Where(h => h.HorseResultInfo != null)
                                        .Where(h => h.HorseResultInfo.FinishingPosition > 1 && h.HorseResultInfo.FinishingPosition < 5)
                                        .ToList()
                                        .ForEach(h =>
                                        {
                                            double q = h.HorseResultInfo.TotalTime.TotalSeconds / winner.HorseResultInfo.TotalTime.TotalSeconds;
                                            //if (q < 1.01D && h.StakeDistributionShare < 0.1M)
                                            if (q < 1.007D && h.StakeDistributionShare < 0.09M)
                                            {
                                                if (h.OwnInformation == null)
                                                {
                                                    h.OwnInformation = new HPTHorseOwnInformation()
                                                    {
                                                        Name = h.HorseName,
                                                        ATGId = h.ATGId,
                                                        HomeTrack = h.HomeTrack,
                                                        Sex = h.Sex,
                                                        Age = h.Age,
                                                        Owner = h.OwnerName,
                                                        Trainer = h.TrainerName,
                                                        CreationDate = DateTime.Now,
                                                    };
                                                }
                                                if (h.OwnInformation.HorseOwnInformationCommentList == null)
                                                {
                                                    h.OwnInformation.HorseOwnInformationCommentList = new ObservableCollection<HPTHorseOwnInformationComment>();
                                                }

                                                HPTConfig.Config.HorseOwnInformationCollection.MergeHorseOwnInformation(h);

                                                if (h.OwnInformation.HorseOwnInformationCommentList.Any(hc => hc.CommentUser == "HPT"))
                                                {
                                                    h.OwnInformation.HorseOwnInformationCommentList
                                                        .ToList()
                                                        .ForEach(hc => h.OwnInformation.HorseOwnInformationCommentList.Remove(hc));
                                                }

                                                var sb = new StringBuilder();
                                                double loseMarginInTenths = (h.HorseResultInfo.TotalTime.TotalMilliseconds - winner.HorseResultInfo.TotalTime.TotalMilliseconds) / 100D;

                                                sb.AppendFormat("{0}, Lopp {1}.", this.RaceDayInfo.ToDateAndTrackString(), h.ParentRace.RaceNr);
                                                sb.AppendLine();

                                                sb.AppendFormat("Plats {0}, {1} tiondelar bakom {2}.", h.HorseResultInfo.FinishingPosition, loseMarginInTenths, winner.HorseNumberAndName);
                                                sb.AppendLine();

                                                sb.AppendFormat("Insatsfördelning {0:P2}, Vinnarodds {1:N2}.", h.StakeDistributionShare, h.VinnarOddsExact);

                                                var comment = new HPTHorseOwnInformationComment()
                                                {
                                                    Comment = sb.ToString(),
                                                    CommentDate = DateTime.Now,
                                                    CommentUser = "HPT",
                                                    HasComment = true,
                                                    IsOwnComment = false,
                                                    NextTimer = true
                                                };
                                                h.OwnInformation.HorseOwnInformationCommentList.Add(comment);
                                                h.OwnInformation.NextTimer = true;
                                            }
                                        });
                        }
                    });
        }
    }

    public enum RecalculateReason
    {
        All,
        XReduction,
        Rank,
        Other
    }
}
