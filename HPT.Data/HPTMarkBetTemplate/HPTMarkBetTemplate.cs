using System.Xml.Serialization;

namespace HPTClient
{
    public class HPTMarkBetTemplate : Notifier
    {
        public virtual HPTMarkBetTemplate Clone()
        {
            return new HPTMarkBetTemplate()
            {
                DesiredSystemSize = DesiredSystemSize,
                Name = Name,
                NumberOfSpikes = NumberOfSpikes,
                RankTemplate = RankTemplate,
                RankTemplateName = RankTemplateName,
                TypeCategory = TypeCategory
            };
        }

        private string name;
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

        private string rankTemplatename;
        public string RankTemplateName
        {
            get
            {
                return rankTemplatename;
            }
            set
            {
                rankTemplatename = value;
                OnPropertyChanged("RankTemplateName");
            }
        }

        private HPTRankTemplate rankTemplate;
        [XmlIgnore]
        public HPTRankTemplate RankTemplate
        {
            get
            {
                return rankTemplate;
            }
            set
            {
                rankTemplate = value;
                if (value != null)
                {
                    RankTemplateName = rankTemplate.Name;
                }
                OnPropertyChanged("RankTemplate");
            }
        }

        public bool Use { get; set; }

        public BetTypeCategory TypeCategory { get; set; }

        public int ReductionPercentage { get; set; }

        public int NumberOfSpikes { get; set; }

        public int DesiredSystemSize { get; set; }

        public bool IsDefault { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
