using System.Collections.ObjectModel;

namespace HPTClient
{
    public class HPTCouponCorrector : Notifier, IHorseListContainer
    {
        public HPTCouponCorrector()
        {
            this.HorseList = new ObservableCollection<HPTHorse>();
            this.ParentRaceDayInfo = new HPTRaceDayInfo();
            this.ParentRaceDayInfo.DataToShow = HPTConfig.Config.DataToShowCorrection;
        }

        #region Methods

        public void CorrectCoupons(int racesToCorrect)
        {
            if (racesToCorrect == 0 || racesToCorrect > this.RaceDayInfo.RaceList.Count)
            {
                racesToCorrect = this.RaceDayInfo.RaceList.Count;
            }
            this.HorseList.Clear();
            for (int raceNumber = 1; raceNumber <= racesToCorrect; raceNumber++)
            {
                HPTRace hptRace = this.RaceDayInfo.RaceList.FirstOrDefault(hr => hr.LegNr == raceNumber);

                HPTLegResult legResult = hptRace.LegResult;
                if (legResult != null)
                {
                    // Hämta värdet vid Smygen...
                    if (legResult.Value == 0 && this.RaceDayInfo.ResultMarkingBet != null)
                    {
                        var serviceLegResult = this.RaceDayInfo.ResultMarkingBet.LegResultList.FirstOrDefault(lr => lr.LegNr == legResult.LegNr);
                        if (serviceLegResult != null)
                        {
                            legResult.SystemsLeft = serviceLegResult.SystemsLeft;
                            legResult.Value = serviceLegResult.Value;
                        }
                    }
                    legResult.LegNrString = this.RaceDayInfo.BetType.Code + "-" + legResult.LegNr.ToString();
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
                        this.HorseList.Add(horse);
                        legResult.WinnerStrings[i] = legResult.Winners[i].ToString() + "-" + horse.HorseName;
                        hptRace.LegResult.Value = legResult.Value;
                        hptRace.LegResult.SystemsLeft = legResult.SystemsLeft;
                        winnerList[i] = hptRace.HorseList.First(h => h.StartNr == legResult.Winners[i]);
                    }
                    hptRace.LegResult.WinnerStrings = legResult.WinnerStrings;
                    hptRace.LegResult.WinnerList = winnerList;
                    hptRace.SplitVictory = winnerList.Count() > 1;

                    // Skapa resultatlänk till atg.se
                    ATGLinkCreator.CreateRaceResultLink(hptRace);
                }
            }


