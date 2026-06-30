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
            NumberOfRaces = numberOfRaces;
            HorseRankVariable = horseRankVariable;
            PropertyName = horseRankVariable.PropertyName;
            IncrementLower = 1;
            IncrementUpper = 1;
            //this.MinSum = numberOfRaces;
            MinSum = 0;
            MaxSum = numberOfRaces * 15;
            ReductionRuleList = new ObservableCollection<HPTHorseRankReductionRule>();
        }

        // KOMMANDE
        public HPTHorseRankSumReductionRule Clone()
        {
            var clonedHorseRankSumReductionRule = (HPTHorseRankSumReductionRule)HPTSerializer.CreateDeepCopy(this);
            clonedHorseRankSumReductionRule.HorseRankVariable = HorseRankVariable;
            return clonedHorseRankSumReductionRule;
        }

        // KOMMANDE
        public void ApplyRule(HPTHorseRankSumReductionRule horseRankSumReductionRule)
        {
            MinSum = horseRankSumReductionRule.MinSum;
            MaxSum = horseRankSumReductionRule.MaxSum;
            ReductionRuleList.Clear();
            foreach (var templateReductionRule in horseRankSumReductionRule.ReductionRuleList)
            {
                var rankReductionRule = (HPTHorseRankReductionRule)HPTSerializer.CreateDeepCopy(templateReductionRule);
                ReductionRuleList.Add(rankReductionRule);
            }
        }

        [DataMember]
        public int NumberOfRaces { get; set; }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            if (!Use)
            {
                return true;
            }

            // Skapa lista med alla rankpoäng
            var rankList = singleRow.HorseList
                .Select(h => h.RankList.First(r => r.Name == PropertyName))
                .Select(r => r.Rank)
                .ToList();

            // Beräkna summan
            int rankSum = rankList.Sum();
            if (rankSum >= MinSum && rankSum <= MaxSum)
            {
                foreach (var rule in ReductionRuleList)
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
                return propertyName;
            }
            set
            {
                propertyName = value;
                OnPropertyChanged("PropertyName");
            }
        }

        private HPTHorseRankVariable horseRankVariable;
        [XmlIgnore]
        public HPTHorseRankVariable HorseRankVariable
        {
            get
            {
                return horseRankVariable;
            }
            set
            {
                horseRankVariable = value;
                OnPropertyChanged("HorseRankVariable");
            }
        }

        private ObservableCollection<HPTHorseRankReductionRule> reductionRuleList;
        [DataMember]
        public ObservableCollection<HPTHorseRankReductionRule> ReductionRuleList
        {
            get
            {
                return reductionRuleList;
            }
            set
            {
                reductionRuleList = value;
                OnPropertyChanged("ReductionRuleList");
            }
        }

        public override string ReductionTypeString
        {
            get
            {
                return "RANKPOÄNGSVILLKOR: " + HorseRankVariable.CategoryText + " - " + HorseRankVariable.Text;
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            var sb = new StringBuilder();

            sb.Append("Ranksumma ");
            sb.Append(MinSum);
            sb.Append(" - ");
            sb.Append(MaxSum);
            foreach (var rule in ReductionRuleList)
            {
                sb.AppendLine();
                sb.Append(rule.ToString(markBet));
            }
            sb.AppendLine();

            ClipboardString = ReductionTypeString + "\r\n" + sb.ToString();
            return sb.ToString();
        }
    }
}
