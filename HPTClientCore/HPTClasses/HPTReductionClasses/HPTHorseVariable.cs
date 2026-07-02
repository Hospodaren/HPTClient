using System.Reflection;
using System.Xml.Serialization;

namespace HPTClient
{
    public class HPTHorseVariable : Notifier
    {
        public static SortedList<string, HPTHorseVariable> SortedVariableList;

        public static List<HPTHorseVariable> CreateVariableList()
        {
            SortedVariableList = new SortedList<string, HPTHorseVariable>();
            var variableList = new List<HPTHorseVariable>();

            foreach (var pi in (typeof(HPTHorse)).GetProperties())
            {
                foreach (var o in pi.GetCustomAttributes(true))
                {
                    if (o.GetType() == typeof(GroupReductionAttribute))
                    {
                        var gra = (GroupReductionAttribute)o;
                        var variable = new HPTHorseVariable();
                        variable.GroupReductionInfo = gra;
                        variable.PropertyName = pi.Name;
                        variableList.Add(variable);
                        SortedVariableList.Add(pi.Name, variable);
                    }
                }
            }
            return variableList;
        }

        private GroupReductionAttribute groupReductionInfo;
        [XmlIgnore]
        public GroupReductionAttribute GroupReductionInfo
        {
            get
            {
                return groupReductionInfo;
            }
            set
            {
                groupReductionInfo = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public PropertyInfo HorseProperty { get; set; }

        public string PropertyName
        {
            get;
            set
            {
                field = value;
                if (groupReductionInfo == null || HorseProperty == null)
                {
                    //this.propertyName = this.HorseProperty.Name;
                    HorseProperty = typeof(HPTHorse).GetProperty(PropertyName);

                    //object[] attributeArray = this.HorseProperty.GetCustomAttributes(typeof(HorseRankAttribute), true);
                    //HorseRankAttribute hra = (HorseRankAttribute)attributeArray[0];

                    //this.IsStatic = hra.IsStatic;
                    //this.Descending = hra.Descending;
                    //this.Category = hra.Category;
                    //this.Text = hra.Name;
                    //this.Sort = hra.Sort;
                }
            }
        }

        public override string ToString()
        {
            return GroupReductionInfo.Name;
        }
    }
}
