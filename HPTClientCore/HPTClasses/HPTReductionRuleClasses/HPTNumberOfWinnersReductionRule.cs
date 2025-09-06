using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace HPTClient
{
    [XmlInclude(typeof(HPTDriverReductionRule))]
    [XmlInclude(typeof(HPTTrainerReductionRule))]
    [XmlInclude(typeof(HPTComplementaryReductionRule))]
    [XmlInclude(typeof(HPTGroupIntervalReductionRule))]
    [DataContract]
    [KnownType(typeof(HPTDriverReductionRule))]
    [KnownType(typeof(HPTTrainerReductionRule))]
    [KnownType(typeof(HPTComplementaryReductionRule))]
    [KnownType(typeof(HPTGroupIntervalReductionRule))]
    public class HPTNumberOfWinnersReductionRule : HPTReductionRule
    {
        public HPTNumberOfWinnersReductionRule()
        {
        }

        public HPTNumberOfWinnersReductionRule(int numberOfRaces, bool use)
        {
            this.Use = use;
            this.NumberOfRaces = numberOfRaces;

            var nowList = Enumerable.Range(0, numberOfRaces + 1)
                .Select(i => new HPTNumberOfWinners()
                {
                    NumberOfWinners = i,
                    Selectable = i == 0 ? true : false
                });

            this.NumberOfWinnersList = new ObservableCollection<HPTNumberOfWinners>(nowList);
        }

        public virtual void SetSkipRule()
        {
            int numberOfSelected = this.NumberOfWinnersList.Count(now => now.Selected);
            this.SkipRule = numberOfSelected == 0 || numberOfSelected == this.NumberOfWinnersList.Count;
            if (this.LegSelectionList == null || this.LegSelectionList.Count == 0)
            {
                this.OnlyInSpecifiedLegs = false;
                return;
            }
            this.OnlyInSpecifiedLegs = this.LegSelectionList.Count(ls => ls.Selected) > 0;
            if (this.OnlyInSpecifiedLegs)
            {
                this.LegList = this.LegSelectionList
                .Where(ls => ls.Selected)
                .Select(ls => ls.LegNumber)
                .ToList();
            }
        }

        private int numberOfRaces;
        [DataMember]
        public int NumberOfRaces
        {
            get
            {
                return this.numberOfRaces;
            }
            set
            {
                this.numberOfRaces = value;
                OnPropertyChanged("NumberOfRaces");
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

        private string name;
        [DataMember]
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

        private int numberOfSelected;
        [DataMember]
        public int NumberOfSelected
        {
            get
            {
                return this.numberOfSelected;
            }
            set
            {
                this.numberOfSelected = value;
                OnPropertyChanged("NumberOfSelected");
            }
        }

        [XmlIgnore]
        public int MinNumberOfX { get; set; }

        [XmlIgnore]
        public int MaxNumberOfX { get; set; }

        [XmlIgnore]
        public bool SkipRule { get; set; }

        public override void Reset()
        {
            var hptNow = this.NumberOfWinnersList.OrderBy(now => now.NumberOfWinners).FirstOrDefault(now => now.Selected);
            this.MinNumberOfX = hptNow == null ? 0 : hptNow.NumberOfWinners;
            hptNow = this.NumberOfWinnersList.Where(now => now.Selected).OrderByDescending(now => now.NumberOfWinners).FirstOrDefault();
            this.MaxNumberOfX = hptNow == null ? 0 : hptNow.NumberOfWinners;
            foreach (var now in this.NumberOfWinnersList)
            {
                now.IsSuperfluous = false;
            }
        }

        private ObservableCollection<HPTNumberOfWinners> numberOfWinnersList;
        [DataMember]
        public ObservableCollection<HPTNumberOfWinners> NumberOfWinnersList
        {
            get
            {
                return this.numberOfWinnersList;
            }
            set
            {
                this.numberOfWinnersList = value;
                OnPropertyChanged("NumberOfWinnersList");
            }
        }

        public override void SetReductionSpecificationString()
        {
            var sb = new StringBuilder();
            sb.Append(this.NumberOfWinnersString);
            sb.Append(" vinnare");
            if (this.OnlyInSpecifiedLegs)
            {
                sb.Append(" (");
                sb.Append(this.SelectedRacesString);
                sb.Append(")");
            }
            this.ReductionSpecificationString = sb.ToString();
        }

        protected virtual string NumberOfWinnersString
        {
            get
            {
                Reset();
                if (this.numberOfWinnersList.Count(now => now.Selected) == 0)
                {
                    return string.Empty;
                }
                return this.numberOfWinnersList
                    .Where(now => now.Selected)
                    .Select(now => now.NumberOfWinners.ToString())
                    .Aggregate((now, next) => now + ", " + next);
            }
        }

        protected virtual string SelectedRacesString
        {
            get
            {
                return "Avd " + this.LegList
                    .Select(l => l.ToString())
                    .Aggregate((l, next) => l + ", " + next);
            }
        }

        public void SelectInterval(int start, int length, bool selected)
        {
            for (int i = start; i < start + length; i++)
            {
                var hptNow = this.NumberOfWinnersList.FirstOrDefault(now => now.NumberOfWinners == i);
                if (hptNow != null)
                {
                    hptNow.Selected = selected;
                }
            }
        }

        public void SetSelectable(int upperBoundary)
        {
            for (int i = 0; i <= upperBoundary; i++)
            {
                var hptNow = this.NumberOfWinnersList.FirstOrDefault(now => now.NumberOfWinners == i);
                if (hptNow != null)
                {
                    hptNow.Selectable = true;
                }
            }
        }

        internal void now_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                OnPropertyChanged("NumberOfWinnersSelected");
            }
        }

        //protected virtual string NumberOfWinnersString
        //{
        //    get
        //    {
        //        Reset();
        //        StringBuilder sb = new StringBuilder();

        //        for (int i = this.MinNumberOfX; i <= this.MaxNumberOfX; i++)
        //        {
        //            HPTNumberOfWinners hptNow = this.NumberOfWinnersList.FirstOrDefault(now => now.NumberOfWinners == i);
        //            if (hptNow != null && hptNow.Selected)
        //            {
        //                sb.Append(i);
        //                sb.Append(",");
        //            }
        //        }
        //        if (sb.Length > 1)
        //        {
        //            sb.Remove(sb.Length - 1, 1);
        //        }
        //        return sb.ToString();
        //    }
        //}
    }
}
