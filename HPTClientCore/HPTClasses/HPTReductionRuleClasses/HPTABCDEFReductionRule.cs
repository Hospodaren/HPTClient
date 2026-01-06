using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;

namespace HPTClient
{
    [DataContract]
    public class HPTABCDEFReductionRule : HPTReductionRule
    {
        public HPTABCDEFReductionRule()
        {
        }

        public HPTABCDEFReductionRule(HPTMarkBet markBet)
        {
            XReductionRuleList = new ObservableCollection<HPTXReductionRule>()
            {
                new HPTXReductionRule(HPTPrio.A, markBet.RaceDayInfo.RaceList.Count, HPTConfig.Config.UseA),
                new HPTXReductionRule(HPTPrio.B, markBet.RaceDayInfo.RaceList.Count, HPTConfig.Config.UseB),
                new HPTXReductionRule(HPTPrio.C, markBet.RaceDayInfo.RaceList.Count, HPTConfig.Config.UseC),
                new HPTXReductionRule(HPTPrio.D, markBet.RaceDayInfo.RaceList.Count, HPTConfig.Config.UseD),
                new HPTXReductionRule(HPTPrio.E, markBet.RaceDayInfo.RaceList.Count, HPTConfig.Config.UseE),
                new HPTXReductionRule(HPTPrio.F, markBet.RaceDayInfo.RaceList.Count, HPTConfig.Config.UseF)
            };

        }

        private ObservableCollection<HPTXReductionRule> xReductionRuleList;
        [DataMember]
        public ObservableCollection<HPTXReductionRule> XReductionRuleList
        {
            get
            {
                return xReductionRuleList;
            }
            set
            {
                xReductionRuleList = value;
                OnPropertyChanged("XReductionRuleList");
            }
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            foreach (var hptXReductionRule in RulesToUse)
            {
                if (!hptXReductionRule.IncludeRow(markBet, singleRow))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            foreach (var hptXReductionRule in RulesToUse)
            {
                if (!hptXReductionRule.IncludeRow(markBet, horseList, numberOfRacesToTest))
                {
                    return false;
                }
            }
            return true;
        }

        private List<HPTXReductionRule> rulesToUse;
        private List<HPTXReductionRule> RulesToUse
        {
            get
            {
                if (rulesToUse == null)
                {
                    rulesToUse = XReductionRuleList.Where(x => x.Use).ToList();
                }
                return rulesToUse;
            }
        }

        public override void Reset()
        {
            base.Reset();
            rulesToUse = null;
            LowestMax = RulesToUse
                .OrderBy(rr => rr.MaxNumberOfX)
                .First().MaxNumberOfX;

            foreach (var reductionRule in XReductionRuleList)
            {
                reductionRule.Reset();
            }
        }

        private int lowestMax;
        [DataMember]
        public int LowestMax
        {
            get
            {
                return lowestMax;
            }
            set
            {
                lowestMax = value;
                OnPropertyChanged("LowestMax");
            }
        }

        private bool use;
        [DataMember]
        public bool Use
        {
            get
            {
                return use;
            }
            set
            {
                use = value;
                OnPropertyChanged("Use");
            }
        }

        public void Clear()
        {
            foreach (var hptxReductionRule in XReductionRuleList)
            {
                foreach (var hptNumberOfWinners in hptxReductionRule.NumberOfWinnersList)
                {
                    hptNumberOfWinners.Selected = false;
                }
            }
        }

        //public override void SetReductionSpecificationString()
        //{
        //    if (this.OnlyInSpecifiedLegs)
        //    {

        //    }
        //    //this.ReductionSpecificationString = "Radvärde " + this.MinSum.ToString() + " kr - " + this.MaxSum.ToString() + " kr";
        //}

        public override string ReductionTypeString
        {
            get
            {
                return "ABCD-villkor";
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("ABCD-Villkor");

            XReductionRuleList
                .Where(r => r.NumberOfRacesWithX > 0)
                .OrderBy(r => r.Prio)
                .ToList()
                .ForEach(r =>
                {
                    r.SetReductionSpecificationString();
                    sb.AppendLine(r.ReductionSpecificationString);
                });

            ClipboardString = sb.ToString();
            return sb.ToString();

            //foreach (var xReductionRule in this.XReductionRuleList.Where(r => r.NumberOfRacesWithX > 0))
            //{
            //    var sbRule = new StringBuilder();
            //    for (int i = xReductionRule.MinNumberOfX; i <= xReductionRule.MaxNumberOfX; i++)
            //    {
            //        HPTNumberOfWinners hptNow = xReductionRule.NumberOfWinnersList.FirstOrDefault(now => now.NumberOfWinners == i);
            //        if (hptNow != null && hptNow.Selected)
            //        {
            //            sbRule.Append(i);
            //            sbRule.Append(",");
            //        }
            //    }
            //    if (sbRule.Length > 1)
            //    {
            //        sbRule.Remove(sbRule.Length - 1, 1);
            //        sbRule.Append(" ");
            //        sbRule.Append(xReductionRule.Prio);
            //        sbRule.Append("-Hästar");
            //    }
            //    sb.AppendLine(sbRule.ToString());
            //}
        }
    }
}
