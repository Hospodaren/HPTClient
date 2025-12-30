using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseRankSumReductionRule : HPTIntervalReductionRule
    {
        public HPTHorseRankSumReductionRule()
        {
        }

        public HPTHorseRankSumReductionRule(HPTHorseRankVariable horseRankVariable, int numberOfRaces)
        {
            this.NumberOfRaces = numberOfRaces;
            this.HorseRankVariable = horseRankVariable;
            this.PropertyName = horseRankVariable.PropertyName;
            this.IncrementLower = 1;
            this.IncrementUpper = 1;
            //this.MinSum = numberOfRaces;
            this.MinSum = 0;
            this.MaxSum = numberOfRaces * 15;
            this.ReductionRuleList = new ObservableCollection<HPTHorseRankReductionRule>();
        }

        // KOMMANDE
        public HPTHorseRankSumReductionRule Clone()
        {
            var clonedHorseRankSumReductionRule = (HPTHorseRankSumReductionRule)HPTSerializer.CreateDeepCopy(this);
            clonedHorseRankSumReductionRule.HorseRankVariable = this.HorseRankVariable;
            return clonedHorseRankSumReductionRule;
        }

        // KOMMANDE
        public void ApplyRule(HPTHorseRankSumReductionRule horseRankSumReductionRule)
        {
            this.MinSum = horseRankSumReductionRule.MinSum;
            this.MaxSum = horseRankSumReductionRule.MaxSum;
            this.ReductionRuleList.Clear();
            foreach (var templateReductionRule in horseRankSumReductionRule.ReductionRuleList)
            {
                var rankReductionRule = (HPTHorseRankReductionRule)HPTSerializer.CreateDeepCopy(templateReductionRule);
                this.ReductionRuleList.Add(rankReductionRule);
            }
        }

        [DataMember]
        public int NumberOfRaces { get; set; }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            if (!this.Use)
            {
                return true;
            }

            // Skapa lista med alla rankpoäng
            var rankList = singleRow.HorseList
                .Select(h => h.RankList.First(r => r.Name == this.PropertyName))
                .Select(r => r.Rank)
                .ToList();

            // Beräkna summan
            int rankSum = rankList.Sum();
            if (rankSum >= this.MinSum && rankSum <= this.MaxSum)
            {
                foreach (var rule in this.ReductionRuleList)
                {
                    if (!rule.OnlyInSpecifiedLegs)
                    {
                        int numberInInterval = rankList.Count(r => r >= rule.LowerBoundary && r <= rule.UpperBoundary);
                        if (!rule.NumberOfWinnersList.First(now => now.NumberOfWinners == numberInInterval).Selected)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        int numberOfX = 0;
                        foreach (var legNumber in rule.LegList)
                        {
                            int rankValue = rankList[legNumber - 1];
                            numberOfX += rankValue >= rule.LowerBoundary && rankValue <= rule.UpperBoundary ? 1 : 0;
                        }
                        if (!rule.NumberOfWinnersList[numberOfX].Selected)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        //public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        //{
        //    this.RuleResultForCorrectRow = "Summa " + markBet.CouponCorrector.HorseList.Sum(h => h.RankAlternate).ToString();
        //    return true;
        //}

        //public override void Reset()
        //{
        //    foreach (var rule in this.ReductionRuleList)
        //    {
        //        rule.Use = false;
        //    }
        //    this.Use = false;
        //}

        private string propertyName;
        [DataMember]
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
            set
            {
                this.propertyName = value;
                OnPropertyChanged("PropertyName");
            }
        }

        private HPTHorseRankVariable horseRankVariable;
        [XmlIgnore]
        public HPTHorseRankVariable HorseRankVariable
        {
            get
            {
                return this.horseRankVariable;
            }
            set
            {
                this.horseRankVariable = value;
                OnPropertyChanged("HorseRankVariable");
            }
        }

        private ObservableCollection<HPTHorseRankReductionRule> reductionRuleList;
        [DataMember]
        public ObservableCollection<HPTHorseRankReductionRule> ReductionRuleList
        {
            get
            {
                return this.reductionRuleList;
            }
            set
            {
                this.reductionRuleList = value;
                OnPropertyChanged("ReductionRuleList");
            }
        }

        public override string ReductionTypeString
        {
            get
            {
                return "RANKPOÄNGSVILLKOR: " + this.HorseRankVariable.CategoryText + " - " + this.HorseRankVariable.Text;
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            var sb = new StringBuilder();

            sb.Append("Ranksumma ");
            sb.Append(this.MinSum);
            sb.Append(" - ");
            sb.Append(this.MaxSum);
            foreach (var rule in this.ReductionRuleList)
            {
                sb.AppendLine();
                sb.Append(rule.ToString(markBet));
            }
            sb.AppendLine();

            this.ClipboardString = this.ReductionTypeString + "\r\n" + sb.ToString();
            return sb.ToString();
        }
    }
}
