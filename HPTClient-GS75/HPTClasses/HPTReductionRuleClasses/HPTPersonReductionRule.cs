using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

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
            this.PersonList = new ObservableCollection<HPTPerson>();
        }

        public HPTPersonReductionRule(int numberOfRaces, bool use)
            : base(numberOfRaces, use)
        {
            this.PersonList = new ObservableCollection<HPTPerson>();
        }

        private IEnumerable<HPTHorse> horseList;
        internal IEnumerable<HPTHorse> HorseList
        {
            get
            {
                if (this.horseList == null)
                {
                    this.horseList = this.PersonList.SelectMany(p => p.HorseList);
                }
                return this.horseList;
            }
        }

        [DataMember]
        public List<string> NameList { get; set; }

        //public void UpdateSelectable(IList<HPTHorse> horseList)
        public void UpdateSelectable(ICollection<HPTHorse> horseList)
        {
            this.NumberOfSelected = 0;
            int[] raceNumbers = new int[horseList.Count];
            for (int i = 0; i < horseList.Count; i++)
            {
                //raceNumbers[i] = horseList[i].ParentRace.LegNr;
                raceNumbers[i] = horseList.ElementAt(i).ParentRace.LegNr;
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

        public void SetShortDescriptionString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-");
            sb.Append(": ");
            foreach (HPTPerson person in this.PersonList)
            {
                sb.Append(person.ShortName);
                sb.Append(", ");
            }
            this.ShortDescription = sb.ToString();
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
                if (this.personShortNameList == null)
                {
                    this.personShortNameList = new List<string>();
                }
                foreach (HPTPerson person in this.PersonList)
                {
                    for (int i = 0; i < this.personShortNameList.Count; i++)
                    {
                        if (this.personShortNameList[i] == person.ShortName)
                        {
                            this.personShortNameList.RemoveAt(i);
                            i--;
                        }
                    }
                    this.personShortNameList.Add(person.ShortName);
                }

                return this.personShortNameList;
            }
            set
            {
                this.personShortNameList = value;
            }
        }

        private List<string> personNameList;
        [DataMember]
        public List<string> PersonNameList
        {
            get
            {
                if (this.personNameList == null)
                {
                    this.personNameList = new List<string>();
                }
                foreach (HPTPerson person in this.PersonList)
                {
                    for (int i = 0; i < this.personNameList.Count; i++)
                    {
                        if (this.personNameList[i] == person.Name)
                        {
                            this.personNameList.RemoveAt(i);
                            i--;
                        }
                    }
                    this.personNameList.Add(person.Name);
                }

                return this.personNameList;
            }
            set
            {
                this.personNameList = value;
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
            if (this.HorseList.Any())
            {
                this.LowestLegNumber = this.HorseList.Min(h => h.ParentRace.LegNr);
                this.HighestLegNumber = this.HorseList.Max(h => h.ParentRace.LegNr);
            }
            this.horseList = null;
        }

        [XmlIgnore]
        public int LowestLegNumber { get; set; }

        [XmlIgnore]
        public int HighestLegNumber { get; set; }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            if (!this.Use)
            {
                return true;
            }
            int numberOfHorses = this.HorseList.Intersect(singleRow.HorseList).Count();
            return this.NumberOfWinnersList.First(now => now.NumberOfWinners == numberOfHorses).Selected;
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            if (this.HighestLegNumber > numberOfRacesToTest || this.LowestLegNumber > numberOfRacesToTest || !this.Use)
            {
                return true;
            }

            int numberOfHorses = horseList.Take(numberOfRacesToTest).Intersect(horseList).Count();
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

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            // Skapa dictionary för att kontrollera hur många vinstrader villkoret skulle gett
            if (markBet.RaceDayInfo.ResultComplete)
            {
                int numberOfCorrectHorses = markBet.CouponCorrector.HorseList
                    .Intersect(this.HorseList)
                    .Count();

                this.RuleResultForCorrectRow = numberOfCorrectHorses.ToString() + " Häst(ar)";
            }
            return true;
        }
    }
}
