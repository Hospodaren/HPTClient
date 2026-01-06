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
            horseList = new ObservableCollection<HPTHorse>();
        }

        public HPTComplementaryReductionRule(int numberOfRaces, bool use)
        {
            horseList = new ObservableCollection<HPTHorse>();
            Use = use;
            NumberOfRaces = numberOfRaces;

            NumberOfWinnersList = new ObservableCollection<HPTNumberOfWinners>();
            for (int i = 0; i <= numberOfRaces; i++)
            {
                HPTNumberOfWinners now = new HPTNumberOfWinners();
                now.NumberOfWinners = i;
                NumberOfWinnersList.Add(now);
            }
            NumberOfWinnersList.First(now => now.NumberOfWinners == 0).Selectable = true;
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            if (HighestLegNumber > numberOfRacesToTest || LowestLegNumber > numberOfRacesToTest || !Use)
            {
                return true;
            }

            int numberOfHorses = horseList
                .Take(numberOfRacesToTest)
                .Intersect(HorseList)
                .Count();

            if (numberOfHorses > MaxNumberOfX) // Maxantalet har redan överskridits innan alla lopp kontrollerats
            {
                return false;
            }
            if (numberOfHorses + markBet.BetType.NumberOfRaces - numberOfRacesToTest < MinNumberOfX)   // Det går inte att komma upp i minimiantalet med kvarvarande lopp
            {
                return false;
            }
            return true;
        }

        public override void Reset()
        {
            base.Reset();
            if (HorseList.Any())
            {
                LowestLegNumber = HorseList.Min(h => h.ParentRace.LegNr);
                HighestLegNumber = HorseList.Max(h => h.ParentRace.LegNr);
            }
        }

        [XmlIgnore]
        public int LowestLegNumber { get; set; }

        [XmlIgnore]
        public int HighestLegNumber { get; set; }

        public void UpdateSelectable()
        {
            NumberOfSelected = 0;
            int[] raceNumbers = new int[HorseList.Count];
            for (int i = 0; i < HorseList.Count; i++)
            {
                //raceNumbers[i] = this.HorseList[i].ParentRace.LegNr;
                raceNumbers[i] = HorseList.ElementAt(i).ParentRace.LegNr;
            }
            int antal = raceNumbers.Distinct().Count();
            for (int i = 0; i <= NumberOfRaces; i++)
            {
                HPTNumberOfWinners hptNow = NumberOfWinnersList.First(now => now.NumberOfWinners == i);
                if (hptNow.NumberOfWinners > antal)
                {
                    hptNow.Selectable = false;
                    hptNow.Selected = false;
                }
                else
                {
                    hptNow.Selectable = true;
                    NumberOfSelected += hptNow.Selected ? 1 : 0;
                }
            }
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            int numberOfHorses = HorseList.Intersect(singleRow.HorseList).Count();
            return NumberOfWinnersList.First(now => now.NumberOfWinners == numberOfHorses).Selected;
        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            // Skapa dictionary för att kontrollera hur många vinstrader villkoret skulle gett
            if (markBet.RaceDayInfo.ResultComplete)
            {
                int numberOfXHorses = markBet.CouponCorrector.HorseList
                    .Intersect(HorseList)
                    .Count();

                RuleResultForCorrectRow = numberOfXHorses.ToString() + " U-Häst(ar)";
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
                return horseList;
            }
            set
            {
                horseList = value;
                OnPropertyChanged("HorseList");
            }
        }

        [DataMember]
        public List<HPTHorseLight> HorseLightList { get; set; }

        public override string ReductionTypeString
        {
            get
            {
                return "Utgång: " + NumberOfWinnersString + " vinnare";
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            var sb = new StringBuilder();

            foreach (HPTHorse horse in HorseList.OrderBy(h => h.ParentRace.LegNr).ThenBy(h => h.StartNr))
            {
                sb.Append(horse.ParentRace.LegNrString);
                sb.Append(": ");
                sb.Append(horse.StartNr);
                sb.Append(" - ");
                sb.AppendLine(horse.HorseName);
            }
            ClipboardString = ReductionTypeString + "\r\n" + sb.ToString();
            return sb.ToString();
        }
    }
}
