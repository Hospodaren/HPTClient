namespace HPTClient
{
    public class HPTMarkBetTemplateRank : HPTMarkBetTemplate
    {
        public new HPTMarkBetTemplateRank Clone()
        {
            var template = new HPTMarkBetTemplateRank()
            {
                DesiredSystemSize = DesiredSystemSize,
                Name = Name,
                NumberOfSpikes = NumberOfSpikes,
                RankTemplate = RankTemplate,
                RankTemplateName = RankTemplateName,
                TypeCategory = TypeCategory,
                LowerPercentageLimit = LowerPercentageLimit,
                UpperPercentageLimit = UpperPercentageLimit,
                ReductionPercentage = ReductionPercentage
            };

            return template;
        }

        public int LowerPercentageLimit { get; set; }

        public int UpperPercentageLimit { get; set; }

        public decimal MinRankValue { get; set; }

        public decimal MaxRankValue { get; set; }
    }
}
