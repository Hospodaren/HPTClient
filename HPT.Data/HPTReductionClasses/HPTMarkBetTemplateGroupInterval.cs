using System.Collections.ObjectModel;

namespace HPTClient
{
    public class HPTMarkBetTemplateGroupInterval : HPTMarkBetTemplate
    {
        public HPTGroupIntervalRulesCollection GetGroupIntervalRulesCollection(string betType)
        {
            switch (betType)
            {
                case "V4":
                    return GroupIntervalRulesCollectionV4;
                case "V5":
                    return GroupIntervalRulesCollectionV5;
                case "V64":
                case "V65":
                    return GroupIntervalRulesCollectionV6X;
                case "V75":
                case "GS75":
                    return GroupIntervalRulesCollectionV75;
                case "V86":
                case "V85":
                    return GroupIntervalRulesCollectionV86;
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
                if (groupIntervalRulesCollectionList == null)
                {
                    groupIntervalRulesCollectionList = new ObservableCollection<HPTGroupIntervalRulesCollection>()
                    {
                        GroupIntervalRulesCollectionV4,
                        GroupIntervalRulesCollectionV5,
                        GroupIntervalRulesCollectionV6X,
                        GroupIntervalRulesCollectionV75,
                        GroupIntervalRulesCollectionV86
                    };
                }
                return groupIntervalRulesCollectionList;
            }
        }
    }
}
