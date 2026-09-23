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

        [DataMember]
        public BetTypeCategory TypeCategory
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string Name
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public ObservableCollection<HPTHorseRankSumReductionRule> RankSumReductionRuleList { get; set; }
        //public List<HPTHorseRankSumReductionRule> RankSumReductionRuleList { get; set; }
    }
}
