using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTDataToShow : Notifier
    {
        public void Clone(HPTDataToShow dataToShow)
        {
            foreach (PropertyInfo pi in (dataToShow.GetType()).GetProperties())
            {
                if (pi.PropertyType == typeof(bool))
                {
                    pi.SetValue(dataToShow, pi.GetValue(this, null), null);
                }
            }
        }

        [DataMember]
        private DataToShowUsage usage;
        public DataToShowUsage Usage
        {
            get
            {
                return this.usage;
            }
            set
            {
                this.usage = value;
            }
        }

        [DataMember]
        public bool EnableConfiguration { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }

        public List<HorseDataToShowAttribute> GetHorseDataToShowAttributes()
        {
            List<HorseDataToShowAttribute> attributeList = new List<HorseDataToShowAttribute>();
            foreach (PropertyInfo pi in (this.GetType()).GetProperties())
            {
                foreach (object o in pi.GetCustomAttributes(true))
                {
                    if (o.GetType() == typeof(HorseDataToShowAttribute))
                    {
                        HorseDataToShowAttribute hda = (HorseDataToShowAttribute)o;
                        //if (hda.PropertyName == "ShowReserv")
                        //{
                        //    string s = string.Empty;
                        //}
                        if (hda.Usage.HasFlag(this.Usage) || hda.Usage == DataToShowUsage.Everywhere)
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

        private GUIProfile guiProfile = GUIProfile.Normal;
        [DataMember]
        public GUIProfile GUIProfile
        {
            get
            {
                return guiProfile;
            }
            set
            {
                guiProfile = value;
                OnPropertyChanged("GUIProfile");
            }
        }

    }
}
