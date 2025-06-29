using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTGroupIntervalRulesCollection : HPTRulesCollection
    {
        public HPTGroupIntervalRulesCollection() : base()
        {
        }

        public HPTGroupIntervalRulesCollection(int numberOfRaces, bool use) : base(numberOfRaces, use)
        {
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            if (ReductionRuleList.Count == 0 || !this.Use)
            {
                return true;
            }
            int numberOfRules = 0;
            IEnumerable<HPTGroupIntervalReductionRule> ruleList = this.ReductionRuleList.Cast<HPTGroupIntervalReductionRule>();
            foreach (var propertyInfo in this.DistinctProperties)
            {
                singleRow.SetCurrentGroupIntervalValues(propertyInfo);
                foreach (var rule in ruleList.Where(r => r.PropertyName == propertyInfo.Name))
                {
                    if (rule.IncludeRow(markBet, singleRow))
                    {
                        numberOfRules++;
                    }
                    else if (this.AllRulesMustApply)
                    {
                        return false;
                    }
                }
            }
            return this.NumberOfWinnersList.First(now => now.NumberOfWinners == numberOfRules).Selected;
        }

        [DataMember]
        public BetTypeCategory TypeCategory { get; set; }

        [XmlIgnore]
        public System.Reflection.PropertyInfo[] DistinctProperties { get; set; }

        public override void Reset()
        {
            base.Reset();

            this.DistinctProperties = this.ReductionRuleList
                .Cast<HPTGroupIntervalReductionRule>()
                .Where(r => r.HorseVariable != null)
                .Select(r => r.HorseVariable.HorseProperty)
                .Distinct()
                .ToArray();
        }

        public override string ReductionTypeString
        {
            get
            {
                //return "Gruppintervallregler: " + this.NumberOfWinnersString + " av " + this.ReductionRuleList.Count(r => r.Use).ToString();
                if (this.ReductionRuleList.Count(r => r.Use) > 1)
                {
                    return "GRUPPINTERVALLREGLER "; 
                }
                return string.Empty;
            }
        }
    }
}
