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
            List<HPTHorseVariable> variableList = new List<HPTHorseVariable>();

            foreach (PropertyInfo pi in (typeof(HPTHorse)).GetProperties())
            {
                foreach (object o in pi.GetCustomAttributes(true))
                {
                    if (o.GetType() == typeof(GroupReductionAttribute))
                    {
                        GroupReductionAttribute gra = (GroupReductionAttribute)o;
                        HPTHorseVariable variable = new HPTHorseVariable();
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
                OnPropertyChanged("GroupReductionInfo");
            }
        }

        [XmlIgnore]
        public PropertyInfo HorseProperty { get; set; }

        private string propertyName;
        public string PropertyName
        {
            get
            {
                return propertyName;
            }
            set
            {
                propertyName = value;
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
