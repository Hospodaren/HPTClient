using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;

namespace HPTClient
{
    [DataContract]
    public class HPTCategoryReductionRuleCollection : HPTReductionRule
    {
        public HPTCategoryReductionRuleCollection()
        {
        }

        public HPTCategoryReductionRuleCollection(HPTMarkBet markBet)
        {
            CCReductionRuleList = new ObservableCollection<HPTCategoryReductionRule>();
            var categoryCodes = Enum.GetValues(typeof(StartCategoryCode));
            foreach (StartCategoryCode categoryCode in categoryCodes)
            {
                if (categoryCode == StartCategoryCode.None)
                {
                    continue;
                }
                var categoryRule = new HPTCategoryReductionRule(categoryCode, markBet.NumberOfRaces, false);
                CCReductionRuleList.Add(categoryRule);
            }

        }

        [DataMember]
        public ObservableCollection<HPTCategoryReductionRule> CCReductionRuleList
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
            foreach (var hptCategoryReductionRule in RulesToUse)
            {
                if (!hptCategoryReductionRule.IncludeRow(markBet, singleRow))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            foreach (var hptCategoryReductionRule in RulesToUse)
            {
                if (!hptCategoryReductionRule.IncludeRow(markBet, horseList, numberOfRacesToTest))
                {
                    return false;
                }
            }
            return true;
        }

        private List<HPTCategoryReductionRule> rulesToUse;
        private List<HPTCategoryReductionRule> RulesToUse
        {
            get
            {
                if (rulesToUse == null)
                {
                    rulesToUse = CCReductionRuleList.Where(x => x.Use).ToList();
                }
                return rulesToUse;
            }
        }

        public override void Reset()
        {
            base.Reset();
            rulesToUse = null;
            //LowestMax = RulesToUse
            //    .OrderBy(rr => rr.MaxNumberOfX)
            //    .First().MaxNumberOfX;

            foreach (var reductionRule in CCReductionRuleList)
            {
                reductionRule.Reset();
            }
        }

        [DataMember]
        public int LowestMax
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
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
        } = true;

        public void Clear()
        {
            foreach (var hptxReductionRule in CCReductionRuleList)
            {
                foreach (var hptNumberOfWinners in hptxReductionRule.NumberOfWinnersList)
                {
                    hptNumberOfWinners.Selected = false;
                }
            }
        }

        public override string ReductionTypeString
        {
            get
            {
                return "Kategori-villkor";
            }
        }


        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            var sb = new StringBuilder();
            //sb.AppendLine("ABCD-Villkor");

            CCReductionRuleList
                .Where(r => r.NumberOfRacesWithX > 0)
                .OrderBy(r => r.CategoryCode)
                .ToList()
                .ForEach(r =>
                {
                    r.SetReductionSpecificationString();
                    sb.AppendLine(r.ReductionSpecificationString);
                });

            ClipboardString = sb.ToString();
            return sb.ToString();
        }
    }
}
