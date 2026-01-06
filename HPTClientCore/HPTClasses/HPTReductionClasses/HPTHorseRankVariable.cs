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

            foreach (PropertyInfo pi in (typeof(HPTHorse)).GetProperties())
            {
                foreach (object o in pi.GetCustomAttributes(true))
                {
                    if (o.GetType() == typeof(HorseRankAttribute))
                    {
                        HorseRankAttribute hra = (HorseRankAttribute)o;
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
                            string s = exc.Message;
                        }
                    }
                }
            }
            return variableList;
            //return variableList.OrderBy(rv => rv.Order).ToList();
        }

        private bool show;
        [DataMember]
        public bool Show
        {
            get
            {
                return show;
            }
            set
            {
                if (show == value)
                {
                    return;
                }
                show = value;
                OnPropertyChanged("Show");
            }
        }

        private string propertyName;
        [DataMember]
        public virtual string PropertyName
        {
            get
            {
                return propertyName;
            }
            set
            {
                if (value == "MarksPercent" || value == "MarksQuantity" || string.IsNullOrEmpty(value))
                {
                    return;
                }
                propertyName = value;

                try
                {
                    var pi = (typeof(HPTHorse)).GetProperty(propertyName);
                    var hra = (HorseRankAttribute)pi.GetCustomAttribute(typeof(HorseRankAttribute));
                    Text = hra.Name;
                    CategoryText = EnumHelper.GetTextFromRankCategory(hra.Category);
                }
                catch (Exception exc)
                {
                    string s = exc.Message;
                }
            }
        }

        [XmlIgnore]
        public string CategoryText { get; set; }

        [XmlIgnore]
        public string Text { get; set; }

        public override string ToString()
        {
            return Text + " (" + CategoryText + ")";
        }
    }

    [DataContract]
    public class HPTHorseRank : Notifier
    {
        private int rank;
        [DataMember]
        public int Rank
        {
            get
            {
                return rank;
            }
            set
            {
                if (rank != value)
                {
                    rank = value;
                    OnPropertyChanged("Rank");
                }
            }
        }

        private decimal rankWeighted;
        [DataMember]
        public decimal RankWeighted
        {
            get
            {
                return rankWeighted;
            }
            set
            {
                if (rankWeighted != value)
                {
                    rankWeighted = value;
                    OnPropertyChanged("RankWeighted");
                }
            }
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

        private SolidColorBrush backColor;
        [XmlIgnore]
        public SolidColorBrush BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
                OnPropertyChanged("BackColor");
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
            HPTHorseRankVariable rankVariable = new HPTHorseRankVariable();

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
            List<HPTHorseRankVariable> variableList = new List<HPTHorseRankVariable>();

            foreach (PropertyInfo pi in (typeof(HPTHorse)).GetProperties())
            {
                foreach (object o in pi.GetCustomAttributes(true))
                {
                    if (o.GetType() == typeof(HorseRankAttribute))
                    {
                        HorseRankAttribute hra = (HorseRankAttribute)o;
                        HPTHorseRankVariable variable = new HPTHorseRankVariable();
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
                            string s = exc.Message;
                        }
                    }
                }
            }
            return variableList.OrderBy(rv => rv.Order).ToList();
        }

        #endregion

        #region Properties

        private string propertyName;
        [DataMember]
        public override string PropertyName
        {
            get
            {
                return propertyName;
            }
            set
            {
                if (value == "MarksPercent" || value == "MarksQuantity" || string.IsNullOrEmpty(value))
                {
                    return;
                }

                propertyName = value;
                if (propertyName == "HistoryRelativeDifference")
                {
                    propertyName = "HistoryRelativeDifferenceUnadjusted";
                }

                try
                {
                    if (horseRankInfo == null || HorseProperty == null)
                    {
                        HorseProperty = typeof(HPTHorse).GetProperty(propertyName);
                        if (HorseProperty == null)
                        {
                            return;
                        }

                        object[] attributeArray = HorseProperty.GetCustomAttributes(typeof(HorseRankAttribute), true);
                        if (attributeArray == null || attributeArray.Length == 0)
                        {
                            propertyName = null;
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
                    string s = exc.Message;
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
                if (use == value)
                {
                    return;
                }
                use = value;
                OnPropertyChanged("Use");
            }
        }

        private decimal weight;
        [DataMember]
        public decimal Weight
        {
            get
            {
                return weight;
            }
            set
            {
                weight = value;
                OnPropertyChanged("Weight");
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

                int legNr = horseList[0].ParentRace.LegNr;

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
                int sortRank = 0;
                for (int i = 0; i < horseList.Count; i++)
                {
                    int tempRank = i;
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
                            decimal previousRankValue = Convert.ToDecimal(HorseProperty.GetValue(previousHorse, null));
                            decimal rankValue = Convert.ToDecimal(HorseProperty.GetValue(horse, null));
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
                    Color c = Colors.LightGray;
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
                        List<HPTHorseRank> raceHorseRankList = horseList.OrderBy(h => h.StartNr).SelectMany(h => h.RankList.Where(hrv => hrv.Name == PropertyName)).ToList();
                        raceHorseRank.HorseRankList = new ObservableCollection<HPTHorseRank>(raceHorseRankList);
                    }
                }
                catch (Exception exc)
                {
                    string s = exc.Message;
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

                decimal exactDiff = Convert.ToDecimal(HorseProperty.GetValue(h1, null)) - Convert.ToDecimal(HorseProperty.GetValue(h2, null));
                if (exactDiff == 0M)
                {
                    return 0;
                }

                int diff = exactDiff > 0M ? 1 : -1;
                return Descending ? diff * -1 : diff;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
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
                OnPropertyChanged("HorseRankInfo");
            }
        }

        #region Hanteringen av översiktsvy för Rankvariabler

        private ObservableCollection<RaceHorseRank> raceHorseRankList;
        [XmlIgnore]
        public ObservableCollection<RaceHorseRank> RaceHorseRankList
        {
            get
            {
                return raceHorseRankList;
            }
            set
            {
                raceHorseRankList = value;
                OnPropertyChanged("RaceHorseRankList");
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

        private int raceNumber;
        [DataMember]
        public int RaceNumber
        {
            get
            {
                return raceNumber;
            }
            set
            {
                raceNumber = value;
                OnPropertyChanged("RaceNumber");
            }
        }

        private ObservableCollection<HPTHorseRank> horseRankList;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTHorseRank> HorseRankList
        {
            get
            {
                return horseRankList;
            }
            set
            {
                horseRankList = value;
                OnPropertyChanged("HorseRankList");
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
