using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                return this.show;
            }
            set
            {
                if (this.show == value)
                {
                    return;
                }
                this.show = value;
                OnPropertyChanged("Show");
            }
        }

        private string propertyName;
        [DataMember]
        public virtual string PropertyName
        {
            get
            {
                return this.propertyName;
            }
            set
            {
                if (value == "MarksPercent" || value == "MarksQuantity" || string.IsNullOrEmpty(value))
                {
                    return;
                }
                this.propertyName = value;

                try
                {
                    var pi = (typeof(HPTHorse)).GetProperty(this.propertyName);
                    var hra = (HorseRankAttribute)pi.GetCustomAttribute(typeof(HorseRankAttribute));
                    this.Text = hra.Name;
                    this.CategoryText = EnumHelper.GetTextFromRankCategory(hra.Category);
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
            return this.Text + " (" + this.CategoryText + ")";
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
                return this.rank;
            }
            set
            {
                if (this.rank != value)
                {
                    this.rank = value;
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
                return this.rankWeighted;
            }
            set
            {
                if (this.rankWeighted != value)
                {
                    this.rankWeighted = value;
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
                return this.name;
            }
            set
            {
                this.name = value;
                OnPropertyChanged("Name");
            }
        }

        private SolidColorBrush backColor;
        [XmlIgnore]
        public SolidColorBrush BackColor
        {
            get
            {
                return this.backColor;
            }
            set
            {
                this.backColor = value;
                OnPropertyChanged("BackColor");
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string RankValueString { get; set; }

        public bool Use { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    [DataContract]
    public class HPTHorseRankVariable : HPTHorseRankVariableBase
    {
        public HPTHorseRankVariable()
        {
            this.RaceHorseRankList = new ObservableCollection<RaceHorseRank>();
        }

        [OnDeserialized]
        public void InitializeOnDeserialized(StreamingContext sc)
        {
            this.RaceHorseRankList = new ObservableCollection<RaceHorseRank>();
        }

        public HPTHorseRankVariable Clone()
        {
            HPTHorseRankVariable rankVariable = new HPTHorseRankVariable();

            rankVariable.Calculated = this.Calculated;
            rankVariable.Category = this.Category;
            rankVariable.CategoryText = this.CategoryText;
            rankVariable.Descending = this.Descending;
            rankVariable.HorseProperty = this.HorseProperty;
            rankVariable.HorseRankInfo = this.HorseRankInfo;
            rankVariable.IsStatic = this.IsStatic;
            rankVariable.RaceHorseRankList = this.RaceHorseRankList;
            rankVariable.Sort = this.Sort;
            rankVariable.PropertyName = this.PropertyName;
            rankVariable.Text = this.Text;
            rankVariable.Use = this.Use;
            rankVariable.Weight = this.Weight;

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
                return this.propertyName;
            }
            set
            {
                if (value == "MarksPercent" || value == "MarksQuantity" || string.IsNullOrEmpty(value))
                {
                    return;
                }

                this.propertyName = value;
                if (this.propertyName == "HistoryRelativeDifference")
                {
                    this.propertyName = "HistoryRelativeDifferenceUnadjusted";
                }

                try
                {
                    if (this.horseRankInfo == null || this.HorseProperty == null)
                    {
                        this.HorseProperty = typeof(HPTHorse).GetProperty(this.propertyName);
                        if (this.HorseProperty == null)
                        {
                            return;
                        }

                        object[] attributeArray = this.HorseProperty.GetCustomAttributes(typeof(HorseRankAttribute), true);
                        if (attributeArray == null || attributeArray.Length == 0)
                        {
                            this.propertyName = null;
                            return;
                        }
                        this.HorseRankInfo = (HorseRankAttribute)attributeArray[0];

                        this.IsStatic = this.HorseRankInfo.IsStatic;
                        this.Descending = this.HorseRankInfo.Descending;
                        this.Category = this.HorseRankInfo.Category;
                        this.CategoryText = EnumHelper.GetTextFromRankCategory(this.Category);
                        this.Text = this.HorseRankInfo.Name;
                        this.Sort = this.HorseRankInfo.Sort;
                        this.DisplayPropertyName = this.HorseRankInfo.DisplayPropertyName;
                        this.ValueForMissing = this.HorseRankInfo.ValueForMissing;
                        this.StringFormat = this.HorseRankInfo.StringFormat;
                        this.Order = this.HorseRankInfo.Order;
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
                return this.use;
            }
            set
            {
                if (this.use == value)
                {
                    return;
                }
                this.use = value;
                OnPropertyChanged("Use");
            }
        }

        private decimal weight;
        [DataMember]
        public decimal Weight
        {
            get
            {
                return this.weight;
            }
            set
            {
                this.weight = value;
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
                if (this.HorseProperty == null)
                {
                    this.HorseProperty = (typeof(HPTHorse)).GetProperty(this.PropertyName);
                }

                int legNr = horseList[0].ParentRace.LegNr;

                var raceHorseRank = this.RaceHorseRankList.FirstOrDefault(hr => hr.RaceNumber == legNr);
                if (raceHorseRank == null)
                {
                    raceHorseRank = new RaceHorseRank()
                    {
                        RaceNumber = legNr,
                        HorseRankList = new ObservableCollection<HPTHorseRank>()
                    };
                    this.RaceHorseRankList.Add(raceHorseRank);
                }

                // Sortera på vald variabel
                if (this.Sort)
                {
                    horseList.Sort(CompareHorses);
                }

                HPTHorse previousHorse = null;
                int sortRank = 0;
                for (int i = 0; i < horseList.Count; i++)
                {
                    int tempRank = i;
                    var horse = horseList[i];
                    var horseRank = horse.RankList.FirstOrDefault(r => r.Name == this.PropertyName);
                    if (horseRank == null)
                    {
                        return;
                    }
                    if (this.Sort)
                    {
                        if (previousHorse != null)
                        {
                            decimal previousRankValue = Convert.ToDecimal(this.HorseProperty.GetValue(previousHorse, null));
                            decimal rankValue = Convert.ToDecimal(this.HorseProperty.GetValue(horse, null));
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
                        sortRank = Convert.ToInt32(this.HorseProperty.GetValue(horse, null));
                        if (this.PropertyName == "RankABC")
                        {
                            horseRank.Rank = horse.GetRankABC();
                        }
                        else
                        {
                            horseRank.Rank = sortRank == 0 ? horse.ParentRace.HorseList.Count : sortRank;
                        }
                    }
                    horseRank.Use = false;
                    horseRank.RankWeighted = horseRank.Rank * this.Weight;

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
                        List<HPTHorseRank> raceHorseRankList = horseList.OrderBy(h => h.StartNr).SelectMany(h => h.RankList.Where(hrv => hrv.Name == this.PropertyName)).ToList();
                        raceHorseRank.HorseRankList = new ObservableCollection<HPTHorseRank>(raceHorseRankList);
                    }
                }
                catch (Exception exc)
                {
                    string s = exc.Message;
                }
                this.Calculated = true;
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

                decimal exactDiff = Convert.ToDecimal(this.HorseProperty.GetValue(h1, null)) - Convert.ToDecimal(this.HorseProperty.GetValue(h2, null));
                if (exactDiff == 0M)
                {
                    return 0;
                }

                int diff = exactDiff > 0M ? 1 : -1;
                return this.Descending ? diff * -1 : diff;
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
                return this.horseRankInfo;
            }
            set
            {
                this.horseRankInfo = value;
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
                return this.raceHorseRankList;
            }
            set
            {
                this.raceHorseRankList = value;
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
            this.HorseRankList = new ObservableCollection<HPTHorseRank>();
        }

        private int raceNumber;
        [DataMember]
        public int RaceNumber
        {
            get
            {
                return this.raceNumber;
            }
            set
            {
                this.raceNumber = value;
                OnPropertyChanged("RaceNumber");
            }
        }

        private ObservableCollection<HPTHorseRank> horseRankList;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTHorseRank> HorseRankList
        {
            get
            {
                return this.horseRankList;
            }
            set
            {
                this.horseRankList = value;
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
