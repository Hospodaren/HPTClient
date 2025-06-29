using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
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
                return this.typeCategory;
            }
            set
            {
                this.typeCategory = value;
                OnPropertyChanged("TypeCategory");
            }
        }

        private string name;
        [DataMember]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                OnPropertyChanged("Name");
            }
        }

        [DataMember]
        public ObservableCollection<HPTHorseRankSumReductionRule> RankSumReductionRuleList { get; set; }
        //public List<HPTHorseRankSumReductionRule> RankSumReductionRuleList { get; set; }
    }
}
