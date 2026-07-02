using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace HPTClient
{
    [XmlInclude(typeof(HPTDriverReductionRule))]
    [XmlInclude(typeof(HPTTrainerReductionRule))]
    [DataContract]
    [KnownType(typeof(HPTDriverReductionRule))]
    [KnownType(typeof(HPTTrainerReductionRule))]
    public class HPTPersonReductionRule : HPTNumberOfWinnersReductionRule
    {
        public HPTPersonReductionRule()
        {
            PersonList = new ObservableCollection<HPTPerson>();
        }

        public HPTPersonReductionRule(int numberOfRaces, bool use)
            : base(numberOfRaces, use)
        {
            PersonList = new ObservableCollection<HPTPerson>();
        }

        private IEnumerable<HPTHorse> horseList;
        internal IEnumerable<HPTHorse> HorseList
        {
            get
            {
                if (horseList == null)
                {
                    horseList = PersonList.SelectMany(p => p.HorseList);
                }
                return horseList;
            }
        }

        [DataMember]
        public List<string> NameList { get; set; }

        //public void UpdateSelectable(IList<HPTHorse> horseList)
        public void UpdateSelectable(ICollection<HPTHorse> horseList)
        {
            NumberOfSelected = 0;
            var raceNumbers = new int[horseList.Count];
            for (var i = 0; i < horseList.Count; i++)
            {
                //raceNumbers[i] = horseList[i].ParentRace.LegNr;
                raceNumbers[i] = horseList.ElementAt(i).ParentRace.LegNr;
            }
            var antal = raceNumbers.Distinct().Count();
            for (var i = 0; i <= NumberOfRaces; i++)
            {
                var hptNow = NumberOfWinnersList.First(now => now.NumberOfWinners == i);
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

        public void SetShortDescriptionString()
        {
            var sb = new StringBuilder();
            sb.Append("-");
            sb.Append(": ");
            foreach (var person in PersonList)
            {
                sb.Append(person.ShortName);
                sb.Append(", ");
            }
            ShortDescription = sb.ToString();
        }

        [DataMember]
        public string ShortDescription
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public List<string> PersonShortNameList
        {
            get
            {
                if (field == null)
                {
                    field = new List<string>();
                }

                foreach (var person in PersonList)
                {
                    for (var i = 0; i < field.Count; i++)
                    {
                        if (field[i] == person.ShortName)
                        {
                            field.RemoveAt(i);
                            i--;
                        }
                    }

                    field.Add(person.ShortName);
                }

                return field;
            }
            set;
        }

        [DataMember]
        public List<string> PersonNameList
        {
            get
            {
                if (field == null)
                {
                    field = new List<string>();
                }

                foreach (var person in PersonList)
                {
                    for (var i = 0; i < field.Count; i++)
                    {
                        if (field[i] == person.Name)
                        {
                            field.RemoveAt(i);
                            i--;
                        }
                    }

                    field.Add(person.Name);
                }

                return field;
            }
            set;
        }

        [XmlIgnore]
        public ObservableCollection<HPTPerson> PersonList
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public override void Reset()
        {
            base.Reset();
            if (HorseList.Any())
            {
                LowestLegNumber = HorseList.Min(h => h.ParentRace.LegNr);
                HighestLegNumber = HorseList.Max(h => h.ParentRace.LegNr);
            }
            horseList = null;
        }

        [XmlIgnore]
        public int LowestLegNumber { get; set; }

        [XmlIgnore]
        public int HighestLegNumber { get; set; }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            if (!Use)
            {
                return true;
            }
            var numberOfHorses = HorseList.Intersect(singleRow.HorseList).Count();
            return NumberOfWinnersList.First(now => now.NumberOfWinners == numberOfHorses).Selected;
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            if (HighestLegNumber > numberOfRacesToTest || LowestLegNumber > numberOfRacesToTest || !Use)
            {
                return true;
            }

            var numberOfHorses = horseList.Take(numberOfRacesToTest).Intersect(horseList).Count();
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

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            // Skapa dictionary för att kontrollera hur många vinstrader villkoret skulle gett
            if (markBet.RaceDayInfo.ResultComplete)
            {
                var numberOfCorrectHorses = markBet.CouponCorrector.HorseList
                    .Intersect(HorseList)
                    .Count();

                RuleResultForCorrectRow = $"{numberOfCorrectHorses} Häst(ar)";
            }
            return true;
        }
    }
}
