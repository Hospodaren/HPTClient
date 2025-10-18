using System;
using System.Linq;
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
            this.NumberOfWinnersList
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
            groupIntervalReductionRule.HorseVariable = this.HorseVariable;
            return groupIntervalReductionRule;
        }

        [DataMember]
        public string PropertyName { get; set; }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            if (!this.Use || this.SkipRule)
            {
                return true;
            }
            if (singleRow.CurrentGroupIntervalValues == null)
            {
                singleRow.SetCurrentGroupIntervalValues(this.HorseVariable.HorseProperty);
            }

            if (!this.OnlyInSpecifiedLegs)
            {
                int horsesInInterval = singleRow.CurrentGroupIntervalValues.Count(d => d >= this.LowerBoundary && d <= this.UpperBoundary);
                return this.NumberOfWinnersList[horsesInInterval].Selected;
            }
            else
            {
                int numberOfX = 0;
                foreach (var legNumber in this.LegList)
                {
                    decimal horseValue = singleRow.CurrentGroupIntervalValues[legNumber - 1];
                    numberOfX += horseValue >= this.LowerBoundary && horseValue <= this.UpperBoundary ? 1 : 0;
                }
                return this.NumberOfWinnersList[numberOfX].Selected;
            }
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            if (!this.Use)
            {
                return true;
            }

            if (!this.OnlyInSpecifiedLegs)
            {
                int horsesInInterval = 0;
                foreach (HPTHorse horse in horseList)
                {
                    decimal horseValue = Convert.ToDecimal(this.HorseVariable.HorseProperty.GetValue(horse, null));
                    horsesInInterval += IsInInterval(horseValue) ? 1 : 0;
                }
                return this.MaxNumberOfX >= horsesInInterval;
            }
            else if (numberOfRacesToTest >= this.LegList.Max())
            {
                int numberOfX = 0;
                foreach (var legNumber in this.LegList.Where(l => l <= numberOfRacesToTest))
                {
                    var horse = horseList[legNumber - 1];
                    decimal horseValue = Convert.ToDecimal(this.HorseVariable.HorseProperty.GetValue(horse, null));
                    numberOfX += horseValue >= this.LowerBoundary && horseValue <= this.UpperBoundary ? 1 : 0;
                }
                return this.NumberOfWinnersList
                    .Where(now => now.Selected)
                    .Max(now => now.NumberOfWinners) >= numberOfX;
            }
            return true;
        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            var currentGroupIntervalValues = markBet.CouponCorrector.HorseList
            .Select(h => Convert.ToDecimal(this.HorseVariable.HorseProperty.GetValue(h, null)))
            .ToArray();

            if (!this.OnlyInSpecifiedLegs)
            {

                int horsesInInterval = currentGroupIntervalValues.Count(d => d >= this.LowerBoundary && d <= this.UpperBoundary);
                this.RuleResultForCorrectRow = horsesInInterval.ToString() + " Hästar";
                return true;
            }
            else
            {
                int numberOfX = 0;
                foreach (var legNumber in this.LegList)
                {
                    decimal horseValue = currentGroupIntervalValues[legNumber - 1];
                    numberOfX += horseValue >= this.LowerBoundary && horseValue <= this.UpperBoundary ? 1 : 0;
                }
                this.RuleResultForCorrectRow = numberOfX.ToString() + " Hästar";
                return true;
            }
        }

        private decimal lowerBoundary;
        [DataMember]
        public decimal LowerBoundary
        {
            get
            {
                return lowerBoundary;
            }
            set
            {
                this.lowerBoundary = value;
                OnPropertyChanged("LowerBoundary");
            }
        }

        private decimal upperBoundary;
        [DataMember]
        public decimal UpperBoundary
        {
            get
            {
                return upperBoundary;
            }
            set
            {
                this.upperBoundary = value;
                OnPropertyChanged("UpperBoundary");
            }
        }

        public bool IsInInterval(decimal Value)
        {
            return (Value >= this.LowerBoundary && Value <= this.UpperBoundary);
        }

        public override string ReductionTypeString
        {
            get
            {
                if (this.NumberOfWinnersList.Count(now => now.Selected) == 0)
                {
                    return string.Empty;
                }
                return "Gruppintervallvillkor";
            }
        }

        private HPTHorseVariable horseVariable;
        [XmlIgnore]
        public HPTHorseVariable HorseVariable
        {
            get
            {
                return this.horseVariable;
            }
            set
            {
                this.horseVariable = value;
                if (this.horseVariable != null)
                {
                    this.PropertyName = this.horseVariable.PropertyName;
                }
                OnPropertyChanged("HorseVariable");
            }
        }

        public override void SetReductionSpecificationString()
        {
            if (this.NumberOfWinnersList.Count(now => now.Selected) == 0)
            {
                this.ReductionSpecificationString = string.Empty;
                return;
            }

            var sb = new StringBuilder();
            sb.Append(this.NumberOfWinnersString);
            sb.Append(" vinnare");
            if (this.OnlyInSpecifiedLegs)
            {
                sb.Append(" (");
                sb.Append(this.SelectedRacesString);
                sb.Append(")");
            }
            this.ReductionSpecificationString = sb.ToString();
        }

        public override string ToString(HPTMarkBet markBet)
        {
            if (this.NumberOfWinnersList.Count(now => now.Selected) == 0)
            {
                return string.Empty;
            }

            // Create String representation
            var sb = new StringBuilder();
            sb.Append(this.HorseVariable.GroupReductionInfo.Name);
            sb.Append(" (");
            sb.Append(this.LowerBoundary);
            sb.Append(" - ");
            sb.Append(this.UpperBoundary);
            sb.AppendLine(")");
            sb.Append(this.NumberOfWinnersString);
            sb.AppendLine(" vinnare");

            this.ClipboardString = this.ReductionTypeString + "\r\n" + sb.ToString();
            return sb.ToString();
        }
    }
}
