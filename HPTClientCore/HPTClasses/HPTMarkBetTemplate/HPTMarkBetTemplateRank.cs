namespace HPTClient
{
    public class HPTMarkBetTemplateRank : HPTMarkBetTemplate
    {
        public new HPTMarkBetTemplateRank Clone()
        {
            var template = new HPTMarkBetTemplateRank()
            {
                DesiredSystemSize = this.DesiredSystemSize,
                Name = this.Name,
                NumberOfSpikes = this.NumberOfSpikes,
                RankTemplate = this.RankTemplate,
                RankTemplateName = this.RankTemplateName,
                TypeCategory = this.TypeCategory,
                LowerPercentageLimit = this.LowerPercentageLimit,
                UpperPercentageLimit = this.UpperPercentageLimit,
                ReductionPercentage = this.ReductionPercentage
            };

            return template;
        }

        public int LowerPercentageLimit { get; set; }

        public int UpperPercentageLimit { get; set; }

        public decimal MinRankValue { get; set; }

        public decimal MaxRankValue { get; set; }
    }
}
