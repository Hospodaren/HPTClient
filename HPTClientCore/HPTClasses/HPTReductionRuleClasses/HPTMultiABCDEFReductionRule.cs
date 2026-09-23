using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;

namespace HPTClient
{
    [DataContract]
    public class HPTMultiABCDEFReductionRule : HPTReductionRule
    {
        [DataMember]
        public ObservableCollection<HPTABCDEFReductionRule> ABCDEFReductionRuleList
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            foreach (var abcdefReductionRule in ABCDEFReductionRuleList.Where(r => r.Use))
            {
                if (abcdefReductionRule.IncludeRow(markBet, singleRow))
                {
                    return true;
                }
            }
            return false;
        }

        [DataMember]
        public bool Use
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public override void Reset()
        {
            base.Reset();
            foreach (var rule in ABCDEFReductionRuleList)
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

            ABCDEFReductionRuleList
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
            foreach (var abcdefReductionRule in ABCDEFReductionRuleList.Where(r => r.Use))
            {
                sb.AppendLine(abcdefReductionRule.ToString(markBet));
            }
            return sb.ToString();
        }
    }
}
