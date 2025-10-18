using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTRankTemplate : Notifier
    {
        public HPTRankTemplate Clone()
        {
            var rankTemplate = new HPTRankTemplate()
            {
                Name = this.Name,
                HorseRankVariableList = HPTHorseRankVariable.CreateVariableList()
            };

            var rankVariablesToUse = this.HorseRankVariableList.Where(rv => rv.Use).ToList();
            foreach (var rankVariableToUse in rankVariablesToUse)
            {
                var rankVariable = rankTemplate.HorseRankVariableList.FirstOrDefault(rv => rv.PropertyName == rankVariableToUse.PropertyName);
                if (rankVariable != null)
                {
                    rankVariable.Weight = rankVariableToUse.Weight;
                    rankVariable.Use = rankVariableToUse.Use;
                }
            }
            return rankTemplate;
        }

        public virtual void InitializeTemplate()
        {
            this.HorseRankVariableList = HPTHorseRankVariable.CreateVariableList();
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

        [DataMember]
        public bool IsDefault { get; set; }

        [DataMember]
        public bool IsDefaultDouble { get; set; }

        [DataMember]
        public bool IsDefaultTvilling { get; set; }

        [DataMember]
        public bool IsDefaultTrio { get; set; }

        #region Different template types



        #endregion

        #region Variable lists

        private ObservableCollection<HPTHorseRankVariable> CreateRankVariableList(HPTRankCategory category)
        {
            if (this.HorseRankVariableList != null)
            {
                IEnumerable<HPTHorseRankVariable> tempList = this.HorseRankVariableList.Where(hrv => hrv.Category == category);
                return new ObservableCollection<HPTHorseRankVariable>(tempList);
            }
            return new ObservableCollection<HPTHorseRankVariable>();
        }

        private List<HPTHorseRankVariable> horseRankVariableList;
        [DataMember]
        public List<HPTHorseRankVariable> HorseRankVariableList
        {
            get
            {
                return this.horseRankVariableList;
            }
            set
            {
                this.horseRankVariableList = value;
                OnPropertyChanged("HorseRankVariableList");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListMarksAndOdds;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListMarksAndOdds
        {
            get
            {
                if (this.horseRankVariableListMarksAndOdds == null)
                {
                    this.horseRankVariableListMarksAndOdds = CreateRankVariableList(HPTRankCategory.MarksAndOdds);
                }
                return this.horseRankVariableListMarksAndOdds;
            }
            set
            {
                this.horseRankVariableListMarksAndOdds = value;
                OnPropertyChanged("HorseRankVariableListMarksAndOdds");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListRecords;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListRecords
        {
            get
            {
                if (this.horseRankVariableListRecords == null)
                {
                    this.horseRankVariableListRecords = CreateRankVariableList(HPTRankCategory.Record);
                }
                return this.horseRankVariableListRecords;
            }
            set
            {
                this.horseRankVariableListRecords = value;
                OnPropertyChanged("HorseRankVariableListRecords");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListWinning;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListWinning
        {
            get
            {
                if (this.horseRankVariableListWinning == null)
                {
                    this.horseRankVariableListWinning = CreateRankVariableList(HPTRankCategory.Winnings);
                }
                return this.horseRankVariableListWinning;
            }
            set
            {
                this.horseRankVariableListWinning = value;
                OnPropertyChanged("HorseRankVariableListWinning");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListPlace;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListPlace
        {
            get
            {
                if (this.horseRankVariableListPlace == null)
                {
                    this.horseRankVariableListPlace = CreateRankVariableList(HPTRankCategory.Place);
                }
                return this.horseRankVariableListPlace;
            }
            set
            {
                this.horseRankVariableListPlace = value;
                OnPropertyChanged("HorseRankVariableListPlace");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListRest;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListRest
        {
            get
            {
                if (this.horseRankVariableListRest == null)
                {
                    this.horseRankVariableListRest = CreateRankVariableList(HPTRankCategory.Rest);
                }
                return this.horseRankVariableListRest;
            }
            set
            {
                this.horseRankVariableListRest = value;
                OnPropertyChanged("HorseRankVariableListRest");
            }
        }

        #endregion

        public override string ToString()
        {
            return this.Name;
        }

    }
}
