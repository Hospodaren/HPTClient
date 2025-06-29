using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace HPTClient
{
    public class HPTMarkBetTemplateResult : Notifier
    {
        private bool useABCD;
        private bool useRankSum;

        public HPTMarkBetTemplateResult(HPTMarkBet markBet)
        {
            this.XReductionRuleList = new ObservableCollection<HPTXReductionRule>();
            if (markBet.ABCDEFReductionRule.Use)
            {
                foreach (HPTXReductionRule rule in markBet.ABCDEFReductionRule.XReductionRuleList)
                {
                    HPTXReductionRule clonedRule = rule.Clone();
                    clonedRule.Reset();
                    this.XReductionRuleList.Add(clonedRule);
                }
            }

            this.MinRankSum = markBet.MinRankSum;
            this.MaxRankSum = markBet.MaxRankSum;
            this.MinRowValue = markBet.RowValueReductionRule.LowestIncludedSum;
            this.MaxRowValue = markBet.RowValueReductionRule.HighestIncludedSum;
            this.ReducedSize = markBet.ReducedSize;
            this.useABCD = markBet.ABCDEFReductionRule.Use;
            this.useRankSum = markBet.ReductionRank;
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
            sb.Append(this.ReducedSize);
            sb.Append(" rader (");
            if (this.useABCD)
            {
                foreach (HPTXReductionRule rule in this.XReductionRuleList)
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
            if (this.useRankSum)
            {
                sb.Append(this.MinRankSum.ToString("F1"));
                sb.Append(" - ");
                sb.Append(this.MaxRankSum.ToString("F1"));
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
