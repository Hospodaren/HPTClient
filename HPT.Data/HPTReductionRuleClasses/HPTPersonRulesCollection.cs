using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTPersonRulesCollection : HPTRulesCollection
    {
        internal Func<HPTPersonReductionRule> ReductionRuleFactory;

        public HPTPersonRulesCollection() : base()
        {
        }

        public HPTPersonRulesCollection(int numberOfRaces, bool use, PersonReductionType reductionType) : base(numberOfRaces, use)
        {
            switch (reductionType)
            {
                case PersonReductionType.Driver:
                    ReductionRuleFactory = CreateNewDriverReductionRule;
                    Text = "Kuskar";
                    break;
                case PersonReductionType.Trainer:
                    ReductionRuleFactory = CreateNewTrainerReductionRule;
                    Text = "Tränare";
                    break;
                case PersonReductionType.Owner:
                    break;
                case PersonReductionType.Breeder:
                    break;
                default:
                    break;
            }
        }

        public override string ReductionTypeString
        {
            get
            {
                return Text + ": " + NumberOfWinnersString + " av " + ReductionRuleList.Count(r => r.Use).ToString();
            }
        }

        internal HPTPersonReductionRule CreateNewDriverReductionRule()
        {
            return new HPTDriverReductionRule(NumberOfRaces, true);
        }

        internal HPTPersonReductionRule CreateNewTrainerReductionRule()
        {
            return new HPTTrainerReductionRule(NumberOfRaces, true);
        }
        public ObservableCollection<HPTPerson> PersonList { get; set; }

        [DataMember]
        public string Text { get; set; }
    }
}
