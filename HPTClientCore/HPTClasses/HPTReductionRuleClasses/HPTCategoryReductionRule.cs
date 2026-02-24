using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTCategoryReductionRule : HPTNumberOfWinnersReductionRule
    {
        public HPTCategoryReductionRule()
        {
        }

        public HPTCategoryReductionRule(StartCategoryCode categoryCode, int numberOfRaces, bool use)
            : base(numberOfRaces, use)
        {
            CategoryCode = categoryCode;
            InitializeLegSelectionList(numberOfRaces);
        }

        public HPTCategoryReductionRule Clone()
        {
            var rule = new HPTCategoryReductionRule(CategoryCode, NumberOfWinnersList.Count - 1, Use);
            foreach (HPTNumberOfWinners now in NumberOfWinnersList)
            {
                var hptNow = rule.NumberOfWinnersList.FirstOrDefault(n => n.NumberOfWinners == now.NumberOfWinners);
                if (hptNow != null)
                {
                    hptNow.Selected = now.Selected;
                    hptNow.Selectable = now.Selectable;
                }
            }
            return rule;
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            if (NumberOfRacesWithX == 0 || SkipRule)
            {
                return true;
            }
            if (!OnlyInSpecifiedLegs)
            {
                int numberOfX = singleRow.HorseList.Count(h => h.CategoryCode == CategoryCode);
                return NumberOfWinnersList[numberOfX].Selected;
            }
            else
            {
                int numberOfX = 0;
                foreach (var legNumber in LegList)
                {
                    numberOfX += singleRow.HorseList[legNumber - 1].CategoryCode == CategoryCode ? 1 : 0;
                }
                return NumberOfWinnersList[numberOfX].Selected;
            }
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            if (NumberOfRacesWithX == 0 || numberOfRacesToTest <= MaxNumberOfX || SkipRule)
            {
                return true;
            }

            if (!OnlyInSpecifiedLegs)
            {
                int numberOfX = horseList.Take(numberOfRacesToTest).Count(h => h.CategoryCode == CategoryCode);
                if (numberOfX > MaxNumberOfX)  // Högsta antal har överskridits redan innan alla lopp kontrollerats
                {
                    return false;
                }
                if (numberOfX + markBet.BetType.NumberOfRaces - numberOfRacesToTest < MinNumberOfX)    // Det går inte att komma upp i minimiantal på resterande lopp
                {
                    return false;
                }
                return true;
            }
            else
            {
                int numberOfX = 0;
                foreach (var legNumber in LegList.Where(ln => ln <= numberOfRacesToTest))
                {
                    numberOfX += horseList[legNumber - 1].CategoryCode == CategoryCode ? 1 : 0;
                }
                return numberOfX <= MaxNumberOfX;
            }
        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            // Skapa dictionary för att kontrollera hur många vinstrader villkoret skulle gett
            if (markBet.RaceDayInfo.ResultComplete)
            {
                int numberOfXHorses = markBet.CouponCorrector.HorseList.Count(h => h.CategoryCode == CategoryCode);
                RuleResultForCorrectRow = numberOfXHorses.ToString() + " " + CategoryCode.GetString();
            }
            return true;
        }

        public override void SetReductionSpecificationString()
        {
            var sb = new StringBuilder();
            sb.Append(NumberOfWinnersString);
            //sb.Append(" ");
            //sb.Append(Prio);
            //sb.Append("-Hästar");
            //if (OnlyInSpecifiedLegs)
            //{
            //    sb.Append(" (");
            //    sb.Append(SelectedRacesString);
            //    sb.Append(")");
            //}
            ReductionSpecificationString = sb.ToString();
        }

        private StartCategoryCode _categoryCode;
        [DataMember]
        public StartCategoryCode CategoryCode
        {
            get
            {
                return _categoryCode;
            }
            set
            {
                _categoryCode = value;
                OnPropertyChanged("CategoryCode");
            }
        }

        private int numberOfX;
        [DataMember]
        public int NumberOfX
        {
            get
            {
                return numberOfX;
            }
            set
            {
                numberOfX = value;
                OnPropertyChanged("NumberOfX");
            }
        }

        private int numberOfRacesWithX;
        [DataMember]
        public int NumberOfRacesWithX
        {
            get
            {
                return numberOfRacesWithX;
            }
            set
            {
                numberOfRacesWithX = value;
                OnPropertyChanged("NumberOfRacesWithX");
            }
        }

        public int NumberOfXInRace;

        [XmlIgnore]
        public List<System.Collections.BitArray> CombinationsToTest;


        public override string ReductionTypeString
        {
            get
            {
                return CategoryCode.GetString() + "-villkor";
            }
        }
                public override string ToString()
        {
            return CategoryCode.GetString();
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            StringBuilder sb = new StringBuilder();
            sb.Append(NumberOfWinnersString);
            //for (int i = this.MinNumberOfX; i <= this.MaxNumberOfX; i++)
            //{
            //    var hptNow = this.NumberOfWinnersList.FirstOrDefault(now => now.NumberOfWinners == i);
            //    if (hptNow != null && hptNow.Selected)
            //    {
            //        sb.Append(i);
            //        sb.Append(",");
            //    }
            //}
            if (sb.Length > 1)
            {
                //sb.Remove(sb.Length - 1, 1);
                //sb.Append(" ");
                //sb.Append(Prio);
                //sb.Append("-Hästar");
            }

            ClipboardString = sb.ToString();
            return sb.ToString();
        }
    }
}
