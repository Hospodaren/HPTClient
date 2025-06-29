using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace HPTClient
{
    public class HPTMarkBetTemplate : Notifier
    {
        public virtual HPTMarkBetTemplate Clone()
        {
            return new HPTMarkBetTemplate()
            {
                DesiredSystemSize = this.DesiredSystemSize,
                Name = this.Name,
                NumberOfSpikes = this.NumberOfSpikes,
                RankTemplate = this.RankTemplate,
                RankTemplateName = this.RankTemplateName,
                TypeCategory = this.TypeCategory
            };
        }

        private string name;
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

        private string rankTemplatename;
        public string RankTemplateName
        {
            get
            {
                return this.rankTemplatename;
            }
            set
            {
                this.rankTemplatename = value;
                OnPropertyChanged("RankTemplateName");
            }
        }

        private HPTRankTemplate rankTemplate;
        [XmlIgnore]
        public HPTRankTemplate RankTemplate
        {
            get
            {
                return this.rankTemplate;
            }
            set
            {
                this.rankTemplate = value;
                if (value != null)
                {
                    this.RankTemplateName = this.rankTemplate.Name;
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
            return this.Name;
        }
    }
}
