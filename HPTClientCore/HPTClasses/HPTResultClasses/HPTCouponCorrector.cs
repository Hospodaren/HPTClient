using System.Collections.ObjectModel;

namespace HPTClient
{
    public class HPTCouponCorrector : Notifier, IHorseListContainer
    {
        public HPTCouponCorrector()
        {
            HorseList = new ObservableCollection<HPTHorse>();
            ParentRaceDayInfo = new HPTRaceDayInfo();
            ParentRaceDayInfo.DataToShow = HPTConfig.Config.DataToShowCorrection;
        }

        #region Methods

        public void CorrectCoupons(int racesToCorrect)
        {
            if (racesToCorrect == 0 || racesToCorrect > RaceDayInfo.RaceList.Count)
            {
                racesToCorrect = RaceDayInfo.RaceList.Count;
            }
            HorseList.Clear();
            for (int raceNumber = 1; raceNumber <= racesToCorrect; raceNumber++)
            {
                HPTRace hptRace = RaceDayInfo.RaceList.FirstOrDefault(hr => hr.LegNr == raceNumber);

                HPTLegResult legResult = hptRace.LegResult;
                if (legResult != null)
                {
                    // TODO: Ta bort smygen?
                    //// Hämta värdet vid Smygen...
                    //if (legResult.Value == 0 && RaceDayInfo.ResultMarkingBet != null)
                    //{
                    //    var serviceLegResult = RaceDayInfo.ResultMarkingBet.LegResultList.FirstOrDefault(lr => lr.LegNr == legResult.LegNr);
                    //    if (serviceLegResult != null)
                    //    {
                    //        legResult.SystemsLeft = serviceLegResult.SystemsLeft;
                    //        legResult.Value = serviceLegResult.Value;
                    //    }
                    //}
                    legResult.LegNrString = RaceDayInfo.BetType.Code + "-" + legResult.LegNr.ToString();
                    legResult.WinnerStrings = new string[legResult.Winners.Length];


                    if (hptRace == null)
                    {
                        return;
                    }

                    hptRace.LegResult = legResult;
                    HPTHorse[] winnerList = new HPTHorse[legResult.Winners.Length];

                    for (int i = 0; i < legResult.Winners.Length; i++)
                    {
                        HPTHorse horse = hptRace.HorseList.FirstOrDefault(h => h.StartNr == legResult.Winners[i]);
                        if (horse == null)
                        {
                            return;
                        }
                        horse.Correct = true;
                        HorseList.Add(horse);
                        legResult.WinnerStrings[i] = legResult.Winners[i].ToString() + "-" + horse.HorseName;
                        hptRace.LegResult.Value = legResult.Value;
                        hptRace.LegResult.SystemsLeft = legResult.SystemsLeft;
                        winnerList[i] = hptRace.HorseList.First(h => h.StartNr == legResult.Winners[i]);
                    }
                    hptRace.LegResult.WinnerStrings = legResult.WinnerStrings;
                    hptRace.LegResult.WinnerList = winnerList;
                    hptRace.SplitVictory = winnerList.Count() > 1;

                    // // Skapa resultatlänk till atg.se
                    // ATGLinkCreator.CreateRaceResultLink(hptRace);
                }
            }


            // Beräkna vinnande rader och potentiella vinstrader i nästa lopp
            CouponHelper.TotalWinnings = 0;
            CouponHelper.TotalNumberOfAllCorrect = 0;
            CouponHelper.TotalNumberOfOneError = 0;
            CouponHelper.TotalNumberOfTwoErrors = 0;
            CouponHelper.TotalNumberOfThreeErrors = 0;
            foreach (HPTCoupon coupon in CouponHelper.CouponList)
            {
                coupon.CorrectCoupon(RaceDayInfo, racesToCorrect);
                CouponHelper.TotalNumberOfAllCorrect += coupon.NumberOfAllCorrect * coupon.BetMultiplier;
                CouponHelper.TotalNumberOfOneError += coupon.NumberOfOneError * coupon.BetMultiplier;
                CouponHelper.TotalNumberOfTwoErrors += coupon.NumberOfTwoErrors * coupon.BetMultiplier;
                CouponHelper.TotalNumberOfThreeErrors += coupon.NumberOfThreeErrors * coupon.BetMultiplier;
                if (RaceDayInfo.ResultComplete && racesToCorrect == RaceDayInfo.RaceList.Count)
                {
                    CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[0].PayOutAmount * coupon.NumberOfAllCorrect * coupon.BetMultiplier;
                    switch (coupon.BetType)
                    {
                        case "V65":
                            if (coupon.V6)  // 2 ggr vinsten, men bara utdelning på alla rätt
                            {
                                CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[0].PayOutAmount * coupon.NumberOfAllCorrect * coupon.BetMultiplier;
                            }
                            else
                            {
                                CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[1].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                            }
                            break;
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            if (coupon.V6)  // 2.5 ggr vinsten, men bara utdelning på alla rätt
                            {
                                CouponHelper.TotalWinnings += Convert.ToInt32(RaceDayInfo.PayOutList[0].PayOutAmount * 1.5M * coupon.NumberOfAllCorrect * coupon.BetMultiplier);
                            }
                            else
                            {
                                CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[1].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                                CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[2].PayOutAmount * coupon.NumberOfTwoErrors * coupon.BetMultiplier;
                            }
                            break;
                        case "V85":

                            CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[1].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                            CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[2].PayOutAmount * coupon.NumberOfTwoErrors * coupon.BetMultiplier;
                            CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[3].PayOutAmount * coupon.NumberOfThreeErrors * coupon.BetMultiplier;
                            break;
                        case "V4":
                        case "V5":
                            if (RaceDayInfo.PayOutList[0].NumberOfCorrect == RaceDayInfo.BetType.NumberOfRaces - 1)
                            {
                                CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[0].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                                if (coupon.NumberOfOneError > 0)
                                {
                                    coupon.NumberOfCorrectsColor = new System.Windows.Media.SolidColorBrush(HPTConfig.Config.ColorGood);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            // TODO: Skita i smygen?
            //// Smygen, utdelningen har inte visats upp
            //if (RaceDayInfo.PayOutList.Count == 0 && RaceDayInfo.ResultMarkingBet != null && racesToCorrect == RaceDayInfo.RaceList.Count)
            //{
            //    foreach (var servicePayOut in RaceDayInfo.ResultMarkingBet.PayOutList)
            //    {
            //        var payOut = new HPTPayOut()
            //        {
            //            NumberOfCorrect = servicePayOut.NumberOfCorrect,
            //            NumberOfSystems = servicePayOut.NumberOfSystems,
            //            NumberOfWinningRows = 0,
            //            OwnWinnings = 0,
            //            PayOutAmount = servicePayOut.PayOutAmount,
            //            TotalAmount = servicePayOut.TotalAmount
            //        };
            //        RaceDayInfo.PayOutList.Add(payOut);
            //    }
            //}

            if (RaceDayInfo.ResultComplete && racesToCorrect == RaceDayInfo.RaceList.Count && RaceDayInfo.PayOutList.Count > 0)
            {
                RaceDayInfo.PayOutList[0].NumberOfWinningRows = CouponHelper.TotalNumberOfAllCorrect;
                switch (RaceDayInfo.BetType.Code)
                {
                    case "V65":
                        RaceDayInfo.PayOutList[1].NumberOfWinningRows = CouponHelper.TotalNumberOfOneError;
                        break;
                    case "V64":
                    case "V75":
                    case "GS75":
                    case "V86":
                        RaceDayInfo.PayOutList[1].NumberOfWinningRows = CouponHelper.TotalNumberOfOneError;
                        RaceDayInfo.PayOutList[2].NumberOfWinningRows = CouponHelper.TotalNumberOfTwoErrors;
                        break;
                    case "V85":
                        RaceDayInfo.PayOutList[1].NumberOfWinningRows = CouponHelper.TotalNumberOfOneError;
                        RaceDayInfo.PayOutList[2].NumberOfWinningRows = CouponHelper.TotalNumberOfTwoErrors;
                        RaceDayInfo.PayOutList[3].NumberOfWinningRows = CouponHelper.TotalNumberOfThreeErrors; // TODO
                        break;
                    case "V4":
                    case "V5":
                        if (RaceDayInfo.PayOutList[0].NumberOfCorrect == RaceDayInfo.BetType.NumberOfRaces - 1)
                        {
                            RaceDayInfo.PayOutList[0].NumberOfWinningRows = CouponHelper.TotalNumberOfOneError;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void ClearCorrection()
        {
            CouponHelper.TotalWinnings = 0;
            CouponHelper.TotalNumberOfAllCorrect = 0;
            CouponHelper.TotalNumberOfOneError = 0;
            CouponHelper.TotalNumberOfTwoErrors = 0;
            CouponHelper.TotalNumberOfThreeErrors = 0;
            foreach (HPTCoupon coupon in CouponHelper.CouponList)
            {
                coupon.NumberOfCorrect = 0;
                coupon.NumberOfAllCorrect = 0;
                coupon.NumberOfOneError = 0;
                coupon.NumberOfTwoErrors = 0;
                coupon.NumberOfThreeErrors = 0;
                coupon.NumberOfCorrectsColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
            }
        }

        public void CorrectCouponsSimulatedResult()
        {
            CouponHelper.TotalWinnings = 0;
            CouponHelper.TotalNumberOfAllCorrect = 0;
            CouponHelper.TotalNumberOfOneError = 0;
            CouponHelper.TotalNumberOfTwoErrors = 0;
            CouponHelper.TotalNumberOfThreeErrors = 0;
            foreach (HPTCoupon coupon in CouponHelper.CouponList)
            {
                coupon.CorrectCouponSimulated(RaceDayInfo);
                CouponHelper.TotalNumberOfAllCorrect += coupon.NumberOfAllCorrect * coupon.BetMultiplier;
                CouponHelper.TotalNumberOfOneError += coupon.NumberOfOneError * coupon.BetMultiplier;
                CouponHelper.TotalNumberOfTwoErrors += coupon.NumberOfTwoErrors * coupon.BetMultiplier;
                CouponHelper.TotalNumberOfThreeErrors += coupon.NumberOfThreeErrors * coupon.BetMultiplier;
            }

            var horseList = RaceDayInfo.RaceList
                .Where(r => r.LegResult != null && r.LegResult.WinnerList != null && r.LegResult.WinnerList.Any())
                .Select(r => r.LegResult.WinnerList.First()).ToArray();

            if (horseList.Count() == RaceDayInfo.RaceList.Count)
            {
                RaceDayInfo.PayOutList = new ObservableCollection<HPTPayOut>(RaceDayInfo.BetType.PayOutDummyList);
                RaceDayInfo.PayOutList[0].NumberOfWinningRows = CouponHelper.TotalNumberOfAllCorrect;

                var markBetSingleRow = new HPTMarkBetSingleRow(horseList);

                // Alla rätt
                RaceDayInfo.PayOutList[0].NumberOfWinningRows = CouponHelper.TotalNumberOfAllCorrect;
                RaceDayInfo.PayOutList[0].PayOutAmount = CalculatePayOut(horseList, RaceDayInfo.BetType.PoolShare * RaceDayInfo.BetType.RowCost);

                // Ett fel
                if (RaceDayInfo.PayOutList.Count > 1)
                {
                    RaceDayInfo.PayOutList[1].NumberOfWinningRows = CouponHelper.TotalNumberOfOneError;
                    RaceDayInfo.PayOutList[1].PayOutAmount = CalculatePayOutOneError(horseList, RaceDayInfo.BetType.PoolShareOneError * RaceDayInfo.BetType.RowCost);
                }

                // Två fel
                if (RaceDayInfo.PayOutList.Count > 2)
                {
                    RaceDayInfo.PayOutList[2].NumberOfWinningRows = CouponHelper.TotalNumberOfTwoErrors;
                    RaceDayInfo.PayOutList[2].PayOutAmount = CalculatePayOutTwoErrors(horseList, RaceDayInfo.BetType.PoolShareTwoErrors * RaceDayInfo.BetType.RowCost);
                }

                // Tre fel
                if (RaceDayInfo.PayOutList.Count > 3)
                {
                    RaceDayInfo.PayOutList[3].NumberOfWinningRows = CouponHelper.TotalNumberOfThreeErrors;
                    RaceDayInfo.PayOutList[3].PayOutAmount = CalculatePayOutThreeErrors(horseList, RaceDayInfo.BetType.PoolShareThreeErrors * RaceDayInfo.BetType.RowCost);
                }

                foreach (HPTCoupon coupon in CouponHelper.CouponList)
                {
                    CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[0].PayOutAmount * coupon.NumberOfAllCorrect * coupon.BetMultiplier;
                    switch (coupon.BetType)
                    {
                        case "V65":
                            if (coupon.V6)  // 2 ggr vinsten, men bara utdelning på alla rätt
                            {
                                CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[0].PayOutAmount * coupon.NumberOfAllCorrect * coupon.BetMultiplier;
                            }
                            else
                            {
                                CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[1].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                            }
                            break;
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            if (coupon.V6)  // 2.5 ggr vinsten, men bara utdelning på alla rätt
                            {
                                CouponHelper.TotalWinnings += Convert.ToInt32(RaceDayInfo.PayOutList[0].PayOutAmount * 1.5M * coupon.NumberOfAllCorrect * coupon.BetMultiplier);
                            }
                            else
                            {
                                CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[1].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                                CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[2].PayOutAmount * coupon.NumberOfTwoErrors * coupon.BetMultiplier;
                            }
                            break;
                        case "V85":
                            CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[1].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                            CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[2].PayOutAmount * coupon.NumberOfTwoErrors * coupon.BetMultiplier;
                            CouponHelper.TotalWinnings += RaceDayInfo.PayOutList[3].PayOutAmount * coupon.NumberOfThreeErrors * coupon.BetMultiplier;    // TODO
                            break;
                        default:
                            break;
                    }
                }
            }

            HorseList.Clear();
            IEnumerable<HPTHorse> horseListCorrect = RaceDayInfo.RaceList
                 .Where(r => r.LegResult != null && r.LegResult.WinnerList != null)
                 .SelectMany(r => r.LegResult.WinnerList);

            foreach (HPTHorse correctHorse in horseListCorrect.OrderBy(r => r.ParentRace.RaceNr))
            {
                HorseList.Add(correctHorse);
            }
        }

        #region Beräkna värde

        internal int CalculatePayOut(IEnumerable<HPTHorse> horseList, decimal factor)
        {
            decimal rowShareStake = horseList
                   .Select(h => h.StakeDistributionShare)
                   .Aggregate((share, next) => share * next);

            decimal result = RaceDayInfo.MaxPayOut / (rowShareStake * RaceDayInfo.NumberOfGambledRowsTotal);
            result /= RaceDayInfo.V6Factor;

            if (result > RaceDayInfo.MaxPayOut)
            {
                result = RaceDayInfo.MaxPayOut;
            }
            return Convert.ToInt32(result);
        }

        internal int CalculatePayOutOneError(IEnumerable<HPTHorse> horseList, decimal factor)
        {
            decimal totalStakeShare = 0M;
            decimal rowStakeShare = horseList
                .Select(h => h.StakeDistributionShare)
                .Aggregate((share, next) => share * next);

            foreach (var horse in horseList)
            {
                totalStakeShare += (rowStakeShare / horse.StakeDistributionShare * (1M - horse.StakeDistributionShare));
            }
            decimal result = factor / totalStakeShare;
            return Convert.ToInt32(Math.Floor(result));
        }

        internal int CalculatePayOutTwoErrors(IEnumerable<HPTHorse> horseList, decimal factor)
        {
            var horseArray = horseList.ToArray();
            decimal totalStakeShare = 0M;

            decimal rowStakeShare = horseList
                .Select(h => h.StakeDistributionShare)
                .Aggregate((share, next) => share * next);

            for (int i1 = 0; i1 < horseArray.Length - 1; i1++)
            {
                var horse1 = horseArray[i1];
                for (int i2 = i1 + 1; i2 < horseArray.Length; i2++)
                {
                    var horse2 = horseArray[i2];

                    decimal invertedStakeFactor = (1M - horse1.StakeDistributionShare) * (1M - horse2.StakeDistributionShare);
                    decimal stakeFactor = horse1.StakeDistributionShare * horse2.StakeDistributionShare;
                    totalStakeShare += (rowStakeShare / stakeFactor * invertedStakeFactor);
                }
            }
            decimal result = factor / totalStakeShare;
            return Convert.ToInt32(Math.Floor(result));
        }

        internal int CalculatePayOutThreeErrors(IEnumerable<HPTHorse> horseList, decimal factor)
        {
            var horseArray = horseList.ToArray();
            decimal totalStakeShare = 0M;

            decimal rowStakeShare = horseList
                .Select(h => h.StakeDistributionShare)
                .Aggregate((share, next) => share * next);

            for (int i1 = 0; i1 < horseArray.Length - 1; i1++)
            {
                var horse1 = horseArray[i1];
                for (int i2 = i1 + 1; i2 < horseArray.Length; i2++)
                {
                    var horse2 = horseArray[i2];

                    for (int i3 = i2 + 1; i3 < horseArray.Length; i3++)
                    {
                        var horse3 = horseArray[i3];

                        decimal invertedStakeFactor = (1M - horse1.StakeDistributionShare)
                            * (1M - horse2.StakeDistributionShare)
                            * (1M - horse3.StakeDistributionShare);

                        decimal stakeFactor = horse1.StakeDistributionShare * horse2.StakeDistributionShare * horse3.StakeDistributionShare;
                        totalStakeShare += (rowStakeShare / stakeFactor * invertedStakeFactor);
                    }
                }
            }
            decimal result = factor / totalStakeShare;
            return Convert.ToInt32(Math.Floor(result));
        }

        internal int CalculatePayOutOneErrorFinalStakeShare(IEnumerable<HPTHorse> horseList, decimal factor)
        {
            decimal totalStakeShare = 0M;
            decimal rowStakeShare = horseList
                .Select(h => (decimal)h.StakeDistributionShareFinal)
                .Aggregate((share, next) => share * next);

            foreach (var horse in horseList)
            {
                totalStakeShare += (rowStakeShare / (decimal)horse.StakeDistributionShareFinal * (decimal)(1M - horse.StakeDistributionShareFinal));
            }
            decimal result = factor / totalStakeShare;
            return Convert.ToInt32(Math.Floor(result));
        }

        internal int CalculatePayOutTwoErrorsFinalStakeShare(IEnumerable<HPTHorse> horseList, decimal factor)
        {
            var horseArray = horseList.ToArray();
            decimal totalStakeShare = 0M;

            decimal rowStakeShare = horseList
                .Select(h => (decimal)h.StakeDistributionShareFinal)
                .Aggregate((share, next) => share * next);

            for (int i = 0; i < horseArray.Length - 1; i++)
            {
                var horse1 = horseArray[i];
                for (int j = i + 1; j < horseArray.Length; j++)
                {
                    var horse2 = horseArray[j];

                    decimal invertedStakeFactor = (1M - (decimal)horse1.StakeDistributionShareFinal) * (1M - (decimal)horse2.StakeDistributionShareFinal);
                    decimal stakeFactor = (decimal)horse1.StakeDistributionShareFinal * (decimal)horse2.StakeDistributionShareFinal;
                    totalStakeShare += (rowStakeShare / stakeFactor * invertedStakeFactor);
                }
            }
            decimal result = factor / totalStakeShare;
            return Convert.ToInt32(Math.Floor(result));
        }

        internal int CalculatePayOutThreeErrorsFinalStakeShare(IEnumerable<HPTHorse> horseList, decimal factor)
        {
            // TODO:Fixa beräkningen
            var horseArray = horseList.ToArray();
            decimal totalStakeShare = 0M;

            decimal rowStakeShare = horseList
                .Select(h => (decimal)h.StakeDistributionShareFinal)
                .Aggregate((share, next) => share * next);

            for (int i1 = 0; i1 < horseArray.Length - 1; i1++)
            {
                var horse1 = horseArray[i1];
                for (int i2 = i1 + 1; i2 < horseArray.Length; i2++)
                {
                    var horse2 = horseArray[i2];

                    for (int i3 = i2 + 1; i3 < horseArray.Length; i3++)
                    {
                        var horse3 = horseArray[i3];

                        decimal invertedStakeFactor = (1M - (decimal)horse1.StakeDistributionShareFinal)
                            * (1M - (decimal)horse2.StakeDistributionShareFinal)
                            * (1M - (decimal)horse3.StakeDistributionShareFinal);

                        decimal stakeFactor = (decimal)horse1.StakeDistributionShareFinal * (decimal)horse2.StakeDistributionShareFinal * (decimal)horse3.StakeDistributionShareFinal;
                        totalStakeShare += (rowStakeShare / stakeFactor * invertedStakeFactor);
                    }
                }
            }
            decimal result = factor / totalStakeShare;
            return Convert.ToInt32(Math.Floor(result));
        }

        #endregion

        public void UpdateResult(int racesToCorrect, bool setValue)
        {
            try
            {
                var serviceConnector = new HPTServiceConnector();
                serviceConnector.GetResultMarkingBetByTrackAndDate(RaceDayInfo.BetType.Code, RaceDayInfo.TrackId, RaceDayInfo.RaceDayDate, RaceDayInfo, setValue);
                ReplaceScratchedHorses();
                CorrectCoupons(racesToCorrect);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        public void RetrieveResult(bool setValues)
        {
            var serviceConnector = new HPTServiceConnector();
            serviceConnector.GetResultMarkingBetByTrackAndDate(RaceDayInfo.BetType.Code, RaceDayInfo.TrackId, RaceDayInfo.RaceDayDate, RaceDayInfo, setValues);
            ReplaceScratchedHorses();
        }

        public bool ReplaceScratchedHorses()
        {
            bool replaced = false;
            foreach (HPTCoupon coupon in CouponHelper.CouponList)
            {
                foreach (HPTCouponRace couponRace in coupon.CouponRaceList)
                {
                    HPTRace race = RaceDayInfo.RaceList.Where(hr => hr.LegNr == couponRace.LegNr).FirstOrDefault();
                    if (race != null)
                    {
                        if (couponRace.ReplaceScratchedHorses(race))
                        {
                            replaced = true;
                        }
                    }
                }
            }
            return replaced;
        }

        #endregion

        #region Properties

        private ATGCouponHelper couponHelper;
        public ATGCouponHelper CouponHelper
        {
            get
            {
                return couponHelper;
            }
            set
            {
                couponHelper = value;
                OnPropertyChanged("CouponHelper");
            }
        }

        private HPTRaceDayInfo raceDayInfo;
        public HPTRaceDayInfo RaceDayInfo
        {
            get
            {
                return raceDayInfo;
            }
            set
            {
                raceDayInfo = value;
                OnPropertyChanged("RaceDayInfo");
            }
        }

        #endregion

        #region IHorseListContainer implementation

        public ICollection<HPTHorse> HorseList { get; set; }

        public HPTRaceDayInfo ParentRaceDayInfo { get; set; }

        #endregion

        public HPTResultAnalyzer ResultAnalyzer { get; set; }
    }
}
