using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace HPTClient
{
    // KOMMANDE
    [DataContract]
    public class HPTHorseRankSumReductionRuleCollection : Notifier
    {
        //[DataMember]
        //public BetTypeCategory TypeCategory { get; set; }

        private BetTypeCategory typeCategory;
        [DataMember]
        public BetTypeCategory TypeCategory
        {
            get
            {
                return typeCategory;
            }
            set
            {
                typeCategory = value;
                OnPropertyChanged("TypeCategory");
            }
        }

        private string name;
        [DataMember]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        [DataMember]
        public ObservableCollection<HPTHorseRankSumReductionRule> RankSumReductionRuleList { get; set; }
        //public List<HPTHorseRankSumReductionRule> RankSumReductionRuleList { get; set; }
    }
}
