using System.Reflection;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTDataToShow : Notifier
    {
        public void Clone(HPTDataToShow dataToShow)
        {
            foreach (var pi in (dataToShow.GetType()).GetProperties())
            {
                if (pi.PropertyType == typeof(bool))
                {
                    pi.SetValue(dataToShow, pi.GetValue(this, null), null);
                }
            }
        }

        [field: DataMember]
        public DataToShowUsage Usage
        {
            get { return field; }
            set;
        }

        [DataMember]
        public bool EnableConfiguration { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }

        public List<HorseDataToShowAttribute> GetHorseDataToShowAttributes()
        {
            var attributeList = new List<HorseDataToShowAttribute>();
            foreach (var pi in (GetType()).GetProperties())
            {
                foreach (var o in pi.GetCustomAttributes(true))
                {
                    if (o.GetType() == typeof(HorseDataToShowAttribute))
                    {
                        var hda = (HorseDataToShowAttribute)o;
                        //if (hda.PropertyName == "ShowReserv")
                        //{
                        //    string s = string.Empty;
                        //}
                        if (hda.Usage.HasFlag(Usage) || hda.Usage == DataToShowUsage.Everywhere)
                        {
                            attributeList.Add(hda);
                        }
                    }
                }
            }
            return attributeList;
        }

        [DataMember]
        public List<string> ColumnsInOrder { get; set; }

        [DataMember]
        public GUIProfile GUIProfile
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = GUIProfile.Normal;
    }
}
