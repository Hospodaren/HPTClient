using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    class HPTMultipleChoiceReductionRule : HPTReductionRule
    {
        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            if (!this.Use)
            {
                return false;
            }

            int numberOfRulesFulfilled = 0;
            foreach (var reductionRule in markBet.ReductionRulesToApply)
            {
                if (reductionRule.IncludeRow(markBet, singleRow))
                {
                    numberOfRulesFulfilled++;
                }
            }
            return this.NumberOfRulesList.First(nor => nor.NumberOfRules == numberOfRulesFulfilled).Selected;
        }

        internal void UpdateNumberOfRules(int numberOfRules)
        {
            if (this.NumberOfRulesList == null)
            {
                IEnumerable<HPTNumberOfRules> norList = Enumerable.Range(1, numberOfRules)
                    .Select(nor => new HPTNumberOfRules()
                    {
                        NumberOfRules = nor
                    });
                this.NumberOfRulesList = new ObservableCollection<HPTNumberOfRules>(norList);
            }
            else if (this.numberOfRulesList.Count == numberOfRules)
            {
                return;
            }
            else if (this.numberOfRulesList.Count < numberOfRules)
            {
                int position = this.NumberOfRulesList.Count;
                while (position > numberOfRules)
                {
                    HPTNumberOfRules nor = this.NumberOfRulesList[position - 1];
                    nor.Selected = false;
                    this.NumberOfRulesList.Remove(nor);
                    position--;
                }
            }
            else if (this.numberOfRulesList.Count > numberOfRules)
            {
                int position = this.NumberOfRulesList.Count;
                while (position < numberOfRules)
                {
                    HPTNumberOfRules nor = new HPTNumberOfRules()
                    {
                        NumberOfRules = position
                    };
                    this.NumberOfRulesList.Add(nor);
                    position++;
                }
            }
        }

        private bool use;
        [DataMember]
        public bool Use
        {
            get
            {
                return this.use;
            }
            set
            {
                this.use = value;
                OnPropertyChanged("Use");
            }
        }

        private ObservableCollection<HPTNumberOfRules> numberOfRulesList;
        [DataMember]
        public ObservableCollection<HPTNumberOfRules> NumberOfRulesList
        {
            get
            {
                return this.numberOfRulesList;
            }
            set
            {
                this.numberOfRulesList = value;
                OnPropertyChanged("NumberOfRulesList");
            }
        }
    }
}
