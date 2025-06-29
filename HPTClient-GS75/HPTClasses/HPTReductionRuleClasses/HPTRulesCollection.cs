using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

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
            this.Use = use;
            this.NumberOfRaces = numberOfRaces;
            this.ReductionRuleList = new ObservableCollection<HPTNumberOfWinnersReductionRule>();            
            this.NumberOfWinnersList = new ObservableCollection<HPTNumberOfWinners>();
            Initialize();
        }

        public override void SetReductionSpecificationString()
        {
            if (this.AllRulesMustApply)
            {
                this.ReductionSpecificationString = string.Empty;
            }
            else
            {
                this.ReductionSpecificationString = this.NumberOfWinnersString + " villkor";
            }
            
        }

        public void Initialize()
        {
            this.ReductionRuleList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ReductionRuleList_CollectionChanged);
        }

        void ReductionRuleList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IEnumerable<HPTNumberOfWinners> nowList = Enumerable.Range(0, this.ReductionRuleList.Count + 1)
                .Select(i => new HPTNumberOfWinners()
                {
                    NumberOfWinners = i,
                    Selectable = true,
                    Selected = i == this.ReductionRuleList.Count ? true : false
                });

            this.NumberOfWinnersList = new ObservableCollection<HPTNumberOfWinners>(nowList);
        }

        public override void Reset()
        {
            if (this.NumberOfWinnersList == null || this.NumberOfWinnersList.Count < this.ReductionRuleList.Count)
            {
                IEnumerable<HPTNumberOfWinners> nowList = Enumerable.Range(0, this.ReductionRuleList.Count + 1)
                .Select(i => new HPTNumberOfWinners()
                {
                    NumberOfWinners = i,
                    Selectable = true,
                    Selected = i == this.ReductionRuleList.Count ? true : false
                });

                this.NumberOfWinnersList = new ObservableCollection<HPTNumberOfWinners>(nowList);
            }
            if (this.NumberOfWinnersList.FirstOrDefault(now => now.Selected) != null)
            {
                this.AllRulesMustApply = this.NumberOfWinnersList.First(now => now.Selected).NumberOfWinners == this.ReductionRuleList.Count;
            }
            base.Reset();
        }

        public void Clear()
        {
            List<HPTNumberOfWinners> numberOfWinnersList =
                this.NumberOfWinnersList.Where(now => now.NumberOfWinners > 0).ToList();
            foreach (var numberOfWinners in numberOfWinnersList)
            {
                this.NumberOfWinnersList.Remove(numberOfWinners);
            }

            var zeroNumberOfWinners = this.NumberOfWinnersList.FirstOrDefault(now => now.NumberOfWinners == 0);
            if (zeroNumberOfWinners != null)
            {
                zeroNumberOfWinners.Selected = false;
            }
            this.ReductionRuleList.Clear();
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
            foreach (var rule in this.ReductionRuleList)
            {
                if (!rule.Use || rule.IncludeRow(markBet, singleRow))
                {
                    numberOfRules++;
                }
                else if (this.AllRulesMustApply)
                {
                    return false;
                }
            }
            return this.NumberOfWinnersList.First(now => now.NumberOfWinners == numberOfRules).Selected;
        }

        public override IEnumerable<ReductionRuleInfo> GetReductionRuleInfoList(HPTMarkBet markBet)
        {
            var ruleInfoList = new List<ReductionRuleInfo>()
                {
                    new ReductionRuleInfo()
                    {
                        //HeadlineString = "Multi-ABCD"
                        HeadlineString = this.ReductionTypeString
                    }
                };

            this.ReductionRuleList
                .Where(r => r.Use)
                .ToList()
                .ForEach(r => ruleInfoList.Add(r.GetReductionRuleInfo(markBet)));

            return ruleInfoList;
        }

        public override string ToString(HPTMarkBet markBet)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var rule in this.ReductionRuleList.Where(r => r.Use))
            {
                sb.AppendLine(rule.ToString(markBet));
                sb.AppendLine();
            }
            this.ClipboardString = sb.ToString();
            return sb.ToString();
        }
    }
}
