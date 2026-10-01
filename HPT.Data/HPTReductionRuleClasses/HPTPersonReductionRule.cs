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
            int[] raceNumbers = new int[horseList.Count];
            for (int i = 0; i < horseList.Count; i++)
            {
                //raceNumbers[i] = horseList[i].ParentRace.LegNr;
                raceNumbers[i] = horseList.ElementAt(i).ParentRace.LegNr;
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

        public void SetShortDescriptionString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-");
            sb.Append(": ");
            foreach (HPTPerson person in PersonList)
            {
                sb.Append(person.ShortName);
                sb.Append(", ");
            }
            ShortDescription = sb.ToString();
        }

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

        private List<string> personShortNameList;
        [DataMember]
        public List<string> PersonShortNameList
        {
            get
            {
                if (personShortNameList == null)
                {
                    personShortNameList = new List<string>();
                }
                foreach (HPTPerson person in PersonList)
                {
                    for (int i = 0; i < personShortNameList.Count; i++)
                    {
                        if (personShortNameList[i] == person.ShortName)
                        {
                            personShortNameList.RemoveAt(i);
                            i--;
                        }
                    }
                    personShortNameList.Add(person.ShortName);
                }

                return personShortNameList;
            }
            set
            {
                personShortNameList = value;
            }
        }

        private List<string> personNameList;
        [DataMember]
        public List<string> PersonNameList
        {
            get
            {
                if (personNameList == null)
                {
                    personNameList = new List<string>();
                }
                foreach (HPTPerson person in PersonList)
                {
                    for (int i = 0; i < personNameList.Count; i++)
                    {
                        if (personNameList[i] == person.Name)
                        {
                            personNameList.RemoveAt(i);
                            i--;
                        }
                    }
                    personNameList.Add(person.Name);
                }

                return personNameList;
            }
            set
            {
                personNameList = value;
            }
        }

        private ObservableCollection<HPTPerson> personList;
        [XmlIgnore]
        public ObservableCollection<HPTPerson> PersonList
        {
            get
            {
                return personList;
            }
            set
            {
                personList = value;
                OnPropertyChanged("PersonList");
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
            int numberOfHorses = HorseList.Intersect(singleRow.HorseList).Count();
            return NumberOfWinnersList.First(now => now.NumberOfWinners == numberOfHorses).Selected;
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            if (HighestLegNumber > numberOfRacesToTest || LowestLegNumber > numberOfRacesToTest || !Use)
            {
                return true;
            }

            int numberOfHorses = horseList.Take(numberOfRacesToTest).Intersect(horseList).Count();
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
                int numberOfCorrectHorses = markBet.CouponCorrector.HorseList
                    .Intersect(HorseList)
                    .Count();

                RuleResultForCorrectRow = numberOfCorrectHorses.ToString() + " Häst(ar)";
            }
            return true;
        }
    }
}