            // Beräkna vinnande rader och potentiella vinstrader i nästa lopp
            this.CouponHelper.TotalWinnings = 0;
            this.CouponHelper.TotalNumberOfAllCorrect = 0;
            this.CouponHelper.TotalNumberOfOneError = 0;
            this.CouponHelper.TotalNumberOfTwoErrors = 0;
            this.CouponHelper.TotalNumberOfThreeErrors = 0;
            foreach (HPTCoupon coupon in this.CouponHelper.CouponList)
            {
                coupon.CorrectCoupon(this.RaceDayInfo, racesToCorrect);
                this.CouponHelper.TotalNumberOfAllCorrect += coupon.NumberOfAllCorrect * coupon.BetMultiplier;
                this.CouponHelper.TotalNumberOfOneError += coupon.NumberOfOneError * coupon.BetMultiplier;
                this.CouponHelper.TotalNumberOfTwoErrors += coupon.NumberOfTwoErrors * coupon.BetMultiplier;
                this.CouponHelper.TotalNumberOfThreeErrors += coupon.NumberOfThreeErrors * coupon.BetMultiplier;
                if (this.RaceDayInfo.ResultComplete && racesToCorrect == this.RaceDayInfo.RaceList.Count)
                {
                    this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[0].PayOutAmount * coupon.NumberOfAllCorrect * coupon.BetMultiplier;
                    switch (coupon.BetType)
                    {
                        case "V65":
                            if (coupon.V6)  // 2 ggr vinsten, men bara utdelning på alla rätt
                            {
                                this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[0].PayOutAmount * coupon.NumberOfAllCorrect * coupon.BetMultiplier;
                            }
                            else
                            {
                                this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[1].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                            }
                            break;
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            if (coupon.V6)  // 2.5 ggr vinsten, men bara utdelning på alla rätt
                            {
                                this.CouponHelper.TotalWinnings += Convert.ToInt32(this.RaceDayInfo.PayOutList[0].PayOutAmount * 1.5M * coupon.NumberOfAllCorrect * coupon.BetMultiplier);
                            }
                            else
                            {
                                this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[1].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                                this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[2].PayOutAmount * coupon.NumberOfTwoErrors * coupon.BetMultiplier;
                            }
                            break;
                        case "V85":

                            this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[1].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                            this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[2].PayOutAmount * coupon.NumberOfTwoErrors * coupon.BetMultiplier;
                            this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[3].PayOutAmount * coupon.NumberOfThreeErrors * coupon.BetMultiplier;
                            break;
                        case "V4":
                        case "V5":
                            if (this.RaceDayInfo.PayOutList[0].NumberOfCorrect == this.RaceDayInfo.BetType.NumberOfRaces - 1)
                            {
                                this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[0].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
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

            // Smygen, utdelningen har inte visats upp
            if (this.RaceDayInfo.PayOutList.Count == 0 && this.RaceDayInfo.ResultMarkingBet != null && racesToCorrect == this.RaceDayInfo.RaceList.Count)
            {
                foreach (var servicePayOut in this.RaceDayInfo.ResultMarkingBet.PayOutList)
                {
                    var payOut = new HPTPayOut()
                    {
                        NumberOfCorrect = servicePayOut.NumberOfCorrect,
                        NumberOfSystems = servicePayOut.NumberOfSystems,
                        NumberOfWinningRows = 0,
                        OwnWinnings = 0,
                        PayOutAmount = servicePayOut.PayOutAmount,
                        TotalAmount = servicePayOut.TotalAmount
                    };
                    this.RaceDayInfo.PayOutList.Add(payOut);
                }
            }

            if (this.RaceDayInfo.ResultComplete && racesToCorrect == this.RaceDayInfo.RaceList.Count && this.RaceDayInfo.PayOutList.Count > 0)
            {
                this.RaceDayInfo.PayOutList[0].NumberOfWinningRows = this.CouponHelper.TotalNumberOfAllCorrect;
                switch (this.RaceDayInfo.BetType.Code)
                {
                    case "V65":
                        this.RaceDayInfo.PayOutList[1].NumberOfWinningRows = this.CouponHelper.TotalNumberOfOneError;
                        break;
                    case "V64":
                    case "V75":
                    case "GS75":
                    case "V86":
                        this.RaceDayInfo.PayOutList[1].NumberOfWinningRows = this.CouponHelper.TotalNumberOfOneError;
                        this.RaceDayInfo.PayOutList[2].NumberOfWinningRows = this.CouponHelper.TotalNumberOfTwoErrors;
                        break;
                    case "V85":
                        this.RaceDayInfo.PayOutList[1].NumberOfWinningRows = this.CouponHelper.TotalNumberOfOneError;
                        this.RaceDayInfo.PayOutList[2].NumberOfWinningRows = this.CouponHelper.TotalNumberOfTwoErrors;
                        this.RaceDayInfo.PayOutList[3].NumberOfWinningRows = this.CouponHelper.TotalNumberOfThreeErrors; // TODO
                        break;
                    case "V4":
                    case "V5":
                        if (this.RaceDayInfo.PayOutList[0].NumberOfCorrect == this.RaceDayInfo.BetType.NumberOfRaces - 1)
                        {
                            this.RaceDayInfo.PayOutList[0].NumberOfWinningRows = this.CouponHelper.TotalNumberOfOneError;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void ClearCorrection()
        {
            this.CouponHelper.TotalWinnings = 0;
            this.CouponHelper.TotalNumberOfAllCorrect = 0;
            this.CouponHelper.TotalNumberOfOneError = 0;
            this.CouponHelper.TotalNumberOfTwoErrors = 0;
            this.CouponHelper.TotalNumberOfThreeErrors = 0;
            foreach (HPTCoupon coupon in this.CouponHelper.CouponList)
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
            this.CouponHelper.TotalWinnings = 0;
            this.CouponHelper.TotalNumberOfAllCorrect = 0;
            this.CouponHelper.TotalNumberOfOneError = 0;
            this.CouponHelper.TotalNumberOfTwoErrors = 0;
            this.CouponHelper.TotalNumberOfThreeErrors = 0;
            foreach (HPTCoupon coupon in this.CouponHelper.CouponList)
            {
                coupon.CorrectCouponSimulated(this.RaceDayInfo);
                this.CouponHelper.TotalNumberOfAllCorrect += coupon.NumberOfAllCorrect * coupon.BetMultiplier;
                this.CouponHelper.TotalNumberOfOneError += coupon.NumberOfOneError * coupon.BetMultiplier;
                this.CouponHelper.TotalNumberOfTwoErrors += coupon.NumberOfTwoErrors * coupon.BetMultiplier;
                this.CouponHelper.TotalNumberOfThreeErrors += coupon.NumberOfThreeErrors * coupon.BetMultiplier;
            }

            var horseList = this.RaceDayInfo.RaceList
                .Where(r => r.LegResult != null && r.LegResult.WinnerList != null && r.LegResult.WinnerList.Any())
                .Select(r => r.LegResult.WinnerList.First()).ToArray();

            if (horseList.Count() == this.RaceDayInfo.RaceList.Count)
            {
                this.RaceDayInfo.PayOutList = new ObservableCollection<HPTPayOut>(this.RaceDayInfo.BetType.PayOutDummyList);
                this.RaceDayInfo.PayOutList[0].NumberOfWinningRows = this.CouponHelper.TotalNumberOfAllCorrect;

                var markBetSingleRow = new HPTMarkBetSingleRow(horseList);

                // Alla rätt
                this.RaceDayInfo.PayOutList[0].NumberOfWinningRows = this.CouponHelper.TotalNumberOfAllCorrect;
                this.RaceDayInfo.PayOutList[0].PayOutAmount = CalculatePayOut(horseList, this.RaceDayInfo.BetType.PoolShare * this.RaceDayInfo.BetType.RowCost);

                // Ett fel
                if (this.RaceDayInfo.PayOutList.Count > 1)
                {
                    this.RaceDayInfo.PayOutList[1].NumberOfWinningRows = this.CouponHelper.TotalNumberOfOneError;
                    this.RaceDayInfo.PayOutList[1].PayOutAmount = CalculatePayOutOneError(horseList, this.RaceDayInfo.BetType.PoolShareOneError * this.RaceDayInfo.BetType.RowCost);
                }

                // Två fel
                if (this.RaceDayInfo.PayOutList.Count > 2)
                {
                    this.RaceDayInfo.PayOutList[2].NumberOfWinningRows = this.CouponHelper.TotalNumberOfTwoErrors;
                    this.RaceDayInfo.PayOutList[2].PayOutAmount = CalculatePayOutTwoErrors(horseList, this.RaceDayInfo.BetType.PoolShareTwoErrors * this.RaceDayInfo.BetType.RowCost);
                }

                // Tre fel
                if (this.RaceDayInfo.PayOutList.Count > 3)
                {
                    this.RaceDayInfo.PayOutList[3].NumberOfWinningRows = this.CouponHelper.TotalNumberOfThreeErrors;
                    this.RaceDayInfo.PayOutList[3].PayOutAmount = CalculatePayOutThreeErrors(horseList, this.RaceDayInfo.BetType.PoolShareThreeErrors * this.RaceDayInfo.BetType.RowCost);
                }

                foreach (HPTCoupon coupon in this.CouponHelper.CouponList)
                {
                    this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[0].PayOutAmount * coupon.NumberOfAllCorrect * coupon.BetMultiplier;
                    switch (coupon.BetType)
                    {
                        case "V65":
                            if (coupon.V6)  // 2 ggr vinsten, men bara utdelning på alla rätt
                            {
                                this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[0].PayOutAmount * coupon.NumberOfAllCorrect * coupon.BetMultiplier;
                            }
                            else
                            {
                                this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[1].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                            }
                            break;
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            if (coupon.V6)  // 2.5 ggr vinsten, men bara utdelning på alla rätt
                            {
                                this.CouponHelper.TotalWinnings += Convert.ToInt32(this.RaceDayInfo.PayOutList[0].PayOutAmount * 1.5M * coupon.NumberOfAllCorrect * coupon.BetMultiplier);
                            }
                            else
                            {
                                this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[1].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                                this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[2].PayOutAmount * coupon.NumberOfTwoErrors * coupon.BetMultiplier;
                            }
                            break;
                        case "V85":
                            this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[1].PayOutAmount * coupon.NumberOfOneError * coupon.BetMultiplier;
                            this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[2].PayOutAmount * coupon.NumberOfTwoErrors * coupon.BetMultiplier;
                            this.CouponHelper.TotalWinnings += this.RaceDayInfo.PayOutList[3].PayOutAmount * coupon.NumberOfThreeErrors * coupon.BetMultiplier;    // TODO
                            break;
                        default:
                            break;
                    }
                }
            }

            this.HorseList.Clear();
            IEnumerable<HPTHorse> horseListCorrect = this.RaceDayInfo.RaceList
                 .Where(r => r.LegResult != null && r.LegResult.WinnerList != null)
                 .SelectMany(r => r.LegResult.WinnerList);

            foreach (HPTHorse correctHorse in horseListCorrect.OrderBy(r => r.ParentRace.RaceNr))
            {
                this.HorseList.Add(correctHorse);
            }
        }

        #region Beräkna värde

        internal int CalculatePayOut(IEnumerable<HPTHorse> horseList, decimal factor)
        {
            decimal rowShareStake = horseList
                   .Select(h => h.StakeDistributionShare)
                   .Aggregate((share, next) => share * next);

            decimal result = this.RaceDayInfo.MaxPayOut / (rowShareStake * this.RaceDayInfo.NumberOfGambledRowsTotal);
            result /= this.RaceDayInfo.V6Factor;

            if (result > this.RaceDayInfo.MaxPayOut)
            {
                result = this.RaceDayInfo.MaxPayOut;
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
                serviceConnector.GetResultMarkingBetByTrackAndDate(this.RaceDayInfo.BetType.Code, this.RaceDayInfo.TrackId, this.RaceDayInfo.RaceDayDate, this.RaceDayInfo, setValue);
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
            serviceConnector.GetResultMarkingBetByTrackAndDate(this.RaceDayInfo.BetType.Code, this.RaceDayInfo.TrackId, this.RaceDayInfo.RaceDayDate, this.RaceDayInfo, setValues);
            ReplaceScratchedHorses();
        }

        public bool ReplaceScratchedHorses()
        {
            bool replaced = false;
            foreach (HPTCoupon coupon in this.CouponHelper.CouponList)
            {
                foreach (HPTCouponRace couponRace in coupon.CouponRaceList)
                {
                    HPTRace race = this.RaceDayInfo.RaceList.Where(hr => hr.LegNr == couponRace.LegNr).FirstOrDefault();
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
                return this.couponHelper;
            }
            set
            {
                this.couponHelper = value;
                OnPropertyChanged("CouponHelper");
            }
        }

        private HPTRaceDayInfo raceDayInfo;
        public HPTRaceDayInfo RaceDayInfo
        {
            get
            {
                return this.raceDayInfo;
            }
            set
            {
                this.raceDayInfo = value;
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
