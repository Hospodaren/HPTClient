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

        [DataMember]
        public int BetMultiplier
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int NumberOfCorrect
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
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
                    couponRace.StartNrList = couponRace.HorseList
                        .Select(h => h.StartNr)
                        .ToList();
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
            for (var raceNumber = 1; raceNumber <= racesToCorrect; raceNumber++)
            {
                // var hptRace = raceDayInfo.RaceList.First(r => r.LegNr == raceNumber);
                var hptRace = raceDayInfo.RaceDictionary[raceNumber];
                if (hptRace.LegResult != null && hptRace.LegResult.WinnerList != null)
                {
                    var hptLegResult = hptRace.LegResult;
                    var couponRace = CouponRaceList.First(cr => cr.LegNr == raceNumber);
                    var correctNumbersOnCouponRace = couponRace.StartNrList.Intersect(hptLegResult.Winners).Count();
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
            for (var raceNumber = 1; raceNumber <= raceDayInfo.RaceList.Count; raceNumber++)
            {
                // var hptRace = raceDayInfo.RaceList.First(r => r.LegNr == raceNumber);
                var hptRace = raceDayInfo.RaceDictionary[raceNumber];
                if (hptRace.LegResult != null && hptRace.LegResult.WinnerList != null)
                {
                    var hptLegResult = hptRace.LegResult;
                    var couponRace = CouponRaceList.First(cr => cr.LegNr == raceNumber);
                    var correctNumbersOnCouponRace = couponRace.StartNrList.Intersect(hptLegResult.Winners).Count();
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

        [DataMember]
        public int NumberOfAllCorrect
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int NumberOfOneError
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int NumberOfTwoErrors
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int NumberOfThreeErrors
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int CouponIdFile
        {
            get
            {
                if (field == 0)
                {
                    return CouponId;
                }
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged();
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

        [DataMember]
        public DateTime Date
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string TrackCode
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool V6
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public bool CanWin
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        private void SetNumberOfCorrectsColor()
        {
            var c = Colors.White;
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

        [XmlIgnore]
        public Brush NumberOfCorrectsColor
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int SystemSize
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
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

        [DataMember]
        public int PayOutAmount
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
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
            var couponRace = CouponRaceList.First(cr => cr.LegNr == legNr);
            foreach (var startNr in couponRace.StartNrList)
            {
                ARow[legNr - 1] = startNr;
                if (legNr == NumberOfFinishedLegs)
                {
                    var numberOfErrors = 0;
                    for (var i = 1; i <= NumberOfFinishedLegs; i++)
                    {
                        // var race = raceDayInfo.RaceList.First(r => r.LegNr == i);
                        var race = raceDayInfo.RaceDictionary[i];
                        if (race.LegResult != null && race.LegResult.WinnerList != null && race.LegResult.WinnerList[0] != null)
                        {
                            //HPTLegResult legResult = race.LegResult;
                            var winner = race.LegResult.Winners[0];
                            var correct = winner == ARow[i - 1];

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
            var sb = new StringBuilder();

            sb.Append($"Kupong {couponId}");

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

            var orderedRaceList = CouponRaceList.OrderBy(cr => cr.LegNr);

            foreach (var hptCouponRace in orderedRaceList)
            {
                sb.Append("Avd ");
                sb.Append(hptCouponRace.LegNr);
                sb.Append(": ");

                var startNumberString = hptCouponRace.StartNrList
                    .OrderBy(sn => sn)
                    .Select(sn => sn.ToString())
                    .Aggregate((s, next) => $"{s}, {next}");

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
