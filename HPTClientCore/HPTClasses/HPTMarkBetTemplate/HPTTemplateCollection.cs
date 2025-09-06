using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTTemplateCollection
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public bool IsPrivate { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTRankTemplate> RankTemplateList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTMarkBetTemplateABCD> MarkBetTemplateABCDList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTMarkBetTemplateRank> MarkBetTemplateRankList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTGroupIntervalRulesCollection> GroupIntervalTemplateList { get; set; }

        // KOMMANDE
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTHorseRankSumReductionRuleCollection> RankSumReductionRuleCollection { get; set; }
    }
}
