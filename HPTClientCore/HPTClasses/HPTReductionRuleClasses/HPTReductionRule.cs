using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTReductionRule : Notifier
    {
        #region Virtuella metoder och properties

        public virtual string ToString(HPTMarkBet markBet)
        {
            return string.Empty;
        }

        public virtual IEnumerable<ReductionRuleInfo> GetReductionRuleInfoList(HPTMarkBet markBet)
        {
            var rri = GetReductionRuleInfo(markBet);
            return new ReductionRuleInfo[] { rri };
        }

        public virtual ReductionRuleInfo GetReductionRuleInfo(HPTMarkBet markBet)
        {
            string ruleString = this.ToString(markBet);
            ReductionRuleInfo rri = new ReductionRuleInfo()
            {
                ReductionTypeString = this.ReductionTypeString,
                ReductionRuleString = ruleString
            };
            return rri;
        }

        [XmlIgnore]
        public virtual string ClipboardString { get; set; }

        public virtual string ReductionTypeString
        {
            get
            {
                //return "Reduceringsvillkor";
                return string.Empty;
            }
        }

        public virtual void SetReductionSpecificationString()
        {
            this.ReductionSpecificationString = string.Empty;
        }

        public string ReductionSpecificationString { get; set; }

        public virtual void Reset()
        {
            return;
        }

        public virtual bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            return true;
        }

        public virtual bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            return true;
        }

        public virtual decimal CalculateProbability(HPTMarkBet markBet)
        {
            return 0M;
        }

        public virtual bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            this.RuleResultForCorrectRow = string.Empty;
            return true;
        }

        #endregion

        #region Statistisk information om villkoret

        private string ruleResultForCorrectRow;
        public string RuleResultForCorrectRow
        {
            get
            {
                return ruleResultForCorrectRow;
            }
            set
            {
                ruleResultForCorrectRow = value;
                OnPropertyChanged("RuleResultForCorrectRow");
            }
        }

        private decimal probability;
        public decimal Probability
        {
            get
            {
                return probability;
            }
            set
            {
                if (probability == value)
                {
                    return;
                }
                probability = value;
                OnPropertyChanged("Probability");
            }
        }

        private decimal probabilityRelative;
        public decimal ProbabilityRelative
        {
            get
            {
                return probabilityRelative;
            }
            set
            {
                if (probabilityRelative == value)
                {
                    return;
                }
                probabilityRelative = value;
                OnPropertyChanged("ProbabilityRelative");
            }
        }

        private int remainingRows;
        public int RemainingRows
        {
            get
            {
                return remainingRows;
            }
            set
            {
                if (remainingRows == value)
                {
                    return;
                }
                remainingRows = value;
                OnPropertyChanged("RemainingRows");
            }
        }

        private decimal remainingRowsPercentage;
        public decimal RemainingRowsPercentage
        {
            get
            {
                return remainingRowsPercentage;
            }
            set
            {
                if (remainingRowsPercentage == value)
                {
                    return;
                }
                remainingRowsPercentage = value;
                OnPropertyChanged("RemainingRowsPercentage");
            }
        }

        private int numberOfAllCorrect;
        public int NumberOfAllCorrect
        {
            get
            {
                return numberOfAllCorrect;
            }
            set
            {
                if (numberOfAllCorrect == value)
                {
                    return;
                }
                numberOfAllCorrect = value;
                OnPropertyChanged("NumberOfAllCorrect");
            }
        }

        private int numberOfOneError;
        public int NumberOfOneError
        {
            get
            {
                return numberOfOneError;
            }
            set
            {
                if (numberOfOneError == value)
                {
                    return;
                }
                numberOfOneError = value;
                OnPropertyChanged("NumberOfOneError");
            }
        }

        private int numberOfTwoErrors;
        public int NumberOfTwoErrors
        {
            get
            {
                return numberOfTwoErrors;
            }
            set
            {
                if (numberOfTwoErrors == value)
                {
                    return;
                }
                numberOfTwoErrors = value;
                OnPropertyChanged("NumberOfTwoErrors");
            }
        }

        private int numberOfThreeErrors;
        public int NumberOfThreeErrors
        {
            get
            {
                return numberOfThreeErrors;
            }
            set
            {
                if (numberOfThreeErrors == value)
                {
                    return;
                }
                numberOfThreeErrors = value;
                OnPropertyChanged("NumberOfThreeErrors");
            }
        }

        #endregion

        #region Förberett för villkor endast på valda lopp

        private bool onlyInSpecifiedLegs;
        [DataMember]
        public bool OnlyInSpecifiedLegs
        {
            get
            {
                return onlyInSpecifiedLegs;
            }
            set
            {
                if (onlyInSpecifiedLegs == value)
                {
                    return;
                }
                onlyInSpecifiedLegs = value;
                OnPropertyChanged("OnlyInSpecifiedLegs");
            }
        }

        public List<int> LegList { get; set; }

        [DataMember]
        public List<HPTLegSelection> LegSelectionList { get; set; }

        internal void InitializeLegSelectionList(int numberOfRaces)
        {
            this.LegSelectionList = Enumerable
                .Range(1, numberOfRaces)
                .Select(legNumber => new HPTLegSelection() { LegNumber = legNumber, Selected = false })
                .ToList();

            this.LegSelectionList.ForEach(l => l.PropertyChanged += legSelection_PropertyChanged);
        }

        void legSelection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.LegList = this.LegSelectionList
                .Where(ls => ls.Selected)
                .Select(ls => ls.LegNumber)
                .ToList();

            // Skapa lista med de lopp som ingår i regeln
            if (this.LegList.Count > 0)
            {
                this.OnlyInSpecifiedLegs = true;
            }
            else
            {
                this.OnlyInSpecifiedLegs = false;
            }

            //// Fullösning för att dra igång beräkningen...
            //OnPropertyChanged("Use");
        }

        #endregion
    }

    public class HPTLegSelection : Notifier
    {
        public int LegNumber { get; set; }

        private bool selected;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                OnPropertyChanged("Selected");
            }
        }

    }
}
