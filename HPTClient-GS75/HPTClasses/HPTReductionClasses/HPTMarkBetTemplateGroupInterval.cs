using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace HPTClient
{
    public class HPTMarkBetTemplateGroupInterval : HPTMarkBetTemplate
    {
        public HPTGroupIntervalRulesCollection GetGroupIntervalRulesCollection(string betType)
        {
            switch (betType)
            {
                case "V4":
                    return this.GroupIntervalRulesCollectionV4;
                case "V5":
                    return this.GroupIntervalRulesCollectionV5;
                case "V64":
                case "V65":
                    return this.GroupIntervalRulesCollectionV6X;
                case "V75":
                case "GS75":
                    return this.GroupIntervalRulesCollectionV75;
                case "V86":
                    return this.GroupIntervalRulesCollectionV86;
                default:
                    return null;
            }
        }

        public HPTGroupIntervalRulesCollection GroupIntervalRulesCollectionV4 { get; set; }

        public HPTGroupIntervalRulesCollection GroupIntervalRulesCollectionV5 { get; set; }

        public HPTGroupIntervalRulesCollection GroupIntervalRulesCollectionV6X { get; set; }

        public HPTGroupIntervalRulesCollection GroupIntervalRulesCollectionV75 { get; set; }

        public HPTGroupIntervalRulesCollection GroupIntervalRulesCollectionV86 { get; set; }

        private ObservableCollection<HPTGroupIntervalRulesCollection> groupIntervalRulesCollectionList;
        public ObservableCollection<HPTGroupIntervalRulesCollection> GroupIntervalRulesCollection
        {
            get
            {
                if (this.groupIntervalRulesCollectionList == null)
                {
                    this.groupIntervalRulesCollectionList = new ObservableCollection<HPTGroupIntervalRulesCollection>() 
                    { 
                        this.GroupIntervalRulesCollectionV4,
                        this.GroupIntervalRulesCollectionV5,
                        this.GroupIntervalRulesCollectionV6X,
                        this.GroupIntervalRulesCollectionV75,
                        this.GroupIntervalRulesCollectionV86
                    };
                }
                return this.groupIntervalRulesCollectionList;
            }
        }
    }
}
