using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    class HPTMultipleChoiceReductionRule : HPTReductionRule
    {
        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            if (!Use)
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
            return NumberOfRulesList.First(nor => nor.NumberOfRules == numberOfRulesFulfilled).Selected;
        }

        internal void UpdateNumberOfRules(int numberOfRules)
        {
            if (NumberOfRulesList == null)
            {
                IEnumerable<HPTNumberOfRules> norList = Enumerable.Range(1, numberOfRules)
                    .Select(nor => new HPTNumberOfRules()
                    {
                        NumberOfRules = nor
                    });
                NumberOfRulesList = new ObservableCollection<HPTNumberOfRules>(norList);
            }
            else if (numberOfRulesList.Count == numberOfRules)
            {
                return;
            }
            else if (numberOfRulesList.Count < numberOfRules)
            {
                int position = NumberOfRulesList.Count;
                while (position > numberOfRules)
                {
                    HPTNumberOfRules nor = NumberOfRulesList[position - 1];
                    nor.Selected = false;
                    NumberOfRulesList.Remove(nor);
                    position--;
                }
            }
            else if (numberOfRulesList.Count > numberOfRules)
            {
                int position = NumberOfRulesList.Count;
                while (position < numberOfRules)
                {
                    HPTNumberOfRules nor = new HPTNumberOfRules()
                    {
                        NumberOfRules = position
                    };
                    NumberOfRulesList.Add(nor);
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
                return use;
            }
            set
            {
                use = value;
                OnPropertyChanged("Use");
            }
        }

        private ObservableCollection<HPTNumberOfRules> numberOfRulesList;
        [DataMember]
        public ObservableCollection<HPTNumberOfRules> NumberOfRulesList
        {
            get
            {
                return numberOfRulesList;
            }
            set
            {
                numberOfRulesList = value;
                OnPropertyChanged("NumberOfRulesList");
            }
        }
    }
}
