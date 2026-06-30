using System.Collections.ObjectModel;
using System.Text;

namespace HPTClient
{
    public class HPTMarkBetTemplateResult : Notifier
    {
        private bool useABCD;
        private bool useRankSum;

        public HPTMarkBetTemplateResult(HPTMarkBet markBet)
        {
            XReductionRuleList = new ObservableCollection<HPTXReductionRule>();
            if (markBet.ABCDEFReductionRule.Use)
            {
                foreach (HPTXReductionRule rule in markBet.ABCDEFReductionRule.XReductionRuleList)
                {
                    HPTXReductionRule clonedRule = rule.Clone();
                    clonedRule.Reset();
                    XReductionRuleList.Add(clonedRule);
                }
            }

            MinRankSum = markBet.MinRankSum;
            MaxRankSum = markBet.MaxRankSum;
            MinRowValue = markBet.RowValueReductionRule.LowestIncludedSum;
            MaxRowValue = markBet.RowValueReductionRule.HighestIncludedSum;
            ReducedSize = markBet.ReducedSize;
            useABCD = markBet.ABCDEFReductionRule.Use;
            useRankSum = markBet.ReductionRank;
        }

        public bool Use { get; set; }

        public ObservableCollection<HPTXReductionRule> XReductionRuleList { get; set; }

        public decimal MinRankSum { get; set; }

        public decimal MaxRankSum { get; set; }

        public int MinRowValue { get; set; }

        public int MaxRowValue { get; set; }

        public int ReducedSize { get; set; }

        public decimal Quota { get; set; }

        public int AbsDiff { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ReducedSize);
            sb.Append(" rader (");
            if (useABCD)
            {
                foreach (HPTXReductionRule rule in XReductionRuleList)
                {
                    if (rule.Use && rule.MaxNumberOfX > 0)
                    {
                        sb.Append(rule.MinNumberOfX);
                        sb.Append("-");
                        sb.Append(rule.MaxNumberOfX);
                        sb.Append(" ");
                        sb.Append(rule.Prio);
                        sb.Append(",");
                    }

                }
                sb.Remove(sb.Length - 1, 1);
            }
            if (useRankSum)
            {
                sb.Append(MinRankSum.ToString("F1"));
                sb.Append(" - ");
                sb.Append(MaxRankSum.ToString("F1"));
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
