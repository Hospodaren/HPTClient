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
            foreach (var now in NumberOfWinnersList)
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
                var numberOfX = singleRow.HorseList.Count(h => h.CategoryCode.HasFlag(CategoryCode));
                return NumberOfWinnersList[numberOfX].Selected;
            }
            else
            {
                var numberOfX = 0;
                foreach (var legNumber in LegList)
                {
                    numberOfX += singleRow.HorseList[legNumber - 1].CategoryCode.HasFlag(CategoryCode) ? 1 : 0;
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
                var numberOfX = horseList.Take(numberOfRacesToTest).Count(h => h.CategoryCode.HasFlag(CategoryCode));
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
                var numberOfX = 0;
                foreach (var legNumber in LegList.Where(ln => ln <= numberOfRacesToTest))
                {
                    numberOfX += horseList[legNumber - 1].CategoryCode.HasFlag(CategoryCode) ? 1 : 0;
                }
                return numberOfX <= MaxNumberOfX;
            }
        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            // Skapa dictionary för att kontrollera hur många vinstrader villkoret skulle gett
            if (markBet.RaceDayInfo.ResultComplete)
            {
                var numberOfXHorses = markBet.CouponCorrector.HorseList.Count(h => h.CategoryCode == CategoryCode);
                RuleResultForCorrectRow = $"{numberOfXHorses} {CategoryCode.GetString()}";
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

        [DataMember]
        public StartCategoryCode CategoryCode
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int NumberOfX
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int NumberOfRacesWithX
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfXInRace;

        [XmlIgnore]
        public List<System.Collections.BitArray> CombinationsToTest;


        public override string ReductionTypeString
        {
            get
            {
                return $"{CategoryCode.GetString()}-villkor";
            }
        }
                public override string ToString()
        {
            return CategoryCode.GetString();
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            var sb = new StringBuilder();
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
