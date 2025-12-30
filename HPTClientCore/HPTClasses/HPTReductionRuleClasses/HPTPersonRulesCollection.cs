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
                    this.ReductionRuleFactory = CreateNewDriverReductionRule;
                    this.Text = "Kuskar";
                    break;
                case PersonReductionType.Trainer:
                    this.ReductionRuleFactory = CreateNewTrainerReductionRule;
                    this.Text = "Tränare";
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
                return this.Text + ": " + this.NumberOfWinnersString + " av " + this.ReductionRuleList.Count(r => r.Use).ToString();
            }
        }

        internal HPTPersonReductionRule CreateNewDriverReductionRule()
        {
            return new HPTDriverReductionRule(this.NumberOfRaces, true);
        }

        internal HPTPersonReductionRule CreateNewTrainerReductionRule()
        {
            return new HPTTrainerReductionRule(this.NumberOfRaces, true);
        }
        public ObservableCollection<HPTPerson> PersonList { get; set; }

        [DataMember]
        public string Text { get; set; }
    }
}
