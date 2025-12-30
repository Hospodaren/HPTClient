using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTComplementaryRulesCollection : HPTRulesCollection
    {
        public HPTComplementaryRulesCollection() : base()
        {
        }

        public HPTComplementaryRulesCollection(int numberOfRaces, bool use)
            : base(numberOfRaces, use)
        {
        }

        public override string ReductionTypeString
        {
            get
            {
                if (this.ReductionRuleList.Count(r => r.Use) > 1)
                {
                    return "UTGÅNGAR: " + this.NumberOfWinnersString + " av " + this.ReductionRuleList.Count(r => r.Use).ToString();
                }
                return string.Empty;
            }
        }
    }
}
