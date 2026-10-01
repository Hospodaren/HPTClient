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
                if (ReductionRuleList.Count(r => r.Use) > 1)
                {
                    return "UTGÅNGAR: " + NumberOfWinnersString + " av " + ReductionRuleList.Count(r => r.Use).ToString();
                }
                return string.Empty;
            }
        }
    }
}
