using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTComplementaryReductionRule : HPTNumberOfWinnersReductionRule
    {
        public HPTComplementaryReductionRule()
        {
            this.horseList = new ObservableCollection<HPTHorse>();
        }

        public HPTComplementaryReductionRule(int numberOfRaces, bool use)
        {
            this.horseList = new ObservableCollection<HPTHorse>();
            this.Use = use;
            this.NumberOfRaces = numberOfRaces;

            this.NumberOfWinnersList = new ObservableCollection<HPTNumberOfWinners>();
            for (int i = 0; i <= numberOfRaces; i++)
            {
                HPTNumberOfWinners now = new HPTNumberOfWinners();
                now.NumberOfWinners = i;
                this.NumberOfWinnersList.Add(now);
            }
            this.NumberOfWinnersList.First(now => now.NumberOfWinners == 0).Selectable = true;
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            if (this.HighestLegNumber > numberOfRacesToTest || this.LowestLegNumber > numberOfRacesToTest || !this.Use)
            {
                return true;
            }

            int numberOfHorses = horseList
                .Take(numberOfRacesToTest)
                .Intersect(this.HorseList)
                .Count();

            if (numberOfHorses > this.MaxNumberOfX) // Maxantalet har redan överskridits innan alla lopp kontrollerats
            {
                return false;
            }
            if (numberOfHorses + markBet.BetType.NumberOfRaces - numberOfRacesToTest < this.MinNumberOfX)   // Det går inte att komma upp i minimiantalet med kvarvarande lopp
            {
                return false;
            }
            return true;
        }

        public override void Reset()
        {
            base.Reset();
            if (this.HorseList.Any())
            {
                this.LowestLegNumber = this.HorseList.Min(h => h.ParentRace.LegNr);
                this.HighestLegNumber = this.HorseList.Max(h => h.ParentRace.LegNr);
            }
        }

        [XmlIgnore]
        public int LowestLegNumber { get; set; }

        [XmlIgnore]
        public int HighestLegNumber { get; set; }

        public void UpdateSelectable()
        {
            this.NumberOfSelected = 0;
            int[] raceNumbers = new int[this.HorseList.Count];
            for (int i = 0; i < this.HorseList.Count; i++)
            {
                //raceNumbers[i] = this.HorseList[i].ParentRace.LegNr;
                raceNumbers[i] = this.HorseList.ElementAt(i).ParentRace.LegNr;
            }
            int antal = raceNumbers.Distinct().Count();
            for (int i = 0; i <= this.NumberOfRaces; i++)
            {
                HPTNumberOfWinners hptNow = this.NumberOfWinnersList.First(now => now.NumberOfWinners == i);
                if (hptNow.NumberOfWinners > antal)
                {
                    hptNow.Selectable = false;
                    hptNow.Selected = false;
                }
                else
                {
                    hptNow.Selectable = true;
                    this.NumberOfSelected += hptNow.Selected ? 1 : 0;
                }
            }
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            int numberOfHorses = this.HorseList.Intersect(singleRow.HorseList).Count();
            return this.NumberOfWinnersList.First(now => now.NumberOfWinners == numberOfHorses).Selected;
        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            // Skapa dictionary för att kontrollera hur många vinstrader villkoret skulle gett
            if (markBet.RaceDayInfo.ResultComplete)
            {
                int numberOfXHorses = markBet.CouponCorrector.HorseList
                    .Intersect(this.HorseList)
                    .Count();

                this.RuleResultForCorrectRow = numberOfXHorses.ToString() + " U-Häst(ar)";
            }
            return true;
        }

        //public void SetShortDescriptionString()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach (HPTHorse horse in this.HorseList)
        //    {
        //        sb.Append(horse.HorseName);
        //        sb.Append(", ");
        //    }
        //    this.ShortDescription = sb.ToString();
        //}

        private string shortDescription;
        [DataMember]
        public string ShortDescription
        {
            get
            {
                return shortDescription;
            }
            set
            {
                shortDescription = value;
                OnPropertyChanged("ShortDescription");
            }
        }

        private ICollection<HPTHorse> horseList;
        [XmlIgnore]
        public ICollection<HPTHorse> HorseList
        {
            get
            {
                return this.horseList;
            }
            set
            {
                this.horseList = value;
                OnPropertyChanged("HorseList");
            }
        }

        [DataMember]
        public List<HPTHorseLight> HorseLightList { get; set; }

        public override string ReductionTypeString
        {
            get
            {
                return "Utgång: " + this.NumberOfWinnersString + " vinnare";
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            var sb = new StringBuilder();

            foreach (HPTHorse horse in this.HorseList.OrderBy(h => h.ParentRace.LegNr).ThenBy(h => h.StartNr))
            {
                sb.Append(horse.ParentRace.LegNrString);
                sb.Append(": ");
                sb.Append(horse.StartNr);
                sb.Append(" - ");
                sb.AppendLine(horse.HorseName);
            }
            this.ClipboardString = this.ReductionTypeString + "\r\n" + sb.ToString();
            return sb.ToString();
        }
    }
}
