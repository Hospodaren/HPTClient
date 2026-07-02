using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseRankVariableBase : Notifier
    {
        public static List<HPTHorseRankVariableBase> CreateVariableBaseList()
        {
            var variableList = new List<HPTHorseRankVariableBase>();

            foreach (var pi in (typeof(HPTHorse)).GetProperties())
            {
                foreach (var o in pi.GetCustomAttributes(true))
                {
                    if (o.GetType() == typeof(HorseRankAttribute))
                    {
                        var hra = (HorseRankAttribute)o;
                        var variable = new HPTHorseRankVariableBase();
                        try
                        {
                            variable.PropertyName = pi.Name;
                            variable.Text = hra.Name;
                            variable.CategoryText = EnumHelper.GetTextFromRankCategory(hra.Category);
                            variable.Show = true;
                            variableList.Add(variable);
                        }
                        catch (Exception exc)
                        {
                            var s = exc.Message;
                        }
                    }
                }
            }
            return variableList;
            //return variableList.OrderBy(rv => rv.Order).ToList();
        }

        [DataMember]
        public bool Show
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

        [DataMember]
        public virtual string PropertyName
        {
            get;
            set
            {
                if (value == "MarksPercent" || value == "MarksQuantity" || string.IsNullOrEmpty(value))
                {
                    return;
                }
                field = value;

                try
                {
                    var pi = (typeof(HPTHorse)).GetProperty(field);
                    var hra = (HorseRankAttribute)pi.GetCustomAttribute(typeof(HorseRankAttribute));
                    Text = hra.Name;
                    CategoryText = EnumHelper.GetTextFromRankCategory(hra.Category);
                }
                catch (Exception exc)
                {
                    var s = exc.Message;
                }
            }
        }

        [XmlIgnore]
        public string CategoryText { get; set; }

        [XmlIgnore]
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{Text} ({CategoryText})";
        }
    }

    [DataContract]
    public class HPTHorseRank : Notifier
    {
        [DataMember]
        public int Rank
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged();
                }
            }
        }

        [DataMember]
        public decimal RankWeighted
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged();
                }
            }
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

        [XmlIgnore]
        public SolidColorBrush BackColor
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string RankValueString { get; set; }

        public bool Use { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [DataContract]
    public class HPTHorseRankVariable : HPTHorseRankVariableBase
    {
        public HPTHorseRankVariable()
        {
            RaceHorseRankList = new ObservableCollection<RaceHorseRank>();
        }

        [OnDeserialized]
        public void InitializeOnDeserialized(StreamingContext sc)
        {
            RaceHorseRankList = new ObservableCollection<RaceHorseRank>();
        }

        public HPTHorseRankVariable Clone()
        {
            var rankVariable = new HPTHorseRankVariable();

            rankVariable.Calculated = Calculated;
            rankVariable.Category = Category;
            rankVariable.CategoryText = CategoryText;
            rankVariable.Descending = Descending;
            rankVariable.HorseProperty = HorseProperty;
            rankVariable.HorseRankInfo = HorseRankInfo;
            rankVariable.IsStatic = IsStatic;
            rankVariable.RaceHorseRankList = RaceHorseRankList;
            rankVariable.Sort = Sort;
            rankVariable.PropertyName = PropertyName;
            rankVariable.Text = Text;
            rankVariable.Use = Use;
            rankVariable.Weight = Weight;

            return rankVariable;
        }

        #region Static methods and lists

        public static List<HPTHorseRankVariable> CreateVariableList()
        {
            var variableList = new List<HPTHorseRankVariable>();

            foreach (var pi in (typeof(HPTHorse)).GetProperties())
            {
                foreach (var o in pi.GetCustomAttributes(true))
                {
                    if (o.GetType() == typeof(HorseRankAttribute))
                    {
                        var hra = (HorseRankAttribute)o;
                        var variable = new HPTHorseRankVariable();
                        try
                        {
                            variable.PropertyName = pi.Name;
                            variable.IsStatic = hra.IsStatic;
                            variable.Descending = hra.Descending;
                            variable.Category = hra.Category;
                            variable.Text = hra.Name;
                            variable.Sort = hra.Sort;
                            variable.HorseRankInfo = hra;
                            variable.Weight = 1M;
                            variable.DisplayPropertyName = hra.DisplayPropertyName;
                            variable.ValueForMissing = hra.ValueForMissing;
                            variable.StringFormat = hra.StringFormat;
                            variable.Order = hra.Order;
                            variableList.Add(variable);
                        }
                        catch (Exception exc)
                        {
                            var s = exc.Message;
                        }
                    }
                }
            }
            return variableList.OrderBy(rv => rv.Order).ToList();
        }

        #endregion

        #region Properties

        [DataMember]
        public override string PropertyName
        {
            get;
            set
            {
                if (value == "MarksPercent" || value == "MarksQuantity" || string.IsNullOrEmpty(value))
                {
                    return;
                }

                field = value;
                if (field == "HistoryRelativeDifference")
                {
                    field = "HistoryRelativeDifferenceUnadjusted";
                }

                try
                {
                    if (horseRankInfo == null || HorseProperty == null)
                    {
                        HorseProperty = typeof(HPTHorse).GetProperty(field);
                        if (HorseProperty == null)
                        {
                            return;
                        }

                        var attributeArray = HorseProperty.GetCustomAttributes(typeof(HorseRankAttribute), true);
                        if (attributeArray == null || attributeArray.Length == 0)
                        {
                            field = null;
                            return;
                        }
                        HorseRankInfo = (HorseRankAttribute)attributeArray[0];

                        IsStatic = HorseRankInfo.IsStatic;
                        Descending = HorseRankInfo.Descending;
                        Category = HorseRankInfo.Category;
                        CategoryText = EnumHelper.GetTextFromRankCategory(Category);
                        Text = HorseRankInfo.Name;
                        Sort = HorseRankInfo.Sort;
                        DisplayPropertyName = HorseRankInfo.DisplayPropertyName;
                        ValueForMissing = HorseRankInfo.ValueForMissing;
                        StringFormat = HorseRankInfo.StringFormat;
                        Order = HorseRankInfo.Order;
                    }
                }
                catch (Exception exc)
                {
                    var s = exc.Message;
                }
            }
        }

        [XmlIgnore]
        public HPTRankCategory Category { get; set; }

        [XmlIgnore]
        public bool Calculated { get; set; }

        [XmlIgnore]
        public bool Descending { get; set; }

        [XmlIgnore]
        public bool IsStatic { get; set; }

        [DataMember]
        public bool Use
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

        [DataMember]
        public decimal Weight
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string DisplayPropertyName { get; private set; }

        [XmlIgnore]
        public string StringFormat { get; private set; }

        [XmlIgnore]
        public decimal ValueForMissing { get; private set; }

        [XmlIgnore]
        public int Order { get; set; }

        [XmlIgnore]
        public bool Sort { get; set; }

        #endregion

        public void SortHorseList(List<HPTHorse> horseList)
        {
            lock (this)
            {
                if (HorseProperty == null)
                {
                    HorseProperty = (typeof(HPTHorse)).GetProperty(PropertyName);
                }

                var legNr = horseList[0].ParentRace.LegNr;

                var raceHorseRank = RaceHorseRankList.FirstOrDefault(hr => hr.RaceNumber == legNr);
                if (raceHorseRank == null)
                {
                    raceHorseRank = new RaceHorseRank()
                    {
                        RaceNumber = legNr,
                        HorseRankList = new ObservableCollection<HPTHorseRank>()
                    };
                    RaceHorseRankList.Add(raceHorseRank);
                }

                // Sortera på vald variabel
                if (Sort)
                {
                    horseList.Sort(CompareHorses);
                }

                HPTHorse previousHorse = null;
                var sortRank = 0;
                for (var i = 0; i < horseList.Count; i++)
                {
                    var tempRank = i;
                    var horse = horseList[i];
                    var horseRank = horse.RankList.FirstOrDefault(r => r.Name == PropertyName);
                    if (horseRank == null)
                    {
                        return;
                    }
                    if (Sort)
                    {
                        if (previousHorse != null)
                        {
                            var previousRankValue = Convert.ToDecimal(HorseProperty.GetValue(previousHorse, null));
                            var rankValue = Convert.ToDecimal(HorseProperty.GetValue(horse, null));
                            if (previousRankValue != rankValue)
                            {
                                sortRank = i + 1;
                            }
                        }
                        else
                        {
                            sortRank = i + 1;
                        }
                        horseRank.Rank = sortRank;
                    }
                    else
                    {
                        sortRank = Convert.ToInt32(HorseProperty.GetValue(horse, null));
                        if (PropertyName == "RankABC")
                        {
                            horseRank.Rank = horse.GetRankABC();
                        }
                        else
                        {
                            horseRank.Rank = sortRank == 0 ? horse.ParentRace.HorseList.Count : sortRank;
                        }
                    }
                    horseRank.Use = false;
                    horseRank.RankWeighted = horseRank.Rank * Weight;

                    // Bakgrundsfärg för översiktsvyn
                    var c = Colors.LightGray;
                    //switch (tempRank + 1)
                    switch (sortRank)
                    {
                        case 0:
                        case 1: // Rank 1
                            c = Colors.Green;
                            break;
                        case 2: // Rank 2 och 3
                        case 3:
                            c = Colors.Yellow;
                            break;
                        case 4: // Rank 4 till 6
                        case 5:
                        case 6:
                            c = Colors.LightYellow;
                            break;
                        default:    // Sämre rank
                            c = Colors.IndianRed;
                            break;
                    }
                    horseRank.BackColor = new SolidColorBrush(c);
                    previousHorse = horse;
                }
                try
                {
                    if (raceHorseRank.HorseRankList.Count == 0)
                    {
                        var raceHorseRankList = horseList.OrderBy(h => h.StartNr).SelectMany(h => h.RankList.Where(hrv => hrv.Name == PropertyName)).ToList();
                        raceHorseRank.HorseRankList = new ObservableCollection<HPTHorseRank>(raceHorseRankList);
                    }
                }
                catch (Exception exc)
                {
                    var s = exc.Message;
                }
                Calculated = true;
            }
        }

        private int CompareHorses(HPTHorse h1, HPTHorse h2)
        {
            try
            {
                if (h1.Scratched == true && h2.Scratched == true)
                {
                    return 0;
                }
                if (h1.Scratched == true)
                {
                    return 1;
                }
                if (h2.Scratched == true)
                {
                    return -1;
                }

                var exactDiff = Convert.ToDecimal(HorseProperty.GetValue(h1, null)) - Convert.ToDecimal(HorseProperty.GetValue(h2, null));
                if (exactDiff == 0M)
                {
                    return 0;
                }

                var diff = exactDiff > 0M ? 1 : -1;
                return Descending ? diff * -1 : diff;
            }
            catch (Exception exc)
            {
                var s = exc.Message;
                return 0;
            }
        }

        internal PropertyInfo HorseProperty { get; set; }

        private HorseRankAttribute horseRankInfo;
        [XmlIgnore]
        public HorseRankAttribute HorseRankInfo
        {
            get
            {
                return horseRankInfo;
            }
            set
            {
                horseRankInfo = value;
                OnPropertyChanged();
            }
        }

        #region Hanteringen av översiktsvy för Rankvariabler

        [XmlIgnore]
        public ObservableCollection<RaceHorseRank> RaceHorseRankList
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }

    [DataContract]
    public class RaceHorseRank : Notifier
    {
        public RaceHorseRank()
        {
            //this.HorseRankListSorted = new SortedList<int, HPTHorseRank>();
            HorseRankList = new ObservableCollection<HPTHorseRank>();
        }

        [DataMember]
        public int RaceNumber
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTHorseRank> HorseRankList
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public enum HPTRankCategory
    {
        MarksAndOdds,
        Record,
        Winnings,
        Place,
        Rest,
        Top3
    }
}
