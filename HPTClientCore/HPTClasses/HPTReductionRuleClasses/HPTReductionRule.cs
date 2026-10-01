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
            var ruleString = ToString(markBet);
            var rri = new ReductionRuleInfo()
            {
                ReductionTypeString = ReductionTypeString,
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
            ReductionSpecificationString = string.Empty;
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
            RuleResultForCorrectRow = string.Empty;
            return true;
        }

        #endregion

        #region Statistisk information om villkoret

        public string RuleResultForCorrectRow
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public decimal Probability
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        public decimal ProbabilityRelative
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        public int RemainingRows
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        public decimal RemainingRowsPercentage
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfAllCorrect
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfOneError
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfTwoErrors
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfThreeErrors
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Förberett för villkor endast på valda lopp

        [DataMember]
        public bool OnlyInSpecifiedLegs
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        public List<int> LegList { get; set; }

        [DataMember]
        public List<HPTLegSelection> LegSelectionList { get; set; }

        internal void InitializeLegSelectionList(int numberOfRaces)
        {
            LegSelectionList = Enumerable
                .Range(1, numberOfRaces)
                .Select(legNumber => new HPTLegSelection() { LegNumber = legNumber, Selected = false })
                .ToList();

            LegSelectionList.ForEach(l => l.PropertyChanged += legSelection_PropertyChanged);
        }

        void legSelection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            LegList = LegSelectionList
                .Where(ls => ls.Selected)
                .Select(ls => ls.LegNumber)
                .ToList();

            // Skapa lista med de lopp som ingår i regeln
            if (LegList.Count > 0)
            {
                OnlyInSpecifiedLegs = true;
            }
            else
            {
                OnlyInSpecifiedLegs = false;
            }

            //// Fullösning för att dra igång beräkningen...
            //OnPropertyChanged("Use");
        }

        #endregion
    }

    public class HPTLegSelection : Notifier
    {
        public int LegNumber { get; set; }

        public bool Selected
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }
}
