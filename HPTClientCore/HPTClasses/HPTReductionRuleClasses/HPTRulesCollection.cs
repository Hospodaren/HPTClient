using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace HPTClient
{
    [XmlInclude(typeof(HPTPersonRulesCollection))]
    [XmlInclude(typeof(HPTComplementaryRulesCollection))]
    [XmlInclude(typeof(HPTGroupIntervalRulesCollection))]
    [DataContract]
    [KnownType(typeof(HPTPersonRulesCollection))]
    [KnownType(typeof(HPTComplementaryRulesCollection))]
    [KnownType(typeof(HPTGroupIntervalRulesCollection))]
    public class HPTRulesCollection : HPTNumberOfWinnersReductionRule
    {
        public HPTRulesCollection()
        {
        }

        public HPTRulesCollection(int numberOfRaces, bool use)
        {
            Use = use;
            NumberOfRaces = numberOfRaces;
            ReductionRuleList = new ObservableCollection<HPTNumberOfWinnersReductionRule>();
            NumberOfWinnersList = new ObservableCollection<HPTNumberOfWinners>();
            Initialize();
        }

        public override void SetReductionSpecificationString()
        {
            if (AllRulesMustApply)
            {
                ReductionSpecificationString = string.Empty;
            }
            else
            {
                ReductionSpecificationString = NumberOfWinnersString + " villkor";
            }

        }

        public void Initialize()
        {
            ReductionRuleList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ReductionRuleList_CollectionChanged);
        }

        void ReductionRuleList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IEnumerable<HPTNumberOfWinners> nowList = Enumerable.Range(0, ReductionRuleList.Count + 1)
                .Select(i => new HPTNumberOfWinners()
                {
                    NumberOfWinners = i,
                    Selectable = true,
                    Selected = i == ReductionRuleList.Count ? true : false
                });

            NumberOfWinnersList = new ObservableCollection<HPTNumberOfWinners>(nowList);
        }

        public override void Reset()
        {
            if (NumberOfWinnersList == null || NumberOfWinnersList.Count < ReductionRuleList.Count)
            {
                IEnumerable<HPTNumberOfWinners> nowList = Enumerable.Range(0, ReductionRuleList.Count + 1)
                .Select(i => new HPTNumberOfWinners()
                {
                    NumberOfWinners = i,
                    Selectable = true,
                    Selected = i == ReductionRuleList.Count ? true : false
                });

                NumberOfWinnersList = new ObservableCollection<HPTNumberOfWinners>(nowList);
            }
            if (NumberOfWinnersList.FirstOrDefault(now => now.Selected) != null)
            {
                AllRulesMustApply = NumberOfWinnersList.First(now => now.Selected).NumberOfWinners == ReductionRuleList.Count;
            }
            base.Reset();
        }

        public void Clear()
        {
            List<HPTNumberOfWinners> numberOfWinnersList =
                NumberOfWinnersList.Where(now => now.NumberOfWinners > 0).ToList();
            foreach (var numberOfWinners in numberOfWinnersList)
            {
                NumberOfWinnersList.Remove(numberOfWinners);
            }

            var zeroNumberOfWinners = NumberOfWinnersList.FirstOrDefault(now => now.NumberOfWinners == 0);
            if (zeroNumberOfWinners != null)
            {
                zeroNumberOfWinners.Selected = false;
            }
            ReductionRuleList.Clear();
        }

        [DataMember]
        public ObservableCollection<HPTNumberOfWinnersReductionRule> ReductionRuleList { get; set; }

        [XmlIgnore]
        public bool AllRulesMustApply { get; set; }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            if (ReductionRuleList.Count == 0)
            {
                return true;
            }
            int numberOfRules = 0;
            foreach (var rule in ReductionRuleList)
            {
                if (!rule.Use || rule.IncludeRow(markBet, singleRow))
                {
                    numberOfRules++;
                }
                else if (AllRulesMustApply)
                {
                    return false;
                }
            }
            return NumberOfWinnersList.First(now => now.NumberOfWinners == numberOfRules).Selected;
        }

        public override IEnumerable<ReductionRuleInfo> GetReductionRuleInfoList(HPTMarkBet markBet)
        {
            var ruleInfoList = new List<ReductionRuleInfo>()
                {
                    new ReductionRuleInfo()
                    {
                        //HeadlineString = "Multi-ABCD"
                        HeadlineString = ReductionTypeString
                    }
                };

            ReductionRuleList
                .Where(r => r.Use)
                .ToList()
                .ForEach(r => ruleInfoList.Add(r.GetReductionRuleInfo(markBet)));

            return ruleInfoList;
        }

        public override string ToString(HPTMarkBet markBet)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var rule in ReductionRuleList.Where(r => r.Use))
            {
                sb.AppendLine(rule.ToString(markBet));
                sb.AppendLine();
            }
            ClipboardString = sb.ToString();
            return sb.ToString();
        }
    }
}
