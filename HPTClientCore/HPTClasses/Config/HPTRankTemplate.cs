using System.Collections.ObjectModel;
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
                Name = Name,
                HorseRankVariableList = HPTHorseRankVariable.CreateVariableList()
            };

            var rankVariablesToUse = HorseRankVariableList.Where(rv => rv.Use).ToList();
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
            HorseRankVariableList = HPTHorseRankVariable.CreateVariableList();
        }

        private string name;
        [DataMember]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
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
            if (HorseRankVariableList != null)
            {
                IEnumerable<HPTHorseRankVariable> tempList = HorseRankVariableList.Where(hrv => hrv.Category == category);
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
                return horseRankVariableList;
            }
            set
            {
                horseRankVariableList = value;
                OnPropertyChanged("HorseRankVariableList");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListMarksAndOdds;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListMarksAndOdds
        {
            get
            {
                if (horseRankVariableListMarksAndOdds == null)
                {
                    horseRankVariableListMarksAndOdds = CreateRankVariableList(HPTRankCategory.MarksAndOdds);
                }
                return horseRankVariableListMarksAndOdds;
            }
            set
            {
                horseRankVariableListMarksAndOdds = value;
                OnPropertyChanged("HorseRankVariableListMarksAndOdds");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListRecords;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListRecords
        {
            get
            {
                if (horseRankVariableListRecords == null)
                {
                    horseRankVariableListRecords = CreateRankVariableList(HPTRankCategory.Record);
                }
                return horseRankVariableListRecords;
            }
            set
            {
                horseRankVariableListRecords = value;
                OnPropertyChanged("HorseRankVariableListRecords");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListWinning;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListWinning
        {
            get
            {
                if (horseRankVariableListWinning == null)
                {
                    horseRankVariableListWinning = CreateRankVariableList(HPTRankCategory.Winnings);
                }
                return horseRankVariableListWinning;
            }
            set
            {
                horseRankVariableListWinning = value;
                OnPropertyChanged("HorseRankVariableListWinning");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListPlace;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListPlace
        {
            get
            {
                if (horseRankVariableListPlace == null)
                {
                    horseRankVariableListPlace = CreateRankVariableList(HPTRankCategory.Place);
                }
                return horseRankVariableListPlace;
            }
            set
            {
                horseRankVariableListPlace = value;
                OnPropertyChanged("HorseRankVariableListPlace");
            }
        }

        private ObservableCollection<HPTHorseRankVariable> horseRankVariableListRest;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListRest
        {
            get
            {
                if (horseRankVariableListRest == null)
                {
                    horseRankVariableListRest = CreateRankVariableList(HPTRankCategory.Rest);
                }
                return horseRankVariableListRest;
            }
            set
            {
                horseRankVariableListRest = value;
                OnPropertyChanged("HorseRankVariableListRest");
            }
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }

    }
}
