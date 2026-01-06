using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTCoupon : Notifier
    {
        public HPTCoupon()
        {
            //this.CouponRaceListSorted = new SortedList<int, HPTCouponRace>();
            CouponRaceList = new ObservableCollection<HPTCouponRace>();
            //this.ARow = new int[this.NumberOfRaces];
        }

        private int betMultiplier;
        [DataMember]
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
            }
        }

        private int numberOfCorrect;
        [DataMember]
        public int NumberOfCorrect
        {
            get
            {
                return numberOfCorrect;
            }
            set
            {
                numberOfCorrect = value;
                OnPropertyChanged("NumberOfCorrect");
            }
        }

        [DataMember]
        public string BetType { get; set; }

        [DataMember]
        public int RaceNumber { get; set; }

        [DataMember]
        public int Stake { get; set; }

        private HPTRaceDayInfo raceDayInfo;
        public void CorrectCoupon(HPTRaceDayInfo raceDayInfo, int racesToCorrect)
        {
            foreach (var couponRace in CouponRaceList)
            {
                if (couponRace.StartNrList == null)
                {
                    couponRace.StartNrList = couponRace.HorseList.Select(h => h.StartNr).ToList();
                }
            }
            this.raceDayInfo = raceDayInfo;
            ARow = new int[raceDayInfo.RaceList.Count];
            NumberOfFinishedLegs = raceDayInfo.NumberOfFinishedRaces;
            if (racesToCorrect == 0 || racesToCorrect > raceDayInfo.NumberOfFinishedRaces)
            {
                racesToCorrect = raceDayInfo.NumberOfFinishedRaces;
            }
            NumberOfCorrect = 0;
            for (int raceNumber = 1; raceNumber <= racesToCorrect; raceNumber++)
            {
                HPTRace hptRace = raceDayInfo.RaceList.First(r => r.LegNr == raceNumber);
                if (hptRace.LegResult != null && hptRace.LegResult.WinnerList != null)
                {
                    HPTLegResult hptLegResult = hptRace.LegResult;
                    HPTCouponRace couponRace = CouponRaceList.First(cr => cr.LegNr == raceNumber);
                    int correctNumbersOnCouponRace = couponRace.StartNrList.Intersect(hptLegResult.Winners).Count();
                    NumberOfCorrect += correctNumbersOnCouponRace == 0 ? 0 : 1;
                }
            }
            SetNumberOfWinningRows();
            SetNumberOfCorrectsColor();
        }

        public void CorrectCouponSimulated(HPTRaceDayInfo raceDayInfo)
        {
            this.raceDayInfo = raceDayInfo;
            ARow = new int[raceDayInfo.RaceList.Count];
            NumberOfFinishedLegs = raceDayInfo.NumberOfFinishedRaces;

            NumberOfCorrect = 0;
            for (int raceNumber = 1; raceNumber <= raceDayInfo.RaceList.Count; raceNumber++)
            {
                HPTRace hptRace = raceDayInfo.RaceList.First(r => r.LegNr == raceNumber);
                if (hptRace.LegResult != null && hptRace.LegResult.WinnerList != null)
                {
                    HPTLegResult hptLegResult = hptRace.LegResult;
                    HPTCouponRace couponRace = CouponRaceList.First(cr => cr.LegNr == raceNumber);
                    int correctNumbersOnCouponRace = couponRace.StartNrList.Intersect(hptLegResult.Winners).Count();
                    NumberOfCorrect += correctNumbersOnCouponRace == 0 ? 0 : 1;
                }
            }
            SetNumberOfWinningRows();
            SetNumberOfCorrectsColor();
        }

        public int[] ARow { get; set; }

        [DataMember]
        public int NumberOfFinishedLegs { get; set; }

        //[XmlIgnore]
        //public SortedList<int, HPTCouponRace> CouponRaceListSorted { get; set; }

        [DataMember]
        public ObservableCollection<HPTCouponRace> CouponRaceList { get; set; }

        private int numberOfAllCorrect;
        [DataMember]
        public int NumberOfAllCorrect
        {
            get
            {
                return numberOfAllCorrect;
            }
            set
            {
                numberOfAllCorrect = value;
                OnPropertyChanged("NumberOfAllCorrect");
            }
        }

        private int numberOfOneError;
        [DataMember]
        public int NumberOfOneError
        {
            get
            {
                return numberOfOneError;
            }
            set
            {
                numberOfOneError = value;
                OnPropertyChanged("NumberOfOneError");
            }
        }

        private int numberOfTwoErrors;
        [DataMember]
        public int NumberOfTwoErrors
        {
            get
            {
                return numberOfTwoErrors;
            }
            set
            {
                numberOfTwoErrors = value;
                OnPropertyChanged("NumberOfTwoErrors");
            }
        }

        private int numberOfThreeErrors;
        [DataMember]
        public int NumberOfThreeErrors
        {
            get
            {
                return numberOfThreeErrors;
            }
            set
            {
                numberOfThreeErrors = value;
                OnPropertyChanged("NumberOfThreeErrors");
            }
        }

        private int couponId;
        [DataMember]
        public int CouponId
        {
            get
            {
                return couponId;
            }
            set
            {
                couponId = value;
                OnPropertyChanged("CouponId");
            }
        }

        private int couponIdFile;
        [DataMember]
        public int CouponIdFile
        {
            get
            {
                if (couponIdFile == 0)
                {
                    return CouponId;
                }
                return couponIdFile;
            }
            set
            {
                couponIdFile = value;
                OnPropertyChanged("CouponIdFile");
            }
        }

        //// F�r att f� r�tt kupongnummer n�r man m�ste skapa flera filer
        //public int CouponIdFile
        //{
        //    get
        //    {
        //        return this.couponId % 9999;
        //    }
        //}

        private DateTime date;
        [DataMember]
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        private string trackCode;
        [DataMember]
        public string TrackCode
        {
            get
            {
                return trackCode;
            }
            set
            {
                trackCode = value;
                OnPropertyChanged("TrackCode");
            }
        }

        private bool v6;
        [DataMember]
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

        private bool canWin;
        public bool CanWin
        {
            get
            {
                return canWin;
            }
            set
            {
                canWin = value;
                OnPropertyChanged("CanWin");
            }
        }

        private void SetNumberOfCorrectsColor()
        {
            Color c = Colors.White;
            if (NumberOfFinishedLegs > 0)
            {
                if (NumberOfAllCorrect > 0)
                {
                    c = HPTConfig.Config.ColorGood;
                    CanWin = true;
                }
                else if (NumberOfOneError > 0 && !V6)
                {
                    switch (BetType)
                    {
                        case "V64":
                        case "V65":
                        case "V75":
                        case "V85":
                        case "GS75":
                        case "V86":
                            c = HPTConfig.Config.ColorMedium;
                            CanWin = true;
                            break;
                        default:
                            c = HPTConfig.Config.ColorBad;
                            CanWin = false;
                            break;
                    }
                }
                else if (NumberOfTwoErrors > 0 && !V6)
                {
                    switch (BetType)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V85":
                        case "V86":
                            c = HPTConfig.Config.ColorMedium;
                            CanWin = true;
                            break;
                        default:
                            c = HPTConfig.Config.ColorBad;
                            CanWin = false;
                            break;
                    }
                }   // TODO
                else if (NumberOfThreeErrors > 0 && !V6)
                {
                    switch (BetType)
                    {
                        case "V85":
                            c = HPTConfig.Config.ColorMedium;
                            CanWin = true;
                            break;
                        default:
                            c = HPTConfig.Config.ColorBad;
                            CanWin = false;
                            break;
                    }
                }
                else
                {
                    c = HPTConfig.Config.ColorBad;
                    CanWin = false;
                }
            }
            else
            {
                c = Colors.LightGray;
            }
            NumberOfCorrectsColor = new SolidColorBrush(c);
        }

        private Brush numberOfCorrectsColor;
        [XmlIgnore]
        public Brush NumberOfCorrectsColor
        {
            get
            {
                return numberOfCorrectsColor;
            }
            set
            {
                numberOfCorrectsColor = value;
                OnPropertyChanged("NumberOfCorrectsColor");
            }
        }

        private int systemSize;
        [DataMember]
        public int SystemSize
        {
            get
            {
                return systemSize;
            }
            set
            {
                systemSize = value;
                OnPropertyChanged("SystemSize");
            }
        }

        public decimal SystemSizeATG
        {
            get
            {
                if (V6)
                {
                    switch (BetType)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            return BetMultiplier * 2.5M;
                        case "V65":
                            return BetMultiplier * 2M;
                        default:
                            break;
                    }
                }
                return BetMultiplier;
            }
        }

        private int payOutAmount;
        [DataMember]
        public int PayOutAmount
        {
            get
            {
                return payOutAmount;
            }
            set
            {
                payOutAmount = value;
                OnPropertyChanged("PayOutAmount");
            }
        }


        public void SetNumberOfWinningRows()
        {
            NumberOfAllCorrect = 0;
            NumberOfOneError = 0;
            NumberOfTwoErrors = 0;
            NumberOfThreeErrors = 0;
            if (NumberOfFinishedLegs > 0)
            {
                SetNumberOfWinningRows(1);
            }
        }

        private void SetNumberOfWinningRows(int legNr)
        {
            HPTCouponRace couponRace = CouponRaceList.First(cr => cr.LegNr == legNr);
            foreach (int startNr in couponRace.StartNrList)
            {
                ARow[legNr - 1] = startNr;
                if (legNr == NumberOfFinishedLegs)
                {
                    int numberOfErrors = 0;
                    for (int i = 1; i <= NumberOfFinishedLegs; i++)
                    {
                        HPTRace race = raceDayInfo.RaceList.First(r => r.LegNr == i);
                        if (race.LegResult != null && race.LegResult.WinnerList != null && race.LegResult.WinnerList[0] != null)
                        {
                            //HPTLegResult legResult = race.LegResult;
                            int winner = race.LegResult.Winners[0];
                            bool correct = winner == ARow[i - 1];

                            if (!correct && race.LegResult.Winners.Length > 1)
                            {
                                winner = race.LegResult.Winners[1];
                                correct = winner == ARow[i - 1];
                            }
                            numberOfErrors += correct ? 0 : 1;
                        }
                    }
                    NumberOfAllCorrect += numberOfErrors == 0 ? 1 : 0;
                    NumberOfOneError += numberOfErrors == 1 ? 1 : 0;
                    NumberOfTwoErrors += numberOfErrors == 2 ? 1 : 0;
                    NumberOfThreeErrors += numberOfErrors == 3 ? 1 : 0;
                }
                else
                {
                    SetNumberOfWinningRows(legNr + 1);
                }
            }
        }

        internal HPTHorse[] singleRowHorseList;
        internal List<HPTMarkBetSingleRow> singleRowList;
        internal List<HPTMarkBetSingleRow> CreateSingleRows()
        {
            singleRowHorseList = new HPTHorse[CouponRaceList.Count];
            singleRowList = new List<HPTMarkBetSingleRow>();

            CreateSingleRows(0);

            return singleRowList;
        }

        internal void CreateSingleRows(int position)
        {
            if (position == CouponRaceList.Count)
            {
                var singleRow = new HPTMarkBetSingleRow(singleRowHorseList)
                {
                    BetMultiplier = BetMultiplier,
                    V6 = V6
                };
                singleRowList.Add(singleRow);
                return;
            }
            var couponRace = CouponRaceList[position];
            foreach (var horse in couponRace.HorseList)
            {
                singleRowHorseList[position] = horse;
                CreateSingleRows(position + 1);
            }
        }

        public string ToCouponString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Kupong " + couponId.ToString());

            if (V6 || BetMultiplier > 1)
            {
                sb.Append(" (");
                if (V6)
                {
                    switch (BetType)
                    {
                        case "V64":
                        case "V65":
                            sb.Append("V6");
                            break;
                        case "V75":
                        case "GS75":
                            sb.Append("V7");
                            break;
                        case "V86":
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
            sb.AppendLine();

            IOrderedEnumerable<HPTCouponRace> orderedRaceList = CouponRaceList.OrderBy(cr => cr.LegNr);

            foreach (var hptCouponRace in orderedRaceList)
            {
                sb.Append("Avd ");
                sb.Append(hptCouponRace.LegNr);
                sb.Append(": ");

                string startNumberString = hptCouponRace.StartNrList
                    .OrderBy(sn => sn)
                    .Select(sn => sn.ToString())
                    .Aggregate((s, next) => s + ", " + next);

                sb.Append(startNumberString);

                //foreach (var i in hptCouponRace.StartNrList)
                //{
                //    sb.Append(i);
                //    sb.Append(", ");
                //}
                //sb.Remove(sb.Length - 2, 2);
                sb.Append(" (");
                sb.Append(hptCouponRace.Reserv1);
                sb.Append(", ");
                sb.Append(hptCouponRace.Reserv2);
                sb.AppendLine(")");
                //sb.Append("R1: ");
                //sb.Append(hptCouponRace.Reserv1);
                //sb.Append(", R2: ");
                //sb.Append(hptCouponRace.Reserv2);
                //sb.AppendLine();                  
            }
            return sb.ToString();
        }
    }
}
