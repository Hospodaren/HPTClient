using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTGroupIntervalReductionRule : HPTNumberOfWinnersReductionRule
    {
        public HPTGroupIntervalReductionRule()
            : base()
        {
        }

        public HPTGroupIntervalReductionRule(int numberOfRaces, bool use)
            : base(numberOfRaces, use)
        {
            NumberOfWinnersList
                .ToList()
                .ForEach(now => now.Selectable = true);
            //foreach (HPTNumberOfWinners now in this.NumberOfWinnersList)
            //{
            //    now.Selectable = true;
            //}

            InitializeLegSelectionList(numberOfRaces);
        }

        public HPTGroupIntervalReductionRule Clone()
        {
            var groupIntervalReductionRule = (HPTGroupIntervalReductionRule)HPTSerializer.CreateDeepCopy(this);
            groupIntervalReductionRule.HorseVariable = HorseVariable;
            return groupIntervalReductionRule;
        }

        [DataMember]
        public string PropertyName { get; set; }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            if (!Use || SkipRule)
            {
                return true;
            }
            if (singleRow.CurrentGroupIntervalValues == null)
            {
                singleRow.SetCurrentGroupIntervalValues(HorseVariable.HorseProperty);
            }

            if (!OnlyInSpecifiedLegs)
            {
                var horsesInInterval = singleRow.CurrentGroupIntervalValues.Count(d => d >= LowerBoundary && d <= UpperBoundary);
                return NumberOfWinnersList[horsesInInterval].Selected;
            }
            else
            {
                var numberOfX = 0;
                foreach (var legNumber in LegList)
                {
                    var horseValue = singleRow.CurrentGroupIntervalValues[legNumber - 1];
                    numberOfX += horseValue >= LowerBoundary && horseValue <= UpperBoundary ? 1 : 0;
                }
                return NumberOfWinnersList[numberOfX].Selected;
            }
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            if (!Use)
            {
                return true;
            }

            if (!OnlyInSpecifiedLegs)
            {
                var horsesInInterval = 0;
                foreach (var horse in horseList)
                {
                    var horseValue = Convert.ToDecimal(HorseVariable.HorseProperty.GetValue(horse, null));
                    horsesInInterval += IsInInterval(horseValue) ? 1 : 0;
                }
                return MaxNumberOfX >= horsesInInterval;
            }
            else if (numberOfRacesToTest >= LegList.Max())
            {
                var numberOfX = 0;
                foreach (var legNumber in LegList.Where(l => l <= numberOfRacesToTest))
                {
                    var horse = horseList[legNumber - 1];
                    var horseValue = Convert.ToDecimal(HorseVariable.HorseProperty.GetValue(horse, null));
                    numberOfX += horseValue >= LowerBoundary && horseValue <= UpperBoundary ? 1 : 0;
                }
                return NumberOfWinnersList
                    .Where(now => now.Selected)
                    .Max(now => now.NumberOfWinners) >= numberOfX;
            }
            return true;
        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            var currentGroupIntervalValues = markBet.CouponCorrector.HorseList
            .Select(h => Convert.ToDecimal(HorseVariable.HorseProperty.GetValue(h, null)))
            .ToArray();

            if (!OnlyInSpecifiedLegs)
            {

                var horsesInInterval = currentGroupIntervalValues.Count(d => d >= LowerBoundary && d <= UpperBoundary);
                RuleResultForCorrectRow = $"{horsesInInterval} Hästar";
                return true;
            }
            else
            {
                var numberOfX = 0;
                foreach (var legNumber in LegList)
                {
                    var horseValue = currentGroupIntervalValues[legNumber - 1];
                    numberOfX += horseValue >= LowerBoundary && horseValue <= UpperBoundary ? 1 : 0;
                }
                RuleResultForCorrectRow = $"{numberOfX} Hästar";
                return true;
            }
        }

        [DataMember]
        public decimal LowerBoundary
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public decimal UpperBoundary
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public bool IsInInterval(decimal Value)
        {
            return (Value >= LowerBoundary && Value <= UpperBoundary);
        }

        public override string ReductionTypeString
        {
            get
            {
                if (NumberOfWinnersList.Count(now => now.Selected) == 0)
                {
                    return string.Empty;
                }
                return "Gruppintervallvillkor";
            }
        }

        [XmlIgnore]
        public HPTHorseVariable HorseVariable
        {
            get;
            set
            {
                field = value;
                if (field != null)
                {
                    PropertyName = field.PropertyName;
                }
                OnPropertyChanged();
            }
        }

        public override void SetReductionSpecificationString()
        {
            if (NumberOfWinnersList.Count(now => now.Selected) == 0)
            {
                ReductionSpecificationString = string.Empty;
                return;
            }

            var sb = new StringBuilder();
            sb.Append(NumberOfWinnersString);
            sb.Append(" vinnare");
            if (OnlyInSpecifiedLegs)
            {
                sb.Append(" (");
                sb.Append(SelectedRacesString);
                sb.Append(")");
            }
            ReductionSpecificationString = sb.ToString();
        }

        public override string ToString(HPTMarkBet markBet)
        {
            if (NumberOfWinnersList.Count(now => now.Selected) == 0)
            {
                return string.Empty;
            }

            // Create String representation
            var sb = new StringBuilder();
            sb.Append(HorseVariable.GroupReductionInfo.Name);
            sb.Append(" (");
            sb.Append(LowerBoundary);
            sb.Append(" - ");
            sb.Append(UpperBoundary);
            sb.AppendLine(")");
            sb.Append(NumberOfWinnersString);
            sb.AppendLine(" vinnare");

            ClipboardString = $"{ReductionTypeString}\r\n{sb}";
            return sb.ToString();
        }
    }
}
