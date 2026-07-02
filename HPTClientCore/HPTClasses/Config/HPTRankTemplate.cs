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

        [DataMember]
        public string Name
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
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
                var tempList = HorseRankVariableList.Where(hrv => hrv.Category == category);
                return new ObservableCollection<HPTHorseRankVariable>(tempList);
            }
            return new ObservableCollection<HPTHorseRankVariable>();
        }

        [DataMember]
        public List<HPTHorseRankVariable> HorseRankVariableList
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListMarksAndOdds
        {
            get
            {
                if (field == null)
                {
                    field = CreateRankVariableList(HPTRankCategory.MarksAndOdds);
                }
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListRecords
        {
            get
            {
                if (field == null)
                {
                    field = CreateRankVariableList(HPTRankCategory.Record);
                }
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListWinning
        {
            get
            {
                if (field == null)
                {
                    field = CreateRankVariableList(HPTRankCategory.Winnings);
                }
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListPlace
        {
            get
            {
                if (field == null)
                {
                    field = CreateRankVariableList(HPTRankCategory.Place);
                }
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public ObservableCollection<HPTHorseRankVariable> HorseRankVariableListRest
        {
            get
            {
                if (field == null)
                {
                    field = CreateRankVariableList(HPTRankCategory.Rest);
                }
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }

    }
}
