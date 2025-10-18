using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HPTClient
{
    [DataContract]
    public class HPTMultiABCDEFReductionRule : HPTReductionRule
    {
        private ObservableCollection<HPTABCDEFReductionRule> abcdefReductionRuleList;
        [DataMember]
        public ObservableCollection<HPTABCDEFReductionRule> ABCDEFReductionRuleList
        {
            get
            {
                return this.abcdefReductionRuleList;
            }
            set
            {
                this.abcdefReductionRuleList = value;
                OnPropertyChanged("ABCDEFReductionRuleList");
            }
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            foreach (var abcdefReductionRule in this.ABCDEFReductionRuleList.Where(r => r.Use))
            {
                if (abcdefReductionRule.IncludeRow(markBet, singleRow))
                {
                    return true;
                }
            }
            return false;
        }

        private bool use;
        [DataMember]
        public bool Use
        {
            get
            {
                return this.use;
            }
            set
            {
                this.use = value;
                OnPropertyChanged("Use");
            }
        }

        public override void Reset()
        {
            base.Reset();
            foreach (var rule in this.ABCDEFReductionRuleList)
            {
                rule.Reset();
            }
        }

        public override IEnumerable<ReductionRuleInfo> GetReductionRuleInfoList(HPTMarkBet markBet)
        {
            var ruleInfoList = new List<ReductionRuleInfo>()
                {
                    new ReductionRuleInfo()
                    {
                        HeadlineString = "Multi-ABCD"
                    }
                };

            this.ABCDEFReductionRuleList
                .Where(r => r.Use)
                .ToList()
                .ForEach(r => ruleInfoList.Add(r.GetReductionRuleInfo(markBet)));

            return ruleInfoList;


            //List<ReductionRuleInfo> ruleInfoList = new List<ReductionRuleInfo>();

            //var ruleInfo = new ReductionRuleInfo()
            //{
            //    HeadlineString = "Multi-ABCD"
            //};
            //ruleInfoList.Add(ruleInfo);

            //foreach (var rule in this.ABCDEFReductionRuleList.Where(r => r.Use))
            //{
            //    ruleInfoList.Add(rule.GetReductionRuleInfo(markBet));
            //}
        }

        public override string ToString(HPTMarkBet markBet)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Multi-ABCD");
            sb.AppendLine();
            foreach (var abcdefReductionRule in this.ABCDEFReductionRuleList.Where(r => r.Use))
            {
                sb.AppendLine(abcdefReductionRule.ToString(markBet));
            }
            return sb.ToString();
        }
    }
}
