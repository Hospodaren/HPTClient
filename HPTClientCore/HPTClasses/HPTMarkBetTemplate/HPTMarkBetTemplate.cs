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

        public string Name
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public string RankTemplateName
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public HPTRankTemplate RankTemplate
        {
            get;
            set
            {
                field = value;
                if (value != null)
                {
                    RankTemplateName = field.Name;
                }
                OnPropertyChanged();
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
