using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace HPTClient
{
    public class HPTMarkBetTemplateABCD : HPTMarkBetTemplate
    {
        public void InitializeTemplate(IEnumerable<HPTPrio> priosToUse)
        {
            this.ABCDTemplateSettingsList = new ObservableCollection<ABCDTemplateSettings>();
            foreach (HPTPrio prio in HPTConfig.Config.PrioList.Keys)
            {
                ABCDTemplateSettings settings = new ABCDTemplateSettings()
                    {
                        Prio = prio,
                        Selected = priosToUse.Contains(prio)
                    };
                this.ABCDTemplateSettingsList.Add(settings);
            }
        }

        public HPTMarkBetTemplateABCD Clone()
        {
            var template = new HPTMarkBetTemplateABCD()
            {
                DesiredSystemSize = this.DesiredSystemSize,
                Name = this.Name,
                NumberOfSpikes = this.NumberOfSpikes,
                RankTemplate = this.RankTemplate,
                RankTemplateName = this.RankTemplateName,
                TypeCategory = this.TypeCategory,
                ReductionPercentage = this.ReductionPercentage
            };
            template.InitializeTemplate(this.ABCDTemplateSettingsList.Where(abcd => abcd.Selected).Select(abcd => abcd.Prio));

            return template;
        }

        private ObservableCollection<ABCDTemplateSettings> abcdTemplateSettingsList;
        public ObservableCollection<ABCDTemplateSettings> ABCDTemplateSettingsList
        {
            get
            {
                return this.abcdTemplateSettingsList;
            }
            set
            {
                this.abcdTemplateSettingsList = value;
                OnPropertyChanged("ABCDTemplateSettingsList");
            }
        }
        
        internal IEnumerable<HPTPrio> PriosToUse
        {
            get
            {
                return this.ABCDTemplateSettingsList
                    .Where(ts => ts.Selected)
                    .Select(ts => ts.Prio);
            }
        }

        #region What Markbet types to use template for

        //public VxxTemplateABCDSettings GetVxxTemplateSettings(string betType)
        //{
        //    switch (betType)
        //    {
        //        case "V4":
        //            return this.V4Settings;
        //        case "V5":
        //            return this.V5Settings;
        //        case "V64":
        //        case "V65":
        //            return this.V6Settings;
        //        case "V75":
        //            return this.V75Settings;
        //        case "V86":
        //            return this.V86Settings;
        //        default:
        //            return null;
        //    }
        //}

        //private VxxTemplateABCDSettings v4Settings;
        //public VxxTemplateABCDSettings V4Settings
        //{
        //    get
        //    {
        //        return this.v4Settings;
        //    }
        //    set
        //    {
        //        this.v4Settings = value;
        //        OnPropertyChanged("V4Settings");
        //    }
        //}

        //private VxxTemplateABCDSettings v5Settings;
        //public VxxTemplateABCDSettings V5Settings
        //{
        //    get
        //    {
        //        return this.v5Settings;
        //    }
        //    set
        //    {
        //        this.v5Settings = value;
        //        OnPropertyChanged("V5Settings");
        //    }
        //}

        //private VxxTemplateABCDSettings v6Settings;
        //public VxxTemplateABCDSettings V6Settings
        //{
        //    get
        //    {
        //        return this.v6Settings;
        //    }
        //    set
        //    {
        //        this.v6Settings = value;
        //        OnPropertyChanged("V6Settings");
        //    }
        //}

        //private VxxTemplateABCDSettings v75Settings;
        //public VxxTemplateABCDSettings V75Settings
        //{
        //    get
        //    {
        //        return this.v75Settings;
        //    }
        //    set
        //    {
        //        this.v75Settings = value;
        //        OnPropertyChanged("V75Settings");
        //    }
        //}

        //private VxxTemplateABCDSettings v86Settings;
        //public VxxTemplateABCDSettings V86Settings
        //{
        //    get
        //    {
        //        return this.v86Settings;
        //    }
        //    set
        //    {
        //        this.v86Settings = value;
        //        OnPropertyChanged("V86Settings");
        //    }
        //}

        //private ObservableCollection<VxxTemplateABCDSettings> groupIntervalRulesCollectionList;
        //public ObservableCollection<VxxTemplateABCDSettings> GroupIntervalRulesCollection
        //{
        //    get
        //    {
        //        if (this.groupIntervalRulesCollectionList == null)
        //        {
        //            this.groupIntervalRulesCollectionList = new ObservableCollection<VxxTemplateABCDSettings>() 
        //            { 
        //                this.V4Settings,
        //                this.V5Settings,
        //                this.V6Settings,
        //                this.V75Settings,
        //                this.V86Settings
        //            };
        //        }
        //        return this.groupIntervalRulesCollectionList;
        //    }
        //}
        #endregion
    }
}
