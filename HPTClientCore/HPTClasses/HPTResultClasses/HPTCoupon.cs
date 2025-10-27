using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            this.CouponRaceList = new ObservableCollection<HPTCouponRace>();
            //this.ARow = new int[this.NumberOfRaces];
        }

        private int betMultiplier;
        [DataMember]
        public int BetMultiplier
        {
            get
            {
                return this.betMultiplier;
            }
            set
            {
                this.betMultiplier = value;
                OnPropertyChanged("BetMultiplier");
            }
        }

        private int numberOfCorrect;
        [DataMember]
        public int NumberOfCorrect
        {
            get
            {
                return this.numberOfCorrect;
            }
            set
            {
                this.numberOfCorrect = value;
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
            foreach (var couponRace in this.CouponRaceList)
            {
                if (couponRace.StartNrList == null)
                {
                    couponRace.StartNrList = couponRace.HorseList.Select(h => h.StartNr).ToList();
                }
            }
            this.raceDayInfo = raceDayInfo;
            this.ARow = new int[raceDayInfo.RaceList.Count];
            this.NumberOfFinishedLegs = raceDayInfo.NumberOfFinishedRaces;
            if (racesToCorrect == 0 || racesToCorrect > raceDayInfo.NumberOfFinishedRaces)
            {
                racesToCorrect = raceDayInfo.NumberOfFinishedRaces;
            }
            this.NumberOfCorrect = 0;
            for (int raceNumber = 1; raceNumber <= racesToCorrect; raceNumber++)
            {
                HPTRace hptRace = raceDayInfo.RaceList.First(r => r.LegNr == raceNumber);
                if (hptRace.LegResult != null && hptRace.LegResult.WinnerList != null)
                {
                    HPTLegResult hptLegResult = hptRace.LegResult;
                    HPTCouponRace couponRace = this.CouponRaceList.First(cr => cr.LegNr == raceNumber);
                    int correctNumbersOnCouponRace = couponRace.StartNrList.Intersect(hptLegResult.Winners).Count();
                    this.NumberOfCorrect += correctNumbersOnCouponRace == 0 ? 0 : 1;
                }
            }
            SetNumberOfWinningRows();
            SetNumberOfCorrectsColor();
        }

        public void CorrectCouponSimulated(HPTRaceDayInfo raceDayInfo)
        {
            this.raceDayInfo = raceDayInfo;
            this.ARow = new int[raceDayInfo.RaceList.Count];
            this.NumberOfFinishedLegs = raceDayInfo.NumberOfFinishedRaces;

            this.NumberOfCorrect = 0;
            for (int raceNumber = 1; raceNumber <= raceDayInfo.RaceList.Count; raceNumber++)
            {
                HPTRace hptRace = raceDayInfo.RaceList.First(r => r.LegNr == raceNumber);
                if (hptRace.LegResult != null && hptRace.LegResult.WinnerList != null)
                {
                    HPTLegResult hptLegResult = hptRace.LegResult;
                    HPTCouponRace couponRace = this.CouponRaceList.First(cr => cr.LegNr == raceNumber);
                    int correctNumbersOnCouponRace = couponRace.StartNrList.Intersect(hptLegResult.Winners).Count();
                    this.NumberOfCorrect += correctNumbersOnCouponRace == 0 ? 0 : 1;
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
                return this.numberOfAllCorrect;
            }
            set
            {
                this.numberOfAllCorrect = value;
                OnPropertyChanged("NumberOfAllCorrect");
            }
        }

        private int numberOfOneError;
        [DataMember]
        public int NumberOfOneError
        {
            get
            {
                return this.numberOfOneError;
            }
            set
            {
                this.numberOfOneError = value;
                OnPropertyChanged("NumberOfOneError");
            }
        }

        private int numberOfTwoErrors;
        [DataMember]
        public int NumberOfTwoErrors
        {
            get
            {
                return this.numberOfTwoErrors;
            }
            set
            {
                this.numberOfTwoErrors = value;
                OnPropertyChanged("NumberOfTwoErrors");
            }
        }

        private int numberOfThreeErrors;
        [DataMember]
        public int NumberOfThreeErrors
        {
            get
            {
                return this.numberOfThreeErrors;
            }
            set
            {
                this.numberOfThreeErrors = value;
                OnPropertyChanged("NumberOfThreeErrors");
            }
        }

        private int couponId;
        [DataMember]
        public int CouponId
        {
            get
            {
                return this.couponId;
            }
            set
            {
                this.couponId = value;
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
                    return this.CouponId;
                }
                return this.couponIdFile;
            }
            set
            {
                this.couponIdFile = value;
                OnPropertyChanged("CouponIdFile");
            }
        }

        //// För att få rätt kupongnummer när man måste skapa flera filer
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
                return this.date;
            }
            set
            {
                this.date = value;
                OnPropertyChanged("Date");
            }
        }

        private string trackCode;
        [DataMember]
        public string TrackCode
        {
            get
            {
                return this.trackCode;
            }
            set
            {
                this.trackCode = value;
                OnPropertyChanged("TrackCode");
            }
        }

        private bool v6;
        [DataMember]
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
            }
        }

        private bool canWin;
        public bool CanWin
        {
            get
            {
                return this.canWin;
            }
            set
            {
                this.canWin = value;
                OnPropertyChanged("CanWin");
            }
        }

        private void SetNumberOfCorrectsColor()
        {
            Color c = Colors.White;
            if (this.NumberOfFinishedLegs > 0)
            {
                if (this.NumberOfAllCorrect > 0)
                {
                    c = HPTConfig.Config.ColorGood;
                    this.CanWin = true;
                }
                else if (this.NumberOfOneError > 0 && !this.V6)
                {
                    switch (this.BetType)
                    {
                        case "V64":
                        case "V65":
                        case "V75":
                        case "V85":
                        case "GS75":
                        case "V86":
                            c = HPTConfig.Config.ColorMedium;
                            this.CanWin = true;
                            break;
                        default:
                            c = HPTConfig.Config.ColorBad;
                            this.CanWin = false;
                            break;
                    }
                }
                else if (this.NumberOfTwoErrors > 0 && !this.V6)
                {
                    switch (this.BetType)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V85":
                        case "V86":
                            c = HPTConfig.Config.ColorMedium;
                            this.CanWin = true;
                            break;
                        default:
                            c = HPTConfig.Config.ColorBad;
                            this.CanWin = false;
                            break;
                    }
                }   // TODO
                else if (this.NumberOfThreeErrors > 0 && !this.V6)
                {
                    switch (this.BetType)
                    {
                        case "V85":
                            c = HPTConfig.Config.ColorMedium;
                            this.CanWin = true;
                            break;
                        default:
                            c = HPTConfig.Config.ColorBad;
                            this.CanWin = false;
                            break;
                    }
                }
                else
                {
                    c = HPTConfig.Config.ColorBad;
                    this.CanWin = false;
                }
            }
            else
            {
                c = Colors.LightGray;
            }
            this.NumberOfCorrectsColor = new SolidColorBrush(c);
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
                this.numberOfCorrectsColor = value;
                OnPropertyChanged("NumberOfCorrectsColor");
            }
        }

        private int systemSize;
        [DataMember]
        public int SystemSize
        {
            get
            {
                return this.systemSize;
            }
            set
            {
                this.systemSize = value;
                OnPropertyChanged("SystemSize");
            }
        }

        public decimal SystemSizeATG
        {
            get
            {
                if (this.V6)
                {
                    switch (this.BetType)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            return this.BetMultiplier * 2.5M;
                        case "V65":
                            return this.BetMultiplier * 2M;
                        default:
                            break;
                    }
                }
                return this.BetMultiplier;
            }
        }

        private int payOutAmount;
        [DataMember]
        public int PayOutAmount
        {
            get
            {
                return this.payOutAmount;
            }
            set
            {
                this.payOutAmount = value;
                OnPropertyChanged("PayOutAmount");
            }
        }


        public void SetNumberOfWinningRows()
        {
            this.NumberOfAllCorrect = 0;
            this.NumberOfOneError = 0;
            this.NumberOfTwoErrors = 0;
            if (this.NumberOfFinishedLegs > 0)
            {
                SetNumberOfWinningRows(1);
            }
        }

        private void SetNumberOfWinningRows(int legNr)
        {
            HPTCouponRace couponRace = this.CouponRaceList.First(cr => cr.LegNr == legNr);
            foreach (int startNr in couponRace.StartNrList)
            {
                this.ARow[legNr - 1] = startNr;
                if (legNr == this.NumberOfFinishedLegs)
                {
                    int numberOfErrors = 0;
                    for (int i = 1; i <= this.NumberOfFinishedLegs; i++)
                    {
                        HPTRace race = this.raceDayInfo.RaceList.First(r => r.LegNr == i);
                        if (race.LegResult != null && race.LegResult.WinnerList != null && race.LegResult.WinnerList[0] != null)
                        {
                            //HPTLegResult legResult = race.LegResult;
                            int winner = race.LegResult.Winners[0];
                            bool correct = winner == this.ARow[i - 1];

                            if (!correct && race.LegResult.Winners.Length > 1)
                            {
                                winner = race.LegResult.Winners[1];
                                correct = winner == this.ARow[i - 1];
                            }
                            numberOfErrors += correct ? 0 : 1;
                        }
                    }
                    this.NumberOfAllCorrect += numberOfErrors == 0 ? 1 : 0;
                    this.NumberOfOneError += numberOfErrors == 1 ? 1 : 0;
                    this.NumberOfTwoErrors += numberOfErrors == 2 ? 1 : 0;
                    this.NumberOfThreeErrors += numberOfErrors == 3 ? 1 : 0;
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
            this.singleRowHorseList = new HPTHorse[this.CouponRaceList.Count];
            this.singleRowList = new List<HPTMarkBetSingleRow>();

            CreateSingleRows(0);

            return this.singleRowList;
        }

        internal void CreateSingleRows(int position)
        {
            if (position == this.CouponRaceList.Count)
            {
                var singleRow = new HPTMarkBetSingleRow(this.singleRowHorseList)
                {
                    BetMultiplier = this.BetMultiplier,
                    V6 = this.V6
                };
                this.singleRowList.Add(singleRow);
                return;
            }
            var couponRace = this.CouponRaceList[position];
            foreach (var horse in couponRace.HorseList)
            {
                this.singleRowHorseList[position] = horse;
                CreateSingleRows(position + 1);
            }
        }

        public string ToCouponString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Kupong " + this.couponId.ToString());

            if (this.V6 || this.BetMultiplier > 1)
            {
                sb.Append(" (");
                if (this.V6)
                {
                    switch (this.BetType)
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
                if (this.V6 && this.BetMultiplier > 1)
                {
                    sb.Append(" och ");
                }
                if (this.BetMultiplier > 1)
                {
                    sb.Append(this.BetMultiplier);
                    sb.Append(" X Flerbong");
                }
                sb.Append(")");
            }
            sb.AppendLine();

            IOrderedEnumerable<HPTCouponRace> orderedRaceList = this.CouponRaceList.OrderBy(cr => cr.LegNr);

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
